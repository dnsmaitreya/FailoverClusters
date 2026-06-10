using System;

namespace KDDSL.ServerClusters.Management;

internal class ClusterDatabaseNode
{
	private readonly Guid instanceId;

	private readonly string name;

	private readonly string fqdnName;

	public Guid InstanceId => instanceId;

	public string FqdnName => fqdnName;

	public string Name => name;

	internal ClusterDatabaseNode(Guid instanceId, string name, string domain)
	{
		this.instanceId = instanceId;
		this.name = name;
		fqdnName = NetworkHelp.BuildFqdn(name, domain);
	}
}
