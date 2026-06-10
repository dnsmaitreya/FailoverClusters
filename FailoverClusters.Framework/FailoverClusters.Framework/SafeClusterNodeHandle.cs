using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterNodeHandle : SafeHandleZeroIsInvalid
{
	public static SafeClusterNodeHandle InvalidHandle => new SafeClusterNodeHandle(IntPtr.Zero);

	public SafeClusterNodeHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterNodeHandle(IntPtr clusterNodeHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterNodeHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.CloseClusterNode(handle);
	}
}

