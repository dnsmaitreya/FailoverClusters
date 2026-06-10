using System;
using System.Collections.Generic;
using System.Linq;
using FailoverClusters.UI.Common;
using FileServer.Management.ServerManagerProxy;
using Windows.Server;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;
using KDDSL.ServerClusters;

namespace FailoverClusters.Framework;

public class StoragePoolResource : AverageResource
{
	private WeakReferenceEx addDisksCommandWeak;

	private WeakReferenceEx createDisksCommandWeak;

	private WeakReferenceEx moveStoragePoolResourceCommandWeak;

	private AverageGroupMoveCommand groupMoveCommand;

	private bool newVirtualDiskCommandEnabled = true;

	private StoragePoolHealth? health;

	private string poolName;

	private string pooldescription;

	private StoragePoolQuorum? quorum;

	private ulong? freeCapacity;

	private ulong? totalCapacity;

	private string driveIds;

	private ulong? consumedCapacity;

	private IEnumerable<PoolPhysicalDiskInfo> physicalDisksInfo;

	private WeakReferenceEx virtualDisksWeak;

	public StoragePoolHealth Health => LoadAsync<StoragePoolHealth, StoragePoolHealth>(health, 4);

	public string PoolName => LoadAsync(poolName, 4);

	public string PoolDescription => LoadAsync(pooldescription, 4);

	public StoragePoolQuorum State => LoadAsync<StoragePoolQuorum, StoragePoolQuorum>(quorum, 4);

	public ulong? FreeCapacity
	{
		get
		{
			LoadAsync(totalCapacity, 4);
			return freeCapacity;
		}
	}

	public IEnumerable<PoolPhysicalDiskInfo> PhysicalDisksInfo => LoadAsync(physicalDisksInfo, 1024);

	public StorageSize FreeCapacityStorageSize => FormatHelp.GetStorageSizeFromULong(FreeCapacity);

	public ulong? TotalCapacity => LoadAsync(totalCapacity, 4);

	public string DriveIds => LoadAsync(driveIds, 4);

	public StorageSize TotalCapacityStorageSize => FormatHelp.GetStorageSizeFromULong(TotalCapacity);

	public ulong? ConsumedCapacity => LoadAsync(consumedCapacity, 4);

