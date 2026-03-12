using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.Windows.Server;
using Microsoft.WindowsAPICodePack.Dialogs;
using MS.Internal.FailoverClusters.Framework;
using MS.Internal.ServerClusters;

namespace Microsoft.FailoverClusters.Framework;

public class StorageResource : AverageResource
{
	private WeakReferenceEx removeFromGroupCommandWeak;

	private WeakReferenceEx repairCommandWeak;

	private WeakReferenceEx turnOnMaintenanceCommandWeak;

	private WeakReferenceEx turnOffMaintenanceCommandWeak;

	private WeakReferenceEx addToClusterSharedVolumeWeak;

	private WeakReferenceEx replicationClusterSharedVolumeWeak;

	private WeakReferenceEx replicationEnableCommandWeak;

	private WeakReferenceEx replicationRemoveCommandWeak;

	private WeakReferenceEx replicationAddPartnerCommandWeak;

	private static readonly ClusterDisk EmptyClusterDisk = new ClusterDisk();

	private Guid? poolId;

	private Guid? virtualDiskId;

	private string virtualDiskName;

	private string virtualDiskDescription;

	private StoragePoolHealth? virtualDiskHealth;

	private uint? virtualDiskProvisioning;

	private VirtualDiskResiliencyType? virtualDiskResiliencyType;

	private uint? virtualDiskResiliencyColumns;

	private uint? virtualDiskResiliencyInterleave;

	private VirtualDiskState? virtualDiskState;

	private bool? maintenanceMode;

	private uint? diskRunChkDsk;

	private ClusterDisk memberDiskInfo;

	private ReplicationInfo replicationInfo;

	private PoolInfo poolInfo;

	private StorageCaps storageCaps;

	private ReplicationDiskType replicationDiskType;

	private IList<ReplicationStatusInfo> replicationStatus;

