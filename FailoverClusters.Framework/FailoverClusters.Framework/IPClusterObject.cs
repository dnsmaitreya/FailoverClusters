using System;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal interface IPClusterObject<out T> where T : ClusterObject
{
	Guid Id { get; set; }

	string Name { get; set; }

	PCluster Cluster { get; }

	int LoadedSelection { get; set; }

	ClusterIdentityType IdentityType { get; }

	ClusterLoadedEventArgs LoadObject(int loadSelection);

	T GetProxy();

	T GetProxy(ProxyCreateMode createMode);
}

