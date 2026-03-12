using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_Partition : MSFT_StorageObject
{
	public struct DeleteObjectOutParameters
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

		public static bool operator ==(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DeleteObjectOutParameters lhs, DeleteObjectOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetAccessPathsOutParameters
	{
		public uint? ReturnValue { get; set; }

		public string[] AccessPaths { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetAccessPathsOutParameters lhs, GetAccessPathsOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetAccessPathsOutParameters lhs, GetAccessPathsOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct AddAccessPathOutParameters
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

		public static bool operator ==(AddAccessPathOutParameters lhs, AddAccessPathOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AddAccessPathOutParameters lhs, AddAccessPathOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemoveAccessPathOutParameters
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

		public static bool operator ==(RemoveAccessPathOutParameters lhs, RemoveAccessPathOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemoveAccessPathOutParameters lhs, RemoveAccessPathOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ResizeOutParameters
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

		public static bool operator ==(ResizeOutParameters lhs, ResizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ResizeOutParameters lhs, ResizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSupportedSizeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ulong? SizeMin { get; set; }

		public ulong? SizeMax { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSupportedSizeOutParameters lhs, GetSupportedSizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSupportedSizeOutParameters lhs, GetSupportedSizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct OnlineOutParameters
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

		public static bool operator ==(OnlineOutParameters lhs, OnlineOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(OnlineOutParameters lhs, OnlineOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct OfflineOutParameters
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

		public static bool operator ==(OfflineOutParameters lhs, OfflineOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(OfflineOutParameters lhs, OfflineOutParameters rhs)
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

	public MSFT_PartitionMSFT_DiskToPartition MSFT_DiskToPartition { get; private set; }

	public MSFT_PartitionMSFT_PartitionToReplicaPeer MSFT_PartitionToReplicaPeer { get; private set; }

	public MSFT_PartitionMSFT_PartitionToVolume MSFT_PartitionToVolume { get; private set; }

	public MSFT_PartitionMSFT_ReplicationGroupToPartition MSFT_ReplicationGroupToPartition { get; private set; }

	public MSFT_PartitionMSFT_StorageSubSystemToPartition MSFT_StorageSubSystemToPartition { get; private set; }

	public string[] AccessPaths => (string[])base.Instance.CimInstanceProperties["AccessPaths"].Value;

	public string DiskId => (string)base.Instance.CimInstanceProperties["DiskId"].Value;

	public uint? DiskNumber => (uint?)base.Instance.CimInstanceProperties["DiskNumber"].Value;

	public char? DriveLetter => (char?)base.Instance.CimInstanceProperties["DriveLetter"].Value;

	public string GptType => (string)base.Instance.CimInstanceProperties["GptType"].Value;

	public string Guid => (string)base.Instance.CimInstanceProperties["Guid"].Value;

	public bool? IsActive => (bool?)base.Instance.CimInstanceProperties["IsActive"].Value;

	public bool? IsBoot => (bool?)base.Instance.CimInstanceProperties["IsBoot"].Value;

	public bool? IsDAX => (bool?)base.Instance.CimInstanceProperties["IsDAX"].Value;

	public bool? IsHidden => (bool?)base.Instance.CimInstanceProperties["IsHidden"].Value;

	public bool? IsOffline => (bool?)base.Instance.CimInstanceProperties["IsOffline"].Value;

	public bool? IsReadOnly => (bool?)base.Instance.CimInstanceProperties["IsReadOnly"].Value;

	public bool? IsShadowCopy => (bool?)base.Instance.CimInstanceProperties["IsShadowCopy"].Value;

	public bool? IsSystem => (bool?)base.Instance.CimInstanceProperties["IsSystem"].Value;

	public ushort? MbrType => (ushort?)base.Instance.CimInstanceProperties["MbrType"].Value;

	public bool? NoDefaultDriveLetter => (bool?)base.Instance.CimInstanceProperties["NoDefaultDriveLetter"].Value;

	public ulong? Offset => (ulong?)base.Instance.CimInstanceProperties["Offset"].Value;

	public ushort? OperationalStatus => (ushort?)base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public uint? PartitionNumber => (uint?)base.Instance.CimInstanceProperties["PartitionNumber"].Value;

	public ulong? Size => (ulong?)base.Instance.CimInstanceProperties["Size"].Value;

	public ushort? TransitionState => (ushort?)base.Instance.CimInstanceProperties["TransitionState"].Value;

	public MSFT_Partition()
	{
	}

	public MSFT_Partition(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_DiskToPartition = new MSFT_PartitionMSFT_DiskToPartition(session, instance);
		MSFT_PartitionToReplicaPeer = new MSFT_PartitionMSFT_PartitionToReplicaPeer(session, instance);
		MSFT_PartitionToVolume = new MSFT_PartitionMSFT_PartitionToVolume(session, instance);
		MSFT_ReplicationGroupToPartition = new MSFT_PartitionMSFT_ReplicationGroupToPartition(session, instance);
		MSFT_StorageSubSystemToPartition = new MSFT_PartitionMSFT_StorageSubSystemToPartition(session, instance);
	}

	public static MSFT_Partition GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_Partition", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_Partition(session, instance);
	}

	public static MSFT_Partition CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_Partition", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_Partition(session, cimInstance);
	}

	public new static IEnumerable<MSFT_Partition> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_Partition")
			select new MSFT_Partition(session, i);
	}

	public new static IEnumerable<MSFT_Partition> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_Partition";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_Partition(session, i);
	}

	public override void Refresh()
	{
		base.Instance = base.Session.GetInstance("root/microsoft/windows/storage", base.Instance);
	}

	public override void Delete()
	{
		base.Session.DeleteInstance("root/microsoft/windows/storage", base.Instance);
	}

	public override void Save()
	{
		base.Session.ModifyInstance("root/microsoft/windows/storage", base.Instance);
	}

	public DeleteObjectOutParameters DeleteObject()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DeleteObject", methodParameters);
		DeleteObjectOutParameters result = default(DeleteObjectOutParameters);
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

	public GetAccessPathsOutParameters GetAccessPaths()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetAccessPaths", methodParameters);
		GetAccessPathsOutParameters result = default(GetAccessPathsOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["AccessPaths"] != null)
		{
			result.AccessPaths = (string[])cimMethodResult.OutParameters["AccessPaths"].Value;
		}
		else
		{
			result.AccessPaths = null;
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

	public AddAccessPathOutParameters AddAccessPath(string AccessPath = null, bool? AssignDriveLetter = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (AccessPath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccessPath", AccessPath, CimType.String, CimFlags.In));
		}
		if (AssignDriveLetter.HasValue)
		{
			bool? flag = AssignDriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AssignDriveLetter", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "AddAccessPath", cimMethodParametersCollection);
		AddAccessPathOutParameters result = default(AddAccessPathOutParameters);
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

	public RemoveAccessPathOutParameters RemoveAccessPath(string AccessPath)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (AccessPath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AccessPath", AccessPath, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemoveAccessPath", cimMethodParametersCollection);
		RemoveAccessPathOutParameters result = default(RemoveAccessPathOutParameters);
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

	public ResizeOutParameters Resize(ulong? Size)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Size.HasValue)
		{
			ulong? num = Size;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Size", num, CimType.UInt64, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Resize", cimMethodParametersCollection);
		ResizeOutParameters result = default(ResizeOutParameters);
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

	public GetSupportedSizeOutParameters GetSupportedSize()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSupportedSize", methodParameters);
		GetSupportedSizeOutParameters result = default(GetSupportedSizeOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SizeMin"] != null)
		{
			result.SizeMin = (ulong?)cimMethodResult.OutParameters["SizeMin"].Value;
		}
		else
		{
			result.SizeMin = null;
		}
		if (cimMethodResult.OutParameters["SizeMax"] != null)
		{
			result.SizeMax = (ulong?)cimMethodResult.OutParameters["SizeMax"].Value;
		}
		else
		{
			result.SizeMax = null;
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

	public OnlineOutParameters Online()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Online", methodParameters);
		OnlineOutParameters result = default(OnlineOutParameters);
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

	public OfflineOutParameters Offline()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Offline", methodParameters);
		OfflineOutParameters result = default(OfflineOutParameters);
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

	public SetAttributesOutParameters SetAttributes(bool? IsReadOnly = null, bool? NoDefaultDriveLetter = null, bool? IsActive = null, bool? IsHidden = null, bool? IsDAX = null, ushort? MbrType = null, string GptType = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (IsReadOnly.HasValue)
		{
			bool? flag = IsReadOnly;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsReadOnly", flag, CimType.Boolean, CimFlags.In));
		}
		if (NoDefaultDriveLetter.HasValue)
		{
			bool? flag2 = NoDefaultDriveLetter;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NoDefaultDriveLetter", flag2, CimType.Boolean, CimFlags.In));
		}
		if (IsActive.HasValue)
		{
			bool? flag3 = IsActive;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsActive", flag3, CimType.Boolean, CimFlags.In));
		}
		if (IsHidden.HasValue)
		{
			bool? flag4 = IsHidden;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsHidden", flag4, CimType.Boolean, CimFlags.In));
		}
		if (IsDAX.HasValue)
		{
			bool? flag5 = IsDAX;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsDAX", flag5, CimType.Boolean, CimFlags.In));
		}
		if (MbrType.HasValue)
		{
			ushort? num = MbrType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("MbrType", num, CimType.UInt16, CimFlags.In));
		}
		if (GptType != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("GptType", GptType, CimType.String, CimFlags.In));
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
}
