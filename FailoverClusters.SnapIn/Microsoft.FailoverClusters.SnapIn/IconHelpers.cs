using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Controls;

namespace Microsoft.FailoverClusters.SnapIn;

public static class IconHelpers
{
	public static int GetResourceTypeIconIndex(Resource resource)
	{
		Exceptions.ThrowIfNull((object)resource, "resource");
		return GetResourceTypeIconIndex(resource, resource.ResourceType.ResourceKind);
	}

	private static int GetResourceTypeIconIndex(Resource resource, ResourceKind resourceType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IconsHelp.GetIconIndexByState(IconsHelp.GetIconIndexSet(GetIconSetForResourceType(resource, resourceType)), (IconState)0);
	}

	public static int GetResourceIconIndex(Resource resource, ResourceState state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		IconSet iconSetForResource = GetIconSetForResource(resource);
		IconState iconState = GetIconState(state);
		return IconsHelp.GetIconIndexByState(IconsHelp.GetIconIndexSet(iconSetForResource), iconState);
	}

	private static IconState GetIconState(ResourceState resourceState)
	{
		switch (resourceState)
		{
		case ResourceState.Failed:
			return (IconState)2;
		case ResourceState.Offline:
			return (IconState)1;
		case ResourceState.Online:
			return (IconState)0;
		case ResourceState.Initializing:
		case ResourceState.Pending:
		case ResourceState.OnlinePending:
		case ResourceState.OfflinePending:
			return (IconState)4;
		default:
			return (IconState)5;
		}
	}

	private static IconSet GetIconSetForResource(Resource resource)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		IconSet val = GetIconSetForResourceType(resource, (resource == null) ? ResourceKind.Other : resource.ResourceType.ResourceKind);
		if ((int)val == 1 && resource != null)
		{
			if (resource.Class == ResourceClass.Network)
			{
				val = (IconSet)16;
			}
			else if (resource.Class == ResourceClass.Storage)
			{
				val = (IconSet)19;
			}
		}
		return val;
	}

	private static IconSet GetIconSetForResourceType(Resource resource, ResourceKind resourceKind)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		IconSet val = (IconSet)1;
		switch (resourceKind)
		{
		case ResourceKind.DhcpService:
			return (IconSet)4;
		case ResourceKind.DistributedFileSystem:
			return (IconSet)9;
		case ResourceKind.FileShareWitness:
			return (IconSet)2;
		case ResourceKind.FileServer:
			return (IconSet)2;
		case ResourceKind.GenericApplication:
			return (IconSet)10;
		case ResourceKind.GenericScript:
			return (IconSet)12;
		case ResourceKind.GenericService:
			return (IconSet)11;
		case ResourceKind.IPAddress:
		case ResourceKind.HyperVNetworkVirtualizationProviderAddress:
		case ResourceKind.NetworkAddressTranslator:
			return (IconSet)16;
		case ResourceKind.IPv6Address:
			return (IconSet)16;
		case ResourceKind.IPv6TunnelAddress:
			return (IconSet)16;
		case ResourceKind.DisjointIPv4Address:
			return (IconSet)16;
		case ResourceKind.DisjointIPv6Address:
			return (IconSet)16;
		case ResourceKind.StorageReplica:
			return (IconSet)19;
		case ResourceKind.IScsiNameService:
			return (IconSet)13;
		case ResourceKind.Msmsq:
			return (IconSet)6;
		case ResourceKind.MsmsqTrigger:
			return (IconSet)7;
		case ResourceKind.NetworkName:
			return (IconSet)17;
		case ResourceKind.NfsShare:
			return (IconSet)22;
		case ResourceKind.PhysicalDisk:
			if (resource != null && resource.OwnerGroup.GroupType == GroupType.ClusterSharedVolume)
			{
				return (IconSet)18;
			}
			return (IconSet)19;
		case ResourceKind.VirtualMachine:
			return (IconSet)14;
		case (ResourceKind)45:
			return (IconSet)14;
		case ResourceKind.VirtualMachineReplicationBroker:
			return (IconSet)27;
		case ResourceKind.VirtualMachineConfiguration:
			return (IconSet)15;
		case ResourceKind.VolumeShadowCopyServiceTask:
			return (IconSet)21;
		case ResourceKind.WinsService:
			return (IconSet)8;
		case ResourceKind.DfsReplicatedFolder:
			return (IconSet)23;
		case ResourceKind.ScaleOutFileServer:
			return (IconSet)3;
		case ResourceKind.DistributedNetworkName:
			return (IconSet)17;
		case ResourceKind.NetworkFileSystem:
			return (IconSet)28;
		case ResourceKind.TaskScheduler:
			return (IconSet)21;
		case ResourceKind.ClusterAwareUpdating:
			return (IconSet)29;
		case ResourceKind.StoragePool:
			return (IconSet)20;
		case ResourceKind.HealthService:
			return (IconSet)31;
		case ResourceKind.HyperVClusterWmi:
			return (IconSet)32;
		case ResourceKind.StorageQoS:
			return (IconSet)33;
		case ResourceKind.VirtualMachineReplicationCoordinator:
			return (IconSet)34;
		case (ResourceKind)44:
			return (IconSet)19;
		case (ResourceKind)46:
			return (IconSet)11;
		default:
			if (resource != null && resource.Class == ResourceClass.Storage)
			{
				return (IconSet)19;
			}
			return (IconSet)1;
		}
	}
}
