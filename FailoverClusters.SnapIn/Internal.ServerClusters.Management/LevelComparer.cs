using System.Collections.Generic;

namespace MS.Internal.ServerClusters.Management;

internal class LevelComparer : IComparer<EventLogEvent>
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return first.Level - second.Level;
	}
}
