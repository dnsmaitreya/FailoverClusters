using System.Collections.Generic;
using System.Globalization;

namespace KDDSL.ServerClusters.Management;

internal class TaskComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return string.Compare(first.Task, second.Task, ignoreCase: true, CultureInfo.CurrentCulture);
	}

	public string Format(EventLogEvent e)
	{
		return e.Task;
	}
}
