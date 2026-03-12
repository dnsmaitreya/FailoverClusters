using System;
using System.ComponentModel;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterableStoragePool : INotifyPropertyChanged
{
	private Exception error;

	private ClusterableStoragePoolAddOperationState operationState;

	public Guid PoolId { get; set; }

	public string DisplayName { get; set; }

	public StoragePoolHealth Health { get; set; }

	public StoragePoolQuorum Quorum { get; set; }

	public ulong TotalCapacity { get; set; }

	public string DriveIds { get; set; }

	public ulong ConsumedCapacity { get; set; }

	public ClusterableStoragePoolAddOperationState State
	{
		get
		{
			return operationState;
		}
		set
		{
			if (operationState != value)
			{
				operationState = value;
				NotifyPropertyChanged("State");
			}
		}
	}

	public Exception Error
	{
		get
		{
			return error;
		}
		set
		{
			error = value;
			NotifyPropertyChanged("Error");
		}
	}

	public Guid ContextId { get; private set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public ClusterableStoragePool(Guid contextId)
	{
		operationState = ClusterableStoragePoolAddOperationState.NotStarted;
		ContextId = contextId;
	}

	private void NotifyPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
