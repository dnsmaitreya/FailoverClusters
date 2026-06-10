using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

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

