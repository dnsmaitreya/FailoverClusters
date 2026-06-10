using System.Collections.Generic;
using System.Globalization;

namespace KDDSL.ServerClusters.Management;

internal class UserComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return string.Compare(first.User, second.User, ignoreCase: true, CultureInfo.CurrentCulture);
	}

	public string Format(EventLogEvent e)
	{
		return e.User;
	}
}
