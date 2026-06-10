using System;
using System.Collections.Generic;
using System.ComponentModel;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class PNode : PClusterObject<Node>
{
	private Node proxyNode;

	private string fqdn = string.Empty;

	[DefaultValue(null)]
	public NodeState? State { get; set; }

	public NodeOperatingSystemInformation OperatingSystemInformation { get; private set; }

	public ServerInformation ServerInformation { get; private set; }

	public ProcessorInformation ProcessorInformation { get; private set; }

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Node;

	internal string Fqdn
	{
		get
		{
			if (string.IsNullOrWhiteSpace(fqdn) && NetworkHelper.CanPing(base.Name))
			{
				fqdn = WmiHelper.GetNodeFullyQualifiedDomainName(base.Name);
			}
			return fqdn;
		}
	}

	public PNode(PCluster cluster, Guid id, string name)
		: base(cluster)
	{
		base.Id = id;
		base.Name = name;
	}

	public void Start()
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Node.Start(base.Name);
		});
	}

	public void Stop()
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Node.Stop(base.Name);
		});
	}

	public void Pause(NodePauseDrainType drainType)
	{
		Pause(drainType, null);
	}

	public void Pause(NodePauseDrainType drainType, string targetNode)
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Node.Pause(base.Name, drainType, targetNode);
		});
	}

	public void Resume(NodeResumeFailbackType failbackType)
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Node.Resume(base.Name, failbackType);
		});
	}

	public bool WillOfflineLoseQuorum()
	{
		return ProtectedScope(() => base.Cluster.Server.Node.WillOfflineLoseQuorum(base.Name));
	}

	public bool WillEvictLoseQuorum()
	{
		return ProtectedScope(() => base.Cluster.Server.Node.WillEvictLoseQuorum(base.Name));
	}

	public override void Delete()
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Node.Delete(base.Id);
		});
	}

	public override Node GetProxy()
	{
		return GetProxy(ProxyCreateMode.Singleton);
	}

	public override Node GetProxy(ProxyCreateMode createMode)
	{
		if (createMode == ProxyCreateMode.Singleton && proxyNode != null)
		{
			return proxyNode;
		}
		Node node = new Node(base.Cluster.GetProxy());
		node.TransferInternalData(this, subscribeToEvents: true);
		if (createMode == ProxyCreateMode.Singleton)
		{
			proxyNode = node;
		}
		return node;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		if ((loadSelectionNeutral & 0x20000000) == 536870912)
		{
			loadSelectionNeutral ^= 0x20000000;
			base.LoadedSelection &= ~loadSelectionNeutral;
		}
		if ((base.LoadedSelection & loadSelectionNeutral) == loadSelectionNeutral)
		{
			return new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		NodeLoadSelection loadSelection = (NodeLoadSelection)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			base.Cluster.Server.Node.Load(this, loadSelection);
			if (loadSelection.HasFlag(NodeLoadSelection.OperatingSystemInformation))
			{
				try
				{
					OperatingSystemInformation = base.Cluster.Server.Node.GetOperatingSystemInformation(base.Name);
					base.LoadedSelection |= 0x1000;
				}
				catch (ClusterException innerException)
				{
					throw new ClusterNodeGetOperatingSystemInformationException(base.Name, innerException);
				}
			}
			if (loadSelection.HasFlag(NodeLoadSelection.ServerInformation))
			{
				try
				{
					ServerInformation = base.Cluster.Server.Node.GetServerInformation(base.Name);
					base.LoadedSelection |= 0x4000;
				}
				catch (ClusterException innerException2)
				{
					throw new ClusterNodeGetServerInformationException(base.Name, innerException2);
				}
			}
			if (loadSelection.HasFlag(NodeLoadSelection.ProcessorInformation))
			{
				try
				{
					ProcessorInformation = base.Cluster.Server.Node.GetProcessorInformation(base.Name);
					base.LoadedSelection |= 0x2000;
				}
				catch (ClusterException innerException3)
				{
					throw new ClusterNodeGetProcessorInformationException(base.Name, innerException3);
				}
			}
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
		List<ClusterWrapperEventArgs> list = new List<ClusterWrapperEventArgs>(10);
		if (((NodeLoadSelection)loadSelection).HasFlag(NodeLoadSelection.Basic))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.NodeStateChanged, new ClusterNodeStateEventArgs(base.Id, State, null)));
		}
		if (((NodeLoadSelection)loadSelection).HasFlag(NodeLoadSelection.CommonProperties) || ((NodeLoadSelection)loadSelection).HasFlag(NodeLoadSelection.PrivateProperties))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
			{
				Properties = base.Properties
			}));
		}
		if (((NodeLoadSelection)loadSelection).HasFlag(NodeLoadSelection.OperatingSystemInformation))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.NodeOperatingSystemInformationChanged, new ClusterNodeOperatingSystemInformationEventArgs(base.Id, OperatingSystemInformation, null)));
		}
		if (((NodeLoadSelection)loadSelection).HasFlag(NodeLoadSelection.ServerInformation))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.NodeServerInformationChanged, new ClusterNodeServerInformationEventArgs(base.Id, ServerInformation, null)));
		}
		if (((NodeLoadSelection)loadSelection).HasFlag(NodeLoadSelection.ProcessorInformation))
		{
			list.Add(new ClusterWrapperEventArgs(EventType.NodeProcessorInformationChanged, new ClusterNodeProcessorInformationEventArgs(base.Id, ProcessorInformation, null)));
		}
		if (raiseLoadedEvent)
		{
			ClusterLoadedEventArgs eventArgs = new ClusterLoadedEventArgs(base.Id, loaded: true, base.LoadedSelection, null);
			list.Add(new ClusterWrapperEventArgs(EventType.Loaded, eventArgs));
		}
		RouteEvent(new ClusterWrapperEventArgs(EventType.BatchChanges, new ClusterBatchChangesEventArgs(base.Id, list)));
	}

	public override void Rename(string newName)
	{
		ProtectedScope(delegate
		{
			base.Cluster.Server.Node.Rename(base.Id, newName);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Renamed, new ClusterRenamedEventArgs(base.Id, base.Name, ex)));
			}
		});
	}

	public override void TransferFrom(PClusterObject source, bool cacheIsLocked, int loadSelection)
	{
		PNode sourceObject = source as PNode;
		if (sourceObject == null)
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
		LockTransferAndBroadCast(source, loadSelection, delegate
		{
			NodeLoadSelection nodeLoadSelection = (NodeLoadSelection)loadSelection;
			if (nodeLoadSelection.HasFlag(NodeLoadSelection.Basic))
			{
				State = sourceObject.State;
				fqdn = sourceObject.fqdn;
				base.LoadedSelection |= 1;
			}
		});
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> list = base.ProcessNotification(notification);
		if (notification.Payload is ClusterNodeStateEventArgs)
		{
			ClusterNodeStateEventArgs args2 = (ClusterNodeStateEventArgs)notification.Payload;
			State = args2.State;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.NodeStateChanged, args2));
				base.Cluster.RealtimeCollections.Change(this, "State");
			});
		}
		else if (notification.Payload is ClusterRemovedEventArgs)
		{
			ClusterRemovedEventArgs args = (ClusterRemovedEventArgs)notification.Payload;
			args.Cluster.CacheManager.CacheLock.EnterWriteLock();
			try
			{
				base.IsRemoved = true;
				args.Cluster.CacheManager.RemoveObject(this);
				list.Add(delegate
				{
					RouteEvent(new ClusterWrapperEventArgs(EventType.Removed, notification.Payload));
					args.Cluster.RealtimeCollections.Remove<Node>(this);
				});
			}
			finally
			{
				args.Cluster.CacheManager.CacheLock.ExitWriteLock();
			}
		}
		return list;
	}

	public static bool ProcessNotificationSpecial(Notification notification)
	{
		if (notification.Payload is ClusterAddedEventArgs clusterAddedEventArgs)
		{
			PNode clusterObject = new PNode(clusterAddedEventArgs.Cluster, clusterAddedEventArgs.Id, clusterAddedEventArgs.Name);
			clusterObject = (PNode)clusterAddedEventArgs.Cluster.CacheManager.AddObject(clusterObject, clusterAddedEventArgs is ClusterUpsertEventArgs);
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

	protected override void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
		e.Properties.Get("NodeDrainStatus", delegate(ClusterPropertyUInt nodeDrainStatusProperty)
		{
			if (nodeDrainStatusProperty.TypedValue == 3)
			{
				SetError(new ClusterNodeDrainFailedException(base.Name));
			}
		});
		base.OnPropertiesChanged(sender, e);
	}
}

