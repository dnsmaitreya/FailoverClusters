namespace FailoverClusters.Framework;

internal class WmiVMUtilities
{
	public static string GetFailureStatusDescription(VirtualMachineComputerSystemHealthState healthState, VirtualMachineComputerSystemOperationalStatus[] operationalStatus, string[] statusDescriptions)
	{
		string result = string.Empty;
		if (healthState != VirtualMachineComputerSystemHealthState.Ok && IsValidOperationalStatus(operationalStatus) && IsValidStatusDescriptions(statusDescriptions) && operationalStatus[0].GetOperationStatus() != VirtualMachineComputerSystemOperationalStatus.Ok)
		{
			result = statusDescriptions[0];
		}
		return result;
	}

	internal static bool IsVMComputerSystemStateValid(VirtualMachineState state)
	{
		if (state != VirtualMachineState.Migrating && state != VirtualMachineState.Paused && state != VirtualMachineState.PowerOff && state != VirtualMachineState.Reset && state != VirtualMachineState.Resuming && state != VirtualMachineState.Running && state != VirtualMachineState.Saved && state != VirtualMachineState.Saving && state != VirtualMachineState.ShuttingDown && state != VirtualMachineState.SnapshotInProgress && state != VirtualMachineState.Starting && state != VirtualMachineState.Stopping)
		{
			return state == VirtualMachineState.Pausing;
		}
		return true;
	}

	internal static bool IsVMComputerSystemHealthStateValid(VirtualMachineComputerSystemHealthState healthState)
	{
		if (healthState != VirtualMachineComputerSystemHealthState.Ok && healthState != VirtualMachineComputerSystemHealthState.MajorFailure)
		{
			return healthState == VirtualMachineComputerSystemHealthState.CriticalFailure;
		}
		return true;
	}

	internal static bool IsVMComputerSystemOperationalStatusValid(VirtualMachineComputerSystemOperationalStatus operationalStatus)
	{
		if (operationalStatus != VirtualMachineComputerSystemOperationalStatus.ApplyingSnapshot && operationalStatus != VirtualMachineComputerSystemOperationalStatus.CreatingSnapshot && operationalStatus != VirtualMachineComputerSystemOperationalStatus.Degraded && operationalStatus != VirtualMachineComputerSystemOperationalStatus.DeletingSnapshot && operationalStatus != VirtualMachineComputerSystemOperationalStatus.ExportingVirtualMachine && operationalStatus != VirtualMachineComputerSystemOperationalStatus.InService && operationalStatus != VirtualMachineComputerSystemOperationalStatus.MergingDisks && operationalStatus != VirtualMachineComputerSystemOperationalStatus.MigratingVirtualMachine && operationalStatus != VirtualMachineComputerSystemOperationalStatus.Ok && operationalStatus != VirtualMachineComputerSystemOperationalStatus.PredictiveFailure && operationalStatus != VirtualMachineComputerSystemOperationalStatus.WaitingToStart && operationalStatus != VirtualMachineComputerSystemOperationalStatus.BackingUpVirtualMachine && operationalStatus != VirtualMachineComputerSystemOperationalStatus.ModifyingUpVirtualMachine && operationalStatus != VirtualMachineComputerSystemOperationalStatus.StorageMigrationPhaseOne)
		{
			return operationalStatus == VirtualMachineComputerSystemOperationalStatus.StorageMigrationPhaseTwo;
		}
		return true;
	}

	internal static VirtualMachineComputerSystemOperationalStatus[] ConvertOperationalStatus(ushort[] operationalStatusValues)
	{
		if (operationalStatusValues == null)
		{
			return null;
		}
		VirtualMachineComputerSystemOperationalStatus[] array = new VirtualMachineComputerSystemOperationalStatus[operationalStatusValues.Length];
		for (int i = 0; i < operationalStatusValues.Length; i++)
		{
			if (operationalStatusValues[i] == 10 || operationalStatusValues[i] == 15)
			{
				array[i] = VirtualMachineComputerSystemOperationalStatus.Ok;
			}
			else
			{
				array[i] = (VirtualMachineComputerSystemOperationalStatus)operationalStatusValues[i];
			}
			IsVMComputerSystemOperationalStatusValid(array[i]);
		}
		return array;
	}

	internal static VirtualMachineComputerSystemOperationalStatus GetMigrationStatus(ClusterPropertyULong status)
	{
		return (VirtualMachineComputerSystemOperationalStatus)(status.TypedValue >> 32);
	}

	internal static int GetMigrationProgress(ClusterPropertyULong status)
	{
		return (int)(status.TypedValue & 0xFF);
	}

	private static bool IsValidOperationalStatus(VirtualMachineComputerSystemOperationalStatus[] operationalStatus)
	{
		if (operationalStatus == null || operationalStatus.Length == 0)
		{
			return false;
		}
		return true;
	}

	private static bool IsValidStatusDescriptions(string[] statusDescriptions)
	{
		if (statusDescriptions == null || statusDescriptions.Length == 0 || string.IsNullOrEmpty(statusDescriptions[0]))
		{
			return false;
		}
		return true;
	}
}

