using System.Collections.Generic;
using System.Globalization;

namespace KDDSL.ServerClusters.Management;

internal class OpCodeComparer : IComparer<EventLogEvent>, IFormatter
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return string.Compare(first.OpCode, second.OpCode, ignoreCase: true, CultureInfo.CurrentCulture);
	}

	public string Format(EventLogEvent e)
	{
		return e.OpCode;
	}
}
