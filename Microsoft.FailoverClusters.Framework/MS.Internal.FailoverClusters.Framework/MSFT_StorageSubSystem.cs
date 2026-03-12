using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystem : MSFT_StorageObject
{
	public struct CreateStoragePoolOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StoragePool CreatedStoragePool { get; set; }

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

		public static bool operator ==(CreateStoragePoolOutParameters lhs, CreateStoragePoolOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateStoragePoolOutParameters lhs, CreateStoragePoolOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateVirtualDiskOutParameters
	{
		public uint? ReturnValue { get; set; }

		public ulong? Size { get; set; }

		public MSFT_VirtualDisk CreatedVirtualDisk { get; set; }

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

		public static bool operator ==(CreateVirtualDiskOutParameters lhs, CreateVirtualDiskOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateVirtualDiskOutParameters lhs, CreateVirtualDiskOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateMaskingSetOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_MaskingSet CreatedMaskingSet { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateMaskingSetOutParameters lhs, CreateMaskingSetOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateMaskingSetOutParameters lhs, CreateMaskingSetOutParameters rhs)
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

	public struct CreateReplicationRelationshipOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_ReplicationGroup SourceGroup { get; set; }

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

		public static bool operator ==(CreateReplicationRelationshipOutParameters lhs, CreateReplicationRelationshipOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateReplicationRelationshipOutParameters lhs, CreateReplicationRelationshipOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct DeleteReplicationRelationshipOutParameters
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

		public static bool operator ==(DeleteReplicationRelationshipOutParameters lhs, DeleteReplicationRelationshipOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DeleteReplicationRelationshipOutParameters lhs, DeleteReplicationRelationshipOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateReplicationGroupOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_StorageJob CreatedStorageJob { get; set; }

		public MSFT_ReplicationGroup CreatedReplicationGroup { get; set; }

		public MSFT_StorageExtendedStatus ExtendedStatus { get; set; }

		public override bool Equals(object rhs)
		{
			return base.Equals(rhs);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(CreateReplicationGroupOutParameters lhs, CreateReplicationGroupOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateReplicationGroupOutParameters lhs, CreateReplicationGroupOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct CreateFileServerOutParameters
	{
		public uint? ReturnValue { get; set; }

		public MSFT_FileServer CreatedFileServer { get; set; }

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

		public static bool operator ==(CreateFileServerOutParameters lhs, CreateFileServerOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(CreateFileServerOutParameters lhs, CreateFileServerOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct GetDiagnosticInfoOutParameters
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

		public static bool operator ==(GetDiagnosticInfoOutParameters lhs, GetDiagnosticInfoOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GetDiagnosticInfoOutParameters lhs, GetDiagnosticInfoOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct ClearDiagnosticInfoOutParameters
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

		public static bool operator ==(ClearDiagnosticInfoOutParameters lhs, ClearDiagnosticInfoOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ClearDiagnosticInfoOutParameters lhs, ClearDiagnosticInfoOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct StartDiagnosticLogOutParameters
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

		public static bool operator ==(StartDiagnosticLogOutParameters lhs, StartDiagnosticLogOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(StartDiagnosticLogOutParameters lhs, StartDiagnosticLogOutParameters rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct StopDiagnosticLogOutParameters
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

		public static bool operator ==(StopDiagnosticLogOutParameters lhs, StopDiagnosticLogOutParameters rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(StopDiagnosticLogOutParameters lhs, StopDiagnosticLogOutParameters rhs)
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

	public MSFT_StorageSubSystemMSFT_StorageProviderToStorageSubSystem MSFT_StorageProviderToStorageSubSystem { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToDisk MSFT_StorageSubSystemToDisk { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToFileServer MSFT_StorageSubSystemToFileServer { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToFileShare MSFT_StorageSubSystemToFileShare { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToInitiatorId MSFT_StorageSubSystemToInitiatorId { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToMaskingSet MSFT_StorageSubSystemToMaskingSet { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToOffloadDataTransferSetting MSFT_StorageSubSystemToOffloadDataTransferSetting { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToPartition MSFT_StorageSubSystemToPartition { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToPhysicalDisk MSFT_StorageSubSystemToPhysicalDisk { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicaPeer MSFT_StorageSubSystemToReplicaPeer { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicationCapabilities MSFT_StorageSubSystemToReplicationCapabilities { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicationGroup MSFT_StorageSubSystemToReplicationGroup { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageEnclosure MSFT_StorageSubSystemToStorageEnclosure { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageFaultDomain MSFT_StorageSubSystemToStorageFaultDomain { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageHealth MSFT_StorageSubSystemToStorageHealth { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageNode MSFT_StorageSubSystemToStorageNode { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToStoragePool MSFT_StorageSubSystemToStoragePool { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToTargetPort MSFT_StorageSubSystemToTargetPort { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToTargetPortal MSFT_StorageSubSystemToTargetPortal { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToVirtualDisk MSFT_StorageSubSystemToVirtualDisk { get; private set; }

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToVolume MSFT_StorageSubSystemToVolume { get; private set; }

	public bool? AutomaticClusteringEnabled => (bool?)base.Instance.CimInstanceProperties["AutomaticClusteringEnabled"].Value;

	public string CimServerName => (string)base.Instance.CimInstanceProperties["CimServerName"].Value;

	public ushort? CurrentCacheLevel => (ushort?)base.Instance.CimInstanceProperties["CurrentCacheLevel"].Value;

	public ushort? DataTieringType => (ushort?)base.Instance.CimInstanceProperties["DataTieringType"].Value;

	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public ushort? FaultDomainAwarenessDefault => (ushort?)base.Instance.CimInstanceProperties["FaultDomainAwarenessDefault"].Value;

	public string FirmwareVersion => (string)base.Instance.CimInstanceProperties["FirmwareVersion"].Value;

	public string FriendlyName => (string)base.Instance.CimInstanceProperties["FriendlyName"].Value;

	public ushort? HealthStatus => (ushort?)base.Instance.CimInstanceProperties["HealthStatus"].Value;

	public ushort? iSCSITargetCreationScheme => (ushort?)base.Instance.CimInstanceProperties["iSCSITargetCreationScheme"].Value;

	public string Manufacturer => (string)base.Instance.CimInstanceProperties["Manufacturer"].Value;

	public bool? MaskingClientSelectableDeviceNumbers => (bool?)base.Instance.CimInstanceProperties["MaskingClientSelectableDeviceNumbers"].Value;

	public ushort? MaskingMapCountMax => (ushort?)base.Instance.CimInstanceProperties["MaskingMapCountMax"].Value;

	public bool? MaskingOneInitiatorIdPerView => (bool?)base.Instance.CimInstanceProperties["MaskingOneInitiatorIdPerView"].Value;

	public string[] MaskingOtherValidInitiatorIdTypes => (string[])base.Instance.CimInstanceProperties["MaskingOtherValidInitiatorIdTypes"].Value;

	public ushort? MaskingPortsPerView => (ushort?)base.Instance.CimInstanceProperties["MaskingPortsPerView"].Value;

	public ushort[] MaskingValidInitiatorIdTypes => (ushort[])base.Instance.CimInstanceProperties["MaskingValidInitiatorIdTypes"].Value;

	public string Model => (string)base.Instance.CimInstanceProperties["Model"].Value;

	public string Name => (string)base.Instance.CimInstanceProperties["Name"].Value;

	public ushort? NameFormat => (ushort?)base.Instance.CimInstanceProperties["NameFormat"].Value;

	public uint? NumberOfSlots => (uint?)base.Instance.CimInstanceProperties["NumberOfSlots"].Value;

	public ushort[] OperationalStatus => (ushort[])base.Instance.CimInstanceProperties["OperationalStatus"].Value;

	public string[] OtherHostTypeDescription => (string[])base.Instance.CimInstanceProperties["OtherHostTypeDescription"].Value;

	public string[] OtherIdentifyingInfo => (string[])base.Instance.CimInstanceProperties["OtherIdentifyingInfo"].Value;

	public string[] OtherIdentifyingInfoDescription => (string[])base.Instance.CimInstanceProperties["OtherIdentifyingInfoDescription"].Value;

	public string OtherOperationalStatusDescription => (string)base.Instance.CimInstanceProperties["OtherOperationalStatusDescription"].Value;

	public ushort? PhysicalDisksPerStoragePoolMin => (ushort?)base.Instance.CimInstanceProperties["PhysicalDisksPerStoragePoolMin"].Value;

	public ushort? ReplicasPerSourceCloneMax => (ushort?)base.Instance.CimInstanceProperties["ReplicasPerSourceCloneMax"].Value;

	public ushort? ReplicasPerSourceMirrorMax => (ushort?)base.Instance.CimInstanceProperties["ReplicasPerSourceMirrorMax"].Value;

	public ushort? ReplicasPerSourceSnapshotMax => (ushort?)base.Instance.CimInstanceProperties["ReplicasPerSourceSnapshotMax"].Value;

	public string SerialNumber => (string)base.Instance.CimInstanceProperties["SerialNumber"].Value;

	public ushort? StorageConnectionType => (ushort?)base.Instance.CimInstanceProperties["StorageConnectionType"].Value;

	public ushort[] SupportedDeduplicationFileSystemTypes => (ushort[])base.Instance.CimInstanceProperties["SupportedDeduplicationFileSystemTypes"].Value;

	public ushort[] SupportedDeduplicationObjectTypes => (ushort[])base.Instance.CimInstanceProperties["SupportedDeduplicationObjectTypes"].Value;

	public ushort[] SupportedFileServerProtocols => (ushort[])base.Instance.CimInstanceProperties["SupportedFileServerProtocols"].Value;

	public ushort[] SupportedFileSystems => (ushort[])base.Instance.CimInstanceProperties["SupportedFileSystems"].Value;

	public ushort[] SupportedHostType => (ushort[])base.Instance.CimInstanceProperties["SupportedHostType"].Value;

	public bool? SupportsAutomaticStoragePoolSelection => (bool?)base.Instance.CimInstanceProperties["SupportsAutomaticStoragePoolSelection"].Value;

	public bool? SupportsCloneLocal => (bool?)base.Instance.CimInstanceProperties["SupportsCloneLocal"].Value;

	public bool? SupportsCloneRemote => (bool?)base.Instance.CimInstanceProperties["SupportsCloneRemote"].Value;

	public bool? SupportsContinuouslyAvailableFileServer => (bool?)base.Instance.CimInstanceProperties["SupportsContinuouslyAvailableFileServer"].Value;

	public bool? SupportsFileServer => (bool?)base.Instance.CimInstanceProperties["SupportsFileServer"].Value;

	public bool? SupportsFileServerCreation => (bool?)base.Instance.CimInstanceProperties["SupportsFileServerCreation"].Value;

	public bool? SupportsMaskingVirtualDiskToHosts => (bool?)base.Instance.CimInstanceProperties["SupportsMaskingVirtualDiskToHosts"].Value;

	public bool? SupportsMirrorLocal => (bool?)base.Instance.CimInstanceProperties["SupportsMirrorLocal"].Value;

	public bool? SupportsMirrorRemote => (bool?)base.Instance.CimInstanceProperties["SupportsMirrorRemote"].Value;

	public bool? SupportsMultipleResiliencySettingsPerStoragePool => (bool?)base.Instance.CimInstanceProperties["SupportsMultipleResiliencySettingsPerStoragePool"].Value;

	public bool? SupportsSnapshotLocal => (bool?)base.Instance.CimInstanceProperties["SupportsSnapshotLocal"].Value;

	public bool? SupportsSnapshotRemote => (bool?)base.Instance.CimInstanceProperties["SupportsSnapshotRemote"].Value;

	public bool? SupportsStoragePoolAddPhysicalDisk => (bool?)base.Instance.CimInstanceProperties["SupportsStoragePoolAddPhysicalDisk"].Value;

	public bool? SupportsStoragePoolCreation => (bool?)base.Instance.CimInstanceProperties["SupportsStoragePoolCreation"].Value;

	public bool? SupportsStoragePoolDeletion => (bool?)base.Instance.CimInstanceProperties["SupportsStoragePoolDeletion"].Value;

	public bool? SupportsStoragePoolFriendlyNameModification => (bool?)base.Instance.CimInstanceProperties["SupportsStoragePoolFriendlyNameModification"].Value;

	public bool? SupportsStoragePoolRemovePhysicalDisk => (bool?)base.Instance.CimInstanceProperties["SupportsStoragePoolRemovePhysicalDisk"].Value;

	public bool? SupportsStorageTierCreation => (bool?)base.Instance.CimInstanceProperties["SupportsStorageTierCreation"].Value;

	public bool? SupportsStorageTierDeletion => (bool?)base.Instance.CimInstanceProperties["SupportsStorageTierDeletion"].Value;

	public bool? SupportsStorageTieredVirtualDiskCreation => (bool?)base.Instance.CimInstanceProperties["SupportsStorageTieredVirtualDiskCreation"].Value;

	public bool? SupportsStorageTierFriendlyNameModification => (bool?)base.Instance.CimInstanceProperties["SupportsStorageTierFriendlyNameModification"].Value;

	public bool? SupportsStorageTierResize => (bool?)base.Instance.CimInstanceProperties["SupportsStorageTierResize"].Value;

	public bool? SupportsVirtualDiskCapacityExpansion => (bool?)base.Instance.CimInstanceProperties["SupportsVirtualDiskCapacityExpansion"].Value;

	public bool? SupportsVirtualDiskCapacityReduction => (bool?)base.Instance.CimInstanceProperties["SupportsVirtualDiskCapacityReduction"].Value;

	public bool? SupportsVirtualDiskCreation => (bool?)base.Instance.CimInstanceProperties["SupportsVirtualDiskCreation"].Value;

	public bool? SupportsVirtualDiskDeletion => (bool?)base.Instance.CimInstanceProperties["SupportsVirtualDiskDeletion"].Value;

	public bool? SupportsVirtualDiskModification => (bool?)base.Instance.CimInstanceProperties["SupportsVirtualDiskModification"].Value;

	public bool? SupportsVirtualDiskRepair => (bool?)base.Instance.CimInstanceProperties["SupportsVirtualDiskRepair"].Value;

	public bool? SupportsVolumeCreation => (bool?)base.Instance.CimInstanceProperties["SupportsVolumeCreation"].Value;

	public string Tag => (string)base.Instance.CimInstanceProperties["Tag"].Value;

	public MSFT_StorageSubSystem()
	{
	}

	public MSFT_StorageSubSystem(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
		MSFT_StorageProviderToStorageSubSystem = new MSFT_StorageSubSystemMSFT_StorageProviderToStorageSubSystem(session, instance);
		MSFT_StorageSubSystemToDisk = new MSFT_StorageSubSystemMSFT_StorageSubSystemToDisk(session, instance);
		MSFT_StorageSubSystemToFileServer = new MSFT_StorageSubSystemMSFT_StorageSubSystemToFileServer(session, instance);
		MSFT_StorageSubSystemToFileShare = new MSFT_StorageSubSystemMSFT_StorageSubSystemToFileShare(session, instance);
		MSFT_StorageSubSystemToInitiatorId = new MSFT_StorageSubSystemMSFT_StorageSubSystemToInitiatorId(session, instance);
		MSFT_StorageSubSystemToMaskingSet = new MSFT_StorageSubSystemMSFT_StorageSubSystemToMaskingSet(session, instance);
		MSFT_StorageSubSystemToOffloadDataTransferSetting = new MSFT_StorageSubSystemMSFT_StorageSubSystemToOffloadDataTransferSetting(session, instance);
		MSFT_StorageSubSystemToPartition = new MSFT_StorageSubSystemMSFT_StorageSubSystemToPartition(session, instance);
		MSFT_StorageSubSystemToPhysicalDisk = new MSFT_StorageSubSystemMSFT_StorageSubSystemToPhysicalDisk(session, instance);
		MSFT_StorageSubSystemToReplicaPeer = new MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicaPeer(session, instance);
		MSFT_StorageSubSystemToReplicationCapabilities = new MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicationCapabilities(session, instance);
		MSFT_StorageSubSystemToReplicationGroup = new MSFT_StorageSubSystemMSFT_StorageSubSystemToReplicationGroup(session, instance);
		MSFT_StorageSubSystemToStorageEnclosure = new MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageEnclosure(session, instance);
		MSFT_StorageSubSystemToStorageFaultDomain = new MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageFaultDomain(session, instance);
		MSFT_StorageSubSystemToStorageHealth = new MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageHealth(session, instance);
		MSFT_StorageSubSystemToStorageNode = new MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageNode(session, instance);
		MSFT_StorageSubSystemToStoragePool = new MSFT_StorageSubSystemMSFT_StorageSubSystemToStoragePool(session, instance);
		MSFT_StorageSubSystemToTargetPort = new MSFT_StorageSubSystemMSFT_StorageSubSystemToTargetPort(session, instance);
		MSFT_StorageSubSystemToTargetPortal = new MSFT_StorageSubSystemMSFT_StorageSubSystemToTargetPortal(session, instance);
		MSFT_StorageSubSystemToVirtualDisk = new MSFT_StorageSubSystemMSFT_StorageSubSystemToVirtualDisk(session, instance);
		MSFT_StorageSubSystemToVolume = new MSFT_StorageSubSystemMSFT_StorageSubSystemToVolume(session, instance);
	}

	public static MSFT_StorageSubSystem GetInstance(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageSubSystem", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		CimInstance instance = session.GetInstance("root/microsoft/windows/storage", cimInstance);
		if (instance == null)
		{
			return null;
		}
		return new MSFT_StorageSubSystem(session, instance);
	}

	public static MSFT_StorageSubSystem CreateReference(CimSession session, string ObjectId)
	{
		CimInstance cimInstance = new CimInstance("MSFT_StorageSubSystem", "root/microsoft/windows/storage");
		cimInstance.CimInstanceProperties.Add(CimProperty.Create("ObjectId", ObjectId, CimFlags.Key));
		return new MSFT_StorageSubSystem(session, cimInstance);
	}

	public new static IEnumerable<MSFT_StorageSubSystem> Enumerate(CimSession session)
	{
		return from i in session.EnumerateInstances("root/microsoft/windows/storage", "MSFT_StorageSubSystem")
			select new MSFT_StorageSubSystem(session, i);
	}

	public new static IEnumerable<MSFT_StorageSubSystem> Query(CimSession session, string where = null, string select = "*")
	{
		string text = "SELECT " + select + " FROM MSFT_StorageSubSystem";
		if (where != null)
		{
			text = text + " WHERE " + where;
		}
		return from i in session.QueryInstances("root/microsoft/windows/storage", "WQL", text)
			select new MSFT_StorageSubSystem(session, i);
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

	public CreateStoragePoolOutParameters CreateStoragePool(string FriendlyName, MSFT_PhysicalDisk[] PhysicalDisks, ushort? Usage = null, string OtherUsageDescription = null, string ResiliencySettingNameDefault = null, ushort? ProvisioningTypeDefault = null, ulong? LogicalSectorSizeDefault = null, bool? EnclosureAwareDefault = null, ushort? FaultDomainAwarenessDefault = null, ulong? WriteCacheSizeDefault = null, bool? AutoWriteCacheSize = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
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
		if (OtherUsageDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OtherUsageDescription", OtherUsageDescription, CimType.String, CimFlags.In));
		}
		if (ResiliencySettingNameDefault != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ResiliencySettingNameDefault", ResiliencySettingNameDefault, CimType.String, CimFlags.In));
		}
		if (ProvisioningTypeDefault.HasValue)
		{
			ushort? num2 = ProvisioningTypeDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProvisioningTypeDefault", num2, CimType.UInt16, CimFlags.In));
		}
		if (LogicalSectorSizeDefault.HasValue)
		{
			ulong? num3 = LogicalSectorSizeDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("LogicalSectorSizeDefault", num3, CimType.UInt64, CimFlags.In));
		}
		if (EnclosureAwareDefault.HasValue)
		{
			bool? flag = EnclosureAwareDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("EnclosureAwareDefault", flag, CimType.Boolean, CimFlags.In));
		}
		if (FaultDomainAwarenessDefault.HasValue)
		{
			ushort? num4 = FaultDomainAwarenessDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwarenessDefault", num4, CimType.UInt16, CimFlags.In));
		}
		if (WriteCacheSizeDefault.HasValue)
		{
			ulong? num5 = WriteCacheSizeDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("WriteCacheSizeDefault", num5, CimType.UInt64, CimFlags.In));
		}
		if (AutoWriteCacheSize.HasValue)
		{
			bool? flag2 = AutoWriteCacheSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutoWriteCacheSize", flag2, CimType.Boolean, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag3 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag3, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateStoragePool", cimMethodParametersCollection);
		CreateStoragePoolOutParameters result = default(CreateStoragePoolOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedStoragePool"] != null)
		{
			result.CreatedStoragePool = new MSFT_StoragePool(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedStoragePool"].Value);
		}
		else
		{
			result.CreatedStoragePool = null;
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

	public CreateVirtualDiskOutParameters CreateVirtualDisk(string FriendlyName, ulong? Size, ushort? Usage = null, string OtherUsageDescription = null, bool? UseMaximumSize = null, ushort? NumberOfDataCopies = null, ushort? PhysicalDiskRedundancy = null, ushort? NumberOfColumns = null, ulong? Interleave = null, ushort? ParityLayout = null, bool? RequestNoSinglePointOfFailure = null, bool? IsEnclosureAware = null, ushort? FaultDomainAwareness = null, ushort? ProvisioningType = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (Size.HasValue)
		{
			ulong? num = Size;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Size", num, CimType.UInt64, CimFlags.In));
		}
		if (Usage.HasValue)
		{
			ushort? num2 = Usage;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Usage", num2, CimType.UInt16, CimFlags.In));
		}
		if (OtherUsageDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("OtherUsageDescription", OtherUsageDescription, CimType.String, CimFlags.In));
		}
		if (UseMaximumSize.HasValue)
		{
			bool? flag = UseMaximumSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("UseMaximumSize", flag, CimType.Boolean, CimFlags.In));
		}
		if (NumberOfDataCopies.HasValue)
		{
			ushort? num3 = NumberOfDataCopies;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfDataCopies", num3, CimType.UInt16, CimFlags.In));
		}
		if (PhysicalDiskRedundancy.HasValue)
		{
			ushort? num4 = PhysicalDiskRedundancy;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("PhysicalDiskRedundancy", num4, CimType.UInt16, CimFlags.In));
		}
		if (NumberOfColumns.HasValue)
		{
			ushort? num5 = NumberOfColumns;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("NumberOfColumns", num5, CimType.UInt16, CimFlags.In));
		}
		if (Interleave.HasValue)
		{
			ulong? num6 = Interleave;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Interleave", num6, CimType.UInt64, CimFlags.In));
		}
		if (ParityLayout.HasValue)
		{
			ushort? num7 = ParityLayout;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ParityLayout", num7, CimType.UInt16, CimFlags.In));
		}
		if (RequestNoSinglePointOfFailure.HasValue)
		{
			bool? flag2 = RequestNoSinglePointOfFailure;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RequestNoSinglePointOfFailure", flag2, CimType.Boolean, CimFlags.In));
		}
		if (IsEnclosureAware.HasValue)
		{
			bool? flag3 = IsEnclosureAware;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IsEnclosureAware", flag3, CimType.Boolean, CimFlags.In));
		}
		if (FaultDomainAwareness.HasValue)
		{
			ushort? num8 = FaultDomainAwareness;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwareness", num8, CimType.UInt16, CimFlags.In));
		}
		if (ProvisioningType.HasValue)
		{
			ushort? num9 = ProvisioningType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ProvisioningType", num9, CimType.UInt16, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag4 = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag4, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateVirtualDisk", cimMethodParametersCollection);
		CreateVirtualDiskOutParameters result = default(CreateVirtualDiskOutParameters);
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
		if (cimMethodResult.OutParameters["CreatedVirtualDisk"] != null)
		{
			result.CreatedVirtualDisk = new MSFT_VirtualDisk(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedVirtualDisk"].Value);
		}
		else
		{
			result.CreatedVirtualDisk = null;
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

	public CreateMaskingSetOutParameters CreateMaskingSet(string FriendlyName, string[] VirtualDiskNames = null, ushort[] DeviceAccesses = null, string[] DeviceNumbers = null, string[] TargetPortAddresses = null, string[] InitiatorAddresses = null, ushort? HostType = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (VirtualDiskNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("VirtualDiskNames", VirtualDiskNames, CimType.StringArray, CimFlags.In));
		}
		if (DeviceAccesses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DeviceAccesses", DeviceAccesses, CimType.UInt16Array, CimFlags.In));
		}
		if (DeviceNumbers != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DeviceNumbers", DeviceNumbers, CimType.StringArray, CimFlags.In));
		}
		if (TargetPortAddresses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetPortAddresses", TargetPortAddresses, CimType.StringArray, CimFlags.In));
		}
		if (InitiatorAddresses != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("InitiatorAddresses", InitiatorAddresses, CimType.StringArray, CimFlags.In));
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
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateMaskingSet", cimMethodParametersCollection);
		CreateMaskingSetOutParameters result = default(CreateMaskingSetOutParameters);
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
		if (cimMethodResult.OutParameters["CreatedMaskingSet"] != null)
		{
			result.CreatedMaskingSet = new MSFT_MaskingSet(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedMaskingSet"].Value);
		}
		else
		{
			result.CreatedMaskingSet = null;
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

	public SetAttributesOutParameters SetAttributes(bool? AutomaticClusteringEnabled = null, ushort? FaultDomainAwarenessDefault = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (AutomaticClusteringEnabled.HasValue)
		{
			bool? flag = AutomaticClusteringEnabled;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("AutomaticClusteringEnabled", flag, CimType.Boolean, CimFlags.In));
		}
		if (FaultDomainAwarenessDefault.HasValue)
		{
			ushort? num = FaultDomainAwarenessDefault;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FaultDomainAwarenessDefault", num, CimType.UInt16, CimFlags.In));
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

	public CreateReplicationRelationshipOutParameters CreateReplicationRelationship(ushort? SyncType, string SourceReplicationGroupFriendlyName, MSFT_ReplicationSettings SourceGroupSettings, string TargetReplicationGroupFriendlyName, MSFT_ReplicationSettings TargetGroupSettings, string FriendlyName = null, MSFT_ReplicaPeer TargetStorageSubsystem = null, string SourceReplicationGroupDescription = null, MSFT_StorageObject[] SourceStorageElements = null, string TargetReplicationGroupDescription = null, MSFT_StorageObject[] TargetStorageElements = null, MSFT_StoragePool TargetStoragePool = null, MSFT_StoragePool[] TargetStoragePools = null, uint? RecoveryPointObjective = null, bool? RunAsJob = null, MSFT_ReplicationGroup SourceGroup = null, MSFT_ReplicationGroup TargetGroup = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (SyncType.HasValue)
		{
			ushort? num = SyncType;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SyncType", num, CimType.UInt16, CimFlags.In));
		}
		if (SourceReplicationGroupFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceReplicationGroupFriendlyName", SourceReplicationGroupFriendlyName, CimType.String, CimFlags.In));
		}
		if (SourceGroupSettings != null)
		{
			CimInstance value = SourceGroupSettings?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceGroupSettings", value, CimType.Instance, CimFlags.In));
		}
		if (TargetReplicationGroupFriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetReplicationGroupFriendlyName", TargetReplicationGroupFriendlyName, CimType.String, CimFlags.In));
		}
		if (TargetGroupSettings != null)
		{
			CimInstance value2 = TargetGroupSettings?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetGroupSettings", value2, CimType.Instance, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (TargetStorageSubsystem != null)
		{
			CimInstance value3 = TargetStorageSubsystem?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStorageSubsystem", value3, CimType.Instance, CimFlags.In));
		}
		if (SourceReplicationGroupDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceReplicationGroupDescription", SourceReplicationGroupDescription, CimType.String, CimFlags.In));
		}
		if (SourceStorageElements != null)
		{
			CimInstance[] value4 = SourceStorageElements?.Select((MSFT_StorageObject i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceStorageElements", value4, CimType.InstanceArray, CimFlags.In));
		}
		if (TargetReplicationGroupDescription != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetReplicationGroupDescription", TargetReplicationGroupDescription, CimType.String, CimFlags.In));
		}
		if (TargetStorageElements != null)
		{
			CimInstance[] value5 = TargetStorageElements?.Select((MSFT_StorageObject i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStorageElements", value5, CimType.InstanceArray, CimFlags.In));
		}
		if (TargetStoragePool != null)
		{
			CimInstance value6 = TargetStoragePool?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStoragePool", value6, CimType.Instance, CimFlags.In));
		}
		if (TargetStoragePools != null)
		{
			CimInstance[] value7 = TargetStoragePools?.Select((MSFT_StoragePool i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetStoragePools", value7, CimType.InstanceArray, CimFlags.In));
		}
		if (RecoveryPointObjective.HasValue)
		{
			uint? num2 = RecoveryPointObjective;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RecoveryPointObjective", num2, CimType.UInt32, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		if (SourceGroup != null)
		{
			CimInstance value8 = SourceGroup?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceGroup", value8, CimType.Instance, CimFlags.In));
		}
		if (TargetGroup != null)
		{
			CimInstance value9 = TargetGroup?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetGroup", value9, CimType.Instance, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateReplicationRelationship", cimMethodParametersCollection);
		CreateReplicationRelationshipOutParameters result = default(CreateReplicationRelationshipOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["SourceGroup"] != null)
		{
			result.SourceGroup = new MSFT_ReplicationGroup(base.Session, (CimInstance)cimMethodResult.OutParameters["SourceGroup"].Value);
		}
		else
		{
			result.SourceGroup = null;
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

	public DeleteReplicationRelationshipOutParameters DeleteReplicationRelationship(MSFT_ReplicationGroup SourceReplicationGroup, MSFT_ReplicaPeer TargetGroupReplicaPeer, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (SourceReplicationGroup != null)
		{
			CimInstance value = SourceReplicationGroup?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("SourceReplicationGroup", value, CimType.Instance, CimFlags.In));
		}
		if (TargetGroupReplicaPeer != null)
		{
			CimInstance value2 = TargetGroupReplicaPeer?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TargetGroupReplicaPeer", value2, CimType.Instance, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "DeleteReplicationRelationship", cimMethodParametersCollection);
		DeleteReplicationRelationshipOutParameters result = default(DeleteReplicationRelationshipOutParameters);
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

	public CreateReplicationGroupOutParameters CreateReplicationGroup(string FriendlyName, MSFT_StorageObject[] StorageElements, MSFT_ReplicationSettings ReplicationSettings, string Description = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (StorageElements != null)
		{
			CimInstance[] value = StorageElements?.Select((MSFT_StorageObject i) => i.Instance).ToArray();
			cimMethodParametersCollection.Add(CimMethodParameter.Create("StorageElements", value, CimType.InstanceArray, CimFlags.In));
		}
		if (ReplicationSettings != null)
		{
			CimInstance value2 = ReplicationSettings?.Instance;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ReplicationSettings", value2, CimType.Instance, CimFlags.In));
		}
		if (Description != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Description", Description, CimType.String, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateReplicationGroup", cimMethodParametersCollection);
		CreateReplicationGroupOutParameters result = default(CreateReplicationGroupOutParameters);
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
		if (cimMethodResult.OutParameters["CreatedReplicationGroup"] != null)
		{
			result.CreatedReplicationGroup = new MSFT_ReplicationGroup(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedReplicationGroup"].Value);
		}
		else
		{
			result.CreatedReplicationGroup = null;
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

	public CreateFileServerOutParameters CreateFileServer(ushort[] FileSharingProtocols, string[] HostNames, string FriendlyName = null, bool? RunAsJob = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (FileSharingProtocols != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FileSharingProtocols", FileSharingProtocols, CimType.UInt16Array, CimFlags.In));
		}
		if (HostNames != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("HostNames", HostNames, CimType.StringArray, CimFlags.In));
		}
		if (FriendlyName != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("FriendlyName", FriendlyName, CimType.String, CimFlags.In));
		}
		if (RunAsJob.HasValue)
		{
			bool? flag = RunAsJob;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("RunAsJob", flag, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "CreateFileServer", cimMethodParametersCollection);
		CreateFileServerOutParameters result = default(CreateFileServerOutParameters);
		if (cimMethodResult.ReturnValue != null)
		{
			result.ReturnValue = (uint?)cimMethodResult.ReturnValue.Value;
		}
		else
		{
			result.ReturnValue = null;
		}
		if (cimMethodResult.OutParameters["CreatedFileServer"] != null)
		{
			result.CreatedFileServer = new MSFT_FileServer(base.Session, (CimInstance)cimMethodResult.OutParameters["CreatedFileServer"].Value);
		}
		else
		{
			result.CreatedFileServer = null;
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

	public GetDiagnosticInfoOutParameters GetDiagnosticInfo(string DestinationPath, uint? TimeSpan = null, string ActivityId = null, bool? ExcludeOperationalLog = null, bool? ExcludeDiagnosticLog = null, bool? IncludeLiveDump = null, bool? CopyExistingInfoOnly = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (DestinationPath != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("DestinationPath", DestinationPath, CimType.String, CimFlags.In));
		}
		if (TimeSpan.HasValue)
		{
			uint? num = TimeSpan;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("TimeSpan", num, CimType.UInt32, CimFlags.In));
		}
		if (ActivityId != null)
		{
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ActivityId", ActivityId, CimType.String, CimFlags.In));
		}
		if (ExcludeOperationalLog.HasValue)
		{
			bool? flag = ExcludeOperationalLog;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ExcludeOperationalLog", flag, CimType.Boolean, CimFlags.In));
		}
		if (ExcludeDiagnosticLog.HasValue)
		{
			bool? flag2 = ExcludeDiagnosticLog;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("ExcludeDiagnosticLog", flag2, CimType.Boolean, CimFlags.In));
		}
		if (IncludeLiveDump.HasValue)
		{
			bool? flag3 = IncludeLiveDump;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("IncludeLiveDump", flag3, CimType.Boolean, CimFlags.In));
		}
		if (CopyExistingInfoOnly.HasValue)
		{
			bool? flag4 = CopyExistingInfoOnly;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("CopyExistingInfoOnly", flag4, CimType.Boolean, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "GetDiagnosticInfo", cimMethodParametersCollection);
		GetDiagnosticInfoOutParameters result = default(GetDiagnosticInfoOutParameters);
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

	public ClearDiagnosticInfoOutParameters ClearDiagnosticInfo()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "ClearDiagnosticInfo", methodParameters);
		ClearDiagnosticInfoOutParameters result = default(ClearDiagnosticInfoOutParameters);
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

	public StartDiagnosticLogOutParameters StartDiagnosticLog(ushort? Level = null, ulong? MaxLogSize = null)
	{
		CimMethodParametersCollection cimMethodParametersCollection = new CimMethodParametersCollection();
		if (Level.HasValue)
		{
			ushort? num = Level;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("Level", num, CimType.UInt16, CimFlags.In));
		}
		if (MaxLogSize.HasValue)
		{
			ulong? num2 = MaxLogSize;
			cimMethodParametersCollection.Add(CimMethodParameter.Create("MaxLogSize", num2, CimType.UInt64, CimFlags.In));
		}
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "StartDiagnosticLog", cimMethodParametersCollection);
		StartDiagnosticLogOutParameters result = default(StartDiagnosticLogOutParameters);
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

	public StopDiagnosticLogOutParameters StopDiagnosticLog()
	{
		CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
		CimMethodResult cimMethodResult = base.Session.InvokeMethod(base.Instance, "StopDiagnosticLog", methodParameters);
		StopDiagnosticLogOutParameters result = default(StopDiagnosticLogOutParameters);
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
