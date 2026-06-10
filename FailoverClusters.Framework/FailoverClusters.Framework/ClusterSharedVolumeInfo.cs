namespace FailoverClusters.Framework;

public class ClusterSharedVolumeInfo
{
	public ClusterDiskPartition Owner { get; private set; }

	public ulong VolumeOffset { get; set; }

	public uint PartitionNumber { get; set; }

	public ClusterSharedVolumeFaultState FaultState { get; set; }

	public uint BackupState { get; set; }

	public string VolumeGuid { get; set; }

	public string VolumeName { get; set; }

	public string FriendlyVolumeName { get; set; }

	public string RootPath { get; set; }

	public ClusterSharedVolumeInfo(ClusterDiskPartition owner)
	{
		Owner = owner;
	}
}

