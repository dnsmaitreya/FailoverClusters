using System.Collections.Generic;
using System.Globalization;

namespace MS.Internal.ServerClusters.Management;

internal class ChannelComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return string.Compare(first.Channel, second.Channel, ignoreCase: true, CultureInfo.CurrentCulture);
	}

	public string Format(EventLogEvent e)
	{
		return e.Channel;
	}
}
