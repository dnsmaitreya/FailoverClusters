namespace FailoverClusters.Framework;

public class ObservableDiskCollection : ObservableKeyCollection<MsftDiskInfo>
{
	public ObservableDiskCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.Disk)
	{
	}

	public ObservableDiskCollection(object owner, Cluster cluster, ObservableCollectionFilter<MsftDiskInfo> filter)
		: base(owner, cluster, ObservableCollectionItem.Disk, filter)
	{
	}

	public static ObservableDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservableDiskCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<MsftDiskInfo> filter)
	{
		return (ObservableDiskCollection)ObservableKeyCollection<MsftDiskInfo>.GetAssociationInstance(owner, cluster, () => new ObservableDiskCollection(owner, cluster, filter), association);
	}

	public static ObservableDiskCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservableDiskCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<MsftDiskInfo> filter)
	{
		return (ObservableDiskCollection)ObservableKeyCollection<MsftDiskInfo>.GetCollectionInstance(owner, cluster, () => new ObservableDiskCollection(owner, cluster, filter));
	}
}

