using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterLastErrorException : ClusterDialogException
{
	public string ResourceName { get; private set; }

	public ClusterLastErrorException()
	{
	}

	public ClusterLastErrorException(string message)
		: base(message)
	{
	}

	public ClusterLastErrorException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterLastErrorException(string message, string resourceName, Exception innerException)
		: base(message, innerException)
	{
		ResourceName = resourceName;
		base.Caption = CommonResources.LastErrorException_Caption;
		base.Icon = TaskDialogStandardIcon.Error;
		if (!string.IsNullOrEmpty(message))
		{
			base.Header = message.Replace(Environment.NewLine, "");
		}
		else
		{
			base.Header = ExceptionResources.ClusterLastErrorExceptionNoErrorText_Header;
		}
		base.Content = string.Empty;
	}

	protected ClusterLastErrorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			ResourceName = info.GetString("resourceName");
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info?.AddValue("resourceName", ResourceName);
	}

	public static ClusterLastErrorException Create(int error, string name)
	{
		if (ExceptionHelpers.Succeeded(error))
		{
			return null;
		}
		int num = error;
		if (!ExceptionHelpers.IsHResult(error))
		{
			num = (int)Utilities.Win32ToHResult(error);
		}
		Win32Exception ex = new Win32Exception(num);
		string text = Utilities.FormatError(num);
		if (!text.ToLower(CultureInfo.CurrentCulture).Contains(num.ToString("x", CultureInfo.CurrentCulture)))
		{
			text = text.TrimEnd((Environment.NewLine + ".").ToCharArray()) + ".";
			return new ClusterLastErrorException(text, name, ex);
		}
		return new ClusterLastErrorException(string.Empty, name, ex);
	}

	public static ClusterLastErrorException Create(ulong? clusterError, string name)
	{
		if (clusterError.HasValue)
		{
			ClusterLastErrorException ex = Create(ExceptionHelpers.GetStatusCodeFromClusterError(clusterError.Value), name);
			if (ExceptionHelpers.Information(clusterError.Value))
			{
				ex.Icon = TaskDialogStandardIcon.Information;
			}
			return ex;
		}
		return null;
	}

	public static ClusterLastErrorException Create(uint? statusCode, string name)
	{
		if (statusCode.HasValue)
		{
			return Create((int)statusCode.Value, name);
		}
		return null;
	}
}

