using System;
using System.Collections.Generic;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class PNetwork : PClusterObject<Network>
{
	private Network proxyNetwork;

	public NetworkState? State { get; set; }

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Network;

	public PNetwork(PCluster cluster, Guid id, string name)
		: base(cluster)
	{
		base.Id = id;
		base.Name = name;
	}

	public override void Delete()
	{
		throw new NotSupportedException();
	}

	public override void Rename(string newName)
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Network.Rename(base.Id, newName);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, new ClusterRenamedEventArgs(base.Id, base.Name, ex)));
			}
		});
	}

	public override Network GetProxy()
	{
		return GetProxy(ProxyCreateMode.Singleton);
	}

	public override Network GetProxy(ProxyCreateMode createMode)
	{
		if (createMode == ProxyCreateMode.Singleton && proxyNetwork != null)
		{
			return proxyNetwork;
		}
		Network network = new Network(base.Cluster.GetProxy());
		network.TransferInternalData(this, subscribeToEvents: true);
		if (createMode == ProxyCreateMode.Singleton)
		{
			proxyNetwork = network;
		}
		return network;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		if ((base.LoadedSelection & loadSelectionNeutral) == loadSelectionNeutral)
		{
			return new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		NetworkLoadSelection loadSelection = (NetworkLoadSelection)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			base.Cluster.Server.Network.Load(this, loadSelection);
		}, delegate(ClusterException ex)
		{
			if (ex == null)
			{
				int loadSelection2 = currentSelection ^ base.LoadedSelection;
				BroadcastChanges(loadSelection2);
			}
			loadedArgs = new ClusterLoadedEventArgs(base.Id, ex == null, base.LoadedSelection, ex);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, loadedArgs));
		}, resetIsProcessing: true, affectsIsProcessing: false);
		return loadedArgs;
	}

	public override void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false)
	{
		if (((NetworkInterfaceLoadSelection)loadSelection).HasFlag(NetworkInterfaceLoadSelection.Basic))
		{
			RouteEvent(new ClusterWrapperEventArgs(EventType.NetworkStateChanged, new ClusterNetworkStateEventArgs(base.Id, State, null)));
		}
		if (((NetworkInterfaceLoadSelection)loadSelection).HasFlag(NetworkInterfaceLoadSelection.CommonProperties) || ((NetworkInterfaceLoadSelection)loadSelection).HasFlag(NetworkInterfaceLoadSelection.PrivateProperties))
		{
			RouteEvent(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
			{
				Properties = base.Properties
			}));
		}
		if (raiseLoadedEvent)
		{
			ClusterLoadedEventArgs eventArgs = new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, eventArgs));
		}
	}

	public override void TransferFrom(PClusterObject source, bool cacheIsLocked, int loadSelection)
	{
		PNetwork sourceObject = source as PNetwork;
		if (sourceObject == null)
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
		LockTransferAndBroadCast(source, loadSelection, delegate
		{
			NetworkLoadSelection networkLoadSelection = (NetworkLoadSelection)loadSelection;
			if (networkLoadSelection.HasFlag(NetworkLoadSelection.Basic))
			{
				State = sourceObject.State;
				base.LoadedSelection |= 1;
			}
		});
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> list = base.ProcessNotification(notification);
		ClusterNetworkStateEventArgs clusterNetworkStateEventArgs = notification.Payload as ClusterNetworkStateEventArgs;
		if (clusterNetworkStateEventArgs != null)
		{
			State = clusterNetworkStateEventArgs.State;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.NetworkStateChanged, clusterNetworkStateEventArgs));
				base.Cluster.RealtimeCollections.Change(this, "State");
			});
		}
		ClusterRenamedEventArgs clusterRenamedEventArgs = notification.Payload as ClusterRenamedEventArgs;
		if (clusterRenamedEventArgs != null)
		{
			base.Name = clusterRenamedEventArgs.NewName;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, clusterRenamedEventArgs));
				base.Cluster.RealtimeCollections.Change(this, "Name");
			});
		}
		ClusterRemovedEventArgs clusterRemovedEventArgs = notification.Payload as ClusterRemovedEventArgs;
		if (clusterRemovedEventArgs != null)
		{
			clusterRemovedEventArgs.Cluster.CacheManager.CacheLock.EnterWriteLock();
			try
			{
				base.IsRemoved = true;
				clusterRemovedEventArgs.Cluster.CacheManager.RemoveObject(this);
				list.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.Removed, notification.Payload));
					clusterRemovedEventArgs.Cluster.RealtimeCollections.Remove<Network>(this);
				});
			}
			finally
			{
				clusterRemovedEventArgs.Cluster.CacheManager.CacheLock.ExitWriteLock();
			}
		}
		return list;
	}

	public static bool ProcessNotificationSpecial(Notification notification)
	{
		if (notification.Payload is ClusterAddedEventArgs clusterAddedEventArgs)
		{
			PNetwork clusterObject = new PNetwork(clusterAddedEventArgs.Cluster, clusterAddedEventArgs.Id, clusterAddedEventArgs.Name);
			clusterObject = (PNetwork)clusterAddedEventArgs.Cluster.CacheManager.AddObject(clusterObject, clusterAddedEventArgs is ClusterUpsertEventArgs);
			clusterObject.LockObject.Reader();
			try
			{
				clusterAddedEventArgs.Cluster.RealtimeCollections.Add(clusterObject, (clusterAddedEventArgs is ClusterUpsertEventArgs) ? RTCOperation.Replace : RTCOperation.Add);
			}
			finally
			{
				clusterObject.LockObject.UnlockReader();
			}
			return true;
		}
		return false;
	}
}

