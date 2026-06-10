using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public abstract class Group : ClusterObject
{
	private WeakReferenceEx groupCommandsWeak;

	private WeakReferenceEx ownershipCommandsWeak;

	private WeakReferenceEx moreActionsCommandWeak;

	private WeakReferenceEx addResourceCommandWeak;

	private WeakReferenceEx manageApplicationCommandWeak;

	private StateCommandCollectionWrapper stateCommandCollectionWrapper;

	private PriorityCommand priorityCommand;

	private AddShareCommand addShareCommand;

	public const uint MaximumFailoverThreshold = uint.MaxValue;

	public const uint MaximumFailoverPeriod = 1193u;

	public const uint MaximumFailbackWindowStart = 23u;

	public const uint MaximumFailbackWindowEnd = 23u;

	public const uint FailbackWindowNone = uint.MaxValue;

	public const uint AutoFailbackEnabled = 1u;

	private readonly List<FileShareErrorItem> fileShareErrors = new List<FileShareErrorItem>();

	private GroupState? groupState;

	private GroupSubStatus? subStatus;

	private Priority? priority;

	private GroupType? groupType;

	private Node ownerNode;

	private bool? isCore;

	private GroupFlags? flags;

	private WeakReferenceEx resourcesWeak;

	private WeakReferenceEx allResourcesWeak;

	private WeakReferenceEx allResourcesNonCsvWeak;

	private WeakReferenceEx moveTargetsWeak;

	private WeakReferenceEx storageResourcesWeak;

	private ReadOnlyObservableCollection<Node> preferredOwners;

	private WeakReferenceEx clientAccessPointsWeak;

	private WeakLazy<Icon2> icon;

	private WeakLazy<FileShareCollection> lazyShares;

	public virtual CommandCollection GroupCommands => WeakReferenceEx.ReturnInstance(ref groupCommandsWeak, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.Group);
		InitializeGroupCommands(commandCollection);
		return commandCollection;
	});

	public virtual CommandCollection StateCommands => stateCommandCollectionWrapper.GetInstance();

	public virtual CommandCollection ApplicationCommands
	{
		get
		{
			CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.Group);
			InitializeApplicationCommands(commandCollection);
			return commandCollection;
		}
	}

	public virtual ClusterCommand OwnershipCommand => WeakReferenceEx.ReturnInstance(ref ownershipCommandsWeak, InitializeOwnershipCommand);

	public virtual ClusterCommand PriorityCommand => InitializePriorityCommand();

	public virtual ClusterCommand ManageApplicationCommand => WeakReferenceEx.ReturnInstance(ref manageApplicationCommandWeak, InitializeManageApplicationCommand);

	internal IClusterList<Node> MoveTargets
	{
		get
		{
			if (ownerNode != null)
			{
				Guid nodeId = ownerNode.Id;
				return WeakReferenceEx.ReturnInstance(ref moveTargetsWeak, () => ((IClusterList<Node>)(from node in base.Cluster.Nodes
					where node.Id != nodeId && (int)node.State == 0
					orderby node.Name
					select node)).ForceLoadStart());
			}
			return WeakReferenceEx.ReturnInstance(ref moveTargetsWeak, () => ((ClusterList<Node>)(from node in base.Cluster.Nodes
				where (int)node.State == 0
				orderby node.Name
				select node)).ForceLoadStart());
		}
	}

	protected ClusterCommand AddShareCommand => addShareCommand;

	public static Dictionary<GroupType, ResourceKind> GroupRoleToResourceMapping { get; private set; }

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Group;

	private ApplicationStatus ApplicationStatusBackStore { get; set; }

	[PropertyStore("ApplicationStatusBackStore")]
	[Column(Name = "ApplicationStatus", Expression = "ApplicationStatus")]
	public virtual ApplicationStatus ApplicationStatus
	{
		get
		{
			ApplicationStatus applicationStatus = GroupState.ToApplicationStatus();
			if (LastApplicationStatus != applicationStatus)
			{
				LastApplicationStatus = applicationStatus;
				Error = null;
			}
			return applicationStatus;
		}
	}

	public virtual VirtualMachineReplicationHealth ReplicationHealth => VirtualMachineReplicationHealth.NotApplicable;

	private GroupState GroupStateBackStore { get; set; }

	[PropertyStore("GroupStateBackStore")]
	[Column(Name = "State")]
	public GroupState GroupState
	{
		get
		{
			return LoadAsync<GroupState, GroupState>(groupState, 1);
		}
		set
		{
			if (value != GroupState.Fetching && groupState != value)
			{
				ChangeStateInternal(value, base.PropertiesOperationType);
			}
		}
	}

	public bool IsHidden { get; protected set; }

	public GroupSubStatus GroupSubStatus => LoadAsync<GroupSubStatus, GroupSubStatus>(subStatus, 2);

	[Column(Name = "GroupType", AutoSync = AutoSync.Always)]
	public abstract GroupType GroupType { get; }

	[Column(Name = "GroupProperties")]
	public override ClusterPropertyCollection Properties => base.Properties;

	private Priority PriorityBackStore { get; set; }

	[PropertyStore("PriorityBackStore")]
	[Column(Name = "Priority")]
	public Priority Priority
	{
		get
		{
			return LoadAsync<Priority, Priority>(priority, 3);
		}
		set
		{
			if (priority != value)
			{
				ChangePriorityInternal(value, base.PropertiesOperationType, null);
			}
		}
	}

	[Column(Name = "OwnerNode")]
	public Node OwnerNode
	{
		get
		{
			return LoadAsync(ownerNode, 1);
		}
		set
		{
			if (ownerNode != value)
			{
				ChangeOwnerNodeInternal(value);
			}
		}
	}

	public string OwnerNodeName
	{
		get
		{
			if (OwnerNode != null)
			{
				return ownerNode.Name;
			}
			return CommonResources.LoadingText;
		}
	}

	private bool IsCoreBackStore { get; set; }

	[PropertyStore("IsCoreBackStore")]
	[Column(Name = "IsCore")]
	public bool? IsCore => LoadAsync(isCore, 1);

	[Column(Name = "Flags")]
	public GroupFlags Flags => LoadAsync<GroupFlags, GroupFlags>(flags, 1);

	public override Icon2 Icon => icon.GetInstance();

	public bool IsResourceSpecificClientAccessPointDependenciesLoaded { get; set; }

	public ClusterList<Resource> AllResources => WeakReferenceEx.ReturnInstance(ref allResourcesWeak, () => (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
	{
		Name = "All Resources"
	}.Where((Resource r) => r.OwnerGroup == this || (int)r.ResourceType.ResourceKind == 65539));

	public ClusterList<Resource> AllResourcesNonCsv => WeakReferenceEx.ReturnInstance(ref allResourcesNonCsvWeak, () => (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
	{
		Name = "All Resources Non CSV"
	}.Where((Resource r) => r.OwnerGroup == this));

	public ClusterList<Resource> PhysicalDiskResources => WeakReferenceEx.ReturnInstance(ref storageResourcesWeak, () => (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
	{
		Name = "Physical Disk Resources"
	}.Where((Resource r) => r.OwnerGroup == this && (int)r.ResourceType.ResourceKind == 3));

	public ClusterList<Resource> TopLevelResources => WeakReferenceEx.ReturnInstance(ref resourcesWeak, delegate
	{
		((ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
		{
			Name = "TopLevelResources-VM Config Files"
		}.Where((Resource r) => r.OwnerGroup == this && (int)r.ResourceType.ResourceKind == 22)).ExecuteQuery(VirtualMachineConfigResourcesQuery);
		return new ClusterList<Resource>(base.Cluster)
		{
			Name = "Top Level Resources Empty",
			IsLoadingRtc = true
		};
	});

	public ReadOnlyObservableCollection<Node> PreferredOwners
	{
		get
		{
			return LoadAsync(preferredOwners, 8);
		}
		set
		{
			if (preferredOwners == value)
			{
				return;
			}
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				((PGroup)lockObject.Owner).SetPreferredOwners(value.ConvertAll((Node node) => node.Name));
			}, null, LockAccess.Reader);
		}
	}

	public ClusterList<Resource> ClientAccessPoints => WeakReferenceEx.ReturnInstance(ref clientAccessPointsWeak, () => (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
	{
		Name = "Client Access Point"
	}.Where((Resource r) => r.OwnerGroup == this && ((int)r.ResourceType.ResourceKind == 7 || (int)r.ResourceType.ResourceKind == 26)));

	public FileShareCollection Shares => lazyShares;

	internal override Type OwnerType => typeof(PGroup);

	protected ApplicationStatus LastApplicationStatus { get; set; }

	public event EventHandler<ClusterApplicationStatusEventArgs> ApplicationStatusChanged;

	public event EventHandler<ClusterGroupStateEventArgs> StateChanged;

	public event EventHandler<ClusterGroupPriorityEventArgs> PriorityChanged;

	public event EventHandler<ClusterGroupTypeEventArgs> GroupTypeChanged;

	public event EventHandler<ClusterGroupOwnerNodeEventArgs> OwnerNodeChanged;

	public event EventHandler<ClusterGroupIsCoreEventArgs> IsCoreChanged;

	public event EventHandler<ClusterGroupFlagsEventArgs> FlagsChanged;

	public event EventHandler<ClusterGroupPreferredOwnersChangedEventArgs> PreferredOwnersChanged;

	protected override void InitializeCommands(CommandCollection collection)
	{
		foreach (ClusterCommand stateCommand in StateCommands)
		{
			collection.Add(stateCommand);
		}
		foreach (ClusterCommand applicationCommand in ApplicationCommands)
		{
			collection.Add(applicationCommand);
		}
		collection.Add(OwnershipCommand);
		collection.Add(PriorityCommand);
		foreach (ClusterCommand miscellaneousCommand in MiscellaneousCommands)
		{
			collection.Add(miscellaneousCommand);
		}
		foreach (ClusterCommand groupCommand in GroupCommands)
		{
			collection.Add(groupCommand);
		}
	}

	private void CreateResourceUntilSucceed(string resourceName, ResourceType resourceType, int version = 0)
	{
		string text = resourceName;
		if (version != 0)
		{
			text = "{0} ({1})".FormatCurrentCulture(text, version.ToString(CultureInfo.CurrentCulture));
		}
		CreateResource(text, resourceType, delegate(OperationResult<Resource> opCreationResult)
		{
			if (opCreationResult.Error != null)
			{
				if (opCreationResult.Error is ClusterResourceAlreadyExistException)
				{
					CreateResourceUntilSucceed(resourceName, resourceType, ++version);
				}
				else
				{
					Error = opCreationResult.Error;
				}
			}
		});
	}

	protected virtual void InitializeGroupCommands(CommandCollection commandsCollection)
	{
		commandsCollection.Add(CreateStorageCommand(ClusterCommandCollectionId.GroupGeneral));
		commandsCollection.Add(CreateAddResourcesCommand(commandsCollection.Category));
		ClusterCommandContainer item = WeakReferenceEx.ReturnInstance(ref moreActionsCommandWeak, delegate
		{
			ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(this, "GroupMoreActions", ClusterCommandId.GroupMoreActions)
			{
				Text = CommandResources.MoreActions,
				ExecuteIfNoChildren = false
			};
			InitializeMoreActionsCommands(clusterCommandContainer);
			return clusterCommandContainer;
		});
		commandsCollection.Add(item);
		commandsCollection.Add(ClusterCommandBase<DeleteCommand>.Create(this, commandsCollection.Category));
		ClusterCommand item2 = new ClusterCommand(this, "GroupProperties", ClusterCommandId.GroupProperties, ClusterCommandCollectionId.GroupProperties)
		{
			Text = CommandResources.Properties,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for show properties.");
			},
			CommandParameter = this
		};
		commandsCollection.Add(item2);
	}

	private ClusterCommand CreateStorageCommand(ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "AddStorage", ClusterCommandId.GroupAddStorage, collectionId)
		{
			Text = CommandResources.Group_AddStorage,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for the Add Storage command.");
			},
			CommandParameter = this
		};
	}

	private ClusterCommand CreateAddResourcesCommand(ClusterCommandCollectionId collectionId)
	{
		return WeakReferenceEx.ReturnInstance(ref addResourceCommandWeak, delegate
		{
			if (!Cluster.ResourcesTypeNoWizardDict.TryGetValue(base.Cluster.CacheId, out var value))
			{
				value = new ObservableCollection<AddResourceInputParameter>();
			}
			if (!Cluster.ResourcesTypeWizardDict.TryGetValue(base.Cluster.CacheId, out var value2))
			{
				value2 = new ObservableCollection<AddResourceInputParameter>();
			}
			ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(this, "AddResource", ClusterCommandId.GroupAddResource, ClusterCommandCollectionId.GroupGeneral)
			{
				Text = CommandResources.Group_AddResource
			};
			foreach (AddResourceInputParameter item2 in value2)
			{
				AddResourceInputParameter addResourceInputParameter = item2.Clone();
				if (GroupType == GroupType.StandAloneDfs || addResourceInputParameter.ResourceType.ResourceKind != ResourceKind.DistributedFileSystem)
				{
					ClusterCommand clusterCommand = new ClusterCommand(this, "AddResourceItem", ClusterCommandId.GroupAddResourceItem, collectionId)
					{
						Text = addResourceInputParameter.Text,
						ExecuteDelegate = delegate
						{
							throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for the correct wizard.");
						}
					};
					addResourceInputParameter.Group = this;
					clusterCommand.RegisterExecuteParameter(addResourceInputParameter);
					clusterCommandContainer.ChildrenInternal.Add(clusterCommand);
				}
			}
			ClusterCommand item = new ClusterCommand(this, "MoreResources", ClusterCommandId.GroupAddMoreResource, collectionId)
			{
				InputParameters = new InputParameterList<AddResourceInputParameter>(value),
				Text = CommandResources.Group_MoreResources,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate(object x)
				{
					Exceptions.ThrowIfNull(x, "x", ExceptionResources.ArgumentNull_AddResourceCommand);
					if (!(x is AddResourceInputParameter addResourceInputParameter2))
					{
						throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsAddResourceCommand);
					}
					CreateResourceUntilSucceed(addResourceInputParameter2.SuggestedName, addResourceInputParameter2.ResourceType);
				}
			};
			clusterCommandContainer.ChildrenInternal.Add(item);
			return clusterCommandContainer;
		});
	}

	protected void SendCanExecuteUpdateIfNeeded(WeakReferenceEx weakReference)
	{
		SendCanExecuteUpdateIfNeeded(weakReference, Thread.CurrentThread.ManagedThreadId);
	}

	protected void SendCanExecuteUpdateIfNeeded(WeakReferenceEx commandWeakReference, int sourceThreadId)
	{
		if (sourceThreadId == Thread.CurrentThread.ManagedThreadId)
		{
			return;
		}
		if (commandWeakReference == null)
		{
			return;
		}
		ClusterCommand commandInner = (ClusterCommand)commandWeakReference.Target;
		if (commandInner != null)
		{
			UIHelper.ExecuteOnDispatcher(delegate
			{
				commandInner.CanExecuteUpdate(this, new ClusterEventArgs(Id, null));
			}, OperationType.Async);
		}
	}

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		base.InitializeMoreActionsCommands(commandContainer);
		ClusterCommand item = new ClusterCommand(this, "ShowDependencyReport", ClusterCommandId.GroupShowDependencyReport, ClusterCommandCollectionId.GroupGeneral)
		{
			Text = CommandResources.ShowDependencyReport,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for the Show Dependency Report.");
			},
			CommandParameter = this
		};
		commandContainer.ChildrenInternal.Add(item);
	}

	protected virtual void InitializeApplicationCommands(CommandCollection commandsCollection)
	{
		ClusterCommand manageApplicationCommand = ManageApplicationCommand;
		if (manageApplicationCommand != null)
		{
			commandsCollection.Add(manageApplicationCommand);
		}
		commandsCollection.Add((ClusterCommand)addShareCommand);
	}

	protected virtual ClusterCommand InitializePriorityCommand()
	{
		return priorityCommand.GetInstance();
	}

	protected virtual ClusterCommand InitializeManageApplicationCommand()
	{
		return null;
	}

	protected void CommandUpdate(WeakReferenceEx weakCommand, object sender, EventArgs e)
	{
		if (weakCommand != null && weakCommand.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.CanExecuteUpdate(sender, e);
		}
	}

	protected abstract ClusterCommand InitializeOwnershipCommand();

	internal static CommandCollection InitializeStateCommands(IEnumerable<Group> groups)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleGroupState)
		{
			Name = "Multi Group State Commands"
		};
		ClusterCommand item = new ClusterCommand(null, "MultiOnline", ClusterCommandId.MultipleGroupOnline, commandCollection.Category)
		{
			Text = CommandResources.BringGroupOnlineAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				BringOnline(groups);
			}
		};
		commandCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(null, "MultiOffline", ClusterCommandId.MultipleGroupOffline, commandCollection.Category)
		{
			Text = CommandResources.TakeGroupOfflineAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				TakeOffline(groups);
			}
		};
		commandCollection.Add(item2);
		return commandCollection;
	}

	internal static CommandCollection InitializeApplicationCommands(Cluster cluster, GroupType groupType, IEnumerable<Group> groups)
	{
		CommandCollection result = null;
		if (groupType == GroupType.VirtualMachine)
		{
			return VirtualMachineGroup.InitializeGroupVirtualMachineCommands(cluster, groups);
		}
		return result;
	}

	internal static CommandCollection InitializeOwnershipHeterogenousCommands(Cluster cluster, IEnumerable<Group> groups)
	{
		CommandCollection obj = new CommandCollection(ClusterCommandCollectionId.MultipleGroupMove)
		{
			Name = "Multi Group Ownership Commands"
		};
		ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(null, "MoveTo", ClusterCommandId.MultipleGroupMove, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_Move,
			CommandParameter = groups,
			ExecuteDelegate = delegate(object node)
			{
				if (node != null && !(node is Node))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotNodeMoveCommand);
				}
				Node node2 = node as Node;
				MoveGroups(groups, node2);
			}
		};
		ClusterCommand item = CreateMultipleMoveToBestNodeCommand(clusterCommandContainer, cluster, groups);
		clusterCommandContainer.ChildrenInternal.Add(item);
		ClusterCommand item2 = CreateMultipleMoveToSelectedNodeCommand(clusterCommandContainer, cluster, groups);
		clusterCommandContainer.ChildrenInternal.Add(item2);
		obj.Add(clusterCommandContainer);
		obj.AddAll(InitializePriorityCommands(cluster, groups));
		return obj;
	}

	protected static ClusterCommand CreateMultipleMoveToBestNodeCommand(ClusterCommand parentCommand, Cluster cluster, IEnumerable<Group> groups)
	{
		return new ClusterCommand(cluster, "MultiMoveTo", ClusterCommandId.GroupMoveToBest, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_MoveToBestPossible,
			CommandParameter = null,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = parentCommand.ExecuteDelegate
		};
	}

	protected static ClusterCommand CreateMultipleMoveToSelectedNodeCommand(ClusterCommand parentCommand, Cluster cluster, IEnumerable<Group> groups)
	{
		ClusterCommand clusterCommand = new ClusterCommand(cluster, "MultiMoveSelectTo", ClusterCommandId.GroupMoveToSelectedNode, ClusterCommandCollectionId.GroupOwnership);
		clusterCommand.Text = CommandResources.Group_MoveToSelected;
		clusterCommand.InputParameters = (ClusterList<Node>)((ClusterList<Node>)cluster.Nodes.OrderBy((Node node) => node.Name)).ForceLoadStart();
		clusterCommand.CommandParameter = groups;
		clusterCommand.Description = string.Empty;
		clusterCommand.CanExecuteDelegate = (object x) => true;
		clusterCommand.ExecuteDelegate = parentCommand.ExecuteDelegate;
		return clusterCommand;
	}

	internal static CommandCollection InitializeOwnershipHomogenousCommands(Cluster cluster, GroupType groupType, IEnumerable<Group> groups)
	{
		if (groupType == GroupType.VirtualMachine)
		{
			return VirtualMachineGroup.InitializeGroupVirtualMachineOwnershipCommands(cluster, groups);
		}
		return InitializeOwnershipHeterogenousCommands(cluster, groups);
	}

	internal static CommandCollection InitializePriorityCommands(Cluster cluster, IEnumerable<Group> groups)
	{
		Utilities.UnreferencedParameter(cluster);
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleGroupPriority)
		{
			Name = "Multi Group Priority Commands"
		};
		ClusterCommand item = new ClusterCommand(null, "Priority", ClusterCommandId.MultipleGroupPriority, commandCollection.Category)
		{
			InputParameters = new InputParameterList<Priority>(Priority.Fetching.GetFilterableValues().Cast<Priority>()),
			Text = "{0}",
			CanExecuteDelegate = (object priority) => true,
			ExecuteDelegate = delegate(object priority)
			{
				Exceptions.ThrowIfNull(priority, "priority", ExceptionResources.ArgumentNull_PriorityCommand);
				if (!(priority is Priority))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotPriorityCommand);
				}
				SetPriority(groups, (Priority)priority);
			}
		};
		commandCollection.Add(item);
		return commandCollection;
	}

	internal static CommandCollection InitializeOtherCommands(Cluster cluster, IEnumerable<Group> groups)
	{
		Utilities.UnreferencedParameter(cluster);
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleGroupOther)
		{
			Name = "Multi Group Other Commands"
		};
		ClusterCommand item = new ClusterCommand(null, "Delete", ClusterCommandId.MultipleGroupDelete, commandCollection.Category)
		{
			Text = CommandResources.RemoveCommand_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				DeleteGroups(groups);
			}
		};
		commandCollection.Add(item);
		return commandCollection;
	}

	public static void DeleteGroups(IEnumerable<Group> groups, bool askConfirmation = true)
	{
		Group group2 = groups.FirstOrDefault();
		if (group2 == null)
		{
			return;
		}
		GroupType groupType = group2.GroupType;
		bool flag = groups.All((Group x) => x.GroupType == groupType);
		if (askConfirmation)
		{
			ConfirmationDialog confirmationDialog = new ConfirmationDialog
			{
				Icon = TaskDialogStandardIcon.Question
			};
			if (flag && (groupType == GroupType.VirtualMachine || groupType == GroupType.FileServer || groupType == GroupType.ScaleOutFileServer))
			{
				if (groupType == GroupType.VirtualMachine)
				{
					confirmationDialog.CustomIcon = InvariantResources.VirtualMachineGroup;
					confirmationDialog.Caption = DialogResources.DeleteMultipleServicesAndApplicationsVirtualMachines_Title;
					confirmationDialog.Header = DialogResources.DeleteMultipleServicesAndApplicationsVirtualMachines_Header;
					confirmationDialog.Content = DialogResources.DeleteMultipleServicesAndApplicationsVirtualMachines_Content.FormatCurrentCulture(groups.Count());
				}
				else
				{
					confirmationDialog.CustomIcon = InvariantResources.FileServer;
					confirmationDialog.Caption = DialogResources.DeleteMultipleServicesAndApplicationsFileServers_Title;
					confirmationDialog.Header = DialogResources.DeleteMultipleServicesAndApplicationsFileServers_Header;
					confirmationDialog.Content = DialogResources.DeleteMultipleServicesAndApplicationsFileServers_Content.FormatCurrentCulture(groups.Count());
				}
			}
			else
			{
				confirmationDialog.Caption = DialogResources.DeleteMultipleServicesAndApplications_Title;
				confirmationDialog.Header = DialogResources.DeleteMultipleServicesAndApplications_Header;
				confirmationDialog.Content = DialogResources.DeleteMultipleServicesAndApplications_Content.FormatCurrentCulture(groups.Count());
			}
			if (confirmationDialog.ShowDialog() != TaskDialogResult.Yes)
			{
				return;
			}
		}
		EnqueueAndThrottleRequests(groups, delegate(Group group, Action<OperationResult> operationResult)
		{
			group.Delete(operationResult);
		});
	}

	internal static void SetPriority(IEnumerable<Group> groups, Priority priority)
	{
		EnqueueAndThrottleRequests(groups, delegate(Group group, Action<OperationResult> operationResult)
		{
			group.ChangePriorityInternal(priority, OperationType.Async, operationResult);
		});
	}

	internal static void MoveGroups(IEnumerable<Group> groups, Node node)
	{
		groups = groups.OrderByDescending((Group group) => (int)group.Priority);
		EnqueueAndThrottleRequests(groups, delegate(Group group, Action<OperationResult> operationResult)
		{
			group.Move(node, operationResult, groups.Take(2).Count() == 1);
		});
	}

	private static void BringOnline(IEnumerable<Group> groups)
	{
		groups = groups.OrderByDescending((Group group) => (int)group.Priority);
		EnqueueAndThrottleRequests(groups, delegate(Group group, Action<OperationResult> operationResult)
		{
			group.Online(delegate(OperationResult localOpResult)
			{
				group.Error = localOpResult.Error;
				operationResult(localOpResult);
			}, groups.Take(2).Count() == 1);
		});
	}

	private static void TakeOffline(IEnumerable<Group> groups)
	{
		groups = groups.OrderByDescending((Group group) => (int)group.Priority);
		EnqueueAndThrottleRequests(groups, delegate(Group group, Action<OperationResult> operationResult)
		{
			group.Offline(delegate(OperationResult localOpResult)
			{
				group.Error = localOpResult.Error;
				operationResult(localOpResult);
			}, groups.Take(2).Count() == 1);
		});
	}

	static Group()
	{
		GroupRoleToResourceMapping = new Dictionary<GroupType, ResourceKind>
		{
			{
				GroupType.DhcpServer,
				ResourceKind.DhcpService
			},
			{
				GroupType.StandAloneDfs,
				ResourceKind.DistributedFileSystem
			},
			{
				GroupType.Dtc,
				ResourceKind.Dtc
			},
			{
				GroupType.Wins,
				ResourceKind.WinsService
			},
			{
				GroupType.Msmq,
				ResourceKind.Msmsq
			},
			{
				GroupType.IScsiNameService,
				ResourceKind.IScsiNameService
			}
		};
	}

	protected Group()
		: this((string)null)
	{
	}

	protected Group(string name)
		: base(null)
	{
		SetNameInternal(name);
		Init();
	}

	private void VirtualMachineConfigResourcesQuery(OperationResult<IClusterList<Resource>> queryExecuted)
	{
		ConcurrentDictionary<Guid, Guid> csvDisksDictionary = new ConcurrentDictionary<Guid, Guid>();
		base.Cluster.LoadAsync(queryExecuted.Result, delegate(ClusterLoadedEventArgs cfgLoaded)
		{
			(cfgLoaded.Sender as VirtualMachineConfigurationResource).Properties.Get("DependsOnSharedVolumes", delegate(ClusterPropertyMultipleStrings csvVolumes)
			{
				foreach (string item in csvVolumes.TypedValue)
				{
					csvDisksDictionary.TryAdd(new Guid(item), new Guid(item));
				}
			});
		}, delegate(OperationResult cfgsLoaded)
		{
			if (cfgsLoaded.Error != null)
			{
				ClusterLog.LogException(cfgsLoaded.Error);
				SetError(cfgsLoaded.Error);
			}
			else
			{
				WeakReferenceEx weakReferenceEx = resourcesWeak;
				if (weakReferenceEx != null)
				{
					List<Guid> localCsvDisks = new List<Guid>(csvDisksDictionary.Keys);
					Group ownerGroup = this;
					weakReferenceEx.Target = (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
					{
						Name = "TopLevelResources",
						Dependency = queryExecuted.Result
					}.Where((Resource r) => ((r.OwnerGroup == ownerGroup && r.IsChild == (bool?)false) || localCsvDisks.Contains(r.Id)) && (long)r.Subclass != 268435456);
					OnPropertyChanged("TopLevelResources");
					queryExecuted.Result.CollectionChanged += delegate
					{
						resourcesWeak = null;
						OnPropertyChanged("TopLevelResources");
					};
				}
				else
				{
					OnPropertyChanged("TopLevelResources");
				}
			}
		}, 4);
	}

	public Group Init(params object[] operations)
	{
		Utilities.UnreferencedParameter(operations);
		return this;
	}

	public static void Get(Cluster cluster, Guid groupId, Action<OperationResult<Group>> operationResult, OperationType operationType)
	{
		Group group = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(groupId, ClusterIdentityType.Group, LockAccess.Reader);
				if (clusterLock != null)
				{
					group = ((PGroup)clusterLock.Owner).GetProxy();
				}
				else
				{
					PGroup pGroup = (PGroup)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Group, groupId);
					try
					{
						pGroup.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pGroup);
						throw;
					}
					group = pGroup.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Group> obj = new OperationResult<Group>(cluster, group, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public static void Get(Cluster cluster, string groupName, Action<OperationResult<Group>> operationResult, OperationType operationType)
	{
		Group group = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(groupName, ClusterIdentityType.Group, LockAccess.Reader);
				if (clusterLock != null)
				{
					group = ((PGroup)clusterLock.Owner).GetProxy();
				}
				else
				{
					PGroup pGroup = (PGroup)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Group, groupName);
					try
					{
						pGroup.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pGroup);
						throw;
					}
					group = pGroup.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Group> obj = new OperationResult<Group>(cluster, group, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public void Move(Node node, bool enableOverride = false)
	{
		Move(node, base.SetLastError, enableOverride);
	}

	public void Move(Node node, Action<OperationResult> groupOpMove, bool enableOverride = false)
	{
		ChangeOwnerNodeInternal(node, delegate(OperationResult result)
		{
			ConfirmOverrideAndExecuteOnLocked(result, delegate
			{
				ChangeOwnerNodeInternal(node, base.SetLastError, overrideLockState: true);
			}, groupOpMove, enableOverride, overrideMaintenanceMode: false);
		});
	}

	public void CancelLiveMigration()
	{
		CancelLiveMigration(base.SetLastError);
	}

	public void CancelLiveMigration(Action<OperationResult> groupOpOnline)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PGroup)lockObject.Owner).Cancel();
		}, groupOpOnline, LockAccess.Reader, setErrorOnObject: false);
	}

	public void Online(bool enableOverride = false)
	{
		Online(base.SetLastError, enableOverride);
	}

	public void Online(Action<OperationResult> groupOpOnline, bool enableOverride = false)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PGroup)lockObject.Owner).Online();
		}, delegate(OperationResult result)
		{
			ConfirmOverrideAndExecuteOnLocked(result, delegate(ILockable lockObject)
			{
				((PGroup)lockObject.Owner).Online(overrideLockState: true);
			}, groupOpOnline, enableOverride, enableOverride);
		}, LockAccess.Reader, setErrorOnObject: false);
	}

	public void Offline(bool enableOverride = false)
	{
		Offline(base.SetLastError, enableOverride);
	}

	public void Offline(Action<OperationResult> groupOpOffline, bool enableOverride = false)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PGroup)lockObject.Owner).Offline();
		}, delegate(OperationResult result)
		{
			ConfirmOverrideAndExecuteOnLocked(result, delegate(ILockable lockObject)
			{
				((PGroup)lockObject.Owner).Offline(overrideLockState: true);
			}, groupOpOffline, enableOverride, enableOverride);
		}, LockAccess.Reader, setErrorOnObject: false);
	}

	public override void Delete(bool askConfirmation = false)
	{
		Delete(base.SetLastError, askConfirmation);
	}

	public override void Delete(Action<OperationResult> groupOpDelete, bool askConfirmation = false)
	{
		CreateDeleteDialog(delegate(ConfirmationDialog confirmationDialog)
		{
			ShowDialog(confirmationDialog, delegate
			{
				DeleteOperation(groupOpDelete);
			}, null);
		}, askConfirmation);
	}

	public void CreateResource(string name, ResourceType resourceType)
	{
		CreateResource(name, resourceType, base.SetLastError);
	}

	public void CreateResource(string name, ResourceType resourceType, Action<OperationResult<Resource>> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			Resource newResource = null;
			ClusterObject.ProtectedScope(delegate
			{
				PResource pResource = ((PGroup)lockObject.Owner).CreateResource(name, resourceType);
				newResource = pResource.GetProxy();
			}, delegate(ClusterException ex)
			{
				OperationResult<Resource> obj = new OperationResult<Resource>(this, newResource, ex);
				operationResult(obj);
			});
		}, OperationType.Async, LockAccess.Reader, setErrorOnObject: false);
	}

	public void ClearFileShareErrors()
	{
		if (fileShareErrors.Count > 0)
		{
			fileShareErrors.Clear();
			Error = null;
		}
	}

	public override string ToString()
	{
		return string.Concat("Group:", Id, ":", base.Name, ":", base.IsOpen.ToString());
	}

	internal Group(Cluster cluster)
		: base(cluster)
	{
		Init();
	}

	private void Init()
	{
		icon = new WeakLazy<Icon2>(GenerateIcon);
		stateCommandCollectionWrapper = new StateCommandCollectionWrapper(this);
		priorityCommand = new PriorityCommand(this);
		addShareCommand = new AddShareCommand(this);
		lazyShares = new WeakLazy<FileShareCollection>((Func<FileShareCollection>)delegate
		{
			Task.Factory.StartNew(delegate
			{
				Thread.Sleep(1000);
				FileShareCollection fileShareCollection = lazyShares.TryGetInstance();
				if (fileShareCollection == null)
				{
					lazyShares.ReleaseReference();
				}
				else
				{
					fileShareCollection.Subscribe();
				}
			});
			return new FileShareCollection(this);
		});
	}

	protected virtual void CreateDeleteDialog(Action<ConfirmationDialog> confirmationDialogCreation, bool createDialog)
	{
		if (confirmationDialogCreation != null)
		{
			if (!createDialog)
			{
				confirmationDialogCreation(null);
			}
			else
			{
				AllResourcesNonCsv.ExecuteQuery(ResultExecution.OnDispatcher, AllResourcesNonCsvQuery, confirmationDialogCreation);
			}
		}
	}

	private void AllResourcesNonCsvQuery(OperationResult<IClusterList<Resource>> operationResult)
	{
		Action<ConfirmationDialog> confirmationDialogCreation = (Action<ConfirmationDialog>)operationResult.Parameter;
		ConfirmationDialog confirmation = new ConfirmationDialog
		{
			CustomIcon = Icon.NativeIcon,
			Caption = DialogResources.DeleteServiceOrApplication_Title.FormatCurrentCulture(GroupType.Translate()),
			Header = DialogResources.DeleteServiceOrApplication_Header.FormatCurrentCulture(DisplayName)
		};
		Resource netNameResource = Enumerable.FirstOrDefault(AllResourcesNonCsv, (Resource resource) => resource.ResourceType.ResourceKind == ResourceKind.NetworkName || resource.ResourceType.ResourceKind == ResourceKind.DistributedNetworkName);
		if (netNameResource != null)
		{
			netNameResource.ResourceType.LoadAsync(delegate
			{
				netNameResource.ResourceType.Properties.Get("DeleteVcoOnResCleanup", delegate(ClusterPropertyUInt deleteVcoProperty)
				{
					confirmation.Content = confirmation.Content.AppendLine((deleteVcoProperty.TypedValue == 0) ? DialogResources.DeleteServiceOrApplicationNetNameNotRemoveVCO_Content : DialogResources.DeleteServiceOrApplicationNetName_Content);
				});
				confirmationDialogCreation(confirmation);
			}, ResourceTypeLoadSelection.PrivateProperties);
			return;
		}
		int count = AllResourcesNonCsv.Count;
		if (count == 1)
		{
			confirmation.Content = DialogResources.DeleteServiceOrApplicationSingle_Content;
		}
		else if (count > 1)
		{
			confirmation.Content = DialogResources.DeleteServiceOrApplicationMany_Content.FormatCurrentCulture(count);
		}
		confirmationDialogCreation(confirmation);
	}

	protected abstract Icon2 GenerateIcon();

	protected virtual void DeleteOperation(Action<OperationResult> groupOpDelete)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.Delete();
		}, groupOpDelete, LockAccess.Reader, setErrorOnObject: false);
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents: false, ignorePossibleOwners);
		PGroup pGroup = (PGroup)privateObject;
		SetIdInternal(pGroup.Id);
		SetNameInternal(pGroup.Name);
		groupState = pGroup.GroupState;
		priority = pGroup.Priority;
		isCore = pGroup.IsCore;
		IsHidden = pGroup.IsHidden;
		flags = pGroup.Flags;
		groupType = pGroup.GroupType;
		ownerNode = ((pGroup.OwnerNode != null) ? pGroup.OwnerNode.GetProxy() : null);
		base.LoadSelection = pGroup.LoadedSelection;
		Properties = pGroup.Properties;
		if (!ignorePossibleOwners)
		{
			preferredOwners = NodesFromNodeIds(base.Cluster, pGroup.PreferredOwners, subscribeToEvents: false);
		}
		ParseProperties(pGroup.Properties, trackChanges: false);
		PostCreate(pGroup);
		if (subscribeToEvents)
		{
			SubscribeToEvents(pGroup);
		}
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		if (ParseProperty(properties, "StatusInformation", ref subStatus, list))
		{
			list.TryAdd("GroupSubStatus");
			list.TryAdd("ApplicationStatus");
		}
		return list;
	}

	private void ChangeStateInternal(GroupState newState, OperationType operationType)
	{
		Utilities.UnreferencedParameter(operationType);
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			PGroup pGroup = (PGroup)lockObject.Owner;
			switch (newState)
			{
			case GroupState.Online:
				pGroup.Online();
				break;
			case GroupState.Offline:
				pGroup.Offline();
				break;
			}
		}, base.PropertiesOperationType, LockAccess.Reader);
	}

	private void ChangePriorityInternal(Priority newPriority, OperationType operationType, Action<OperationResult> operationResult)
	{
		Utilities.UnreferencedParameter(operationType);
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PGroup)lockObject.Owner).SetPriority(newPriority);
		}, base.PropertiesOperationType, operationResult, LockAccess.Reader);
	}

	private void ChangeOwnerNodeInternal(Node newNode, Action<OperationResult> groupOpMove = null, bool overrideLockState = false)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PGroup)lockObject.Owner).Move((newNode == null) ? null : newNode.Name, overrideLockState);
		}, groupOpMove, LockAccess.Reader);
	}

	protected virtual void OnResourceAdded(ClusterGainedEventArgs args)
	{
	}

	protected virtual void OnResourceRemoved(ClusterLostEventArgs args)
	{
	}

	protected void RefreshStorageResources()
	{
		resourcesWeak = null;
		storageResourcesWeak = null;
		OnPropertyChanged("TopLevelResources");
		OnPropertyChanged("PhysicalDiskResources");
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		groupState = null;
		subStatus = null;
		priority = null;
		resourcesWeak = null;
		allResourcesWeak = null;
		allResourcesNonCsvWeak = null;
		storageResourcesWeak = null;
		moveTargetsWeak = null;
		preferredOwners = null;
		clientAccessPointsWeak = null;
		lazyShares.ReleaseReference();
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("ApplicationStatus");
			OnPropertyChanged("GroupState");
			OnPropertyChanged("GroupSubStatus");
			OnPropertyChanged("Properties");
			OnPropertyChanged("Priority");
			OnPropertyChanged("OwnerNode");
			OnPropertyChanged("OwnerNodeName");
			OnPropertyChanged("IsCore");
			OnPropertyChanged("Flags");
			OnPropertyChanged("AllResources");
			OnPropertyChanged("PhysicalDiskResources");
			OnPropertyChanged("TopLevelResources");
			OnPropertyChanged("PreferredOwners");
			OnPropertyChanged("ClientAccessPoints");
			OnPropertyChanged("Shares");
			this.StateChanged.SafeCall(this, null);
			this.ApplicationStatusChanged.SafeCall(this, null);
			this.PriorityChanged.SafeCall(this, null);
			this.OwnerNodeChanged.SafeCall(this, null);
			this.IsCoreChanged.SafeCall(this, null);
			this.FlagsChanged.SafeCall(this, null);
			this.PreferredOwnersChanged.SafeCall(this, null);
		}, OperationType.Async);
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.Gained:
			if (e.EventArgument is ClusterGainedEventArgs args9)
			{
				OnResourceAdded(args9);
			}
			break;
		case EventType.Lost:
			if (e.EventArgument is ClusterLostEventArgs args10)
			{
				OnResourceRemoved(args10);
			}
			break;
		case EventType.GroupStateChanged:
		{
			ClusterGroupStateEventArgs args5 = e.EventArgument as ClusterGroupStateEventArgs;
			if (groupState != args5.State || args5.Error != null)
			{
				groupState = args5.State;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("GroupState");
					OnPropertyChanged("ApplicationStatus");
					this.StateChanged.SafeCall(this, args5);
					OnApplicationStatusChanged(new ClusterApplicationStatusEventArgs(args5.Id, args5.Error));
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.GroupTypeChanged:
		{
			ClusterGroupTypeEventArgs args8 = e.EventArgument as ClusterGroupTypeEventArgs;
			if (groupType != args8.GroupType || args8.Error != null)
			{
				groupType = args8.GroupType;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("GroupType");
					this.GroupTypeChanged.SafeCall(this, args8);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.GroupPriorityChanged:
		{
			ClusterGroupPriorityEventArgs args2 = e.EventArgument as ClusterGroupPriorityEventArgs;
			if (priority != args2.Priority || args2.Error != null)
			{
				priority = args2.Priority;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("Priority");
					this.PriorityChanged.SafeCall(this, args2);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.GroupIsCoreChanged:
		{
			ClusterGroupIsCoreEventArgs args7 = e.EventArgument as ClusterGroupIsCoreEventArgs;
			if (isCore != args7.IsCore || args7.Error != null)
			{
				isCore = args7.IsCore;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("IsCore");
					this.IsCoreChanged.SafeCall(this, args7);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.Hidden:
		{
			ClusterIsHiddenEventArgs clusterIsHiddenEventArgs = e.EventArgument as ClusterIsHiddenEventArgs;
			if (IsHidden != clusterIsHiddenEventArgs.IsHidden || clusterIsHiddenEventArgs.Error != null)
			{
				IsHidden = clusterIsHiddenEventArgs.IsHidden;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("IsHidden");
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.GroupFlagsChanged:
		{
			ClusterGroupFlagsEventArgs args4 = e.EventArgument as ClusterGroupFlagsEventArgs;
			if (flags != args4.Flags || args4.Error != null)
			{
				flags = args4.Flags;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("Flags");
					this.FlagsChanged.SafeCall(this, args4);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.GroupOwnerNodeChanged:
		{
			ClusterGroupOwnerNodeEventArgs args6 = e.EventArgument as ClusterGroupOwnerNodeEventArgs;
			if (ownerNode != null && ownerNode.Id == args6.NodeId && args6.Error == null)
			{
				break;
			}
			ManualResetEventSlim resetEvent = args6.WaitEvent();
			Node.Get(base.Cluster, args6.NodeId, delegate(OperationResult<Node> nodeOpGet)
			{
				bool updated = false;
				try
				{
					if (nodeOpGet.Error is ClusterObjectNotFoundException)
					{
						return;
					}
					if (nodeOpGet.Error != null)
					{
						Error = nodeOpGet.Error;
						return;
					}
					this.ExecuteMethod(delegate(ILockable groupLock)
					{
						if (groupLock != null)
						{
							ownerNode = nodeOpGet.Result;
							moveTargetsWeak = null;
							updated = true;
						}
					}, LockAccess.Reader);
				}
				finally
				{
					resetEvent.Set();
				}
				if (updated)
				{
					UIHelper.ExecuteOnDispatcher(delegate
					{
						OnPropertyChanged("OwnerNode");
						OnPropertyChanged("OwnerNodeName");
						this.OwnerNodeChanged.SafeCall(this, args6);
					}, OperationType.Async);
				}
			}, OperationType.Async);
			ClearFileShareErrors();
			break;
		}
		case EventType.GroupPreferredOwnersChanged:
		{
			ClusterGroupPreferredOwnersChangedEventArgs args = e.EventArgument as ClusterGroupPreferredOwnersChangedEventArgs;
			if (args.Error != null || args.PreferredNodes == null)
			{
				preferredOwners = null;
				break;
			}
			preferredOwners = NodesFromNodeIds(base.Cluster, args.PreferredNodes, subscribeToEvents: false);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("PreferredOwners");
				this.PreferredOwnersChanged.SafeCall(this, args);
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs args3 = e.EventArgument as ClusterPropertiesEventArgs;
			if (args3.Error != null)
			{
				break;
			}
			IEnumerable<string> propertiesChanged = ParseProperties(args3.Properties, trackChanges: true);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				propertiesChanged.ForEach(delegate(string propertyChanged)
				{
					OnPropertyChanged(propertyChanged);
					if (propertyChanged == "ApplicationStatus")
					{
						this.ApplicationStatusChanged.SafeCall(sender, new ClusterApplicationStatusEventArgs(args3.Id, args3.Error));
					}
				});
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		case EventType.ResourceTypeChanged:
			mCommands = null;
			addResourceCommandWeak = null;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("Commands");
			}, OperationType.Async, queueOnDispatcher);
			break;
		case EventType.ForwardPayload:
		{
			ClusterForwardedEventArgs clusterForwardedEventArgs = e.EventArgument as ClusterForwardedEventArgs;
			if (clusterForwardedEventArgs.Key != 4 || !(clusterForwardedEventArgs.ForwardedPayload is ClusterFileShareErrorEventArgs clusterFileShareErrorEventArgs))
			{
				break;
			}
			fileShareErrors.AddRange(clusterFileShareErrorEventArgs.ErrorItems);
			IEnumerable<string> source = fileShareErrors.Where((FileShareErrorItem errorItem) => errorItem.Protocol == FileShareProtocol.Smb).Select(delegate(FileShareErrorItem errorItem)
			{
				if (errorItem.ErrorType == CimObservableErrorType.ServerQuotaReached)
				{
					return CommonResources.FileShareServerBusy_Format.FormatCurrentCulture(errorItem.NetName);
				}
				return (errorItem.ErrorType == CimObservableErrorType.ConnectionFailure) ? CommonResources.FileShareConnectionError_Format.FormatCurrentCulture(errorItem.NetName) : CommonResources.FileShareGenericError_Format.FormatCurrentCulture(errorItem.NetName);
			});
			string text = string.Join(Environment.NewLine, source.Distinct());
			if (!string.IsNullOrWhiteSpace(text))
			{
				Error = new ClusterDialogException(CommonResources.FileShareError_Text)
				{
					Details = text,
					Header = CommonResources.FileShareError_Text
				};
			}
			break;
		}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private static ReadOnlyObservableCollection<Node> NodesFromNodeIds(Cluster cluster, IEnumerable<Guid> nodesId, bool subscribeToEvents)
	{
		Utilities.UnreferencedParameter(subscribeToEvents);
		if (cluster == null)
		{
			return null;
		}
		if (nodesId == null)
		{
			return null;
		}
		ObservableCollection<Node> preferredOwners = new ObservableCollection<Node>();
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			foreach (Guid item in nodesId)
			{
				using ClusterLock clusterLock = ((PCluster)lockObject.Owner).CacheManager.Get(item, ClusterIdentityType.Node, LockAccess.Reader);
				if (clusterLock != null)
				{
					Node proxy = ((PNode)clusterLock.Owner).GetProxy();
					preferredOwners.Add(proxy);
				}
			}
		}, LockAccess.Reader);
		return new ReadOnlyObservableCollection<Node>(preferredOwners);
	}

	protected ProcessStartInfo GetExtensionStartInfoWithEnvironmentVariables()
	{
		ProcessStartInfo processStartInfo = ProcessHelper.CreateSnapinExtensionStartInfo();
		string roleSpecificClientAccessPointServerName = GetRoleSpecificClientAccessPointServerName(this);
		processStartInfo.EnvironmentVariables["CLUSTER_SNAPIN_EXTENSION_NAME"] = GroupRoleToResourceMapping[GroupType].Translate();
		processStartInfo.EnvironmentVariables["CLUSTER_SNAPIN_VIRTUAL_SERVER_FULL_NAME"] = roleSpecificClientAccessPointServerName;
		processStartInfo.EnvironmentVariables["CLUSTER_SNAPIN_VIRTUAL_SERVER_NAME"] = roleSpecificClientAccessPointServerName;
		return processStartInfo;
	}

	internal static string GetRoleSpecificClientAccessPointServerName(Group group)
	{
		Resource netNameFromRoleSpecificResource = GetNetNameFromRoleSpecificResource(group);
		if (netNameFromRoleSpecificResource.ResourceState != ResourceState.Online)
		{
			throw new ClusterClientAccessPointNotOnlineException(netNameFromRoleSpecificResource);
		}
		return netNameFromRoleSpecificResource.Properties.Get("DnsName", delegate(ClusterPropertyString serverName)
		{
			if (!DnsSupport.IsNetworkNameReady(serverName.TypedValue))
			{
				throw new ClusterClientAccessPointNotAvailableException(serverName.TypedValue);
			}
			return serverName.TypedValue;
		});
	}

	internal static Resource GetNetNameFromRoleSpecificResource(Group group)
	{
		bool moreThanOne = false;
		Resource toReturn = null;
		Cluster cluster = group.Cluster;
		if (!GroupRoleToResourceMapping.ContainsKey(group.GroupType))
		{
			throw new InvalidOperationException(ExceptionResources.UnableToFindRoleSpecificResource.FormatCurrentCulture(group.GroupType.Translate()));
		}
		ResourceKind resourceKind = GroupRoleToResourceMapping[group.GroupType];
		foreach (Resource allResource in group.AllResources)
		{
			if (allResource.ResourceType.ResourceKind != resourceKind)
			{
				continue;
			}
			if (allResource.Dependencies == null)
			{
				throw new InvalidOperationException("The role specific resource must already have its dependencies loaded in order to call this function.");
			}
			foreach (Guid dependencyId in allResource.Dependencies)
			{
				Resource.Get(cluster, dependencyId, delegate(OperationResult<Resource> result)
				{
					if (result.Error != null)
					{
						ClusterLog.LogException(LogLevel.Info, result.Error, "There was an error getting a dependency '{0}'".FormatCurrentCulture(dependencyId));
					}
					else if (result.Result != null && (result.Result.ResourceType.ResourceKind == ResourceKind.NetworkName || result.Result.ResourceType.ResourceKind == ResourceKind.DistributedNetworkName))
					{
						if (toReturn == null)
						{
							toReturn = result.Result;
						}
						else
						{
							moreThanOne = true;
						}
					}
				}, OperationType.Sync);
			}
			if (moreThanOne)
			{
				throw new ClusterClientAccessPointException(ExceptionResources.DuplicateClientAccessPointDependencies_Text.FormatCurrentCulture(group.GroupType.Translate(), group.DisplayName));
			}
		}
		if (toReturn == null)
		{
			throw new ClusterClientAccessPointException(ExceptionResources.NoClientAccessPointDependencies_Text.FormatCurrentCulture(group.GroupType.Translate(), group.DisplayName));
		}
		return toReturn;
	}

	internal bool CanExecuteOnRoleSpecificResourcesClientAccessPoint(Group group)
	{
		Exceptions.ThrowIfNull(group, "group");
		if (group.IsResourceSpecificClientAccessPointDependenciesLoaded)
		{
			return true;
		}
		Cluster cluster = group.Cluster;
		if (!GroupRoleToResourceMapping.ContainsKey(group.GroupType))
		{
			throw new InvalidOperationException(ExceptionResources.UnableToFindRoleSpecificResource.FormatCurrentCulture(group.GroupType.Translate()));
		}
		ResourceKind importantResourceType = GroupRoleToResourceMapping[group.GroupType];
		int originalThreadId = Thread.CurrentThread.ManagedThreadId;
		group.AllResources.ExecuteQuery(delegate(OperationResult<IClusterList<Resource>> opResult)
		{
			if (opResult.Error != null)
			{
				throw opResult.Error;
			}
			foreach (Resource item in opResult.Result)
			{
				if (item.ResourceType.ResourceKind == importantResourceType)
				{
					Resource resourceCopy = item;
					resourceCopy.LoadAsync(delegate(ClusterLoadedEventArgs dependenciesLoadedEventArgs)
					{
						if (dependenciesLoadedEventArgs.Error != null)
						{
							ClusterLog.LogException(dependenciesLoadedEventArgs.Error, "Unable to load dependencies for important resource for manage command.");
							EnableManageCommand(group, originalThreadId);
							return;
						}
						foreach (Guid dependencyId in resourceCopy.Dependencies)
						{
							Resource.Get(cluster, dependencyId, delegate(OperationResult<Resource> result)
							{
								if (result.Error != null)
								{
									ClusterLog.LogException(LogLevel.Info, result.Error, "There was an error getting the dependency '{0}'".FormatCurrentCulture(dependencyId));
								}
								if (result.Result == null)
								{
									EnableManageCommand(group, originalThreadId);
								}
								else if (result.Result.ResourceType.ResourceKind == ResourceKind.NetworkName || result.Result.ResourceType.ResourceKind == ResourceKind.DistributedNetworkName)
								{
									result.Result.LoadAsync(delegate(ClusterLoadedEventArgs privatePropertiesLoadedEventArgs)
									{
										if (privatePropertiesLoadedEventArgs.Error != null)
										{
											ClusterLog.LogException(privatePropertiesLoadedEventArgs.Error, "Unable to load private properties for important CAP resource for manage command.");
										}
										EnableManageCommand(group, originalThreadId);
									}, ResourceLoadSelection.PrivateProperties);
								}
							}, OperationType.Sync);
						}
					}, ResourceLoadSelection.Dependencies);
					break;
				}
			}
		});
		return false;
	}

	private void EnableManageCommand(Group group, int sourceThreadId)
	{
		group.IsResourceSpecificClientAccessPointDependenciesLoaded = true;
		SendCanExecuteUpdateIfNeeded(manageApplicationCommandWeak, sourceThreadId);
	}

	protected internal void OnApplicationStatusChanged(ClusterApplicationStatusEventArgs args)
	{
		this.ApplicationStatusChanged.SafeCall(this, args);
	}

	protected static void EnqueueAndThrottleRequests(IEnumerable<Group> groups, Action<Group, Action<OperationResult>> operationToExecute)
	{
		ClusterObject.EnqueueAndThrottleRequests(groups.Cast<ClusterObject>(), delegate(ClusterObject clusterObject, Action<OperationResult> operationResult)
		{
			if (operationToExecute != null)
			{
				operationToExecute((Group)clusterObject, operationResult);
			}
		});
	}

	internal virtual void PostCreate(PGroup privateGroup)
	{
	}

	public static bool operator ==(Group node1, Group node2)
	{
		return node1?.Equals(node2) ?? ((object)node2 == null);
	}

	public static bool operator !=(Group node1, Group node2)
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

	protected override void AskForResourceLockOverride(string item, Action overrideAction)
	{
		if (CreateConfirmationDialog(DialogResources.OverrideChildLockOrMM_Title, DialogResources.OverrideChildLockOrMM_ConfirmationMessage.FormatCurrentCulture(item)).ShowDialog() == TaskDialogResult.Yes)
		{
			overrideAction();
		}
	}
}

