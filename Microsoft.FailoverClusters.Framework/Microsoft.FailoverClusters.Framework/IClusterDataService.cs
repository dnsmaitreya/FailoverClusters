using System;
using System.Collections.Generic;

namespace Microsoft.FailoverClusters.Framework;

public interface IClusterDataService
{
	event EventHandler<ClusterDataChangedEventArgs> ClustersDataChanged;

	IEnumerable<Cluster> GetClusters();
}
