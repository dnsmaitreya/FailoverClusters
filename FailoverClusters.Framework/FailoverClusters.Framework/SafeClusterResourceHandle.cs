using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterResourceHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterResourceHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterResourceHandle(IntPtr clusterNetworkHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterNetworkHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.CloseClusterResource(handle);
	}
}

