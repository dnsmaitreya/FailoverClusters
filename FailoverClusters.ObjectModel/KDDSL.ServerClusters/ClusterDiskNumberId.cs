using System.Globalization;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class ClusterDiskNumberId : ClusterDiskId
{
	private uint m_number;

	public uint Number => m_number;

	public override DiskIdType IdType => DiskIdType.Number;

	public override string ToString()
	{
		uint number = m_number;
		return number.ToString(CultureInfo.InvariantCulture);
	}

	public override int GetHashCode()
	{
		return (int)m_number;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskNumberId clusterDiskNumberId))
		{
			return false;
		}
		uint number = m_number;
		return number.Equals(clusterDiskNumberId.m_number);
	}

	internal ClusterDiskNumberId(uint number)
	{
		m_number = number;
	}

	internal unsafe override _CLUSDSK_DISKID* GetClusDiskId(_CLUSDSK_DISKID* P_0)
	{
		*(int*)P_0 = 4000;
		*(uint*)((ulong)(nint)P_0 + 4uL) = m_number;
		return P_0;
	}
}
