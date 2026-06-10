using System;
using System.Collections.Generic;
using System.Linq;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class PCsvVolumeResource : PStorageResource
{
	public PCsvVolumeResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.ClusterFileSystem, cluster.GetResourceType("Physical Disk")))
	{
	}

	internal void SetCsvRedirectedAccess(Guid deviceId, bool csvRedirectedAccessMode)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.SetCsvRedirectedAccess(this, deviceId, csvRedirectedAccessMode);
			});
		});
	}

	public override void Delete()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				try
				{
					base.Cluster.Server.Resource.RemoveFromClusterSharedVolumes(this);
				}
				catch (ClusterResourceLockedException)
				{
					base.IsProcessing = false;
				}
			});
		});
	}

	public override void Offline(bool overrideLockState = false)
	{
		ProtectedScope(delegate
		{
			try
			{
				ClusterLog.LogVerbose(LogSubcategory.FxPrivateObject, "Offlining dependent resources for {0}", base.Name);
				ReleaseExecuteAndReacquire(delegate
				{
					base.Cluster.Server.Resource.OfflineDependents(base.Id, overrideLockState);
				});
			}
			catch (ClusterException exception)
			{
				ClusterLog.LogException(exception, "There was an error offlining dependents resources for {0}", base.Name);
				throw;
			}
			finally
			{
				ClusterLog.LogVerbose(LogSubcategory.FxPrivateObject, "Offlining resource {0}", base.Name);
				ReleaseExecuteAndReacquire(delegate
				{
					base.Cluster.Server.Resource.Offline(base.Id, overrideLockState);
				});
			}
		}, (ClusterException ex) => ex, resetIsProcessing: false);
		if (base.ResourceState == FailoverClusters.Framework.ResourceState.Offline)
		{
			base.IsProcessing = false;
		}
	}

	private bool DoesResourceDependOnThisSharedVolume(PResource resource)
	{
		if ((resource.LoadedSelection & 4) != 4)
		{
			resource.LoadObject(4);
		}
		return ((ClusterPropertyMultipleStrings)resource.Properties["DependsOnSharedVolumes"]).TypedValue?.Any((string id) => base.Id.ToString().Equals(id, StringComparison.OrdinalIgnoreCase)) ?? false;
	}

	internal void ResumeClusterSharedVolume()
	{
		SetMaintenanceMode(maintMode: false);
		if ((base.LoadedSelection & 1) != 1)
		{
			LoadObject(1);
		}
		if (base.ResourceState != FailoverClusters.Framework.ResourceState.Online)
		{
			Online();
			LoadObject(536870913);
			if (base.ResourceState != FailoverClusters.Framework.ResourceState.Online)
			{
				throw new ClusterSharedVolumeNotOnlineException(base.Name);
			}
		}
	}

	internal void SuspendClusterSharedVolume()
	{
		ClusterList<Resource> query = (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster.GetProxy())
		{
			Name = "Client Configuration Resources"
		}.Where((Resource r) => (int)r.ResourceType.ResourceKind == 22);
		foreach (PResource item in base.Cluster.Server.Select(query))
		{
			if (DoesResourceDependOnThisSharedVolume(item))
			{
				if ((item.LoadedSelection & 1) != 1)
				{
					item.LoadObject(1);
				}
				PGroup ownerGroup = item.OwnerGroup;
				if ((ownerGroup.LoadedSelection & 1) != 1)
				{
					ownerGroup.LoadObject(1);
				}
				if (ownerGroup.GroupState == GroupState.Online || ownerGroup.GroupState == GroupState.PartialOnline)
				{
					ownerGroup.Offline();
				}
			}
		}
		SetMaintenanceMode(maintMode: true);
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> result = base.ProcessNotification(notification);
		if (notification.Payload is ClusterResourceSharedVolumeStateEventArgs)
		{
			base.LoadedSelection &= -257;
			ClusterStoragePropertiesChangedEventArgs forceLoadArgs = new ClusterStoragePropertiesChangedEventArgs(base.Id, null, reload: true, null);
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceStoragePropertiesChanged, forceLoadArgs));
			});
		}
		return result;
	}

	public CsvVolumeInformation GetVolumeInformation()
	{
		return ProtectedScope(() => ReleaseExecuteAndReacquire(() => base.Cluster.Server.Resource.GetCsvVolumeInformation(this)));
	}
}

