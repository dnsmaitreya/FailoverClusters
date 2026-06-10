using System;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterValueListHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterValueListHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterValueListHandle(IntPtr clusterValueListHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterValueListHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.DestroyValueList(handle));
	}
}

