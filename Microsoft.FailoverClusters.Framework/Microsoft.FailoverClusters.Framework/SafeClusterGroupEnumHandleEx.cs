using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterGroupEnumHandleEx : SafeHandleZeroIsInvalid
{
	public SafeClusterGroupEnumHandleEx()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterGroupEnumHandleEx(IntPtr clusterGroupOpenEnumHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterGroupOpenEnumHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterGroupCloseEnumEx(handle));
	}
}
