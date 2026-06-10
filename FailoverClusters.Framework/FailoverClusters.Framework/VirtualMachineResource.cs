using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FailoverClusters.Configuration;
using FailoverClusters.PowerShell;
using FailoverClusters.UI.Common;
using Virtualization.Client.Common;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;
using KDDSL.ServerClusters;

namespace FailoverClusters.Framework;

public class VirtualMachineResource : Resource
{
	private WeakReferenceEx connectCommandWeak;

	private WeakReferenceEx manageCommandWeak;

	private WeakReferenceEx settingsCommandWeak;

	private WeakReferenceEx configureMonitoringCommandWeak;

	private WeakReferenceEx startCommandWeak;

	private WeakReferenceEx turnoffCommandWeak;

	private WeakReferenceEx shutdownCommandWeak;

	private WeakReferenceEx saveCommandWeak;

	private WeakReferenceEx deleteSavedStateCommandWeak;

	private WeakReferenceEx pauseCommandWeak;

	private WeakReferenceEx resumeCommandWeak;

	private WeakReferenceEx resetCommandWeak;

	private WeakReferenceEx applicationCommandsWeak;

	private WeakReferenceEx replicationCommandsContainerWeak;

	private WeakReferenceEx enableReplicationCommandWeak;

	private WeakReferenceEx initializeReplicationCommandWeak;

	private WeakReferenceEx importReplicationCommandWeak;

	private WeakReferenceEx reverseReplicationCommandWeak;

	private WeakReferenceEx removeReplicationCommandWeak;

	private WeakReferenceEx pauseReplicationCommandWeak;

	private WeakReferenceEx cancelResynchronizeCommandWeak;

	private WeakReferenceEx resumeReplicationCommandWeak;

	private WeakReferenceEx startRecoveryCommandWeak;

	private WeakReferenceEx cancelFailoverReplicationCommandWeak;

	private WeakReferenceEx testRecoveryCommandWeak;

	private WeakReferenceEx cancelTestFailoverReplicationCommandWeak;

	private WeakReferenceEx prepareFailoverReplicationCommandWeak;

	private WeakReferenceEx cancelPrepareFailoverReplicationCommandWeak;

	private WeakReferenceEx viewReplicationHealthCommandWeak;

	private WeakReferenceEx cancelInitializeReplicationCommandWeak;

	private WeakReferenceEx cancelDiskUpdateReplicationCommandWeak;

	private WeakReferenceEx commitFailoverCommandWeak;

	private WeakReferenceEx extendReplicationCommandWeak;

	private WeakReferenceEx pauseExtendedReplicationCommandWeak;

	private WeakReferenceEx resumeExtendedReplicationCommandWeak;

	private WeakReferenceEx revertCheckpointCommandWeak;

	private WeakReferenceEx checkpointCommandWeak;

	private WeakReferenceEx exportCommandWeak;

	private WeakReferenceEx<ClusterCommandContainer> groupCommandContainer;

	public const string VirtualMachineStatePropertyName = "VmState";

	public const string VirtualMachineReplicationStatePropertyName = "ResourceSpecificData2";

	private static readonly object virtualMachineToolsLock;

	private bool areCheckpointsLoaded;

	private const string RevertCheckpointConfirmationPrompt = "RevertCheckpointConfirmationPrompt";

	private const string VirtMgmtSnapin = "VirtMgmt.msc";

	private const int CheckpointsRefreshPeriod = 5000;

	private const int PropertiesRefreshPeriod = 5000;

	private static Queue<VirtualMachineResource> virtualMachineQueueReload;

	private readonly object vmSettingsLoadingLock = new object();

	private readonly object exportLoadingLock = new object();

	private VirtualMachineState? virtualMachineState;

	private WeakReferenceEx<ImageSource> desktopThumbnailWeak;

	private Bitmap memberTemporaryThumbnail;

	private ulong? guestAssignedMemory;

	private ulong? guestAvailableMemory;

	private ulong? guestMemoryDemand;

	private ushort? guestCpuUsage;

	private VirtualMachineHeartbeatStatus? heartbeatStatus;

	private string vmVersion;

	private TimeSpan? guestUpTime;

	private DateTime? guestCreationTime;

	private string guestOperatingSystem;

	private string guestComputerName;

	private string integrationServicesVersion;

	private int? guestOsMajorVersion;

	private int? guestOsMinorVersion;

	private int? guestOsBuildNumber;

	private OSProductType? guestOsProductType;

	private VirtualMachineStorageInformation storageInformation;

	private VirtualMachineCheckpointInformation checkpointInformation;

	private string guestNotes;

	private string guestStatus;

	private VirtualMachineComputerSystemOperationalStatus? migrationState;

	private int? migrationProgress;

	private VirtualMachineIntegrationServicesStatus? integrationServicesStatus;

	private Process vmConnectCurrentProcess;

	private Icon2 warningIcon;

	private WeakReferenceEx<ObservableCollection<VMServiceInfo>> vmServices;

	private WeakReferenceEx<ObservableCollection<VMServiceInfo>> vmMonitoredServices;

	private VirtualMachineReplicationData primaryRelationship;

	private VirtualMachineReplicationData extendedRelationship;

	private VirtualMachineReplicationVersion version = VirtualMachineReplicationVersion.WindowsBlue;

	private ISettingsDialog settingDialog;

	private ISettingsDialog exportDialog;

	private IWizard createReplicationRelationshipWizard;

	private IForm viewReplicationHealthForm;

	private IForm plannedFailoverForm;

	private IForm startInitalReplicationForm;

	private IForm completeInitialReplicationForm;

	private IForm startReplicationFailoverForm;

	private IForm testReplicationFailoverForm;

	private IForm resynchronizeReplicationForm;

	private IForm removeReplicationForm;

	private IForm resumeExtendedReplicationForm;

	private static bool? isVirtualMachineToolsFeatureInstalled;

	private bool isVmSettingsLoading;

	private bool isExportLoading;

	private int isReplicationFormOpeningOrClosing;

	private VirtualMachineCheckpointInformation temporaryCheckpointInformation;

	private WeakReferenceEx<VirtualMachineCheckpointInformation> checkpointInformationWeak;

	private static readonly VirtualMachineCheckpointInformation EmptyCheckpointInformation;

