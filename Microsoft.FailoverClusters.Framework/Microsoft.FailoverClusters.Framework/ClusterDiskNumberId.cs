using System.Globalization;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterDiskNumberId : ClusterDiskId
{
	private readonly ulong number;

	public override DiskIdType IdType => DiskIdType.Number;

	public ulong Number => number;

	internal ClusterDiskNumberId(ulong number)
	{
		this.number = number;
	}

	public override string ToString()
	{
		return number.ToString(CultureInfo.InvariantCulture);
	}

	public override int GetHashCode()
	{
		return (int)number;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskNumberId clusterDiskNumberId))
		{
			return false;
		}
		return number.Equals(clusterDiskNumberId.number);
	}

	internal override NativeMethods.CLUSDSK_DISKID GetClusDiskId()
	{
		NativeMethods.CLUSDSK_DISKID result = default(NativeMethods.CLUSDSK_DISKID);
		result.DiskIdType = DiskIdType.Number;
		result.DeviceNumber = number;
		return result;
	}
}
