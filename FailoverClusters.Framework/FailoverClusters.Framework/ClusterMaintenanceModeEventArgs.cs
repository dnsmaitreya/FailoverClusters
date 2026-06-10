using System;

namespace FailoverClusters.Framework;

public class ClusterMaintenanceModeEventArgs : ClusterEventArgs
{
	public bool MaintenanceMode { get; internal set; }

	public ClusterObject Sender { get; internal set; }

	public ClusterMaintenanceModeEventArgs(Guid id, bool maintenanceMode, ClusterException exception)
		: base(id, exception)
	{
		MaintenanceMode = maintenanceMode;
	}

	public ClusterMaintenanceModeEventArgs(ClusterObject clusterObject, bool maintenanceMode, ClusterException exception)
		: base((clusterObject == null) ? Guid.Empty : clusterObject.Id, exception)
	{
		Sender = clusterObject;
		MaintenanceMode = maintenanceMode;
	}
}

