using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UIFramework;

namespace FailoverClusters.ClusterSnapIn;

internal class ClusterNetworksViewAdapter : ClusterGridViewAdapterBase<ClusterNetworksViewModel>
{
	protected override ClusterNetworksViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		return new ClusterNetworksViewModel(cluster);
	}
}

