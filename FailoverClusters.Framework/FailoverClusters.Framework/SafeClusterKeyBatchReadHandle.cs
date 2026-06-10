using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterKeyBatchReadHandle : SafeHandleZeroIsInvalid
{
	private SafeClusterKeyBatchReplyHandle replyHandle;

	public SafeClusterKeyBatchReplyHandle ReplyHandle => replyHandle;

	public SafeClusterKeyBatchReadHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterKeyBatchReadHandle(IntPtr registryReadBatchHandle)
		: base(ownsHandle: true)
	{
		SetHandle(registryReadBatchHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterRegCloseReadBatch(handle, out replyHandle));
	}
}

