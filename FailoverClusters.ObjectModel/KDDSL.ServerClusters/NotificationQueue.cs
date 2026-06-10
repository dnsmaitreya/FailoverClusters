using System;
using System.Collections.Generic;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class NotificationQueue
{
	private AutoResetEvent m_event;

	private object m_lockObject;

	private LinkedList<NotificationPayload> m_notifications;

	internal WaitHandle WaitHandle => m_event;

	internal int Count => m_notifications.Count;

	private void EnterLock()
	{
		Monitor.Enter(m_lockObject);
	}

	private void LeaveLock()
	{
		Monitor.Exit(m_lockObject);
	}

	internal NotificationQueue()
	{
		m_event = new AutoResetEvent(initialState: false);
		m_lockObject = new object();
		m_notifications = new LinkedList<NotificationPayload>();
	}

	internal void Enqueue(NotificationPayload payload)
	{
		Monitor.Enter(m_lockObject);
		try
		{
			m_notifications.AddLast(payload);
			m_event.Set();
		}
		finally
		{
			Monitor.Exit(m_lockObject);
		}
	}

	internal void Enqueue(ulong dwNotifyKey, string name, uint dwFilterType)
	{
		NotificationPayload payload = new NotificationPayload(dwNotifyKey, name, dwFilterType);
		Enqueue(payload);
	}

	internal NotificationPayload Dequeue(ref int notificationsDiscarded)
	{
		NotificationPayload notificationPayload = null;
		Monitor.Enter(m_lockObject);
		try
		{
			if (m_notifications.Count > 0)
			{
				notificationPayload = m_notifications.First.Value;
				m_notifications.RemoveFirst();
				notificationsDiscarded = 0;
				while (m_notifications.Count > 0)
				{
					NotificationPayload value = m_notifications.First.Value;
					if (!string.Equals(notificationPayload.Name, value.Name, StringComparison.CurrentCultureIgnoreCase) || notificationPayload.NotifyKey != value.NotifyKey || notificationPayload.FilterType != value.FilterType)
					{
						break;
					}
					notificationPayload = m_notifications.First.Value;
					m_notifications.RemoveFirst();
					notificationsDiscarded++;
				}
				if (m_notifications.Count > 0)
				{
					m_event.Set();
				}
			}
		}
		finally
		{
			Monitor.Exit(m_lockObject);
		}
		return notificationPayload;
	}
}
