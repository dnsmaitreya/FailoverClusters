using System;

namespace MS.Internal.ServerClusters;

public class ClusterRegistryEventArgs : EventArgs
{
	private string registryName;

	private ClusterRegistryChangeType type;

	public ClusterRegistryChangeType RegistryChangeType => type;

	public string RegistryName => registryName;

	internal ClusterRegistryEventArgs(string registryName, ClusterRegistryChangeType type)
	{
		this.registryName = registryName;
		this.type = type;
	}
}
