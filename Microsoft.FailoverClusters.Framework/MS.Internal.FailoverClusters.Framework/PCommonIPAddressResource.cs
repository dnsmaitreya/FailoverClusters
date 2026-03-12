using System;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class PCommonIPAddressResource : PResource
{
	protected PCommonIPAddressResource(PCluster cluster, Guid id, string name, PResourceType resourceType)
		: base(cluster, id, name, resourceType)
	{
	}

	public void Renew()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.Renew(this);
			});
		});
	}

	public void Release()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.Release(this);
			});
		});
	}

	public override bool IsChildResource()
	{
		if (base.Dependents == null)
		{
			LoadObject(16);
			base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
		}
		if (base.Dependents == null)
		{
			return false;
		}
		foreach (Guid dependent in base.Dependents)
		{
			using ClusterLock clusterLock = base.Cluster.CacheManager.Get(dependent, ClusterIdentityType.Resource, LockAccess.Reader);
			PResource pResource = null;
			if (clusterLock != null)
			{
				pResource = (PResource)clusterLock.Owner;
			}
			if (pResource == null)
			{
				try
				{
					pResource = base.Cluster.Server.Resource.Open(dependent);
				}
				catch (ClusterObjectNotFoundException exception)
				{
					ClusterLog.LogException(LogLevel.Info, exception, "Dependent resource '{0}' not found in the cluster".FormatCurrentCulture(dependent));
					goto end_IL_005d;
				}
				pResource = (PResource)base.Cluster.CacheManager.AddObject(pResource);
			}
			if (pResource.ResourceType.ResourceKind == ResourceKind.NetworkName || pResource.ResourceType.ResourceKind == ResourceKind.DistributedNetworkName)
			{
				return true;
			}
			end_IL_005d:;
		}
		return false;
	}
}
