namespace FailoverClusters.Framework;

public class ObservableVolumeCollection : ObservableKeyCollection<MsftVolumeInfo>
{
	public ObservableVolumeCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.VolumeInfo)
	{
	}

	public ObservableVolumeCollection(object owner, Cluster cluster, ObservableCollectionFilter<MsftVolumeInfo> filter)
		: base(owner, cluster, ObservableCollectionItem.VolumeInfo, filter)
	{
	}

	public static ObservableVolumeCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservableVolumeCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<MsftVolumeInfo> filter)
	{
		return (ObservableVolumeCollection)ObservableKeyCollection<MsftVolumeInfo>.GetAssociationInstance(owner, cluster, () => new ObservableVolumeCollection(owner, cluster, filter), association);
	}

	public static ObservableVolumeCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservableVolumeCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<MsftVolumeInfo> filter)
	{
		return (ObservableVolumeCollection)ObservableKeyCollection<MsftVolumeInfo>.GetCollectionInstance(owner, cluster, () => new ObservableVolumeCollection(owner, cluster, filter));
	}
}

