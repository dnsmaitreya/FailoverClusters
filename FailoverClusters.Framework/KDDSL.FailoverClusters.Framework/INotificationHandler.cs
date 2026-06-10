using System;

namespace KDDSL.FailoverClusters.Framework;

internal interface INotificationHandler
{
	bool NotificationArrived(NativeMethods.NOTIFY_FILTER_AND_TYPE filterType, string objectName, string objectId, string parentId, string objectType, IntPtr buffer, int bufferSize);
}
