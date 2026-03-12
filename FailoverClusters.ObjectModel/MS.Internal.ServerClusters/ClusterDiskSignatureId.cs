using System.Globalization;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class ClusterDiskSignatureId : ClusterDiskId
{
	private uint m_signature;

	public uint Signature => m_signature;

	public override DiskIdType IdType => DiskIdType.Signature;

	public override string ToString()
	{
		uint signature = m_signature;
		return signature.ToString(CultureInfo.InvariantCulture);
	}

	public override int GetHashCode()
	{
		return (int)m_signature;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskSignatureId clusterDiskSignatureId))
		{
			return false;
		}
		uint signature = m_signature;
		return signature.Equals(clusterDiskSignatureId.m_signature);
	}

	internal ClusterDiskSignatureId(uint signature)
	{
		m_signature = signature;
	}

	internal unsafe override _CLUSDSK_DISKID* GetClusDiskId(_CLUSDSK_DISKID* P_0)
	{
		*(int*)P_0 = 0;
		*(uint*)((ulong)(nint)P_0 + 4uL) = m_signature;
		return P_0;
	}
}
