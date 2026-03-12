using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public enum ObservableCollectionItem
{
	Enclosures,
	Disk,
	PhysicalDisk,
	DiskAndPhysicalDisk,
	VirtualDisk,
	DiskPartition,
	VolumeInfo,
	StorageNode,
	PhysicallyConnectedStorageNode
}
public abstract class ObservableCollectionItem<T> : IKeyQueryable<T>, IKeyQueryable, IDataItem, INotifyPropertyChanged where T : IKeyQueryable
{
	public Cluster Cluster { get; private set; }

	public abstract string Key { get; }

	public Guid Id => FormatHelper.UIntToGuid(FormatHelper.StringHash(Key));

	public abstract IEnumerable<ICommand> Commands { get; }

	public abstract string DisplayName { get; }

	public event PropertyChangedEventHandler PropertyChanged;

	protected ObservableCollectionItem(ClusterObject owner)
	{
		Cluster = owner.Cluster;
	}

	public abstract void CopyFrom(T source);

	protected void OnPropertyChanged(string propertyName)
	{
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged != null)
		{
			UIHelper.ExecuteOnDispatcher(delegate
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}, OperationType.Async);
		}
	}

	protected static void GetInstance<T1>(Cluster cluster, string key, Action<OperationResult<T1>> operationResult, string serverName = null) where T1 : IKeyQueryable
	{
		Worker.Start(delegate
		{
			T1 returnValue = default(T1);
			cluster.ExecuteMethod(delegate(ILockable lockObject)
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				if (pCluster != null)
				{
					IConnectionAdapterStorage instanceAdapter = GetInstanceAdapter(pCluster);
					returnValue = instanceAdapter.GetInstance<T1>(key, serverName);
				}
			}, LockAccess.Reader, setErrorOnObject: false);
			operationResult.SafeCall(new OperationResult<T1>(cluster, returnValue, null));
		}, delegate(ClusterException exception)
		{
			operationResult.SafeCall(new OperationResult<T1>(cluster, default(T1), exception));
		});
	}

	internal static IConnectionAdapterStorage GetInstanceAdapter(PCluster cluster)
	{
		return cluster.Server.Storage;
	}
}
