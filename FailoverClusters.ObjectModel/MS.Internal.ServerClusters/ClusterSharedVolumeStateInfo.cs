namespace MS.Internal.ServerClusters;

public class ClusterSharedVolumeStateInfo
{
	private string _volumeName;

	private string _nodeName;

	private CsvState _volumeState;

	private string _volumeFriendlyName;

	private CsvRedirectedReason _redirectedReason;

	private CsvVolumeRedirectedReason _volumeRedirectedReason;

	public string FriendlyVolumeName => _volumeFriendlyName;

	public CsvVolumeRedirectedReason VolumeRedirectedReason => _volumeRedirectedReason;

	public CsvRedirectedReason RedirectedReason => _redirectedReason;

	public CsvState VolumeState => _volumeState;

	public string NodeName => _nodeName;

	public string VolumeName => _volumeName;

	public unsafe ClusterSharedVolumeStateInfo(_CLUSTER_SHARED_VOLUME_STATE_INFO_EX* pStateInfo)
	{
		//IL_0020: Expected I, but got I8
		//IL_0033: Expected I, but got I8
		_volumeName = new string((char*)pStateInfo);
		_nodeName = new string((char*)((ulong)(nint)pStateInfo + 520uL));
		_volumeFriendlyName = new string((char*)((ulong)(nint)pStateInfo + 1044uL));
		_redirectedReason = *(CsvRedirectedReason*)((ulong)(nint)pStateInfo + 1568uL);
		_volumeRedirectedReason = *(CsvVolumeRedirectedReason*)((ulong)(nint)pStateInfo + 1576uL);
		_volumeState = *(CsvState*)((ulong)(nint)pStateInfo + 1040uL);
	}
}
