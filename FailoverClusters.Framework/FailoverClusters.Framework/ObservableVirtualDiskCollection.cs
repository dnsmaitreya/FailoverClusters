namespace FailoverClusters.Framework;

public class ObservableVirtualDiskCollection : ObservableKeyCollection<VirtualDiskInfo>
{
	public ObservableVirtualDiskCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.PhysicalDisk)
	{
	}

	public ObservableVirtualDiskCollection(object owner, Cluster cluster, ObservableCollectionFilter<VirtualDiskInfo> filter)
		: base(owner, cluster, ObservableCollectionItem.VirtualDisk, filter)
	{
	}

	public static ObservableVirtualDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservableVirtualDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<VirtualDiskInfo> filter)
	{
		return (ObservableVirtualDiskCollection)ObservableKeyCollection<VirtualDiskInfo>.GetAssociationInstance(owner, cluster, () => new ObservableVirtualDiskCollection(owner, cluster, filter), association);
	}

	public static ObservableVirtualDiskCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservableVirtualDiskCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<VirtualDiskInfo> filter)
	{
		return (ObservableVirtualDiskCollection)ObservableKeyCollection<VirtualDiskInfo>.GetCollectionInstance(owner, cluster, () => new ObservableVirtualDiskCollection(owner, cluster, filter));
	}
}

