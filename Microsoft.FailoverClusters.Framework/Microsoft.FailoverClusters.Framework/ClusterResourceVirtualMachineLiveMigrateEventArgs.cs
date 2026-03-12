using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterResourceVirtualMachineLiveMigrateEventArgs : ClusterEventArgs
{
	public VirtualMachineMigrateState MigrationState { get; internal set; }

	public ClusterResourceVirtualMachineLiveMigrateEventArgs(Guid id, VirtualMachineMigrateState migrationState, ClusterException exception)
		: base(id, exception)
	{
		MigrationState = migrationState;
	}
}
