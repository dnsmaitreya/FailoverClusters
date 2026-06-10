using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class VirtualMachineReplicationData
{
	public string PrimaryServerName { get; set; }

	public string PrimaryConnectionPoint { get; set; }

	public string RecoveryServerName { get; set; }

	public string RecoveryConnectionPoint { get; set; }

	public DateTime? LastReplicaTime { get; set; }

	public int ReplicationTaskProgress { get; set; }

	public string ReplicationTaskName { get; set; }

	internal ReplicationRelationshipType RelationshipType { get; set; }

	public VirtualMachineReplicationMode ReplicationMode { get; set; }

	internal bool IsTestFailoverRunning { get; set; }

	internal bool IsPlannedFailover { get; set; }

	internal bool IsInitialReplicationPending { get; set; }

	internal bool IsEndpointProviderUsed { get; set; }

	public VirtualMachineReplicationState ReplicationState { get; set; }

	public VirtualMachineReplicationHealth ReplicationHealth { get; set; }
}

