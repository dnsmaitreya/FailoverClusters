using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterGroupHandle : SafeHandleZeroIsInvalid
{
	public static SafeClusterGroupHandle InvalidHandle => new SafeClusterGroupHandle(IntPtr.Zero);

	public SafeClusterGroupHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterGroupHandle(IntPtr clusterNetworkHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterNetworkHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.CloseClusterGroup(handle);
	}
}

