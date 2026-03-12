using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class CsvVolumeInformation
{
	private NativeMethods.CLUS_CSV_VOLUME_INFO csvVolumeInfo;

	public ulong VolumeOffset => csvVolumeInfo.VolumeOffset;

	public uint PartitionNumber => csvVolumeInfo.PartitionNumber;

	public CsvVolumeFaultState FaultState => (CsvVolumeFaultState)csvVolumeInfo.FaultState;

	public CsvVolumeBackupState BackupState => (CsvVolumeBackupState)csvVolumeInfo.BackupState;

	public string VolumeFriendlyName => csvVolumeInfo.szVolumeFriendlyName;

	public string VolumeName => csvVolumeInfo.szVolumeName;

	internal CsvVolumeInformation(NativeMethods.CLUS_CSV_VOLUME_INFO csvVolumeInfo)
	{
		this.csvVolumeInfo = csvVolumeInfo;
	}
}
