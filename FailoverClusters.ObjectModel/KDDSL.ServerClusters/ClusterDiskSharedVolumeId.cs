using System;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class ClusterDiskSharedVolumeId : ClusterDiskId
{
	private string sharedVolumeId;

	public override DiskIdType IdType => DiskIdType.SharedVolume;

	public override string ToString()
	{
		return sharedVolumeId;
	}

	public override int GetHashCode()
	{
		return sharedVolumeId.GetHashCode();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskSharedVolumeId clusterDiskSharedVolumeId))
		{
			return false;
		}
		return string.Compare(sharedVolumeId, clusterDiskSharedVolumeId.sharedVolumeId, StringComparison.OrdinalIgnoreCase) == 0;
	}

	internal ClusterDiskSharedVolumeId(string sharedVolumeId)
	{
		this.sharedVolumeId = sharedVolumeId;
	}

	internal unsafe override _CLUSDSK_DISKID* GetClusDiskId(_CLUSDSK_DISKID* P_0)
	{
		//Discarded unreachable code: IL_0006
		//IL_0010: Expected I, but got I8
		throw new NotSupportedException();
	}
}
