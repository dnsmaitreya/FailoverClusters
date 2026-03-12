using System;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class NotificationV2
{
	private int notifyKey;

	public NotificationDataV2 NotificationData { get; set; }

	public int NotifyKey => notifyKey;

	public NotificationV2()
	{
		NotificationData = new NotificationDataV2();
	}

	public override string ToString()
	{
		return string.Concat(NotificationData, "\t", notifyKey);
	}

	internal int GetNextNotification(SafeClusterNotifyPortHandle notificationPort, int timeout)
	{
		return NotificationData.GetNext(notificationPort, timeout, out notifyKey);
	}

	internal int GetNextV1Notification(IntPtr intPtr, int timeout)
	{
		return NotificationData.GetNextV1(intPtr, timeout, out notifyKey);
	}
}
