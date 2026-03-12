using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public static class EnumLocalization
{
	public static List<string> ClusterIdentityTypeValues => GetStringValues<ClusterIdentityType>();

	public static List<string> GroupStateValues => GetStringValues<GroupState>();

	public static List<string> ApplicationStatusValues => GetStringValues<ApplicationStatus>();

	public static List<string> GroupSubStatusValues => GetStringValues<GroupSubStatus>();

	public static List<string> ResourceSubStatusValues => GetStringValues<ResourceSubStatus>();

	public static List<string> VirtualMachineStateValues => GetStringValues<VirtualMachineState>();

	public static List<string> NodeStateValues => GetStringValues<NodeState>();

	public static List<string> NodeStatusValues => GetStringValues<NodeStatus>();

	public static List<string> NetworkInterfaceStateValues => GetStringValues<NetworkInterfaceState>();

	public static List<string> NetworkStateValues => GetStringValues<NetworkState>();

	public static List<string> NetworkRoleValues => GetStringValues<NetworkRole>();

	public static List<string> GroupTypeValues => GetStringValues<GroupType>();

	public static List<string> ResourceStateValues => GetStringValues<ResourceState>();

	public static List<string> ResourceFlagsValues => GetStringValues<ResourceFlags>();

	public static List<string> ResourceClassValues => GetStringValues<ResourceClass>();

	public static List<string> ResourceTypeValues => GetStringValues<ResourceKind>();

	public static List<string> ReplicationTypeValues => GetStringValues<ReplicationType>();

	public static List<string> ReplicationDiskTypeValues => GetStringValues<ReplicationDiskType>();

	public static List<string> ReplicationStatusValues => GetStringValues<ReplicationStatus>();

	public static List<string> PriorityValues => GetStringValues<Priority>();

	public static List<string> FilterStringValues => GetStringValues<FilterString>();

	public static List<string> VirtualMachineHeartbeatValues => GetStringValues<VirtualMachineHeartbeatStatus>();

	public static List<string> VirtualMachineIntegrationServicesValues => GetStringValues<VirtualMachineIntegrationServicesStatus>();

	public static List<string> VirtualMachineReplicationStateValues => GetStringValues<VirtualMachineReplicationState>();

	public static List<string> VirtualMachineReplicationModeValues => GetStringValues<VirtualMachineReplicationMode>();

	public static List<string> PartitionStyleValues => GetStringValues<PartitionStyle>();

	public static List<string> GetStringValues<T>()
	{
		Type typeFromHandle = typeof(T);
		if (!typeFromHandle.IsEnum)
		{
			throw new ArgumentException("ObjectType '" + typeFromHandle.Name + "' is not an enum");
		}
		LocalizedEnum localizedEnum = GetEnumerationType(typeFromHandle);
		return GetValues<T>().ConvertAll((T enumValue) => (localizedEnum == LocalizedEnum.Unknown) ? enumValue.ToString() : ((Enum)(object)enumValue).Translate(localizedEnum));
	}

	public static List<string> GetStringValues(Enum enumeration)
	{
		Type type = enumeration.GetType();
		LocalizedEnum localizedEnum = GetEnumerationType(type);
		return GetValues(enumeration).ConvertAll((Enum enumValue) => (localizedEnum == LocalizedEnum.Unknown) ? enumValue.ToString() : enumValue.Translate(localizedEnum));
	}

	public static string Translate(this Enum enumeration, LocalizedEnum enumType)
	{
		return enumType switch
		{
			LocalizedEnum.IdentityType => ((ClusterIdentityType)(object)enumeration).Translate(), 
			LocalizedEnum.StoragePoolQuorum => ((StoragePoolQuorum)(object)enumeration).Translate(), 
			LocalizedEnum.StoragePoolHealth => ((StoragePoolHealth)(object)enumeration).Translate(), 
			LocalizedEnum.ReplicationStatus => ((ReplicationStatus)(object)enumeration).Translate(), 
			LocalizedEnum.StoragePhysicalDriveState => ((StoragePhysicalDriveState)(object)enumeration).Translate(), 
			LocalizedEnum.StoragePhysicalDriveUsage => ((StoragePhysicalDriveUsage)(object)enumeration).Translate(), 
			LocalizedEnum.PhysicalDiskBusType => ((PhysicalDiskBusType)(object)enumeration).Translate(), 
			LocalizedEnum.VirtualDiskState => ((VirtualDiskState)(object)enumeration).Translate(), 
			LocalizedEnum.FileShareProtocol => ((FileShareProtocol)(object)enumeration).Translate(), 
			LocalizedEnum.ClusterSharedVolumeFaultState => ((ClusterSharedVolumeFaultState)(object)enumeration).Translate(), 
			LocalizedEnum.SpaceResiliencyType => ((VirtualDiskResiliencyType)(object)enumeration).Translate(), 
			LocalizedEnum.GroupState => ((GroupState)(object)enumeration).Translate(), 
			LocalizedEnum.ApplicationStatus => ((ApplicationStatus)(object)enumeration).Translate(), 
			LocalizedEnum.GroupSubStatus => ((GroupSubStatus)(object)enumeration).Translate(), 
			LocalizedEnum.ResourceSubStatus => ((ResourceSubStatus)(object)enumeration).Translate(), 
			LocalizedEnum.VirtualMachineState => ((VirtualMachineState)(object)enumeration).Translate(), 
			LocalizedEnum.NodeState => ((NodeState)(object)enumeration).Translate(), 
			LocalizedEnum.NodeStatus => ((NodeStatus)(object)enumeration).Translate(), 
			LocalizedEnum.NetworkState => ((NetworkState)(object)enumeration).Translate(), 
			LocalizedEnum.NetworkRole => ((NetworkRole)(object)enumeration).Translate(), 
			LocalizedEnum.NetworkInterfaceState => ((NetworkInterfaceState)(object)enumeration).Translate(), 
			LocalizedEnum.GroupType => ((GroupType)(object)enumeration).Translate(), 
			LocalizedEnum.ResourceState => ((ResourceState)(object)enumeration).Translate(), 
			LocalizedEnum.ResourceFlags => ((ResourceFlags)(object)enumeration).Translate(), 
			LocalizedEnum.ResourceClass => ((ResourceClass)(object)enumeration).Translate(), 
			LocalizedEnum.ResourceType => ((ResourceKind)(object)enumeration).Translate(), 
			LocalizedEnum.ReplicationType => ((ReplicationType)(object)enumeration).Translate(), 
			LocalizedEnum.ReplicationDiskType => ((ReplicationDiskType)(object)enumeration).Translate(), 
			LocalizedEnum.Priority => ((Priority)(object)enumeration).Translate(), 
			LocalizedEnum.FilterString => ((FilterString)(object)enumeration).Translate(), 
			LocalizedEnum.VirtualMachineHeartbeat => ((VirtualMachineHeartbeatStatus)(object)enumeration).Translate(), 
			LocalizedEnum.VirtualMachineIntegrationServices => ((VirtualMachineIntegrationServicesStatus)(object)enumeration).Translate(), 
			LocalizedEnum.VirtualMachineReplicationState => ((VirtualMachineReplicationState)(object)enumeration).Translate(), 
			LocalizedEnum.VirtualMachineReplicationMode => ((VirtualMachineReplicationMode)(object)enumeration).Translate(), 
			LocalizedEnum.VirtualMachineReplicationHealth => ((VirtualMachineReplicationHealth)(object)enumeration).Translate(), 
			LocalizedEnum.EnclosureHealthStatus => ((EnclosureHealthStatus)(object)enumeration).Translate(), 
			LocalizedEnum.DiskHealthStatus => ((PhysicalDiskHealthStatus)(object)enumeration).Translate(), 
			LocalizedEnum.PhysicalDiskUsage => ((PhysicalDiskUsage)(object)enumeration).Translate(), 
			LocalizedEnum.MediaType => ((MediaType)(object)enumeration).Translate(), 
			LocalizedEnum.FanOperationalStatus => ((FanOperationalStatus)(object)enumeration).Translate(), 
			LocalizedEnum.PowerSupplyOperationalStatus => ((PowerSupplyOperationalStatus)(object)enumeration).Translate(), 
			LocalizedEnum.TemperatureSensorOperationalStatus => ((TemperatureSensorOperationalStatus)(object)enumeration).Translate(), 
			LocalizedEnum.VoltageSensorOperationalStatus => ((VoltageSensorOperationalStatus)(object)enumeration).Translate(), 
			LocalizedEnum.CurrentSensorOperationalStatus => ((CurrentSensorOperationalStatus)(object)enumeration).Translate(), 
			LocalizedEnum.StorageNodeOperationalStatus => ((StorageNodeOperationalStatus)(object)enumeration).Translate(), 
			LocalizedEnum.IOControllerOperationalStatus => ((IOControllerOperationalStatus)(object)enumeration).Translate(), 
			LocalizedEnum.PartitionStyle => ((PartitionStyle)(object)enumeration).Translate(), 
			LocalizedEnum.BitLockerStatus => ((BitLockerStatus)(object)enumeration).Translate(), 
			LocalizedEnum.Unknown => enumeration.ToString(), 
			_ => throw new NotImplementedException("Enumeration {0} does not implement translate".FormatCurrentCulture(enumeration.ToString())), 
		};
	}

	public static string Translate(this FileShareProtocol protocol)
	{
		return protocol switch
		{
			FileShareProtocol.Smb => EnumResources.FileShareProtocol_Smb, 
			FileShareProtocol.Nfs => EnumResources.FileShareProtocol_Nfs, 
			_ => protocol.ToString(), 
		};
	}

	public static string Translate(this StoragePoolQuorum quorum)
	{
		return quorum switch
		{
			StoragePoolQuorum.Unknown => EnumResources.Unknown, 
			StoragePoolQuorum.Minority => EnumResources.StoragePoolQuorum_Majority, 
			StoragePoolQuorum.Majority => EnumResources.StoragePoolQuorum_Minority, 
			StoragePoolQuorum.Ok => EnumResources.OkString, 
			StoragePoolQuorum.Max => EnumResources.VirtualDiskState_Max, 
			StoragePoolQuorum.Fetching => CommonResources.LoadingText, 
			_ => quorum.ToString(), 
		};
	}

	public static string Translate(this VirtualDiskState virtualDiskState)
	{
		return virtualDiskState switch
		{
			VirtualDiskState.Unknown => EnumResources.Unknown, 
			VirtualDiskState.Degraded => EnumResources.VirtualDiskState_Degraded, 
			VirtualDiskState.Detached => EnumResources.VirtualDiskState_Detached, 
			VirtualDiskState.NoRedundancy => EnumResources.VirtualDiskState_NoRedundancy, 
			VirtualDiskState.Incomplete => EnumResources.VirtualDiskState_Incomplete, 
			VirtualDiskState.Max => EnumResources.StoragePoolResiliencyType_Max, 
			VirtualDiskState.Ok => EnumResources.OkString, 
			VirtualDiskState.Regenerating => EnumResources.VirtualDiskState_Regenerating, 
			VirtualDiskState.NeedsRebalance => EnumResources.VirtualDiskState_NeedsRebalance, 
			VirtualDiskState.Fetching => CommonResources.LoadingText, 
			_ => virtualDiskState.ToString(), 
		};
	}

	public static string Translate(this ReplicationStatus status)
	{
		return status switch
		{
			ReplicationStatus.Unknown => EnumResources.Unknown, 
			ReplicationStatus.ContinuouslyReplicatingOutOfRpo => EnumResources.ReplicationStatus_ReplicatingOutRpo, 
			ReplicationStatus.ContinuouslyReplicatingInRpo => EnumResources.ReplicationStatus_ReplicatingInRpo, 
			ReplicationStatus.ContinuouslyReplicating => EnumResources.ReplicationStatus_Replicating, 
			ReplicationStatus.WaitingForQuorum => EnumResources.ReplicationStatus_WaitingForQuorum, 
			ReplicationStatus.RecoveringFromReplicationLog => EnumResources.ReplicationStatus_RecoveringFromReplicationLog, 
			ReplicationStatus.LogRecordCopyFromSource => EnumResources.ReplicationStatus_LogRecordCopyFromSource, 
			ReplicationStatus.LogRecordCopyToDestination => EnumResources.ReplicationStatus_LogRecordCopyToDestination, 
			ReplicationStatus.BlockCopyToDestination => EnumResources.ReplicationStatus_BlockCopyToDestination, 
			ReplicationStatus.BlockCopyFromSource => EnumResources.ReplicationStatus_BlockCopyFromSource, 
			ReplicationStatus.Failed => EnumResources.ReplicationStatus_Failed, 
			ReplicationStatus.ReplicationSuspended => EnumResources.ReplicationStatus_Suspended, 
			ReplicationStatus.ConnectingToSource => EnumResources.ReplicationStatus_ConnectingToSource, 
			ReplicationStatus.WaitingForDestination => EnumResources.ReplicationStatus_WaitingForDestination, 
			ReplicationStatus.InitialBlockCopy => EnumResources.ReplicationStatus_InitialBlockCopy, 
			ReplicationStatus.Fetching => CommonResources.LoadingText, 
			ReplicationStatus.NotInPartnership => EnumResources.ReplicationStatus_NotInPartnership, 
			_ => status.ToString(), 
		};
	}

	public static string Translate(this IList<ReplicationStatusInfo> status)
	{
		if (status == null)
		{
			return EnumResources.Unknown;
		}
		if (status.Count == 0)
		{
			return string.Empty;
		}
		if (status[0].ReplicationStatus == ReplicationStatus.Fetching)
		{
			return CommonResources.LoadingText;
		}
		List<string> list = status.Select((ReplicationStatusInfo replicationStatus) => (replicationStatus.PercentageRecovered != 0 && replicationStatus.PercentageRecovered < 100) ? (replicationStatus.ReplicationStatus.Translate() + " (" + replicationStatus.PercentageRecovered + "%)") : replicationStatus.ReplicationStatus.Translate()).ToList();
		if (list.Distinct().Count() == 1)
		{
			return list[0];
		}
		return string.Join(", ", list);
	}

	public static string Translate(this StoragePoolHealth health)
	{
		return health switch
		{
			StoragePoolHealth.Warning => EnumResources.StoragePoolHealth_Warning, 
			StoragePoolHealth.Unknown => EnumResources.Unknown, 
			StoragePoolHealth.Unhealthy => EnumResources.StoragePoolHealth_Unhealthy, 
			StoragePoolHealth.Healthy => EnumResources.StoragePoolHealth_Healthy, 
			StoragePoolHealth.Fetching => CommonResources.LoadingText, 
			_ => health.ToString(), 
		};
	}

	public static string Translate(this StoragePhysicalDriveState driveState)
	{
		return driveState switch
		{
			StoragePhysicalDriveState.Unknown => EnumResources.Unknown, 
			StoragePhysicalDriveState.BecomingReady => EnumResources.StoragePhysicalDriveState_BecomingReady, 
			StoragePhysicalDriveState.UnrecognizedMetadata => EnumResources.StoragePhysicalDriveState_UnrecognizedMetadata, 
			StoragePhysicalDriveState.FailedMedia => EnumResources.StoragePhysicalDriveState_FailedMedia, 
			StoragePhysicalDriveState.HardwareError => EnumResources.StoragePhysicalDriveState_HardwareError, 
			StoragePhysicalDriveState.Split => EnumResources.StoragePhysicalDriveState_Split, 
			StoragePhysicalDriveState.StaleMetadata => EnumResources.StoragePhysicalDriveState_StaleMetadata, 
			StoragePhysicalDriveState.IOError => EnumResources.StoragePhysicalDriveState_IoError, 
			StoragePhysicalDriveState.Missing => EnumResources.StoragePhysicalDriveState_Missing, 
			StoragePhysicalDriveState.PredictingFailure => EnumResources.StoragePhysicalDriveState_PredictingFailure, 
			StoragePhysicalDriveState.NotUsable => EnumResources.StoragePhysicalDriveState_NotUsable, 
			StoragePhysicalDriveState.TransientError => EnumResources.StoragePhysicalDriveState_TransientError, 
			StoragePhysicalDriveState.InService => EnumResources.StoragePhysicalDriveState_InService, 
			StoragePhysicalDriveState.InMaintenance => EnumResources.StoragePhysicalDriveState_InMaintenance, 
			StoragePhysicalDriveState.Okay => EnumResources.OkString, 
			StoragePhysicalDriveState.Max => EnumResources.StoragePhysicalDriveState_Max, 
			StoragePhysicalDriveState.Fetching => CommonResources.LoadingText, 
			_ => driveState.ToString(), 
		};
	}

	public static string Translate(this StoragePhysicalDriveUsage driveUsage)
	{
		return driveUsage switch
		{
			StoragePhysicalDriveUsage.Unknown => EnumResources.Unknown, 
			StoragePhysicalDriveUsage.AutoAllocation => EnumResources.StoragePhysicalDriveUsage_AutoAllocation, 
			StoragePhysicalDriveUsage.ManualAllocation => EnumResources.StoragePhysicalDriveUsage_ManualAllocation, 
			StoragePhysicalDriveUsage.Spare => EnumResources.StoragePhysicalDriveUsage_Spare, 
			StoragePhysicalDriveUsage.Journal => EnumResources.StoragePhysicalDriveUsage_Journal, 
			StoragePhysicalDriveUsage.Retired => EnumResources.StoragePhysicalDriveUsage_Retired, 
			StoragePhysicalDriveUsage.Max => EnumResources.StoragePhysicalDriveUsage_Max, 
			_ => driveUsage.ToString(), 
		};
	}

	public static string Translate(this PhysicalDiskBusType busType)
	{
		return busType switch
		{
			PhysicalDiskBusType.Unknown => EnumResources.Unknown, 
			PhysicalDiskBusType.Scsi => EnumResources.StoragePhysicalDriveBusType_Scsi, 
			PhysicalDiskBusType.Atapi => EnumResources.StoragePhysicalDriveBusType_Atapi, 
			PhysicalDiskBusType.Ata => EnumResources.StoragePhysicalDriveBusType_Ata, 
			PhysicalDiskBusType.Bus1394 => EnumResources.StoragePhysicalDriveBusType_1394, 
			PhysicalDiskBusType.Ssa => EnumResources.StoragePhysicalDriveBusType_Ssa, 
			PhysicalDiskBusType.Fibre => EnumResources.StoragePhysicalDriveBusType_Fibre, 
			PhysicalDiskBusType.Usb => EnumResources.StoragePhysicalDriveBusType_Usb, 
			PhysicalDiskBusType.Raid => EnumResources.StoragePhysicalDriveBusType_RAID, 
			PhysicalDiskBusType.iScsi => EnumResources.StoragePhysicalDriveBusType_iScsi, 
			PhysicalDiskBusType.Sas => EnumResources.StoragePhysicalDriveBusType_Sas, 
			PhysicalDiskBusType.Sata => EnumResources.StoragePhysicalDriveBusType_Sata, 
			PhysicalDiskBusType.Sd => EnumResources.StoragePhysicalDriveBusType_Sd, 
			PhysicalDiskBusType.Mmc => EnumResources.StoragePhysicalDriveBusType_Mmc, 
			PhysicalDiskBusType.Virtual => EnumResources.StoragePhysicalDriveBusType_Virtual, 
			PhysicalDiskBusType.FileBackedVirtual => EnumResources.StoragePhysicalDriveBusType_FileBackedVirtual, 
			PhysicalDiskBusType.Spaces => EnumResources.StoragePhysicalDriveBusType_Spaces, 
			PhysicalDiskBusType.Nvme => EnumResources.StoragePhysicalDriveBusType_Nvme, 
			PhysicalDiskBusType.Scm => EnumResources.StoragePhysicalDriveBusType_Scm, 
			PhysicalDiskBusType.Ufs => EnumResources.StoragePhysicalDriveBusType_Ufs, 
			PhysicalDiskBusType.Max => EnumResources.StoragePhysicalDriveBusType_Max, 
			PhysicalDiskBusType.MaxReserved => EnumResources.StoragePhysicalDriveBusType_MaxReserved, 
			PhysicalDiskBusType.Fetching => CommonResources.LoadingText, 
			_ => busType.ToString(), 
		};
	}

	public static string Translate(this VirtualDiskResiliencyType resiliencyType)
	{
		return resiliencyType switch
		{
			VirtualDiskResiliencyType.Unknown => EnumResources.Unknown, 
			VirtualDiskResiliencyType.Simple => EnumResources.StoragePoolResiliencyType_Simple, 
			VirtualDiskResiliencyType.Mirror => EnumResources.StoragePoolResiliencyType_Mirror, 
			VirtualDiskResiliencyType.Parity => EnumResources.StoragePoolResiliencyType_Parity, 
			VirtualDiskResiliencyType.Max => EnumResources.StoragePoolResiliencyType_Max, 
			VirtualDiskResiliencyType.Fetching => CommonResources.LoadingText, 
			_ => resiliencyType.ToString(), 
		};
	}

	public static string Translate(this ClusterSharedVolumeFaultState fault)
	{
		return fault switch
		{
			ClusterSharedVolumeFaultState.Dismounted => EnumResources.ClusterSharedVolumeFaultState_Dismounted, 
			ClusterSharedVolumeFaultState.InMaintenance => EnumResources.ClusterSharedVolumeFaultState_InMaintenance, 
			ClusterSharedVolumeFaultState.NoAccess => EnumResources.ClusterSharedVolumeFaultState_NoAccess, 
			ClusterSharedVolumeFaultState.NoDirectIO => EnumResources.ClusterSharedVolumeFaultState_NoDirectIO, 
			ClusterSharedVolumeFaultState.NoFaults => string.Empty, 
			ClusterSharedVolumeFaultState.Fetching => CommonResources.LoadingText, 
			_ => string.Empty, 
		};
	}

	public static string Translate(this Priority state)
	{
		return state switch
		{
			Priority.High => EnumResources.Priority_High, 
			Priority.NoAutoStart => EnumResources.Priority_NoAutoStart, 
			Priority.Medium => EnumResources.Priority_Medium, 
			Priority.Low => EnumResources.Priority_Low, 
			Priority.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Priority_Custom, state), 
		};
	}

	public static string Translate(this ClusterIdentityType identity)
	{
		return identity switch
		{
			ClusterIdentityType.Cluster => EnumResources.ClusterIdentityType_Cluster, 
			ClusterIdentityType.Group => EnumResources.ClusterIdentityType_Group, 
			ClusterIdentityType.Resource => EnumResources.ClusterIdentityType_Resource, 
			ClusterIdentityType.Network => EnumResources.ClusterIdentityType_Network, 
			ClusterIdentityType.NetworkInterface => EnumResources.ClusterIdentityType_NetworkInterface, 
			ClusterIdentityType.Node => EnumResources.ClusterIdentityType_Node, 
			ClusterIdentityType.ResourceType => EnumResources.ClusterIdentityType_ResourceType, 
			_ => identity.ToString(), 
		};
	}

	public static string Translate(this GroupState state)
	{
		return state switch
		{
			GroupState.Failed => EnumResources.FailedStatus, 
			GroupState.Offline => EnumResources.GroupState_Offline, 
			GroupState.Online => EnumResources.GroupState_Online, 
			GroupState.PartialOnline => EnumResources.GroupState_PartialOnline, 
			GroupState.Pending => EnumResources.GroupState_Pending, 
			GroupState.Unknown => EnumResources.Unknown, 
			GroupState.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this ApplicationStatus status)
	{
		return status switch
		{
			ApplicationStatus.Failed => EnumResources.FailedStatus, 
			ApplicationStatus.PartiallyRunning => EnumResources.ApplicationStatus_PartiallyRunning, 
			ApplicationStatus.Paused => EnumResources.ApplicationStatus_Paused, 
			ApplicationStatus.Pending => EnumResources.ApplicationStatus_Pending, 
			ApplicationStatus.Running => EnumResources.ApplicationStatus_Running, 
			ApplicationStatus.Saved => EnumResources.ApplicationStatus_Saved, 
			ApplicationStatus.Saving => EnumResources.ApplicationStatus_Saving, 
			ApplicationStatus.Starting => EnumResources.ApplicationStatus_Starting, 
			ApplicationStatus.Stopped => EnumResources.ApplicationStatus_Stopped, 
			ApplicationStatus.Stopping => EnumResources.ApplicationStatus_Stopping, 
			ApplicationStatus.LiveMigrationQueued => EnumResources.ApplicationStatus_LiveMigrationQueued, 
			ApplicationStatus.LiveMigrating => EnumResources.ApplicationStatus_LiveMigrating, 
			ApplicationStatus.SnapshotInProgress => EnumResources.VirtualMachine_SnapshotInProgress, 
			ApplicationStatus.PowerOff => EnumResources.VirtualMachine_PowerOff, 
			ApplicationStatus.Unmonitored => EnumResources.ApplicationStatus_Unmonitored, 
			ApplicationStatus.Unknown => EnumResources.Unknown, 
			ApplicationStatus.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, status), 
		};
	}

	public static string Translate(this GroupSubStatus subStatus)
	{
		List<string> list = new List<string>();
		if (subStatus.HasFlag(GroupSubStatus.Locked))
		{
			list.Add(EnumResources.GroupSubStatus_Locked);
		}
		if (subStatus.HasFlag(GroupSubStatus.Preempted))
		{
			list.Add(EnumResources.GroupSubStatus_Preempted);
		}
		if (subStatus.HasFlag(GroupSubStatus.WaitingInQueueForMove))
		{
			list.Add(EnumResources.GroupSubStatus_WaitingInQueueForMove);
		}
		if (subStatus.HasFlag(GroupSubStatus.PhysicalResourcesLacking))
		{
			list.Add(EnumResources.GroupSubStatus_PhysicalResourcesLacking);
		}
		if (subStatus.HasFlag(GroupSubStatus.WaitingToStart))
		{
			list.Add(EnumResources.GroupSubStatus_WaitingToStart);
		}
		if (subStatus.HasFlag(GroupSubStatus.EmbeddedFailure))
		{
			list.Add(EnumResources.GroupSubStatus_EmbeddedFailure);
		}
		if (subStatus.HasFlag(GroupSubStatus.AffinityConflict))
		{
			list.Add(EnumResources.GroupSubStatus_AffinityConflict);
		}
		if (subStatus.HasFlag(GroupSubStatus.WaitingForDependencies))
		{
			list.Add(EnumResources.GroupSubStatus_WaitingForDependencies);
		}
		if (list.Count > 0)
		{
			return string.Join(EnumResources.EnumExtensionMethods_ListJoinSeparator, list);
		}
		return string.Empty;
	}

	public static string Translate(this ResourceSubStatus subStatus)
	{
		List<string> list = new List<string>();
		if (subStatus.HasFlag(ResourceSubStatus.Locked))
		{
			list.Add(EnumResources.ResourceSubStatus_Locked);
		}
		if (subStatus.HasFlag(ResourceSubStatus.EmbeddedFailure))
		{
			list.Add(EnumResources.GroupSubStatus_EmbeddedFailure);
		}
		if (subStatus.HasFlag(ResourceSubStatus.FailedDueToInsufficientCpu))
		{
			list.Add(EnumResources.ResourceSubStatus_FailedDueToInsufficientCpu);
		}
		if (subStatus.HasFlag(ResourceSubStatus.FailedDueToInsufficientMemory))
		{
			list.Add(EnumResources.ResourceSubStatus_FailedDueToInsufficientMemory);
		}
		if (subStatus.HasFlag(ResourceSubStatus.FailedDueToInsufficientGenericResources))
		{
			list.Add(EnumResources.ResourceSubStatus_FailedDueToInsufficientGenericResources);
		}
		if (list.Count > 0)
		{
			return string.Join(EnumResources.EnumExtensionMethods_ListJoinSeparator, list);
		}
		return string.Empty;
	}

	public static string Translate(this VirtualMachineState state)
	{
		return state switch
		{
			VirtualMachineState.PowerOff => EnumResources.VirtualMachine_PowerOff, 
			VirtualMachineState.Running => EnumResources.GroupState_VirtualMachine_Running, 
			VirtualMachineState.Paused => EnumResources.GroupState_VirtualMachine_Paused, 
			VirtualMachineState.Pausing => EnumResources.GroupState_VirtualMachine_Pausing, 
			VirtualMachineState.Resuming => EnumResources.GroupState_VirtualMachine_Resuming, 
			VirtualMachineState.Saved => EnumResources.GroupState_VirtualMachine_Saved, 
			VirtualMachineState.Saving => EnumResources.GroupState_VirtualMachine_Saving, 
			VirtualMachineState.ShuttingDown => EnumResources.GroupState_VirtualMachine_ShuttingDown, 
			VirtualMachineState.Starting => EnumResources.GroupState_VirtualMachine_Starting, 
			VirtualMachineState.Stopping => EnumResources.GroupState_VirtualMachine_Stopping, 
			VirtualMachineState.Unknown => EnumResources.Unknown, 
			VirtualMachineState.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this NodeState state)
	{
		return state switch
		{
			NodeState.Unknown => EnumResources.Unknown, 
			NodeState.Up => EnumResources.NodeState_Up, 
			NodeState.Down => EnumResources.NodeState_Down, 
			NodeState.Pause => EnumResources.NodeState_Pause, 
			NodeState.Joining => EnumResources.NodeState_Joining, 
			NodeState.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this NodeStatus status)
	{
		return status switch
		{
			NodeStatus.Unknown => EnumResources.Unknown, 
			NodeStatus.Up => EnumResources.NodeState_Up, 
			NodeStatus.Down => EnumResources.NodeState_Down, 
			NodeStatus.Paused => EnumResources.NodeState_Pause, 
			NodeStatus.Joining => EnumResources.NodeState_Joining, 
			NodeStatus.Draining => EnumResources.NodeStatus_Draining, 
			NodeStatus.DrainFailed => EnumResources.NodeStatus_DrainFailed, 
			NodeStatus.Isolated => EnumResources.NodeStatus_Isolated, 
			NodeStatus.Quarantined => EnumResources.NodeStatus_Quarantined, 
			NodeStatus.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, status), 
		};
	}

	public static string Translate(this NetworkInterfaceState state)
	{
		return state switch
		{
			NetworkInterfaceState.Unknown => EnumResources.Unknown, 
			NetworkInterfaceState.Unavailable => EnumResources.NetworkInterfaceState_Unavailable, 
			NetworkInterfaceState.Failed => EnumResources.NetworkInterfaceState_Failed, 
			NetworkInterfaceState.Unreachable => EnumResources.NetworkInterfaceState_Unreachable, 
			NetworkInterfaceState.Up => EnumResources.NetworkInterfaceState_Up, 
			NetworkInterfaceState.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this NetworkState state)
	{
		return state switch
		{
			NetworkState.Unknown => EnumResources.Unknown, 
			NetworkState.Unavailable => EnumResources.NetworkState_Unavailable, 
			NetworkState.Down => EnumResources.NetworkState_Down, 
			NetworkState.Partitioned => EnumResources.NetworkState_Partitioned, 
			NetworkState.Up => EnumResources.NetworkState_Up, 
			NetworkState.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this NetworkRole state)
	{
		return state switch
		{
			NetworkRole.Unknown => EnumResources.Unknown, 
			NetworkRole.ClientAccess => EnumResources.NetworkRoleState_ClientAccess, 
			NetworkRole.InternalAndClient => EnumResources.NetworkRoleState_InternalAndClient, 
			NetworkRole.InternalUse => EnumResources.NetworkRoleState_InternalUse, 
			NetworkRole.None => EnumResources.NetworkRoleState_None, 
			NetworkRole.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this GroupType groupType)
	{
		return groupType switch
		{
			GroupType.CoreCluster => EnumResources.GroupType_CoreCluster, 
			GroupType.AvailableStorage => EnumResources.GroupType_AvailableStorage, 
			GroupType.Temporary => EnumResources.GroupType_Temporary, 
			GroupType.ClusterSharedVolume => EnumResources.ClusterSharedVolume, 
			GroupType.FileServer => EnumResources.GroupType_FileServer, 
			GroupType.DhcpServer => EnumResources.GroupType_DhcpServer, 
			GroupType.Dtc => EnumResources.GroupType_Dtc, 
			GroupType.Msmq => EnumResources.GroupType_Msmq, 
			GroupType.Wins => EnumResources.GroupType_Wins, 
			GroupType.StandAloneDfs => EnumResources.GroupType_StandAloneDfs, 
			GroupType.GenericApplication => EnumResources.GroupType_GenericApplication, 
			GroupType.GenericService => EnumResources.GroupType_GenericService, 
			GroupType.GenericScript => EnumResources.GroupType_GenericScript, 
			GroupType.IScsiNameService => EnumResources.GroupType_InternetStorageService, 
			GroupType.IScsiTarget => EnumResources.GroupType_IScsiTarget, 
			GroupType.VirtualMachine => EnumResources.GroupType_VirtualMachine, 
			GroupType.VMReplicaBroker => EnumResources.GroupType_VMReplicaBroker, 
			GroupType.TsSessionBroker => EnumResources.GroupType_TsSessionBroker, 
			GroupType.ScaleOutFileServer => EnumResources.ScaleOutFileServer, 
			GroupType.InfrastructureFileServer => EnumResources.InfrastructureFileServer, 
			GroupType.TaskScheduler => EnumResources.ResourceType_TaskScheduler, 
			GroupType.ClusterAwareUpdating => CommonResources.ClusterAwareUpdating_Text, 
			GroupType.Unknown => EnumResources.GroupType_Unknown, 
			GroupType.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, groupType), 
		};
	}

	public static string Translate(this ResourceState state)
	{
		return state switch
		{
			ResourceState.Failed => EnumResources.FailedStatus, 
			ResourceState.Inherited => EnumResources.ResourceState_Inherited, 
			ResourceState.Initializing => EnumResources.ResourceState_Initializing, 
			ResourceState.Offline => EnumResources.ResourceState_Offline, 
			ResourceState.OfflinePending => EnumResources.ResourceState_OfflinePending, 
			ResourceState.Online => EnumResources.ResourceState_Online, 
			ResourceState.OnlinePending => EnumResources.ResourceState_OnlinePending, 
			ResourceState.Pending => EnumResources.ResourceState_Pending, 
			ResourceState.Unknown => EnumResources.Unknown, 
			ResourceState.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this ResourceFlags state)
	{
		return state switch
		{
			ResourceFlags.Core => EnumResources.ResourceFlags_Core, 
			ResourceFlags.None => EnumResources.ResourceFlags_None, 
			ResourceFlags.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this ResourceClass state)
	{
		return state switch
		{
			ResourceClass.Network => EnumResources.ResourceClass_Network, 
			ResourceClass.Storage => EnumResources.ResourceClass_Storage, 
			ResourceClass.Unknown => EnumResources.Unknown, 
			ResourceClass.User => EnumResources.ResourceClass_User, 
			ResourceClass.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this ReplicationType replicationType)
	{
		return replicationType switch
		{
			ReplicationType.None => EnumResources.None, 
			ReplicationType.Asynchronous => EnumResources.ReplicationTypeAsynchronous, 
			ReplicationType.Synchronous => EnumResources.ReplicationTypeSynchronous, 
			ReplicationType.Unknown => EnumResources.Unknown, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, replicationType), 
		};
	}

	public static string Translate(this ReplicationDiskType replicationDiskType)
	{
		return replicationDiskType switch
		{
			ReplicationDiskType.Destination => EnumResources.ReplicationDiskType_Destination, 
			ReplicationDiskType.LogDestination => EnumResources.ReplicationDiskType_DestinationLog, 
			ReplicationDiskType.LogSource => EnumResources.ReplicationDiskType_SourceLog, 
			ReplicationDiskType.None => EnumResources.ReplicationDiskType_None, 
			ReplicationDiskType.Source => EnumResources.ReplicationDiskType_Source, 
			ReplicationDiskType.NotInParthership => EnumResources.ReplicationDiskType_NotInPartnership, 
			ReplicationDiskType.LogNotInParthership => EnumResources.ReplicationDiskType_LogNotInPartnership, 
			ReplicationDiskType.Other => EnumResources.ReplicationDiskType_Other, 
			_ => replicationDiskType.ToString(), 
		};
	}

	public static string Translate(this ResourceKind state)
	{
		return state switch
		{
			ResourceKind.ScaleOutFileServer => EnumResources.ScaleOutFileServer, 
			ResourceKind.InfrastructureFileServer => EnumResources.InfrastructureFileServer, 
			ResourceKind.DistributedNetworkName => EnumResources.ResourceType_DistributedNetworkName, 
			ResourceKind.DistributedFileSystem => EnumResources.ResourceType_DfsNamespace, 
			ResourceKind.DfsReplicatedFolder => EnumResources.ResourceType_DfsReplicatedFolder, 
			ResourceKind.DhcpService => EnumResources.ResourceType_DHCPService, 
			ResourceKind.Dtc => EnumResources.ResourceType_DistributedTransactionCoordinator, 
			ResourceKind.FileServer => EnumResources.ResourceType_FileServer, 
			ResourceKind.FileShareWitness => EnumResources.ResourceType_FileShareWitness, 
			ResourceKind.CloudWitness => EnumResources.ResourceType_CloudWitness, 
			ResourceKind.GenericApplication => EnumResources.ResourceType_GenericApplication, 
			ResourceKind.GenericScript => EnumResources.ResourceType_GenericScript, 
			ResourceKind.GenericService => EnumResources.ResourceType_GenericService, 
			ResourceKind.IPAddress => EnumResources.ResourceType_IPAddress, 
			ResourceKind.IPv6Address => EnumResources.ResourceType_IPv6Address, 
			ResourceKind.IPv6TunnelAddress => EnumResources.ResourceType_IPv6TunnelAddress, 
			ResourceKind.IScsiNameService => EnumResources.ResourceType_ISNSClusterResource, 
			ResourceKind.IScsiTarget => EnumResources.ResourceType_IScsiTarget, 
			ResourceKind.Msmsq => EnumResources.ResourceType_MessageQueuing, 
			ResourceKind.MsmsqTrigger => EnumResources.ResourceType_MessageQueueTriggers, 
			ResourceKind.NetworkName => EnumResources.ResourceType_NetName, 
			ResourceKind.NetworkFileSystem => EnumResources.ResourceType_NetworkFileSystem, 
			ResourceKind.NfsShare => EnumResources.ResourceType_NfsShare, 
			ResourceKind.ClusterFileSystem => EnumResources.ClusterSharedVolume, 
			ResourceKind.PhysicalDisk => EnumResources.ResourceType_PhysicalDisk, 
			ResourceKind.StoragePool => EnumResources.ResourceType_StoragePool, 
			ResourceKind.TaskScheduler => EnumResources.ResourceType_TaskScheduler, 
			ResourceKind.ClusterAwareUpdating => CommonResources.ClusterAwareUpdating_Text, 
			ResourceKind.HyperVNetworkVirtualizationProviderAddress => CommonResources.HyperVNetworkVirtualizationProviderAddress_Text, 
			ResourceKind.NetworkAddressTranslator => CommonResources.NetworkAddressTranslator_Text, 
			ResourceKind.DisjointIPv4Address => CommonResources.DisjointIPv4Address_Text, 
			ResourceKind.DisjointIPv6Address => CommonResources.DisjointIPv6Address_Text, 
			ResourceKind.StorageReplica => CommonResources.StorageReplica_Text, 
			ResourceKind.VirtualMachine => EnumResources.ResourceType_VirtualMachine, 
			ResourceKind.VirtualMachineConfiguration => EnumResources.ResourceType_VirtualMachineConfiguration, 
			ResourceKind.VirtualMachineReplicationBroker => EnumResources.ResourceType_VMReplicaBroker, 
			ResourceKind.VolumeShadowCopyServiceTask => EnumResources.ResourceType_VolumeShadowCopyServiceTask, 
			ResourceKind.WinsService => EnumResources.ResourceType_WINSService, 
			ResourceKind.HealthService => CommonResources.HealthService_Text, 
			ResourceKind.HyperVClusterWmi => CommonResources.HyperVClusterWMI_Text, 
			ResourceKind.SDDCManagement => CommonResources.SDDCManagement_Text, 
			ResourceKind.CrossClusterOrchestrator => CommonResources.CrossClusterDependencyOrchestrator_Text, 
			ResourceKind.StorageQoS => CommonResources.StorageQoS_Text, 
			ResourceKind.VirtualMachineReplicationCoordinator => CommonResources.VirtualMachineReplicationCoordinator_Text, 
			ResourceKind.Other => EnumResources.ResourceType_Other, 
			ResourceKind.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, state), 
		};
	}

	public static string Translate(this FilterString filter)
	{
		return filter switch
		{
			FilterString.BeginsWith => EnumResources.FilterString_BeginsWith, 
			FilterString.DoesNotContains => EnumResources.FilterString_DoesNotContains, 
			FilterString.Contains => EnumResources.FilterString_Contains, 
			FilterString.EndsWith => EnumResources.FilterString_EndsWith, 
			FilterString.Equals => EnumResources.FilterString_Equals, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this VirtualMachineHeartbeatStatus filter)
	{
		return filter switch
		{
			VirtualMachineHeartbeatStatus.Unknown => string.Empty, 
			VirtualMachineHeartbeatStatus.Disabled => EnumResources.FilterString_Heartbeat_Disabled, 
			VirtualMachineHeartbeatStatus.Error => EnumResources.FilterString_Heartbeat_Error, 
			VirtualMachineHeartbeatStatus.LostCommunication => EnumResources.FilterString_Hearbeat_LostCommunications, 
			VirtualMachineHeartbeatStatus.NoContact => EnumResources.FilterString_Heatbeat_No_Contact, 
			VirtualMachineHeartbeatStatus.Ok => EnumResources.FilterString_Heatbeat_Okay, 
			VirtualMachineHeartbeatStatus.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this VirtualMachineIntegrationServicesStatus filter)
	{
		return filter switch
		{
			VirtualMachineIntegrationServicesStatus.Installed => EnumResources.FilterString_IntegrationServices_Installed, 
			VirtualMachineIntegrationServicesStatus.NotInstalled => EnumResources.FilterString_IntegrationServices_Not_Installed, 
			VirtualMachineIntegrationServicesStatus.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	internal static string Translate(this VirtualMachineReplicationHealth filter)
	{
		return filter switch
		{
			VirtualMachineReplicationHealth.Warning => EnumResources.FilterString_VirtualMachine_ReplicationHealth_Warning, 
			VirtualMachineReplicationHealth.Normal => EnumResources.FilterString_VirtualMachine_ReplicationHealth_Normal, 
			VirtualMachineReplicationHealth.Error => EnumResources.FilterString_VirtualMachine_ReplicationHealth_Critical, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this EnclosureHealthStatus filter)
	{
		return filter switch
		{
			EnclosureHealthStatus.Warning => EnumResources.FilterString_Enclosure_HealthStatus_Warning, 
			EnclosureHealthStatus.Healthy => EnumResources.FilterString_Enclosure_HealthStatus_Healthy, 
			EnclosureHealthStatus.Unhealthy => EnumResources.FilterString_Enclosure_HealthStatus_Unhealthy, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this PhysicalDiskHealthStatus filter)
	{
		return filter switch
		{
			PhysicalDiskHealthStatus.Warning => EnumResources.FilterString_Enclosure_HealthStatus_Warning, 
			PhysicalDiskHealthStatus.Healthy => EnumResources.FilterString_Enclosure_HealthStatus_Healthy, 
			PhysicalDiskHealthStatus.Unhealthy => EnumResources.FilterString_Enclosure_HealthStatus_Unhealthy, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this PhysicalDiskUsage filter)
	{
		return filter switch
		{
			PhysicalDiskUsage.AutoSelect => EnumResources.FilterString_PhysicalDisk_Usage_AutoSelect, 
			PhysicalDiskUsage.ManualSelect => EnumResources.FilterString_PhysicalDisk_Usage_ManualSelect, 
			PhysicalDiskUsage.HotSpare => EnumResources.FilterString_PhysicalDisk_Usage_HotSpare, 
			PhysicalDiskUsage.Retired => EnumResources.FilterString_PhysicalDisk_Usage_Retired, 
			PhysicalDiskUsage.Journal => EnumResources.FilterString_PhysicalDisk_Usage_Journal, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this MediaType filter)
	{
		return filter switch
		{
			MediaType.Hdd => EnumResources.FilterString_PhysicalDisk_MediaType_Hdd, 
			MediaType.Ssd => EnumResources.FilterString_PhysicalDisk_MediaType_Ssd, 
			MediaType.Unspecified => EnumResources.FilterString_PhysicalDisk_MediaType_Unspecified, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this FanOperationalStatus filter)
	{
		return filter switch
		{
			FanOperationalStatus.Ok => EnumResources.FilterString_Enclosure_OperationalStatus_Ok, 
			FanOperationalStatus.Degraded => EnumResources.FilterString_Enclosure_OperationalStatus_Degraded, 
			FanOperationalStatus.NonRecoverable => EnumResources.FilterString_Enclosure_OperationalStatus_NonRecoverable, 
			FanOperationalStatus.NotInstalled => EnumResources.FilterString_Enclosure_OperationalStatus_NotInstalled, 
			FanOperationalStatus.NotAvailable => EnumResources.FilterString_Enclosure_OperationalStatus_NotAvailable, 
			FanOperationalStatus.NoAccessAllowed => EnumResources.FilterString_Enclosure_OperationalStatus_NoAccessAllowed, 
			FanOperationalStatus.Error => EnumResources.FilterString_Enclosure_OperationalStatus_Error, 
			FanOperationalStatus.NotReported => EnumResources.FilterString_Enclosure_OperationalStatus_NotReported, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this PowerSupplyOperationalStatus filter)
	{
		return filter switch
		{
			PowerSupplyOperationalStatus.Ok => EnumResources.FilterString_Enclosure_OperationalStatus_Ok, 
			PowerSupplyOperationalStatus.Degraded => EnumResources.FilterString_Enclosure_OperationalStatus_Degraded, 
			PowerSupplyOperationalStatus.NonRecoverable => EnumResources.FilterString_Enclosure_OperationalStatus_NonRecoverable, 
			PowerSupplyOperationalStatus.NotInstalled => EnumResources.FilterString_Enclosure_OperationalStatus_NotInstalled, 
			PowerSupplyOperationalStatus.NotAvailable => EnumResources.FilterString_Enclosure_OperationalStatus_NotAvailable, 
			PowerSupplyOperationalStatus.NoAccessAllowed => EnumResources.FilterString_Enclosure_OperationalStatus_NoAccessAllowed, 
			PowerSupplyOperationalStatus.Error => EnumResources.FilterString_Enclosure_OperationalStatus_Error, 
			PowerSupplyOperationalStatus.NotReported => EnumResources.FilterString_Enclosure_OperationalStatus_NotReported, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this TemperatureSensorOperationalStatus filter)
	{
		return filter switch
		{
			TemperatureSensorOperationalStatus.Ok => EnumResources.FilterString_Enclosure_OperationalStatus_Ok, 
			TemperatureSensorOperationalStatus.Degraded => EnumResources.FilterString_Enclosure_OperationalStatus_Degraded, 
			TemperatureSensorOperationalStatus.NonRecoverable => EnumResources.FilterString_Enclosure_OperationalStatus_NonRecoverable, 
			TemperatureSensorOperationalStatus.NotInstalled => EnumResources.FilterString_Enclosure_OperationalStatus_NotInstalled, 
			TemperatureSensorOperationalStatus.NotAvailable => EnumResources.FilterString_Enclosure_OperationalStatus_NotAvailable, 
			TemperatureSensorOperationalStatus.NoAccessAllowed => EnumResources.FilterString_Enclosure_OperationalStatus_NoAccessAllowed, 
			TemperatureSensorOperationalStatus.Error => EnumResources.FilterString_Enclosure_OperationalStatus_Error, 
			TemperatureSensorOperationalStatus.NotReported => EnumResources.FilterString_Enclosure_OperationalStatus_NotReported, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this VoltageSensorOperationalStatus filter)
	{
		return filter switch
		{
			VoltageSensorOperationalStatus.Ok => EnumResources.FilterString_Enclosure_OperationalStatus_Ok, 
			VoltageSensorOperationalStatus.Degraded => EnumResources.FilterString_Enclosure_OperationalStatus_Degraded, 
			VoltageSensorOperationalStatus.NonRecoverable => EnumResources.FilterString_Enclosure_OperationalStatus_NonRecoverable, 
			VoltageSensorOperationalStatus.NotInstalled => EnumResources.FilterString_Enclosure_OperationalStatus_NotInstalled, 
			VoltageSensorOperationalStatus.NotAvailable => EnumResources.FilterString_Enclosure_OperationalStatus_NotAvailable, 
			VoltageSensorOperationalStatus.NoAccessAllowed => EnumResources.FilterString_Enclosure_OperationalStatus_NoAccessAllowed, 
			VoltageSensorOperationalStatus.Error => EnumResources.FilterString_Enclosure_OperationalStatus_Error, 
			VoltageSensorOperationalStatus.NotReported => EnumResources.FilterString_Enclosure_OperationalStatus_NotReported, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this CurrentSensorOperationalStatus filter)
	{
		return filter switch
		{
			CurrentSensorOperationalStatus.Ok => EnumResources.FilterString_Enclosure_OperationalStatus_Ok, 
			CurrentSensorOperationalStatus.Degraded => EnumResources.FilterString_Enclosure_OperationalStatus_Degraded, 
			CurrentSensorOperationalStatus.NonRecoverable => EnumResources.FilterString_Enclosure_OperationalStatus_NonRecoverable, 
			CurrentSensorOperationalStatus.NotInstalled => EnumResources.FilterString_Enclosure_OperationalStatus_NotInstalled, 
			CurrentSensorOperationalStatus.NotAvailable => EnumResources.FilterString_Enclosure_OperationalStatus_NotAvailable, 
			CurrentSensorOperationalStatus.NoAccessAllowed => EnumResources.FilterString_Enclosure_OperationalStatus_NoAccessAllowed, 
			CurrentSensorOperationalStatus.Error => EnumResources.FilterString_Enclosure_OperationalStatus_Error, 
			CurrentSensorOperationalStatus.NotReported => EnumResources.FilterString_Enclosure_OperationalStatus_NotReported, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this IOControllerOperationalStatus filter)
	{
		return filter switch
		{
			IOControllerOperationalStatus.Ok => EnumResources.FilterString_Enclosure_OperationalStatus_Ok, 
			IOControllerOperationalStatus.Degraded => EnumResources.FilterString_Enclosure_OperationalStatus_Degraded, 
			IOControllerOperationalStatus.NonRecoverable => EnumResources.FilterString_Enclosure_OperationalStatus_NonRecoverable, 
			IOControllerOperationalStatus.NotInstalled => EnumResources.FilterString_Enclosure_OperationalStatus_NotInstalled, 
			IOControllerOperationalStatus.NotAvailable => EnumResources.FilterString_Enclosure_OperationalStatus_NotAvailable, 
			IOControllerOperationalStatus.NoAccessAllowed => EnumResources.FilterString_Enclosure_OperationalStatus_NoAccessAllowed, 
			IOControllerOperationalStatus.Error => EnumResources.FilterString_Enclosure_OperationalStatus_Error, 
			IOControllerOperationalStatus.NotReported => EnumResources.FilterString_Enclosure_OperationalStatus_NotReported, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this StorageNodeOperationalStatus filter)
	{
		return filter switch
		{
			StorageNodeOperationalStatus.Down => EnumResources.ServiceStatusText_Stopped, 
			StorageNodeOperationalStatus.Joining => EnumResources.NodeState_Joining, 
			StorageNodeOperationalStatus.Paused => EnumResources.ServiceStatusText_Paused, 
			StorageNodeOperationalStatus.Up => EnumResources.ServiceStatusText_Running, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	internal static string Translate(this VirtualMachineReplicationState filter)
	{
		return filter switch
		{
			VirtualMachineReplicationState.Critical => EnumResources.FilterString_VirtualMachine_ReplicationState_Critical, 
			VirtualMachineReplicationState.Disabled => EnumResources.FilterString_VirtualMachine_ReplicationState_Disabled, 
			VirtualMachineReplicationState.WaitingToCompleteInitialReplication => EnumResources.FilterString_VirtualMachine_ReplicationState_WaitingToCompleteInitialReplication, 
			VirtualMachineReplicationState.SyncedReplicationComplete => EnumResources.FilterString_VirtualMachine_ReplicationState_SyncedReplicationComplete, 
			VirtualMachineReplicationState.Ready => EnumResources.FilterString_VirtualMachine_ReplicationState_Ready, 
			VirtualMachineReplicationState.Recovered => EnumResources.FilterString_VirtualMachine_ReplicationState_Recovered, 
			VirtualMachineReplicationState.Committed => EnumResources.FilterString_VirtualMachine_ReplicationState_Committed, 
			VirtualMachineReplicationState.Replicating => EnumResources.FilterString_VirtualMachine_ReplicationState_Replicating, 
			VirtualMachineReplicationState.Resynchronize => EnumResources.FilterString_VirtualMachine_ReplicationState_Resynchronize, 
			VirtualMachineReplicationState.WaitingForStartResynchronize => EnumResources.FilterString_VirtualMachine_ReplicationState_WaitingForStartResynchronize, 
			VirtualMachineReplicationState.ResynchronizeSuspended => EnumResources.FilterString_VirtualMachine_ReplicationState_ResynchronizeSuspended, 
			VirtualMachineReplicationState.Suspended => EnumResources.FilterString_VirtualMachine_ReplicationState_Suspended, 
			VirtualMachineReplicationState.FailoverInProgress => EnumResources.FilterString_VirtualMachine_ReplicationState_Failover_In_Progress, 
			VirtualMachineReplicationState.FailbackInProgress => EnumResources.FilterString_VirtualMachine_ReplicationState_Failback_In_Progress, 
			VirtualMachineReplicationState.FailbackComplete => EnumResources.FilterString_VirtualMachine_ReplicationState_Failback_Complete, 
			VirtualMachineReplicationState.WaitingForUpdateCompletion => EnumResources.FilterString_VirtualMachine_ReplicationState_Waiting_For_Update_Completion, 
			VirtualMachineReplicationState.UpdateCritical => EnumResources.FilterString_VirtualMachine_ReplicationState_Update_Critical, 
			VirtualMachineReplicationState.Fetching => CommonResources.LoadingText, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter), 
		};
	}

	public static string Translate(this VirtualMachineReplicationMode filter)
	{
		switch (filter)
		{
		case VirtualMachineReplicationMode.None:
			return EnumResources.FilterString_VirtualMachine_ReplicationMode_None;
		case VirtualMachineReplicationMode.Primary:
			return EnumResources.FilterString_VirtualMachine_ReplicationMode_Primary;
		case VirtualMachineReplicationMode.Recovery:
		case VirtualMachineReplicationMode.ExtendedReplica:
			return EnumResources.FilterString_VirtualMachine_ReplicationMode_Recovery;
		case VirtualMachineReplicationMode.TestReplica:
			return EnumResources.FilterString_VirtualMachine_ReplicationMode_TestReplica;
		case VirtualMachineReplicationMode.Fetching:
			return CommonResources.LoadingText;
		default:
			return string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, filter);
		}
	}

	public static string Translate(this PartitionStyle partitionStyle)
	{
		return partitionStyle switch
		{
			PartitionStyle.Raw => EnumResources.PartitionStyle_Raw, 
			PartitionStyle.MasterBootRecord => EnumResources.PartitionStyle_MasterBootRecord, 
			PartitionStyle.GuidPartitionTable => EnumResources.PartitionStyle_GuidPartitionTable, 
			_ => string.Format(CultureInfo.CurrentCulture, EnumResources.Unknown, partitionStyle), 
		};
	}

	public static string Translate(this BitLockerStatus status)
	{
		if (status <= BitLockerStatus.OfflineRaw)
		{
			if (status <= BitLockerStatus.HasTpmData)
			{
				if (status <= BitLockerStatus.Encrypting)
				{
					BitLockerStatus num = status;
					if ((ulong)num <= 8uL)
					{
						switch (num)
						{
						case BitLockerStatus.None:
							return EnumResources.Unknown;
						case BitLockerStatus.Decrypted:
							return EnumResources.BitLockerStatus_Decrypted;
						case BitLockerStatus.Encrypted:
							return EnumResources.BitLockerStatus_Encrypted;
						case BitLockerStatus.NeedReboot:
							return EnumResources.BitLockerStatus_NeedReboot;
						case BitLockerStatus.On:
							return EnumResources.BitLockerStatus_On;
						case BitLockerStatus.On | BitLockerStatus.NeedReboot:
						case BitLockerStatus.On | BitLockerStatus.Decrypted:
						case BitLockerStatus.NeedReboot | BitLockerStatus.Decrypted:
						case BitLockerStatus.On | BitLockerStatus.NeedReboot | BitLockerStatus.Decrypted:
							goto IL_02e7;
						}
					}
					switch (status)
					{
					case BitLockerStatus.Decrypting:
						return EnumResources.BitLockerStatus_Decrypting;
					case BitLockerStatus.Encrypting:
						return EnumResources.BitLockerStatus_Encrypting;
					}
				}
				else
				{
					switch (status)
					{
					case BitLockerStatus.HasRecoveryData:
						return EnumResources.BitLockerStatus_HasRecovertyData;
					case BitLockerStatus.HasTpmData:
						return EnumResources.BitLockerStatus_HasTPMData;
					case BitLockerStatus.Paused:
						return EnumResources.BitLockerStatus_Paused;
					case BitLockerStatus.Stopped:
						return EnumResources.BitLockerStatus_Stopped;
					}
				}
			}
			else
			{
				switch (status)
				{
				case BitLockerStatus.BootPartition:
					return EnumResources.BitLockerStatus_BootPartition;
				case BitLockerStatus.Converting:
					return EnumResources.BitLockerStatus_Converting;
				case BitLockerStatus.Disabled:
					return EnumResources.BitLockerStatus_Disabled;
				case BitLockerStatus.Locked:
					return EnumResources.BitLockerStatus_Locked;
				case BitLockerStatus.OfflineError:
					return EnumResources.BitLockerStatus_OfflineError;
				case BitLockerStatus.OfflineRaw:
					return EnumResources.BitLockerStatus_OfflineRAW;
				case BitLockerStatus.Secure:
					return EnumResources.BitLockerStatus_Secure;
				}
			}
		}
		else
		{
			switch (status)
			{
			case BitLockerStatus.Clustered:
				return EnumResources.BitLockerStatus_Clustered;
			case BitLockerStatus.Csv:
				return EnumResources.BitLockerStatus_CSV;
			case BitLockerStatus.CsvMetadata:
				return EnumResources.BitLockerStatus_CSVMetadata;
			case BitLockerStatus.DataOnly:
				return EnumResources.BitLockerStatus_DataOnly;
			case BitLockerStatus.HasExternalKey:
				return EnumResources.BitLockerStatus_HasExternalKey;
			case BitLockerStatus.HasPassphrase:
				return EnumResources.BitLockerStatus_HasPassPhrase;
			case BitLockerStatus.HasPassword:
				return EnumResources.BitLockerStatus_HasPassword;
			case BitLockerStatus.HasPin:
				return EnumResources.BitLockerStatus_HasPIN;
			case BitLockerStatus.HasStartupKey:
				return EnumResources.BitLockerStatus_HasStartupKey;
			case BitLockerStatus.HasUserCertificateKey:
				return EnumResources.BitLockerStatus_HasUserCertificateKey;
			case BitLockerStatus.PreProvisioned:
				return EnumResources.BitLockerStatus_PreProvisioned;
			case BitLockerStatus.RoamingDevice:
				return EnumResources.BitLockerStatus_RoamingDevice;
			case BitLockerStatus.SupportsEdrv:
				return EnumResources.BitLockerStatus_SupportsEDRV;
			case BitLockerStatus.ThinProvisioned:
				return EnumResources.BitLockerStatus_ThinProvisioned;
			case BitLockerStatus.UsesEdrv:
				return EnumResources.BitLockerStatus_UsesEDRV;
			}
		}
		goto IL_02e7;
		IL_02e7:
		return status.ToString();
	}

	public static ApplicationStatus ToApplicationStatus(this GroupState state)
	{
		return state switch
		{
			GroupState.Failed => ApplicationStatus.Failed, 
			GroupState.Offline => ApplicationStatus.Stopped, 
			GroupState.Online => ApplicationStatus.Running, 
			GroupState.PartialOnline => ApplicationStatus.PartiallyRunning, 
			GroupState.Pending => ApplicationStatus.Pending, 
			GroupState.Unknown => ApplicationStatus.Unknown, 
			GroupState.Fetching => ApplicationStatus.Fetching, 
			_ => ApplicationStatus.Unknown, 
		};
	}

	public static ApplicationStatus ToApplicationStatus(this VirtualMachineState? state)
	{
		if (!state.HasValue)
		{
			return ApplicationStatus.Fetching;
		}
		return state.Value switch
		{
			VirtualMachineState.SnapshotInProgress => ApplicationStatus.SnapshotInProgress, 
			VirtualMachineState.PowerOff => ApplicationStatus.PowerOff, 
			VirtualMachineState.Running => ApplicationStatus.Running, 
			VirtualMachineState.Fetching => ApplicationStatus.Fetching, 
			VirtualMachineState.Paused => ApplicationStatus.Paused, 
			VirtualMachineState.Resuming => ApplicationStatus.Starting, 
			VirtualMachineState.Saving => ApplicationStatus.Saving, 
			VirtualMachineState.ShuttingDown => ApplicationStatus.Stopping, 
			VirtualMachineState.Starting => ApplicationStatus.Starting, 
			VirtualMachineState.Stopping => ApplicationStatus.Stopping, 
			VirtualMachineState.Saved => ApplicationStatus.Saved, 
			VirtualMachineState.Unknown => ApplicationStatus.Unknown, 
			_ => ApplicationStatus.Unknown, 
		};
	}

	public static ApplicationStatus ToApplicationStatus(this VirtualMachineState state, GroupState groupState, GroupSubStatus subStatus, VirtualMachineComputerSystemOperationalStatus migrationState)
	{
		bool flag = migrationState == VirtualMachineComputerSystemOperationalStatus.MigratingVirtualMachine;
		bool flag2 = subStatus.HasFlag(GroupSubStatus.WaitingInQueueForMove);
		if (state == VirtualMachineState.PowerOff && migrationState == VirtualMachineComputerSystemOperationalStatus.WaitingToStart)
		{
			return ApplicationStatus.Starting;
		}
		if (state == VirtualMachineState.Fetching || subStatus == GroupSubStatus.Fetching)
		{
			return ApplicationStatus.Fetching;
		}
		if (flag)
		{
			return ApplicationStatus.LiveMigrating;
		}
		if (flag2)
		{
			return ApplicationStatus.LiveMigrationQueued;
		}
		if (groupState == GroupState.Offline)
		{
			if (state != VirtualMachineState.Saved)
			{
				return ApplicationStatus.PowerOff;
			}
			return ApplicationStatus.Saved;
		}
		return ToApplicationStatus(state);
	}

	public static VirtualMachineComputerSystemOperationalStatus GetMigrationState(this VirtualMachineComputerSystemOperationalStatus operationStatus)
	{
		return (VirtualMachineComputerSystemOperationalStatus)((uint)operationStatus >> 16);
	}

	public static VirtualMachineComputerSystemOperationalStatus GetOperationStatus(this VirtualMachineComputerSystemOperationalStatus operationStatus)
	{
		return operationStatus & (VirtualMachineComputerSystemOperationalStatus)65535;
	}

	public static VirtualMachineState ToVirtualMachineRealState(this VirtualMachineState? state, ResourceState? resourceState)
	{
		if (!state.HasValue)
		{
			return VirtualMachineState.Fetching;
		}
		if (resourceState == ResourceState.Offline && (state == VirtualMachineState.Running || state == VirtualMachineState.Paused))
		{
			return VirtualMachineState.PowerOff;
		}
		return state.Value;
	}

	public static List<T> GetValues<T>()
	{
		Type enumType = typeof(T);
		if (!enumType.IsEnum)
		{
			throw new ArgumentException("ObjectType '" + enumType.Name + "' is not an enum");
		}
		return (from field in enumType.GetFields()
			where field.IsLiteral
			select field.GetValue(enumType) into value
			select (T)value).ToList();
	}

	public static List<Enum> GetValues(Enum enumeration)
	{
		Type type = enumeration.GetType();
		List<Enum> list = new List<Enum>();
		foreach (FieldInfo item in from field in type.GetFields()
			where field.IsLiteral
			select field)
		{
			object value = item.GetValue(type);
			list.Add((Enum)value);
		}
		return list;
	}

	public static LocalizedEnum GetEnumerationType(Type enumeration)
	{
		if (enumeration == typeof(ClusterIdentityType))
		{
			return LocalizedEnum.IdentityType;
		}
		if (enumeration == typeof(GroupState))
		{
			return LocalizedEnum.GroupState;
		}
		if (enumeration == typeof(ApplicationStatus))
		{
			return LocalizedEnum.ApplicationStatus;
		}
		if (enumeration == typeof(GroupSubStatus))
		{
			return LocalizedEnum.GroupSubStatus;
		}
		if (enumeration == typeof(ResourceSubStatus))
		{
			return LocalizedEnum.ResourceSubStatus;
		}
		if (enumeration == typeof(VirtualMachineState))
		{
			return LocalizedEnum.VirtualMachineState;
		}
		if (enumeration == typeof(NodeState))
		{
			return LocalizedEnum.NodeState;
		}
		if (enumeration == typeof(NodeStatus))
		{
			return LocalizedEnum.NodeStatus;
		}
		if (enumeration == typeof(NetworkInterfaceState))
		{
			return LocalizedEnum.NetworkInterfaceState;
		}
		if (enumeration == typeof(NetworkState))
		{
			return LocalizedEnum.NetworkState;
		}
		if (enumeration == typeof(NetworkRole))
		{
			return LocalizedEnum.NetworkRole;
		}
		if (enumeration == typeof(GroupType))
		{
			return LocalizedEnum.GroupType;
		}
		if (enumeration == typeof(ResourceState))
		{
			return LocalizedEnum.ResourceState;
		}
		if (enumeration == typeof(ResourceFlags))
		{
			return LocalizedEnum.ResourceFlags;
		}
		if (enumeration == typeof(ResourceClass))
		{
			return LocalizedEnum.ResourceClass;
		}
		if (enumeration == typeof(ResourceKind))
		{
			return LocalizedEnum.ResourceType;
		}
		if (enumeration == typeof(ReplicationType))
		{
			return LocalizedEnum.ReplicationType;
		}
		if (enumeration == typeof(ReplicationDiskType))
		{
			return LocalizedEnum.ReplicationDiskType;
		}
		if (enumeration == typeof(Priority))
		{
			return LocalizedEnum.Priority;
		}
		if (enumeration == typeof(FilterString))
		{
			return LocalizedEnum.FilterString;
		}
		if (enumeration == typeof(VirtualMachineHeartbeatStatus))
		{
			return LocalizedEnum.VirtualMachineHeartbeat;
		}
		if (enumeration == typeof(VirtualMachineIntegrationServicesStatus))
		{
			return LocalizedEnum.VirtualMachineIntegrationServices;
		}
		if (enumeration == typeof(VirtualMachineReplicationState))
		{
			return LocalizedEnum.VirtualMachineReplicationState;
		}
		if (enumeration == typeof(VirtualMachineReplicationMode))
		{
			return LocalizedEnum.VirtualMachineReplicationMode;
		}
		if (enumeration == typeof(StoragePhysicalDriveState))
		{
			return LocalizedEnum.StoragePhysicalDriveState;
		}
		if (enumeration == typeof(StoragePhysicalDriveUsage))
		{
			return LocalizedEnum.StoragePhysicalDriveUsage;
		}
		if (enumeration == typeof(PhysicalDiskBusType))
		{
			return LocalizedEnum.PhysicalDiskBusType;
		}
		if (enumeration == typeof(FileShareProtocol))
		{
			return LocalizedEnum.FileShareProtocol;
		}
		if (enumeration == typeof(StoragePoolHealth))
		{
			return LocalizedEnum.StoragePoolHealth;
		}
		if (enumeration == typeof(ReplicationStatus))
		{
			return LocalizedEnum.ReplicationStatus;
		}
		if (enumeration == typeof(StoragePoolQuorum))
		{
			return LocalizedEnum.StoragePoolQuorum;
		}
		if (enumeration == typeof(VirtualDiskState))
		{
			return LocalizedEnum.VirtualDiskState;
		}
		if (enumeration == typeof(VirtualDiskResiliencyType))
		{
			return LocalizedEnum.SpaceResiliencyType;
		}
		if (enumeration == typeof(EnclosureHealthStatus))
		{
			return LocalizedEnum.EnclosureHealthStatus;
		}
		if (enumeration == typeof(PhysicalDiskHealthStatus))
		{
			return LocalizedEnum.DiskHealthStatus;
		}
		if (enumeration == typeof(MediaType))
		{
			return LocalizedEnum.MediaType;
		}
		if (enumeration == typeof(PhysicalDiskBusType))
		{
			return LocalizedEnum.PhysicalDiskBusType;
		}
		if (enumeration == typeof(PhysicalDiskUsage))
		{
			return LocalizedEnum.PhysicalDiskUsage;
		}
		if (enumeration == typeof(FanOperationalStatus))
		{
			return LocalizedEnum.FanOperationalStatus;
		}
		if (enumeration == typeof(PowerSupplyOperationalStatus))
		{
			return LocalizedEnum.PowerSupplyOperationalStatus;
		}
		if (enumeration == typeof(TemperatureSensorOperationalStatus))
		{
			return LocalizedEnum.TemperatureSensorOperationalStatus;
		}
		if (enumeration == typeof(CurrentSensorOperationalStatus))
		{
			return LocalizedEnum.CurrentSensorOperationalStatus;
		}
		if (enumeration == typeof(VoltageSensorOperationalStatus))
		{
			return LocalizedEnum.VoltageSensorOperationalStatus;
		}
		if (enumeration == typeof(IOControllerOperationalStatus))
		{
			return LocalizedEnum.IOControllerOperationalStatus;
		}
		if (enumeration == typeof(StorageNodeOperationalStatus))
		{
			return LocalizedEnum.StorageNodeOperationalStatus;
		}
		if (enumeration == typeof(PartitionStyle))
		{
			return LocalizedEnum.PartitionStyle;
		}
		if (enumeration == typeof(BitLockerStatus))
		{
			return LocalizedEnum.BitLockerStatus;
		}
		return LocalizedEnum.Unknown;
	}

	public static string Translate(Enum enumeration)
	{
		return enumeration.Translate(GetEnumerationType(enumeration.GetType()));
	}

	public static Array GetFilterableValues(this Enum enumeration)
	{
		return (from object value in Enum.GetValues(enumeration.GetType())
			where !value.FilteredEnumValue()
			orderby value descending
			select value).ToArray();
	}

	public static bool FilteredEnumValue(this object value)
	{
		FilterableAttribute filterableAttribute = (FilterableAttribute)value.GetType().GetField(value.ToString()).GetCustomAttributes(inherit: true)
			.FirstOrDefault((object attribute) => attribute is FilterableAttribute);
		if (filterableAttribute != null)
		{
			return !filterableAttribute.Filterable;
		}
		return false;
	}
}
