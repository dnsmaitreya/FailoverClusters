using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterNetworkEnumHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterNetworkEnumHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterNetworkEnumHandle(IntPtr clusterNetworkOpenEnumHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterNetworkOpenEnumHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterNetworkCloseEnum(handle));
	}
}
