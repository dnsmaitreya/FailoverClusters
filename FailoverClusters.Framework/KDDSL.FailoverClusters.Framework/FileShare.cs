using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_FileShare : MSFT_StorageObject
{
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

	public struct SetDescriptionOutParameters
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

		public static bool operator ==(SetDescriptionOutParameters lhs, SetDescriptionOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetDescriptionOutParameters lhs, SetDescriptionOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetAttributesOutParameters
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

		public static bool operator ==(SetAttributesOutParameters lhs, SetAttributesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetAttributesOutParameters lhs, SetAttributesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetAccessControlEntriesOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_FileShareAccessControlEntry[] AccessControlEntries { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetAccessControlEntriesOutParameters lhs, GetAccessControlEntriesOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetAccessControlEntriesOutParameters lhs, GetAccessControlEntriesOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GrantAccessOutParameters
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

		public static bool operator ==(GrantAccessOutParameters lhs, GrantAccessOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GrantAccessOutParameters lhs, GrantAccessOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RevokeAccessOutParameters
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

		public static bool operator ==(RevokeAccessOutParameters lhs, RevokeAccessOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RevokeAccessOutParameters lhs, RevokeAccessOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct BlockAccessOutParameters
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

		public static bool operator ==(BlockAccessOutParameters lhs, BlockAccessOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BlockAccessOutParameters lhs, BlockAccessOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct UnblockAccessOutParameters
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

		public static bool operator ==(UnblockAccessOutParameters lhs, UnblockAccessOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(UnblockAccessOutParameters lhs, UnblockAccessOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DiagnoseOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageDiagnoseResult[] DiagnoseResults { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(DiagnoseOutParameters lhs, DiagnoseOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DiagnoseOutParameters lhs, DiagnoseOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetActionsOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_HealthAction[] ActionResults { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetActionsOutParameters lhs, GetActionsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetActionsOutParameters lhs, GetActionsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_FileShareMSFT_FileServerToFileShare MSFT_FileServerToFileShare { get; private set; }

	public MSFT_FileShareMSFT_StorageSubSystemToFileShare MSFT_StorageSubSystemToFileShare { get; private set; }

	public MSFT_FileShareMSFT_VolumeToFileShare MSFT_VolumeToFileShare { get; private set; }

	public bool? ContinuouslyAvailable => (bool?)base.Instance.CimInstanceProperties["ContinuouslyAvailable"].Value;

	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public bool? EncryptData => (bool?)base.Instance.CimInstanceProperties["EncryptData"].Value;

	public ushort? FileSharingProtocol => (ushort?)base.Instance.CimInstanceProperties["FileSharingProtocol"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public ushort? ShareState => (ushort?)base.Instance.CimInstanceProperties["ShareState"].Value;

	public string VolumeRelativePath => (string)base.Instance.CimInstanceProperties["VolumeRelativePath"].Value;

	public MSFT_FileShare()
	{
	}

	public MSFT_FileShare(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_FileServerToFileShare = new MSFT_FileShareMSFT_FileServerToFileShare(session, instance);
		MSFT_StorageSubSystemToFileShare = new MSFT_FileShareMSFT_StorageSubSystemToFileShare(session, instance);
		MSFT_VolumeToFileShare = new MSFT_FileShareMSFT_VolumeToFileShare(session, instance);
	}

	public static MSFT_FileShare GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_FileShare", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_FileShare(session, instance);
	}

	public static MSFT_FileShare CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_FileShare", "root/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_FileShare(session, cimInstance);
	}

	public new static IEnumerable<MSFT_FileShare> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/windows/storage", "MSFT_FileShare")
			select new MSFT_FileShare(session, i);
	}

	public new static IEnumerable<MSFT_FileShare> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_FileShare";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/windows/storage", "WQL", text)
			select new MSFT_FileShare(session, i);
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

	public SetDescriptionOutParameters SetDescription(string Description)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetDescription", cimMethodParametersCollection);
		SetDescriptionOutParameters result = default(SetDescriptionOutParameters);
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

	public SetAttributesOutParameters SetAttributes(bool? EncryptData = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (EncryptData.HasValue)
		{
			bool? flag = EncryptData;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EncryptData", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetAttributes", cimMethodParametersCollection);
		SetAttributesOutParameters result = default(SetAttributesOutParameters);
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

	public GetAccessControlEntriesOutParameters GetAccessControlEntries()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetAccessControlEntries", methodParameters);
		GetAccessControlEntriesOutParameters result = default(GetAccessControlEntriesOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["AccessControlEntries"] != null)
		{
			result.AccessControlEntries = ((cimMethodResult.OutParameters["AccessControlEntries"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["AccessControlEntries"].Value).Select((CimInstance i) => new MSFT_FileShareAccessControlEntry(base.Session, i)).ToArray());
		}
		else
		{
			result.AccessControlEntries = null;
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

	public GrantAccessOutParameters GrantAccess(string[] AccountNames, uint? AccessRight)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (AccountNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountNames", AccountNames, CimType.StringArray, CimFlags.In));
		}
		if (AccessRight.HasValue)
		{
			uint? num = AccessRight;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccessRight", num, CimType.UInt32, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GrantAccess", cimMethodParametersCollection);
		GrantAccessOutParameters result = default(GrantAccessOutParameters);
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

	public RevokeAccessOutParameters RevokeAccess(string[] AccountNames)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (AccountNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountNames", AccountNames, CimType.StringArray, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RevokeAccess", cimMethodParametersCollection);
		RevokeAccessOutParameters result = default(RevokeAccessOutParameters);
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

	public BlockAccessOutParameters BlockAccess(string[] AccountNames)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (AccountNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountNames", AccountNames, CimType.StringArray, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "BlockAccess", cimMethodParametersCollection);
		BlockAccessOutParameters result = default(BlockAccessOutParameters);
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

	public UnblockAccessOutParameters UnblockAccess(string[] AccountNames)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (AccountNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccountNames", AccountNames, CimType.StringArray, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "UnblockAccess", cimMethodParametersCollection);
		UnblockAccessOutParameters result = default(UnblockAccessOutParameters);
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

	public DiagnoseOutParameters Diagnose()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Diagnose", methodParameters);
		DiagnoseOutParameters result = default(DiagnoseOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["DiagnoseResults"] != null)
		{
			result.DiagnoseResults = ((cimMethodResult.OutParameters["DiagnoseResults"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["DiagnoseResults"].Value).Select((CimInstance i) => new MSFT_StorageDiagnoseResult(base.Session, i)).ToArray());
		}
		else
		{
			result.DiagnoseResults = null;
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

	public GetActionsOutParameters GetActions()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetActions", methodParameters);
		GetActionsOutParameters result = default(GetActionsOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["ActionResults"] != null)
		{
			result.ActionResults = ((cimMethodResult.OutParameters["ActionResults"].Value == null) ? null : ((IEnumerable<CimInstance>)cimMethodResult.OutParameters["ActionResults"].Value).Select((CimInstance i) => new MSFT_HealthAction(base.Session, i)).ToArray());
		}
		else
		{
			result.ActionResults = null;
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

