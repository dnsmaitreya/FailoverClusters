using System;

namespace MS.Internal.ServerClusters;

public class ClusterGroupEventArgs : EventArgs
{
	private string groupName;

	private ClusterGroup group;

	public ClusterGroup Group => group;

	public string GroupName => groupName;

	internal ClusterGroupEventArgs(string groupName, ClusterGroup group)
	{
		this.groupName = groupName;
		this.group = group;
	}
}
