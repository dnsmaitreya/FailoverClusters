using System;
using System.Collections.Generic;
using FailoverClusters.Framework;

namespace KDDSL.FailoverClusters.Framework;

internal class PDistributedNetworkNameResource : PNetNameResource
{
	private ClusterPropertyMultipleStrings inUseNetworks;

	public List<NetworkInfo> ClusterNetworkInfos { get; internal set; }

	public PDistributedNetworkNameResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.DistributedNetworkName))
	{
		ClusterNetworkInfos = new List<NetworkInfo>();
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		ClusterLoadedEventArgs result = base.LoadObject(loadSelectionNeutral);
		if ((base.LoadedSelection & 4) == 4 && ((loadSelectionNeutral & 0x20000000) == 536870912 || inUseNetworks == null))
		{
			inUseNetworks = (ClusterPropertyMultipleStrings)base.Properties["InUseNetworks"];
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
		e.Properties.Get("ResourceSpecificStatus", delegate(ClusterPropertyString resourceSpecificStatusProperty)
		{
			ClusterInformationEventArgs payload = new ClusterInformationEventArgs(base.OwnerGroup.Id, resourceSpecificStatusProperty.TypedValue);
			base.Cluster.Server.EnqueueNotification(new GroupNotification(payload));
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Information, new ClusterInformationEventArgs(base.Id, resourceSpecificStatusProperty.TypedValue)));
			});
		});
		e.Properties.Get("InUseNetworks", delegate(ClusterPropertyMultipleStrings inUseNetworksProperty)
		{
			if (inUseNetworks != null && inUseNetworksProperty.TypedValue.Count != inUseNetworks.TypedValue.Count)
			{
				inUseNetworks = inUseNetworksProperty;
				LoadObject(536870917);
				ClusterNetworkChangedEventArgs eventArgs = new ClusterNetworkChangedEventArgs(base.Id, ClusterNetworkInfos, null);
				base.ExecuteOnReaderCallbacks.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.NetworkInterfacesChanged, eventArgs));
				});
			}
		});
		base.OnPropertiesChanged(sender, e);
	}
}

