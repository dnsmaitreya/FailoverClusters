using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class EventListViewItem : ListViewItem
{
	private EventLogEvent logEvent;

	public EventLogEvent Event
	{
		get
		{
			return logEvent;
		}
		set
		{
			logEvent = value;
		}
	}

	public EventListViewItem(string text)
		: base(text)
	{
		logEvent = null;
	}
}
