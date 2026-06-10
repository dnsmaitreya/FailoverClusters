using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterSharedVolumeStateInfo
{
	public string VolumeName { get; internal set; }

	public string NodeName { get; internal set; }

	public ClusterSharedVolumeState StateInfo { get; internal set; }

	public string VolumeFriendlyName { get; internal set; }

	public ClusterSharedRedirectedIoReason RedirectedIoReason { get; internal set; }

	public ClusterSharedVolumeRedirectedIoReason VolumeRedirectedIoReason { get; internal set; }

	internal ClusterSharedVolumeStateInfo()
	{
	}

	internal ClusterSharedVolumeStateInfo(NativeMethods.CLUSTER_SHARED_VOLUME_STATE_INFO_EX csvVolumeStateInfo)
	{
		VolumeFriendlyName = csvVolumeStateInfo.szVolumeFriendlyName;
		VolumeName = csvVolumeStateInfo.szVolumeName;
		NodeName = csvVolumeStateInfo.szNodeName;
		VolumeRedirectedIoReason = (ClusterSharedVolumeRedirectedIoReason)csvVolumeStateInfo.VolumeRedirectedIoReason;
		StateInfo = (ClusterSharedVolumeState)csvVolumeStateInfo.VolumeState;
		RedirectedIoReason = (ClusterSharedRedirectedIoReason)csvVolumeStateInfo.RedirectedIoReason;
	}
}

