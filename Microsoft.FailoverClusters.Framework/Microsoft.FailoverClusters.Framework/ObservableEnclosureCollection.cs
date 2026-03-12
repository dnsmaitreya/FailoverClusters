namespace Microsoft.FailoverClusters.Framework;

public class ObservableEnclosureCollection : ObservableKeyCollection<Enclosure>
{
	public ObservableEnclosureCollection(object owner, Cluster cluster)
		: base(owner, cluster, ObservableCollectionItem.Enclosures)
	{
	}

	public ObservableEnclosureCollection(object owner, Cluster cluster, ObservableCollectionFilter<Enclosure> filter)
		: base(owner, cluster, ObservableCollectionItem.Enclosures, filter)
	{
	}

	public static ObservableEnclosureCollection GetAssociation<T>(object owner, Cluster cluster, T association)
	{
		return GetAssociation(owner, cluster, association, null);
	}

	public static ObservableEnclosureCollection GetAssociation<T>(object owner, Cluster cluster, T association, ObservableCollectionFilter<Enclosure> filter)
	{
		return (ObservableEnclosureCollection)ObservableKeyCollection<Enclosure>.GetAssociationInstance(owner, cluster, () => new ObservableEnclosureCollection(owner, cluster, filter), association);
	}

	public static ObservableEnclosureCollection GetInstanceFrom(object owner, Cluster cluster)
	{
		return GetInstanceFrom(owner, cluster, null);
	}

	public static ObservableEnclosureCollection GetInstanceFrom(object owner, Cluster cluster, ObservableCollectionFilter<Enclosure> filter)
	{
		return (ObservableEnclosureCollection)ObservableKeyCollection<Enclosure>.GetCollectionInstance(owner, cluster, () => new ObservableEnclosureCollection(owner, cluster, filter));
	}
}
