using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UIFramework;

namespace FailoverClusters.ClusterSnapIn;

internal class ClusterNodesViewAdapter : ClusterGridViewAdapterBase<ClusterNodesViewModel>
{
	protected override ClusterNodesViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		return new ClusterNodesViewModel(cluster);
	}
}

