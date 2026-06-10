namespace FailoverClusters.Framework;

public class ObservableDiskPartitionCollection : ObservableKeyCollection<MsftPartitionInfo>
{
	public ObservableDiskPartitionCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.DiskPartition)
	{
	}

	public ObservableDiskPartitionCollection(object owner, Cluster cluster, ObservableCollectionFilter<MsftPartitionInfo> filter)
		: base(owner, cluster, ObservableCollectionItem.DiskPartition, filter)
	{
	}

	public static ObservableDiskPartitionCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservableDiskPartitionCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<MsftPartitionInfo> filter)
	{
		return (ObservableDiskPartitionCollection)ObservableKeyCollection<MsftPartitionInfo>.GetAssociationInstance(owner, cluster, () => new ObservableDiskPartitionCollection(owner, cluster, filter), association);
	}

	public static ObservableDiskPartitionCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservableDiskPartitionCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<MsftPartitionInfo> filter)
	{
		return (ObservableDiskPartitionCollection)ObservableKeyCollection<MsftPartitionInfo>.GetCollectionInstance(owner, cluster, () => new ObservableDiskPartitionCollection(owner, cluster, filter));
	}
}

