namespace KDDSL.ServerClusters.Management;

internal interface IViewContext
{
	ClusterContext ClusterContext { get; }

	string[] DisplayColumns { get; }

	string EmptyMessage { get; }

	bool IsEnumerating { get; }
}
