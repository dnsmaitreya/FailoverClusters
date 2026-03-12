using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterEnumHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterEnumHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterEnumHandle(IntPtr clusterOpenEnumHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterOpenEnumHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterCloseEnumEx(handle));
	}
}
