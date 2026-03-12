using System.Collections.Generic;
using System.Globalization;

namespace MS.Internal.ServerClusters.Management;

internal class EventIdComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return first.EventId - second.EventId;
	}

	public string Format(EventLogEvent e)
	{
		return e.EventId.ToString(CultureInfo.CurrentCulture);
	}
}
