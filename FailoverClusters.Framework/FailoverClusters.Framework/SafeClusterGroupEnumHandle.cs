using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterGroupEnumHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterGroupEnumHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterGroupEnumHandle(IntPtr clusterGroupOpenEnumHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterGroupOpenEnumHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterGroupCloseEnum(handle));
	}
}

