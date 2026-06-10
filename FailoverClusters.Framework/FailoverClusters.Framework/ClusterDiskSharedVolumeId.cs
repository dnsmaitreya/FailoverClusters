using System;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterDiskSharedVolumeId : ClusterDiskId
{
	private readonly string sharedVolumeId;

	public override DiskIdType IdType => DiskIdType.SharedVolume;

	internal ClusterDiskSharedVolumeId(string sharedVolumeId)
	{
		this.sharedVolumeId = sharedVolumeId;
	}

	public override string ToString()
	{
		return sharedVolumeId;
	}

	public override int GetHashCode()
	{
		return sharedVolumeId.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskSharedVolumeId clusterDiskSharedVolumeId))
		{
			return false;
		}
		return string.Compare(sharedVolumeId, clusterDiskSharedVolumeId.sharedVolumeId, StringComparison.OrdinalIgnoreCase) == 0;
	}

	internal override NativeMethods.CLUSDSK_DISKID GetClusDiskId()
	{
		throw new NotSupportedException();
	}
}

