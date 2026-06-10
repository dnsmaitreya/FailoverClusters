namespace KDDSL.ServerClusters;

public interface IHasQuorumResource
{
	ClusterResource QuorumResource { get; }
}
