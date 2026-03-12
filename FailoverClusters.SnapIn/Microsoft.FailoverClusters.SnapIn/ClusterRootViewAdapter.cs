using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.UIFramework;

namespace Microsoft.FailoverClusters.SnapIn;

internal class ClusterRootViewAdapter : WpfViewAdapterBase<ClusterRootViewModel>
{
	protected override ClusterRootViewModel InitializeAndCreateViewModel(ViewModelData viewModelData)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		return new ClusterRootViewModel(viewModelData.ViewCommandsProvider, (IScopeNodeNavigationCommandsProvider)(object)new ScopeNodeNavigationCommandsProvider(this));
	}
}
