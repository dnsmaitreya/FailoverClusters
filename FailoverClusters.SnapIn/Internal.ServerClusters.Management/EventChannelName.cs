namespace MS.Internal.ServerClusters.Management;

internal class EventChannelName
{
	private string displayName;

	private string pathName;

	public string DisplayName => displayName;

	public string PathName => pathName;

	public EventChannelName(string displayName, string pathName)
	{
		this.displayName = displayName;
		this.pathName = pathName;
	}

	public override string ToString()
	{
		return DisplayName;
	}
}
