using System.Collections.Generic;
using System.Globalization;

namespace MS.Internal.ServerClusters.Management;

internal class KeywordComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return string.Compare(first.Keywords, second.Keywords, ignoreCase: true, CultureInfo.CurrentCulture);
	}

	public string Format(EventLogEvent e)
	{
		return e.Keywords;
	}
}
