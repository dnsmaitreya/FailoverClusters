using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VirtualDisk : MSFT_StorageObject
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

	public struct ShowOutParameters
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

		public static bool operator ==(ShowOutParameters lhs, ShowOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ShowOutParameters lhs, ShowOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct HideOutParameters
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

		public static bool operator ==(HideOutParameters lhs, HideOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(HideOutParameters lhs, HideOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateSnapshotOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_VirtualDisk CreatedVirtualDisk { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateSnapshotOutParameters lhs, CreateSnapshotOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateSnapshotOutParameters lhs, CreateSnapshotOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateCloneOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_VirtualDisk CreatedVirtualDisk { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateCloneOutParameters lhs, CreateCloneOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateCloneOutParameters lhs, CreateCloneOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ResizeOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ulong? Size { get; set; }

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

		public static bool operator ==(ResizeOutParameters lhs, ResizeOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ResizeOutParameters lhs, ResizeOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RepairOutParameters
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

		public static bool operator ==(RepairOutParameters lhs, RepairOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RepairOutParameters lhs, RepairOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetSecurityDescriptorOutParameters
	{
		public uint? ReturnValue { get; set; }

		public string SecurityDescriptor { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(GetSecurityDescriptorOutParameters lhs, GetSecurityDescriptorOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetSecurityDescriptorOutParameters lhs, GetSecurityDescriptorOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetSecurityDescriptorOutParameters
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

		public static bool operator ==(SetSecurityDescriptorOutParameters lhs, SetSecurityDescriptorOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetSecurityDescriptorOutParameters lhs, SetSecurityDescriptorOutParameters rhs)
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

	public struct SetUsageOutParameters
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

		public static bool operator ==(SetUsageOutParameters lhs, SetUsageOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetUsageOutParameters lhs, SetUsageOutParameters rhs)
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

	public struct AttachOutParameters
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

		public static bool operator ==(AttachOutParameters lhs, AttachOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AttachOutParameters lhs, AttachOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DetachOutParameters
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

		public static bool operator ==(DetachOutParameters lhs, DetachOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DetachOutParameters lhs, DetachOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct AddPhysicalDiskOutParameters
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

		public static bool operator ==(AddPhysicalDiskOutParameters lhs, AddPhysicalDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(AddPhysicalDiskOutParameters lhs, AddPhysicalDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct RemovePhysicalDiskOutParameters
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

		public static bool operator ==(RemovePhysicalDiskOutParameters lhs, RemovePhysicalDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(RemovePhysicalDiskOutParameters lhs, RemovePhysicalDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateReplicaOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_ReplicaPeer CreatedReplicaPeer { get; set; }

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

		public static bool operator ==(CreateReplicaOutParameters lhs, CreateReplicaOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateReplicaOutParameters lhs, CreateReplicaOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct SetReplicationRelationshipOutParameters
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

		public static bool operator ==(SetReplicationRelationshipOutParameters lhs, SetReplicationRelationshipOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(SetReplicationRelationshipOutParameters lhs, SetReplicationRelationshipOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public MSFT_VirtualDiskMSFT_InitiatorIdToVirtualDisk MSFT_InitiatorIdToVirtualDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_MaskingSetToVirtualDisk MSFT_MaskingSetToVirtualDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_ReplicationGroupToVirtualDisk MSFT_ReplicationGroupToVirtualDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_StorageNodeToVirtualDisk MSFT_StorageNodeToVirtualDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_StoragePoolToVirtualDisk MSFT_StoragePoolToVirtualDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_StorageSubSystemToVirtualDisk MSFT_StorageSubSystemToVirtualDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_TargetPortToVirtualDisk MSFT_TargetPortToVirtualDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_VirtualDiskToDisk MSFT_VirtualDiskToDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_VirtualDiskToPhysicalDisk MSFT_VirtualDiskToPhysicalDisk { get; private set; }

	public MSFT_VirtualDiskMSFT_VirtualDiskToReplicaPeer MSFT_VirtualDiskToReplicaPeer { get; private set; }

	public MSFT_VirtualDiskMSFT_VirtualDiskToStorageTier MSFT_VirtualDiskToStorageTier { get; private set; }

	public MSFT_VirtualDiskMSFT_VirtualDiskToVirtualDisk MSFT_VirtualDiskToVirtualDisk { get; private set; }

	public ushort? Access => (ushort?)base.Instance.CimInstanceProperties["Access"].Value;

	public ulong? AllocatedSize => (ulong?)base.Instance.CimInstanceProperties["AllocatedSize"].Value;

	public ulong? AllocationUnitSize => (ulong?)base.Instance.CimInstanceProperties["AllocationUnitSize"].Value;

	public ushort? DetachedReason => (ushort?)base.Instance.CimInstanceProperties["DetachedReason"].Value;

	public ushort? FaultDomainAwareness => (ushort?)base.Instance.CimInstanceProperties["FaultDomainAwareness"].Value;

	public ulong? FootprintOnPool => (ulong?)base.Instance.CimInstanceProperties["FootprintOnPool"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public ulong? Interleave => (ulong?)base.Instance.CimInstanceProperties["Interleave"].Value;

	public bool? IsDeduplicationEnabled => (bool?)base.Instance.CimInstanceProperties["IsDeduplicationEnabled"].Value;

	public bool? IsEnclosureAware => (bool?)base.Instance.CimInstanceProperties["IsEnclosureAware"].Value;

	public bool? IsManualAttach => (bool?)base.Instance.CimInstanceProperties["IsManualAttach"].Value;

	public bool? IsSnapshot => (bool?)base.Instance.CimInstanceProperties["IsSnapshot"].Value;

	public ulong? LogicalSectorSize => (ulong?)base.Instance.CimInstanceProperties["LogicalSectorSize"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort? NameFormat => (ushort?)base.Instance.CimInstanceProperties["NameFormat"].Value;

	public ushort? NumberOfAvailableCopies => (ushort?)base.Instance.CimInstanceProperties["NumberOfAvailableCopies"].Value;

	public ushort? NumberOfColumns => (ushort?)base.Instance.CimInstanceProperties["NumberOfColumns"].Value;

	public ushort? NumberOfDataCopies => (ushort?)base.Instance.CimInstanceProperties["NumberOfDataCopies"].Value;

	public ushort? NumberOfGroups => (ushort?)base.Instance.CimInstanceProperties["NumberOfGroups"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string OtherOperationalStatusDescription => (string)base.Instance.CimInstanceProperties["OtherOperationalStatusDescription"].Value;

	public string OtherUsageDescription => (string)base.Instance.CimInstanceProperties["OtherUsageDescription"].Value;

	public ushort? ParityLayout => (ushort?)base.Instance.CimInstanceProperties["ParityLayout"].Value;

	public ushort? PhysicalDiskRedundancy => (ushort?)base.Instance.CimInstanceProperties["PhysicalDiskRedundancy"].Value;

	public ulong? PhysicalSectorSize => (ulong?)base.Instance.CimInstanceProperties["PhysicalSectorSize"].Value;

	public ushort? ProvisioningType => (ushort?)base.Instance.CimInstanceProperties["ProvisioningType"].Value;

	public ulong? ReadCacheSize
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["ReadCacheSize"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ReadCacheSize"].Value = value;
		}
	}

	public bool? RequestNoSinglePointOfFailure => (bool?)base.Instance.CimInstanceProperties["RequestNoSinglePointOfFailure"].Value;

	public string ResiliencySettingName => (string)base.Instance.CimInstanceProperties["ResiliencySettingName"].Value;

	public ulong? Size => (ulong?)base.Instance.CimInstanceProperties["Size"].Value;

	public ushort? UniqueIdFormat => (ushort?)base.Instance.CimInstanceProperties["UniqueIdFormat"].Value;

	public string UniqueIdFormatDescription => (string)base.Instance.CimInstanceProperties["UniqueIdFormatDescription"].Value;

	public ushort? Usage => (ushort?)base.Instance.CimInstanceProperties["Usage"].Value;

	public ulong? WriteCacheSize
	{
		get
		{
			return (ulong?)base.Instance.CimInstanceProperties["WriteCacheSize"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["WriteCacheSize"].Value = value;
		}
	}

	public MSFT_VirtualDisk()
	{
	}

	public MSFT_VirtualDisk(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_InitiatorIdToVirtualDisk = new MSFT_VirtualDiskMSFT_InitiatorIdToVirtualDisk(session, instance);
		MSFT_MaskingSetToVirtualDisk = new MSFT_VirtualDiskMSFT_MaskingSetToVirtualDisk(session, instance);
		MSFT_ReplicationGroupToVirtualDisk = new MSFT_VirtualDiskMSFT_ReplicationGroupToVirtualDisk(session, instance);
		MSFT_StorageNodeToVirtualDisk = new MSFT_VirtualDiskMSFT_StorageNodeToVirtualDisk(session, instance);
		MSFT_StoragePoolToVirtualDisk = new MSFT_VirtualDiskMSFT_StoragePoolToVirtualDisk(session, instance);
		MSFT_StorageSubSystemToVirtualDisk = new MSFT_VirtualDiskMSFT_StorageSubSystemToVirtualDisk(session, instance);
		MSFT_TargetPortToVirtualDisk = new MSFT_VirtualDiskMSFT_TargetPortToVirtualDisk(session, instance);
		MSFT_VirtualDiskToDisk = new MSFT_VirtualDiskMSFT_VirtualDiskToDisk(session, instance);
		MSFT_VirtualDiskToPhysicalDisk = new MSFT_VirtualDiskMSFT_VirtualDiskToPhysicalDisk(session, instance);
		MSFT_VirtualDiskToReplicaPeer = new MSFT_VirtualDiskMSFT_VirtualDiskToReplicaPeer(session, instance);
		MSFT_VirtualDiskToStorageTier = new MSFT_VirtualDiskMSFT_VirtualDiskToStorageTier(session, instance);
		MSFT_VirtualDiskToVirtualDisk = new MSFT_VirtualDiskMSFT_VirtualDiskToVirtualDisk(session, instance);
	}

	public static MSFT_VirtualDisk GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_VirtualDisk", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_VirtualDisk(session, instance);
	}

	public static MSFT_VirtualDisk CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_VirtualDisk", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_VirtualDisk(session, cimInstance);
	}

	public new static IEnumerable<MSFT_VirtualDisk> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_VirtualDisk")
			select new MSFT_VirtualDisk(session, i);
	}

	public new static IEnumerable<MSFT_VirtualDisk> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_VirtualDisk";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_VirtualDisk(session, i);
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

	public ShowOutParameters Show(string[] TargetPortAddresses, string InitiatorAddress, ushort? HostType = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetPortAddresses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortAddresses", TargetPortAddresses, CimType.StringArray, CimFlags.In));
		}
		if (InitiatorAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorAddress", InitiatorAddress, CimType.String, CimFlags.In));
		}
		if (HostType.HasValue)
		{
			ushort? num = HostType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("HostType", num, CimType.UInt16, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Show", cimMethodParametersCollection);
		ShowOutParameters result = default(ShowOutParameters);
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

	public HideOutParameters Hide(string[] TargetPortAddresses, string InitiatorAddress, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetPortAddresses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortAddresses", TargetPortAddresses, CimType.StringArray, CimFlags.In));
		}
		if (InitiatorAddress != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorAddress", InitiatorAddress, CimType.String, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Hide", cimMethodParametersCollection);
		HideOutParameters result = default(HideOutParameters);
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

	public CreateSnapshotOutParameters CreateSnapshot(string FriendlyName, string TargetStoragePoolName = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (TargetStoragePoolName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStoragePoolName", TargetStoragePoolName, CimType.String, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateSnapshot", cimMethodParametersCollection);
		CreateSnapshotOutParameters result = default(CreateSnapshotOutParameters);
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
		if (cimMethodResult.OutParameters["CreatedVirtualDisk"] != null)
		{
			result.CreatedVirtualDisk = new MSFT_VirtualDisk(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedVirtualDisk"].Value);
		}
		else
		{
			result.CreatedVirtualDisk = null;
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

	public CreateCloneOutParameters CreateClone(string FriendlyName, string TargetStoragePoolName = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (TargetStoragePoolName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStoragePoolName", TargetStoragePoolName, CimType.String, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateClone", cimMethodParametersCollection);
		CreateCloneOutParameters result = default(CreateCloneOutParameters);
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
		if (cimMethodResult.OutParameters["CreatedVirtualDisk"] != null)
		{
			result.CreatedVirtualDisk = new MSFT_VirtualDisk(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedVirtualDisk"].Value);
		}
		else
		{
			result.CreatedVirtualDisk = null;
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

	public ResizeOutParameters Resize(ulong? Size, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Size.HasValue)
		{
			ulong? num = Size;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Size", num, CimType.UInt64, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
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
		if (cimMethodResult.OutParameters["Size"] != null)
		{
			result.Size = (ulong?)cimMethodResult.OutParameters["Size"].Value;
		}
		else
		{
			result.Size = null;
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

	public RepairOutParameters Repair(bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Repair", cimMethodParametersCollection);
		RepairOutParameters result = default(RepairOutParameters);
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

	public GetSecurityDescriptorOutParameters GetSecurityDescriptor()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetSecurityDescriptor", methodParameters);
		GetSecurityDescriptorOutParameters result = default(GetSecurityDescriptorOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SecurityDescriptor"] != null)
		{
			result.SecurityDescriptor = (string)cimMethodResult.OutParameters["SecurityDescriptor"].Value;
		}
		else
		{
			result.SecurityDescriptor = null;
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

	public SetSecurityDescriptorOutParameters SetSecurityDescriptor(string SecurityDescriptor)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (SecurityDescriptor != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SecurityDescriptor", SecurityDescriptor, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetSecurityDescriptor", cimMethodParametersCollection);
		SetSecurityDescriptorOutParameters result = default(SetSecurityDescriptorOutParameters);
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

	public SetUsageOutParameters SetUsage(ushort? Usage, string OtherUsageDescription = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		if (OtherUsageDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OtherUsageDescription", OtherUsageDescription, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetUsage", cimMethodParametersCollection);
		SetUsageOutParameters result = default(SetUsageOutParameters);
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

	public SetAttributesOutParameters SetAttributes(bool? IsManualAttach = null, string StorageNodeName = null, ushort? Access = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (IsManualAttach.HasValue)
		{
			bool? flag = IsManualAttach;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsManualAttach", flag, CimType.Boolean, CimFlags.In));
		}
		if (StorageNodeName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageNodeName", StorageNodeName, CimType.String, CimFlags.In));
		}
		if (Access.HasValue)
		{
			ushort? num = Access;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Access", num, CimType.UInt16, CimFlags.In));
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

	public AttachOutParameters Attach(string StorageNodeName = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (StorageNodeName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageNodeName", StorageNodeName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Attach", cimMethodParametersCollection);
		AttachOutParameters result = default(AttachOutParameters);
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

	public DetachOutParameters Detach(string StorageNodeName = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (StorageNodeName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageNodeName", StorageNodeName, CimType.String, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "Detach", cimMethodParametersCollection);
		DetachOutParameters result = default(DetachOutParameters);
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

	public AddPhysicalDiskOutParameters AddPhysicalDisk(MSFT_PhysicalDisk[] PhysicalDisks, ushort? Usage = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PhysicalDisks != null)
		{
			CimInstance[] value = PhysicalDisks?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisks", value, CimType.InstanceArray, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num, CimType.UInt16, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "AddPhysicalDisk", cimMethodParametersCollection);
		AddPhysicalDiskOutParameters result = default(AddPhysicalDiskOutParameters);
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

	public RemovePhysicalDiskOutParameters RemovePhysicalDisk(MSFT_PhysicalDisk[] PhysicalDisks, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (PhysicalDisks != null)
		{
			CimInstance[] value = PhysicalDisks?.Select((MSFT_PhysicalDisk i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDisks", value, CimType.InstanceArray, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "RemovePhysicalDisk", cimMethodParametersCollection);
		RemovePhysicalDiskOutParameters result = default(RemovePhysicalDiskOutParameters);
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

	public CreateReplicaOutParameters CreateReplica(MSFT_ReplicaPeer TargetStorageSubsystem, MSFT_ReplicationSettings ReplicationSettings, ushort? SyncType, string FriendlyName = null, string TargetVirtualDiskObjectId = null, string TargetStoragePoolObjectId = null, ushort? RecoveryPointObjective = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (TargetStorageSubsystem != null)
		{
			CimInstance value = TargetStorageSubsystem?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStorageSubsystem", value, CimType.Instance, CimFlags.In));
		}
		if (ReplicationSettings != null)
		{
			CimInstance value2 = ReplicationSettings?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationSettings", value2, CimType.Instance, CimFlags.In));
		}
		if (SyncType.HasValue)
		{
			ushort? num = SyncType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SyncType", num, CimType.UInt16, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (TargetVirtualDiskObjectId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetVirtualDiskObjectId", TargetVirtualDiskObjectId, CimType.String, CimFlags.In));
		}
		if (TargetStoragePoolObjectId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStoragePoolObjectId", TargetStoragePoolObjectId, CimType.String, CimFlags.In));
		}
		if (RecoveryPointObjective.HasValue)
		{
			ushort? num2 = RecoveryPointObjective;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RecoveryPointObjective", num2, CimType.UInt16, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateReplica", cimMethodParametersCollection);
		CreateReplicaOutParameters result = default(CreateReplicaOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedReplicaPeer"] != null)
		{
			result.CreatedReplicaPeer = new MSFT_ReplicaPeer(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedReplicaPeer"].Value);
		}
		else
		{
			result.CreatedReplicaPeer = null;
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

	public SetReplicationRelationshipOutParameters SetReplicationRelationship(ushort? Operation, MSFT_ReplicaPeer VirtualDiskReplicaPeer = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Operation.HasValue)
		{
			ushort? num = Operation;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Operation", num, CimType.UInt16, CimFlags.In));
		}
		if (VirtualDiskReplicaPeer != null)
		{
			CimInstance value = VirtualDiskReplicaPeer?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskReplicaPeer", value, CimType.Instance, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "SetReplicationRelationship", cimMethodParametersCollection);
		SetReplicationRelationshipOutParameters result = default(SetReplicationRelationshipOutParameters);
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
}