	public bool? MaintenanceMode
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.Maintenance))
			{
				return null;
			}
			return LoadAsync(maintenanceMode, 4);
		}
	}

	public bool PassThroughDisk
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return false;
			}
			uint? num = LoadAsync(diskRunChkDsk, 4);
			if (num.HasValue && num.Value == 6)
			{
				return true;
			}
			return false;
		}
	}

	public ClusterDisk DiskInfo
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VolumeInfo))
			{
				return null;
			}
			if (base.ResourceState != ResourceState.Online)
			{
				return EmptyClusterDisk;
			}
			return LoadAsync(memberDiskInfo, 261);
		}
	}

	public StorageCaps StorageCaps => storageCaps;

	public StorageSize DiskCapacity
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VolumeInfo))
			{
				return null;
			}
			ClusterDisk diskInfo = DiskInfo;
			if (diskInfo != null)
			{
				ulong? size = diskInfo.Size;
				if (size.HasValue)
				{
					return FormatHelp.GetStorageSizeFromULong(size.Value);
				}
			}
			return null;
		}
	}

	public ReplicationDiskType ReplicationDiskType => LoadAsync<ReplicationDiskType, ReplicationDiskType>(replicationDiskType, 1);

	public IList<ReplicationStatusInfo> ReplicationStatus => LoadAsync(replicationStatus, 2049);

	public ReplicationInfo ReplicationInfo
	{
		get
		{
			if (ReplicationDiskType == ReplicationDiskType.None)
			{
				return null;
			}
			return LoadAsync(replicationInfo, 2049);
		}
	}

	public PhysicalDiskBusType BusType => PhysicalDiskBusType.Unknown;

	public string PoolName => string.Empty;

	public string Role => base.OwnerGroup.DisplayName;

	public int? Slot => null;

	public string EnclosureName => string.Empty;

	public string DiskNumber
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VolumeInfo))
			{
				return null;
			}
			ClusterDisk diskInfo = DiskInfo;
			if (diskInfo != null)
			{
				if (!diskInfo.DiskNumber.HasValue)
				{
					return string.Empty;
				}
				return diskInfo.DiskNumber.Value.ToString(CultureInfo.CurrentCulture);
			}
			return null;
		}
	}

	public bool? IsInAvailableStorage
	{
		get
		{
			if (base.OwnerGroup != null)
			{
				return base.OwnerGroup == base.Cluster.AvailableStorage;
			}
			return null;
		}
	}

	public override Guid? PoolId
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(poolId, 4);
		}
	}

	public Guid? VirtualDiskId
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(virtualDiskId, 4);
		}
	}

	public string VirtualDiskName
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(virtualDiskName, 4);
		}
	}

	public string VirtualDiskDescription
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(virtualDiskDescription, 4);
		}
	}

	public StoragePoolHealth VirtualDiskHealth
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return StoragePoolHealth.Unknown;
			}
			return LoadAsync<StoragePoolHealth, StoragePoolHealth>(virtualDiskHealth, 4);
		}
	}

	public uint? VirtualDiskProvisioning
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(virtualDiskProvisioning, 4);
		}
	}

	public VirtualDiskResiliencyType VirtualDiskResiliencyType
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return VirtualDiskResiliencyType.Unknown;
			}
			return LoadAsync<VirtualDiskResiliencyType, VirtualDiskResiliencyType>(virtualDiskResiliencyType, 4);
		}
	}

	public uint? VirtualDiskResiliencyColumns
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(virtualDiskResiliencyColumns, 4);
		}
	}

	public uint? VirtualDiskResiliencyInterleave
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(virtualDiskResiliencyInterleave, 4);
		}
	}

	public VirtualDiskState VirtualDiskState
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return VirtualDiskState.Unknown;
			}
			return LoadAsync<VirtualDiskState, VirtualDiskState>(virtualDiskState, 4);
		}
	}

	public PoolInfo PoolInfo
	{
		get
		{
			if (!storageCaps.HasFlag(StorageCaps.VirtualDisk))
			{
				return null;
			}
			return LoadAsync(poolInfo, 516);
		}
	}

	public string AssignedToDisplayName
	{
		get
		{
			if (base.IsQuorum)
			{
				return EnumResources.DiskWitnessInQuorum;
			}
			if (base.OwnerGroup != null)
			{
				return base.OwnerGroup.DisplayName;
			}
			return string.Empty;
		}
	}

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.StorageResource));

	public event EventHandler<ClusterMaintenanceModeEventArgs> MaintenanceModeChanged;

	public event EventHandler<ClusterStoragePropertiesChangedEventArgs> StoragePropertiesChanged;

	public event EventHandler<ClusterStorageReplicationInfoChangedEventArgs> StorageReplicationInfoChanged;

	public void AddToClusterSharedVolumes()
	{
		AddToClusterSharedVolumes(base.SetLastError);
	}

	public void AddToClusterSharedVolumes(Action<OperationResult> addStorageToClusterSharedVolumeOperation)
	{
		if (!storageCaps.HasFlag(StorageCaps.Csv))
		{
			addStorageToClusterSharedVolumeOperation.SafeCall(new OperationResult(this, null));
		}
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			if (lockObject.Owner is PStorageResource pStorageResource)
			{
				pStorageResource.AddToClusterSharedVolumes();
			}
		}, addStorageToClusterSharedVolumeOperation, LockAccess.Reader);
	}

	public override void Offline(bool enableOverride)
	{
		ConfirmationDialog confirmationDialog = null;
		if (this is CsvVolumeResource)
		{
			confirmationDialog = CreateConfirmationDialog(DialogResources.StorageCSVOffline_Title, DialogResources.StorageOffline_Header.FormatCurrentCulture(base.Name), DialogResources.StorageCSVOffline_Content);
		}
		else if (!(base.OwnerGroup is AvailableStorageGroup))
		{
			confirmationDialog = CreateConfirmationDialog(DialogResources.StorageOffline_Title, DialogResources.StorageOffline_Header.FormatCurrentCulture(base.Name), DialogResources.StorageOffline_Content.FormatCurrentCulture(base.Name));
		}
		if (confirmationDialog == null || confirmationDialog.ShowDialog() == TaskDialogResult.Yes)
		{
			base.Offline(enableOverride);
		}
	}

	internal static CommandCollection InitializeCommands(Cluster cluster, IEnumerable<StorageResource> resources)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceState)
		{
			Name = "Multi StorageResource State Commands"
		};
		if (resources.All((StorageResource r) => r.IsInAvailableStorage == true && r.storageCaps.HasFlag(StorageCaps.Csv)))
		{
			ClusterCommand item = new ClusterCommand(null, "MultiAddToClusterSharedVolumes", ClusterCommandId.MultipleStorageResourceAddToClusterSharedVolumes, commandCollection.Category)
			{
				Text = CommandResources.AddClusterSharedVolumesAction_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					StorageResource storageResource = resources.FirstOrDefault();
					if (storageResource != null)
					{
						AddAvailableDisksToClusterSharedVolume(storageResource.Cluster, resources, askConfirmation: true);
					}
				}
			};
			commandCollection.Add(item);
		}
		bool flag = resources.All((StorageResource r) => r is CsvVolumeResource);
		if (flag)
		{
			ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(null, "MoveMultipleClusterSharedVolume", ClusterCommandId.MultipleCsvMoveTo, commandCollection.Category)
			{
				Text = CommandResources.MoveResourceAction_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration.");
				},
				CommandParameter = null
			};
			IClusterList<Node> clusterList = resources.FirstOrDefault().Cluster.AllUpNodes.ForceLoadStart();
			ClusterCommand item2 = new ClusterCommand(null, "MultipleGroupMoveToBestNode", ClusterCommandId.MultipleCsvMoveToBestNode, ClusterCommandCollectionId.GroupOwnership)
			{
				Text = CommandResources.Group_MoveToBestPossible,
				InputParameters = null,
				CommandParameter = null,
				Description = string.Empty,
				ExecuteDelegate = delegate(object node)
				{
					List<GroupMoveDescriptor> groupMoveDescriptors2 = new List<GroupMoveDescriptor>();
					resources.OfType<CsvVolumeResource>().ToList().ForEach(delegate(CsvVolumeResource csv)
					{
						groupMoveDescriptors2.Add(new GroupMoveDescriptor
						{
							ClusterObjectIssuingMoveRequest = csv,
							OwnerGroup = csv.OwnerGroup
						});
					});
					GroupMoveCommandBase.DefaultExecuteMove(node, groupMoveDescriptors2);
				}
			};
			ClusterCommand item3 = new ClusterCommand(null, "MultipleGroupMoveToSelectedNode", ClusterCommandId.MultipleCsvMoveToSelectedNode, ClusterCommandCollectionId.GroupOwnership)
			{
				Text = CommandResources.Group_MoveToSelected,
				InputParameters = (ClusterList<Node>)clusterList,
				CommandParameter = null,
				Description = string.Empty,
				ExecuteDelegate = delegate(object node)
				{
					List<GroupMoveDescriptor> groupMoveDescriptors = new List<GroupMoveDescriptor>();
					resources.OfType<CsvVolumeResource>().ToList().ForEach(delegate(CsvVolumeResource csv)
					{
						groupMoveDescriptors.Add(new GroupMoveDescriptor
						{
							ClusterObjectIssuingMoveRequest = csv,
							OwnerGroup = csv.OwnerGroup
						});
					});
					GroupMoveCommandBase.DefaultExecuteMove(node, groupMoveDescriptors);
				}
			};
			clusterCommandContainer.ChildrenInternal.Add(item2);
			clusterCommandContainer.ChildrenInternal.Add(item3);
			commandCollection.Add(clusterCommandContainer);
		}
		if (resources.All((StorageResource sr) => sr.storageCaps.HasFlag(StorageCaps.Maintenance)))
		{
			ClusterCommandContainer clusterCommandContainer2 = new ClusterCommandContainer(null, "MultiMoreActionCommand", ClusterCommandId.MultipleStorageResourceMoreAction, commandCollection.Category)
			{
				Text = CommandResources.MoreActions,
				CanExecuteDelegate = (object x) => false,
				ExecuteDelegate = delegate
				{
					throw new InvalidOperationException("This is a command container");
				}
			};
			ClusterCommand item4 = new ClusterCommand(null, "MultiTurnOnMaintenanceMode", ClusterCommandId.MultipleStorageResourceTurnOnMaintenanceMode, commandCollection.Category)
			{
				Text = CommandResources.MaintenanceModeOn_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					SetMaintenanceMode(resources, maintenanceMode: true, askConfirmation: true);
				}
			};
			ClusterCommand item5 = new ClusterCommand(null, "MultiTurnOffMaintenanceMode", ClusterCommandId.MultipleStorageResourceTurnoffMaintenanceMode, commandCollection.Category)
			{
				Text = CommandResources.MaintenanceModeOff_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					SetMaintenanceMode(resources, maintenanceMode: false, askConfirmation: true);
				}
			};
			clusterCommandContainer2.ChildrenInternal.Add(item4);
			clusterCommandContainer2.ChildrenInternal.Add(item5);
			commandCollection.Add(clusterCommandContainer2);
		}
		if (!resources.Any((StorageResource r) => r.IsQuorum))
		{
			bool flag2 = resources.All((StorageResource r) => r.IsInAvailableStorage == true);
			IEnumerable<string> source = (from r in resources
				where r.OwnerGroup != null
				select r.OwnerGroup.Name).Distinct(StringComparer.OrdinalIgnoreCase);
			bool flag3 = source.Count() == 1;
			ClusterCommand clusterCommand = new ClusterCommand(null, "MultiStorageRemove", ClusterCommandId.MultipleStorageResourceRemove, commandCollection.Category)
			{
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					DeleteStorageResources(resources, askConfirmation: true);
				}
			};
			if (flag)
			{
				clusterCommand.Text = CommandResources.RemoveFromClusterSharedVolumeCommand_Text;
			}
			else if (flag2)
			{
				clusterCommand.Text = CommandResources.RemoveCommand_Text;
			}
			else if (flag3)
			{
				clusterCommand.Text = CommandResources.RemoveFromGroupActionFormat_Text.FormatCurrentCulture(source.FirstOrDefault());
			}
			else
			{
				clusterCommand.Text = CommandResources.RemoveCommand_Text;
			}
			commandCollection.Add(clusterCommand);
		}
		return commandCollection;
	}

	public static void DeleteStorageResources(IEnumerable<StorageResource> storageResources, bool askConfirmation)
	{
		StorageResource storageResource = storageResources.FirstOrDefault();
		if (storageResource == null)
		{
			return;
		}
		if (storageResources.Any((StorageResource r) => r.IsQuorum))
		{
			ConfirmationDialog confirmationDialog = new ConfirmationDialog(new TaskDialogButtonsStyle(TaskDialogButtonsSettings.Ok, TaskDialogStandardButtons.Ok));
			confirmationDialog.Icon = TaskDialogStandardIcon.Information;
			confirmationDialog.Caption = DialogResources.RemoveClusterDisksConfirmationDialog_Title;
			confirmationDialog.Header = DialogResources.RemoveStorageResource_CannotRemoveStorageResourceInQuorum;
			confirmationDialog.ShowDialog();
			return;
		}
		int num = storageResources.Count();
		if (num == 1)
		{
			storageResource.Delete(askConfirmation);
			return;
		}
		if (askConfirmation)
		{
			IEnumerable<string> source = storageResources.Select((StorageResource r) => r.OwnerGroup.Name).Distinct(StringComparer.OrdinalIgnoreCase);
			bool flag = storageResources.All((StorageResource r) => r is CsvVolumeResource);
			bool flag2 = source.Count() == 1;
			ConfirmationDialog confirmationDialog2 = new ConfirmationDialog();
			confirmationDialog2.Caption = (flag ? DialogResources.RemoveResource_Title.FormatCurrentCulture(ResourceKind.ClusterFileSystem.Translate()) : DialogResources.RemoveResource_Title.FormatCurrentCulture(ResourceKind.PhysicalDisk.Translate()));
			if (flag)
			{
				confirmationDialog2.Caption = DialogResources.DeleteClusterSharedVolume_Caption;
				confirmationDialog2.Header = DialogResources.DeleteClusterSharedVolume_Header;
				confirmationDialog2.Content = DialogResources.DeleteClusterSharedVolume_Content;
				confirmationDialog2.CustomIcon = storageResource.Icon.NativeIcon;
			}
			else if (flag2)
			{
				confirmationDialog2.Caption = DialogResources.DeleteStorageResourceFromRole_Caption.FormatCurrentCulture(source.FirstOrDefault());
				confirmationDialog2.Header = DialogResources.DeleteStorageResourceFromRole_Header.FormatCurrentCulture(source.FirstOrDefault());
				confirmationDialog2.Content = DialogResources.DeleteStorageResourcesFromRole_Content;
				confirmationDialog2.CustomIcon = storageResource.Icon.NativeIcon;
			}
			else
			{
				confirmationDialog2.Caption = DialogResources.RemoveClusterDisksConfirmationDialog_Title;
				confirmationDialog2.Header = DialogResources.RemoveClusterDisksConfirmationDialog_Header;
				confirmationDialog2.Content = DialogResources.RemoveClusterDisksConfirmationDialog_Content.FormatCurrentCulture(num);
				confirmationDialog2.Icon = TaskDialogStandardIcon.Question;
			}
			if (confirmationDialog2.ShowDialog() != TaskDialogResult.Yes)
			{
				return;
			}
		}
		Resource.EnqueueAndThrottleRequests(storageResources, delegate(Resource resource, Action<OperationResult> operationResult)
		{
			resource.Delete(operationResult);
		});
	}

	public static void AddAvailableDisksToClusterSharedVolume(Cluster cluster, IEnumerable<StorageResource> storageResources, bool askConfirmation)
	{
		Exceptions.ThrowIfNull(cluster, "cluster");
		Exceptions.ThrowIfNull(storageResources, "storageResources");
		cluster.LoadAsync(storageResources, null, delegate
		{
			if (storageResources.All((StorageResource sr) => sr.IsInAvailableStorage.HasValue && sr.IsInAvailableStorage.Value && sr.storageCaps.HasFlag(StorageCaps.Csv)))
			{
				Resource.EnqueueAndThrottleRequests(storageResources, delegate(Resource resource, Action<OperationResult> result)
				{
					StorageResource storageResource = resource as StorageResource;
					if (storageResource != null)
					{
						storageResource.AddToClusterSharedVolumes(result);
					}
				});
			}
		}, 1);
	}

	public static void SetMaintenanceMode(IEnumerable<StorageResource> storageResources, bool maintenanceMode, bool askConfirmation)
	{
		Exceptions.ThrowIfNull(storageResources, "storageResources");
		if (storageResources.Any((StorageResource sr) => !sr.storageCaps.HasFlag(StorageCaps.Maintenance)))
		{
			return;
		}
		int num = storageResources.Count();
		if (num <= 0)
		{
			return;
		}
		if (askConfirmation)
		{
			ConfirmationDialog confirmationDialog = new ConfirmationDialog
			{
				Icon = TaskDialogStandardIcon.Question
			};
			if (maintenanceMode)
			{
				confirmationDialog.Caption = DialogResources.TurnOnMaintenanceModeForStorageResourceMessage_Title;
				confirmationDialog.Header = DialogResources.TurnOnMaintenanceModeForStorageResourceMessage_Header;
				confirmationDialog.Content = DialogResources.TurnOnMaintenanceModeForStorageResourceMessage_Content.FormatCurrentCulture(num);
			}
			else
			{
				confirmationDialog.Caption = DialogResources.TurnOffMaintenanceModeForStorageResourceMessage_Title;
				confirmationDialog.Header = DialogResources.TurnOffMaintenanceModeForStorageResourceMessage_Header;
				confirmationDialog.Content = DialogResources.TurnOffMaintenanceModeForStorageResourceMessage_Content.FormatCurrentCulture(num);
			}
			if (confirmationDialog.ShowDialog() != TaskDialogResult.Yes)
			{
				return;
			}
		}
		Resource.EnqueueAndThrottleRequests(storageResources, delegate(Resource resource, Action<OperationResult> result)
		{
			StorageResource storageResource = resource as StorageResource;
			if (storageResource != null)
			{
				storageResource.SetMaintenanceMode(result, maintenanceMode);
			}
		});
	}

	public bool CanRemoveProceed()
	{
		if (base.OwnerGroup is ClusterSharedVolumeGroup && base.ResourceType.ResourceKind == ResourceKind.PhysicalDisk)
		{
			return false;
		}
		if (MaintenanceMode != false)
		{
			return !MaintenanceMode.HasValue;
		}
		return true;
	}

	protected override void InitializeStateCommands(CommandCollection commandsCollection)
	{
		base.InitializeStateCommands(commandsCollection);
		AddStorageCommands(commandsCollection);
	}

	protected virtual void InitializeReplicationCommands(CommandCollection commandsCollection)
	{
		if (base.Cluster.FunctionalLevel <= NativeMethods.NT9_MAJOR_VERSION)
		{
			return;
		}
		ClusterCommandContainer replicationClusterSharedVolumeCommand = WeakReferenceEx.ReturnInstance(ref replicationClusterSharedVolumeWeak, delegate
		{
			ClusterCommandContainer obj = new ClusterCommandContainer(this, "ReplicationClusterSharedVolume", ClusterCommandId.ReplicationGroup, ClusterCommandCollectionId.StorageReplication)
			{
				Text = CommandResources.ReplicationAction_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration.");
				},
				CommandParameter = base.OwnerGroup
			};
			ClusterCommand item = WeakReferenceEx.ReturnInstance(ref replicationEnableCommandWeak, () => new ClusterCommand(this, "ReplicationEnable", ClusterCommandId.ReplicationNew, ClusterCommandCollectionId.StorageReplicationEnable)
			{
				Text = CommandResources.ReplicationActionEnable_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for the correct wizard.");
				},
				CommandParameter = this
			});
			obj.ChildrenInternal.Add(item);
			ClusterCommand item2 = WeakReferenceEx.ReturnInstance(ref replicationRemoveCommandWeak, () => new ClusterCommand(this, "ReplicationRemove", ClusterCommandId.ReplicationRemove, ClusterCommandCollectionId.StorageReplicationRemove)
			{
				Text = CommandResources.ReplicationActionRemove_Text,
				CanExecuteDelegate = delegate(object x)
				{
					StorageResource storageResource2 = (StorageResource)x;
					return storageResource2.ReplicationInfo != null && storageResource2.ReplicationDiskType == ReplicationDiskType.Source;
				},
				ExecuteDelegate = delegate(object x)
				{
					StorageResource resource = (StorageResource)x;
					RemoveReplication(resource);
				},
				CommandParameter = this
			});
			obj.ChildrenInternal.Add(item2);
			ClusterCommand item3 = WeakReferenceEx.ReturnInstance(ref replicationAddPartnerCommandWeak, () => new ClusterCommand(this, "ReplicationAddPartner", ClusterCommandId.ReplicationAdd, ClusterCommandCollectionId.StorageReplicationAddPartner)
			{
				Text = CommandResources.ReplicationActionAddPartner_Text,
				CanExecuteDelegate = delegate(object x)
				{
					StorageResource storageResource = (StorageResource)x;
					return storageResource.ReplicationInfo != null && storageResource.ReplicationDiskType == ReplicationDiskType.Source;
				},
				ExecuteDelegate = delegate
				{
					throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for the correct wizard.");
				},
				CommandParameter = this
			});
			obj.ChildrenInternal.Add(item3);
			return obj;
		});
		StorageReplicationInfoChanged += ReplicationCommandUpdate;
		replicationClusterSharedVolumeCommand.Finalizing += delegate
		{
			StoragePropertiesChanged -= ReplicationCommandUpdate;
			replicationClusterSharedVolumeCommand = null;
		};
		commandsCollection.Add(replicationClusterSharedVolumeCommand);
	}

	protected override void InitializeResourceCommands(CommandCollection commandsCollection)
	{
		InitializeReplicationCommands(commandsCollection);
		base.InitializeResourceCommands(commandsCollection);
		ClusterCommand clusterCommand = commandsCollection.Cast<ClusterCommand>().FirstOrDefault((ClusterCommand command) => command.Id == ClusterCommandId.ResourceDelete);
		int index = commandsCollection.IndexOf(clusterCommand);
		ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref removeFromGroupCommandWeak, () => new ClusterCommand(this, "RemoveFromGroup", ClusterCommandId.StorageRemoveFromGroup, ClusterCommandCollectionId.ResourceGeneral)
		{
			Text = GetRemoveFromGroupCommandText(),
			CanExecuteDelegate = (object x) => CanRemoveProceed(),
			ExecuteDelegate = delegate
			{
				Delete(askConfirmation: true);
			},
			CommandParameter = this
		});
		base.OwnerGroupChanged += OnOwnerGroupChangedForRemoveFromGroupCommand;
		base.IsQuorumChanged += OnIsQuorumChangedForRemoveFromGroupCommand;
		clusterCommand2.Finalizing += delegate
		{
			base.OwnerGroupChanged -= OnOwnerGroupChangedForRemoveFromGroupCommand;
			base.IsQuorumChanged -= OnIsQuorumChangedForRemoveFromGroupCommand;
			removeFromGroupCommandWeak = null;
		};
		clusterCommand2.Visible = !base.IsQuorum;
		commandsCollection.Insert(index, clusterCommand2);
		if (clusterCommand != null)
		{
			commandsCollection.Remove(clusterCommand);
		}
	}

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		if (storageCaps.HasFlag(StorageCaps.Repair))
		{
			ClusterCommand item = WeakReferenceEx.ReturnInstance(ref repairCommandWeak, () => new ClusterCommand(this, "Repair", ClusterCommandId.StorageRepair, ClusterCommandCollectionId.ResourceGeneral)
			{
				Text = CommandResources.RepairPhysicalDiskAction_Text,
				CanExecuteDelegate = (object x) => base.ResourceState == ResourceState.Offline || base.ResourceState == ResourceState.Failed,
				ExecuteDelegate = delegate
				{
					throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration.");
				},
				CommandParameter = this
			});
			commandContainer.ChildrenInternal.Add(item);
		}
		if (storageCaps.HasFlag(StorageCaps.Maintenance))
		{
			ClusterCommand turnOnMaintenanceCommand = WeakReferenceEx.ReturnInstance(ref turnOnMaintenanceCommandWeak, () => new ClusterCommand(this, "TurnOnMaintenance", ClusterCommandId.StorageTurnOnMaintenanceMode, ClusterCommandCollectionId.StorageMaintenance)
			{
				Text = CommandResources.MaintenanceModeOn_Text,
				CanExecuteDelegate = delegate(object x)
				{
					StorageResource storageResource2 = (StorageResource)x;
					return storageResource2.ResourceState == ResourceState.Online && storageResource2.MaintenanceMode == false;
				},
				ExecuteDelegate = delegate(object x)
				{
					((StorageResource)x).SetMaintenanceMode(storageResourceMaintenanceMode: true, askConfirmation: true);
				},
				CommandParameter = this
			});
			StoragePropertiesChanged += MaintenanceCommandUpdate;
			base.StateChanged += MaintenanceCommandStateUpdate;
			base.IsQuorumChanged += MaintenanceCommandIsQuorumUpdate;
			turnOnMaintenanceCommand.Visible = !base.IsQuorum;
			turnOnMaintenanceCommand.Finalizing += delegate
			{
				StoragePropertiesChanged -= MaintenanceCommandUpdate;
				base.StateChanged -= MaintenanceCommandStateUpdate;
				base.IsQuorumChanged -= MaintenanceCommandIsQuorumUpdate;
				turnOnMaintenanceCommand = null;
			};
			commandContainer.ChildrenInternal.Add(turnOnMaintenanceCommand);
			ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref turnOffMaintenanceCommandWeak, () => new ClusterCommand(this, "TurnOffMaintenance", ClusterCommandId.StorageTurnoffMaintenanceMode, ClusterCommandCollectionId.StorageMaintenance)
			{
				Text = CommandResources.MaintenanceModeOff_Text,
				CanExecuteDelegate = delegate(object x)
				{
					StorageResource storageResource = (StorageResource)x;
					return storageResource.ResourceState == ResourceState.Online && storageResource.MaintenanceMode == true;
				},
				ExecuteDelegate = delegate(object x)
				{
					((StorageResource)x).SetMaintenanceMode(storageResourceMaintenanceMode: false);
				},
				CommandParameter = this
			});
			clusterCommand.Visible = !base.IsQuorum;
			commandContainer.ChildrenInternal.Add(clusterCommand);
		}
		base.InitializeMoreActionsCommands(commandContainer);
	}

	protected virtual string GetRemoveFromGroupCommandText()
	{
		if (!(base.OwnerGroup != null) || base.OwnerGroup.GroupType == GroupType.AvailableStorage)
		{
			return CommandResources.RemoveCommand_Text;
		}
		return CommandResources.RemoveFromGroupActionFormat_Text.FormatCurrentCulture(base.OwnerGroup.DisplayName);
	}

	private void MaintenanceCommandUpdate(object sender, EventArgs e)
	{
		ExecuteMaintenanceUpdate(turnOnMaintenanceCommandWeak, sender, e);
		ExecuteMaintenanceUpdate(turnOffMaintenanceCommandWeak, sender, e);
	}

	private void MaintenanceCommandStateUpdate(object sender, ClusterResourceStateEventArgs e)
	{
		ExecuteMaintenanceUpdate(turnOnMaintenanceCommandWeak, sender, e);
		ExecuteMaintenanceUpdate(turnOffMaintenanceCommandWeak, sender, e);
	}

	private void MaintenanceCommandIsQuorumUpdate(object sender, ClusterResourceIsQuorumChangedEventArgs e)
	{
		ExecuteMaintenanceVisibilityUpdate(turnOnMaintenanceCommandWeak, sender, e);
		ExecuteMaintenanceVisibilityUpdate(turnOffMaintenanceCommandWeak, sender, e);
		RefreshCommands();
	}

	private void ReplicationCommandUpdate(object sender, EventArgs e)
	{
		if (!(replicationClusterSharedVolumeWeak.Target is ClusterCommandContainer clusterCommandContainer))
		{
			return;
		}
		foreach (ClusterCommand child in clusterCommandContainer.Children)
		{
			child.CanExecuteUpdate(sender, e);
		}
	}

	private static void ExecuteMaintenanceUpdate(WeakReferenceEx command, object sender, EventArgs e)
	{
		if (command != null && command.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.CanExecuteUpdate(sender, e);
		}
	}

	private void ExecuteMaintenanceVisibilityUpdate(WeakReferenceEx command, object sender, EventArgs e)
	{
		if (command != null && command.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.Visible = !base.IsQuorum;
		}
	}

	private void AddStorageCommands(CommandCollection applicationCommands)
	{
		if (!storageCaps.HasFlag(StorageCaps.Csv))
		{
			return;
		}
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref addToClusterSharedVolumeWeak, () => new ClusterCommand(this, "AddToClusterSharedVolumes", ClusterCommandId.StorageAddToClusterSharedVolume, applicationCommands.Category)
		{
			Text = CommandResources.AddClusterSharedVolumesAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				AddToClusterSharedVolumes();
			},
			CommandParameter = this
		});
		clusterCommand.Visible = IsInAvailableStorage == true;
		base.OwnerGroupChanged += OnOwnerGroupChangedForAddToClusterSharedVolumeCommand;
		clusterCommand.Finalizing += delegate
		{
			base.OwnerGroupChanged -= OnOwnerGroupChangedForAddToClusterSharedVolumeCommand;
			addToClusterSharedVolumeWeak = null;
		};
		applicationCommands.Add(clusterCommand);
	}

	private void OnOwnerGroupChangedForAddToClusterSharedVolumeCommand(object sender, ClusterResourceOwnerGroupEventArgs args)
	{
		WeakReferenceEx weakReferenceEx = addToClusterSharedVolumeWeak;
		if (weakReferenceEx != null && weakReferenceEx.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.Visible = IsInAvailableStorage == true;
			RefreshCommands();
		}
	}

	private void OnOwnerGroupChangedForRemoveFromGroupCommand(object sender, ClusterResourceOwnerGroupEventArgs args)
	{
		WeakReferenceEx weakReferenceEx = removeFromGroupCommandWeak;
		if (weakReferenceEx != null && weakReferenceEx.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.Text = GetRemoveFromGroupCommandText();
		}
	}

	private void OnIsQuorumChangedForRemoveFromGroupCommand(object sender, ClusterResourceIsQuorumChangedEventArgs args)
	{
		WeakReferenceEx weakReferenceEx = removeFromGroupCommandWeak;
		if (weakReferenceEx != null && weakReferenceEx.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.Visible = !base.IsQuorum;
			RefreshCommands();
		}
	}

	internal StorageResource(Cluster cluster)
		: base(cluster)
	{
	}

	protected override void CreateDeleteDialog(Action<ConfirmationDialog> confirmationDialogCreation, bool createDialog)
	{
		base.CreateDeleteDialog(delegate(ConfirmationDialog baseConfirmation)
		{
			if (baseConfirmation == null)
			{
				confirmationDialogCreation(null);
			}
			else
			{
				if (this is CsvVolumeResource)
				{
					baseConfirmation.Caption = DialogResources.DeleteClusterSharedVolume_Caption;
					baseConfirmation.Header = DialogResources.DeleteClusterSharedVolume_Header;
					baseConfirmation.Content = DialogResources.DeleteClusterSharedVolume_Content;
				}
				else
				{
					string arg = ((base.OwnerGroup == null) ? DialogResources.DefaultDeleteRoleName : base.OwnerGroup.Name);
					baseConfirmation.Caption = DialogResources.DeleteStorageResourceFromRole_Caption.FormatCurrentCulture(arg);
					baseConfirmation.Header = DialogResources.DeleteStorageResourceFromRole_Header.FormatCurrentCulture(arg);
					baseConfirmation.Content = DialogResources.DeleteStorageResourcesFromRole_Content;
				}
				confirmationDialogCreation(baseConfirmation);
			}
		}, createDialog);
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		PStorageResource pStorageResource = (PStorageResource)privateObject;
		base.TransferInternalData((PClusterObject)pStorageResource, subscribeToEvents, ignorePossibleOwners);
		storageCaps = pStorageResource.StorageCaps;
		replicationDiskType = pStorageResource.ReplicationDiskType;
		replicationStatus = pStorageResource.ReplicationStatus;
		if (pStorageResource.ReplicationInfo != null)
		{
			replicationInfo = pStorageResource.ReplicationInfo.Clone();
			replicationInfo.StorageResource = this;
		}
		ParseProperties(pStorageResource.Properties, trackChanges: false);
		if (pStorageResource.DiskInfo != null)
		{
			memberDiskInfo = pStorageResource.DiskInfo.Clone();
			memberDiskInfo.OwnerResource = this;
		}
		if (pStorageResource.PoolInfo != null)
		{
			poolInfo = (PoolInfo)pStorageResource.PoolInfo.Clone();
		}
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		if (storageCaps.HasFlag(StorageCaps.Maintenance))
		{
			ParseProperty(properties, "MaintenanceMode", ref maintenanceMode, list);
		}
		if (storageCaps.HasFlag(StorageCaps.PassThrough) && ParseProperty(properties, "DiskRunChkDsk", ref diskRunChkDsk, list) && trackChanges)
		{
			list.TryAdd("PassThroughDisk");
		}
		if (storageCaps.HasFlag(StorageCaps.VirtualDisk))
		{
			ParseProperty(properties, "PoolId", ref poolId, list);
			ParseProperty(properties, "VirtualDiskId", ref virtualDiskId, list);
			ParseProperty(properties, "VirtualDiskName", ref virtualDiskName, list);
			ParseProperty(properties, "VirtualDiskDescription", ref virtualDiskDescription, list);
			ParseProperty(properties, "VirtualDiskHealth", ref virtualDiskHealth, list);
			ParseProperty(properties, "VirtualDiskProvisioning", ref virtualDiskProvisioning, list);
			ParseProperty(properties, "VirtualDiskResiliencyType", ref virtualDiskResiliencyType, list);
			ParseProperty(properties, "VirtualDiskResiliencyColumns", ref virtualDiskResiliencyColumns, list);
			ParseProperty(properties, "VirtualDiskResiliencyInterleave", ref virtualDiskResiliencyInterleave, list);
			ParseProperty(properties, "VirtualDiskState", ref virtualDiskState, list);
		}
		return list;
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.ResourceStoragePropertiesChanged:
		{
			ClusterStoragePropertiesChangedEventArgs args = e.EventArgument as ClusterStoragePropertiesChangedEventArgs;
			if (args.Error != null)
			{
				return true;
			}
			if (args.Disk != null)
			{
				memberDiskInfo = args.Disk.Clone();
				memberDiskInfo.OwnerResource = this;
			}
			else if (args.Reload)
			{
				base.LoadSelection &= -257;
				memberDiskInfo = null;
			}
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("DiskInfo");
				OnPropertyChanged("DiskCapacity");
				this.StoragePropertiesChanged.SafeCall(this, args);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs clusterPropertiesEventArgs = e.EventArgument as ClusterPropertiesEventArgs;
			if (clusterPropertiesEventArgs.Error != null)
			{
				break;
			}
			IEnumerable<string> propertiesChanged = ParseProperties(clusterPropertiesEventArgs.Properties, trackChanges: true);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				propertiesChanged.ForEach(delegate(string propertyChanged)
				{
					OnPropertyChanged(propertyChanged);
					if (propertyChanged.Equals("MaintenanceMode", StringComparison.OrdinalIgnoreCase))
					{
						this.MaintenanceModeChanged.SafeCall(this, new ClusterMaintenanceModeEventArgs(Id, maintenanceMode.Value, null));
					}
				});
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		case EventType.ResourceIsQuorumChanged:
			if (e.EventArgument is ClusterResourceIsQuorumChangedEventArgs clusterResourceIsQuorumChangedEventArgs && clusterResourceIsQuorumChangedEventArgs.Error == null)
			{
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("AssignedToDisplayName");
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		case EventType.ResourceStoragePoolInfoChanged:
			if (e.EventArgument is ClusterStoragePoolInfoChangedEventArgs clusterStoragePoolInfoChangedEventArgs && clusterStoragePoolInfoChangedEventArgs.Error == null)
			{
				poolInfo = clusterStoragePoolInfoChangedEventArgs.PoolInfo;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("PoolInfo");
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		case EventType.ResourceOwnerGroupChanged:
		{
			ClusterDisk clusterDisk = memberDiskInfo;
			if (clusterDisk != null && clusterDisk.Partitions != null)
			{
				clusterDisk.Partitions.ForEach(delegate(ClusterDiskPartition p)
				{
					p.UpdateCommandCanExecute();
				});
			}
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("AssignedToDisplayName");
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		case EventType.StorageReplicationDiskTypeChanged:
			if (e.EventArgument is ClusterStorageReplicationDiskTypeChangedEventArgs clusterStorageReplicationDiskTypeChangedEventArgs)
			{
				replicationDiskType = clusterStorageReplicationDiskTypeChangedEventArgs.ReplicationDiskType;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("ReplicationDiskType");
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		case EventType.StorageReplicationStatusChanged:
			if (e.EventArgument is ClusterStorageReplicationStatusChangedEventArgs clusterStorageReplicationStatusChangedEventArgs)
			{
				replicationStatus = clusterStorageReplicationStatusChangedEventArgs.ReplicationStatus;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("ReplicationStatus");
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		case EventType.StorageReplicationInfoChanged:
		{
			ClusterStorageReplicationInfoChangedEventArgs args2 = e.EventArgument as ClusterStorageReplicationInfoChangedEventArgs;
			if (args2 != null)
			{
				replicationInfo = args2.ReplicationInfo;
				if (replicationInfo != null)
				{
					replicationInfo.StorageResource = this;
				}
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("ReplicationInfo");
					this.StorageReplicationInfoChanged.SafeCall(this, args2);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.ResourceStateChanged:
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("DiskInfo");
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private void RemoveReplication(StorageResource resource)
	{
		if (resource.ReplicationDiskType == ReplicationDiskType.Source && resource.ReplicationInfo.ReplicationPrivateStorageResources != null && resource.ReplicationInfo.ReplicationPrivateStorageResources.Count <= 2)
		{
			ClusterDialogException.ShowTaskDialog(new ClusterReplicationNotSupportedException(ExceptionResources.ClusterReplicationSupportedException_Remove_ClusterToCluster_Header, ExceptionResources.ClusterReplicationSupportedException_AddPartner_ClusterToCluster_Text));
			return;
		}
		ConfirmationDialog confirmation = new ConfirmationDialog
		{
			CustomIcon = Icon.NativeIcon,
			Caption = DialogResources.StopReplication_Title,
			Header = DialogResources.StopReplication_Header.FormatCurrentCulture(resource.DisplayName),
			Content = DialogResources.StopReplication_Content
		};
		if (confirmation.ShowDialog() == TaskDialogResult.Yes)
		{
			resource.ExecuteMethod(delegate(ILockable lockObject)
			{
				((PStorageResource)lockObject.Owner).RemoveReplication(confirmation.IsFooterChecked);
			}, base.SetLastError, LockAccess.Reader);
		}
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		maintenanceMode = null;
		memberDiskInfo = null;
		replicationInfo = null;
		replicationDiskType = ReplicationDiskType.None;
		replicationStatus = null;
		poolInfo = null;
		poolId = null;
		virtualDiskId = null;
		virtualDiskName = null;
		virtualDiskDescription = null;
		virtualDiskHealth = null;
		virtualDiskProvisioning = null;
		virtualDiskResiliencyType = null;
		virtualDiskResiliencyColumns = null;
		virtualDiskResiliencyInterleave = null;
		virtualDiskState = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("MaintenanceMode");
			OnPropertyChanged("PassThroughDisk");
			OnPropertyChanged("PoolInfo");
			OnPropertyChanged("PoolId");
			OnPropertyChanged("VirtualDiskId");
			OnPropertyChanged("VirtualDiskName");
			OnPropertyChanged("VirtualDiskDescription");
			OnPropertyChanged("VirtualDiskHealth");
			OnPropertyChanged("VirtualDiskProvisioning");
			OnPropertyChanged("VirtualDiskResiliencyType");
			OnPropertyChanged("VirtualDiskResiliencyColumns");
			OnPropertyChanged("VirtualDiskResiliencyInterleave");
			OnPropertyChanged("VirtualDiskState");
			OnPropertyChanged("StorageCaps");
			OnPropertyChanged("ReplicationDiskType");
			OnPropertyChanged("ReplicationStatus");
			OnPropertyChanged("ReplicationInfo");
			this.MaintenanceModeChanged.SafeCall(this, null);
			this.StoragePropertiesChanged.SafeCall(this, null);
			this.StorageReplicationInfoChanged.SafeCall(this, null);
		}, OperationType.Async);
	}

	internal void SetMaintenanceMode(bool storageResourceMaintenanceMode, bool askConfirmation = false)
	{
		SetMaintenanceMode(base.SetLastError, storageResourceMaintenanceMode, askConfirmation);
	}

	internal void SetMaintenanceMode(Action<OperationResult> toggleMaintennaceModeOperation, bool storageResourceMaintenanceMode, bool askConfirmation = false)
	{
		if (askConfirmation && this is CsvVolumeResource && CreateConfirmationDialog(CommandResources.MaintenanceModeOn_Text.RemoveAccelerator(), DialogResources.StorageCSVMaintenanceOn_Header.FormatCurrentCulture(base.Name), DialogResources.StorageCSVMaintenanceOn_Content.FormatCurrentCulture(base.Name)).ShowDialog() != TaskDialogResult.Yes)
		{
			return;
		}
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			if (lockObject.Owner is PStorageResource pStorageResource)
			{
				pStorageResource.SetMaintenanceMode(storageResourceMaintenanceMode);
			}
		}, toggleMaintennaceModeOperation, LockAccess.Reader);
	}

	protected override void AskForResourceLockOverride(string item, Action overrideAction)
	{
		if (MaintenanceMode == true)
		{
			if (CreateConfirmationDialog(DialogResources.OverrideMaintenanceMode_ConfirmationTitle, DialogResources.OverrideMaintenanceMode_ConfirmationMessage.FormatCurrentCulture(item)).ShowDialog() == TaskDialogResult.Yes)
			{
				overrideAction();
			}
		}
		else
		{
			base.AskForResourceLockOverride(item, overrideAction);
		}
	}
}
