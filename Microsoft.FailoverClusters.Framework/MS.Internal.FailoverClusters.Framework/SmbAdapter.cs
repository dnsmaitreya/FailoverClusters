using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.Framework.CustomExceptions;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Common.Reactive.Linq;
using Microsoft.FailoverClusters.UI.Common.Services;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Options;

namespace MS.Internal.FailoverClusters.Framework;

internal class SmbAdapter : FileShareDataSourceBase
{
	private const string RawSmbNamespace = "root\\microsoft\\windows\\smb";

	private const string Win32Namespace = "root\\cimv2";

	private const string WmiClass = "MSFT_SmbShare";

	private const string EventSharePropertyName = "Share";

	private const string SmbSubscriptionQueryFormat = "SELECT * FROM MSFT_SmbShareChangeEvent WHERE Share.ScopeName='{0}'";

	private const string ScopeNamePropertyName = "ScopeName";

	private const string SpecialPropertyName = "Special";

	private const string ContinuouslyAvailablePropertyName = "ContinuouslyAvailable";

	private const string PopulateVolumePropertyPropertyName = "PopulateVolumeProperty";

	private const string EnumerateSharesMethodName = "EnumerateShares";

	private const string DescriptionPropertyName = "Description";

	private const string EventTypePropertyName = "EventType";

	public SmbAdapter()
		: base(FileShareProtocol.Smb)
	{
	}

