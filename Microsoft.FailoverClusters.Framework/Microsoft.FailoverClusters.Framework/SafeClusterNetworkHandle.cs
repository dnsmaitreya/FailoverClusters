using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterNetworkHandle : SafeHandleZeroIsInvalid
{
	public static SafeClusterNetworkHandle InvalidHandle => new SafeClusterNetworkHandle(IntPtr.Zero);

	public SafeClusterNetworkHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterNetworkHandle(IntPtr clusterNetworkHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterNetworkHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.CloseClusterNetwork(handle);
	}
}
