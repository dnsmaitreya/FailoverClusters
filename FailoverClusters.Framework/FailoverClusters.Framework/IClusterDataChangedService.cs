using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal interface IClusterDataChangedService
{
	event EventHandler<ClusterDataChangedEventArgs> ClustersDataChanged;

	void NotifyClustersDataChanged(PCluster cluster, ClusterDataChangedType clusterDataChangedType);
}

