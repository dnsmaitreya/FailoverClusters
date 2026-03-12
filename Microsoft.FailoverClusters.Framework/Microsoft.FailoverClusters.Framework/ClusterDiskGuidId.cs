using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterDiskGuidId : ClusterDiskId
{
	private readonly Guid guid;

	public override DiskIdType IdType => DiskIdType.Guid;

	public Guid DiskGuid => guid;

	internal ClusterDiskGuidId(Guid guid)
	{
		this.guid = guid;
	}

	public override string ToString()
	{
		return guid.ToString("B");
	}

	public override int GetHashCode()
	{
		Guid guid = this.guid;
		return guid.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskGuidId clusterDiskGuidId))
		{
			return false;
		}
		return guid.Equals(clusterDiskGuidId.guid);
	}

	internal override NativeMethods.CLUSDSK_DISKID GetClusDiskId()
	{
		NativeMethods.CLUSDSK_DISKID result = default(NativeMethods.CLUSDSK_DISKID);
		result.DiskIdType = DiskIdType.Guid;
		result.DiskGuid = guid;
		return result;
	}
}
