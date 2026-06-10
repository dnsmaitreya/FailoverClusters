using System;

namespace MS.Internal.ServerClusters;

public class EventLogEventLoggedEventArgs : EventArgs
{
	private EventLogEvent m_eventLogEvent;

	public EventLogEvent Event => m_eventLogEvent;

	public EventLogEventLoggedEventArgs(EventLogEvent eventLogEvent)
	{
		m_eventLogEvent = eventLogEvent;
	}
}
