using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UIFramework;

namespace Microsoft.FailoverClusters.SnapIn;

internal class ClusterDisksViewAdapter : ClusterGridViewAdapterBase<ClusterDisksViewModel>
{
	protected override ClusterDisksViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		return new ClusterDisksViewModel(cluster);
	}
}
