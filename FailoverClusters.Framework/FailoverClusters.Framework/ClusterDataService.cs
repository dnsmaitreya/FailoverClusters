using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class ClusterDataService : IClusterDataService, IClusterDataChangedService
{
	public event EventHandler<ClusterDataChangedEventArgs> ClustersDataChanged;

	public IEnumerable<Cluster> GetClusters()
	{
		List<Cluster> list = new List<Cluster>();
		IEnumerable<PCluster> clusters = CacheManager.GetClusters();
		list.AddRange(clusters.Select((PCluster pc) => pc.GetProxy()));
		return new ReadOnlyCollection<Cluster>(list);
	}

	public void NotifyClustersDataChanged(PCluster privateCluster, ClusterDataChangedType clusterDataChangedType)
	{
		EventHandler<ClusterDataChangedEventArgs> eventHandler = this.ClustersDataChanged;
		if (eventHandler != null)
		{
			ClusterDataChangedEventArgs eventArgs = ((clusterDataChangedType == ClusterDataChangedType.Addition) ? new ClusterDataChangedEventArgs(privateCluster.GetProxy()) : new ClusterDataChangedEventArgs(privateCluster.CacheId));
			UIHelper.ExecuteOnDispatcher(delegate
			{
				eventHandler(this, eventArgs);
			}, OperationType.Async);
		}
	}
}

