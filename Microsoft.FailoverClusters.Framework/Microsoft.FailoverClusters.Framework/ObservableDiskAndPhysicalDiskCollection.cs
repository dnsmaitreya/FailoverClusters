namespace Microsoft.FailoverClusters.Framework;

public class ObservableDiskAndPhysicalDiskCollection : ObservableKeyCollection<DiskInfoBase>
{
	public ObservableDiskAndPhysicalDiskCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.DiskAndPhysicalDisk)
	{
	}

	public ObservableDiskAndPhysicalDiskCollection(object owner, Cluster cluster, ObservableCollectionFilter<DiskInfoBase> filter)
		: base(owner, cluster, ObservableCollectionItem.DiskAndPhysicalDisk, filter)
	{
	}

	public static ObservableDiskAndPhysicalDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservableDiskAndPhysicalDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<DiskInfoBase> filter)
	{
		return (ObservableDiskAndPhysicalDiskCollection)ObservableKeyCollection<DiskInfoBase>.GetAssociationInstance(owner, cluster, () => new ObservableDiskAndPhysicalDiskCollection(owner, cluster, filter), association);
	}

	public static ObservableDiskAndPhysicalDiskCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservableDiskAndPhysicalDiskCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<DiskInfoBase> filter)
	{
		return (ObservableDiskAndPhysicalDiskCollection)ObservableKeyCollection<DiskInfoBase>.GetCollectionInstance(owner, cluster, () => new ObservableDiskAndPhysicalDiskCollection(owner, cluster, filter));
	}
}
