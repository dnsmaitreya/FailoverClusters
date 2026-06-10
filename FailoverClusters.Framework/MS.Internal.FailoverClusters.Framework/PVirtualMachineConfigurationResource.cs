using System;
using System.Collections.ObjectModel;
using System.Linq;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class PVirtualMachineConfigurationResource : PResource
{
	private ReadOnlyCollection<string> csvDisks;

	private ulong? lastOperationStatusCode;

	public PVirtualMachineConfigurationResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.VirtualMachineConfiguration))
	{
	}

	public override bool IsChildResource()
	{
		if (base.Dependents == null)
		{
			LoadObject(16);
			base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
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
					goto end_IL_0053;
				}
				pResource = (PResource)base.Cluster.CacheManager.AddObject(pResource);
			}
			if (pResource.ResourceType.ResourceKind == ResourceKind.VirtualMachine)
			{
				return true;
			}
			end_IL_0053:;
		}
		return false;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		int loadedSelection = base.LoadedSelection;
		ClusterLoadedEventArgs result = base.LoadObject(loadSelectionNeutral);
		int num = loadedSelection ^ base.LoadedSelection;
		if ((base.LoadedSelection & 4) == 4 && csvDisks == null)
		{
			csvDisks = new ReadOnlyCollection<string>(((ClusterPropertyMultipleStrings)base.Properties["DependsOnSharedVolumes"]).TypedValue);
		}
		if ((num & 2) == 2 && base.OwnerGroup != null)
		{
			ClusterPropertiesEventArgs forwardedPayload = new ClusterPropertiesEventArgs(base.Id, base.Name, (int)base.ResourceType.ResourceKind, null)
			{
				Properties = base.Properties
			};
			base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, forwardedPayload, 3)));
		}
		return result;
	}

	protected override void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
		if ((base.LoadedSelection & 1) != 1)
		{
			LoadObject(1);
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
			});
		}
		e.Properties.Get("DependsOnSharedVolumes", delegate(ClusterPropertyMultipleStrings csvVolumes)
		{
			if (csvDisks != null)
			{
				bool flag = true;
				if (csvDisks.All((string item) => csvVolumes.TypedValue.Contains(item)) && csvVolumes.TypedValue.All((string item) => csvDisks.Contains(item)))
				{
					flag = false;
				}
				if (flag)
				{
					csvDisks = csvVolumes.TypedValue;
					base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, e, 1)));
				}
			}
		});
		e.Properties.Get("LastOperationStatusCode", delegate(ClusterPropertyULong lastOperationStatusCodeProperty)
		{
			if (lastOperationStatusCodeProperty.TypedValue != lastOperationStatusCode)
			{
				lastOperationStatusCode = lastOperationStatusCodeProperty.TypedValue;
				base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, e, 3)));
			}
		});
		base.OnPropertiesChanged(sender, e);
	}
}

