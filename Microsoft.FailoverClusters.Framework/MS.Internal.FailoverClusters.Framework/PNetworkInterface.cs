using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class PNetworkInterface : PClusterObject<NetworkInterface>
{
	private NetworkInterface proxyNetworkInterface;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.NetworkInterface;

	[DefaultValue(null)]
	public NetworkInterfaceState? State { get; set; }

	public PNetworkInterface(PCluster cluster, Guid id, string name)
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
		throw new NotSupportedException();
	}

	public override NetworkInterface GetProxy()
	{
		return GetProxy(ProxyCreateMode.Singleton);
	}

	public override NetworkInterface GetProxy(ProxyCreateMode createMode)
	{
		if (createMode == ProxyCreateMode.Singleton && proxyNetworkInterface != null)
		{
			return proxyNetworkInterface;
		}
		NetworkInterface networkInterface = new NetworkInterface(base.Cluster.GetProxy());
		networkInterface.TransferInternalData(this, subscribeToEvents: true);
		if (createMode == ProxyCreateMode.Singleton)
		{
			proxyNetworkInterface = networkInterface;
		}
		return networkInterface;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		if ((base.LoadedSelection & loadSelectionNeutral) == loadSelectionNeutral)
		{
			return new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		NetworkInterfaceLoadSelection loadSelection = (NetworkInterfaceLoadSelection)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			base.Cluster.Server.NetworkInterface.Load(this, loadSelection);
		}, delegate(ClusterException ex)
		{
			if (ex == null)
			{
				int loadSelection2 = currentSelection ^ base.LoadedSelection;
				BroadcastChanges(loadSelection2);
			}
			loadedArgs = new ClusterLoadedEventArgs(base.Id, ex == null, base.LoadedSelection, ex);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, loadedArgs));
		});
		return loadedArgs;
	}

	public override void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false)
	{
		if (((NetworkLoadSelection)loadSelection).HasFlag(NetworkLoadSelection.Basic))
		{
			RouteEvent(new ClusterWrapperEventArgs(EventType.NetworkInterfaceStateChanged, new ClusterNetworkInterfaceStateEventArgs(base.Id, State, null)));
		}
		if (((NetworkLoadSelection)loadSelection).HasFlag(NetworkLoadSelection.CommonProperties) || ((NetworkLoadSelection)loadSelection).HasFlag(NetworkLoadSelection.PrivateProperties))
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
		PNetworkInterface sourceObject = source as PNetworkInterface;
		if (sourceObject == null)
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
		LockTransferAndBroadCast(source, loadSelection, delegate
		{
			NetworkInterfaceLoadSelection networkInterfaceLoadSelection = (NetworkInterfaceLoadSelection)loadSelection;
			if (networkInterfaceLoadSelection.HasFlag(NetworkInterfaceLoadSelection.Basic))
			{
				State = sourceObject.State;
				base.LoadedSelection |= 1;
			}
		});
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> list = base.ProcessNotification(notification);
		ClusterNetworkInterfaceStateEventArgs clusterNetworkInterfaceStateEventArgs = notification.Payload as ClusterNetworkInterfaceStateEventArgs;
		if (clusterNetworkInterfaceStateEventArgs != null)
		{
			State = clusterNetworkInterfaceStateEventArgs.State;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.NetworkInterfaceStateChanged, clusterNetworkInterfaceStateEventArgs));
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
					clusterRemovedEventArgs.Cluster.RealtimeCollections.Remove<NetworkInterface>(this);
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
			PNetworkInterface clusterObject = new PNetworkInterface(clusterAddedEventArgs.Cluster, clusterAddedEventArgs.Id, clusterAddedEventArgs.Name);
			clusterObject = (PNetworkInterface)clusterAddedEventArgs.Cluster.CacheManager.AddObject(clusterObject, clusterAddedEventArgs is ClusterUpsertEventArgs);
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
