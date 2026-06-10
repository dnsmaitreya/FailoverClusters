using System;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterPropertyListHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterPropertyListHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterPropertyListHandle(IntPtr clusterPropertyListHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterPropertyListHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.DestroyPropList(handle));
	}
}

