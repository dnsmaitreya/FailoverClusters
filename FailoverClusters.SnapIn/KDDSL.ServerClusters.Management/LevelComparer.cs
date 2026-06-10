using System.Collections.Generic;

namespace KDDSL.ServerClusters.Management;

internal class LevelComparer : IComparer<EventLogEvent>
{
	public int Compare(EventLogEvent first, EventLogEvent second)
	{
		return first.Level - second.Level;
	}
}
