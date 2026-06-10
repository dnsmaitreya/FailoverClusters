using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public interface IClusterDataService
{
	event EventHandler<ClusterDataChangedEventArgs> ClustersDataChanged;

	IEnumerable<Cluster> GetClusters();
}

