namespace FailoverClusters.Framework;

public class ObservablePhysicallyConnectedStorageNodeCollection : ObservableKeyCollection<StorageNode>
{
	public ObservablePhysicallyConnectedStorageNodeCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.PhysicallyConnectedStorageNode)
	{
	}

	public ObservablePhysicallyConnectedStorageNodeCollection(object owner, Cluster cluster, ObservableCollectionFilter<StorageNode> filter)
		: base(owner, cluster, ObservableCollectionItem.PhysicallyConnectedStorageNode, filter)
	{
	}

	public static ObservablePhysicallyConnectedStorageNodeCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservablePhysicallyConnectedStorageNodeCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<StorageNode> filter)
	{
		return (ObservablePhysicallyConnectedStorageNodeCollection)ObservableKeyCollection<StorageNode>.GetAssociationInstance(owner, cluster, () => new ObservablePhysicallyConnectedStorageNodeCollection(owner, cluster, filter), association);
	}

	public static ObservablePhysicallyConnectedStorageNodeCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservablePhysicallyConnectedStorageNodeCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<StorageNode> filter)
	{
		return (ObservablePhysicallyConnectedStorageNodeCollection)ObservableKeyCollection<StorageNode>.GetCollectionInstance(owner, cluster, () => new ObservablePhysicallyConnectedStorageNodeCollection(owner, cluster));
	}
}

