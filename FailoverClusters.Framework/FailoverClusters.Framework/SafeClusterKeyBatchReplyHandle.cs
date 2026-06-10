using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterKeyBatchReplyHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterKeyBatchReplyHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterKeyBatchReplyHandle(IntPtr registryReadBatchHandle)
		: base(ownsHandle: true)
	{
		SetHandle(registryReadBatchHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterRegCloseReadBatchReply(handle));
	}
}

