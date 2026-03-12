using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceVirtualMachineReplicationHealthEventArgs : ClusterEventArgs
{
	public VirtualMachineReplicationHealth ReplicationHealth { get; internal set; }

	public ClusterResourceVirtualMachineReplicationHealthEventArgs(Guid id, VirtualMachineReplicationHealth virtualMachineReplicationHealth)
		: base(id, null)
	{
		ReplicationHealth = virtualMachineReplicationHealth;
	}
}
