using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class MoveVirtualMachineStorageMapping
{
	public string RemoteClusterSharedVolumesRootPath { get; private set; }

	public string LocalSharedVolumesRootPath { get; private set; }

	public MoveVirtualMachineStorageMapping(Cluster cluster)
	{
		Exceptions.ThrowIfNull(cluster, "cluster");
		cluster.Properties.Get("SharedVolumesRoot", delegate(ClusterPropertyString property)
		{
			LocalSharedVolumesRootPath = property.TypedValue;
		});
		RemoteClusterSharedVolumesRootPath = "\\\\{0}\\{1}".FormatCurrentCulture(cluster.FullyQualifiedDomainName, "ClusterStorage$");
	}
}
