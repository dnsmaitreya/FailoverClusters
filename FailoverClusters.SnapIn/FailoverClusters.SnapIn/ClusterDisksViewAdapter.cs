using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UIFramework;

namespace FailoverClusters.SnapIn;

internal class ClusterDisksViewAdapter : ClusterGridViewAdapterBase<ClusterDisksViewModel>
{
	protected override ClusterDisksViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		return new ClusterDisksViewModel(cluster);
	}
}

