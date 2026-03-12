using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceVirtualMachineStorageSummaryEventArgs : ClusterEventArgs
{
	public VirtualMachineStorageInformation StorageSummaryInformation { get; private set; }

	public ClusterResourceVirtualMachineStorageSummaryEventArgs(Guid id, VirtualMachineStorageInformation storageSummaryInformation, ClusterException exception)
		: base(id, exception)
	{
		StorageSummaryInformation = storageSummaryInformation;
	}
}