	public override IObservable<IFileShareDataItem> GetSharesObservable(string nodeFqdn, string scopeName)
	{
		try
		{
			CimSession cimSession = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimSession(nodeFqdn);
			CimObservableErrorType cimObservableErrorType = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).CanConnectToCim(cimSession, "root\\microsoft\\windows\\smb", "MSFT_SmbShare");
			if (cimObservableErrorType != 0)
			{
				return Observable.Return(new FileShareErrorItem(cimObservableErrorType, base.SupportedProtocol, "MSFT_SmbShare", scopeName));
			}
			CimMethodParametersCollection methodParameters = new CimMethodParametersCollection
			{
				CimMethodParameter.Create("PopulateVolumeProperty", false, CimType.Boolean, CimFlags.In),
				CimMethodParameter.Create("ScopeName", scopeName, CimType.String, CimFlags.In)
			};
			return ((IObservable<IFileShareDataItem>)(from result in cimSession.InvokeMethodAsync("root\\microsoft\\windows\\smb", "MSFT_SmbShare", "EnumerateShares", methodParameters, new CimOperationOptions
				{
					EnableMethodResultStreaming = true
				}).OfType<CimMethodStreamedResult>()
				select TransformCimInstance((CimInstance)result.ItemValue, ShareEventType.None, nodeFqdn) into fs
				where string.Compare(fs.Name, "IPC$", StringComparison.OrdinalIgnoreCase) != 0
				select fs)).Catch((Func<CimException, IObservable<IFileShareDataItem>>)delegate(CimException ex)
			{
				ClusterLog.LogException(ex, "SMB share enumeration observable was terminated with an exception");
				return Observable.Return(new FileShareErrorItem(CimObservableErrorType.ExceptionObserved, base.SupportedProtocol, "MSFT_SmbShare", scopeName, ex));
			});
		}
		catch (CimException exception)
		{
			ClusterLog.LogException(exception);
			return Observable.Return(new FileShareErrorItem(CimObservableErrorType.SubscriptionFailure, base.SupportedProtocol, "MSFT_SmbShare", scopeName));
		}
	}

	public override void SetShare(FileShare fileShare)
	{
		using CimSession cimSession = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimSession(fileShare.ServerName);
		CimInstance cimInstance = GetCimInstance(cimSession, fileShare.ServerName, fileShare.Name, fileShare.Path);
		if (cimInstance != null)
		{
			cimInstance.CimInstanceProperties["Description"].Value = fileShare.Remark;
			try
			{
				cimSession.ModifyInstance(cimInstance);
				return;
			}
			catch (CimException exception)
			{
				ClusterLog.LogException(exception, "Failed to update file share");
				return;
			}
		}
	}

	public override IObservable<IFileShareDataItem> GetSubscriptionObservable(string nodeFqdn, string serverName)
	{
		return GetShareEventObservable(nodeFqdn, "root\\microsoft\\windows\\smb", "MSFT_SmbShare", "SELECT * FROM MSFT_SmbShareChangeEvent WHERE Share.ScopeName='{0}'".FormatInvariantCulture(serverName), "Share", "EventType", TransformCimInstance);
	}

	protected override FileShare TransformCimInstance(CimInstance instance, ShareEventType eventType, string connectionFqdn)
	{
		CimKeyedCollection<CimProperty> cimInstanceProperties = instance.CimInstanceProperties;
		FileShare fileShare = new FileShare
		{
			Name = cimInstanceProperties["Name"].Value.ToString(),
			ServerName = cimInstanceProperties["ScopeName"].Value.ToString()
		};
		object cimPropertyValue = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimPropertyValue(cimInstanceProperties, "Path");
		if (cimPropertyValue != null)
		{
			fileShare.Path = cimPropertyValue.ToString();
		}
		object cimPropertyValue2 = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimPropertyValue(cimInstanceProperties, "Description");
		if (cimPropertyValue2 != null)
		{
			fileShare.Remark = cimPropertyValue2.ToString();
		}
		object cimPropertyValue3 = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimPropertyValue(cimInstanceProperties, "Special");
		if (cimPropertyValue3 != null && Convert.ToBoolean(cimPropertyValue3, CultureInfo.InvariantCulture))
		{
			fileShare.ShareInfoType = ShareInfoType.Special;
		}
		object cimPropertyValue4 = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimPropertyValue(cimInstanceProperties, "ContinuouslyAvailable");
		if (cimPropertyValue4 != null)
		{
			fileShare.ContinuousAvailability = Convert.ToBoolean(cimPropertyValue4, CultureInfo.InvariantCulture);
		}
		fileShare.EventType = eventType;
		fileShare.Protocol = FileShareProtocol.Smb;
		fileShare.ConnectionFqdn = connectionFqdn;
		return fileShare;
	}

	public void GrantAccess(string share, string account, SmbAccessRight smbAccess, FileSystemRights fsAccess)
	{
		if (!Regex.IsMatch(share, FileShareAdapter.ValidSharePathRegex, RegexOptions.IgnoreCase))
		{
			throw new InvalidFileSharePathException(share);
		}
		IEnumerable<string> source = from s in share.Trim('\\').Split('\\')
			where s != ""
			select s;
		string text = source.ElementAt(0);
		string text2 = source.ElementAt(1);
		using CimSession cimSession = ServiceContainer.Container.Resolve<ICimUtilities>(Array.Empty<object>()).GetCimSession(text);
		try
		{
			IEnumerable<CimInstance> cimShares = cimSession.QueryInstances("root\\microsoft\\windows\\smb", "WQL", "select * from Msft_SmbShare where Name='{0}'".FormatInvariantCulture(text2));
			Func<string, IEnumerable<CimInstance>> func = (string scope) => cimShares.Where((CimInstance s) => ((string)s.CimInstanceProperties["ScopeName"].Value).ToLower(CultureInfo.InvariantCulture) == scope.ToLower(CultureInfo.InvariantCulture));
			string arg = text.Split('.')[0];
			CimInstance cimInstance = func(arg).FirstOrDefault();
			if (cimInstance == null)
			{
				cimInstance = func("*").FirstOrDefault();
				if (cimInstance == null)
				{
					throw new FileShareNotFoundException(text, text2, share);
				}
			}
			if ((uint)cimInstance.CimInstanceProperties["ShareType"].Value != 0)
			{
				throw new ShareNotAFileSystemShareException(text, text2, share);
			}
			CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccessRight", (uint)smbAccess, CimFlags.In));
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountName", new string[1] { account }, CimFlags.In));
			CimMethodParametersCollection methodParameters = cimMethodParametersCollection;
			cimSession.InvokeMethod(cimInstance, "GrantAccess", methodParameters);
			string text3 = (string)cimInstance.CimInstanceProperties["Path"].Value;
			if (source.Count() > 2)
			{
				string text4 = (text3.EndsWith("\\", StringComparison.OrdinalIgnoreCase) ? "" : "\\");
				text3 = text3 + text4 + string.Join("\\", source.Where((string _, int i) => i >= 2));
			}
			text3 = Regex.Replace(text3, "\\\\", "\\\\");
			CimInstance cimInstance2 = cimSession.QueryInstances("root\\cimv2", "WQL", "select * from Win32_LogicalFileSecuritySetting where Path='{0}'".FormatInvariantCulture(text3)).FirstOrDefault();
			if (cimInstance2 == null)
			{
				throw new UNCLocalPathNotFoundException(share, text);
			}
			CimInstance value = (CimInstance)cimSession.InvokeMethod(cimInstance2, "GetSecurityDescriptor", null).OutParameters["Descriptor"].Value;
			methodParameters = new CimMethodParametersCollection { CimMethodParameter.Create("Descriptor", value, CimFlags.In) };
			string securityDescriptorSddlForm = (string)cimSession.InvokeMethod("root\\cimv2", "Win32_SecurityDescriptorHelper", "Win32SDToSDDL", methodParameters).OutParameters["SDDL"].Value;
			DirectorySecurity directorySecurity = new DirectorySecurity();
			directorySecurity.SetSecurityDescriptorSddlForm(securityDescriptorSddlForm);
			directorySecurity.AddAccessRule(new FileSystemAccessRule(account, fsAccess, AccessControlType.Allow));
			securityDescriptorSddlForm = directorySecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);
			methodParameters = new CimMethodParametersCollection { CimMethodParameter.Create("SDDL", securityDescriptorSddlForm, CimFlags.In) };
			value = (CimInstance)cimSession.InvokeMethod("root\\cimv2", "Win32_SecurityDescriptorHelper", "SDDLToWin32SD", methodParameters).OutParameters["Descriptor"].Value;
			methodParameters = new CimMethodParametersCollection { CimMethodParameter.Create("Descriptor", value, CimFlags.In) };
			cimSession.InvokeMethod(cimInstance2, "SetSecurityDescriptor", methodParameters);
		}
		catch (CimException e)
		{
			throw new SmbShareGrantAccessException(account, share, e);
		}
	}

	protected override CimInstance GetCimInstance(CimSession session, string scopeName, string shareName, string sharePath)
	{
		string query = "select * from Msft_SmbShare where Name='{0}' and ScopeName='{1}'".FormatInvariantCulture(shareName, scopeName);
		return FileShareDataSourceBase.SearchCimInstances(session, "MSFT_SmbShare", "root\\microsoft\\windows\\smb", query).FirstOrDefault();
	}
}
