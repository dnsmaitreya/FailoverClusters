using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class ClusterDiskGuidId : ClusterDiskId
{
	private Guid m_guid;

	public Guid DiskGuid => m_guid;

	public override DiskIdType IdType => DiskIdType.Guid;

	public override string ToString()
	{
		return m_guid.ToString("B");
	}

	public override int GetHashCode()
	{
		return m_guid.GetHashCode();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskGuidId clusterDiskGuidId))
		{
			return false;
		}
		return m_guid.Equals(clusterDiskGuidId.m_guid);
	}

	internal ClusterDiskGuidId(Guid guid)
	{
		m_guid = guid;
	}

	internal unsafe override _CLUSDSK_DISKID* GetClusDiskId(_CLUSDSK_DISKID* P_0)
	{
		*(int*)P_0 = 1;
		fixed (byte* ptr = &m_guid.ToByteArray()[0])
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned((long)(nint)P_0 + 4L, ptr, 16);
			return P_0;
		}
	}
}
