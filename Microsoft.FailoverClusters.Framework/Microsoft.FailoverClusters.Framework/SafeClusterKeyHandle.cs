using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterKeyHandle : SafeHandleZeroIsInvalid
{
	public static SafeClusterKeyHandle InvalidHandle => new SafeClusterKeyHandle(IntPtr.Zero);

	public SafeClusterKeyHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterKeyHandle(IntPtr clusterKeyHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterKeyHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterRegCloseKey(handle));
	}
}
