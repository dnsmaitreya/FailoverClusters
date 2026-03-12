using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class SafeClusterNotifyPortHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterNotifyPortHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterNotifyPortHandle(IntPtr clusterNotificationPortHandle)
		: base(ownsHandle: true)
	{
		SetHandle(clusterNotificationPortHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.CloseClusterNotifyPort(handle);
	}
}
