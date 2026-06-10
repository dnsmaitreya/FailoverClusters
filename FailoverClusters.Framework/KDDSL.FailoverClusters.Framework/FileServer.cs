using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileServer : MSFT_StorageObject
{
	public struct CreateFileShareOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_FileShare CreatedFileShare { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateFileShareOutParameters lhs, CreateFileShareOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateFileShareOutParameters lhs, CreateFileShareOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DeleteObjectOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetFriendlyNameOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(SetFriendlyNameOutParameters lhs, SetFriendlyNameOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetFriendlyNameOutParameters lhs, SetFriendlyNameOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_FileServerMSFT_FileServerToFileShare MSFT_FileServerToFileShare { get; private set; }

	public MSFT_FileServerMSFT_FileServerToVolume MSFT_FileServerToVolume { get; private set; }

	public MSFT_FileServerMSFT_StorageSubSystemToFileServer MSFT_StorageSubSystemToFileServer { get; private set; }

	public ushort[] FileSharingProtocols => (ushort[])base.Instance.CimInstanceProperties["FileSharingProtocols"].Value;

	public string[] FileSharingProtocolVersions => (string[])base.Instance.CimInstanceProperties["FileSharingProtocolVersions"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public string[] HostNames => (string[])base.Instance.CimInstanceProperties["HostNames"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string OtherOperationalStatusDescription => (string)base.Instance.CimInstanceProperties["OtherOperationalStatusDescription"].Value;

	public bool? SupportsContinuouslyAvailableFileShare => (bool?)base.Instance.CimInstanceProperties["SupportsContinuouslyAvailableFileShare"].Value;

	public bool? SupportsFileShareCreation => (bool?)base.Instance.CimInstanceProperties["SupportsFileShareCreation"].Value;

	public MSFT_FileServer()
	{
	}

	public MSFT_FileServer(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_FileServerToFileShare = new MSFT_FileServerMSFT_FileServerToFileShare(session, instance);
		MSFT_FileServerToVolume = new MSFT_FileServerMSFT_FileServerToVolume(session, instance);
		MSFT_StorageSubSystemToFileServer = new MSFT_FileServerMSFT_StorageSubSystemToFileServer(session, instance);
	}

	public static MSFT_FileServer GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_FileServer", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_FileServer(session, instance);
	}

	public static MSFT_FileServer CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_FileServer", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_FileServer(session, cimInstance);
	}

	public new static IEnumerable<MSFT_FileServer> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_FileServer")
			select new MSFT_FileServer(session, i);
	}

	public new static IEnumerable<MSFT_FileServer> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_FileServer";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_FileServer(session, i);
	}

	public override void Refresh()
	{
		base.Instance = base.Session.GetInstance("root/windows/storage", base.Instance);
	}

	public override void Delete()
	{
		base.Session.DeleteInstance("root/windows/storage", base.Instance);
	}

	public override void Save()
	{
		base.Session.ModifyInstance("root/windows/storage", base.Instance);
	}

	public CreateFileShareOutParameters CreateFileShare(string Name, MSFT_Volume SourceVolume, string Description = null, string VolumeRelativePath = null, bool? ContinuouslyAvailable = null, bool? EncryptData = null, ushort? FileSharingProtocol = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Name != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Name", Name, CimType.String, CimFlags.In));
		}
		if (SourceVolume != null)
		{
			CimInstance value = SourceVolume?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceVolume", value, CimType.Instance, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (VolumeRelativePath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VolumeRelativePath", VolumeRelativePath, CimType.String, CimFlags.In));
		}
		if (ContinuouslyAvailable.HasValue)
		{
			bool? flag = ContinuouslyAvailable;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ContinuouslyAvailable", flag, CimType.Boolean, CimFlags.In));
		}
		if (EncryptData.HasValue)
		{
			bool? flag2 = EncryptData;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EncryptData", flag2, CimType.Boolean, CimFlags.In));
		}
		if (FileSharingProtocol.HasValue)
		{
			ushort? num = FileSharingProtocol;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSharingProtocol", num, CimType.UInt16, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag3 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag3, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateFileShare", cimMethodParametersCollection);
		CreateFileShareOutParameters result = default(CreateFileShareOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedFileShare"] != null)
		{
			result.CreatedFileShare = new MSFT_FileShare(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedFileShare"].Value);
		}
		else
		{
			result.CreatedFileShare = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
		}
		if (cimMethodResult.OutParameters["ExtendedStatus"] != null)
		{
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(base.Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}

	public DeleteObjectOutParameters DeleteObject(bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DeleteObject", cimMethodParametersCollection);
		DeleteObjectOutParameters result = default(DeleteObjectOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedStorageJob"] != null)
		{
			result.CreatedStorageJob = new MSFT_StorageJob(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStorageJob"].Value);
		}
		else
		{
			result.CreatedStorageJob = null;
		}
		if (cimMethodResult.OutParameters["ExtendedStatus"] != null)
		{
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(base.Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}

	public SetFriendlyNameOutParameters SetFriendlyName(string FriendlyName)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetFriendlyName", cimMethodParametersCollection);
		SetFriendlyNameOutParameters result = default(SetFriendlyNameOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["ExtendedStatus"] != null)
		{
			result.ExtendedStatus = new MSFT_StorageExtendedStatus(base.Session, (CimInstance)cimMethodResult.OutParameters["ExtendedStatus"].Value);
		}
		else
		{
			result.ExtendedStatus = null;
		}
		return result;
	}
}

