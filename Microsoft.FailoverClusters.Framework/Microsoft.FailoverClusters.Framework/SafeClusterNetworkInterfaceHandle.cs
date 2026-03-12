using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterNetworkInterfaceHandle : SafeHandleZeroIsInvalid
{
	public static SafeClusterNetworkInterfaceHandle InvalidHandle => new SafeClusterNetworkInterfaceHandle(IntPtr.Zero);

	public SafeClusterNetworkInterfaceHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterNetworkInterfaceHandle(IntPtr clusterNetworkInterfaceHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterNetworkInterfaceHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.CloseClusterNetInterface(handle);
	}
}
