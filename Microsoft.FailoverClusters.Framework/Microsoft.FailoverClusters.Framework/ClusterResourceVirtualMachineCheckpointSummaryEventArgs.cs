using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceVirtualMachineCheckpointSummaryEventArgs : ClusterEventArgs
{
	public VirtualMachineCheckpointInformation CheckpointSummaryInformation { get; private set; }

	public ClusterResourceVirtualMachineCheckpointSummaryEventArgs(Guid id, VirtualMachineCheckpointInformation checkpointSummaryInformation, ClusterException exception)
		: base(id, exception)
	{
		CheckpointSummaryInformation = checkpointSummaryInformation;
	}
}
