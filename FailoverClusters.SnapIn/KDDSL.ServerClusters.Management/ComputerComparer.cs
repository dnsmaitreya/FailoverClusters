using System.Collections.Generic;
using System.Globalization;

namespace KDDSL.ServerClusters.Management;

internal class ComputerComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return string.Compare(first.Computer, second.Computer, ignoreCase: true, CultureInfo.CurrentCulture);
	}

	public string Format(EventLogEvent e)
	{
		return e.Computer;
	}
}