	public StorageSize ConsumedCapacityStorageSize => FormatHelp.GetStorageSizeFromULong(ConsumedCapacity);

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.StoragePool));

	public virtual ClusterList<Resource> VirtualDisks => WeakReferenceEx.ReturnInstance(ref virtualDisksWeak, delegate
	{
		Guid poolId = Id;
		return (ClusterList<Resource>)(from r in new ClusterList<Resource>(base.Cluster)
			{
				Name = "All PDRs for with a specific PoolId"
			}
			where ((int)r.ResourceType.ResourceKind == 3 || (int)r.ResourceType.ResourceKind == 65539) && r.PoolId == poolId
			orderby r.Name
			select r);
	});

	public override void Offline(bool enableOverride)
	{
		if (CreateConfirmationDialog(DialogResources.StoragePoolOffline_Title, DialogResources.StorageOffline_Header.FormatCurrentCulture(base.Name), DialogResources.StoragePoolOffline_Content).ShowDialog() == TaskDialogResult.Yes)
		{
			base.Offline(enableOverride);
		}
	}

	internal static CommandCollection InitializeCommands(Cluster cluster, IEnumerable<StoragePoolResource> resources)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceState)
		{
			Name = "Multi StorageResource State Commands"
		};
		ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(null, "MoveMultipleStoragePoolResource", ClusterCommandId.MultipleStoragePoolResourceMoveTo, commandCollection.Category)
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
		ClusterCommand item = new ClusterCommand(null, "MultipleStoragePoolResourceMoveToBestNode", ClusterCommandId.MultipleStoragePoolResourceMoveToBestNode, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_MoveToBestPossible,
			InputParameters = null,
			CommandParameter = null,
			Description = string.Empty,
			ExecuteDelegate = delegate(object node)
			{
				List<GroupMoveDescriptor> groupMoveDescriptors2 = new List<GroupMoveDescriptor>();
				resources.OfType<StoragePoolResource>().ToList().ForEach(delegate(StoragePoolResource storagePoolResource)
				{
					groupMoveDescriptors2.Add(new GroupMoveDescriptor
					{
						ClusterObjectIssuingMoveRequest = storagePoolResource,
						OwnerGroup = storagePoolResource.OwnerGroup
					});
				});
				GroupMoveCommandBase.DefaultExecuteMove(node, groupMoveDescriptors2);
			}
		};
		ClusterCommand item2 = new ClusterCommand(null, "MultipleStoragePoolResourceMoveToSelectedNode", ClusterCommandId.MultipleStoragePoolResourceMoveToSelectedNode, ClusterCommandCollectionId.GroupOwnership)
		{
			Text = CommandResources.Group_MoveToSelected,
			InputParameters = (ClusterList<Node>)clusterList,
			CommandParameter = null,
			Description = string.Empty,
			ExecuteDelegate = delegate(object node)
			{
				List<GroupMoveDescriptor> groupMoveDescriptors = new List<GroupMoveDescriptor>();
				resources.OfType<StoragePoolResource>().ToList().ForEach(delegate(StoragePoolResource storagePoolResource)
				{
					groupMoveDescriptors.Add(new GroupMoveDescriptor
					{
						ClusterObjectIssuingMoveRequest = storagePoolResource,
						OwnerGroup = storagePoolResource.OwnerGroup
					});
				});
				GroupMoveCommandBase.DefaultExecuteMove(node, groupMoveDescriptors);
			}
		};
		clusterCommandContainer.ChildrenInternal.Add(item);
		clusterCommandContainer.ChildrenInternal.Add(item2);
		commandCollection.Add(clusterCommandContainer);
		return commandCollection;
	}

	protected override void InitializeResourceCommands(CommandCollection commandsCollection)
	{
		if (base.OwnerGroup as AverageGroup == null)
		{
			throw new InvalidOperationException("Owner group must be an average group.");
		}
		groupMoveCommand = new AverageGroupMoveCommand(base.OwnerGroup, this);
		ClusterCommand item = groupMoveCommand.AverageGroupMoveToBestCommand;
		ClusterCommand item2 = groupMoveCommand.AverageGroupMoveToSelectedCommand;
		ClusterCommandContainer clusterCommandContainer = WeakReferenceEx.ReturnInstance(ref moveStoragePoolResourceCommandWeak, () => new ClusterCommandContainer(this, "MoveStoragePoolResource", ClusterCommandId.GroupMoveTo, commandsCollection.Category)
		{
			Text = CommandResources.MoveResourceAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration.");
			},
			CommandParameter = base.OwnerGroup
		});
		base.OwnerGroupChanged += OnOwnerGroupChangedForGroupMoveCommand;
		clusterCommandContainer.Finalizing += delegate
		{
			base.OwnerGroupChanged -= OnOwnerGroupChangedForGroupMoveCommand;
			moveStoragePoolResourceCommandWeak = null;
		};
		clusterCommandContainer.ChildrenInternal.Add(item);
		clusterCommandContainer.ChildrenInternal.Add(item2);
		commandsCollection.Add(clusterCommandContainer);
		ClusterCommand item3 = WeakReferenceEx.ReturnInstance(ref addDisksCommandWeak, () => new ClusterCommand(this, "AddDisks", ClusterCommandId.StoragePoolAddDisks, ClusterCommandCollectionId.ResourceGeneral)
		{
			Text = CommandResources.AddPooledDiskAction_Text,
			CanExecuteDelegate = (object x) => base.ResourceState == ResourceState.Online,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration.");
			},
			CommandParameter = this
		});
		commandsCollection.Add(item3);
		ClusterCommand item4 = WeakReferenceEx.ReturnInstance(ref createDisksCommandWeak, () => new ClusterCommand(this, "CreateDisk", ClusterCommandId.StoragePoolCreateDisks, ClusterCommandCollectionId.ResourceGeneral)
		{
			Text = CommandResources.CreatePooledDiskAction_Text,
			CanExecuteDelegate = (object x) => base.ResourceState == ResourceState.Online && newVirtualDiskCommandEnabled,
			ExecuteDelegate = delegate
			{
				ServerManagerProxy.StartStorageWizardAsync(StorageWizardType.VirtualDiskWizard, base.Cluster, delegate
				{
					UpdateNewVirtualDisksCommandEnabledState(state: false);
				}, delegate
				{
					UpdateNewVirtualDisksCommandEnabledState(state: true);
				});
			},
			CommandParameter = this
		});
		commandsCollection.Add(item4);
		base.InitializeResourceCommands(commandsCollection);
	}

	private void OnOwnerGroupChangedForGroupMoveCommand(object sender, ClusterResourceOwnerGroupEventArgs args)
	{
		WeakReferenceEx weakReferenceEx = moveStoragePoolResourceCommandWeak;
		if (weakReferenceEx != null && weakReferenceEx.Target is ClusterCommandContainer clusterCommandContainer)
		{
			clusterCommandContainer.CommandParameter = base.OwnerGroup;
			foreach (ClusterCommand child in clusterCommandContainer.Children)
			{
				child.SetClusterObject(base.OwnerGroup);
				child.CommandParameter = base.OwnerGroup;
			}
		}
		if (groupMoveCommand != null)
		{
			groupMoveCommand.AverageGroupMoveToBestCommand.ClusterGroup = base.OwnerGroup;
			groupMoveCommand.AverageGroupMoveToSelectedCommand.ClusterGroup = base.OwnerGroup;
		}
	}

	private void UpdateNewVirtualDisksCommandEnabledState(bool state)
	{
		newVirtualDiskCommandEnabled = state;
		WeakReferenceEx weakReferenceEx = createDisksCommandWeak;
		if (weakReferenceEx != null && weakReferenceEx.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.CanExecuteUpdate(null, null);
		}
	}

	internal StoragePoolResource(Cluster cluster)
		: base(cluster)
	{
	}

	public void RefreshPhysicalDisksInfo()
	{
		LoadAsync(physicalDisksInfo, 536871936);
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents, ignorePossibleOwners);
		ParseProperties(privateObject.Properties, trackChanges: false);
		PStoragePoolResource pStoragePoolResource = (PStoragePoolResource)privateObject;
		List<PoolPhysicalDiskInfo> newPhysicalDisksInfo = new List<PoolPhysicalDiskInfo>();
		if (pStoragePoolResource.PhysicalDisksInfo != null)
		{
			pStoragePoolResource.PhysicalDisksInfo.ForEach(delegate(PoolPhysicalDiskInfoInternal d)
			{
				newPhysicalDisksInfo.Add(new PoolPhysicalDiskInfo(d, this));
			});
			physicalDisksInfo = newPhysicalDisksInfo;
		}
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
				baseConfirmation.Caption = DialogResources.RemovePoolTitle;
				baseConfirmation.Header = DialogResources.RemovePoolHeader;
				baseConfirmation.Content = DialogResources.RemovePoolContent;
				confirmationDialogCreation(baseConfirmation);
			}
		}, createDialog);
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		ClusterProperty clusterProperty = properties["Name"];
		if (clusterProperty != null && clusterProperty.PropertyKind == ClusterPropertyKind.Private)
		{
			ParseProperty(properties, "Name", ref poolName, list);
		}
		else
		{
			clusterProperty = properties["Name_"];
			if (clusterProperty != null && clusterProperty.PropertyKind == ClusterPropertyKind.Private)
			{
				ParseProperty(properties, "Name_", ref poolName, list);
			}
		}
		ClusterProperty clusterProperty2 = properties["Description"];
		if (clusterProperty2 != null && clusterProperty2.PropertyKind == ClusterPropertyKind.Private)
		{
			ParseProperty(properties, "Description", ref pooldescription, list);
		}
		else
		{
			clusterProperty2 = properties["Description_"];
			if (clusterProperty2 != null && clusterProperty2.PropertyKind == ClusterPropertyKind.Private)
			{
				ParseProperty(properties, "Description_", ref pooldescription, list);
			}
		}
		ParseProperty(properties, "Health", ref health, list);
		ParseProperty(properties, "State", ref quorum, list);
		ParseProperty(properties, "DriveIds", ref driveIds, list);
		if (ParseProperty(properties, "TotalCapacity", ref totalCapacity, list) && list != null)
		{
			list.TryAdd("TotalCapacityStorageSize");
			list.TryAdd("FreeCapacity");
			list.TryAdd("FreeCapacityStorageSize");
		}
		if (ParseProperty(properties, "ConsumedCapacity", ref consumedCapacity, list) && list != null)
		{
			list.TryAdd("ConsumedCapacityStorageSize");
			list.TryAdd("FreeCapacity");
			list.TryAdd("FreeCapacityStorageSize");
		}
		if (totalCapacity.HasValue && consumedCapacity.HasValue)
		{
			freeCapacity = totalCapacity - consumedCapacity;
		}
		return list;
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs clusterPropertiesEventArgs = e.EventArgument as ClusterPropertiesEventArgs;
			if (clusterPropertiesEventArgs.Error != null)
			{
				if (clusterPropertiesEventArgs.Error is ClusterPropertiesNotAvailableException)
				{
					health = StoragePoolHealth.Unknown;
					quorum = StoragePoolQuorum.Unknown;
				}
			}
			else
			{
				IEnumerable<string> propertiesChanged = ParseProperties(clusterPropertiesEventArgs.Properties, trackChanges: true);
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					propertiesChanged.ForEach(base.OnPropertyChanged);
				}, OperationType.Async, queueOnDispatcher);
			}
			break;
		}
		case EventType.PoolPhysicalDisksInfoChanged:
		{
			List<PoolPhysicalDiskInfo> newPhysicalDisksInfo = new List<PoolPhysicalDiskInfo>();
			ClusterStoragePoolPhysicalDisksInfoChangedEventArgs clusterStoragePoolPhysicalDisksInfoChangedEventArgs = e.EventArgument as ClusterStoragePoolPhysicalDisksInfoChangedEventArgs;
			if (clusterStoragePoolPhysicalDisksInfoChangedEventArgs.Error == null)
			{
				if (clusterStoragePoolPhysicalDisksInfoChangedEventArgs.PoolPhysicalDisks != null)
				{
					clusterStoragePoolPhysicalDisksInfoChangedEventArgs.PoolPhysicalDisks.ForEach(delegate(PoolPhysicalDiskInfoInternal d)
					{
						newPhysicalDisksInfo.Add(new PoolPhysicalDiskInfo(d, this));
					});
				}
				else
				{
					base.LoadSelection &= -1025;
					newPhysicalDisksInfo = null;
				}
			}
			else
			{
				Error = clusterStoragePoolPhysicalDisksInfoChangedEventArgs.Error;
			}
			physicalDisksInfo = newPhysicalDisksInfo;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("PhysicalDisksInfo");
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		health = null;
		poolName = null;
		pooldescription = null;
		quorum = null;
		freeCapacity = null;
		totalCapacity = null;
		consumedCapacity = null;
		driveIds = null;
		physicalDisksInfo = null;
		virtualDisksWeak = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("Health");
			OnPropertyChanged("PoolName");
			OnPropertyChanged("PoolDescription");
			OnPropertyChanged("Quorum");
			OnPropertyChanged("FreeCapacity");
			OnPropertyChanged("FreeCapacityStorageSize");
			OnPropertyChanged("TotalCapacity");
			OnPropertyChanged("DriveIds");
			OnPropertyChanged("TotalCapacityStorageSize");
			OnPropertyChanged("ConsumedCapacity");
			OnPropertyChanged("ConsumedCapacityStorageSize");
			OnPropertyChanged("PhysicalDisksInfo");
			OnPropertyChanged("VirtualDisks");
		}, OperationType.Async);
	}
}

