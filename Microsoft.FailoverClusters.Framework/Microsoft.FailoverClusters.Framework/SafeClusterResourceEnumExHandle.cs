using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterResourceEnumExHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterResourceEnumExHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterResourceEnumExHandle(IntPtr clusterResourceOpenEnumHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterResourceOpenEnumHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterResourceCloseEnumEx(handle));
	}
}
