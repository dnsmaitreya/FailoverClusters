using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UIFramework;

namespace Microsoft.FailoverClusters.SnapIn;

internal class ClusterOverviewViewAdapter : ClusterGridViewAdapterBase<ClusterOverviewViewModel>
{
	protected override ClusterOverviewViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		return new ClusterOverviewViewModel(cluster, viewModelData.ViewCommandsProvider, (IScopeNodeNavigationCommandsProvider)(object)new ScopeNodeNavigationCommandsProvider(this));
	}
}
