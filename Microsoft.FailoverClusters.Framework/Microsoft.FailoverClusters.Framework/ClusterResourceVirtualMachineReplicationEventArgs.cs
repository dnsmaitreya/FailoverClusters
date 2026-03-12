using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceVirtualMachineReplicationEventArgs : ClusterEventArgs
{
	public VirtualMachineReplicationData ReplicationData { get; internal set; }

	public ClusterResourceVirtualMachineReplicationEventArgs(Guid id, VirtualMachineReplicationData replicationData, ClusterException exception)
		: base(id, exception)
	{
		ReplicationData = replicationData;
	}
}
