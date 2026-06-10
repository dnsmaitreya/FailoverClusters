namespace KDDSL.FailoverClusters.Framework;

internal class OrderByItem
{
	public ClusterObjectMetaDataMember DataMember { get; set; }

	public OrderDirection Direction { get; set; }

	internal int OrderIndex { get; set; }
}
