using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

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
