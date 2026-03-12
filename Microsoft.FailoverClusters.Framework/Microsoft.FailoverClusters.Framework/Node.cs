using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class Node : ClusterObject
{
	private WeakReferenceEx stateCommandsWeak;

	private WeakReferenceEx moreActionsCommandWeak;

	private WeakReferenceEx pauseActionsCommandWeak;

	private WeakReferenceEx resumeActionsCommandWeak;

	private NodeState? state;

	private NodeOperatingSystemInformation operatingSystemInformation;

	private ServerInformation serverInformation;

	private ProcessorInformation processorInformation;

	private WeakReferenceEx<ProcessorInformation> processorInformationWeak;

	private ProcessorInformation temporaryProcessorInformation;

	private ClusterNodeDrainStatus? nodeDrainStatus;

	private NodeStatusInformation? statusInfo;

	private WeakReferenceEx networkInterfacesWeak;

	private WeakReferenceEx virtualDisksWeak;

	private WeakReferenceEx rolesWeak;

	private WeakReferenceEx poolsWeak;

	private Icon2 nodeIcon;

	private uint? nodeWeight;

	private uint? dynamicWeight;

	private string site = string.Empty;

	private string rack = string.Empty;

	private string chassis = string.Empty;

	private static Queue<Node> nodeReloadQueue;

	private static readonly ProcessorInformation EmptyProcessorInformation = new ProcessorInformation
	{
		IsLoaded = false
	};

	public virtual CommandCollection StateCommands => WeakReferenceEx.ReturnInstance(ref stateCommandsWeak, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.NodeState);
		InitializeStateCommands(commandCollection);
		return commandCollection;
	});

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Node;

	[Column(Name = "NodeInstanceId", AutoSync = AutoSync.Always)]
	public override Guid Id
	{
		get
		{
			return base.Id;
		}
		internal set
		{
			base.Id = value;
		}
	}

	public NodeStatus NodeStatus => ComputeNodeStatus();

	public NodeState State => LoadAsync<NodeState, NodeState>(state, 1);

	public int NodeId
	{
		get
		{
			string text = Id.ToString();
			return int.Parse(text.Substring(text.Length - 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		}
	}

	public Type NodeType => GetType();

	public uint? NodeWeight => LoadAsync<uint, uint>(nodeWeight, 2);

	public uint? DynamicWeight => LoadAsync<uint, uint>(dynamicWeight, 2);

	public string Site => LoadAsync(site, 2);

	public string Rack => LoadAsync(rack, 2);

	public string Chassis => LoadAsync(chassis, 2);

	public ClusterList<NetworkInterface> NetworkInterfaces => WeakReferenceEx.ReturnInstance(ref networkInterfacesWeak, delegate
	{
		string nodeName = base.Name;
		return (ClusterList<NetworkInterface>)base.Cluster.NetworkInterfaces.Where((NetworkInterface ni) => ni.Node == nodeName);
	});

	public ClusterList<Resource> VirtualDisks => WeakReferenceEx.ReturnInstance(ref virtualDisksWeak, () => (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
	{
		Name = "Storage class resources on node"
	}.Where((Resource r) => r.OwnerGroup.OwnerNode == this && (int)r.Class == 1));

	public ClusterList<Group> Roles => WeakReferenceEx.ReturnInstance(ref rolesWeak, () => (ClusterList<Group>)(from g in base.Cluster.Groups
		where g.IsCore == (bool?)false && (int)g.GroupType != 4 && (int)g.GroupType != 5 && (int)g.GroupType != 116 && (int)g.GroupType != 117 && g.OwnerNode == this
		orderby g.Name
		select g));

	public ClusterList<Resource> StoragePools => WeakReferenceEx.ReturnInstance(ref poolsWeak, () => (ClusterList<Resource>)(from r in new ClusterList<Resource>(base.Cluster)
		{
			Name = "All Storage Pools on a node"
		}
		where r.OwnerGroup.OwnerNode == this && (int)r.ResourceType.ResourceKind == 29
		orderby r.Name
		select r));

	public IEnumerable<PhysicalDiskInfo> PhysicalDisks => ObservablePhysicalDiskCollection.GetAssociation(this, base.Cluster, this);

	public IEnumerable<Enclosure> Enclosures => ObservableEnclosureCollection.GetAssociation(this, base.Cluster, this);

	public NodeOperatingSystemInformation OperatingSystemInformation => LoadAsync(operatingSystemInformation, 4096);

	public ServerInformation ServerInformation => LoadAsync(serverInformation, 16384);

	public ProcessorInformation ProcessorInformation
	{
		get
		{
			ProcessorInformation result = GetProcessorInformation();
			EnqueueRefreshOperation();
			return result;
		}
	}

	public ClusterNodeDrainStatus NodeDrainStatus => LoadAsync<ClusterNodeDrainStatus, ClusterNodeDrainStatus>(nodeDrainStatus, 3);

	public NodeStatusInformation StatusInformation => LoadAsync<NodeStatusInformation, NodeStatusInformation>(statusInfo, 3);

	public override Icon2 Icon => ReturnInstance(ref nodeIcon, () => new Icon2(InvariantResources.Node));

	internal override Type OwnerType => typeof(PNode);

	public event EventHandler<ClusterNodeStateEventArgs> StateChanged;

	private void InitializePauseCommands(CommandCollection commandsCollection)
	{
		ClusterCommand item = new ClusterCommand(this, "DrainNode", ClusterCommandId.NodePauseDrainNode, ClusterCommandCollectionId.NodePauseDrainNode)
		{
			Text = CommandResources.PauseNodeDrainAction_Text,
			Description = CommandResources.PauseNodeDrainActionDescription_Text,
			CanExecuteDelegate = (object x) => CanExecuteDrainCommand(this),
			ExecuteDelegate = delegate
			{
				Pause(NodePauseDrainType.Drain);
			},
			CommandParameter = this
		};
		commandsCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(this, "DoNotDrainNode", ClusterCommandId.NodePauseDoNotDrainNode, ClusterCommandCollectionId.NodePauseDoNotDrainNode)
		{
			Text = CommandResources.PauseNodeNoDrainAction_Text,
			Description = CommandResources.PauseNodeNoDrainActionDescription_Text,
			CanExecuteDelegate = (object x) => CanExecuteDoNotDrainCommand(this),
			ExecuteDelegate = delegate
			{
				Pause(NodePauseDrainType.NoDrain);
			},
			CommandParameter = this
		};
		commandsCollection.Add(item2);
	}

	private static bool CanExecuteDrainCommand(Node node)
	{
		if (node.State != 0 || node.NodeDrainStatus != 0)
		{
			if (node.State == NodeState.Pause)
			{
				return node.NodeDrainStatus == ClusterNodeDrainStatus.Failed;
			}
			return false;
		}
		return true;
	}

	private static bool CanExecuteDoNotDrainCommand(Node node)
	{
		if (node.State == NodeState.Up)
		{
			return node.NodeDrainStatus == ClusterNodeDrainStatus.NotInitiated;
		}
		return false;
	}

	private void InitializeResumeCommands(CommandCollection commandsCollection)
	{
		ClusterCommand item = new ClusterCommand(this, "ResumeNodeFailbackRoles", ClusterCommandId.NodeResumeFailbackRoles, ClusterCommandCollectionId.NodeResumeFailbackRoles)
		{
			Text = CommandResources.ResumeNodeFailbackAction_Text,
			Description = CommandResources.ResumeNodeNoFailbackActionDescription_Text,
			CanExecuteDelegate = (object x) => CanExecuteResumeCommand(this),
			ExecuteDelegate = delegate
			{
				Resume(NodeResumeFailbackType.FailbackGroupsImmediately);
			},
			CommandParameter = this
		};
		commandsCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(this, "ResumeNodeDoNotFailbackRoles", ClusterCommandId.NodeResumeDoNotFailbackRoles, ClusterCommandCollectionId.NodeResumeDoNotFailbackRoles)
		{
			Text = CommandResources.ResumeNodeNoFailbackAction_Text,
			Description = CommandResources.ResumeNodeNoFailbackActionDescription_Text,
			CanExecuteDelegate = (object x) => CanExecuteResumeCommand(this),
			ExecuteDelegate = delegate
			{
				Resume(NodeResumeFailbackType.DoNotFailbackGroups);
			},
			CommandParameter = this
		};
		commandsCollection.Add(item2);
	}

	private static bool CanExecuteResumeCommand(Node node)
	{
		return node.State == NodeState.Pause;
	}

	protected override void InitializeCommands(CommandCollection collection)
	{
		base.InitializeCommands(collection);
		ClusterCommandContainer item = WeakReferenceEx.ReturnInstance(ref pauseActionsCommandWeak, delegate
		{
			ClusterCommandContainer clusterCommandContainer3 = new ClusterCommandContainer(this, "NodePauseActions", ClusterCommandId.NodePauseActions)
			{
				Text = CommandResources.PauseAction_Text,
				CommandParameter = this,
				ExecuteIfNoChildren = false
			};
			InitializePauseCommands(clusterCommandContainer3.ChildrenInternal);
			return clusterCommandContainer3;
		});
		collection.Add(item);
		ClusterCommandContainer item2 = WeakReferenceEx.ReturnInstance(ref resumeActionsCommandWeak, delegate
		{
			ClusterCommandContainer clusterCommandContainer2 = new ClusterCommandContainer(this, "NodeResumeActions", ClusterCommandId.NodeResumeActions)
			{
				Text = CommandResources.ResumeAction_Text,
				CommandParameter = this,
				ExecuteIfNoChildren = false
			};
			InitializeResumeCommands(clusterCommandContainer2.ChildrenInternal);
			return clusterCommandContainer2;
		});
		collection.Add(item2);
		ClusterCommand item3 = new ClusterCommand(this, "RemoteDesktop", ClusterCommandId.NodeRemoteDesktop, ClusterCommandCollectionId.NodeRemoteDesktop)
		{
			Text = CommandResources.RemoteDesktop,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				StartRemoteDesktop();
			},
			CommandParameter = this
		};
		collection.Add(item3);
		foreach (ClusterCommand miscellaneousCommand in MiscellaneousCommands)
		{
			collection.Add(miscellaneousCommand);
		}
		ClusterCommandContainer item5 = WeakReferenceEx.ReturnInstance(ref moreActionsCommandWeak, delegate
		{
			ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(this, "NodeMoreActions", ClusterCommandId.NodeMoreActions)
			{
				Text = CommandResources.MoreActions,
				CommandParameter = this,
				ExecuteIfNoChildren = false
			};
			InitializeMoreActionsCommands(clusterCommandContainer);
			return clusterCommandContainer;
		});
		collection.Add(item5);
	}

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		base.InitializeMoreActionsCommands(commandContainer);
		InitializeStateCommands(commandContainer.ChildrenInternal);
		ClusterCommand item = new ClusterCommand(this, "EvictNode", ClusterCommandId.NodeEvict, ClusterCommandCollectionId.NodeEvict)
		{
			Text = CommandResources.EvictNodeAction_Text,
			CanExecuteDelegate = (object x) => NodeDrainStatus != ClusterNodeDrainStatus.InProgress,
			ExecuteDelegate = delegate
			{
				Delete(askConfirmation: true);
			},
			CommandParameter = this
		};
		commandContainer.ChildrenInternal.Add(item);
	}

	internal static CommandCollection InitializeApplicationCommands(Cluster cluster, IEnumerable<Node> nodes)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceApplication)
		{
			Name = "Multi node Application Commands"
		};
		ClusterCommand item = new ClusterCommand(null, "MultiRemoteDesktop", ClusterCommandId.NodeMultipleRemoteDesktop, commandCollection.Category)
		{
			Text = CommandResources.RemoteDesktop,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				StartRemoteDesktops(nodes);
			}
		};
		commandCollection.Add(item);
		ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(null, "NodeMultiPauseActions", ClusterCommandId.NodeMultiplePauseActions, ClusterCommandCollectionId.NodeMultiplePause)
		{
			Text = CommandResources.PauseAction_Text,
			CommandParameter = nodes,
			ExecuteIfNoChildren = false
		};
		InitializeMultiPauseCommands(clusterCommandContainer.ChildrenInternal, nodes);
		ClusterCommandContainer clusterCommandContainer2 = new ClusterCommandContainer(null, "NodeMultiResumeActions", ClusterCommandId.NodeMultipleResumeActions, ClusterCommandCollectionId.NodeMultipleResume)
		{
			Text = CommandResources.ResumeAction_Text,
			CommandParameter = nodes,
			ExecuteIfNoChildren = false
		};
		InitializeMultiResumeCommands(clusterCommandContainer2.ChildrenInternal, nodes);
		ClusterCommandContainer clusterCommandContainer3 = new ClusterCommandContainer(null, "NodeMultiMoreActions", ClusterCommandId.NodeMultipleMoreActions, ClusterCommandCollectionId.NodeMultipleMoreActions)
		{
			Text = CommandResources.MoreActions,
			CommandParameter = nodes,
			ExecuteIfNoChildren = false
		};
		InitializeMultipleMoreActionsCommands(clusterCommandContainer3.ChildrenInternal, nodes);
		commandCollection.Add(clusterCommandContainer);
		commandCollection.Add(clusterCommandContainer2);
		commandCollection.Add(clusterCommandContainer3);
		return commandCollection;
	}

	private static void InitializeMultipleMoreActionsCommands(CommandCollection commandsCollection, IEnumerable<Node> nodes)
	{
		ClusterCommand item = new ClusterCommand(null, "StartService", ClusterCommandId.NodeStartService, ClusterCommandCollectionId.NodeStartService)
		{
			Text = CommandResources.StartServiceAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				StartService(nodes);
			}
		};
		commandsCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(null, "StopService", ClusterCommandId.NodeMultipleStopService, ClusterCommandCollectionId.NodeStopService)
		{
			Text = CommandResources.StopServiceAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				StopService(nodes);
			}
		};
		commandsCollection.Add(item2);
	}

	private static void StartRemoteDesktops(IEnumerable<Node> nodes)
	{
		ClusterObject.EnqueueAndThrottleRequests(nodes, delegate(ClusterObject node, Action<OperationResult> operationResult)
		{
			((Node)node).StartRemoteDesktop(delegate(OperationResult localOpResult)
			{
				node.Error = localOpResult.Error;
				node.SetProcessingFlag(processing: false);
				operationResult(localOpResult);
			});
		});
	}

	private static void StopService(IEnumerable<Node> nodes)
	{
		ClusterObject.EnqueueAndThrottleRequests(nodes, delegate(ClusterObject nodeObj, Action<OperationResult> operationResult)
		{
			Node node = (Node)nodeObj;
			node.Stop(delegate(OperationResult localOpResult)
			{
				node.Error = localOpResult.Error;
				node.SetProcessingFlag(processing: false);
				operationResult(localOpResult);
			});
		});
	}

	private static void StartService(IEnumerable<Node> nodes)
	{
		ClusterObject.EnqueueAndThrottleRequests(nodes, delegate(ClusterObject nodeObj, Action<OperationResult> operationResult)
		{
			Node node = (Node)nodeObj;
			node.Start(delegate(OperationResult localOpResult)
			{
				node.Error = localOpResult.Error;
				node.SetProcessingFlag(processing: false);
				operationResult(localOpResult);
			});
		});
	}

	private static void InitializeMultiPauseCommands(CommandCollection commandsCollection, IEnumerable<Node> nodes)
	{
		ClusterCommand item = new ClusterCommand(null, "MultipleDrainNode", ClusterCommandId.NodeMultiplePauseDrainNode, ClusterCommandCollectionId.NodeMultiplePauseDrainNode)
		{
			Text = CommandResources.PauseNodeDrainAction_Text,
			Description = CommandResources.PauseNodeDrainActionDescription_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				Pause(NodePauseDrainType.Drain, nodes);
			}
		};
		commandsCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(null, "MultipleDoNotDrainNode", ClusterCommandId.NodeMultiplePauseDoNotDrainNode, ClusterCommandCollectionId.NodeMultiplePauseDoNotDrainNode)
		{
			Text = CommandResources.PauseNodeNoDrainAction_Text,
			Description = CommandResources.PauseNodeNoDrainActionDescription_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				Pause(NodePauseDrainType.NoDrain, nodes);
			}
		};
		commandsCollection.Add(item2);
	}

	private static void InitializeMultiResumeCommands(CommandCollection commandsCollection, IEnumerable<Node> nodes)
	{
		ClusterCommand item = new ClusterCommand(null, "MultipleResumeNodeFailbackRoles", ClusterCommandId.NodeMultipleResumeFailbackRoles, ClusterCommandCollectionId.NodeMultipleResumeFailbackRoles)
		{
			Text = CommandResources.ResumeNodeFailbackAction_Text,
			Description = CommandResources.ResumeNodeNoFailbackActionDescription_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				Resume(NodeResumeFailbackType.FailbackGroupsImmediately, nodes);
			}
		};
		commandsCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(null, "MultipleResumeNodeDoNotFailbackRoles", ClusterCommandId.NodeMultipleResumeDoNotFailbackRoles, ClusterCommandCollectionId.NodeMultipleResumeDoNotFailbackRoles)
		{
			Text = CommandResources.ResumeNodeNoFailbackAction_Text,
			Description = CommandResources.ResumeNodeNoFailbackActionDescription_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				Resume(NodeResumeFailbackType.DoNotFailbackGroups, nodes);
			}
		};
		commandsCollection.Add(item2);
	}

	private static void Pause(NodePauseDrainType drainType, IEnumerable<Node> nodes)
	{
		ClusterObject.EnqueueAndThrottleRequests(nodes, delegate(ClusterObject nodeObj, Action<OperationResult> operationResult)
		{
			Node node = (Node)nodeObj;
			if ((drainType == NodePauseDrainType.Drain) ? CanExecuteDrainCommand(node) : CanExecuteDoNotDrainCommand(node))
			{
				node.Pause(drainType, delegate(OperationResult localOpResult)
				{
					node.Error = localOpResult.Error;
					operationResult(localOpResult);
				});
			}
			else
			{
				node.SetProcessingFlag(processing: false);
			}
		});
	}

	private static void Resume(NodeResumeFailbackType failbackType, IEnumerable<Node> nodes)
	{
		ClusterObject.EnqueueAndThrottleRequests(nodes, delegate(ClusterObject nodeObj, Action<OperationResult> operationResult)
		{
			Node node = (Node)nodeObj;
			if (CanExecuteResumeCommand(node))
			{
				node.Resume(failbackType, delegate(OperationResult localOpResult)
				{
					node.Error = localOpResult.Error;
					operationResult(localOpResult);
				});
			}
			else
			{
				node.SetProcessingFlag(processing: false);
			}
		});
	}

	protected virtual void InitializeStateCommands(CommandCollection commandsCollection)
	{
		ClusterCommand item = new ClusterCommand(this, "StartService", ClusterCommandId.NodeStartService, ClusterCommandCollectionId.NodeStartService)
		{
			Text = CommandResources.StartServiceAction_Text,
			CanExecuteDelegate = (object x) => State == NodeState.Down,
			ExecuteDelegate = delegate
			{
				Start();
			},
			CommandParameter = this
		};
		commandsCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(this, "StopService", ClusterCommandId.NodeStopService, ClusterCommandCollectionId.NodeStopService)
		{
			Text = CommandResources.StopServiceAction_Text,
			CanExecuteDelegate = (object x) => State != NodeState.Down && NodeDrainStatus != ClusterNodeDrainStatus.InProgress,
			ExecuteDelegate = delegate
			{
				Stop();
			},
			CommandParameter = this
		};
		commandsCollection.Add(item2);
	}

	public Node(Cluster cluster)
		: base(cluster)
	{
	}

	public Node Init(params object[] operations)
	{
		Utilities.UnreferencedParameter(operations);
		return this;
	}

	public override void Delete(bool askConfirmation = false)
	{
		Delete(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public override void Delete(Action<OperationResult> nodeOpDelete, bool askConfirmation = false)
	{
		base.Cluster.WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck.Evict, NodeId.ToString(CultureInfo.InvariantCulture), delegate(OperationResult<bool> nodeQuorumLossOpResult)
		{
			if (nodeQuorumLossOpResult.Error != null)
			{
				nodeOpDelete(nodeQuorumLossOpResult);
			}
			else if (nodeQuorumLossOpResult.Result)
			{
				nodeOpDelete(new OperationResult(this, new ClusterNodeEvictionWillCauseQuorumLossException(DisplayName)));
			}
			else
			{
				CreateDeleteDialog(delegate(ConfirmationDialog confirmationDialog)
				{
					ShowDialog(confirmationDialog, delegate
					{
						DeleteOperation(nodeOpDelete);
					}, null);
				}, askConfirmation);
			}
		});
	}

	public void StartRemoteDesktop()
	{
		StartRemoteDesktop(base.SetLastError);
	}

	public void StartRemoteDesktop(Action<OperationResult> startRemoteDesktopOperation)
	{
		string arguments = string.Format(CultureInfo.CurrentCulture, "/v:{0}", base.Name);
		ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mstsc.exe"), arguments)
		{
			ErrorDialog = true,
			WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
		};
		OperationResult operationResult = null;
		try
		{
			UIHelper.ApplicationActivate(Process.Start(startInfo));
			operationResult = new OperationResult(this, null);
		}
		catch (Win32Exception ex)
		{
			ClusterLog.LogException(ex, "An error occurred opening a remote desktop connection to node {0}", base.Name);
			operationResult = new OperationResult(this, new ClusterDefaultException(ex));
		}
		catch (FileNotFoundException ex2)
		{
			ClusterLog.LogException(ex2, "An error occurred opening a remote desktop connection to node {0}", base.Name);
			operationResult = new OperationResult(this, new ClusterDefaultException(ex2));
		}
		finally
		{
			startRemoteDesktopOperation.SafeCall(operationResult);
		}
	}

	public void Pause(NodePauseDrainType drainType)
	{
		Pause(drainType, base.SetLastError);
	}

	public void Pause(NodePauseDrainType drainType, Action<OperationResult> pauseNodeOperation)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PNode)lockObject.Owner).Pause(drainType);
		}, pauseNodeOperation, LockAccess.Reader);
	}

	public void Resume(NodeResumeFailbackType failbackType)
	{
		Resume(failbackType, base.SetLastError);
	}

	public void Resume(NodeResumeFailbackType failbackType, Action<OperationResult> resumeNodeOperation)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PNode)lockObject.Owner).Resume(failbackType);
		}, resumeNodeOperation, LockAccess.Reader);
	}

	public void Start()
	{
		Start(base.SetLastError);
	}

	public void Start(Action<OperationResult> startNodeOperation)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PNode)lockObject.Owner).Start();
		}, startNodeOperation, LockAccess.Reader);
	}

	public void Stop()
	{
		Stop(base.SetLastError);
	}

	public void Stop(Action<OperationResult> stopNodeOperation)
	{
		base.Cluster.WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck.Down, NodeId.ToString(CultureInfo.InvariantCulture), delegate(OperationResult<bool> nodeQuorumLossOpResult)
		{
			if (nodeQuorumLossOpResult.Error != null)
			{
				stopNodeOperation(nodeQuorumLossOpResult);
			}
			else if (nodeQuorumLossOpResult.Result)
			{
				stopNodeOperation(new OperationResult(this, new ClusterNodeDownWillCauseQuorumLossException(DisplayName)));
			}
			else
			{
				this.ExecuteMethod(delegate(ILockable lockObject)
				{
					((PNode)lockObject.Owner).Stop();
				}, stopNodeOperation, LockAccess.Reader);
			}
		});
	}

	public static void Get(Cluster cluster, Guid nodeId, Action<OperationResult<Node>> operationResult, OperationType operationType)
	{
		Node node = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(nodeId, ClusterIdentityType.Node, LockAccess.Reader);
				if (clusterLock != null)
				{
					node = ((PNode)clusterLock.Owner).GetProxy();
				}
				else
				{
					PNode pNode = (PNode)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Node, nodeId);
					try
					{
						pNode.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pNode);
						throw;
					}
					node = pNode.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Node> obj = new OperationResult<Node>(cluster, node, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public static void Get(Cluster cluster, string nodeName, Action<OperationResult<Node>> operationResult, OperationType operationType)
	{
		Node node = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(nodeName, ClusterIdentityType.Node, LockAccess.Reader);
				if (clusterLock != null)
				{
					node = ((PNode)clusterLock.Owner).GetProxy();
				}
				else
				{
					PNode pNode = (PNode)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Node, nodeName);
					try
					{
						pNode.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pNode);
						throw;
					}
					node = pNode.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Node> obj = new OperationResult<Node>(cluster, node, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public override string ToString()
	{
		return string.Concat("Node:", Id, ":", base.Name, ":", base.IsOpen.ToString());
	}

	public Node()
		: base(null)
	{
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents: false, ignorePossibleOwners);
		PNode pNode = (PNode)privateObject;
		SetIdInternal(pNode.Id);
		SetNameInternal(pNode.Name);
		state = pNode.State;
		operatingSystemInformation = pNode.OperatingSystemInformation;
		serverInformation = pNode.ServerInformation;
		processorInformation = pNode.ProcessorInformation;
		base.LoadSelection = pNode.LoadedSelection;
		Properties = pNode.Properties;
		ParseProperties(pNode.Properties, trackChanges: false);
		if (subscribeToEvents)
		{
			SubscribeToEvents(pNode);
		}
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		if (ParseProperty(properties, "NodeDrainStatus", ref nodeDrainStatus, list))
		{
			list.TryAdd("NodeStatus");
		}
		if (ParseProperty(properties, "NodeWeight", ref nodeWeight, list))
		{
			list.TryAdd("NodeWeight");
		}
		if (ParseProperty(properties, "DynamicWeight", ref dynamicWeight, list))
		{
			list.TryAdd("DynamicWeight");
		}
		if (ParseProperty(properties, "StatusInformation", ref statusInfo, list))
		{
			list.TryAdd("NodeStatus");
		}
		IList<string> memberVariable = null;
		if (ParseProperty(properties, "FaultDomain", ref memberVariable, list))
		{
			site = string.Empty;
			rack = string.Empty;
			chassis = string.Empty;
			if (memberVariable != null)
			{
				foreach (string item in memberVariable)
				{
					ParsePropertyAndValue(item, out var property, out var value);
					switch (property)
					{
					case "Site":
						site = value;
						break;
					case "Rack":
						rack = value;
						break;
					case "Chassis":
						chassis = value;
						break;
					}
				}
			}
			list.TryAdd("Site");
			list.TryAdd("Rack");
			list.TryAdd("Chassis");
		}
		return list;
	}

	private void ParsePropertyAndValue(string faultDomain, out string property, out string value)
	{
		property = null;
		value = null;
		if (faultDomain != null)
		{
			string[] array = faultDomain.Split(new char[1] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
			property = array[0];
			if (array.Length > 1)
			{
				value = array[1];
			}
		}
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs clusterPropertiesEventArgs = e.EventArgument as ClusterPropertiesEventArgs;
			if (clusterPropertiesEventArgs.Error == null)
			{
				IEnumerable<string> propertiesChanged = ParseProperties(clusterPropertiesEventArgs.Properties, trackChanges: true);
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					propertiesChanged.ForEach(base.OnPropertyChanged);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.NodeServerInformationChanged:
		{
			ClusterNodeServerInformationEventArgs clusterNodeServerInformationEventArgs = e.EventArgument as ClusterNodeServerInformationEventArgs;
			if (serverInformation == clusterNodeServerInformationEventArgs.ServerInformation && clusterNodeServerInformationEventArgs.Error == null)
			{
				return true;
			}
			serverInformation = clusterNodeServerInformationEventArgs.ServerInformation;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("ServerInformation");
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.NodeProcessorInformationChanged:
		{
			ClusterNodeProcessorInformationEventArgs clusterNodeProcessorInformationEventArgs = e.EventArgument as ClusterNodeProcessorInformationEventArgs;
			if (processorInformation == clusterNodeProcessorInformationEventArgs.ProcessorInformation && clusterNodeProcessorInformationEventArgs.Error == null)
			{
				return true;
			}
			WeakReferenceEx<ProcessorInformation> weakReferenceEx = processorInformationWeak;
			if (weakReferenceEx != null && weakReferenceEx.Target != null)
			{
				temporaryProcessorInformation = clusterNodeProcessorInformationEventArgs.ProcessorInformation;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("ProcessorInformation");
				}, OperationType.Async, queueOnDispatcher);
				return true;
			}
			processorInformationWeak = null;
			temporaryProcessorInformation = null;
			return false;
		}
		case EventType.NodeOperatingSystemInformationChanged:
		{
			ClusterNodeOperatingSystemInformationEventArgs clusterNodeOperatingSystemInformationEventArgs = e.EventArgument as ClusterNodeOperatingSystemInformationEventArgs;
			if (operatingSystemInformation == clusterNodeOperatingSystemInformationEventArgs.OperatingSystemInformation && clusterNodeOperatingSystemInformationEventArgs.Error == null)
			{
				return true;
			}
			operatingSystemInformation = clusterNodeOperatingSystemInformationEventArgs.OperatingSystemInformation;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("OperatingSystemInformation");
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.NodeStateChanged:
		{
			ClusterNodeStateEventArgs args = e.EventArgument as ClusterNodeStateEventArgs;
			if (state == args.State && args.Error == null)
			{
				return true;
			}
			state = args.State;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("State");
				this.StateChanged.SafeCall(this, args);
				OnPropertyChanged("NodeStatus");
				OnPropertyChanged("OperatingSystemInformation");
				OnPropertyChanged("ProcessorInformation");
				OnPropertyChanged("ServerInformation");
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private NodeStatus ComputeNodeStatus()
	{
		if (StatusInformation.HasFlag(NodeStatusInformation.Isolated))
		{
			return NodeStatus.Isolated;
		}
		if (StatusInformation.HasFlag(NodeStatusInformation.Quarantined))
		{
			return NodeStatus.Quarantined;
		}
		if (NodeDrainStatus == ClusterNodeDrainStatus.InProgress)
		{
			return NodeStatus.Draining;
		}
		if (NodeDrainStatus == ClusterNodeDrainStatus.Failed)
		{
			return NodeStatus.DrainFailed;
		}
		return (NodeStatus)State;
	}

	private ProcessorInformation GetProcessorInformation()
	{
		ProcessorInformation result;
		if (temporaryProcessorInformation != null)
		{
			result = (processorInformation = temporaryProcessorInformation);
			processorInformationWeak = new WeakReferenceEx<ProcessorInformation>(processorInformation);
			temporaryProcessorInformation = null;
		}
		else
		{
			result = WeakReferenceEx.ReturnInstance(ref processorInformationWeak, delegate
			{
				BeginLoadNodeProperties(this);
				return EmptyProcessorInformation;
			});
		}
		return result;
	}

	private static void BeginLoadNodeProperties(Node node)
	{
		node.LoadAsync(536883200);
	}

	private void EnqueueRefreshOperation()
	{
		bool flag = false;
		lock (base.LockObject)
		{
			if (nodeReloadQueue == null)
			{
				nodeReloadQueue = new Queue<Node>();
				flag = true;
			}
			if (!nodeReloadQueue.Contains(this))
			{
				nodeReloadQueue.Enqueue(this);
			}
		}
		if (!flag)
		{
			return;
		}
		Worker.Start(delegate
		{
			Thread.Sleep(5000);
			while (true)
			{
				Node node;
				lock (base.LockObject)
				{
					if (nodeReloadQueue == null || nodeReloadQueue.Count == 0)
					{
						nodeReloadQueue = null;
						break;
					}
					node = nodeReloadQueue.Dequeue();
				}
				WeakReferenceEx<ProcessorInformation> weakReferenceEx = node.processorInformationWeak;
				if (weakReferenceEx != null && weakReferenceEx.Target != null)
				{
					BeginLoadNodeProperties(node);
				}
			}
		}, delegate(ClusterException exception)
		{
			nodeReloadQueue = null;
			ClusterLog.LogException(exception);
		});
	}

	public static bool operator ==(Node node1, Node node2)
	{
		return node1?.Equals(node2) ?? ((object)node2 == null);
	}

	public static bool operator !=(Node node1, Node node2)
	{
		return !(node1 == node2);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	protected virtual void CreateDeleteDialog(Action<ConfirmationDialog> confirmationDialogCreation, bool createDialog)
	{
		if (confirmationDialogCreation != null)
		{
			if (!createDialog)
			{
				confirmationDialogCreation(null);
				return;
			}
			ConfirmationDialog obj = new ConfirmationDialog
			{
				CustomIcon = Icon.NativeIcon,
				Caption = DialogResources.DeleteNode_Title.FormatCurrentCulture(DisplayName),
				Header = DialogResources.DeleteNode_Header.FormatCurrentCulture(DisplayName),
				Content = DialogResources.DeleteNode_Content
			};
			confirmationDialogCreation(obj);
		}
	}

	protected virtual void DeleteOperation(Action<OperationResult> nodeOpDelete)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.Delete();
		}, nodeOpDelete, LockAccess.Reader);
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		state = null;
		operatingSystemInformation = null;
		serverInformation = null;
		processorInformation = null;
		networkInterfacesWeak = null;
		virtualDisksWeak = null;
		rolesWeak = null;
		nodeWeight = null;
		dynamicWeight = null;
		site = null;
		rack = null;
		chassis = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("State");
			OnPropertyChanged("NodeStatus");
			OnPropertyChanged("operatingSystemInformation");
			OnPropertyChanged("ServerInformation");
			OnPropertyChanged("ProcessorInformation");
			OnPropertyChanged("NetworkInterfaces");
			OnPropertyChanged("TopLevelResources");
			OnPropertyChanged("Roles");
			OnPropertyChanged("NodeWeight");
			OnPropertyChanged("DynamicWeight");
			OnPropertyChanged("Site");
			OnPropertyChanged("Rack");
			OnPropertyChanged("Chassis");
			this.StateChanged.SafeCall(this, null);
		}, OperationType.Async);
	}
}
