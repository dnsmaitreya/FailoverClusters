using System;
using System.Collections.Generic;
using System.Linq;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class VirtualMachineGroup : Group
{
	private WeakReferenceEx connectCommandWeak;

	private WeakReferenceEx manageCommandWeak;

	private WeakReferenceEx settingsCommandWeak;

	private WeakReferenceEx configureMonitoringCommandWeak;

	private WeakReferenceEx startCommandWeak;

	private WeakReferenceEx turnoffCommandWeak;

	private WeakReferenceEx shutdownCommandWeak;

	private WeakReferenceEx saveCommandWeak;

	private VirtualMachineMoveCommand virtualMachineMoveCommand;

	private VirtualMachineDeleteSavedStateCommand virtualMachineDeleteSavedStateCommand;

	private VirtualMachinePauseCommand virtualMachinePauseCommand;

	private VirtualMachineResumeCommand virtualMachineResumeCommand;

	private VirtualMachineResetCommand virtualMachineResetCommand;

	private VirtualMachineCancelLiveMigrationCommand virtualMachineCancelLiveMigrationCommand;

	private readonly Dictionary<Guid, VirtualMachineState> memberAllVirtualMachinesState = new Dictionary<Guid, VirtualMachineState>();

	private readonly Queue<Action<OperationResult<VirtualMachineResource>>> callbackLoadQueue = new Queue<Action<OperationResult<VirtualMachineResource>>>();

	private VirtualMachineResource vmResource;

	private bool? vmResourceLoaded = false;

	private VirtualMachineState? memberVirtualMachineState;

	private WeakReferenceEx virtualMachinesWeak;

	private ClusterException lastVmError;

	private VirtualMachineComputerSystemOperationalStatus? migrationState;

	private int? migrationProgress;

	private VirtualMachineReplicationHealth replicationHealth;

	private ulong? lastOperationStatusCode;

	private string lastVmOrVmConfigResourceErrorName;

	public override GroupType GroupType => GroupType.VirtualMachine;

	private ApplicationStatus ApplicationStatusBackStore { get; set; }

	[PropertyStore("ApplicationStatusBackStore")]
	public override ApplicationStatus ApplicationStatus
	{
		get
		{
			ApplicationStatus applicationStatus = ((base.GroupState == GroupState.Failed) ? base.GroupState.ToApplicationStatus() : (base.GroupSubStatus.HasFlag(GroupSubStatus.Unmonitored) ? ApplicationStatus.Unmonitored : ((memberVirtualMachineState.HasValue && memberAllVirtualMachinesState.Count == 1) ? memberVirtualMachineState.Value.ToApplicationStatus(base.GroupState, base.GroupSubStatus, MigrationState) : base.GroupState.ToApplicationStatus())));
			if (base.LastApplicationStatus != applicationStatus)
			{
				base.LastApplicationStatus = applicationStatus;
				Error = null;
			}
			return applicationStatus;
		}
	}

	public override VirtualMachineReplicationHealth ReplicationHealth => replicationHealth;

	public VirtualMachineComputerSystemOperationalStatus MigrationState => LoadAsync<VirtualMachineComputerSystemOperationalStatus, VirtualMachineComputerSystemOperationalStatus>(migrationState, 0).GetMigrationState();

	public int? MigrationProgress => migrationProgress;

	public ClusterList<Resource> VirtualMachines => WeakReferenceEx.ReturnInstance(ref virtualMachinesWeak, delegate
	{
		Group ownerGroup = this;
		return (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
		{
			Name = "Virtual Machines"
		}.Where((Resource r) => r.OwnerGroup == ownerGroup && (int)r.ResourceType.ResourceKind == 21);
	});

	internal VirtualMachineState? ApplicationVirtualMachineState => memberVirtualMachineState;

	protected override void InitializeCommands(CommandCollection collection)
	{
		foreach (ClusterCommand applicationCommand in ApplicationCommands)
		{
			collection.Add(applicationCommand);
		}
		collection.Add(OwnershipCommand);
		collection.Add(virtualMachineCancelLiveMigrationCommand.GetInstance());
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

	protected override void InitializeApplicationCommands(CommandCollection commandsCollection)
	{
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref connectCommandWeak, () => VirtualMachineResource.GetConnectCommand(this, commandsCollection.Category, delegate
		{
			SendCanExecuteUpdateIfNeeded(connectCommandWeak);
		}, ClusterCommandId.VirtualMachineGroupConnect));
		base.ApplicationStatusChanged += ConnectCommandUpdate;
		clusterCommand.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ConnectCommandUpdate;
			connectCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand);
		ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref startCommandWeak, () => VirtualMachineResource.GetStartCommand(this, commandsCollection.Category, delegate
		{
			SendCanExecuteUpdateIfNeeded(startCommandWeak);
		}, ClusterCommandId.VirtualMachineGroupStart));
		base.ApplicationStatusChanged += StartCommandUpdate;
		clusterCommand2.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= StartCommandUpdate;
			startCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand2);
		ClusterCommand clusterCommand3 = WeakReferenceEx.ReturnInstance(ref saveCommandWeak, () => VirtualMachineResource.GetSaveCommand(this, commandsCollection.Category, delegate
		{
			SendCanExecuteUpdateIfNeeded(saveCommandWeak);
		}, ClusterCommandId.VirtualMachineGroupSave));
		base.ApplicationStatusChanged += SaveCommandUpdate;
		clusterCommand3.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= SaveCommandUpdate;
			saveCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand3);
		ClusterCommand clusterCommand4 = WeakReferenceEx.ReturnInstance(ref shutdownCommandWeak, () => VirtualMachineResource.GetShutdownCommand(this, commandsCollection.Category, delegate
		{
			SendCanExecuteUpdateIfNeeded(shutdownCommandWeak);
		}, ClusterCommandId.VirtualMachineGroupShutdown));
		base.ApplicationStatusChanged += ShutdownCommandUpdate;
		clusterCommand4.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ShutdownCommandUpdate;
			shutdownCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand4);
		ClusterCommand clusterCommand5 = WeakReferenceEx.ReturnInstance(ref turnoffCommandWeak, () => VirtualMachineResource.GetTurnOffCommand(this, commandsCollection.Category, delegate
		{
			SendCanExecuteUpdateIfNeeded(turnoffCommandWeak);
		}, ClusterCommandId.VirtualMachineGroupTurnoff));
		base.ApplicationStatusChanged += TurnOffCommandUpdate;
		clusterCommand5.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= TurnOffCommandUpdate;
			turnoffCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand5);
		ClusterCommand clusterCommand6 = WeakReferenceEx.ReturnInstance(ref settingsCommandWeak, () => VirtualMachineResource.GetSettingsCommand(this, ClusterCommandCollectionId.AdvancedApplication, delegate
		{
			SendCanExecuteUpdateIfNeeded(settingsCommandWeak);
		}, ClusterCommandId.VirtualMachineGroupSettings));
		base.ApplicationStatusChanged += SettingsCommandUpdate;
		clusterCommand6.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= SettingsCommandUpdate;
			settingsCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand6);
		ClusterCommand manageCommand = WeakReferenceEx.ReturnInstance(ref manageCommandWeak, () => VirtualMachineResource.GetManageCommand(this, ClusterCommandCollectionId.AdvancedApplication, delegate
		{
			SendCanExecuteUpdateIfNeeded(manageCommandWeak);
		}, ClusterCommandId.VirtualMachineGroupManage));
		base.ApplicationStatusChanged += ManageCommandUpdate;
		manageCommand.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ManageCommandUpdate;
			manageCommandWeak = null;
		};
		commandsCollection.Add(manageCommand);
		commandsCollection.Add(base.AddShareCommand);
		LoadVmResource(delegate(OperationResult<VirtualMachineResource> virtualMachineOperationLoad)
		{
			if (virtualMachineOperationLoad.Error != null)
			{
				ClusterLog.LogException(virtualMachineOperationLoad.Error);
			}
			else
			{
				VirtualMachineResource virtualMachineResource = virtualMachineOperationLoad.Result;
				virtualMachineResource.LoadAsync(163840);
				if (virtualMachineOperationLoad.Parameter != null)
				{
					commandsCollection.Add(virtualMachineResource.GetReplicationCommandsContainer(this, commandsCollection, ClusterIdentityType.Group, delegate
					{
						RefreshCommands();
					}));
				}
				else
				{
					OnCommandCollection(delegate(CommandCollection commandCollection)
					{
						commandCollection.Insert(commandCollection.IndexOf(manageCommand) + 1, virtualMachineResource.GetReplicationCommandsContainer(this, commandsCollection, ClusterIdentityType.Group, delegate
						{
							RefreshCommands();
						}));
						RefreshCommands();
					});
				}
			}
		});
	}

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		base.InitializeMoreActionsCommands(commandContainer);
		ClusterCommand configureMonitoringCommand = WeakReferenceEx.ReturnInstance(ref configureMonitoringCommandWeak, () => VirtualMachineResource.GetConfigureMonitoringCommand(this, ClusterCommandCollectionId.Group, delegate
		{
			SendCanExecuteUpdateIfNeeded(configureMonitoringCommandWeak);
		}));
		ExecuteOnVmResource(delegate
		{
			configureMonitoringCommand.Visible = CalculateConfigureMonitoringVisibility();
		});
		base.ApplicationStatusChanged += ConfigureMonitoringCommandUpdate;
		configureMonitoringCommand.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ConfigureMonitoringCommandUpdate;
			configureMonitoringCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(configureMonitoringCommand);
		foreach (ClusterCommand stateCommand in StateCommands)
		{
			commandContainer.ChildrenInternal.Add(stateCommand);
		}
		commandContainer.ChildrenInternal.Add(virtualMachineDeleteSavedStateCommand.GetInstance());
		commandContainer.ChildrenInternal.Add(virtualMachinePauseCommand.GetInstance());
		commandContainer.ChildrenInternal.Add(virtualMachineResumeCommand.GetInstance());
		commandContainer.ChildrenInternal.Add(virtualMachineResetCommand.GetInstance());
	}

	protected override ClusterCommand InitializeOwnershipCommand()
	{
		LoadVmResource(null);
		return virtualMachineMoveCommand.GetInstance();
	}

	private void ConnectCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(connectCommandWeak, sender, e);
	}

	private void ManageCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(manageCommandWeak, sender, e);
	}

	private void SettingsCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(settingsCommandWeak, sender, e);
	}

	private void ConfigureMonitoringCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(configureMonitoringCommandWeak, sender, e);
		if (configureMonitoringCommandWeak != null && configureMonitoringCommandWeak.Target != null)
		{
			ClusterCommand command = (ClusterCommand)configureMonitoringCommandWeak.Target;
			ExecuteOnVmResource(delegate
			{
				command.Visible = CalculateConfigureMonitoringVisibility();
			});
		}
	}

	private void StartCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(startCommandWeak, sender, e);
	}

	private void TurnOffCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(turnoffCommandWeak, sender, e);
	}

	private void ShutdownCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(shutdownCommandWeak, sender, e);
	}

	private void SaveCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(saveCommandWeak, sender, e);
	}

	internal static CommandCollection InitializeGroupVirtualMachineCommands(Cluster cluster, IEnumerable<Group> groups)
	{
		Utilities.UnreferencedParameter(cluster);
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleGroupStateVirtualMachine)
		{
			Name = "Multi Virtual Machine Group Commands"
		};
		ClusterCommand item = new ClusterCommand(null, "MultiConnect", ClusterCommandId.VirtualMachineMultipleGroupConnect, commandCollection.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Connect,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineConnect(groups.Cast<VirtualMachineGroup>());
			}
		};
		commandCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(null, "MultiStart", ClusterCommandId.VirtualMachineMultipleGroupStart, commandCollection.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Start,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineStart(groups.Cast<VirtualMachineGroup>());
			}
		};
		commandCollection.Add(item2);
		ClusterCommand item3 = new ClusterCommand(null, "MultiSave", ClusterCommandId.VirtualMachineMultipleGroupSave, commandCollection.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Save,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineSave(groups.Cast<VirtualMachineGroup>());
			}
		};
		commandCollection.Add(item3);
		ClusterCommand item4 = new ClusterCommand(null, "MultiShutdown", ClusterCommandId.VirtualMachineMultipleGroupShutdown, commandCollection.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Shutdown,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineShutdown(groups.Cast<VirtualMachineGroup>(), askConfirmation: true);
			}
		};
		commandCollection.Add(item4);
		ClusterCommand item5 = new ClusterCommand(null, "MultiTurnOff", ClusterCommandId.VirtualMachineMultipleGroupTurnoff, commandCollection.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_TurnOff,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineTurnoff(groups.Cast<VirtualMachineGroup>(), askConfirmation: true);
			}
		};
		commandCollection.Add(item5);
		ClusterCommand item6 = new ClusterCommand(null, "MultiCheckpoint", ClusterCommandId.VirtualMachineGroupMultipleCheckpoint, commandCollection.Category)
		{
			Text = CommandResources.VirtualMachineCheckpointCommand_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineTakeCheckpoint(groups.Cast<VirtualMachineGroup>());
			}
		};
		commandCollection.Add(item6);
		return commandCollection;
	}

	private static IEnumerable<ClusterCommand> InitializeMultiSelectMoreActions(IEnumerable<Group> groups, CommandCollection virtualMachineCommands)
	{
		foreach (ClusterCommand item in Group.InitializeStateCommands(groups))
		{
			yield return item;
		}
		yield return new ClusterCommand(null, "MultiDeleteSavedState", ClusterCommandId.VirtualMachineMultipleGroupSave, virtualMachineCommands.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_DeleteSavedState,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineDeleteSavedState(groups.Cast<VirtualMachineGroup>(), askConfirmation: true);
			}
		};
		yield return new ClusterCommand(null, "MultiPause", ClusterCommandId.VirtualMachineMultipleGroupPause, virtualMachineCommands.Category)
		{
			Text = CommandResources.PauseAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachinePause(groups.Cast<VirtualMachineGroup>());
			}
		};
		yield return new ClusterCommand(null, "MultiResume", ClusterCommandId.VirtualMachineMultipleGroupResume, virtualMachineCommands.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Resume,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineResume(groups.Cast<VirtualMachineGroup>());
			}
		};
		yield return new ClusterCommand(null, "MultiReset", ClusterCommandId.VirtualMachineMultipleGroupReset, virtualMachineCommands.Category)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Reset,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				VirtualMachineReset(groups.Cast<VirtualMachineGroup>(), askConfirmation: true);
			}
		};
	}

	internal static CommandCollection InitializeGroupVirtualMachineOwnershipCommands(Cluster cluster, IEnumerable<Group> groups)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleGroupStateVirtualMachine)
		{
			Name = "Multi Virtual Machine Ownership Commands"
		};
		ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(null, "MoveVirtualMachines", ClusterCommandId.GroupMoveTo, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_Move
		};
		clusterCommandContainer.ChildrenInternal.AddAll(GetMultiselectMoveCommands(groups, commandCollection.Category));
		foreach (ClusterCommandContainer item3 in clusterCommandContainer.ChildrenInternal)
		{
			item3.ChildrenInternal.Add(Group.CreateMultipleMoveToBestNodeCommand(item3, cluster, groups));
			item3.ChildrenInternal.Add(Group.CreateMultipleMoveToSelectedNodeCommand(item3, cluster, groups));
		}
		ClusterCommand item = new ClusterCommand(null, "MultiMoveStorage", ClusterCommandId.VirtualMachineMultipleGroupMoveStorage, ClusterCommandCollectionId.GroupStorageOwnership)
		{
			Text = CommandResources.VirtualMachineGroup_MoveStorageCommand_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate(object x)
			{
				if (!(x is List<VirtualMachineStorageMoveParameters> list))
				{
					return;
				}
				foreach (VirtualMachineStorageMoveParameters item4 in list)
				{
					item4.Resource.MoveStorage(item4);
				}
			},
			InputParameters = new InputParameterList<VirtualMachineGroup>(groups.Cast<VirtualMachineGroup>()),
			CommandParameter = new MoveVirtualMachineStorageMapping(cluster)
		};
		clusterCommandContainer.ChildrenInternal.Add(item);
		commandCollection.Add(clusterCommandContainer);
		ClusterCommand item2 = new ClusterCommand(null, "CancelLiveMigration", ClusterCommandId.VirtualMachineGroupCancelLiveMigration, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.VirtualMachineGroup_CancelLiveMigration,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				groups.ForEach(delegate(Group group)
				{
					if (VirtualMachineCancelLiveMigrationCommand.CanExecute(group))
					{
						group.CancelLiveMigration();
					}
				});
			}
		};
		commandCollection.Add(item2);
		commandCollection.AddAll(Group.InitializePriorityCommands(cluster, groups));
		ClusterCommandContainer clusterCommandContainer3 = new ClusterCommandContainer(null, "VirtualMachineMoreActions", ClusterCommandId.GroupMoreActions)
		{
			Text = CommandResources.MoreActions,
			ExecuteIfNoChildren = false
		};
		commandCollection.Add(clusterCommandContainer3);
		clusterCommandContainer3.ChildrenInternal.AddAll(InitializeMultiSelectMoreActions(groups, commandCollection));
		return commandCollection;
	}

	private static IEnumerable<ClusterCommand> GetMultiselectMoveCommands(IEnumerable<Group> groups, ClusterCommandCollectionId clusterCommandCollectionId)
	{
		yield return new ClusterCommandContainer(null, "LiveMigration", ClusterCommandId.VirtualMachineMultipleGroupLiveMigrate, clusterCommandCollectionId)
		{
			Text = CommandResources.Group_VM_Live_Migration,
			CommandParameter = groups,
			CanExecuteDelegate = (object node) => true,
			ExecuteDelegate = delegate(object node)
			{
				if (node != null && !(node is Node))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotNodeMoveCommand);
				}
				Node node3 = node as Node;
				Migrate(groups.Cast<VirtualMachineGroup>(), VirtualMachineMigrationType.Live, node3);
			}
		};
		yield return new ClusterCommandContainer(null, "QuickMigrate", ClusterCommandId.VirtualMachineMultipleGroupQuickMigrate, clusterCommandCollectionId)
		{
			Text = CommandResources.Group_VM_Quick_Migrate,
			CommandParameter = groups,
			CanExecuteDelegate = (object node) => true,
			ExecuteDelegate = delegate(object node)
			{
				if (node != null && !(node is Node))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotNodeMoveCommand);
				}
				Node node2 = node as Node;
				Migrate(groups.Cast<VirtualMachineGroup>(), VirtualMachineMigrationType.Quick, node2);
			}
		};
	}

	public static void Migrate(IEnumerable<VirtualMachineGroup> virtualMachines, VirtualMachineMigrationType migrationType, Node node)
	{
		if (migrationType == VirtualMachineMigrationType.Live)
		{
			virtualMachines = virtualMachines.Where((VirtualMachineGroup vmGroup) => vmGroup.ApplicationStatus == ApplicationStatus.Running || vmGroup.ApplicationStatus == ApplicationStatus.PartiallyRunning);
		}
		EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
		{
			virtualMachine.Migrate(node, migrationType, operationResult);
		});
	}

	public static void VirtualMachineSave(IEnumerable<VirtualMachineGroup> virtualMachines)
	{
		EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
		{
			virtualMachine.Save(operationResult);
		});
	}

	public static void VirtualMachineShutdown(IEnumerable<VirtualMachineGroup> virtualMachines, bool askConfirmation = false)
	{
		if (!askConfirmation || new ConfirmationDialog
		{
			CustomIcon = InvariantResources.VirtualMachine,
			Caption = DialogResources.VirtualMachineShutdownMany_Title,
			Header = DialogResources.VirtualMachineShutdownMany_Header,
			Content = DialogResources.VirtualMachineShutdownMany_Content.FormatCurrentCulture(virtualMachines.Count())
		}.ShowDialog() == TaskDialogResult.Yes)
		{
			EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
			{
				virtualMachine.Shutdown(operationResult);
			});
		}
	}

	public static void VirtualMachineTurnoff(IEnumerable<VirtualMachineGroup> virtualMachines, bool askConfirmation = false)
	{
		if (!askConfirmation || new ConfirmationDialog
		{
			CustomIcon = InvariantResources.VirtualMachine,
			Caption = DialogResources.VirtualMachineTurnoffMany_Title,
			Header = DialogResources.VirtualMachineTurnoffMany_Header,
			Content = DialogResources.VirtualMachineTurnoffMany_Content
		}.ShowDialog() == TaskDialogResult.Yes)
		{
			EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
			{
				virtualMachine.Turnoff(operationResult);
			});
		}
	}

	public static void VirtualMachineStart(IEnumerable<VirtualMachineGroup> virtualMachines)
	{
		EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
		{
			virtualMachine.Start(operationResult);
		});
	}

	public static void VirtualMachineConnect(IEnumerable<VirtualMachineGroup> virtualMachines)
	{
		virtualMachines.AsParallel().ForAll(delegate(VirtualMachineGroup virtualMachine)
		{
			virtualMachine.Connect();
		});
	}

	public static void VirtualMachineDeleteSavedState(IEnumerable<VirtualMachineGroup> virtualMachines, bool askConfirmation = false)
	{
		if (!askConfirmation || new ConfirmationDialog
		{
			CustomIcon = InvariantResources.VirtualMachine,
			Caption = DialogResources.VirtualMachineDeleteSavedStateMany_Title,
			Header = DialogResources.VirtualMachineDeleteSavedStateMany_Header,
			Content = DialogResources.VirtualMachineDeleteSavedStateMany_Content.FormatCurrentCulture(virtualMachines.Count())
		}.ShowDialog() == TaskDialogResult.Yes)
		{
			EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
			{
				virtualMachine.DeleteSavedState(operationResult);
			});
		}
	}

	public static void VirtualMachineReset(IEnumerable<VirtualMachineGroup> virtualMachines, bool askConfirmation = false)
	{
		if (!askConfirmation || new ConfirmationDialog
		{
			CustomIcon = InvariantResources.VirtualMachine,
			Caption = DialogResources.VirtualMachineResetMany_Title,
			Header = DialogResources.VirtualMachineResetMany_Header,
			Content = DialogResources.VirtualMachineResetMany_Content.FormatCurrentCulture(virtualMachines.Count())
		}.ShowDialog() == TaskDialogResult.Yes)
		{
			EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
			{
				virtualMachine.Reset(operationResult);
			});
		}
	}

	public static void VirtualMachinePause(IEnumerable<VirtualMachineGroup> virtualMachines)
	{
		EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
		{
			virtualMachine.Pause(operationResult);
		});
	}

	public static void VirtualMachineResume(IEnumerable<VirtualMachineGroup> virtualMachines)
	{
		EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
		{
			virtualMachine.Resume(operationResult);
		});
	}

	public static void VirtualMachineTakeCheckpoint(IEnumerable<VirtualMachineGroup> virtualMachines)
	{
		EnqueueAndThrottleRequests(virtualMachines, delegate(VirtualMachineGroup virtualMachine, Action<OperationResult> operationResult)
		{
			virtualMachine.TakeCheckpoint(operationResult);
		});
	}

	private bool CalculateConfigureMonitoringVisibility()
	{
		return VirtualMachines.Count <= 1;
	}

	public VirtualMachineGroup()
	{
		Init();
	}

	public VirtualMachineGroup(string name)
		: base(name)
	{
		Init();
	}

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.VirtualMachineGroup);
	}

	public void Migrate(Node node, VirtualMachineMigrationType migrationType, bool enableOverride = false)
	{
		Migrate(node, migrationType, base.SetLastErrorIfNecessary, enableOverride);
	}

	public void Migrate(Node node, VirtualMachineMigrationType migrationType, Action<OperationResult> operationResult, bool enableOverride = false)
	{
		MigrateInternal(node, migrationType, delegate(OperationResult result)
		{
			ConfirmOverrideAndExecuteOnLocked(result, delegate
			{
				MigrateInternal(node, migrationType, base.SetLastErrorIfNecessary, overrideLockState: true);
			}, operationResult, enableOverride, overrideMaintenanceMode: false);
		});
	}

	private void MigrateInternal(Node node, VirtualMachineMigrationType migrationType, Action<OperationResult> operationResult, bool overrideLockState = false)
	{
		if (migrationType == VirtualMachineMigrationType.Live && ApplicationStatus != 0 && ApplicationStatus != ApplicationStatus.PartiallyRunning && ApplicationStatus != ApplicationStatus.Paused)
		{
			ClusterVirtualMachineMigrateException ex = new ClusterVirtualMachineMigrateException(ExceptionResources.VirtualMachineGroup_LiveMigration_VirtualMachineWrongStatus, base.Name, migrationType);
			operationResult.SafeCall(new OperationResult(this, ex));
			return;
		}
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			PVirtualMachineGroup privateVirtualMachineGroup = (PVirtualMachineGroup)lockObject.Owner;
			if (node != null)
			{
				node.ExecuteMethod(delegate(ILockable privateNode)
				{
					privateVirtualMachineGroup.Migrate((PNode)privateNode.Owner, migrationType, overrideLockState);
				}, OperationType.Sync, null, LockAccess.Reader, setErrorOnObject: false);
			}
			else
			{
				privateVirtualMachineGroup.Migrate(null, migrationType, overrideLockState);
			}
		}, operationResult, LockAccess.Reader);
	}

	public void Turnoff(bool askConfirmation = false)
	{
		Turnoff(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void Turnoff(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Turnoff(delegate(OperationResult turnoffOpResult)
			{
				ProcessOperationResult(turnoffOpResult, operationResult);
			}, askConfirmation);
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if (((Resource)resourceOpLoaded.Sender).ResourceState == ResourceState.Offline)
				{
					SetProcessingFlag(processing: false);
				}
			}, ResourceLoadSelection.Basic);
		});
	}

	public void Start()
	{
		Start(base.SetLastErrorIfNecessary);
	}

	public void Start(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Start(delegate(OperationResult startOpResult)
			{
				ProcessOperationResult(startOpResult, operationResult);
			});
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if (resourceOpLoaded.Error != null)
				{
					Error = resourceOpLoaded.Error;
				}
				else if (((Resource)resourceOpLoaded.Sender).ResourceState == ResourceState.Online)
				{
					SetProcessingFlag(processing: false);
				}
			}, ResourceLoadSelection.Basic);
		});
	}

	public void Connect()
	{
		Connect(base.SetLastErrorIfNecessary);
	}

	public void Connect(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Connect(operationResult);
		});
	}

	public void Manage()
	{
		Manage(base.SetLastErrorIfNecessary);
	}

	public void Manage(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Manage(operationResult);
		});
	}

	public void Settings()
	{
		Settings(base.SetLastErrorIfNecessary);
	}

	public void Settings(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Settings(operationResult);
		});
	}

	public void Save()
	{
		Save(base.SetLastErrorIfNecessary);
	}

	public void Save(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Save(delegate(OperationResult saveOpResult)
			{
				ProcessOperationResult(saveOpResult, operationResult);
			});
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if (((Resource)resourceOpLoaded.Sender).ResourceState == ResourceState.Offline)
				{
					SetProcessingFlag(processing: false);
				}
			}, ResourceLoadSelection.Basic);
		});
	}

	public void DeleteSavedState(bool askConfirmation = false)
	{
		DeleteSavedState(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void DeleteSavedState(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.DeleteSavedState(delegate(OperationResult deleteSavedStateOpResult)
			{
				ProcessOperationResult(deleteSavedStateOpResult, operationResult);
			}, askConfirmation);
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if ((VirtualMachineState)((VirtualMachineResource)resourceOpLoaded.Sender).ApplicationStatus != VirtualMachineState.Saved)
				{
					SetProcessingFlag(processing: false);
				}
			}, ResourceLoadSelection.Basic);
		});
	}

	public void Pause()
	{
		Pause(base.SetLastErrorIfNecessary);
	}

	public void Pause(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Pause(delegate(OperationResult pauseOpResult)
			{
				ProcessOperationResult(pauseOpResult, operationResult);
			});
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if ((VirtualMachineState)((VirtualMachineResource)resourceOpLoaded.Sender).ApplicationStatus == VirtualMachineState.Paused)
				{
					SetProcessingFlag(processing: false);
				}
			}, ResourceLoadSelection.Basic);
		});
	}

	public void Resume()
	{
		Resume(base.SetLastErrorIfNecessary);
	}

	public void Resume(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Resume(delegate(OperationResult resumeOpResult)
			{
				ProcessOperationResult(resumeOpResult, operationResult);
			});
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if ((VirtualMachineState)((VirtualMachineResource)resourceOpLoaded.Sender).ApplicationStatus == VirtualMachineState.Running)
				{
					SetProcessingFlag(processing: false);
				}
			}, ResourceLoadSelection.Basic);
		});
	}

	public void Reset(bool askConfirmation = false)
	{
		Reset(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void Reset(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Reset(delegate(OperationResult resetOpResult)
			{
				ProcessOperationResult(resetOpResult, operationResult);
			}, askConfirmation);
			SetProcessingFlag(processing: false);
		});
	}

	public void Shutdown(bool askConfirmation = false)
	{
		Shutdown(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void Shutdown(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.Shutdown(delegate(OperationResult shutdownOpResult)
			{
				ProcessOperationResult(shutdownOpResult, operationResult);
			}, askConfirmation);
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if (((Resource)resourceOpLoaded.Sender).ResourceState == ResourceState.Offline)
				{
					SetProcessingFlag(processing: false);
				}
			}, ResourceLoadSelection.Basic);
		});
	}

	public void TakeCheckpoint(Action<OperationResult> operationResult)
	{
		ExecuteOnVmResource(delegate(VirtualMachineResource resourceParam)
		{
			resourceParam.LoadAsync(delegate(ClusterLoadedEventArgs resourceOpLoaded)
			{
				if (resourceOpLoaded.Error != null)
				{
					operationResult(new OperationResult(this, resourceOpLoaded.Error));
				}
				resourceParam.TakeCheckpoint(delegate(OperationResult takeCheckpointOpResult)
				{
					ProcessOperationResult(takeCheckpointOpResult, operationResult);
				});
				SetProcessingFlag(processing: false);
			}, ResourceLoadSelection.PrivateProperties);
		});
	}

	internal VirtualMachineGroup(Cluster cluster)
		: base(cluster)
	{
		Init();
	}

	private void Init()
	{
		virtualMachineMoveCommand = new VirtualMachineMoveCommand(this);
		virtualMachineDeleteSavedStateCommand = new VirtualMachineDeleteSavedStateCommand(this);
		virtualMachinePauseCommand = new VirtualMachinePauseCommand(this);
		virtualMachineResumeCommand = new VirtualMachineResumeCommand(this);
		virtualMachineResetCommand = new VirtualMachineResetCommand(this);
		virtualMachineCancelLiveMigrationCommand = new VirtualMachineCancelLiveMigrationCommand(this);
	}

	internal void ExecuteOnVmResource(Action<VirtualMachineResource> virtualMachineResourceAction)
	{
		LoadVmResource(delegate(OperationResult<VirtualMachineResource> loadResourceOperation)
		{
			if (loadResourceOperation.Error != null && !(loadResourceOperation.Error is ClusterVirtualMachineMultipleVMFoundException))
			{
				Error = loadResourceOperation.Error;
			}
			else if (loadResourceOperation.Result != null)
			{
				virtualMachineResourceAction(loadResourceOperation.Result);
			}
			else
			{
				SetProcessingFlag(processing: false);
			}
		});
	}

	internal void LoadVmResource(Action<OperationResult<VirtualMachineResource>> operationResult)
	{
		if (vmResourceLoaded == false)
		{
			vmResourceLoaded = null;
			if (memberAllVirtualMachinesState.Count == 1)
			{
				Guid resourceId = memberAllVirtualMachinesState.Keys.FirstOrDefault();
				Resource.Get(base.Cluster, resourceId, delegate(OperationResult<Resource> virtualMachineResoureOperationGet)
				{
					vmResourceLoaded = true;
					if (virtualMachineResoureOperationGet.Error == null)
					{
						vmResource = (VirtualMachineResource)virtualMachineResoureOperationGet.Result;
						VirtualMachines.ExecuteQuery(ResultExecution.OnDispatcher, VirtualMachinesQuery, operationResult);
					}
					else
					{
						operationResult.SafeCall(new OperationResult<VirtualMachineResource>(this, vmResource, virtualMachineResoureOperationGet.Error));
					}
				}, OperationType.Async);
			}
			else
			{
				VirtualMachines.ExecuteQuery(ResultExecution.OnDispatcher, VirtualMachinesQuery, operationResult);
			}
		}
		else
		{
			if (operationResult == null)
			{
				return;
			}
			lock (callbackLoadQueue)
			{
				if (vmResource == null && !vmResourceLoaded.HasValue)
				{
					callbackLoadQueue.Enqueue(operationResult);
					return;
				}
			}
			operationResult.SafeCall(new OperationResult<VirtualMachineResource>(this, vmResource, lastVmError, new object()));
		}
	}

	private void ProcessOperationResult(OperationResult operationResult, Action<OperationResult> operationResultCallback)
	{
		try
		{
			if (operationResult.Error != null)
			{
				SetProcessingFlag(processing: false);
			}
		}
		finally
		{
			operationResultCallback(operationResult);
		}
	}

	private void VirtualMachinesQuery(OperationResult<IClusterList<Resource>> result)
	{
		Action<OperationResult<VirtualMachineResource>> operationResultCallback = (Action<OperationResult<VirtualMachineResource>>)result.Parameter;
		ClusterException ex = result.Error;
		lock (callbackLoadQueue)
		{
			vmResourceLoaded = true;
			if (ex == null)
			{
				switch (VirtualMachines.Count)
				{
				case 0:
					ex = new ClusterVirtualMachineNotFoundInGroupException(base.Name);
					Worker.Start(delegate
					{
						SetProcessingFlag(processing: false);
					});
					break;
				case 1:
					vmResource = (VirtualMachineResource)VirtualMachines[0];
					break;
				default:
					ex = new ClusterVirtualMachineMultipleVMFoundException(VirtualMachines.Count);
					break;
				}
				lastVmError = ex;
			}
		}
		if (ex != null)
		{
			ClusterLog.LogException(ex, "There was an error getting VM resource from VM group.");
		}
		operationResultCallback.SafeCall(new OperationResult<VirtualMachineResource>(this, vmResource, ex));
		lock (callbackLoadQueue)
		{
			while (callbackLoadQueue.Count > 0)
			{
				callbackLoadQueue.Dequeue().SafeCall(new OperationResult<VirtualMachineResource>(this, vmResource, ex));
			}
		}
		NotifyApplicationStatusChanged();
	}

	protected static void EnqueueAndThrottleRequests(IEnumerable<VirtualMachineGroup> virtualMachines, Action<VirtualMachineGroup, Action<OperationResult>> operationToExecute)
	{
		virtualMachines = virtualMachines.OrderByDescending((VirtualMachineGroup virtualMachine) => (int)virtualMachine.Priority);
		ClusterObject.EnqueueAndThrottleRequests(virtualMachines.Cast<ClusterObject>(), delegate(ClusterObject clusterObject, Action<OperationResult> operationResult)
		{
			operationToExecute((VirtualMachineGroup)clusterObject, operationResult);
		});
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		PVirtualMachineGroup pVirtualMachineGroup = (PVirtualMachineGroup)privateObject;
		base.TransferInternalData((PClusterObject)pVirtualMachineGroup, subscribeToEvents, ignorePossibleOwners);
		foreach (KeyValuePair<Guid, VirtualMachineState> virtualMachineChildResource in pVirtualMachineGroup.GetVirtualMachineChildResources())
		{
			lock (memberAllVirtualMachinesState)
			{
				memberVirtualMachineState = virtualMachineChildResource.Value;
				if (memberAllVirtualMachinesState.ContainsKey(virtualMachineChildResource.Key))
				{
					memberAllVirtualMachinesState[virtualMachineChildResource.Key] = memberVirtualMachineState.Value;
				}
				else
				{
					memberAllVirtualMachinesState.Add(virtualMachineChildResource.Key, memberVirtualMachineState.Value);
				}
			}
			NotifyApplicationStatusChanged();
		}
		replicationHealth = pVirtualMachineGroup.ReplicationHealth;
		foreach (ClusterForwardedEventArgs value in pVirtualMachineGroup.ForwardedArgs.Values)
		{
			ProcessForwardedArgs(value);
		}
	}

	protected override void OnResourceAdded(ClusterGainedEventArgs args)
	{
		try
		{
			lock (callbackLoadQueue)
			{
				vmResourceLoaded = false;
				lastVmError = null;
			}
			lock (memberAllVirtualMachinesState)
			{
				if (memberAllVirtualMachinesState.ContainsKey(args.GainedId))
				{
					return;
				}
			}
			base.Cluster.GetResource(args.GainedId, delegate(OperationResult<Resource> resourceOpGet)
			{
				if (resourceOpGet.Error != null)
				{
					ClusterLog.LogException(resourceOpGet.Error, "There was an error to get the resource from the cluster");
				}
				else
				{
					Resource result = resourceOpGet.Result;
					if (result.ResourceType.ResourceKind == ResourceKind.VirtualMachine)
					{
						if (result.Properties["VmState"] != null)
						{
							UpdateVirtualMachineState(result.Id, result.Properties);
						}
						else
						{
							result.LoadAsync(5);
						}
					}
				}
			}, OperationType.Async);
		}
		finally
		{
			base.OnResourceAdded(args);
		}
	}

	protected override void OnResourceRemoved(ClusterLostEventArgs args)
	{
		try
		{
			lock (callbackLoadQueue)
			{
				vmResourceLoaded = false;
				lastVmError = null;
			}
			lock (memberAllVirtualMachinesState)
			{
				if (memberAllVirtualMachinesState.ContainsKey(args.LostId))
				{
					memberAllVirtualMachinesState.Remove(args.LostId);
					if (memberAllVirtualMachinesState.Count == 0)
					{
						memberVirtualMachineState = null;
					}
					else if (memberAllVirtualMachinesState.Count == 1)
					{
						memberVirtualMachineState = memberAllVirtualMachinesState.Values.ElementAt(0);
					}
				}
			}
			NotifyApplicationStatusChanged();
		}
		finally
		{
			base.OnResourceRemoved(args);
		}
	}

	private void NotifyApplicationStatusChanged(Queue<Action> queueOnDispatcher = null)
	{
		UIHelper.ExecuteOnDispatcher((Action)delegate
		{
			OnPropertyChanged("ApplicationStatus");
			OnPropertyChanged("GroupState");
			OnApplicationStatusChanged(new ClusterApplicationStatusEventArgs(Id, Error));
		}, OperationType.Async, queueOnDispatcher);
	}

	private void UpdateVirtualMachineState(Guid resourceId, ClusterPropertyCollection properties)
	{
		ClusterProperty clusterProperty = properties["VmState"];
		if (clusterProperty == null)
		{
			return;
		}
		VirtualMachineState virtualMachineState = (VirtualMachineState)(uint)clusterProperty.Value;
		memberVirtualMachineState = virtualMachineState;
		lock (memberAllVirtualMachinesState)
		{
			if (memberAllVirtualMachinesState.TryGetValue(resourceId, out var value))
			{
				if (value != virtualMachineState)
				{
					memberAllVirtualMachinesState[resourceId] = virtualMachineState;
					NotifyApplicationStatusChanged();
				}
			}
			else
			{
				memberAllVirtualMachinesState.Add(resourceId, virtualMachineState);
				NotifyApplicationStatusChanged();
			}
		}
	}

	private void UpdateMigrationState(Guid resourceId, ClusterPropertyCollection properties)
	{
		Utilities.UnreferencedParameter(resourceId);
		ClusterPropertyULong clusterPropertyULong = (ClusterPropertyULong)properties["ResourceSpecificData1"];
		if (clusterPropertyULong == null)
		{
			return;
		}
		VirtualMachineComputerSystemOperationalStatus migrationStatus = WmiVMUtilities.GetMigrationStatus(clusterPropertyULong);
		int num = WmiVMUtilities.GetMigrationProgress(clusterPropertyULong);
		if (migrationState != migrationStatus)
		{
			migrationState = migrationStatus;
			UIHelper.ExecuteOnDispatcher(delegate
			{
				OnPropertyChanged("MigrationState");
				OnPropertyChanged("ApplicationStatus");
			}, OperationType.Async);
		}
		if (migrationProgress != num)
		{
			migrationProgress = num;
			UIHelper.ExecuteOnDispatcher(delegate
			{
				OnPropertyChanged("MigrationProgress");
			}, OperationType.Async);
		}
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.VirtualMachineReplicationHealthChanged:
		{
			ClusterResourceVirtualMachineReplicationHealthEventArgs clusterResourceVirtualMachineReplicationHealthEventArgs = e.EventArgument as ClusterResourceVirtualMachineReplicationHealthEventArgs;
			replicationHealth = clusterResourceVirtualMachineReplicationHealthEventArgs.ReplicationHealth;
			OnPropertyChanged("ReplicationHealth");
			break;
		}
		case EventType.ForwardPayload:
		{
			ClusterForwardedEventArgs clusterForwardedEventArgs = e.EventArgument as ClusterForwardedEventArgs;
			if (clusterForwardedEventArgs.Key == 1)
			{
				RefreshStorageResources();
			}
			else if (clusterForwardedEventArgs.Key == 2 || clusterForwardedEventArgs.Key == 3)
			{
				ProcessForwardedArgs(clusterForwardedEventArgs);
			}
			break;
		}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private void ProcessForwardedArgs(ClusterForwardedEventArgs args)
	{
		ClusterPropertiesEventArgs forwardedProperties = args.ForwardedPayload as ClusterPropertiesEventArgs;
		if (forwardedProperties == null)
		{
			return;
		}
		int num;
		if (args.ClusterElementType is PResourceType)
		{
			num = ((((PResourceType)args.ClusterElementType).ResourceKind == ResourceKind.VirtualMachine) ? 1 : 0);
			if (num != 0)
			{
				UpdateMigrationState(args.ForwardedPayload.Id, forwardedProperties.Properties);
			}
		}
		else
		{
			num = 0;
		}
		forwardedProperties.Properties.Get("LastOperationStatusCode", delegate(ClusterPropertyULong lastOperationStatusCodeProperty)
		{
			if (lastOperationStatusCodeProperty.TypedValue != lastOperationStatusCode || lastVmOrVmConfigResourceErrorName != null)
			{
				lastOperationStatusCode = lastOperationStatusCodeProperty.TypedValue;
				forwardedProperties.Properties.GetOrNull("StatusInformation", delegate(ClusterPropertyULong statusInformation)
				{
					ResourceSubStatus? resourceSubStatus = ((statusInformation != null) ? new ResourceSubStatus?((ResourceSubStatus)statusInformation.TypedValue) : null);
					if (ExceptionHelpers.Failed(lastOperationStatusCode.Value))
					{
						if (resourceSubStatus.HasValue && !resourceSubStatus.Value.HasFlag(ResourceSubStatus.Locked))
						{
							SetErrorProperty(args.ClusterElementType, forwardedProperties, (string n, Resource r) => new ClusterVirtualMachineErrorToLogException(n, r, null));
						}
						else if (lastVmOrVmConfigResourceErrorName != null && lastVmOrVmConfigResourceErrorName == forwardedProperties.Name)
						{
							Error = ClusterLastErrorException.Create(lastOperationStatusCode, forwardedProperties.Name);
							lastVmOrVmConfigResourceErrorName = null;
						}
					}
					else if (ExceptionHelpers.Information(lastOperationStatusCode.Value))
					{
						if (ExceptionHelpers.GetHResultFromClusterError(lastOperationStatusCode.Value) != NativeMethods.VM_E_TASK_CANCELED)
						{
							SetErrorProperty(args.ClusterElementType, forwardedProperties, (string n, Resource r) => new ClusterVirtualMachineWarningToLogException(n, r, null));
						}
						else
						{
							Information = ExceptionResources.VirtualMachineGroup_LiveMigrationCanceled_Text;
						}
					}
					else
					{
						Error = null;
						Information = null;
						lastVmOrVmConfigResourceErrorName = null;
					}
				});
			}
		});
		if (num != 0)
		{
			UpdateVirtualMachineState(args.ForwardedPayload.Id, forwardedProperties.Properties);
		}
	}

	private void SetErrorProperty<T>(object elementType, ClusterPropertiesEventArgs prop, Func<string, Resource, T> create) where T : ClusterException
	{
		Utilities.UnreferencedParameter(elementType);
		Resource.Get(base.Cluster, prop.Id, delegate(OperationResult<Resource> getOperationResource)
		{
			if (getOperationResource.Error == null)
			{
				Resource result = getOperationResource.Result;
				Error = create(prop.Name, result);
				lastVmOrVmConfigResourceErrorName = prop.Name;
			}
		}, OperationType.Async);
	}

	internal void MoveStorage(object storage)
	{
		Exceptions.ThrowIfNull(storage, "storage");
		foreach (VirtualMachineStorageMoveParameters item in (storage as IEnumerable<VirtualMachineStorageMoveParameters>) ?? throw new ArgumentException("storage must be an IEnumerable<VirtualMachineStorageMoveParameters>"))
		{
			item.Resource.MoveStorage(item);
		}
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		vmResource = null;
		vmResourceLoaded = false;
		memberVirtualMachineState = null;
		lastVmError = null;
		migrationState = null;
		migrationProgress = null;
		virtualMachinesWeak = null;
		memberAllVirtualMachinesState.Clear();
		callbackLoadQueue.Clear();
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("VirtualMachines");
			OnPropertyChanged("MigrationState");
			OnPropertyChanged("MigrationProgress");
		}, OperationType.Async);
	}
}