	public override CommandCollection ApplicationCommands => WeakReferenceEx.ReturnInstance(ref applicationCommandsWeak, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.VirtualMachineResource);
		InitializeApplicationCommands(commandCollection);
		return commandCollection;
	});

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.VirtualMachineResource));

	public static bool IsVirtualMachineToolsInstalled
	{
		get
		{
			if (!isVirtualMachineToolsFeatureInstalled.HasValue)
			{
				lock (virtualMachineToolsLock)
				{
					if (!isVirtualMachineToolsFeatureInstalled.HasValue)
					{
						isVirtualMachineToolsFeatureInstalled = new WindowsFeature().IsVirtualMachineClientToolsInstalled();
					}
				}
			}
			return isVirtualMachineToolsFeatureInstalled.Value;
		}
	}

	public override object ApplicationStatus => LoadAsync(virtualMachineState, 4).ToVirtualMachineRealState(base.ResourceState);

	public ulong? GuestAssignedMemory => LoadAsync(guestAssignedMemory, 8193);

	public ulong? GuestAvailableMemory => LoadAsync(guestAvailableMemory, 8193);

	public ulong? GuestMemoryDemand => LoadAsync(guestMemoryDemand, 8193);

	public ushort? GuestCpuUsage => LoadAsync(guestCpuUsage, 8193);

	public VirtualMachineHeartbeatStatus HeartbeatStatus => LoadAsync<VirtualMachineHeartbeatStatus, VirtualMachineHeartbeatStatus>(heartbeatStatus, 8193);

	public string VmVersion => LoadAsync(vmVersion, 8193);

	public TimeSpan? GuestUptime => LoadAsync(guestUpTime, 8193);

	public DateTime? GuestCreationTime => LoadAsync(guestCreationTime, 8193);

	public string GuestNotes => LoadAsync(guestNotes, 8193);

	public string GuestStatus => LoadAsync(guestStatus, 8193);

	public string GuestOperatingSystem => LoadAsync(guestOperatingSystem, 16385);

	public string GuestComputerName => LoadAsync(guestComputerName, 16385);

	public string IntegrationServicesInformation => LoadAsync(integrationServicesVersion, 16385);

	public int? GuestOsMajorVersion => LoadAsync(guestOsMajorVersion, 16385);

	public int? GuestOsMinorVersion => LoadAsync(guestOsMinorVersion, 16385);

	public int? GuestOsBuildNumber => LoadAsync(guestOsBuildNumber, 16385);

	public OSProductType GuestOsProductType => LoadAsync<OSProductType, OSProductType>(guestOsProductType, 16385);

	public VirtualMachineIntegrationServicesStatus IntegrationServicesStatus => LoadAsync<VirtualMachineIntegrationServicesStatus, VirtualMachineIntegrationServicesStatus>(integrationServicesStatus, 16385);

	public VirtualMachineStorageInformation StorageInformation => LoadAsync(storageInformation, 65536);

	public VirtualMachineCheckpointInformation Checkpoints
	{
		get
		{
			VirtualMachineCheckpointInformation result = GetCheckpointInformation();
			EnqueueCheckpointsRefreshOperation();
			return result;
		}
	}

	public bool AreCheckpointsLoaded
	{
		get
		{
			return areCheckpointsLoaded;
		}
		set
		{
			if (areCheckpointsLoaded != value)
			{
				areCheckpointsLoaded = value;
				OnPropertyChanged("AreCheckpointsLoaded");
			}
		}
	}

	public VirtualMachineComputerSystemOperationalStatus MigrationState => LoadAsync<VirtualMachineComputerSystemOperationalStatus, VirtualMachineComputerSystemOperationalStatus>(migrationState, 2).GetMigrationState();

	public int? MigrationProgress => LoadAsync(migrationProgress, 2);

	public VirtualMachineReplicationState? ReplicationState
	{
		get
		{
			LoadAsync(primaryRelationship, 32770);
			VirtualMachineReplicationData virtualMachineReplicationData = primaryRelationship;
			if (virtualMachineReplicationData == null)
			{
				return null;
			}
			if (virtualMachineReplicationData.ReplicationState == VirtualMachineReplicationState.WaitingToCompleteInitialReplication && (virtualMachineReplicationData.ReplicationMode == VirtualMachineReplicationMode.Recovery || virtualMachineReplicationData.ReplicationMode == VirtualMachineReplicationMode.ExtendedReplica))
			{
				return VirtualMachineReplicationState.Ready;
			}
			return virtualMachineReplicationData.ReplicationState;
		}
	}

	public int? ReplicationTaskProgress
	{
		get
		{
			LoadAsync(primaryRelationship, 32769);
			return primaryRelationship?.ReplicationTaskProgress;
		}
	}

	public string ReplicationTaskName
	{
		get
		{
			LoadAsync(primaryRelationship, 32769);
			return primaryRelationship?.ReplicationTaskName;
		}
	}

	public DateTime? LastReplicaTime
	{
		get
		{
			LoadAsync(primaryRelationship, 32769);
			return primaryRelationship?.LastReplicaTime;
		}
	}

	public string ReplicationRecoveryServerFullyQualifiedDomainName
	{
		get
		{
			LoadAsync(primaryRelationship, 32769);
			return primaryRelationship?.RecoveryServerName;
		}
	}

	public string ReplicationPrimaryServerFullyQualifiedDomainName
	{
		get
		{
			LoadAsync(primaryRelationship, 32769);
			return primaryRelationship?.PrimaryServerName;
		}
	}

	public string ReplicationPrimaryConnectionPoint
	{
		get
		{
			LoadAsync(primaryRelationship, 32769);
			return primaryRelationship?.PrimaryConnectionPoint;
		}
	}

	public string ReplicationRecoveryConnectionPoint
	{
		get
		{
			LoadAsync(primaryRelationship, 32769);
			return primaryRelationship?.RecoveryConnectionPoint;
		}
	}

	public VirtualMachineReplicationMode? ReplicationMode
	{
		get
		{
			LoadAsync(primaryRelationship, 32770);
			return primaryRelationship?.ReplicationMode;
		}
	}

	public VirtualMachineReplicationHealth? ReplicationHealth
	{
		get
		{
			LoadAsync(primaryRelationship, 32770);
			return primaryRelationship?.ReplicationHealth;
		}
	}

	public VirtualMachineReplicationState? ExtendedReplicationState
	{
		get
		{
			LoadAsync(extendedRelationship, 131074);
			return extendedRelationship?.ReplicationState;
		}
	}

	public int? ExtendedReplicationTaskProgress
	{
		get
		{
			LoadAsync(extendedRelationship, 131073);
			return extendedRelationship?.ReplicationTaskProgress;
		}
	}

	public string ExtendedReplicationTaskName
	{
		get
		{
			LoadAsync(extendedRelationship, 131073);
			return extendedRelationship?.ReplicationTaskName;
		}
	}

	public DateTime? ExtendedLastReplicaTime
	{
		get
		{
			LoadAsync(extendedRelationship, 131073);
			return extendedRelationship?.LastReplicaTime;
		}
	}

	public string ExtendedReplicationRecoveryServerFullyQualifiedDomainName
	{
		get
		{
			LoadAsync(extendedRelationship, 131073);
			return extendedRelationship?.RecoveryServerName;
		}
	}

	public string ExtendedReplicationPrimaryServerFullyQualifiedDomainName
	{
		get
		{
			LoadAsync(extendedRelationship, 131073);
			return extendedRelationship?.PrimaryServerName;
		}
	}

	public string ExtendedReplicationPrimaryConnectionPoint
	{
		get
		{
			LoadAsync(extendedRelationship, 131073);
			return extendedRelationship?.PrimaryConnectionPoint;
		}
	}

	public string ExtendedReplicationRecoveryConnectionPoint
	{
		get
		{
			LoadAsync(extendedRelationship, 131073);
			return extendedRelationship?.RecoveryConnectionPoint;
		}
	}

	public VirtualMachineReplicationMode? ExtendedReplicationMode
	{
		get
		{
			LoadAsync(extendedRelationship, 131074);
			return extendedRelationship?.ReplicationMode;
		}
	}

	public VirtualMachineReplicationHealth? ExtendedReplicationHealth
	{
		get
		{
			LoadAsync(extendedRelationship, 131074);
			return extendedRelationship?.ReplicationHealth;
		}
	}

	public ImageSource GuestDesktopThumbnail
	{
		get
		{
			ImageSource thumbnail = GetThumbnail();
			EnqueueRefreshOperation();
			return thumbnail;
		}
	}

	public ObservableCollection<VMServiceInfo> VMServices => WeakReferenceEx.ReturnInstance(ref vmServices, delegate
	{
		Global.DefaultDispatcher.Invoke(delegate
		{
			VMServicesException = null;
		});
		OnPropertyChanged("VMServicesException");
		ObservableCollection<VMServiceInfo> services = new ObservableCollection<VMServiceInfo>();
		vmServices = new WeakReferenceEx<ObservableCollection<VMServiceInfo>>(services);
		LoadAsync(delegate(ClusterLoadedEventArgs loadResult)
		{
			if (loadResult.Error != null)
			{
				Error = loadResult.Error;
				Global.DefaultDispatcher.Invoke(delegate
				{
					services.Add(new VMServiceInfo
					{
						DisplayName = string.Empty
					});
				});
			}
			else
			{
				IList<VMServiceInfo> serviceList = new List<VMServiceInfo>();
				string text = GuestComputerName;
				if (!string.IsNullOrEmpty(text))
				{
					SafeServiceHandle serviceControllerHandle = null;
					try
					{
						serviceControllerHandle = NativeMethods.OpenSCManager(text, null, 4);
						if (serviceControllerHandle.IsInvalid)
						{
							throw new Win32Exception(Marshal.GetLastWin32Error(), ExceptionResources.FailedToConnectToServiceManager.FormatCurrentCulture(text));
						}
						ServiceController[] services2 = ServiceController.GetServices(text);
						List<ClusterVMMonitoredItem> monitoredItems = new List<ClusterVMMonitoredItem>(ClusterVMMonitoredItem.GetMonitoredItems(text, CancellationToken.None));
						services2.ForEach(delegate(ServiceController service)
						{
							if (service.ServiceType == ServiceType.Win32OwnProcess || service.ServiceName.Equals("spooler", StringComparison.OrdinalIgnoreCase) || service.ServiceName.Equals("W3SVC", StringComparison.OrdinalIgnoreCase) || service.ServiceName.Equals("WAS", StringComparison.OrdinalIgnoreCase))
							{
								serviceList.Add(new VMServiceInfo
								{
									Name = service.ServiceName,
									DisplayName = service.DisplayName,
									Description = GetServiceDescription(serviceControllerHandle, service.ServiceName),
									IsMonitored = monitoredItems.Exists((ClusterVMMonitoredItem item) => item is ClusterVMMonitoredService && item.Name.Equals(service.ServiceName, StringComparison.CurrentCultureIgnoreCase)),
									Status = ServiceStatusText(service.Status)
								});
							}
						});
						Global.DefaultDispatcher.Invoke(delegate
						{
							services.AddAll(serviceList.OrderBy((VMServiceInfo s) => s.DisplayName));
						});
						return;
					}
					catch (Win32Exception ex)
					{
						Win32Exception ex2 = ex;
						Win32Exception e = ex2;
						Global.DefaultDispatcher.Invoke(delegate
						{
							VMServicesException = e;
						});
						OnPropertyChanged("VMServicesException");
						return;
					}
					finally
					{
						if (serviceControllerHandle != null)
						{
							serviceControllerHandle.Close();
						}
					}
				}
				Global.DefaultDispatcher.Invoke(delegate
				{
					services.Add(new VMServiceInfo
					{
						DisplayName = string.Empty
					});
				});
			}
		}, (ResourceLoadSelection)16384);
		return services;
	});

	public Win32Exception VMServicesException { get; set; }

	public ObservableCollection<VMServiceInfo> VMMonitoredServices => WeakReferenceEx.ReturnInstance(ref vmMonitoredServices, delegate
	{
		ObservableCollection<VMServiceInfo> target = new ObservableCollection<VMServiceInfo>();
		vmMonitoredServices = new WeakReferenceEx<ObservableCollection<VMServiceInfo>>(target);
		RefreshMonitoredServices();
		return vmMonitoredServices.Target;
	});

	public event EventHandler<ClusterResourceImageEventArgs> GuestDesktopImageChanged;

	public event EventHandler<ClusterResourceVirtualMachineGuestStatusEventArgs> GuestStatusChanged;

	public event EventHandler<ClusterResourceVirtualMachineGuestSummaryEventArgs> GuestSummaryChanged;

	public event EventHandler<ClusterResourceVirtualMachineReplicationEventArgs> VirtualMachineReplicationChanged;

	public event EventHandler<ClusterResourceVirtualMachineStorageSummaryEventArgs> StorageInformationChanged;

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		base.InitializeMoreActionsCommands(commandContainer);
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref configureMonitoringCommandWeak, () => GetConfigureMonitoringCommand(this, ClusterCommandCollectionId.Resource));
		base.ApplicationStatusChanged += ConfigureMonitoringCommandUpdate;
		clusterCommand.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ConfigureMonitoringCommandUpdate;
			configureMonitoringCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(clusterCommand);
		ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref deleteSavedStateCommandWeak, () => new ClusterCommand(this, "DeleteSavedState", ClusterCommandId.VirtualMachineResourceDeleteSavedState, ClusterCommandCollectionId.VirtualMachineResource)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_DeleteSavedState,
			CanExecuteDelegate = (object x) => (VirtualMachineState)ApplicationStatus == VirtualMachineState.Saved,
			ExecuteDelegate = delegate
			{
				DeleteSavedState(askConfirmation: true);
			}
		});
		base.ApplicationStatusChanged += DeleteSavedStateCommandUpdate;
		clusterCommand2.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= DeleteSavedStateCommandUpdate;
			deleteSavedStateCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(clusterCommand2);
		ClusterCommand clusterCommand3 = WeakReferenceEx.ReturnInstance(ref pauseCommandWeak, () => new ClusterCommand(this, "Pause", ClusterCommandId.VirtualMachineResourcePause, ClusterCommandCollectionId.VirtualMachineResource)
		{
			Text = CommandResources.PauseAction_Text,
			CanExecuteDelegate = (object x) => (VirtualMachineState)ApplicationStatus == VirtualMachineState.Running,
			ExecuteDelegate = delegate
			{
				Pause();
			}
		});
		base.ApplicationStatusChanged += PauseCommandUpdate;
		clusterCommand3.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= PauseCommandUpdate;
			pauseCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(clusterCommand3);
		ClusterCommand clusterCommand4 = WeakReferenceEx.ReturnInstance(ref resumeCommandWeak, () => new ClusterCommand(this, "Resume", ClusterCommandId.VirtualMachineResourceResume, ClusterCommandCollectionId.VirtualMachineResource)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Resume,
			CanExecuteDelegate = (object x) => (VirtualMachineState)ApplicationStatus == VirtualMachineState.Paused,
			ExecuteDelegate = delegate
			{
				Resume();
			}
		});
		base.ApplicationStatusChanged += ResumeCommandUpdate;
		clusterCommand4.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ResumeCommandUpdate;
			resumeCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(clusterCommand4);
		ClusterCommand clusterCommand5 = WeakReferenceEx.ReturnInstance(ref resetCommandWeak, () => new ClusterCommand(this, "Reset", ClusterCommandId.VirtualMachineResourceReset, ClusterCommandCollectionId.VirtualMachineResource)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Reset,
			CanExecuteDelegate = (object x) => (VirtualMachineState)ApplicationStatus != VirtualMachineState.Paused,
			ExecuteDelegate = delegate
			{
				Reset(askConfirmation: true);
			}
		});
		base.ApplicationStatusChanged += ResetCommandUpdate;
		clusterCommand5.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ResetCommandUpdate;
			resumeCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(clusterCommand5);
	}

	protected virtual void InitializeApplicationCommands(CommandCollection commandsCollection)
	{
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref connectCommandWeak, () => GetConnectCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += ConnectCommandUpdate;
		clusterCommand.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ConnectCommandUpdate;
			connectCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand);
		ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref manageCommandWeak, () => GetManageCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += ManageCommandUpdate;
		clusterCommand2.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ManageCommandUpdate;
			manageCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand2);
		ClusterCommand clusterCommand3 = WeakReferenceEx.ReturnInstance(ref settingsCommandWeak, () => GetSettingsCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += SettingsCommandUpdate;
		clusterCommand3.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= SettingsCommandUpdate;
			settingsCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand3);
		ClusterCommand clusterCommand4 = WeakReferenceEx.ReturnInstance(ref startCommandWeak, () => GetStartCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += StartCommandUpdate;
		clusterCommand4.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= StartCommandUpdate;
			startCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand4);
		ClusterCommand clusterCommand5 = WeakReferenceEx.ReturnInstance(ref turnoffCommandWeak, () => GetTurnOffCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += TurnOffCommandUpdate;
		clusterCommand5.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= TurnOffCommandUpdate;
			turnoffCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand5);
		ClusterCommand clusterCommand6 = WeakReferenceEx.ReturnInstance(ref shutdownCommandWeak, () => GetShutdownCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += ShutdownCommandUpdate;
		clusterCommand6.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ShutdownCommandUpdate;
			shutdownCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand6);
		ClusterCommand clusterCommand7 = WeakReferenceEx.ReturnInstance(ref saveCommandWeak, () => GetSaveCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += SaveCommandUpdate;
		clusterCommand7.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= SaveCommandUpdate;
			saveCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand7);
		ClusterCommand clusterCommand8 = WeakReferenceEx.ReturnInstance(ref checkpointCommandWeak, () => GetCheckpointCommand(this, ClusterCommandCollectionId.Checkpoint));
		base.ApplicationStatusChanged += CheckpointCommandUpdate;
		clusterCommand8.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= CheckpointCommandUpdate;
			checkpointCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand8);
		ClusterCommand clusterCommand9 = WeakReferenceEx.ReturnInstance(ref revertCheckpointCommandWeak, () => GetRevertCheckpointCommand(this, ClusterCommandCollectionId.Checkpoint));
		base.ApplicationStatusChanged += RevertCheckpointCommandUpdate;
		clusterCommand9.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= RevertCheckpointCommandUpdate;
			revertCheckpointCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand9);
		ClusterCommand clusterCommand10 = WeakReferenceEx.ReturnInstance(ref exportCommandWeak, () => GetExportCommand(this, commandsCollection.Category));
		base.ApplicationStatusChanged += ExportCommandUpdate;
		clusterCommand10.Finalizing += delegate
		{
			base.ApplicationStatusChanged -= ExportCommandUpdate;
			exportCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand10);
		commandsCollection.Add(GetReplicationCommandsContainer(this, commandsCollection, ClusterIdentityType.Resource, null));
	}

	internal static ClusterCommand GetConnectCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceConnect)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Connect", commandId, collectionId)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Connect,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => !(resourceParam == null) && (VirtualMachineState)resourceParam.ApplicationStatus != VirtualMachineState.Fetching),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.Connect(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetManageCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceManage)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Manage", commandId, collectionId)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Manage,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => !(resourceParam == null) && (VirtualMachineState)resourceParam.ApplicationStatus != VirtualMachineState.Fetching),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.Manage(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetSettingsCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceSettings)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Settings", commandId, collectionId)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Settings,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => !(resourceParam == null) && (VirtualMachineState)resourceParam.ApplicationStatus != VirtualMachineState.Fetching),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.Settings(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetCheckpointCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceCheckpoint)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Checkpoint", commandId, collectionId)
		{
			Text = CommandResources.VirtualMachineCheckpointCommand_Text,
			Description = CommandResources.VirtualMachineCheckpointCommand_Description,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => !(resourceParam == null) && (VirtualMachineState)resourceParam.ApplicationStatus != VirtualMachineState.Fetching),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.TakeCheckpoint(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetRevertCheckpointCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceCheckpointRevert)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "RevertCheckpoint", commandId, collectionId)
		{
			Text = CommandResources.VirtualMachineRevertCheckpointCommand_Text,
			Description = CommandResources.VirtualMachineRevertCheckpointCommand_Description,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => true),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.RevertCheckpoint(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetExportCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineExport)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Export", commandId, collectionId)
		{
			Text = CommandResources.VirtualMachineExportVmOrCheckpointCommand_Text,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => !(resourceParam == null) && (VirtualMachineState)resourceParam.ApplicationStatus != VirtualMachineState.Fetching),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.Export(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetExportCheckpointCommand(ClusterObject clusterObject, Checkpoint checkpoint, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineExport)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		Exceptions.ThrowIfNull(checkpoint, "checkpoint");
		return new ClusterCommand(clusterObject, "Export", commandId, collectionId)
		{
			Text = CommandResources.VirtualMachineExportVmOrCheckpointCommand_Text,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => !(resourceParam == null) && (VirtualMachineState)resourceParam.ApplicationStatus != VirtualMachineState.Fetching),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.Export(clusterObject.SetLastErrorIfNecessary, checkpoint);
				});
			}
		};
	}

	internal static ClusterCommand GetConfigureMonitoringCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		VirtualMachineResource obj = clusterObject as VirtualMachineResource;
		VirtualMachineGroup virtualMachineGroup = clusterObject as VirtualMachineGroup;
		if (obj == null && virtualMachineGroup == null)
		{
			throw new ArgumentException("Can only monitor VirtualMachineResources and VirtualMachineGroups.");
		}
		return new ClusterCommand(clusterObject, "ConfigureMonitoring", ClusterCommandId.VirtualMachineConfigureMonitoring, collectionId)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_ConfigureMonitoring,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resourceParam) => !(resourceParam == null) && (VirtualMachineState)resourceParam.ApplicationStatus == VirtualMachineState.Running),
			CommandParameter = clusterObject,
			ExecuteDelegate = delegate(object services)
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resourceParam)
				{
					resourceParam.ConfigureMonitoring(services as ObservableCollection<VMServiceInfo>);
				});
			},
			InputParameters = new InputParameterList<object>()
		};
	}

	internal static ClusterCommand GetStartCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceStart)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Start", commandId, collectionId)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Start,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, delegate(VirtualMachineResource resource)
			{
				if (resource == null)
				{
					return false;
				}
				VirtualMachineState virtualMachineState = (VirtualMachineState)resource.ApplicationStatus;
				return virtualMachineState != VirtualMachineState.Fetching && virtualMachineState != VirtualMachineState.Running && virtualMachineState != VirtualMachineState.Pausing && virtualMachineState != VirtualMachineState.Resuming && virtualMachineState != VirtualMachineState.Saving && virtualMachineState != VirtualMachineState.Starting && virtualMachineState != VirtualMachineState.Paused && virtualMachineState != VirtualMachineState.Stopping;
			}),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resource)
				{
					resource.Start(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetTurnOffCommand(ClusterObject clusterObject, ClusterCommandCollectionId id, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceTurnoff)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "TurnOff", commandId, id)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_TurnOff,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, delegate(VirtualMachineResource resource)
			{
				if (resource == null)
				{
					return false;
				}
				VirtualMachineState virtualMachineState = (VirtualMachineState)resource.ApplicationStatus;
				return virtualMachineState == VirtualMachineState.Running || virtualMachineState == VirtualMachineState.Paused;
			}),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resource)
				{
					resource.Turnoff(clusterObject.SetLastErrorIfNecessary, askConfirmation: true);
				});
			}
		};
	}

	internal static ClusterCommand GetShutdownCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceShutdown)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Shutdown", commandId, collectionId)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Shutdown,
			CanExecuteDelegate = (object x) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, (VirtualMachineResource resource) => !(resource == null) && (VirtualMachineState)resource.ApplicationStatus == VirtualMachineState.Running),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resource)
				{
					resource.Shutdown(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	internal static ClusterCommand GetSaveCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId, Action updateCanExecute = null, ClusterCommandId commandId = ClusterCommandId.VirtualMachineResourceSave)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		return new ClusterCommand(clusterObject, "Save", commandId, collectionId)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Save,
			CanExecuteDelegate = (object _) => CanExecuteOnGroupOrResource(clusterObject, updateCanExecute, delegate(VirtualMachineResource resource)
			{
				if (resource == null)
				{
					return false;
				}
				VirtualMachineState virtualMachineState = (VirtualMachineState)resource.ApplicationStatus;
				return virtualMachineState == VirtualMachineState.Running || virtualMachineState == VirtualMachineState.Paused;
			}),
			ExecuteDelegate = delegate
			{
				ExecuteOnGroupOrResource(clusterObject, delegate(VirtualMachineResource resource)
				{
					resource.Save(clusterObject.SetLastErrorIfNecessary);
				});
			}
		};
	}

	private static bool CanExecuteOnGroupOrResource(ClusterObject clusterObject, Action updateCanExecute, Predicate<VirtualMachineResource> resourcePredicate)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		VirtualMachineResource virtualMachineResource2 = clusterObject as VirtualMachineResource;
		VirtualMachineGroup virtualMachineGroup = clusterObject as VirtualMachineGroup;
		if (virtualMachineResource2 == null && virtualMachineGroup == null)
		{
			throw new ArgumentException("Only VirtualMachineResource and VirtualMachineGroup can be saved.");
		}
		if (virtualMachineResource2 != null)
		{
			return resourcePredicate(virtualMachineResource2);
		}
		if (updateCanExecute == null)
		{
			throw new ArgumentException("Need to update command when VM resource is loaded.");
		}
		bool canExecute = false;
		virtualMachineGroup.ExecuteOnVmResource(delegate(VirtualMachineResource virtualMachineResource)
		{
			canExecute = resourcePredicate(virtualMachineResource);
			updateCanExecute();
		});
		return canExecute;
	}

	private static void ExecuteOnGroupOrResource(ClusterObject clusterObject, Action<VirtualMachineResource> executeDelegate)
	{
		Exceptions.ThrowIfNull(clusterObject, "clusterObject");
		VirtualMachineResource virtualMachineResource = clusterObject as VirtualMachineResource;
		VirtualMachineGroup virtualMachineGroup = clusterObject as VirtualMachineGroup;
		if (virtualMachineResource == null && virtualMachineGroup == null)
		{
			throw new ArgumentException("Only VirtualMachineResource and VirtualMachineGroup can be saved.");
		}
		if (virtualMachineResource != null)
		{
			executeDelegate(virtualMachineResource);
		}
		else
		{
			virtualMachineGroup.ExecuteOnVmResource(executeDelegate);
		}
	}

	internal ClusterCommandContainer GetReplicationCommandsContainer(ClusterObject clusterObject, CommandCollection commandsCollection, ClusterIdentityType identityType, Action<string> signalAction)
	{
		ClusterCommandContainer clusterCommandContainer = WeakReferenceEx.ReturnInstance(ref replicationCommandsContainerWeak, delegate
		{
			ClusterCommandContainer obj = new ClusterCommandContainer(this, "Replication", ClusterCommandId.VirtualMachineResourceReplication, signalAction)
			{
				Text = EnumResources.VirtualMachine_Replication,
				CanExecuteDelegate = (object x) => true,
				Visible = CalculateReplicationCommandContainerVisibility()
			};
			ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref enableReplicationCommandWeak, () => GetEnableReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += EnableReplicationCommandUpdate;
			clusterCommand.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= EnableReplicationCommandUpdate;
				enableReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref extendReplicationCommandWeak, () => GetExtendReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += ExtendReplicationCommandUpdate;
			clusterCommand2.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= ExtendReplicationCommandUpdate;
				extendReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand3 = WeakReferenceEx.ReturnInstance(ref viewReplicationHealthCommandWeak, () => GetViewReplicationHealthCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += ViewReplicationHealthCommandUpdate;
			clusterCommand3.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= ViewReplicationHealthCommandUpdate;
				viewReplicationHealthCommandWeak = null;
			};
			ClusterCommand clusterCommand4 = WeakReferenceEx.ReturnInstance(ref initializeReplicationCommandWeak, () => GetInitializeReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += InitializeReplicationCommandUpdate;
			clusterCommand4.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= InitializeReplicationCommandUpdate;
				initializeReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand5 = WeakReferenceEx.ReturnInstance(ref importReplicationCommandWeak, () => GetImportReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += ImportReplicationCommandUpdate;
			clusterCommand5.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= ImportReplicationCommandUpdate;
				importReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand6 = WeakReferenceEx.ReturnInstance(ref removeReplicationCommandWeak, () => GetRemoveReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += RemoveReplicationCommandUpdate;
			clusterCommand6.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= RemoveReplicationCommandUpdate;
				removeReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand7 = WeakReferenceEx.ReturnInstance(ref cancelInitializeReplicationCommandWeak, () => GetCancelInitialReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += CancelInitializeReplicationCommandUpdate;
			clusterCommand7.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= CancelInitializeReplicationCommandUpdate;
				cancelInitializeReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand8 = WeakReferenceEx.ReturnInstance(ref cancelDiskUpdateReplicationCommandWeak, () => GetCancelDiskUpdateReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += CancelDiskUpdateReplicationCommandUpdate;
			clusterCommand8.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= CancelDiskUpdateReplicationCommandUpdate;
				cancelDiskUpdateReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand9 = WeakReferenceEx.ReturnInstance(ref commitFailoverCommandWeak, () => GetCommitFailoverCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += CommitFailoverReplication;
			clusterCommand9.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= CommitFailoverReplication;
				commitFailoverCommandWeak = null;
			};
			ClusterCommand clusterCommand10 = WeakReferenceEx.ReturnInstance(ref pauseReplicationCommandWeak, () => GetPauseReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += PauseReplicationCommandUpdate;
			clusterCommand10.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= PauseReplicationCommandUpdate;
				pauseReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand11 = WeakReferenceEx.ReturnInstance(ref pauseExtendedReplicationCommandWeak, () => GetPauseExtendedReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += PauseExtendReplicationCommandUpdate;
			clusterCommand11.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= PauseExtendReplicationCommandUpdate;
				pauseExtendedReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand12 = WeakReferenceEx.ReturnInstance(ref cancelResynchronizeCommandWeak, () => GetCancelResynchronizeCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += CancelResyncronizeCommandUpdate;
			clusterCommand12.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= CancelResyncronizeCommandUpdate;
				cancelResynchronizeCommandWeak = null;
			};
			ClusterCommand clusterCommand13 = WeakReferenceEx.ReturnInstance(ref resumeReplicationCommandWeak, () => GetResumeReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += ResumeReplicationCommandUpdate;
			clusterCommand13.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= ResumeReplicationCommandUpdate;
				resumeReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand14 = WeakReferenceEx.ReturnInstance(ref resumeExtendedReplicationCommandWeak, () => GetResumeExtendedReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += ResumeExtendReplicationCommandUpdate;
			clusterCommand14.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= ResumeExtendReplicationCommandUpdate;
				resumeExtendedReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand15 = WeakReferenceEx.ReturnInstance(ref reverseReplicationCommandWeak, () => GetReverseReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += ReverseReplicationRelationshipCommandUpdate;
			clusterCommand15.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= ReverseReplicationRelationshipCommandUpdate;
				reverseReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand16 = WeakReferenceEx.ReturnInstance(ref prepareFailoverReplicationCommandWeak, () => GetPrepareFailoverReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += PrepareFailoverReplicationCommandUpdate;
			clusterCommand16.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= PrepareFailoverReplicationCommandUpdate;
				prepareFailoverReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand17 = WeakReferenceEx.ReturnInstance(ref cancelPrepareFailoverReplicationCommandWeak, () => GetCancelPrepareFailoverReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += CancelPrepareFailoverReplicationCommandUpdate;
			clusterCommand17.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= CancelPrepareFailoverReplicationCommandUpdate;
				cancelPrepareFailoverReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand18 = WeakReferenceEx.ReturnInstance(ref startRecoveryCommandWeak, () => GetStartRecoveryCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += StartRecoveryCommandUpdate;
			clusterCommand18.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= StartRecoveryCommandUpdate;
				startRecoveryCommandWeak = null;
			};
			ClusterCommand clusterCommand19 = WeakReferenceEx.ReturnInstance(ref cancelFailoverReplicationCommandWeak, () => GetCancelFailoverReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += CancelFailoverReplicationCommandUpdate;
			clusterCommand19.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= CancelFailoverReplicationCommandUpdate;
				cancelFailoverReplicationCommandWeak = null;
			};
			ClusterCommand clusterCommand20 = WeakReferenceEx.ReturnInstance(ref testRecoveryCommandWeak, () => GetTestRecoveryCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += TestRecoveryCommandUpdate;
			clusterCommand20.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= TestRecoveryCommandUpdate;
				testRecoveryCommandWeak = null;
			};
			ClusterCommand clusterCommand21 = WeakReferenceEx.ReturnInstance(ref cancelTestFailoverReplicationCommandWeak, () => GetCancelTestFailoverReplicationCommand(clusterObject, commandsCollection.Category));
			base.ApplicationStatusChanged += CancelTestFailoverReplicationCommandUpdate;
			clusterCommand21.Finalizing += delegate
			{
				base.ApplicationStatusChanged -= CancelTestFailoverReplicationCommandUpdate;
				cancelTestFailoverReplicationCommandWeak = null;
			};
			obj.ChildrenInternal.Add(clusterCommand);
			obj.ChildrenInternal.Add(clusterCommand15);
			obj.ChildrenInternal.Add(clusterCommand5);
			obj.ChildrenInternal.Add(clusterCommand9);
			obj.ChildrenInternal.Add(clusterCommand16);
			obj.ChildrenInternal.Add(clusterCommand17);
			obj.ChildrenInternal.Add(clusterCommand18);
			obj.ChildrenInternal.Add(clusterCommand19);
			obj.ChildrenInternal.Add(clusterCommand20);
			obj.ChildrenInternal.Add(clusterCommand21);
			obj.ChildrenInternal.Add(clusterCommand10);
			obj.ChildrenInternal.Add(clusterCommand13);
			obj.ChildrenInternal.Add(clusterCommand11);
			obj.ChildrenInternal.Add(clusterCommand14);
			obj.ChildrenInternal.Add(clusterCommand2);
			obj.ChildrenInternal.Add(clusterCommand12);
			obj.ChildrenInternal.Add(clusterCommand4);
			obj.ChildrenInternal.Add(clusterCommand7);
			obj.ChildrenInternal.Add(clusterCommand8);
			obj.ChildrenInternal.Add(clusterCommand3);
			obj.ChildrenInternal.Add(clusterCommand6);
			return obj;
		});
		clusterCommandContainer.Finalizing += delegate
		{
			replicationCommandsContainerWeak = null;
		};
		if (identityType == ClusterIdentityType.Group)
		{
			groupCommandContainer = new WeakReferenceEx<ClusterCommandContainer>(clusterCommandContainer);
		}
		return clusterCommandContainer;
	}

	private ClusterCommand GetViewReplicationHealthCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "ViewReplicationHealth", ClusterCommandId.VirtualMachineViewReplicationHealth, collectionId)
		{
			Text = EnumResources.VirtualMachine_View_Replication_Health,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineViewReplicationHealth),
			ExecuteDelegate = delegate
			{
				ViewReplicationHealth(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetEnableReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "EnableReplication", ClusterCommandId.VirtualMachineEnableReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Enable_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineEnableReplication),
			ExecuteDelegate = delegate
			{
				EnableReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetExtendReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "ExtendReplication", ClusterCommandId.VirtualMachineExtendReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Extend_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineExtendReplication),
			ExecuteDelegate = delegate
			{
				EnableReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetInitializeReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "InitializeReplication", ClusterCommandId.VirtualMachineInitializeReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Initialize_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineInitializeReplication),
			ExecuteDelegate = delegate
			{
				InitializeReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetImportReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "ImportReplication", ClusterCommandId.VirtualMachineImportReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Import_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineImportReplication),
			ExecuteDelegate = delegate
			{
				ImportReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetCancelInitialReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "CancelInitialReplication", ClusterCommandId.VirtualMachineCancelInitialReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Cancel_Initialize_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineCancelInitialReplication),
			ExecuteDelegate = delegate
			{
				CancelInitializeReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetCancelDiskUpdateReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "CancelDiskUpdateReplication", ClusterCommandId.VirtualMachineCancelDiskUpdateReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Cancel_Disk_Update_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineCancelDiskUpdateReplication),
			ExecuteDelegate = delegate
			{
				CancelDiskUpdateReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetCommitFailoverCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "CommitFailover", ClusterCommandId.VirtualMachineCommitFailover, collectionId)
		{
			Text = EnumResources.VirtualMachine_Commit_Failover_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineCommitFailover),
			ExecuteDelegate = delegate
			{
				CommitFailoverReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetRemoveReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "RemoveReplication", ClusterCommandId.VirtualMachineRemoveReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Remove_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineRemoveReplication),
			ExecuteDelegate = delegate
			{
				RemoveReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetPauseReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "PauseReplication", ClusterCommandId.VirtualMachinePauseReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Pause_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachinePauseReplication),
			ExecuteDelegate = delegate
			{
				PauseReplication(clusterObject.SetLastErrorIfNecessary, ClusterCommandId.VirtualMachinePauseReplication);
			}
		};
	}

	internal ClusterCommand GetPauseExtendedReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "PauseExtendedReplication", ClusterCommandId.VirtualMachinePauseExtendedReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Pause_Extended_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachinePauseExtendedReplication),
			ExecuteDelegate = delegate
			{
				PauseReplication(clusterObject.SetLastErrorIfNecessary, ClusterCommandId.VirtualMachinePauseExtendedReplication);
			}
		};
	}

	internal ClusterCommand GetCancelResynchronizeCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "CancelResynchronize", ClusterCommandId.VirtualMachineCancelResynchronize, collectionId)
		{
			Text = EnumResources.VirtualMachine_Cancel_Resynchronize,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineCancelResynchronize),
			ExecuteDelegate = delegate
			{
				CancelResynchronize(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetResumeReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "ResumeReplication", ClusterCommandId.VirtualMachineResumeReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Resume_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineResumeReplication),
			ExecuteDelegate = delegate
			{
				ResumeReplication(clusterObject.SetLastErrorIfNecessary, primaryRelationship, extendedRelationship, ClusterCommandId.VirtualMachineResumeReplication);
			}
		};
	}

	internal ClusterCommand GetResumeExtendedReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "ResumeExtendedReplication", ClusterCommandId.VirtualMachineResumeExtendedReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Resume_Extended_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineResumeExtendedReplication),
			ExecuteDelegate = delegate
			{
				ResumeReplication(clusterObject.SetLastErrorIfNecessary, primaryRelationship, extendedRelationship, ClusterCommandId.VirtualMachineResumeExtendedReplication);
			}
		};
	}

	internal ClusterCommand GetReverseReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "ReverseReplication", ClusterCommandId.VirtualMachineReverseReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Reverse_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineReverseReplication),
			ExecuteDelegate = delegate
			{
				EnableReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetPrepareFailoverReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "PrepareFailoverReplication", ClusterCommandId.VirtualMachinePrepareFailoverReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Prepare_Failover,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachinePrepareFailoverReplication),
			ExecuteDelegate = delegate
			{
				PrepareFailoverReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetCancelPrepareFailoverReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "CancelPrepareFailoverReplication", ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Cancel_Failover_Preparation,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication),
			ExecuteDelegate = delegate
			{
				CancelPrepareFailoverReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetStartRecoveryCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "StartRecovery", ClusterCommandId.VirtualMachineStartRecovery, collectionId)
		{
			Text = EnumResources.VirtualMachine_Start_Recovery,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineStartRecovery),
			ExecuteDelegate = delegate
			{
				StartRecovery(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetCancelFailoverReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "CancelFailoverReplication", ClusterCommandId.VirtualMachineCancelFailoverReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Cancel_Failover_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineCancelFailoverReplication),
			ExecuteDelegate = delegate
			{
				CancelFailoverReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetTestRecoveryCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "TestRecovery", ClusterCommandId.VirtualMachineTestRecovery, collectionId)
		{
			Text = EnumResources.VirtualMachine_Replication_Test_Recovery,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineTestRecovery),
			ExecuteDelegate = delegate
			{
				TestRecovery(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	internal ClusterCommand GetCancelTestFailoverReplicationCommand(ClusterObject clusterObject, ClusterCommandCollectionId collectionId)
	{
		return new ClusterCommand(this, "CancelTestFailoverReplication", ClusterCommandId.VirtualMachineCancelTestFailoverReplication, collectionId)
		{
			Text = EnumResources.VirtualMachine_Cancel_Test_Failover_Replication,
			CanExecuteDelegate = (object x) => true,
			Visible = CalculateReplicationCommandVisibility(ClusterCommandId.VirtualMachineCancelTestFailoverReplication),
			ExecuteDelegate = delegate
			{
				CancelTestFailoverReplication(clusterObject.SetLastErrorIfNecessary);
			}
		};
	}

	private void ConnectCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(connectCommandWeak, sender, e);
	}

	private void CheckpointCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(manageCommandWeak, sender, e);
	}

	private void RevertCheckpointCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(manageCommandWeak, sender, e);
	}

	private void ExportCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(exportCommandWeak, sender, e);
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

	private void DeleteSavedStateCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(deleteSavedStateCommandWeak, sender, e);
	}

	private void PauseCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(pauseCommandWeak, sender, e);
	}

	private void ResumeCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(resumeCommandWeak, sender, e);
	}

	private void ResetCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(resetCommandWeak, sender, e);
	}

	private void EnableReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(enableReplicationCommandWeak, sender, e);
	}

	private void ExtendReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(extendReplicationCommandWeak, sender, e);
	}

	private void ViewReplicationHealthCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(viewReplicationHealthCommandWeak, sender, e);
	}

	private void InitializeReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(initializeReplicationCommandWeak, sender, e);
	}

	private void StartRecoveryCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(startRecoveryCommandWeak, sender, e);
	}

	private void ReverseReplicationRelationshipCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(reverseReplicationCommandWeak, sender, e);
	}

	private void TestRecoveryCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(testRecoveryCommandWeak, sender, e);
	}

	private void CancelFailoverReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(cancelFailoverReplicationCommandWeak, sender, e);
	}

	private void ImportReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(importReplicationCommandWeak, sender, e);
	}

	private void RemoveReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(removeReplicationCommandWeak, sender, e);
	}

	private void CancelInitializeReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(cancelInitializeReplicationCommandWeak, sender, e);
	}

	private void CancelDiskUpdateReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(cancelDiskUpdateReplicationCommandWeak, sender, e);
	}

	private void CommitFailoverReplication(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(commitFailoverCommandWeak, sender, e);
	}

	private void PauseReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(pauseReplicationCommandWeak, sender, e);
	}

	private void PauseExtendReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(pauseExtendedReplicationCommandWeak, sender, e);
	}

	private void CancelResyncronizeCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(cancelResynchronizeCommandWeak, sender, e);
	}

	private void ResumeReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(resumeReplicationCommandWeak, sender, e);
	}

	private void ResumeExtendReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(resumeExtendedReplicationCommandWeak, sender, e);
	}

	private void PrepareFailoverReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(prepareFailoverReplicationCommandWeak, sender, e);
	}

	private void CancelPrepareFailoverReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(cancelPrepareFailoverReplicationCommandWeak, sender, e);
	}

	private void CancelTestFailoverReplicationCommandUpdate(object sender, ClusterApplicationStatusEventArgs e)
	{
		CommandUpdate(cancelTestFailoverReplicationCommandWeak, sender, e);
	}

	private static void CommandUpdate(WeakReferenceEx weakCommand, object sender, ClusterEventArgs e)
	{
		if (weakCommand != null && weakCommand.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.CanExecuteUpdate(sender, e);
		}
	}

	private bool CalculateReplicationCommandContainerVisibility()
	{
		VirtualMachineReplicationData virtualMachineReplicationData = primaryRelationship;
		if (virtualMachineReplicationData == null)
		{
			return false;
		}
		return virtualMachineReplicationData.ReplicationMode != VirtualMachineReplicationMode.TestReplica;
	}

	private bool CalculateReplicationCommandVisibilityForPrimary(ClusterCommandId replicationCommand, VirtualMachineReplicationData replicationData)
	{
		VirtualMachineReplicationState replicationState = replicationData.ReplicationState;
		if (replicationCommand == ClusterCommandId.VirtualMachineEnableReplication)
		{
			return false;
		}
		if (replicationCommand == ClusterCommandId.VirtualMachineViewReplicationHealth || (replicationCommand == ClusterCommandId.VirtualMachineRemoveReplication && !replicationData.IsEndpointProviderUsed))
		{
			return true;
		}
		if (replicationData.IsInitialReplicationPending && replicationState != VirtualMachineReplicationState.Ready && replicationState != VirtualMachineReplicationState.WaitingForUpdateCompletion && replicationCommand == ClusterCommandId.VirtualMachineCancelInitialReplication)
		{
			return true;
		}
		VirtualMachineState virtualMachineState = (this.virtualMachineState.HasValue ? this.virtualMachineState.Value : VirtualMachineState.Unknown);
		switch (replicationState)
		{
		case VirtualMachineReplicationState.Disabled:
			return false;
		case VirtualMachineReplicationState.Ready:
			if (virtualMachineState != VirtualMachineState.Paused && replicationCommand == ClusterCommandId.VirtualMachineInitializeReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.WaitingToCompleteInitialReplication:
			if (replicationCommand == ClusterCommandId.VirtualMachinePauseReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Resynchronize:
			if (replicationCommand == ClusterCommandId.VirtualMachinePauseReplication || replicationCommand == ClusterCommandId.VirtualMachineCancelResynchronize)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Replicating:
			if (replicationCommand == ClusterCommandId.VirtualMachinePauseReplication || (replicationCommand == ClusterCommandId.VirtualMachinePrepareFailoverReplication && !replicationData.IsEndpointProviderUsed))
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.SyncedReplicationComplete:
			if (replicationCommand == ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication && !replicationData.IsEndpointProviderUsed)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Suspended:
		case VirtualMachineReplicationState.Critical:
		case VirtualMachineReplicationState.WaitingForStartResynchronize:
		case VirtualMachineReplicationState.ResynchronizeSuspended:
			if (replicationCommand == ClusterCommandId.VirtualMachineResumeReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.WaitingForUpdateCompletion:
			if (replicationCommand == ClusterCommandId.VirtualMachineCancelDiskUpdateReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.UpdateCritical:
			if (replicationCommand == ClusterCommandId.VirtualMachineCancelDiskUpdateReplication || replicationCommand == ClusterCommandId.VirtualMachineResumeReplication)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private bool CalculateReplicationCommandVisibilityForRecovery(ClusterCommandId replicationCommand, VirtualMachineReplicationData primaryReplicationData, VirtualMachineReplicationData extendedReplicationData)
	{
		switch (replicationCommand)
		{
		case ClusterCommandId.VirtualMachineViewReplicationHealth:
			return true;
		case ClusterCommandId.VirtualMachineRemoveReplication:
			if ((extendedReplicationData.ReplicationState != 0 && !extendedReplicationData.IsEndpointProviderUsed) || (!primaryReplicationData.IsEndpointProviderUsed && (extendedReplicationData == null || extendedReplicationData.ReplicationState == VirtualMachineReplicationState.Disabled)))
			{
				return true;
			}
			break;
		}
		if (!CalculateReplicationCommandVisibilityOnRecoveryForExtendedRelation(replicationCommand, primaryReplicationData, extendedReplicationData))
		{
			return CalculateReplicationCommandVisibilityOnRecoveryForPrimaryRelation(replicationCommand, primaryReplicationData, extendedReplicationData);
		}
		return true;
	}

	private bool CalculateReplicationCommandVisibilityOnRecoveryForPrimaryRelation(ClusterCommandId replicationCommand, VirtualMachineReplicationData primaryReplicationData, VirtualMachineReplicationData extendedReplicationData)
	{
		switch (primaryReplicationData.ReplicationState)
		{
		case VirtualMachineReplicationState.Disabled:
			return false;
		case VirtualMachineReplicationState.WaitingToCompleteInitialReplication:
			if (replicationCommand == ClusterCommandId.VirtualMachineImportReplication || replicationCommand == ClusterCommandId.VirtualMachinePauseReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.WaitingForUpdateCompletion:
			if (!primaryReplicationData.IsInitialReplicationPending && (replicationCommand == ClusterCommandId.VirtualMachineStartRecovery || (replicationCommand == ClusterCommandId.VirtualMachineCancelTestFailoverReplication && primaryReplicationData.IsTestFailoverRunning)))
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Replicating:
			if (replicationCommand == ClusterCommandId.VirtualMachineStartRecovery || replicationCommand == ClusterCommandId.VirtualMachinePauseReplication || (replicationCommand == ClusterCommandId.VirtualMachineTestRecovery && !primaryReplicationData.IsTestFailoverRunning) || (replicationCommand == ClusterCommandId.VirtualMachineCancelTestFailoverReplication && primaryReplicationData.IsTestFailoverRunning))
			{
				return true;
			}
			if (replicationCommand == ClusterCommandId.VirtualMachineExtendReplication && extendedReplicationData != null && primaryReplicationData.ReplicationMode == VirtualMachineReplicationMode.Recovery && extendedReplicationData.ReplicationState == VirtualMachineReplicationState.Disabled && version != 0)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.ResynchronizeSuspended:
			if (replicationCommand == ClusterCommandId.VirtualMachineStartRecovery || replicationCommand == ClusterCommandId.VirtualMachineResumeReplication || (primaryReplicationData.IsTestFailoverRunning && replicationCommand == ClusterCommandId.VirtualMachineCancelTestFailoverReplication))
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Resynchronize:
			if (replicationCommand == ClusterCommandId.VirtualMachineStartRecovery || replicationCommand == ClusterCommandId.VirtualMachinePauseReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.FailoverInProgress:
			if (replicationCommand == ClusterCommandId.VirtualMachineCancelFailoverReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Recovered:
			if (replicationCommand == ClusterCommandId.VirtualMachineCancelFailoverReplication || replicationCommand == ClusterCommandId.VirtualMachineReverseReplication || replicationCommand == ClusterCommandId.VirtualMachineCommitFailover)
			{
				return true;
			}
			if (extendedReplicationData != null && extendedReplicationData.ReplicationState != 0 && extendedReplicationData.ReplicationState != VirtualMachineReplicationState.WaitingToCompleteInitialReplication && extendedReplicationData.ReplicationState != VirtualMachineReplicationState.Resynchronize && replicationCommand == ClusterCommandId.VirtualMachineResumeExtendedReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Committed:
			if (replicationCommand == ClusterCommandId.VirtualMachineReverseReplication)
			{
				return true;
			}
			if (extendedReplicationData != null && extendedReplicationData.ReplicationState != 0 && extendedReplicationData.ReplicationState != VirtualMachineReplicationState.WaitingToCompleteInitialReplication && extendedReplicationData.ReplicationState != VirtualMachineReplicationState.Resynchronize && replicationCommand == ClusterCommandId.VirtualMachineResumeExtendedReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Suspended:
		case VirtualMachineReplicationState.Critical:
			if (replicationCommand == ClusterCommandId.VirtualMachineResumeReplication)
			{
				return true;
			}
			if (!primaryReplicationData.IsInitialReplicationPending)
			{
				if (replicationCommand == ClusterCommandId.VirtualMachineExtendReplication && version != 0 && extendedReplicationData != null && primaryReplicationData.ReplicationMode == VirtualMachineReplicationMode.Recovery && extendedReplicationData.ReplicationState == VirtualMachineReplicationState.Disabled)
				{
					return true;
				}
				if (replicationCommand == ClusterCommandId.VirtualMachineStartRecovery || (replicationCommand == ClusterCommandId.VirtualMachineTestRecovery && !primaryReplicationData.IsTestFailoverRunning) || (replicationCommand == ClusterCommandId.VirtualMachineCancelTestFailoverReplication && primaryReplicationData.IsTestFailoverRunning))
				{
					return true;
				}
			}
			break;
		}
		return false;
	}

	private bool CalculateReplicationCommandVisibilityOnRecoveryForExtendedRelation(ClusterCommandId replicationCommand, VirtualMachineReplicationData primaryReplicationData, VirtualMachineReplicationData extendedReplicationData)
	{
		if (extendedReplicationData == null || extendedReplicationData.ReplicationState == VirtualMachineReplicationState.Disabled)
		{
			return false;
		}
		if (extendedReplicationData.IsInitialReplicationPending && extendedReplicationData.ReplicationState != VirtualMachineReplicationState.Ready && extendedReplicationData.ReplicationState != VirtualMachineReplicationState.WaitingForUpdateCompletion && replicationCommand == ClusterCommandId.VirtualMachineCancelInitialReplication)
		{
			return true;
		}
		if (replicationCommand == ClusterCommandId.VirtualMachinePauseExtendedReplication && (primaryReplicationData.ReplicationState == VirtualMachineReplicationState.Recovered || primaryReplicationData.ReplicationState == VirtualMachineReplicationState.Committed))
		{
			return false;
		}
		VirtualMachineState virtualMachineState = (this.virtualMachineState.HasValue ? this.virtualMachineState.Value : VirtualMachineState.Unknown);
		switch (extendedReplicationData.ReplicationState)
		{
		case VirtualMachineReplicationState.Ready:
			if (virtualMachineState != VirtualMachineState.Paused && (primaryReplicationData.ReplicationState == VirtualMachineReplicationState.Replicating || primaryReplicationData.ReplicationState == VirtualMachineReplicationState.Suspended) && replicationCommand == ClusterCommandId.VirtualMachineInitializeReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.WaitingToCompleteInitialReplication:
			if (replicationCommand == ClusterCommandId.VirtualMachinePauseExtendedReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Resynchronize:
			if (replicationCommand == ClusterCommandId.VirtualMachinePauseExtendedReplication || replicationCommand == ClusterCommandId.VirtualMachineCancelResynchronize)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Replicating:
			if (replicationCommand == ClusterCommandId.VirtualMachinePauseExtendedReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.Suspended:
		case VirtualMachineReplicationState.Critical:
		case VirtualMachineReplicationState.ResynchronizeSuspended:
		case VirtualMachineReplicationState.UpdateCritical:
			if (replicationCommand == ClusterCommandId.VirtualMachineResumeExtendedReplication)
			{
				return true;
			}
			break;
		case VirtualMachineReplicationState.WaitingForStartResynchronize:
			if (primaryReplicationData.ReplicationState != VirtualMachineReplicationState.ResynchronizeSuspended && primaryReplicationData.ReplicationState != VirtualMachineReplicationState.Resynchronize && replicationCommand == ClusterCommandId.VirtualMachineResumeExtendedReplication)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private bool CalculateReplicationCommandVisibility(ClusterCommandId replicationCommand)
	{
		VirtualMachineReplicationData virtualMachineReplicationData = primaryRelationship;
		VirtualMachineReplicationData extendedReplicationData = extendedRelationship;
		if (virtualMachineReplicationData == null)
		{
			return false;
		}
		VirtualMachineReplicationMode replicationMode = virtualMachineReplicationData.ReplicationMode;
		bool result = false;
		switch (replicationMode)
		{
		case VirtualMachineReplicationMode.Primary:
			result = CalculateReplicationCommandVisibilityForPrimary(replicationCommand, virtualMachineReplicationData);
			break;
		case VirtualMachineReplicationMode.Recovery:
		case VirtualMachineReplicationMode.ExtendedReplica:
			result = CalculateReplicationCommandVisibilityForRecovery(replicationCommand, virtualMachineReplicationData, extendedReplicationData);
			break;
		case VirtualMachineReplicationMode.None:
			if (replicationCommand == ClusterCommandId.VirtualMachineEnableReplication)
			{
				result = true;
			}
			break;
		}
		return result;
	}

	private bool UpdateCommandsVisibility(WeakReferenceEx commandReference, ClusterCommandId commandId)
	{
		ClusterCommand clusterCommand;
		if (commandReference == null || (clusterCommand = (ClusterCommand)commandReference.Target) == null)
		{
			return false;
		}
		bool flag = CalculateReplicationCommandVisibility(commandId);
		if (flag == clusterCommand.Visible)
		{
			return false;
		}
		clusterCommand.Visible = flag;
		return true;
	}

	private void UpdateCommandsVisibility()
	{
		bool flag = UpdateCommandsVisibility(enableReplicationCommandWeak, ClusterCommandId.VirtualMachineEnableReplication);
		flag = UpdateCommandsVisibility(initializeReplicationCommandWeak, ClusterCommandId.VirtualMachineInitializeReplication) || flag;
		flag = UpdateCommandsVisibility(importReplicationCommandWeak, ClusterCommandId.VirtualMachineImportReplication) || flag;
		flag = UpdateCommandsVisibility(reverseReplicationCommandWeak, ClusterCommandId.VirtualMachineReverseReplication) || flag;
		flag = UpdateCommandsVisibility(pauseReplicationCommandWeak, ClusterCommandId.VirtualMachinePauseReplication) || flag;
		flag = UpdateCommandsVisibility(resumeReplicationCommandWeak, ClusterCommandId.VirtualMachineResumeReplication) || flag;
		flag = UpdateCommandsVisibility(startRecoveryCommandWeak, ClusterCommandId.VirtualMachineStartRecovery) || flag;
		flag = UpdateCommandsVisibility(cancelFailoverReplicationCommandWeak, ClusterCommandId.VirtualMachineCancelFailoverReplication) || flag;
		flag = UpdateCommandsVisibility(testRecoveryCommandWeak, ClusterCommandId.VirtualMachineTestRecovery) || flag;
		flag = UpdateCommandsVisibility(cancelTestFailoverReplicationCommandWeak, ClusterCommandId.VirtualMachineCancelTestFailoverReplication) || flag;
		flag = UpdateCommandsVisibility(prepareFailoverReplicationCommandWeak, ClusterCommandId.VirtualMachinePrepareFailoverReplication) || flag;
		flag = UpdateCommandsVisibility(cancelPrepareFailoverReplicationCommandWeak, ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication) || flag;
		flag = UpdateCommandsVisibility(removeReplicationCommandWeak, ClusterCommandId.VirtualMachineRemoveReplication) || flag;
		flag = UpdateCommandsVisibility(cancelResynchronizeCommandWeak, ClusterCommandId.VirtualMachineCancelResynchronize) || flag;
		flag = UpdateCommandsVisibility(viewReplicationHealthCommandWeak, ClusterCommandId.VirtualMachineViewReplicationHealth) || flag;
		flag = UpdateCommandsVisibility(cancelInitializeReplicationCommandWeak, ClusterCommandId.VirtualMachineCancelInitialReplication) || flag;
		flag = UpdateCommandsVisibility(cancelDiskUpdateReplicationCommandWeak, ClusterCommandId.VirtualMachineCancelDiskUpdateReplication) || flag;
		flag = UpdateCommandsVisibility(commitFailoverCommandWeak, ClusterCommandId.VirtualMachineCommitFailover) || flag;
		flag = UpdateCommandsVisibility(pauseExtendedReplicationCommandWeak, ClusterCommandId.VirtualMachinePauseExtendedReplication) || flag;
		flag = UpdateCommandsVisibility(resumeExtendedReplicationCommandWeak, ClusterCommandId.VirtualMachineResumeExtendedReplication) || flag;
		if (UpdateCommandsVisibility(extendReplicationCommandWeak, ClusterCommandId.VirtualMachineExtendReplication) || flag)
		{
			RefreshCommands();
			groupCommandContainer.Target?.Signal("RefreshCommands");
		}
	}

	private static string ServiceStatusText(ServiceControllerStatus status)
	{
		return status switch
		{
			ServiceControllerStatus.ContinuePending => EnumResources.ServiceStatusText_ContinuePending, 
			ServiceControllerStatus.Paused => EnumResources.ServiceStatusText_Paused, 
			ServiceControllerStatus.PausePending => EnumResources.ServiceStatusText_PausePending, 
			ServiceControllerStatus.Running => EnumResources.ServiceStatusText_Running, 
			ServiceControllerStatus.StartPending => EnumResources.ServiceStatusText_StartPending, 
			ServiceControllerStatus.Stopped => EnumResources.ServiceStatusText_Stopped, 
			ServiceControllerStatus.StopPending => EnumResources.ServiceStatusText_StopPending, 
			_ => string.Empty, 
		};
	}

	public void Start()
	{
		Start(base.SetLastErrorIfNecessary);
	}

	public void Start(Action<OperationResult> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).Start();
		}, operationResult, LockAccess.Reader);
	}

	public void Turnoff(bool askConfirmation = false)
	{
		Turnoff(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void Turnoff(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		if (!askConfirmation || CreateConfirmationDialog(DialogResources.VirtualMachineTurnoff_Title, DialogResources.VirtualMachineTurnoff_Header.FormatCurrentCulture(DisplayName), DialogResources.VirtualMachineTurnoff_Content).ShowDialog() == TaskDialogResult.Yes)
		{
			this.ExecuteMethod(delegate(ILockable resourceObject)
			{
				((PVirtualMachineResource)resourceObject.Owner).TurnOff();
			}, operationResult, LockAccess.Reader);
		}
	}

	public void DeleteSavedState(bool askConfirmation = false)
	{
		DeleteSavedState(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void DeleteSavedState(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		if (!askConfirmation || CreateConfirmationDialog(DialogResources.VirtualMachineDeleteSavedState_Title, DialogResources.VirtualMachineDeleteSavedState_Header.FormatCurrentCulture(DisplayName)).ShowDialog() == TaskDialogResult.Yes)
		{
			ExecuteSafe(7, operationResult, delegate(ILockable resourceObject)
			{
				((PVirtualMachineResource)resourceObject.Owner).DeleteSavedState();
			}, delegate(Exception exception)
			{
				ClusterException result = new ClusterVirtualMachineDeleteStateException(base.Name, exception);
				ClusterLog.LogException(exception, "Error deleting saved state for virtual machine for '{0}'.", DisplayName);
				return result;
			});
		}
	}

	public void Pause()
	{
		Pause(base.SetLastErrorIfNecessary);
	}

	public void Pause(Action<OperationResult> operationResult)
	{
		ExecuteSafe(7, operationResult, delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).Pause();
		}, delegate(Exception exception)
		{
			ClusterException result = new ClusterVirtualMachinePauseException(base.Name, exception);
			ClusterLog.LogException(exception, "Error resuming the virtual machine for '{0}'.", DisplayName);
			return result;
		});
	}

	public void Resume()
	{
		Resume(base.SetLastErrorIfNecessary);
	}

	public void Resume(Action<OperationResult> operationResult)
	{
		ExecuteSafe(7, operationResult, delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).Resume();
		}, delegate(Exception exception)
		{
			ClusterException result = new ClusterVirtualMachineResumeException(base.Name, exception);
			ClusterLog.LogException(exception, "Error resuming the virtual machine for '{0}'.", DisplayName);
			return result;
		});
	}

	public void Reset(bool askConfirmation = false)
	{
		Reset(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void Reset(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		if (!askConfirmation || CreateConfirmationDialog(DialogResources.VirtualMachineReset_Title, DialogResources.VirtualMachineReset_Header.FormatCurrentCulture(DisplayName), DialogResources.VirtualMachineReset_Content).ShowDialog() == TaskDialogResult.Yes)
		{
			ExecuteSafe(7, operationResult, delegate(ILockable resourceObject)
			{
				((PVirtualMachineResource)resourceObject.Owner).Reset();
			}, delegate(Exception exception)
			{
				ClusterException result = new ClusterVirtualMachineResetException(base.Name, exception);
				ClusterLog.LogException(exception, "Error resuming the virtual machine for '{0}'.", DisplayName);
				return result;
			});
		}
	}

	public void Shutdown(bool askConfirmation = false)
	{
		Shutdown(base.SetLastErrorIfNecessary, askConfirmation);
	}

	public void Shutdown(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		if (!askConfirmation || CreateConfirmationDialog(DialogResources.VirtualMachineShutdown_Title, DialogResources.VirtualMachineShutdown_Header.FormatCurrentCulture(DisplayName)).ShowDialog() == TaskDialogResult.Yes)
		{
			this.ExecuteMethod(delegate(ILockable resourceObject)
			{
				((PVirtualMachineResource)resourceObject.Owner).Shutdown();
			}, operationResult, LockAccess.Reader);
		}
	}

	public void Save()
	{
		Save(base.SetLastErrorIfNecessary);
	}

	public void Save(Action<OperationResult> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).Save();
		}, operationResult, LockAccess.Reader);
	}

	public void MoveStorage(VirtualMachineStorageMoveParameters virtualMachineStorageMoveParameters)
	{
		MoveStorage(virtualMachineStorageMoveParameters, base.SetLastErrorIfNecessary);
	}

	public void MoveStorage(VirtualMachineStorageMoveParameters virtualMachineStorageMoveParameters, Action<OperationResult> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).MoveStorage(virtualMachineStorageMoveParameters);
		}, operationResult, LockAccess.Reader);
	}

	public void ApplyCheckpoint(Checkpoint checkpoint)
	{
		ApplyCheckpoint(base.SetLastErrorIfNecessary, checkpoint);
	}

	public void ApplyCheckpoint(Action<OperationResult> operationResult, Checkpoint checkpoint)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).ApplyCheckpoint(checkpoint);
		}, operationResult, LockAccess.Reader);
	}

	public void DeleteCheckpoint(Checkpoint checkpoint)
	{
		DeleteCheckpoint(base.SetLastErrorIfNecessary, checkpoint);
	}

	private void DeleteCheckpoint(Action<OperationResult> operationResult, Checkpoint checkpoint)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).DeleteCheckpoint(checkpoint);
		}, operationResult, LockAccess.Reader);
	}

	public void DeleteCheckpointTree(Checkpoint checkpoint)
	{
		DeleteCheckpointTree(base.SetLastErrorIfNecessary, checkpoint);
	}

	private void DeleteCheckpointTree(Action<OperationResult> operationResult, Checkpoint checkpoint)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).DeleteCheckpointTree(checkpoint);
		}, operationResult, LockAccess.Reader);
	}

	public void RenameCheckpoint(Checkpoint checkpoint, string newCheckpointName)
	{
		RenameCheckpoint(base.SetLastErrorIfNecessary, checkpoint, newCheckpointName);
	}

	private void RenameCheckpoint(Action<OperationResult> operationResult, Checkpoint checkpoint, string newCheckpointName)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).RenameCheckpoint(checkpoint, newCheckpointName);
		}, operationResult, LockAccess.Reader);
	}

	public Guid GetTestFailoverVirtualMachineId()
	{
		return this.ExecuteMethod((ILockable resourceObject) => ((PVirtualMachineResource)resourceObject.Owner).GetTestFailoverVirtualMachineId(), LockAccess.Reader, setErrorOnObject: false);
	}

	public string GetVirtualMachineOwnerGroup(Guid virtualMachineInstanceId)
	{
		return this.ExecuteMethod((ILockable resourceObject) => ((PVirtualMachineResource)resourceObject.Owner).GetVirtualMachineOwnerGroup(virtualMachineInstanceId), LockAccess.Reader, setErrorOnObject: false);
	}

	public void Connect()
	{
		Connect(base.SetLastErrorIfNecessary);
	}

	public void Connect(Action<OperationResult> operationResult)
	{
		LoadAsync(delegate(ClusterLoadedEventArgs loaded)
		{
			ClusterException exception = null;
			if (loaded.Error != null)
			{
				return;
			}
			try
			{
				string name = base.OwnerGroup.OwnerNode.Name;
				string text = FindVirtualMachineConnect(name);
				if (text != null)
				{
					string typedValue = ((ClusterPropertyString)Properties["vmId"]).TypedValue;
					if (!ActivateExistingVirtualMachineConnect(vmConnectCurrentProcess, name, typedValue))
					{
						vmConnectCurrentProcess = Process.Start(new ProcessStartInfo(text, string.Format(CultureInfo.InvariantCulture, "{0} -G {1}", name, typedValue)));
						UIHelper.ApplicationActivate(vmConnectCurrentProcess);
					}
				}
				else
				{
					exception = new ClusterVirtualMachineConnectNotFoundException();
				}
			}
			catch (ClusterHyperVNotSupportedException ex)
			{
				exception = ex;
				ClusterLog.LogException(exception, "Error connecting to virtual machine '{0}'.", DisplayName);
			}
			catch (Exception innerException)
			{
				exception = new ClusterVirtualMachineConnectException(base.Name, innerException);
				ClusterLog.LogException(exception, "Error connecting to virtual machine '{0}'.", DisplayName);
			}
			finally
			{
				operationResult.SafeCall(new OperationResult(this, exception));
			}
		}, ResourceLoadSelection.Basic | ResourceLoadSelection.PrivateProperties);
	}

	public void Manage()
	{
		Manage(base.SetLastErrorIfNecessary);
	}

	public void Manage(Action<OperationResult> operationResult)
	{
		LoadAsync(delegate
		{
			this.ExecuteMethod(delegate
			{
				ClusterException exception = null;
				try
				{
					string name = base.OwnerGroup.OwnerNode.Name;
					object obj = FindVirtualMachineSnapin(name);
					ClusterVirtualMachineHyperVNotFoundException exceptionToDisplay = new ClusterVirtualMachineHyperVNotFoundException();
					if (obj == null)
					{
						obj = "VirtMgmt.msc";
					}
					ProcessStartInfo processStartInfo = ProcessHelper.CreateSnapinStartInfo((string)obj, exceptionToDisplay);
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					processStartInfo.EnvironmentVariables["VM_CONFIGURATION_ID"] = clusterPropertyString.TypedValue;
					processStartInfo.EnvironmentVariables["VM_CONFIGURATION_SERVER"] = name;
					UIHelper.ApplicationActivate(Process.Start(processStartInfo));
				}
				catch (ClusterHyperVNotSupportedException ex)
				{
					exception = ex;
					ClusterLog.LogException(exception, "Error managing  virtual machine '{0}'.", DisplayName);
				}
				catch (ClusterDialogException ex2)
				{
					exception = ex2;
				}
				catch (Exception ex3)
				{
					exception = new ClusterVirtualMachineManageException(base.Name, ex3);
					ClusterLog.LogException(ex3, string.Format(CultureInfo.CurrentCulture, ExceptionResources.ErrorConnectingToVirtualMachine.FormatCurrentCulture(DisplayName)));
				}
				finally
				{
					operationResult.SafeCall(new OperationResult(this, exception));
				}
			}, LockAccess.Reader);
		});
	}

	public void Settings()
	{
		Settings(base.SetLastErrorIfNecessary);
	}

	public void Settings(Action<OperationResult> operationResult)
	{
		if (isVmSettingsLoading)
		{
			return;
		}
		lock (vmSettingsLoadingLock)
		{
			if (isVmSettingsLoading)
			{
				return;
			}
			isVmSettingsLoading = true;
			ExecuteSafeUi(delegate(OperationResult opResult)
			{
				isVmSettingsLoading = false;
				operationResult(opResult);
			}, ExceptionResources.VirtualMachineSettings_Text, delegate
			{
				if (settingDialog != null)
				{
					((IForm)settingDialog).Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					if (virtualMachineState.HasValue)
					{
						RunVirtualMachineSettings(name, clusterPropertyString.TypedValue, virtualMachineState.Value, ExceptionResources.VirtualMachineSettings_Text);
					}
				}
			}, null);
		}
	}

	public void TakeCheckpoint()
	{
		TakeCheckpoint(base.SetLastErrorIfNecessary);
	}

	public void TakeCheckpoint(Action<OperationResult> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable resourceObject)
		{
			((PVirtualMachineResource)resourceObject.Owner).TakeCheckpoint();
		}, operationResult, LockAccess.Reader);
	}

	public void RevertCheckpoint()
	{
		RevertCheckpoint(base.SetLastErrorIfNecessary);
	}

	public void RevertCheckpoint(Action<OperationResult> operationResult)
	{
		LoadAsync(delegate(ClusterLoadedEventArgs loaded)
		{
			if (loaded.Error != null)
			{
				operationResult.SafeCall(new OperationResult(this, loaded.Error));
			}
			else if (checkpointInformation == null || checkpointInformation.IsEmpty)
			{
				SetInformationInternal(ExceptionResources.ClusterVirtualMachineRevertCheckpointNotAvailableException_Header, OperationType.Async);
				ForwardMessageToOwnerGroup(ExceptionResources.ClusterVirtualMachineRevertCheckpointNotAvailableException_Header);
			}
			else if (base.ResourceState != ResourceState.Offline || base.ResourceState != ResourceState.Online)
			{
				SetInformationInternal(ExceptionResources.ClusterVirtualMachineRevertCheckpointInvalidState_Text, OperationType.Async);
				ForwardMessageToOwnerGroup(ExceptionResources.ClusterVirtualMachineRevertCheckpointInvalidState_Text);
			}
			else
			{
				bool boolValue = UserSettings.GetBoolValue("RevertCheckpointConfirmationPrompt");
				TaskDialogResult taskDialogResult = TaskDialogResult.Yes;
				if (!boolValue)
				{
					ConfirmationDialog confirmationDialog = CreateCheckpointConfirmationDialog(DialogResources.VirtualMachineRevertCheckpoint_Title, DialogResources.VirtualMachineRevertCheckpoint_Header.FormatCurrentCulture(DisplayName), ClusterCommandId.VirtualMachineResourceCheckpointRevert);
					taskDialogResult = confirmationDialog.ShowDialog();
					UserSettings.SetBoolValue("RevertCheckpointConfirmationPrompt", confirmationDialog.IsFooterChecked);
				}
				if (taskDialogResult == TaskDialogResult.Yes)
				{
					if (base.ResourceState == ResourceState.Online)
					{
						Shutdown(delegate(OperationResult shutdownOpResult)
						{
							if (shutdownOpResult.Error != null)
							{
								operationResult(new OperationResult(this, shutdownOpResult.Error));
							}
							else
							{
								this.ExecuteMethod(delegate(ILockable resourceObject)
								{
									((PVirtualMachineResource)resourceObject.Owner).RevertCheckpoint();
								}, operationResult, LockAccess.Reader);
								Start();
							}
						});
					}
					else
					{
						this.ExecuteMethod(delegate(ILockable resourceObject)
						{
							((PVirtualMachineResource)resourceObject.Owner).RevertCheckpoint();
						}, operationResult, LockAccess.Reader);
					}
				}
			}
		}, 262144);
	}

	private void ForwardMessageToOwnerGroup(string message)
	{
		ClusterInformationEventArgs payload = new ClusterInformationEventArgs(base.OwnerGroup.Id, message);
		base.Cluster.EnqueueNotification(new GroupNotification(payload));
	}

	public void Export(Checkpoint checkpoint = null)
	{
		Export(base.SetLastErrorIfNecessary, checkpoint);
	}

	public void Export(Action<OperationResult> operationResult, Checkpoint checkpoint = null)
	{
		if (isExportLoading)
		{
			return;
		}
		lock (exportLoadingLock)
		{
			if (isExportLoading)
			{
				return;
			}
			isExportLoading = true;
			ExecuteSafeUi(delegate(OperationResult opResult)
			{
				isExportLoading = false;
				operationResult(opResult);
			}, ExceptionResources.VirtualMachineExport_Text, delegate
			{
				if (exportDialog != null)
				{
					((IForm)exportDialog).Activate();
				}
				else
				{
					string virtualSystemId = ((checkpoint != null) ? checkpoint.Id.ToString() : ((ClusterPropertyString)Properties["vmId"]).TypedValue);
					string name = base.OwnerGroup.OwnerNode.Name;
					if (virtualMachineState.HasValue)
					{
						RunExport(name, virtualSystemId, virtualMachineState.Value, ExceptionResources.VirtualMachineExport_Text);
					}
				}
			}, null);
		}
	}

	private void OperateSafeReplicationForm(Action replicationFormAction)
	{
		if (Interlocked.CompareExchange(ref isReplicationFormOpeningOrClosing, 1, 0) == 0)
		{
			replicationFormAction();
		}
	}

	public void EnableReplication(Action<OperationResult> operationResult)
	{
		OperateSafeReplicationForm(delegate
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCreateReplicationRelationship_Text, delegate
			{
				if (createReplicationRelationshipWizard != null)
				{
					((IForm)createReplicationRelationshipWizard).Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					RunEnableReplicationWizard(name, clusterPropertyString.TypedValue, ExceptionResources.VirtualMachineCreateReplicationRelationship_Text);
				}
			}, delegate
			{
				Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
			});
		});
	}

	public void ViewReplicationHealth(Action<OperationResult> operationResult)
	{
		OperateSafeReplicationForm(delegate
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineViewReplicationHealth_Text, delegate
			{
				if (viewReplicationHealthForm != null)
				{
					viewReplicationHealthForm.Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					RunViewReplicationHealthForm(name, clusterPropertyString.TypedValue, ExceptionResources.VirtualMachineViewReplicationHealth_Text);
				}
			}, delegate
			{
				Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
			});
		});
	}

	public void InitializeReplication(Action<OperationResult> operationResult)
	{
		OperateSafeReplicationForm(delegate
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineStartInitalReplication_Text, delegate
			{
				if (startInitalReplicationForm != null)
				{
					startInitalReplicationForm.Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					RunInitializeReplicationForm(name, clusterPropertyString.TypedValue, ExceptionResources.VirtualMachineStartInitalReplication_Text);
				}
			}, delegate
			{
				Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
			});
		});
	}

	public void ImportReplication(Action<OperationResult> operationResult)
	{
		OperateSafeReplicationForm(delegate
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCompleteInitialReplication_Text, delegate
			{
				if (completeInitialReplicationForm != null)
				{
					completeInitialReplicationForm.Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					RunImportReplicationForm(name, clusterPropertyString.TypedValue, ExceptionResources.VirtualMachineCompleteInitialReplication_Text);
				}
			}, delegate
			{
				Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
			});
		});
	}

	public void CancelInitializeReplication(Action<OperationResult> operationResult)
	{
		bool isExtended = extendedRelationship?.IsInitialReplicationPending ?? false;
		ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCancelInitializeReplication_Text, delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
			string name = base.OwnerGroup.OwnerNode.Name;
			ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineCancelInitialReplication, isExtended);
		}, null);
	}

	public void CancelDiskUpdateReplication(Action<OperationResult> operationResult)
	{
		ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCancelDiskUpdateReplication_Text, delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
			string name = base.OwnerGroup.OwnerNode.Name;
			ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineCancelDiskUpdateReplication);
		}, null);
	}

	public void CommitFailoverReplication(Action<OperationResult> operationResult)
	{
		if (CreateReplicationActionConfirmationDialog(DialogResources.ReplicationCommitFailoverReplication_Title, DialogResources.ReplicationCommitFailoverReplication_Header.FormatCurrentCulture(DisplayName), ClusterCommandId.VirtualMachineCommitFailover, DialogResources.ReplicationCommitFailoverReplication_Content).ShowDialog() == TaskDialogResult.Yes)
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCancelInitializeReplication_Text, delegate
			{
				ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
				string name = base.OwnerGroup.OwnerNode.Name;
				ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineCommitFailover);
			}, null);
		}
	}

	public void RemoveReplication(Action<OperationResult> operationResult)
	{
		VirtualMachineReplicationData virtualMachineReplicationData = extendedRelationship;
		if (virtualMachineReplicationData != null && virtualMachineReplicationData.ReplicationState != 0)
		{
			OperateSafeReplicationForm(delegate
			{
				ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineRemoveReplicationRelationship_Text, delegate
				{
					if (removeReplicationForm != null)
					{
						removeReplicationForm.Activate();
					}
					else
					{
						ClusterPropertyString clusterPropertyString2 = (ClusterPropertyString)Properties["vmId"];
						string name2 = base.OwnerGroup.OwnerNode.Name;
						RunRemoveReplicationForm(name2, clusterPropertyString2.TypedValue, ExceptionResources.VirtualMachineRemoveReplicationRelationship_Text);
					}
				}, delegate
				{
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			});
		}
		else if (CreateReplicationActionConfirmationDialog(DialogResources.ReplicationRemoveReplication_Title, DialogResources.ReplicationRemoveReplication_Header.FormatCurrentCulture(DisplayName), ClusterCommandId.VirtualMachineRemoveReplication).ShowDialog() == TaskDialogResult.Yes)
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineRemoveReplicationRelationship_Text, delegate
			{
				ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
				string name = base.OwnerGroup.OwnerNode.Name;
				ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineRemoveReplication);
			}, null);
		}
	}

	public void PauseReplication(Action<OperationResult> operationResult, ClusterCommandId pauseCommand)
	{
		bool isExtended = pauseCommand == ClusterCommandId.VirtualMachinePauseExtendedReplication;
		ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachinePauseReplication_Text, delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
			string name = base.OwnerGroup.OwnerNode.Name;
			ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, pauseCommand, isExtended);
		}, null);
	}

	public void CancelResynchronize(Action<OperationResult> operationResult)
	{
		VirtualMachineReplicationData virtualMachineReplicationData = extendedRelationship;
		bool isExtendedInResynchronize = virtualMachineReplicationData != null && virtualMachineReplicationData.ReplicationState == VirtualMachineReplicationState.Resynchronize;
		ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCancelResynchronize_Text, delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
			string name = base.OwnerGroup.OwnerNode.Name;
			ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineCancelResynchronize, isExtendedInResynchronize);
		}, null);
	}

	public void ResumeReplication(Action<OperationResult> operationResult, VirtualMachineReplicationData primaryReplicationData, VirtualMachineReplicationData extendedReplicationData, ClusterCommandId resumeCommand)
	{
		if (primaryReplicationData == null || (resumeCommand == ClusterCommandId.VirtualMachineResumeExtendedReplication && extendedReplicationData == null))
		{
			return;
		}
		if (resumeCommand == ClusterCommandId.VirtualMachineResumeExtendedReplication && extendedReplicationData != null && extendedReplicationData.ReplicationState != 0 && (primaryReplicationData.ReplicationState == VirtualMachineReplicationState.Recovered || primaryReplicationData.ReplicationState == VirtualMachineReplicationState.Committed))
		{
			ClusterPropertyString property2 = (ClusterPropertyString)Properties["vmId"];
			string nodeName2 = base.OwnerGroup.OwnerNode.Name;
			OperateSafeReplicationForm(delegate
			{
				ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineResumeReplication_Text, delegate
				{
					if (resumeExtendedReplicationForm != null)
					{
						resumeExtendedReplicationForm.Activate();
					}
					else
					{
						RunResumeExtendedReplicationForm(nodeName2, property2.TypedValue, ExceptionResources.VirtualMachineResumeReplication_Text);
					}
				}, delegate
				{
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			});
			return;
		}
		VirtualMachineReplicationData obj = ((resumeCommand == ClusterCommandId.VirtualMachineResumeReplication) ? primaryReplicationData : extendedReplicationData) ?? throw new NullReferenceException("Could not get data. replicationData is null");
		bool flag = true;
		VirtualMachineReplicationState replicationState = obj.ReplicationState;
		if (replicationState == VirtualMachineReplicationState.WaitingForStartResynchronize && virtualMachineState == VirtualMachineState.Paused)
		{
			ClusterDialogException.ShowTaskDialogAsync(new ClusterVirtualMachineReplicationVMPausedException(ExceptionResources.VirtualMachineReplicationPausedException_Header.FormatCurrentCulture(base.Name), ExceptionResources.VirtualMachineResynchronizeReplication_VMPaused_Message, base.Name));
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		ClusterPropertyString property = (ClusterPropertyString)Properties["vmId"];
		string nodeName = base.OwnerGroup.OwnerNode.Name;
		if (replicationState == VirtualMachineReplicationState.WaitingForStartResynchronize)
		{
			OperateSafeReplicationForm(delegate
			{
				ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineResynchronizeReplication_Text, delegate
				{
					if (resynchronizeReplicationForm != null)
					{
						resynchronizeReplicationForm.Activate();
					}
					else
					{
						RunResynchronizeReplicationForm(nodeName, property.TypedValue, ExceptionResources.VirtualMachineResynchronizeReplication_Text);
					}
				}, delegate
				{
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			});
		}
		else
		{
			bool isExtended = resumeCommand == ClusterCommandId.VirtualMachineResumeExtendedReplication;
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineResumeReplication_Text, delegate
			{
				ExecuteNonUiReplicationAction(nodeName, property.TypedValue, resumeCommand, isExtended);
			}, null);
		}
	}

	public void StartRecovery(Action<OperationResult> operationResult)
	{
		OperateSafeReplicationForm(delegate
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineStartReplicationFailover_Text, delegate
			{
				if (startReplicationFailoverForm != null)
				{
					startReplicationFailoverForm.Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					RunStartRecoveryForm(name, clusterPropertyString.TypedValue, testMode: false, ExceptionResources.VirtualMachineStartReplicationFailover_Text);
				}
			}, delegate
			{
				Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
			});
		});
	}

	public void CancelFailoverReplication(Action<OperationResult> operationResult)
	{
		if (CreateReplicationActionConfirmationDialog(DialogResources.ReplicationCancelReplicationFailover_Title, DialogResources.ReplicationCancelReplicationFailover_Header.FormatCurrentCulture(DisplayName), ClusterCommandId.VirtualMachineCancelFailoverReplication, DialogResources.ReplicationCancelReplicationFailover_Content).ShowDialog() == TaskDialogResult.Yes)
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCancelReplicationFailover_Text, delegate
			{
				ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
				string name = base.OwnerGroup.OwnerNode.Name;
				ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineCancelFailoverReplication);
			}, null);
		}
	}

	public void TestRecovery(Action<OperationResult> operationResult)
	{
		OperateSafeReplicationForm(delegate
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineReplicationTestFailover_Text, delegate
			{
				if (testReplicationFailoverForm != null)
				{
					testReplicationFailoverForm.Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					RunStartRecoveryForm(name, clusterPropertyString.TypedValue, testMode: true, ExceptionResources.VirtualMachineReplicationTestFailover_Text);
				}
			}, delegate
			{
				Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
			});
		});
	}

	public void PrepareFailoverReplication(Action<OperationResult> operationResult)
	{
		OperateSafeReplicationForm(delegate
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachinePrepareFailoverReplication_Text, delegate
			{
				if (plannedFailoverForm != null)
				{
					plannedFailoverForm.Activate();
				}
				else
				{
					ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
					string name = base.OwnerGroup.OwnerNode.Name;
					RunPrepareFailoverReplicationForm(name, clusterPropertyString.TypedValue, ExceptionResources.VirtualMachinePrepareFailoverReplication_Text);
				}
			}, delegate
			{
				Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
			});
		});
	}

	public void CancelPrepareFailoverReplication(Action<OperationResult> operationResult)
	{
		if (CreateReplicationActionConfirmationDialog(DialogResources.CancelReplicationFailoverPreparation_Title, DialogResources.CancelReplicationFailoverPreparation_Header.FormatCurrentCulture(DisplayName), ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication).ShowDialog() == TaskDialogResult.Yes)
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCancelReplicationFailoverPreparation_Text, delegate
			{
				ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
				string name = base.OwnerGroup.OwnerNode.Name;
				ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication);
			}, null);
		}
	}

	public void CancelTestFailoverReplication(Action<OperationResult> operationResult)
	{
		if (CreateReplicationActionConfirmationDialog(DialogResources.CancelTestFailoverReplication_Title, DialogResources.CancelTestFailoverReplication_Header.FormatCurrentCulture(DisplayName), ClusterCommandId.VirtualMachineCancelTestFailoverReplication, DialogResources.CancelTestFailoverReplication_Content.FormatCurrentCulture(DisplayName)).ShowDialog() == TaskDialogResult.Yes)
		{
			ExecuteSafeUi(operationResult, ExceptionResources.VirtualMachineCancelTestFailoverReplication_Text, delegate
			{
				ClusterPropertyString clusterPropertyString = (ClusterPropertyString)Properties["vmId"];
				string name = base.OwnerGroup.OwnerNode.Name;
				Guid testFailoverVirtualMachineId = GetTestFailoverVirtualMachineId();
				ExecuteNonUiReplicationAction(name, clusterPropertyString.TypedValue, ClusterCommandId.VirtualMachineCancelTestFailoverReplication);
				DeleteTestFailoverVirtualMachine(testFailoverVirtualMachineId);
			}, null);
		}
	}

	public void RefreshStorageInformation()
	{
		storageInformation = null;
		LoadAsync(storageInformation, 536936448);
	}

	static VirtualMachineResource()
	{
		virtualMachineToolsLock = new object();
		EmptyCheckpointInformation = new VirtualMachineCheckpointInformation();
		ChannelServices.RegisterChannel(new IpcChannel(), ensureSecurity: false);
	}

	internal VirtualMachineResource(Cluster cluster)
		: base(cluster)
	{
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		PVirtualMachineResource pVirtualMachineResource = (PVirtualMachineResource)privateObject;
		base.TransferInternalData((PClusterObject)pVirtualMachineResource, subscribeToEvents, ignorePossibleOwners);
		guestAssignedMemory = pVirtualMachineResource.AssignedMemory;
		guestAvailableMemory = pVirtualMachineResource.AvailableMemory;
		guestMemoryDemand = pVirtualMachineResource.MemoryDemand;
		guestCpuUsage = pVirtualMachineResource.GuestCpuUsage;
		heartbeatStatus = pVirtualMachineResource.HeartbeatStatus;
		vmVersion = pVirtualMachineResource.VmVersion;
		guestUpTime = pVirtualMachineResource.GuestUpTime;
		guestCreationTime = pVirtualMachineResource.GuestCreationTime;
		guestOperatingSystem = pVirtualMachineResource.GuestOperatingSystem;
		guestComputerName = pVirtualMachineResource.GuestComputerName;
		integrationServicesVersion = pVirtualMachineResource.IntegrationServicesVersion;
		guestOsMajorVersion = pVirtualMachineResource.GuestOsMajorVersion;
		guestOsMinorVersion = pVirtualMachineResource.GuestOsMinorVersion;
		guestOsBuildNumber = pVirtualMachineResource.GuestOsBuildNumber;
		guestOsProductType = pVirtualMachineResource.GuestOsProductType;
		integrationServicesStatus = pVirtualMachineResource.IntegrationServicesStatus;
		guestNotes = pVirtualMachineResource.GuestNotes;
		guestStatus = pVirtualMachineResource.GuestStatus;
		storageInformation = pVirtualMachineResource.StorageInformation;
		checkpointInformation = pVirtualMachineResource.CheckpointInformation;
		TransferReplicationData(ref primaryRelationship, pVirtualMachineResource.PrimaryReplicationData);
		TransferReplicationData(ref extendedRelationship, pVirtualMachineResource.ExtendedReplicationData);
		virtualMachineState = VirtualMachineState.PowerOff;
		ParseProperties(pVirtualMachineResource.Properties, trackChanges: false);
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs args5 = e.EventArgument as ClusterPropertiesEventArgs;
			if (args5.Error != null)
			{
				break;
			}
			IEnumerable<string> propertiesChanged = ParseProperties(args5.Properties, trackChanges: true);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				propertiesChanged.ForEach(delegate(string propertyChanged)
				{
					OnPropertyChanged(propertyChanged);
					if (propertyChanged == "ApplicationStatus")
					{
						RaiseApplicationStatusChangedEvent(this, new ClusterApplicationStatusEventArgs(args5.Id, args5.Error));
					}
				});
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		case EventType.VirtualMachineDesktopThumbnailChanged:
		{
			ClusterResourceImageEventArgs args = e.EventArgument as ClusterResourceImageEventArgs;
			if (args.Error != null)
			{
				break;
			}
			WeakReferenceEx<ImageSource> weakReferenceEx2 = desktopThumbnailWeak;
			if (weakReferenceEx2 != null && weakReferenceEx2.Target != null)
			{
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					memberTemporaryThumbnail = args.Bitmap;
					OnPropertyChanged("GuestDesktopThumbnail");
					this.GuestDesktopImageChanged?.Invoke(this, args);
				}, OperationType.Async, queueOnDispatcher);
			}
			else
			{
				desktopThumbnailWeak = null;
				memberTemporaryThumbnail = null;
			}
			break;
		}
		case EventType.VirtualMachineGuestStatusChanged:
		{
			ClusterResourceVirtualMachineGuestStatusEventArgs args2 = e.EventArgument as ClusterResourceVirtualMachineGuestStatusEventArgs;
			if (args2.Error == null)
			{
				guestAssignedMemory = args2.GuestAssignedMemory;
				guestAvailableMemory = args2.GuestAvailableMemory;
				guestMemoryDemand = args2.GuestMemoryDemand;
				guestCpuUsage = args2.GuestCpuUsage;
				guestNotes = args2.GuestNotes;
				guestStatus = args2.GuestStatus;
				heartbeatStatus = args2.HeartbeatStatus;
				guestCreationTime = args2.GuestCreationTime;
				guestUpTime = args2.GuestUptime;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("GuestAssignedMemory");
					OnPropertyChanged("GuestAvailableMemory");
					OnPropertyChanged("GuestMemoryDemand");
					OnPropertyChanged("GuestCpuUsage");
					OnPropertyChanged("GuestUpTime");
					OnPropertyChanged("GuestCreationTime");
					OnPropertyChanged("GuestNotes");
					OnPropertyChanged("GuestStatus");
					OnPropertyChanged("HeartbeatStatus");
					this.GuestStatusChanged?.Invoke(this, args2);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.VirtualMachineGuestSummaryChanged:
		{
			ClusterResourceVirtualMachineGuestSummaryEventArgs args3 = e.EventArgument as ClusterResourceVirtualMachineGuestSummaryEventArgs;
			if (args3.Error == null)
			{
				guestOperatingSystem = args3.GuestOperatingSystem;
				guestComputerName = args3.GuestComputerName;
				integrationServicesVersion = args3.IntegrationServicesVersion;
				integrationServicesStatus = args3.IntegrationServicesStatus;
				guestOsMajorVersion = args3.GuestOsMajorVersion;
				guestOsMinorVersion = args3.GuestOsMinorVersion;
				guestOsBuildNumber = args3.GuestOsBuildNumber;
				guestOsProductType = args3.GuestOsProductType;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("GuestOperatingSystem");
					OnPropertyChanged("GuestComputerName");
					OnPropertyChanged("IntegrationServicesVersion");
					OnPropertyChanged("IntegrationServicesInformation");
					OnPropertyChanged("GuestOsProductType");
					OnPropertyChanged("GuestOsMajorVersion");
					OnPropertyChanged("GuestOsMinorVersion");
					OnPropertyChanged("GuestOsBuildNumber");
					OnPropertyChanged("VmVersion");
					this.GuestSummaryChanged?.Invoke(this, args3);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.VirtualMachineReplicationSummaryChanged:
		{
			Action onPropertyChanged2 = delegate
			{
				OnPropertyChanged("LastReplicaTime");
				OnPropertyChanged("ReplicationTaskProgress");
				OnPropertyChanged("ReplicationTaskName");
				OnPropertyChanged("ReplicationPrimaryServerFullyQualifiedDomainName");
				OnPropertyChanged("ReplicationRecoveryServerFullyQualifiedDomainName");
				OnPropertyChanged("ReplicationPrimaryConnectionPoint");
				OnPropertyChanged("ReplicationRecoveryConnectionPoint");
			};
			ProcessReplicationSummaryChanged(e, onPropertyChanged2, queueOnDispatcher);
			break;
		}
		case EventType.VirtualMachineExtendedReplicationSummaryChanged:
		{
			Action onPropertyChanged = delegate
			{
				OnPropertyChanged("ExtendedLastReplicaTime");
				OnPropertyChanged("ExtendedReplicationTaskProgress");
				OnPropertyChanged("ExtendedReplicationTaskName");
				OnPropertyChanged("ExtendedReplicationRecoveryServerFullyQualifiedDomainName");
				OnPropertyChanged("ExtendedReplicationRecoveryConnectionPoint");
			};
			ProcessReplicationSummaryChanged(e, onPropertyChanged, queueOnDispatcher);
			break;
		}
		case EventType.ResourceStateChanged:
			UpdateCommandsVisibility();
			OnPropertyChanged("GuestDesktopThumbnail");
			break;
		case EventType.VirtualMachineStorageSummaryChanged:
		{
			ClusterResourceVirtualMachineStorageSummaryEventArgs args4 = e.EventArgument as ClusterResourceVirtualMachineStorageSummaryEventArgs;
			if (args4.Error == null)
			{
				storageInformation = args4.StorageSummaryInformation;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("StorageInformation");
					this.StorageInformationChanged?.Invoke(this, args4);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.VirtualMachineCheckpointSummaryChanged:
		{
			ClusterResourceVirtualMachineCheckpointSummaryEventArgs clusterResourceVirtualMachineCheckpointSummaryEventArgs = e.EventArgument as ClusterResourceVirtualMachineCheckpointSummaryEventArgs;
			if (checkpointInformation == clusterResourceVirtualMachineCheckpointSummaryEventArgs.CheckpointSummaryInformation && clusterResourceVirtualMachineCheckpointSummaryEventArgs.Error == null)
			{
				return true;
			}
			AreCheckpointsLoaded = true;
			WeakReferenceEx<VirtualMachineCheckpointInformation> weakReferenceEx = checkpointInformationWeak;
			if (weakReferenceEx != null && weakReferenceEx.Target != null)
			{
				temporaryCheckpointInformation = clusterResourceVirtualMachineCheckpointSummaryEventArgs.CheckpointSummaryInformation;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("Checkpoints");
					OnPropertyChanged("AreCheckpointsLoaded");
				}, OperationType.Async, queueOnDispatcher);
				return true;
			}
			checkpointInformationWeak = null;
			temporaryCheckpointInformation = null;
			return false;
		}
		case EventType.VMServiceChanged:
			vmServices = null;
			vmMonitoredServices = null;
			UIHelper.ExecuteOnDispatcher(delegate
			{
				OnPropertyChanged("VMServices");
				OnPropertyChanged("VMMonitoredServices");
			}, OperationType.Async);
			break;
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private void ProcessReplicationSummaryChanged(ClusterWrapperEventArgs e, Action onPropertyChanged, Queue<Action> queueOnDispatcher)
	{
		ClusterResourceVirtualMachineReplicationEventArgs args = e.EventArgument as ClusterResourceVirtualMachineReplicationEventArgs;
		if (args.Error == null)
		{
			TransferReplicationData(ref primaryRelationship, args.ReplicationData);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				onPropertyChanged();
				this.VirtualMachineReplicationChanged?.Invoke(this, args);
			}, OperationType.Async, queueOnDispatcher);
		}
	}

	private void TransferReplicationData(ref VirtualMachineReplicationData replicationData, VirtualMachineReplicationData inputData)
	{
		if (replicationData == null)
		{
			replicationData = new VirtualMachineReplicationData();
		}
		if (inputData != null)
		{
			replicationData.PrimaryServerName = inputData.PrimaryServerName;
			replicationData.RecoveryServerName = inputData.RecoveryServerName;
			replicationData.PrimaryConnectionPoint = inputData.PrimaryConnectionPoint;
			replicationData.RecoveryConnectionPoint = inputData.RecoveryConnectionPoint;
			replicationData.ReplicationTaskProgress = inputData.ReplicationTaskProgress;
			replicationData.ReplicationTaskName = inputData.ReplicationTaskName;
			replicationData.LastReplicaTime = inputData.LastReplicaTime;
			replicationData.RelationshipType = inputData.RelationshipType;
		}
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		bool flag = false;
		if (ParseProperty(properties, "VmState", ref virtualMachineState, list))
		{
			flag = true;
			list.TryAdd("ApplicationStatus");
			list.TryAdd("ResourceState");
		}
		ClusterPropertyULong clusterPropertyULong = (ClusterPropertyULong)properties["ResourceSpecificData1"];
		if (clusterPropertyULong != null)
		{
			VirtualMachineComputerSystemOperationalStatus migrationStatus = WmiVMUtilities.GetMigrationStatus(clusterPropertyULong);
			int num = WmiVMUtilities.GetMigrationProgress(clusterPropertyULong);
			if (migrationState != migrationStatus)
			{
				migrationState = migrationStatus;
				list.TryAdd("MigrationState");
			}
			if (migrationProgress != num)
			{
				migrationProgress = num;
				list.TryAdd("MigrationProgress");
			}
		}
		ClusterPropertyULong clusterPropertyULong2 = (ClusterPropertyULong)properties["ResourceSpecificData2"];
		if (clusterPropertyULong2 != null)
		{
			ulong typedValue = clusterPropertyULong2.TypedValue;
			if (ParseReplicationInformation(typedValue))
			{
				flag = true;
			}
		}
		if (flag && trackChanges)
		{
			UpdateCommandsVisibility();
			list.TryAdd("GuestDesktopThumbnail");
		}
		return list;
	}

	private bool ParseReplicationInformation(ulong data)
	{
		bool result = false;
		VirtualMachineReplicationData virtualMachineReplicationData = primaryRelationship;
		VirtualMachineReplicationData virtualMachineReplicationData2 = extendedRelationship;
		VirtualMachineReplicationVersion virtualMachineReplicationVersion = (VirtualMachineReplicationVersion)((data >> 30) & 3);
		if (version != virtualMachineReplicationVersion)
		{
			version = virtualMachineReplicationVersion;
		}
		if (virtualMachineReplicationData != null)
		{
			VirtualMachineReplicationMode virtualMachineReplicationMode = (VirtualMachineReplicationMode)(data & 0xFF);
			if (virtualMachineReplicationData.ReplicationMode != virtualMachineReplicationMode)
			{
				virtualMachineReplicationData.ReplicationMode = virtualMachineReplicationMode;
				OnPropertyChanged("ReplicationMode");
				result = true;
			}
			VirtualMachineReplicationState virtualMachineReplicationState = (VirtualMachineReplicationState)((data >> 32) & 0xFF);
			if (virtualMachineReplicationData.ReplicationState != virtualMachineReplicationState)
			{
				virtualMachineReplicationData.ReplicationState = virtualMachineReplicationState;
				OnPropertyChanged("ReplicationState");
				result = true;
			}
			VirtualMachineReplicationHealth virtualMachineReplicationHealth = (VirtualMachineReplicationHealth)((data >> 58) & 3);
			if (virtualMachineReplicationData.ReplicationHealth != virtualMachineReplicationHealth)
			{
				virtualMachineReplicationData.ReplicationHealth = virtualMachineReplicationHealth;
				OnPropertyChanged("ReplicationHealth");
				result = true;
			}
			ulong num = data >> 60;
			bool flag = (num & 8) == 8;
			if (virtualMachineReplicationData.IsTestFailoverRunning != flag)
			{
				virtualMachineReplicationData.IsTestFailoverRunning = flag;
				result = true;
			}
			bool flag2 = (num & 4) == 4;
			if (virtualMachineReplicationData.IsPlannedFailover != flag2)
			{
				virtualMachineReplicationData.IsPlannedFailover = flag2;
				result = true;
			}
			bool flag3 = (num & 2) == 2;
			if (virtualMachineReplicationData.IsInitialReplicationPending != flag3)
			{
				virtualMachineReplicationData.IsInitialReplicationPending = flag3;
				result = true;
			}
			bool flag4 = (num & 1) == 1;
			if (virtualMachineReplicationData.IsEndpointProviderUsed != flag4)
			{
				virtualMachineReplicationData.IsEndpointProviderUsed = flag4;
				result = true;
			}
		}
		if (virtualMachineReplicationData2 != null)
		{
			VirtualMachineReplicationMode virtualMachineReplicationMode2 = (VirtualMachineReplicationMode)(data & 0xFF);
			if (virtualMachineReplicationData2.ReplicationMode != virtualMachineReplicationMode2)
			{
				virtualMachineReplicationData2.ReplicationMode = virtualMachineReplicationMode2;
				OnPropertyChanged("ExtendedReplicationMode");
				result = true;
			}
			VirtualMachineReplicationState virtualMachineReplicationState2 = (VirtualMachineReplicationState)((data >> 40) & 0xFF);
			if (virtualMachineReplicationData2.ReplicationState != virtualMachineReplicationState2)
			{
				virtualMachineReplicationData2.ReplicationState = virtualMachineReplicationState2;
				OnPropertyChanged("ExtendedReplicationState");
				result = true;
			}
			VirtualMachineReplicationHealth virtualMachineReplicationHealth2 = (VirtualMachineReplicationHealth)((data >> 50) & 3);
			if (virtualMachineReplicationData2.ReplicationHealth != virtualMachineReplicationHealth2)
			{
				virtualMachineReplicationData2.ReplicationHealth = virtualMachineReplicationHealth2;
				OnPropertyChanged("ExtendedReplicationHealth");
				result = true;
			}
			ulong num2 = data >> 52;
			bool flag5 = (num2 & 8) == 8;
			if (virtualMachineReplicationData2.IsTestFailoverRunning != flag5)
			{
				virtualMachineReplicationData2.IsTestFailoverRunning = flag5;
				result = true;
			}
			bool flag6 = (num2 & 4) == 4;
			if (virtualMachineReplicationData2.IsPlannedFailover != flag6)
			{
				virtualMachineReplicationData2.IsPlannedFailover = flag6;
				result = true;
			}
			bool flag7 = (num2 & 2) == 2;
			if (virtualMachineReplicationData2.IsInitialReplicationPending != flag7)
			{
				virtualMachineReplicationData2.IsInitialReplicationPending = flag7;
				result = true;
			}
			bool flag8 = (num2 & 1) == 1;
			if (virtualMachineReplicationData2.IsEndpointProviderUsed != flag8)
			{
				virtualMachineReplicationData2.IsEndpointProviderUsed = flag8;
				result = true;
			}
		}
		return result;
	}

	private static VmState ConvertVirtualMachineStateToVmState(VirtualMachineState virtualMachineState)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		switch (virtualMachineState)
		{
		case VirtualMachineState.Running:
			return (VmState)2;
		case VirtualMachineState.PowerOff:
			return (VmState)3;
		case VirtualMachineState.ShuttingDown:
		case VirtualMachineState.Stopping:
			return (VmState)4;
		case VirtualMachineState.Saved:
			return (VmState)6;
		case VirtualMachineState.Paused:
			return (VmState)9;
		case VirtualMachineState.Starting:
			return (VmState)10;
		case VirtualMachineState.Reset:
			return (VmState)11;
		case VirtualMachineState.Saving:
			return (VmState)32773;
		case VirtualMachineState.Pausing:
			return (VmState)32776;
		case VirtualMachineState.Resuming:
			return (VmState)32777;
		default:
			return (VmState)1;
		}
	}

	private void RunVirtualMachineSettings(string server, string vmId, VirtualMachineState vmState, string exceptionMessage)
	{
		string vmSettingsInstanceId = "Microsoft:{0}".FormatCurrentCulture(vmId);
		if (vmState == VirtualMachineState.Unknown)
		{
			vmState = VirtualMachineState.Running;
		}
		IServer vmServer = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(vmServer, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			try
			{
				VmState val = ConvertVirtualMachineStateToVmState(vmState);
				settingDialog = HyperVObjectFactory.GetVmSettingsDialog(vmServer, vmSettingsInstanceId, val, (HyperVAssemblyVersion)0);
			}
			catch (VirtualizationException val2)
			{
				VirtualizationException val3 = val2;
				ClusterLog.LogException((Exception)(object)val3, "An error occurred running the VM Settings dialog");
				ClusterDialogException.ShowTaskDialog(new ClusterVirtualMachineSettingsDialogException(ExceptionResources.VirtualMachineSettings_Text, base.Name, (Exception)(object)val3));
				return;
			}
			bool settingsChanged = false;
			settingDialog.SettingsChanged += delegate
			{
				settingsChanged = true;
			};
			((IForm)settingDialog).Closed += delegate
			{
				Thread.Sleep(100);
				if (settingsChanged)
				{
					LoadAsync(delegate(ClusterLoadedEventArgs loadOpResult)
					{
						if (loadOpResult.Error != null)
						{
							ClusterLog.LogException(loadOpResult.Error);
							return;
						}
						foreach (Guid dependency in base.Dependencies)
						{
							Resource.Get(base.Cluster, dependency, delegate(OperationResult<Resource> operationGetResult)
							{
								if (operationGetResult.Error != null)
								{
									ClusterLog.LogException(operationGetResult.Error);
								}
								else
								{
									VirtualMachineConfigurationResource virtualMachineConfigurationResource = operationGetResult.Result as VirtualMachineConfigurationResource;
									if (virtualMachineConfigurationResource != null)
									{
										virtualMachineConfigurationResource.RefreshSettings(delegate(OperationResult operationResult)
										{
											if (operationResult.Error != null)
											{
												virtualMachineConfigurationResource.Error = operationResult.Error;
												base.OwnerGroup.Error = operationResult.Error;
											}
										});
									}
								}
							}, OperationType.Sync);
						}
					}, ResourceLoadSelection.Dependencies);
				}
				((IDisposable)settingDialog).Dispose();
				settingDialog = null;
			};
			((IForm)settingDialog).ShowInTaskbar = true;
			((IForm)settingDialog).Show(Global.DefaultWindow);
		}, exceptionMessage);
	}

	private void RunExport(string server, string virtualSystemId, VirtualMachineState vmState, string exceptionMessage)
	{
		string vmSettingsInstanceId = "Microsoft:{0}".FormatCurrentCulture(virtualSystemId);
		if (vmState == VirtualMachineState.Unknown)
		{
			vmState = VirtualMachineState.Running;
		}
		IServer vmServer = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(vmServer, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			try
			{
				VmState val = ConvertVirtualMachineStateToVmState(vmState);
				exportDialog = HyperVObjectFactory.GetVmSettingsDialog(vmServer, vmSettingsInstanceId, val, (HyperVAssemblyVersion)0);
			}
			catch (VirtualizationException val2)
			{
				VirtualizationException val3 = val2;
				ClusterLog.LogException((Exception)(object)val3, "An error occurred running the VM Settings dialog");
				ClusterDialogException.ShowTaskDialog(new ClusterVirtualMachineSettingsDialogException(ExceptionResources.VirtualMachineSettings_Text, base.Name, (Exception)(object)val3));
				return;
			}
			bool exportChanged = false;
			exportDialog.SettingsChanged += delegate
			{
				exportChanged = true;
			};
			((IForm)exportDialog).Closed += delegate
			{
				Thread.Sleep(100);
				if (exportChanged)
				{
					LoadAsync(delegate(ClusterLoadedEventArgs loadOpResult)
					{
						if (loadOpResult.Error != null)
						{
							ClusterLog.LogException(loadOpResult.Error);
							return;
						}
						foreach (Guid dependency in base.Dependencies)
						{
							Resource.Get(base.Cluster, dependency, delegate(OperationResult<Resource> operationGetResult)
							{
								if (operationGetResult.Error != null)
								{
									ClusterLog.LogException(operationGetResult.Error);
								}
								else
								{
									VirtualMachineConfigurationResource virtualMachineConfigurationResource = operationGetResult.Result as VirtualMachineConfigurationResource;
									if (virtualMachineConfigurationResource != null)
									{
										virtualMachineConfigurationResource.RefreshSettings(delegate(OperationResult operationResult)
										{
											if (operationResult.Error != null)
											{
												virtualMachineConfigurationResource.Error = operationResult.Error;
												base.OwnerGroup.Error = operationResult.Error;
											}
										});
									}
								}
							}, OperationType.Sync);
						}
					}, ResourceLoadSelection.Dependencies);
				}
				((IDisposable)exportDialog).Dispose();
				exportDialog = null;
			};
			((IForm)exportDialog).ShowInTaskbar = true;
			((IForm)exportDialog).Show(Global.DefaultWindow);
		}, exceptionMessage);
	}

	private void RunEnableReplicationWizard(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			createReplicationRelationshipWizard = HyperVObjectFactory.GetReplicationWizard(serverObj, vmId, (HyperVAssemblyVersion)0);
			((IForm)createReplicationRelationshipWizard).Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)createReplicationRelationshipWizard).Dispose();
					createReplicationRelationshipWizard = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			createReplicationRelationshipWizard.StartModeless(Global.DefaultWindow);
		}, exceptionMessage);
	}

	private void RunPrepareFailoverReplicationForm(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			plannedFailoverForm = HyperVObjectFactory.GetPlannedFailoverForm(serverObj, vmId, (HyperVAssemblyVersion)0);
			plannedFailoverForm.Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)plannedFailoverForm).Dispose();
					plannedFailoverForm = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			((IControl)plannedFailoverForm).Show();
		}, exceptionMessage);
	}

	private void RunViewReplicationHealthForm(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			viewReplicationHealthForm = HyperVObjectFactory.GetReplicationMonitoringForm(serverObj, vmId, (HyperVAssemblyVersion)0);
			viewReplicationHealthForm.Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)viewReplicationHealthForm).Dispose();
					viewReplicationHealthForm = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			((IControl)viewReplicationHealthForm).Show();
		}, exceptionMessage);
	}

	private void RunInitializeReplicationForm(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			startInitalReplicationForm = HyperVObjectFactory.GetInitializeReplicationForm(serverObj, vmId, (HyperVAssemblyVersion)0);
			startInitalReplicationForm.Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)startInitalReplicationForm).Dispose();
					startInitalReplicationForm = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			((IControl)startInitalReplicationForm).Show();
		}, exceptionMessage);
	}

	private void RunImportReplicationForm(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			completeInitialReplicationForm = HyperVObjectFactory.GetImportReplicationForm(serverObj, vmId, (HyperVAssemblyVersion)0);
			completeInitialReplicationForm.Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)completeInitialReplicationForm).Dispose();
					completeInitialReplicationForm = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			((IControl)completeInitialReplicationForm).Show();
		}, exceptionMessage);
	}

	private void RunResumeExtendedReplicationForm(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		ExecutePrivateVmActionSingleSelect(delegate
		{
			resumeExtendedReplicationForm = HyperVObjectFactory.GetResumeExtendedReplicationForm(serverObj, vmId, (HyperVAssemblyVersion)0);
			resumeExtendedReplicationForm.Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)resumeExtendedReplicationForm).Dispose();
					resumeExtendedReplicationForm = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			((IControl)resumeExtendedReplicationForm).Show();
		}, exceptionMessage);
	}

	private void RunResynchronizeReplicationForm(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			resynchronizeReplicationForm = HyperVObjectFactory.GetResynchronizeReplicationForm(serverObj, vmId, (HyperVAssemblyVersion)0);
			resynchronizeReplicationForm.Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)resynchronizeReplicationForm).Dispose();
					resynchronizeReplicationForm = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			((IControl)resynchronizeReplicationForm).Show();
		}, exceptionMessage);
	}

	private void RunRemoveReplicationForm(string server, string vmId, string exceptionMessage)
	{
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			removeReplicationForm = HyperVObjectFactory.GetRemoveReplicationForm(serverObj, vmId, (HyperVAssemblyVersion)0);
			removeReplicationForm.Closed += delegate
			{
				OperateSafeReplicationForm(delegate
				{
					((IDisposable)removeReplicationForm).Dispose();
					removeReplicationForm = null;
					Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
				});
			};
			((IControl)removeReplicationForm).Show();
		}, exceptionMessage);
	}

	private void RunStartRecoveryForm(string server, string vmId, bool testMode, string exceptionMessage)
	{
		VirtualMachineReplicationData primaryRelationshipTemp = primaryRelationship;
		if (primaryRelationshipTemp == null)
		{
			return;
		}
		IServer serverObj = HyperVObjectFactory.GetServer(server);
		if (!UIHelper.AssertHyperVToolsSupport(serverObj, (HyperVComponent)0))
		{
			return;
		}
		ExecutePrivateVmActionSingleSelect(delegate
		{
			if (primaryRelationshipTemp.IsPlannedFailover && !testMode)
			{
				startReplicationFailoverForm = HyperVObjectFactory.GetPlannedFailoverForm(serverObj, vmId, (HyperVAssemblyVersion)0);
				startReplicationFailoverForm.Closed += delegate
				{
					OperateSafeReplicationForm(delegate
					{
						((IDisposable)startReplicationFailoverForm).Dispose();
						startReplicationFailoverForm = null;
						Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
					});
				};
				((IControl)startReplicationFailoverForm).Show();
			}
			else if (testMode)
			{
				testReplicationFailoverForm = HyperVObjectFactory.GetFailoverForm(serverObj, vmId, true, (HyperVAssemblyVersion)0);
				testReplicationFailoverForm.Closed += delegate
				{
					OperateSafeReplicationForm(delegate
					{
						((IDisposable)testReplicationFailoverForm).Dispose();
						testReplicationFailoverForm = null;
						Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
						try
						{
							Guid testFailoverVirtualMachineId = GetTestFailoverVirtualMachineId();
							if (testFailoverVirtualMachineId != Guid.Empty)
							{
								base.Cluster.AddVirtualMachine(testFailoverVirtualMachineId, base.OwnerGroup.OwnerNode.Name);
							}
						}
						catch (Exception exception)
						{
							ClusterLog.LogException(exception, "Failed to add test VM to cluster.");
						}
					});
				};
				((IControl)testReplicationFailoverForm).Show();
			}
			else
			{
				startReplicationFailoverForm = HyperVObjectFactory.GetFailoverForm(serverObj, vmId, false, (HyperVAssemblyVersion)0);
				Guid testFailoverVmId = GetTestFailoverVirtualMachineId();
				startReplicationFailoverForm.Closed += delegate
				{
					OperateSafeReplicationForm(delegate
					{
						((IDisposable)startReplicationFailoverForm).Dispose();
						startReplicationFailoverForm = null;
						Interlocked.Exchange(ref isReplicationFormOpeningOrClosing, 0);
					});
					bool flag = GetTestFailoverVirtualMachineId() != Guid.Empty;
					if (testFailoverVmId != Guid.Empty && !flag)
					{
						DeleteTestFailoverVirtualMachine(testFailoverVmId);
					}
				};
				((IControl)startReplicationFailoverForm).Show();
			}
		}, exceptionMessage);
	}

	private void ProcessPerformerResult(IOperationResult result, ClusterCommandId replicationCommand)
	{
		if (!result.IsSucceeded)
		{
			ClusterDialogException.ShowTaskDialogAsync(new ClusterVirtualMachineBizLogicWrapperException(result.Message, DisplayName, result.Error)
			{
				Header = GetReplicationActionErrorHeader(replicationCommand).FormatCurrentCulture(DisplayName),
				Details = result.Details
			});
		}
	}

	private string GetReplicationActionErrorHeader(ClusterCommandId replicationAction)
	{
		return replicationAction switch
		{
			ClusterCommandId.VirtualMachineCancelInitialReplication => ExceptionResources.VirtualMachineStopInitializeReplicationFailed_Header, 
			ClusterCommandId.VirtualMachineCancelResynchronize => ExceptionResources.VirtualMachineStopResynchronizeReplicationFailed_Header, 
			ClusterCommandId.VirtualMachineRemoveReplication => ExceptionResources.VirtualMachineRemoveReplicationFailed_Header, 
			ClusterCommandId.VirtualMachineCancelFailoverReplication => ExceptionResources.VirtualMachineCancelFailoverFailed_Header, 
			ClusterCommandId.VirtualMachineCommitFailover => ExceptionResources.VirtualMachineCommitFailoverFailed_Header, 
			ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication => ExceptionResources.VirtualMachineCancelPrepareFailoverFailed_Header, 
			ClusterCommandId.VirtualMachineCancelTestFailoverReplication => ExceptionResources.VirtualMachineCancelTestFailoverFailed_Header, 
			ClusterCommandId.VirtualMachinePauseReplication => ExceptionResources.VirtualMachinePauseReplicationFailed_Header, 
			ClusterCommandId.VirtualMachineResumeReplication => ExceptionResources.VirtualMachineResumeReplicationFailed_Header, 
			_ => ExceptionResources.VirtualMachineBizLogicWrapper_Header, 
		};
	}

	private static string FindVirtualMachineConnect(string serverName)
	{
		if (!IsVirtualMachineToolsInstalled)
		{
			ClusterLog.LogInfo("The Hyper-V client management tools are not installed.");
			return null;
		}
		IServer server = HyperVObjectFactory.GetServer(serverName);
		UIHelper.VerifyHyperVToolsSupport(server, (HyperVComponent)0, errorAsDialog: false);
		string vmConnectFilePath = HyperVObjectFactory.GetVmConnectFilePath(server, (HyperVAssemblyVersion)0);
		if (!File.Exists(vmConnectFilePath))
		{
			return null;
		}
		return vmConnectFilePath;
	}

	private static string FindVirtualMachineSnapin(string serverName)
	{
		if (!IsVirtualMachineToolsInstalled)
		{
			ClusterLog.LogInfo("The Hyper-V client management tools are not installed.");
			return null;
		}
		UIHelper.VerifyHyperVToolsSupport(HyperVObjectFactory.GetServer(serverName), (HyperVComponent)0, errorAsDialog: false);
		string text = Path.Combine(Environment.SystemDirectory, "VirtMgmt.msc");
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	private Icon2 GetReplicationActionConfirmationIcon(ClusterCommandId command)
	{
		if (command == ClusterCommandId.VirtualMachineCancelTestFailoverReplication)
		{
			return ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.EmptyIcon));
		}
		return ReturnInstance(ref warningIcon, () => new Icon2(InvariantResources.Warning));
	}

	private ConfirmationDialog CreateReplicationActionConfirmationDialog(string title, string header, ClusterCommandId command, string content = null)
	{
		return new ConfirmationDialog
		{
			CustomIcon = GetReplicationActionConfirmationIcon(command).NativeIcon,
			Caption = title,
			Header = header,
			Content = (content ?? string.Empty)
		};
	}

	private Icon2 GetCheckpointActionConfirmationIcon(ClusterCommandId command)
	{
		return ReturnInstance(ref warningIcon, () => new Icon2(InvariantResources.Warning));
	}

	private ConfirmationDialog CreateCheckpointConfirmationDialog(string title, string header, ClusterCommandId command, string content = null)
	{
		return new ConfirmationDialog
		{
			CustomIcon = GetCheckpointActionConfirmationIcon(command).NativeIcon,
			Caption = title,
			Header = header,
			Footer = DialogResources.DoNotAskAgain_Footer,
			Content = (content ?? string.Empty)
		};
	}

	private void ExecuteSafeUi(Action<OperationResult> operationResult, string exceptionMessage, Action action, Action onComplete)
	{
		try
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (!IsVirtualMachineToolsInstalled)
			{
				operationResult.SafeCall(new OperationResult(this, new ClusterVirtualMachineHyperVNotFoundException()));
				return;
			}
			LoadAsync(delegate
			{
				this.ExecuteMethod(delegate
				{
					//IL_0010: Expected O, but got Unknown
					ClusterException ex = null;
					try
					{
						action();
					}
					catch (VirtualizationException val)
					{
						VirtualizationException innerException = val;
						ex = new ClusterVirtualMachineManageException(exceptionMessage, base.Name, (Exception)(object)innerException);
					}
					catch (TargetInvocationException ex2)
					{
						ex = new ClusterVirtualMachineManageException(exceptionMessage, base.Name, ex2.InnerException);
					}
					catch (ClusterException ex3)
					{
						ex = ex3;
					}
					finally
					{
						operationResult.SafeCall(new OperationResult(this, ex));
					}
				}, LockAccess.Reader);
			});
		}
		finally
		{
			onComplete?.Invoke();
		}
	}

	private void DeleteTestFailoverVirtualMachine(Guid testFailoverVmId)
	{
		try
		{
			if (!(testFailoverVmId != Guid.Empty))
			{
				return;
			}
			string virtualMachineOwnerGroup = GetVirtualMachineOwnerGroup(testFailoverVmId);
			if (!(virtualMachineOwnerGroup != string.Empty))
			{
				return;
			}
			Group.Get(base.Cluster, virtualMachineOwnerGroup, delegate(OperationResult<Group> groupOpGet)
			{
				if (groupOpGet.Error == null)
				{
					groupOpGet.Result.Delete();
				}
			}, OperationType.Async);
		}
		catch (ClusterException exception)
		{
			ClusterLog.LogException(exception, "Failed to remove test VM resource from cluster.");
		}
	}

	private void ExecuteNonUiReplicationAction(string server, string vmId, ClusterCommandId replicationCommand, bool isExtended = false)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		IServer server2 = HyperVObjectFactory.GetServer(server);
		if (UIHelper.AssertHyperVToolsSupport(server2, (HyperVComponent)0))
		{
			IFailoverReplicationNonUIActionPerformer failoverReplicationNonUIActionPerformer = HyperVObjectFactory.GetFailoverReplicationNonUIActionPerformer(server2, vmId, (HyperVAssemblyVersion)0);
			IOperationResult result = null;
			switch (replicationCommand)
			{
			case ClusterCommandId.VirtualMachineCancelTestFailoverReplication:
				result = failoverReplicationNonUIActionPerformer.FRActionStopTestFailoverReplication();
				break;
			case ClusterCommandId.VirtualMachineCancelPrepareFailoverReplication:
				result = failoverReplicationNonUIActionPerformer.FRActionChangeReplicationState((VmReplicationState)3, isExtended);
				break;
			case ClusterCommandId.VirtualMachineResumeReplication:
			case ClusterCommandId.VirtualMachineResumeExtendedReplication:
			{
				VirtualMachineReplicationData virtualMachineReplicationData2 = (isExtended ? extendedRelationship : primaryRelationship);
				VmReplicationState val2 = ((virtualMachineReplicationData2 == null || virtualMachineReplicationData2.ReplicationState != VirtualMachineReplicationState.UpdateCritical) ? ((VmReplicationState)3) : ((VmReplicationState)15));
				result = failoverReplicationNonUIActionPerformer.FRActionChangeReplicationState(val2, isExtended);
				break;
			}
			case ClusterCommandId.VirtualMachineCancelResynchronize:
				result = failoverReplicationNonUIActionPerformer.FRActionChangeReplicationState((VmReplicationState)9, isExtended);
				break;
			case ClusterCommandId.VirtualMachineCancelInitialReplication:
				result = failoverReplicationNonUIActionPerformer.FRActionChangeReplicationState((VmReplicationState)1, isExtended);
				break;
			case ClusterCommandId.VirtualMachineCancelDiskUpdateReplication:
			{
				VirtualMachineReplicationData virtualMachineReplicationData = primaryRelationship;
				VmReplicationState val = ((virtualMachineReplicationData == null || !virtualMachineReplicationData.IsInitialReplicationPending) ? ((VmReplicationState)3) : ((VmReplicationState)1));
				result = failoverReplicationNonUIActionPerformer.FRActionChangeReplicationState(val, isExtended);
				break;
			}
			case ClusterCommandId.VirtualMachinePauseReplication:
			case ClusterCommandId.VirtualMachinePauseExtendedReplication:
				result = failoverReplicationNonUIActionPerformer.FRActionChangeReplicationState((VmReplicationState)7, isExtended);
				break;
			case ClusterCommandId.VirtualMachineCommitFailover:
				result = failoverReplicationNonUIActionPerformer.FRActionCommitReplication();
				break;
			case ClusterCommandId.VirtualMachineCancelFailoverReplication:
				result = failoverReplicationNonUIActionPerformer.FRActionCancelFailoverReplication();
				break;
			case ClusterCommandId.VirtualMachineRemoveReplication:
				result = failoverReplicationNonUIActionPerformer.FRActionRemoveVMReplication();
				break;
			}
			ProcessPerformerResult(result, replicationCommand);
		}
	}

	private ImageSource GetThumbnail()
	{
		ImageSource imageSource;
		if (memberTemporaryThumbnail != null)
		{
			IntPtr hbitmap = memberTemporaryThumbnail.GetHbitmap();
			try
			{
				imageSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			catch (Win32Exception)
			{
				imageSource = null;
			}
			finally
			{
				NativeMethods.DeleteObject(hbitmap);
			}
			desktopThumbnailWeak = new WeakReferenceEx<ImageSource>(imageSource);
			memberTemporaryThumbnail = null;
		}
		else
		{
			imageSource = WeakReferenceEx.ReturnInstance(ref desktopThumbnailWeak, delegate
			{
				BeginLoadResourceProperties(this);
				IntPtr hbitmap2 = InvariantResources.VirtualMachineDesktopThumbnail.GetHbitmap();
				try
				{
					return Imaging.CreateBitmapSourceFromHBitmap(hbitmap2, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				}
				catch (Win32Exception)
				{
					return null;
				}
				finally
				{
					NativeMethods.DeleteObject(hbitmap2);
				}
			});
		}
		return imageSource;
	}

	private void EnqueueRefreshOperation()
	{
		bool flag = false;
		lock (base.LockObject)
		{
			if (virtualMachineQueueReload == null)
			{
				virtualMachineQueueReload = new Queue<VirtualMachineResource>();
				flag = true;
			}
			if (!virtualMachineQueueReload.Contains(this))
			{
				virtualMachineQueueReload.Enqueue(this);
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
				VirtualMachineResource virtualMachineResource;
				lock (base.LockObject)
				{
					if (virtualMachineQueueReload == null || virtualMachineQueueReload.Count == 0)
					{
						virtualMachineQueueReload = null;
						break;
					}
					virtualMachineResource = virtualMachineQueueReload.Dequeue();
				}
				WeakReferenceEx<ImageSource> weakReferenceEx = virtualMachineResource.desktopThumbnailWeak;
				if (weakReferenceEx != null && weakReferenceEx.Target != null)
				{
					BeginLoadResourceProperties(virtualMachineResource);
				}
			}
		}, delegate(ClusterException exception)
		{
			virtualMachineQueueReload = null;
			ClusterLog.LogException(exception);
		});
	}

	private static void BeginLoadResourceProperties(VirtualMachineResource virtualMachineResource)
	{
		virtualMachineResource.LoadAsync(536875008);
		virtualMachineResource.LoadAsync(537059328);
	}

	private static string GetServiceDescription(SafeServiceHandle serviceControllerHandle, string serviceName)
	{
		NativeMethods.SERVICE_DESCRIPTION sERVICE_DESCRIPTION = default(NativeMethods.SERVICE_DESCRIPTION);
		using (SafeServiceHandle safeServiceHandle = NativeMethods.OpenService(serviceControllerHandle, serviceName, 1))
		{
			if (safeServiceHandle.IsInvalid)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			int pcbBytesNeeded = 0;
			NativeMethods.QueryServiceConfig2(safeServiceHandle, 1, IntPtr.Zero, 0, ref pcbBytesNeeded);
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 122)
			{
				throw new Win32Exception(lastWin32Error);
			}
			using NativeMethods.UnmanagedBuffer unmanagedBuffer = new NativeMethods.UnmanagedBuffer(pcbBytesNeeded);
			if (unmanagedBuffer.IsMemoryValid)
			{
				if (!NativeMethods.QueryServiceConfig2(safeServiceHandle, 1, unmanagedBuffer.IntPtr, pcbBytesNeeded, ref pcbBytesNeeded))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				sERVICE_DESCRIPTION = (NativeMethods.SERVICE_DESCRIPTION)Marshal.PtrToStructure(unmanagedBuffer.IntPtr, typeof(NativeMethods.SERVICE_DESCRIPTION));
			}
		}
		return sERVICE_DESCRIPTION.description;
	}

	private void RefreshMonitoredServices()
	{
		WeakReferenceEx<ObservableCollection<VMServiceInfo>> weakReferenceEx = vmMonitoredServices;
		ObservableCollection<VMServiceInfo> services;
		if (weakReferenceEx != null)
		{
			services = weakReferenceEx.Target;
		}
		else
		{
			services = new ObservableCollection<VMServiceInfo>();
			weakReferenceEx = new WeakReferenceEx<ObservableCollection<VMServiceInfo>>(services);
			vmMonitoredServices = weakReferenceEx;
		}
		if (services == null)
		{
			services = new ObservableCollection<VMServiceInfo>();
		}
		services.Clear();
		services.Add(new VMServiceInfo
		{
			DisplayName = CommonResources.LoadingText
		});
		LoadAsync(delegate(ClusterLoadedEventArgs loadResult)
		{
			if (loadResult.Error != null)
			{
				Error = loadResult.Error;
			}
			else
			{
				string computerName = GuestComputerName;
				if (string.IsNullOrEmpty(computerName))
				{
					Global.DefaultDispatcher.Invoke(delegate
					{
						services.Clear();
						services.Add(new VMServiceInfo
						{
							DisplayName = string.Empty
						});
					});
				}
				else
				{
					if (ClusterVMMonitoredItem.IsMonitoringSupported(GuestOsProductType, GuestOsMajorVersion, GuestOsMinorVersion))
					{
						try
						{
							bool removedLoadingEntry = false;
							new List<ClusterVMMonitoredItem>(ClusterVMMonitoredItem.GetMonitoredItems(computerName, CancellationToken.None)).ForEach(delegate(ClusterVMMonitoredItem monitoredItem)
							{
								if (monitoredItem is ClusterVMMonitoredService clusterVMMonitoredService)
								{
									ServiceController serviceController = new ServiceController(clusterVMMonitoredService.Name, computerName);
									VMServiceInfo serviceInfo = new VMServiceInfo
									{
										Name = clusterVMMonitoredService.Name,
										DisplayName = serviceController.DisplayName,
										IsMonitored = true
									};
									if (!removedLoadingEntry)
									{
										Global.DefaultDispatcher.Invoke(services.Clear);
										removedLoadingEntry = true;
									}
									Global.DefaultDispatcher.Invoke(delegate
									{
										services.Add(serviceInfo);
									});
								}
							});
							Global.DefaultDispatcher.Invoke((!removedLoadingEntry) ? new Action(services.Clear) : ((Action)delegate
							{
								services.OrderBy((VMServiceInfo s) => s.DisplayName);
							}));
							return;
						}
						catch (Exception ex2)
						{
							Exception ex3 = ex2;
							Exception ex = ex3;
							Global.DefaultDispatcher.BeginInvoke((Action)delegate
							{
								services.Clear();
								services.Add(new VMServiceInfo
								{
									DisplayName = EnumResources.VMMonitoring_FailedToDetermineMonitoring
								});
								services.Add(new VMServiceInfo
								{
									DisplayName = ex.Message
								});
							});
							return;
						}
					}
					Global.DefaultDispatcher.BeginInvoke((Action)delegate
					{
						services.Clear();
						services.Add(new VMServiceInfo
						{
							DisplayName = ExceptionResources.MonitoringNotSupported_Default
						});
					});
				}
			}
		}, (ResourceLoadSelection)16384);
	}

	private static bool ActivateExistingVirtualMachineConnect(Process virtualMachineConnectProcess, string serverName, string vmInstanceId)
	{
		int currentSessionId = Process.GetCurrentProcess().SessionId;
		Func<Process, bool> func = delegate(Process vmConnectProcess)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (vmConnectProcess.SessionId != currentSessionId)
				{
					return false;
				}
				string url = string.Format(CultureInfo.InvariantCulture, "ipc://localhost:{0}/VMConnectCommunicator.rem", vmConnectProcess.Id.ToString(CultureInfo.InvariantCulture));
				NativeMethods.AllowSetForegroundWindow(vmConnectProcess.Id);
				return ((IVMConnectCommunicator)Activator.GetObject(typeof(IVMConnectCommunicator), url)).ActivateIfConnectedToVM(serverName, vmInstanceId);
			}
			catch (RemotingException exception)
			{
				ClusterLog.LogException(exception, "Error switching to vm Server {0}, InstanceId {1}", serverName, vmInstanceId);
			}
			return false;
		};
		if (virtualMachineConnectProcess != null && !virtualMachineConnectProcess.HasExited)
		{
			return func(virtualMachineConnectProcess);
		}
		return Process.GetProcessesByName(HyperVObjectFactory.GetVmConnectFileName(HyperVObjectFactory.GetServer(serverName), (HyperVAssemblyVersion)0)).Any(func);
	}

	private void ExecutePrivateVmActionSingleSelect(Action action, string exceptionMessage)
	{
		UIHelper.ExecuteOnDispatcher(delegate
		{
			//IL_000e: Expected O, but got Unknown
			try
			{
				action();
			}
			catch (VirtualizationException val)
			{
				VirtualizationException innerException = val;
				ClusterDialogException.ShowTaskDialogAsync(new ClusterVirtualMachineManageException(exceptionMessage, base.Name, (Exception)(object)innerException));
			}
		}, OperationType.Async);
	}

	private void EnqueueCheckpointsRefreshOperation()
	{
		bool flag = false;
		lock (base.LockObject)
		{
			if (virtualMachineQueueReload == null)
			{
				virtualMachineQueueReload = new Queue<VirtualMachineResource>();
				flag = true;
			}
			if (!virtualMachineQueueReload.Contains(this))
			{
				virtualMachineQueueReload.Enqueue(this);
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
				VirtualMachineResource virtualMachineResource;
				lock (base.LockObject)
				{
					if (virtualMachineQueueReload == null || virtualMachineQueueReload.Count == 0)
					{
						virtualMachineQueueReload = null;
						break;
					}
					virtualMachineResource = virtualMachineQueueReload.Dequeue();
				}
				WeakReferenceEx<VirtualMachineCheckpointInformation> weakReferenceEx = virtualMachineResource.checkpointInformationWeak;
				if (weakReferenceEx != null && weakReferenceEx.Target != null)
				{
					BeginLoadCheckpoints(virtualMachineResource);
				}
			}
		}, delegate(ClusterException exception)
		{
			virtualMachineQueueReload = null;
			ClusterLog.LogException(exception);
		});
	}

	private VirtualMachineCheckpointInformation GetCheckpointInformation()
	{
		VirtualMachineCheckpointInformation result;
		if (temporaryCheckpointInformation != null)
		{
			result = (checkpointInformation = temporaryCheckpointInformation);
			checkpointInformationWeak = new WeakReferenceEx<VirtualMachineCheckpointInformation>(checkpointInformation);
			temporaryCheckpointInformation = null;
		}
		else
		{
			result = WeakReferenceEx.ReturnInstance(ref checkpointInformationWeak, delegate
			{
				BeginLoadCheckpoints(this);
				return EmptyCheckpointInformation;
			});
		}
		return result;
	}

	private static void BeginLoadCheckpoints(VirtualMachineResource vmResource)
	{
		vmResource.LoadAsync(537133056);
	}

	public void ConfigureMonitoring(ObservableCollection<VMServiceInfo> services)
	{
		if (services == null)
		{
			throw new NullReferenceException("services");
		}
		Worker.Start(delegate
		{
			try
			{
				ClusterVMMonitoredItem.ConnectToTaskScheduler(GuestComputerName, out var taskService, out var taskFolder, CancellationToken.None);
				List<ClusterVMMonitoredItem> list = new List<ClusterVMMonitoredItem>(ClusterVMMonitoredItem.GetMonitoredItems(GuestComputerName, CancellationToken.None));
				vmServices = new WeakReferenceEx<ObservableCollection<VMServiceInfo>>(services);
				bool flag = false;
				foreach (VMServiceInfo serviceInfo in services)
				{
					if (serviceInfo.IsMonitored)
					{
						new ClusterVMMonitoredService(serviceInfo.Name, Guid.NewGuid(), GuestComputerName).RegisterItem(taskService, taskFolder, GuestComputerName);
						flag = true;
					}
					else
					{
						ClusterVMMonitoredItem clusterVMMonitoredItem = list.Find((ClusterVMMonitoredItem i) => i is ClusterVMMonitoredService && i.Name.Equals(serviceInfo.Name, StringComparison.CurrentCultureIgnoreCase));
						if (clusterVMMonitoredItem != null)
						{
							new ClusterVMMonitoredService(serviceInfo.Name, Guid.NewGuid(), GuestComputerName).UnregisterItem(taskService, taskFolder, GuestComputerName);
							list.Remove(clusterVMMonitoredItem);
							flag = true;
						}
					}
				}
				if (flag)
				{
					this.ExecuteMethod(delegate(ILockable lockObject)
					{
						lockObject.Owner.SendEventToProxy(new ClusterWrapperEventArgs(EventType.VMServiceChanged, null));
					}, LockAccess.Reader);
				}
			}
			catch (COMException innerException)
			{
				vmServices = null;
				vmMonitoredServices = null;
				ClusterDialogException.ShowTaskDialog(new ClusterDialogException(ExceptionResources.VirtualMachineConfigureServiceException_Header.FormatCurrentCulture(base.Name), innerException));
			}
			catch (ApplicationException innerException2)
			{
				vmServices = null;
				vmMonitoredServices = null;
				ClusterDialogException.ShowTaskDialog(new ClusterDialogException(ExceptionResources.VirtualMachineConfigureServiceException_Header.FormatCurrentCulture(base.Name), innerException2));
			}
			catch (UnauthorizedAccessException innerException3)
			{
				vmServices = null;
				vmMonitoredServices = null;
				ClusterDialogException.ShowTaskDialog(new ClusterDialogException(ExceptionResources.VirtualMachineConfigureServiceException_Header.FormatCurrentCulture(base.Name), innerException3));
			}
		}, delegate(ClusterException exception)
		{
			vmServices = null;
			vmMonitoredServices = null;
			ClusterDialogException.ShowTaskDialog(exception);
		});
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		virtualMachineQueueReload = null;
		virtualMachineState = null;
		desktopThumbnailWeak = null;
		memberTemporaryThumbnail = null;
		guestAssignedMemory = null;
		guestAvailableMemory = null;
		guestMemoryDemand = null;
		guestCpuUsage = null;
		heartbeatStatus = null;
		guestUpTime = null;
		guestCreationTime = null;
		guestOperatingSystem = null;
		guestComputerName = null;
		integrationServicesVersion = null;
		storageInformation = null;
		checkpointInformation = null;
		guestNotes = null;
		guestStatus = null;
		migrationState = null;
		migrationProgress = null;
		integrationServicesStatus = null;
		vmServices = null;
		vmMonitoredServices = null;
		extendedRelationship = null;
		primaryRelationship = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("ApplicationStatus");
			OnPropertyChanged("GuestAssignedMemory");
			OnPropertyChanged("GuestAvailableMemory");
			OnPropertyChanged("GuestCpuUsage");
			OnPropertyChanged("GuestCreationTime");
			OnPropertyChanged("GuestComputerName");
			OnPropertyChanged("GuestDesktopThumbnail");
			OnPropertyChanged("GuestMemoryDemand");
			OnPropertyChanged("GuestNotes");
			OnPropertyChanged("GuestOperatingSystem");
			OnPropertyChanged("GuestStatus");
			OnPropertyChanged("GuestUptime");
			OnPropertyChanged("HeartbeatStatus");
			OnPropertyChanged("IntegrationServicesInformation");
			OnPropertyChanged("IntegrationServicesStatus");
			OnPropertyChanged("MigrationState");
			OnPropertyChanged("MigrationProgress");
			OnPropertyChanged("StorageInformation");
			OnPropertyChanged("Checkpoints");
			OnPropertyChanged("VMServices");
			OnPropertyChanged("VMMonitoredServices");
			OnPropertyChanged("ReplicationMode");
			OnPropertyChanged("ReplicationHealth");
			OnPropertyChanged("ReplicationPrimaryServerFullyQualifiedDomainName");
			OnPropertyChanged("ReplicationPrimaryConnectionPoint");
			OnPropertyChanged("ReplicationTaskProgress");
			OnPropertyChanged("ReplicationTaskName");
			OnPropertyChanged("ReplicationRecoveryServerFullyQualifiedDomainName");
			OnPropertyChanged("ReplicationRecoveryConnectionPoint");
			OnPropertyChanged("ReplicationState");
			OnPropertyChanged("LastReplicaTime");
			OnPropertyChanged("ExtendedReplicationHealth");
			OnPropertyChanged("ExtendedReplicationTaskProgress");
			OnPropertyChanged("ExtendedReplicationTaskName");
			OnPropertyChanged("ExtendedReplicationRecoveryServerFullyQualifiedDomainName");
			OnPropertyChanged("ExtendedReplicationRecoveryConnectionPoint");
			OnPropertyChanged("ExtendedReplicationState");
			OnPropertyChanged("ExtendedLastReplicaTime");
		}, OperationType.Async);
	}
}

