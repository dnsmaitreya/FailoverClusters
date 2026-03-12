using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualMachineCheckpointInformation
{
	private readonly ObservableCollection<Checkpoint> checkpoints;

	public ObservableCollection<Checkpoint> Checkpoints => checkpoints;

	public bool IsEmpty
	{
		get
		{
			if (Checkpoints != null)
			{
				return Checkpoints.Count == 0;
			}
			return false;
		}
	}

	internal VirtualMachineCheckpointInformation()
	{
		checkpoints = new ObservableCollection<Checkpoint>();
	}

	internal VirtualMachineCheckpointInformation(IEnumerable<Checkpoint> checkpoints)
	{
		if (!checkpoints.All((Checkpoint checkpoint) => checkpoint.IsCurrentVirtualMachine))
		{
			this.checkpoints = new ObservableCollection<Checkpoint>(checkpoints.OrderByDescending((Checkpoint checkpoint) => checkpoint.CreationTime));
		}
		else
		{
			this.checkpoints = new ObservableCollection<Checkpoint>();
		}
	}
}
