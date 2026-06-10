using System;

namespace KDDSL.ServerClusters;

public class ClusterResourceEventArgs : EventArgs
{
	private string resourceName;

	private ClusterResource resource;

	public ClusterResource Resource => resource;

	public string ResourceName => resourceName;

	internal ClusterResourceEventArgs(string resourceName, ClusterResource resource)
	{
		this.resourceName = resourceName;
		this.resource = resource;
	}
}
