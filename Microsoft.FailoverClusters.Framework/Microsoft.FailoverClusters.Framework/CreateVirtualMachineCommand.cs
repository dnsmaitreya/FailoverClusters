namespace Microsoft.FailoverClusters.Framework;

public class CreateVirtualMachineCommand : WeakLazyBase<ClusterCommand>
{
	private readonly Cluster cluster;

	public CreateVirtualMachineCommand(Cluster cluster)
	{
		this.cluster = cluster;
	}

	protected override ClusterCommand GenerateInstance()
	{
		return new ClusterCommand(null, "Create Virtual Machine", ClusterCommandId.CreateVirtualMachine)
		{
			InputParameters = cluster.AllUpNodes
		};
	}
}
