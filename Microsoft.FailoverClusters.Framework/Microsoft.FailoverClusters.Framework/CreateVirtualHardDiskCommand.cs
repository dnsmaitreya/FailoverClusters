namespace Microsoft.FailoverClusters.Framework;

public class CreateVirtualHardDiskCommand : WeakLazyBase<ClusterCommand>
{
	private readonly Cluster cluster;

	public CreateVirtualHardDiskCommand(Cluster cluster)
	{
		this.cluster = cluster;
	}

	protected override ClusterCommand GenerateInstance()
	{
		return new ClusterCommand(null, "Create Virtual HardDisk", ClusterCommandId.CreateVirtualHardDisk)
		{
			InputParameters = cluster.AllUpNodes
		};
	}
}
