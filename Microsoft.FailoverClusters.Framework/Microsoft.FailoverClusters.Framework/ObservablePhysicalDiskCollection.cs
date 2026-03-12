namespace Microsoft.FailoverClusters.Framework;

public class ObservablePhysicalDiskCollection : ObservableKeyCollection<PhysicalDiskInfo>
{
	public ObservablePhysicalDiskCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.PhysicalDisk)
	{
	}

	public ObservablePhysicalDiskCollection(object owner, Cluster cluster, ObservableCollectionFilter<PhysicalDiskInfo> filter)
		: base(owner, cluster, ObservableCollectionItem.PhysicalDisk, filter)
	{
	}

	public static ObservablePhysicalDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservablePhysicalDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<PhysicalDiskInfo> filter)
	{
		return (ObservablePhysicalDiskCollection)ObservableKeyCollection<PhysicalDiskInfo>.GetAssociationInstance(owner, cluster, () => new ObservablePhysicalDiskCollection(owner, cluster, filter), association);
	}

	public static ObservablePhysicalDiskCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservablePhysicalDiskCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<PhysicalDiskInfo> filter)
	{
		return (ObservablePhysicalDiskCollection)ObservableKeyCollection<PhysicalDiskInfo>.GetCollectionInstance(owner, cluster, () => new ObservablePhysicalDiskCollection(owner, cluster, filter));
	}
}
