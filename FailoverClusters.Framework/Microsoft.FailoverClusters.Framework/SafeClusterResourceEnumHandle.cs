using System;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterResourceEnumHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterResourceEnumHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterResourceEnumHandle(IntPtr clusterResourceOpenEnumHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterResourceOpenEnumHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterResourceCloseEnum(handle));
	}
}

