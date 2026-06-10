using System;

namespace MS.Internal.ServerClusters;

public class NotificationEventArgs : EventArgs
{
	private int m_queueSize;

	private int notificationsDiscarded;

	private int m_key;

	private string m_name;

	private string m_filter;

	public string Filter => m_filter;

	public string Name => m_name;

	public int Key => m_key;

	public int NotificationsDiscarded => notificationsDiscarded;

	public int QueueSize => m_queueSize;

	public NotificationEventArgs(int queueSize, int notificationsDiscarded, int key, string name, string filter)
	{
		m_queueSize = queueSize;
		this.notificationsDiscarded = notificationsDiscarded;
		m_key = key;
		m_name = name;
		m_filter = filter;
	}
}
