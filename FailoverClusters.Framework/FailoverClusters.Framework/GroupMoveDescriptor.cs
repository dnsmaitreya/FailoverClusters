namespace FailoverClusters.Framework;

internal struct GroupMoveDescriptor
{
	public Group OwnerGroup { get; set; }

	public ClusterObject ClusterObjectIssuingMoveRequest { get; set; }
}

