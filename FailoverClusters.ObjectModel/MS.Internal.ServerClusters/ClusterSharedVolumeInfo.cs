using System.Globalization;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class ClusterSharedVolumeInfo
{
	private ulong volumeOffset;

	private ClusterSharedVolumeFaultState faultState;

	private ClusterSharedVolumeBackupState backupState;

	private string friendlyVolumeName;

	private uint partitionNumber;

	private ClusterDiskPartition partition;

	private string deviceId;

	private bool? needToDisableMaintenance;

	private string resourceName;

	public string SharedVolumeResourceName => resourceName;

	public bool IsMaintenanceModeOn
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return faultState == ClusterSharedVolumeFaultState.InMaintenance;
		}
	}

	public string DeviceId => deviceId;

	public ClusterDiskPartition Partition => partition;

	public uint PartitionNumber => partitionNumber;

	public string FriendlyVolumeName => friendlyVolumeName;

	public ClusterSharedVolumeBackupState BackupState => backupState;

	public ClusterSharedVolumeFaultState FaultState => faultState;

	public ulong VolumeOffset => volumeOffset;

	public unsafe ClusterSharedVolumeInfo(_CLUS_CSV_VOLUME_INFO* sharedVolumeInfo, ClusterDiskPartition partition)
	{
		//IL_0046: Expected I, but got I8
		volumeOffset = *(ulong*)sharedVolumeInfo;
		faultState = *(ClusterSharedVolumeFaultState*)((ulong)(nint)sharedVolumeInfo + 12uL);
		backupState = *(ClusterSharedVolumeBackupState*)((ulong)(nint)sharedVolumeInfo + 16uL);
		partitionNumber = *(uint*)((ulong)(nint)sharedVolumeInfo + 8uL);
		this.partition = partition;
		deviceId = new string((char*)((ulong)(nint)sharedVolumeInfo + 540uL));
		needToDisableMaintenance = null;
	}

	public unsafe ClusterSharedVolumeInfo(Cluster cluster, _CLUS_CSV_VOLUME_INFO* sharedVolumeInfo, ClusterDiskPartition partition, ClusterResource sharedVolumeResource)
	{
		//IL_006c: Expected I, but got I8
		//IL_0084: Expected I, but got I8
		PropertyCollection commonProperties = cluster.GetCommonProperties(PropertyCollectionSet.ReadOnly);
		string name = "SharedVolumesRoot";
		string arg = (string)commonProperties.GetProperty(name).Value;
		volumeOffset = *(ulong*)sharedVolumeInfo;
		faultState = *(ClusterSharedVolumeFaultState*)((ulong)(nint)sharedVolumeInfo + 12uL);
		backupState = *(ClusterSharedVolumeBackupState*)((ulong)(nint)sharedVolumeInfo + 16uL);
		partitionNumber = *(uint*)((ulong)(nint)sharedVolumeInfo + 8uL);
		this.partition = partition;
		friendlyVolumeName = string.Format(CultureInfo.CurrentCulture, "{0}\\{1}", arg, InteropHelp.WstrToString((ushort*)((ulong)(nint)sharedVolumeInfo + 20uL)));
		deviceId = new string((char*)((ulong)(nint)sharedVolumeInfo + 540uL));
		needToDisableMaintenance = null;
		resourceName = sharedVolumeResource.DisplayName;
	}

	public ClusterSharedVolumeInfo()
	{
		volumeOffset = 0uL;
		faultState = ClusterSharedVolumeFaultState.Unknown;
		backupState = ClusterSharedVolumeBackupState.Unknown;
		friendlyVolumeName = string.Empty;
		partitionNumber = 0u;
		deviceId = string.Empty;
		needToDisableMaintenance = null;
		resourceName = string.Empty;
	}

	public void EnableMaintenance(ClusterResource sharedVolume)
	{
		if (!(faultState == ClusterSharedVolumeFaultState.InMaintenance))
		{
			sharedVolume.PhysicalDisk_EnableSharedVolumeMaintenanceMode(deviceId);
			bool? flag = true;
			needToDisableMaintenance = flag;
		}
	}

	public void DisableMaintenance(ClusterResource sharedVolume)
	{
		if (faultState == ClusterSharedVolumeFaultState.InMaintenance && (!needToDisableMaintenance.HasValue || needToDisableMaintenance.Value))
		{
			sharedVolume.PhysicalDisk_DisableSharedVolumeMaintenanceMode(deviceId);
		}
	}
}
