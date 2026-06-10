namespace MS.Internal.ServerClusters;

public interface IHasQuorumResource
{
	ClusterResource QuorumResource { get; }
}
