using System.Globalization;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterDiskSignatureId : ClusterDiskId
{
	private readonly ulong signature;

	public override DiskIdType IdType => DiskIdType.Signature;

	public ulong Signature => signature;

	internal ClusterDiskSignatureId(ulong signature)
	{
		this.signature = signature;
	}

	public override string ToString()
	{
		return signature.ToString(CultureInfo.InvariantCulture);
	}

	public override int GetHashCode()
	{
		return (int)signature;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ClusterDiskSignatureId clusterDiskSignatureId))
		{
			return false;
		}
		return signature.Equals(clusterDiskSignatureId.signature);
	}

	internal override NativeMethods.CLUSDSK_DISKID GetClusDiskId()
	{
		NativeMethods.CLUSDSK_DISKID result = default(NativeMethods.CLUSDSK_DISKID);
		result.DiskIdType = DiskIdType.Signature;
		result.DiskSignature = signature;
		return result;
	}
}

