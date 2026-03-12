using System;
using System.Management;
using Microsoft.FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal class VirtualMachineReplicationInformation
{
	internal static class ReplicationTaskId
	{
		public const int InitialReplication = 94;

		public const int ImportInitialReplication = 95;

		public const int InitiateFailover = 97;

		public const int InitiateSyncedReplication = 100;

		public const int SendingDelta = 105;

		public const int ReceivingDelta = 106;

		public const int Resynchronization = 107;

		public const int CheckConsistency = 113;

		public const int SendingInitialReplica = 116;

		public const int StartReplicationResync = 117;

		public const int StartReplicationExport = 118;

		public const int ApplyDelta = 120;

		public const int ExtendedResynchronization = 121;

		public const int InitiateFailback = 123;

		public const int FirstNetworkJobType = 130;

		public const int LastNetworkJobType = 139;
	}

	internal static class ReplicationTaskState
	{
		public const int New = 2;

		public const int Starting = 3;

		public const int Running = 4;

		public const int Suspended = 5;

		public const int ShuttingDown = 6;

		public const int Completed = 7;

		public const int Terminated = 8;

		public const int Killed = 9;

		public const int Exception = 10;

		public const int CompletedWithWarnings = 32768;
	}

	private static class WmiPropertyNames
	{
		public const string PrimaryServerName = "PrimaryHostSystem";

		public const string ReoveryServerName = "RecoveryHostSystem";

		public const string LastReplicaTime = "LastReplicationTime";

		public const string PrimaryConnectionPoint = "PrimaryConnectionPoint";

		public const string RecoveryConnectionPoint = "RecoveryConnectionPoint";
	}

	internal string PrimaryServerName { get; set; }

	internal string PrimaryConnectionPoint { get; set; }

	internal string RecoveryServerName { get; set; }

	internal string RecoveryConnectionPoint { get; set; }

	internal DateTime? LastReplicaTime { get; set; }

	internal int ReplicationTaskProgress { get; set; }

	internal string ReplicationTaskName { get; set; }

	internal ReplicationRelationshipType RelationshipType { get; set; }

	internal VirtualMachineReplicationInformation(ManagementObject virtualComputerReplicationRelationship, ManagementObject virtualComputerReplicationSettings)
	{
		DateTime? lastReplicaTime = null;
		object propertyValueOrDefault = virtualComputerReplicationSettings.GetPropertyValueOrDefault("PrimaryHostSystem");
		PrimaryServerName = ((propertyValueOrDefault != null) ? ((string)propertyValueOrDefault) : string.Empty);
		propertyValueOrDefault = virtualComputerReplicationSettings.GetPropertyValueOrDefault("RecoveryHostSystem");
		RecoveryServerName = ((propertyValueOrDefault != null) ? ((string)propertyValueOrDefault) : string.Empty);
		propertyValueOrDefault = virtualComputerReplicationSettings.GetPropertyValueOrDefault("PrimaryConnectionPoint");
		PrimaryConnectionPoint = ((propertyValueOrDefault != null) ? ((string)propertyValueOrDefault) : string.Empty);
		propertyValueOrDefault = virtualComputerReplicationSettings.GetPropertyValueOrDefault("RecoveryConnectionPoint");
		RecoveryConnectionPoint = ((propertyValueOrDefault != null) ? ((string)propertyValueOrDefault) : string.Empty);
		try
		{
			propertyValueOrDefault = virtualComputerReplicationRelationship.GetPropertyValueOrDefault("LastReplicationTime");
			if (propertyValueOrDefault != null)
			{
				lastReplicaTime = ManagementDateTimeConverter.ToDateTime((string)propertyValueOrDefault);
			}
		}
		catch (ManagementException ex)
		{
			if (ex.ErrorCode != ManagementStatus.NotFound)
			{
				throw;
			}
		}
		LastReplicaTime = lastReplicaTime;
	}
}
