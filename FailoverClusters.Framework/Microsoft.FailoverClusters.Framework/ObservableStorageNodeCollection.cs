namespace FailoverClusters.Framework;

public class ObservableStorageNodeCollection : ObservableKeyCollection<StorageNode>
{
	public ObservableStorageNodeCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.StorageNode)
	{
	}

	public ObservableStorageNodeCollection(object owner, Cluster cluster, ObservableCollectionFilter<StorageNode> filter)
		: base(owner, cluster, ObservableCollectionItem.StorageNode, filter)
	{
	}

	public static ObservableStorageNodeCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservableStorageNodeCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<StorageNode> filter)
	{
		return (ObservableStorageNodeCollection)ObservableKeyCollection<StorageNode>.GetAssociationInstance(owner, cluster, () => new ObservableStorageNodeCollection(owner, cluster, filter), association);
	}

	public static ObservableStorageNodeCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservableStorageNodeCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<StorageNode> filter)
	{
		return (ObservableStorageNodeCollection)ObservableKeyCollection<StorageNode>.GetCollectionInstance(owner, cluster, () => new ObservableStorageNodeCollection(owner, cluster));
	}
}

