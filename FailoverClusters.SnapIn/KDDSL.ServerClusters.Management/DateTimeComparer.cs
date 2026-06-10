using System;
using System.Collections.Generic;
using System.Globalization;

namespace KDDSL.ServerClusters.Management;

internal class DateTimeComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return DateTime.Compare(first.TimeCreated, second.TimeCreated);
	}

	public string Format(EventLogEvent e)
	{
		return e.TimeCreated.ToString("G", CultureInfo.CurrentCulture);
	}
}
