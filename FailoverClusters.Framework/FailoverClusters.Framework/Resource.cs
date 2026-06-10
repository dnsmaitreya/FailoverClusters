using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Threading;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public abstract class Resource : ClusterObject
{
	private WeakReferenceEx resourceCommandsWeak;

	private WeakReferenceEx stateCommandsWeak;

	private WeakReferenceEx moreActionsCommandWeak;

	private WeakReferenceEx deleteCommandWeak;

	private WeakReferenceEx onlineCommandWeak;

	private WeakReferenceEx offlineCommandWeak;

	private WeakReferenceEx failCommandWeak;

	private WeakReferenceEx moveCommandWeak;

	public const uint LooksAlivePollIntervalMaximum = uint.MaxValue;

	public const uint LooksAlivePollIntervalMinimum = 10u;

	public const uint IsAlivePollIntervalMaximum = uint.MaxValue;

	public const uint IsAlivePollIntervalMinimum = 10u;

	public const uint RetryPeriodOnFailureMaximum = uint.MaxValue;

	private ResourceState? resourceState;

	private ResourceSubStatus? subStatus;

	private ulong? lastOperationStatusCode;

	private string resourceSpecificStatus = string.Empty;

	private Group ownerGroup;

	private ResourceClass resourceClass;

	private ResourceSubclass resourceSubclass;

	private ResourceFlags? flags;

	private Characteristics? characteristics;

	private IEnumerable<Guid> dependencies;

	private IEnumerable<Guid> dependents;

	private DependencyRelationship dependencyRelationship;

	private RequiredDependencies requiredDependencies;

	private ReadOnlyObservableCollection<Node> possibleOwners;

	private WeakReferenceEx moveTargetsWeak;

	protected WeakReferenceEx childrenWeak;

	protected bool? isChildResource;

	protected Icon2 resourceIcon;

	public virtual CommandCollection ResourceCommands => WeakReferenceEx.ReturnInstance(ref resourceCommandsWeak, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.Resource);
		InitializeResourceCommands(commandCollection);
		return commandCollection;
	});

	public virtual CommandCollection StateCommands => WeakReferenceEx.ReturnInstance(ref stateCommandsWeak, delegate
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceState);
		InitializeStateCommands(commandCollection);
		return commandCollection;
	});

	public virtual CommandCollection ApplicationCommands => StateCommands;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Resource;

	public bool IsQuorum { get; private set; }

	public virtual bool? IsChild => false;

	[PropertyStore("ApplicationStatusBackStore")]
	[Column(Name = "ApplicationStatus", Expression = "ApplicationStatus")]
	public virtual object ApplicationStatus => ResourceState;

	[Column(Name = "ResourceProperties")]
	public override ClusterPropertyCollection Properties => base.Properties;

	[Column(Name = "State")]
	public ResourceState ResourceState
	{
		get
		{
			return LoadAsync<ResourceState, ResourceState>(resourceState, 1);
		}
		set
		{
			if (resourceState != value)
			{
				ChangeStateInternal(value, base.PropertiesOperationType);
			}
		}
	}

	public ResourceSubStatus? ResourceSubStatus => LoadAsync(subStatus, 2, () => FailoverClusters.Framework.ResourceSubStatus.None);

	[Column(Name = "Type", AutoSync = AutoSync.Always)]
	public ResourceType ResourceType { get; internal set; }

	public ulong? LastOperationStatusCode => LoadAsync(lastOperationStatusCode, 2);

	[Column(Name = "OwnerGroup")]
	public Group OwnerGroup => LoadAsync(ownerGroup, 1);

	public ReadOnlyObservableCollection<Node> PossibleOwners
	{
		get
		{
			return LoadAsync(possibleOwners, 128);
		}
		set
		{
			if (possibleOwners == value)
			{
				return;
			}
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				((PResource)lockObject.Owner).SetPossibleOwners(value.ConvertAll((Node node) => node.Id));
			}, null, LockAccess.Reader);
		}
	}

	[Column(Name = "Flags")]
	public ResourceFlags Flags => LoadAsync<ResourceFlags, ResourceFlags>(flags, 1);

	[Column(Name = "Characteristics")]
	public Characteristics Characteristics => LoadAsync<Characteristics, Characteristics>(characteristics, 1);

	[Column(Name = "Resourceclass")]
	public ResourceClass Class => resourceClass;

	public ResourceSubclass Subclass => resourceSubclass;

	public virtual Guid? PoolId
	{
		get
		{
			throw new NotSupportedException("Pool Id is not supported for resources other than Storage Resources");
		}
		internal set
		{
			throw new NotSupportedException("Pool Id is not supported for resources other than Storage Resources");
		}
	}

	public virtual ClusterList<Resource> Children => WeakReferenceEx.ReturnInstance(ref childrenWeak, delegate
	{
		if (IsChild == true)
		{
			return new ClusterList<Resource>(base.Cluster);
		}
		Guid id = Id;
		Group ownerGroup = OwnerGroup;
		return (ClusterList<Resource>)new ClusterList<Resource>(base.Cluster)
		{
			Name = "Children Resources of a Resource"
		}.Where((Resource r) => r.OwnerGroup == ownerGroup && r.Dependents != null && r.IsChild == (bool?)true && r.Dependents.Contains(id));
	});

	public IEnumerable<Guid> Dependencies
	{
		get
		{
			if (dependencies == null)
			{
				LoadAsync(ResourceLoadSelection.Dependencies);
			}
			return dependencies;
		}
	}

	public string OwnerNodeName
	{
		get
		{
			Group group = OwnerGroup;
			if (group != null)
			{
				return group.OwnerNodeName;
			}
			return string.Empty;
		}
	}

	public IEnumerable<Guid> Dependents
	{
		get
		{
			if (dependents == null)
			{
				LoadAsync(ResourceLoadSelection.Dependents);
			}
			return dependents;
		}
	}

	public DependencyRelationship DependencyRelationship
	{
		get
		{
			if (dependencyRelationship == null)
			{
				LoadAsync(ResourceLoadSelection.Dependencies | ResourceLoadSelection.DependenciesRelation);
			}
			return dependencyRelationship;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			ChangeDependencyRelationshipInternal(value, base.PropertiesOperationType);
		}
	}

	public RequiredDependencies RequiredDependencies
	{
		get
		{
			if (requiredDependencies == null)
			{
				LoadAsync(ResourceLoadSelection.RequiredDependencies);
			}
			return requiredDependencies;
		}
	}

	private object ApplicationStatusBackStore { get; set; }

	public override string Information
	{
		get
		{
			string text = base.Information;
			if (!string.IsNullOrWhiteSpace(resourceSpecificStatus))
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					return "{0} - {1}".FormatCurrentCulture(text, resourceSpecificStatus);
				}
				return resourceSpecificStatus;
			}
			return text;
		}
	}

	internal override Type OwnerType => typeof(PResource);

	public event EventHandler<ClusterApplicationStatusEventArgs> ApplicationStatusChanged;

	public event EventHandler<ClusterResourceStateEventArgs> StateChanged;

	public event EventHandler<ClusterResourceOwnerGroupEventArgs> OwnerGroupChanged;

	public event EventHandler<ClusterResourceFlagsEventArgs> FlagsChanged;

	public event EventHandler<ClusterCharacteristicsEventArgs> CharacteristicsChanged;

	public event EventHandler<ClusterDependenciesEventArgs> DependenciesChanged;

	public event EventHandler<ClusterDependentsEventArgs> DependentsChanged;

	public event EventHandler<ClusterDependencyRelationshipEventArgs> DependencyRelationshipChanged;

	public event EventHandler<ClusterRequiredDependenciesEventArgs> RequiredDependenciesChanged;

	public event EventHandler<ClusterResourcePossibleOwnersChangedEventArgs> PossibleOwnersChanged;

	public event EventHandler<ClusterResourceIsQuorumChangedEventArgs> IsQuorumChanged;

	protected override void InitializeCommands(CommandCollection collection)
	{
		base.InitializeCommands(collection);
		foreach (ClusterCommand applicationCommand in ApplicationCommands)
		{
			collection.Add(applicationCommand);
		}
		foreach (ClusterCommand miscellaneousCommand in MiscellaneousCommands)
		{
			collection.Add(miscellaneousCommand);
		}
		foreach (ClusterCommand resourceCommand in ResourceCommands)
		{
			collection.Add(resourceCommand);
		}
	}

	protected IClusterList<Group> GetResourceMoveTargets()
	{
		Guid ownerGroupId = OwnerGroup.Id;
		return WeakReferenceEx.ReturnInstance(ref moveTargetsWeak, () => ((ClusterList<Group>)base.Cluster.Groups.Where((Group g) => g.IsCore == (bool?)false && (int)g.GroupType != 4 && (int)g.GroupType != 116 && (int)g.GroupType != 2 && (int)g.GroupType != 5 && (int)g.GroupType != 117 && (int)g.GroupType != 120 && g.Id != ownerGroupId)).ForceLoadStart());
	}

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		base.InitializeMoreActionsCommands(commandContainer);
		ClusterCommand item = new ClusterCommand(this, "ShowDependencyReport", ClusterCommandId.ResourceShowDependencyReport, ClusterCommandCollectionId.ResourceGeneral)
		{
			Text = CommandResources.ShowDependencyReport,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for the Show Dependency Report.");
			},
			CommandParameter = this
		};
		commandContainer.ChildrenInternal.Add(item);
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref failCommandWeak, () => new ClusterCommand(this, "Failure", ClusterCommandId.ResourceSimulateFailure, ClusterCommandCollectionId.ResourceMoreActions)
		{
			Text = EnumResources.ResourceState_Set_Failed,
			CanExecuteDelegate = (object x) => ResourceState != ResourceState.Offline && ResourceState != ResourceState.Failed,
			ExecuteDelegate = delegate
			{
				ResourceState = ResourceState.Failed;
			}
		});
		StateChanged += FailureCommandUpdate;
		clusterCommand.Finalizing += delegate
		{
			StateChanged -= FailureCommandUpdate;
			failCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(clusterCommand);
		if (this is CsvVolumeResource || Flags.HasFlag(ResourceFlags.Core) || Characteristics.HasFlag(Characteristics.Infrastructure) || this is StoragePoolResource)
		{
			return;
		}
		ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref moveCommandWeak, delegate
		{
			ClusterList<Group> moveTargets = (ClusterList<Group>)GetResourceMoveTargets();
			moveTargets.CollectionChanged += MoveCommandUpdate;
			return new ClusterCommand(this, "Move", ClusterCommandId.ResourceMoveTo, ClusterCommandCollectionId.ResourceMoreActions)
			{
				InputParameters = moveTargets,
				CommandParameter = this,
				Text = CommandResources.AssignToAnotherRole_Text,
				CanExecuteDelegate = (object x) => (!(this is StorageResource) || (!(OwnerGroup == null) && !(OwnerGroup is ClusterSharedVolumeGroup))) && moveTargets.Count > 0,
				ExecuteDelegate = delegate(object group)
				{
					Move(group as Group);
				}
			};
		});
		OwnerGroupChanged += MoveCommandUpdate;
		clusterCommand2.Finalizing += delegate
		{
			OwnerGroupChanged -= MoveCommandUpdate;
			moveCommandWeak = null;
		};
		commandContainer.ChildrenInternal.Add(clusterCommand2);
	}

	protected virtual void InitializeResourceCommands(CommandCollection commandsCollection)
	{
		ClusterCommandContainer item = WeakReferenceEx.ReturnInstance(ref moreActionsCommandWeak, delegate
		{
			ClusterCommandContainer clusterCommandContainer = new ClusterCommandContainer(this, "ResourceMoreActions", ClusterCommandId.ResourceMoreActions)
			{
				Text = CommandResources.MoreActions,
				CommandParameter = this,
				ExecuteIfNoChildren = false
			};
			InitializeMoreActionsCommands(clusterCommandContainer);
			return clusterCommandContainer;
		});
		commandsCollection.Add(item);
		if (!Characteristics.HasFlag(Characteristics.Infrastructure))
		{
			ClusterCommand item2 = WeakReferenceEx.ReturnInstance(ref deleteCommandWeak, () => new ClusterCommand(this, "Delete", ClusterCommandId.ResourceDelete, commandsCollection.Category)
			{
				Text = CommandResources.RemoveCommand_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					Delete(askConfirmation: true);
				}
			});
			commandsCollection.Add(item2);
		}
		ClusterCommand item3 = new ClusterCommand(this, "ResourceProperties", ClusterCommandId.ResourceProperties, ClusterCommandCollectionId.ResourceProperties)
		{
			Text = CommandResources.Properties,
			ExecuteDelegate = delegate
			{
				throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration for show properties.");
			},
			CommandParameter = this
		};
		commandsCollection.Add(item3);
	}

	protected virtual void InitializeStateCommands(CommandCollection commandsCollection)
	{
		ClusterCommand clusterCommand = WeakReferenceEx.ReturnInstance(ref onlineCommandWeak, () => new ClusterCommand(this, "Online", ClusterCommandId.ResourceOnline, commandsCollection.Category)
		{
			Text = CommandResources.BringResourceOnlineAction_Text,
			CanExecuteDelegate = (object x) => ResourceState != ResourceState.Online && ResourceState != ResourceState.Pending && ResourceState != ResourceState.Fetching,
			ExecuteDelegate = delegate
			{
				Online(enableOverride: true);
			}
		});
		StateChanged += OnlineCommandUpdate;
		clusterCommand.Finalizing += delegate
		{
			StateChanged -= OnlineCommandUpdate;
			onlineCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand);
		ClusterCommand clusterCommand2 = WeakReferenceEx.ReturnInstance(ref offlineCommandWeak, () => new ClusterCommand(this, "Offline", ClusterCommandId.ResourceOffline, commandsCollection.Category)
		{
			Text = CommandResources.TakeResourceOfflineAction_Text,
			CanExecuteDelegate = (object x) => ResourceState == ResourceState.Online && ResourceState != ResourceState.Fetching,
			ExecuteDelegate = delegate
			{
				Offline(enableOverride: true);
			}
		});
		StateChanged += OfflineCommandUpdate;
		clusterCommand2.Finalizing += delegate
		{
			StateChanged -= OfflineCommandUpdate;
			offlineCommandWeak = null;
		};
		commandsCollection.Add(clusterCommand2);
	}

	private void OnlineCommandUpdate(object sender, ClusterResourceStateEventArgs e)
	{
		StateCommandUpdate(onlineCommandWeak, sender, e);
	}

	private void OfflineCommandUpdate(object sender, ClusterResourceStateEventArgs e)
	{
		StateCommandUpdate(offlineCommandWeak, sender, e);
	}

	private void FailureCommandUpdate(object sender, ClusterResourceStateEventArgs e)
	{
		StateCommandUpdate(failCommandWeak, sender, e);
	}

	private void MoveCommandUpdate(object sender, EventArgs e)
	{
		WeakReferenceEx weakReferenceEx = moveCommandWeak;
		if (weakReferenceEx != null && weakReferenceEx.Target is ClusterCommand clusterCommand)
		{
			if (clusterCommand.Id == ClusterCommandId.ResourceMoveTo)
			{
				clusterCommand.InputParameters = (ClusterList<Group>)GetResourceMoveTargets();
			}
			clusterCommand.CanExecuteUpdate(sender, e);
		}
	}

	private static void StateCommandUpdate(WeakReferenceEx weakCommand, object sender, ClusterResourceStateEventArgs e)
	{
		if (weakCommand != null && weakCommand.Target is ClusterCommand clusterCommand)
		{
			clusterCommand.CanExecuteUpdate(sender, e);
		}
	}

	internal static CommandCollection InitializeStateCommands(IEnumerable<Resource> resources)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceState)
		{
			Name = "Multi Resource State Commands"
		};
		ClusterCommand item = new ClusterCommand(null, "MultiOnline", ClusterCommandId.MultipleResourceOnline, commandCollection.Category)
		{
			Text = CommandResources.BringResourceOnlineAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				BringOnline(resources);
			}
		};
		commandCollection.Add(item);
		ClusterCommand item2 = new ClusterCommand(null, "MultiOffline", ClusterCommandId.MultipleResourceOffline, commandCollection.Category)
		{
			Text = CommandResources.TakeResourceOfflineAction_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				TakeOffline(resources);
			}
		};
		commandCollection.Add(item2);
		if (resources.All((Resource resource) => !(resource is CsvVolumeResource)) && !resources.All((Resource resource) => resource is StoragePoolResource))
		{
			ClusterCommand item3 = new ClusterCommand(null, "Failure", ClusterCommandId.MultipleResourceSimulateFailure, commandCollection.Category)
			{
				Text = EnumResources.ResourceState_Set_Failed,
				CanExecuteDelegate = (object x) => true,
				CommandParameter = resources,
				ExecuteDelegate = delegate(object x)
				{
					SimulateFailureResources((IEnumerable<Resource>)x);
				}
			};
			commandCollection.Add(item3);
		}
		return commandCollection;
	}

	internal static CommandCollection InitializeApplicationCommands(Cluster cluster, ResourceKind resourceType, IEnumerable<Resource> resources)
	{
		return new CommandCollection(ClusterCommandCollectionId.MultipleResourceApplication)
		{
			Name = "Multi Resource Application Commands"
		};
	}

	internal static CommandCollection InitializeHeterogenousCommands(Cluster cluster, IEnumerable<Resource> resources)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceHeterogeneous)
		{
			Name = "Multi Resource Heterogenous Commands"
		};
		if (resources.All((Resource resource) => resource is StorageResource))
		{
			foreach (ClusterCommand item3 in StorageResource.InitializeCommands(cluster, resources.Cast<StorageResource>()))
			{
				commandCollection.Add(item3);
			}
		}
		else if (resources.All((Resource resource) => resource is StoragePoolResource))
		{
			foreach (ClusterCommand item4 in StoragePoolResource.InitializeCommands(cluster, resources.Cast<StoragePoolResource>()))
			{
				commandCollection.Add(item4);
			}
		}
		return commandCollection;
	}

	internal static CommandCollection InitializeHomogenousCommands(Cluster cluster, ResourceKind resourceKind, IEnumerable<Resource> resources)
	{
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceHomogenous)
		{
			Name = "Multi Resource Homogenous Commands"
		};
		if (resources.All((Resource resource) => resource is StorageResource))
		{
			foreach (ClusterCommand item3 in StorageResource.InitializeCommands(cluster, resources.Cast<StorageResource>()))
			{
				commandCollection.Add(item3);
			}
		}
		else if (resources.All((Resource resource) => resource is StoragePoolResource))
		{
			foreach (ClusterCommand item4 in StoragePoolResource.InitializeCommands(cluster, resources.Cast<StoragePoolResource>()))
			{
				commandCollection.Add(item4);
			}
		}
		return commandCollection;
	}

	internal static CommandCollection InitializeOtherCommands(Cluster cluster, IEnumerable<Resource> resources)
	{
		Utilities.UnreferencedParameter(cluster);
		CommandCollection commandCollection = new CommandCollection(ClusterCommandCollectionId.MultipleResourceOther)
		{
			Name = "Multi Resource Other Commands"
		};
		if (resources.All((Resource resource) => !(resource is StorageResource)))
		{
			ClusterCommand item = new ClusterCommand(null, "Delete", ClusterCommandId.MultipleResourceDelete, commandCollection.Category)
			{
				Text = CommandResources.RemoveCommand_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					DeleteResources(resources);
				}
			};
			commandCollection.Add(item);
		}
		return commandCollection;
	}

	private static void BringOnline(IEnumerable<Resource> resources)
	{
		EnqueueAndThrottleRequests(resources, delegate(Resource resource, Action<OperationResult> operationResult)
		{
			resource.Online(operationResult, resources.Take(2).Count() == 1);
		});
	}

	private static void TakeOffline(IEnumerable<Resource> resources)
	{
		EnqueueAndThrottleRequests(resources, delegate(Resource resource, Action<OperationResult> operationResult)
		{
			resource.Offline(operationResult, resources.Take(2).Count() == 1);
		});
	}

	public static void SimulateFailureResources(IEnumerable<Resource> resources)
	{
		EnqueueAndThrottleRequests(resources, delegate(Resource resource, Action<OperationResult> operationResult)
		{
			if (resource.ResourceState != ResourceState.Offline && resource.ResourceState != ResourceState.Failed)
			{
				resource.RedirectAsyncOutput(delegate
				{
					resource.ResourceState = ResourceState.Failed;
				}, delegate(OperationResult asyncResultOffline)
				{
					operationResult(new OperationResult(resource, asyncResultOffline.Error));
				});
			}
			else
			{
				operationResult(new OperationResult(resource, null));
			}
		});
	}

	public static void DeleteResources(IEnumerable<Resource> resources, bool askConfirmation = true)
	{
		if (resources.FirstOrDefault() == null)
		{
			return;
		}
		if (resources.All((Resource x) => x is StorageResource))
		{
			StorageResource.DeleteStorageResources(resources.Cast<StorageResource>(), askConfirmation);
		}
		else if (!askConfirmation || new ConfirmationDialog
		{
			Icon = TaskDialogStandardIcon.Question,
			Caption = DialogResources.DeleteMultipleResources_Title,
			Header = DialogResources.DeleteMultipleResources_Header,
			Content = DialogResources.DeleteMultipleResources_Content.FormatCurrentCulture(resources.Count())
		}.ShowDialog() == TaskDialogResult.Yes)
		{
			EnqueueAndThrottleRequests(resources, delegate(Resource resource, Action<OperationResult> operationResult)
			{
				resource.Delete(operationResult);
			});
		}
	}

	public void Online(bool enableOverride)
	{
		Online(base.SetLastError, enableOverride);
	}

	public void Online(Action<OperationResult> resourceOperationOnline, bool enableOverride)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PResource)lockObject.Owner).Online();
		}, delegate(OperationResult or)
		{
			ConfirmOverrideAndExecuteOnLocked(or, delegate(ILockable lo)
			{
				((PResource)lo.Owner).Online(overrideLockState: true);
			}, resourceOperationOnline, enableOverride, enableOverride);
		}, LockAccess.Reader, setErrorOnObject: false);
	}

	public virtual void Offline(bool enableOverride)
	{
		Offline(base.SetLastError, enableOverride);
	}

	public virtual void Offline(Action<OperationResult> resourceOperationOffline, bool enableOverride)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PResource)lockObject.Owner).Offline();
		}, delegate(OperationResult or)
		{
			ConfirmOverrideAndExecuteOnLocked(or, delegate(ILockable lo)
			{
				((PResource)lo.Owner).Offline(overrideLockState: true);
			}, resourceOperationOffline, enableOverride, enableOverride);
		}, LockAccess.Reader, setErrorOnObject: false);
	}

	public void Move(Group group)
	{
		Move(group, base.SetLastError);
	}

	public void Move(Group group, Action<OperationResult> resourceOperationMove)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PResource)lockObject.Owner).Move(group.Id);
		}, resourceOperationMove, LockAccess.Reader, setErrorOnObject: false);
	}

	public void RemoveDependency(Resource dependOnResource)
	{
		RemoveDependency(dependOnResource, base.SetLastError);
	}

	public void RemoveDependency(Resource dependOnResource, Action<OperationResult> resourceOperationRemoveDependency)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			dependOnResource.ExecuteMethod(delegate(ILockable dependOnLockObject)
			{
				((PResource)lockObject.Owner).RemoveDependency((PResource)dependOnLockObject.Owner);
			}, resourceOperationRemoveDependency, LockAccess.Reader, setErrorOnObject: false);
		}, LockAccess.Reader);
	}

	public void Recycle()
	{
		Recycle(delegate(OperationResult operationResult)
		{
			SetError(operationResult.Error);
			SetLastError(operationResult);
		});
	}

	public void Recycle(Action<OperationResult> resourceOperationRecycle)
	{
		RedirectAsyncOutput(delegate
		{
			ResourceState = ResourceState.Offline;
		}, delegate(OperationResult asyncResultOffline)
		{
			if (asyncResultOffline.Error != null)
			{
				resourceOperationRecycle.SafeCall(new OperationResult(this, asyncResultOffline.Error));
			}
			else
			{
				RedirectAsyncOutput(delegate
				{
					DateTime now = DateTime.Now;
					while (ResourceState != ResourceState.Offline && ResourceState != ResourceState.Failed)
					{
						Thread.Sleep(100);
						if (now.AddMinutes(30.0) < DateTime.Now)
						{
							throw new ClusterDefaultException(new TimeoutException(ExceptionResources.ResourceRecycleTimeOut_Default));
						}
					}
					if (ResourceState == ResourceState.Offline)
					{
						ResourceState = ResourceState.Online;
					}
				}, delegate(OperationResult asyncResultOnline)
				{
					resourceOperationRecycle.SafeCall(new OperationResult(this, asyncResultOnline.Error));
				});
			}
		});
	}

	public override void Delete(bool askConfirmation = false)
	{
		Delete(base.SetLastError, askConfirmation);
	}

	public override void Delete(Action<OperationResult> resourceOpDelete, bool askConfirmation = false)
	{
		CreateDeleteDialog(delegate(ConfirmationDialog confirmationDialog)
		{
			ShowDialog(confirmationDialog, delegate
			{
				DeleteOperation(resourceOpDelete);
			}, null);
		}, askConfirmation);
	}

	public static void Get(Cluster cluster, Guid resourceId, Action<OperationResult<Resource>> operationResult, OperationType operationType)
	{
		Resource resource = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(resourceId, ClusterIdentityType.Resource, LockAccess.Reader);
				if (clusterLock != null)
				{
					resource = ((PResource)clusterLock.Owner).GetProxy();
				}
				else
				{
					PResource pResource = (PResource)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Resource, resourceId);
					try
					{
						pResource.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pResource);
						throw;
					}
					resource = pResource.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Resource> obj = new OperationResult<Resource>(cluster, resource, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	public static void Get(Cluster cluster, string resourceName, Action<OperationResult<Resource>> operationResult, OperationType operationType)
	{
		Resource resource = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(resourceName, ClusterIdentityType.Resource, LockAccess.Reader);
				if (clusterLock != null)
				{
					resource = ((PResource)clusterLock.Owner).GetProxy();
				}
				else
				{
					PResource pResource = (PResource)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.Resource, resourceName);
					try
					{
						pResource.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pResource);
						throw;
					}
					resource = pResource.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<Resource> obj = new OperationResult<Resource>(cluster, resource, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	internal Resource(Cluster cluster)
		: base(cluster)
	{
	}

	protected virtual void CreateDeleteDialog(Action<ConfirmationDialog> confirmationDialogCreation, bool createDialog)
	{
		if (confirmationDialogCreation == null)
		{
			return;
		}
		if (!createDialog)
		{
			confirmationDialogCreation(null);
			return;
		}
		LoadAsync(delegate
		{
			ConfirmationDialog confirmationDialog = new ConfirmationDialog
			{
				CustomIcon = Icon.NativeIcon,
				Caption = DialogResources.DeleteResource_Title.FormatCurrentCulture(ResourceType.ResourceKind.Translate()),
				Header = DialogResources.DeleteResource_Header.FormatCurrentCulture(DisplayName)
			};
			if (Dependents.Any())
			{
				confirmationDialog.Content = DialogResources.DeleteResource_ContentDependents;
			}
			confirmationDialogCreation(confirmationDialog);
		}, ResourceLoadSelection.Dependents);
	}

	protected virtual void DeleteOperation(Action<OperationResult> resourceOpDelete)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.Delete();
		}, resourceOpDelete, LockAccess.Reader);
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents: false, ignorePossibleOwners);
		PResource pResource = (PResource)privateObject;
		SetIdInternal(pResource.Id);
		SetNameInternal(pResource.Name);
		characteristics = pResource.Characteristics;
		resourceClass = pResource.Class;
		resourceSubclass = pResource.Subclass;
		dependencies = pResource.Dependencies;
		ownerGroup = ((pResource.OwnerGroup != null) ? pResource.OwnerGroup.GetProxy() : null);
		dependents = pResource.Dependents;
		flags = pResource.Flags;
		base.LoadSelection = pResource.LoadedSelection;
		if (!ignorePossibleOwners)
		{
			possibleOwners = NodesFromNodeIds(base.Cluster, pResource.PossibleOwners, subscribeToEvents: false);
		}
		Properties = pResource.Properties;
		requiredDependencies = pResource.RequiredDependencies;
		resourceState = pResource.ResourceState;
		ResourceType = pResource.ResourceType.GetProxy();
		isChildResource = pResource.IsChildUnderlineValue;
		IsQuorum = pResource.IsQuorum;
		ParseProperties(pResource.Properties, trackChanges: false);
		if (subscribeToEvents)
		{
			SubscribeToEvents(pResource);
		}
	}

	protected internal void RaiseApplicationStatusChangedEvent(object sender, ClusterApplicationStatusEventArgs args)
	{
		this.ApplicationStatusChanged.SafeCall(sender, args);
	}

	internal void ClearDependencies(OperationType operationType)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PResource)lockObject.Owner).SetDependencyRelationship(string.Empty);
		}, operationType, LockAccess.Reader);
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		if (ParseProperty(properties, "StatusInformation", ref subStatus, list))
		{
			list.TryAdd("ResourceSubStatus");
		}
		if (ParseProperty(properties, "LastOperationStatusCode", ref lastOperationStatusCode, list) && lastOperationStatusCode.HasValue)
		{
			if (ExceptionHelpers.Failed(lastOperationStatusCode.Value))
			{
				if ((this is VirtualMachineResource || this is VirtualMachineConfigurationResource) && subStatus.HasValue && !subStatus.Value.HasFlag(FailoverClusters.Framework.ResourceSubStatus.Locked))
				{
					Error = new ClusterVirtualMachineErrorToLogException(null, this, null);
				}
				else
				{
					Error = ClusterLastErrorException.Create(lastOperationStatusCode, DisplayName);
				}
			}
			else if (ExceptionHelpers.Information(lastOperationStatusCode.Value))
			{
				if (this is VirtualMachineResource || this is VirtualMachineConfigurationResource)
				{
					if (ExceptionHelpers.GetHResultFromClusterError(lastOperationStatusCode.Value) != NativeMethods.VM_E_TASK_CANCELED)
					{
						Error = new ClusterVirtualMachineWarningToLogException(null, this, null);
					}
					else
					{
						Information = ExceptionResources.VirtualMachineGroup_LiveMigrationCanceled_Text;
					}
				}
				else
				{
					Information = Utilities.FormatError(ExceptionHelpers.GetStatusCodeFromClusterError(lastOperationStatusCode.Value));
				}
			}
			else
			{
				Error = null;
				Information = null;
			}
		}
		if (ParseProperty(properties, "ResourceSpecificStatus", ref resourceSpecificStatus, list))
		{
			list.TryAdd("Information");
		}
		return list;
	}

	private void RemoveAllDependencyLinks(Action<OperationResult> dependenciesRemoved)
	{
		LoadAsync(delegate(ClusterLoadedEventArgs resOpLoaded)
		{
			RemoveAllDependents();
			RemoveAllDependencies();
			dependenciesRemoved.SafeCall(new OperationResult(this, resOpLoaded.Error));
		}, ResourceLoadSelection.Dependencies | ResourceLoadSelection.Dependents);
	}

	private void RemoveAllDependents()
	{
		foreach (Guid dependent in Dependents)
		{
			Guid dependentId = dependent;
			Get(base.Cluster, dependent, delegate(OperationResult<Resource> resOpGet)
			{
				if (resOpGet.Error != null)
				{
					ClusterLog.LogException(LogLevel.Info, resOpGet.Error, "There was an error getting the dependent resource '{0}'".FormatCurrentCulture(dependentId));
				}
				else
				{
					Resource result = resOpGet.Result;
					if (!(result == null) && result.ResourceType.ResourceKind != ResourceKind.VolumeShadowCopyServiceTask)
					{
						result.RemoveDependency(this);
					}
				}
			}, OperationType.Sync);
		}
	}

	private void RemoveAllDependencies()
	{
		foreach (Guid dependency in Dependencies)
		{
			Guid dependecyId = dependency;
			Get(base.Cluster, dependency, delegate(OperationResult<Resource> resOpGet)
			{
				if (resOpGet.Error != null)
				{
					ClusterLog.LogException(LogLevel.Info, resOpGet.Error, "There was an error getting the dependency '{0}'".FormatCurrentCulture(dependecyId));
				}
				else
				{
					Resource result = resOpGet.Result;
					RemoveDependency(result);
				}
			}, OperationType.Sync);
		}
	}

	private void ChangeDependencyRelationshipInternal(DependencyRelationship relationship, OperationType operationType)
	{
		Utilities.UnreferencedParameter(operationType);
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PResource)lockObject.Owner).SetDependencyRelationship(relationship.ToString());
		}, base.PropertiesOperationType, LockAccess.Reader);
	}

	private void ChangeStateInternal(ResourceState newState, OperationType operationType)
	{
		Utilities.UnreferencedParameter(operationType);
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			PResource pResource = (PResource)lockObject.Owner;
			switch (newState)
			{
			case ResourceState.Online:
				pResource.Online();
				break;
			case ResourceState.Offline:
				pResource.Offline();
				break;
			case ResourceState.Failed:
				pResource.Fail();
				break;
			}
		}, base.PropertiesOperationType, LockAccess.Reader);
	}

	protected static void EnqueueAndThrottleRequests(IEnumerable<Resource> resources, Action<Resource, Action<OperationResult>> operationToExecute)
	{
		ClusterObject.EnqueueAndThrottleRequests(resources.Cast<ClusterObject>(), delegate(ClusterObject clusterObject, Action<OperationResult> operationResult)
		{
			operationToExecute((Resource)clusterObject, operationResult);
		});
	}

	protected virtual bool? IsChildFromDependent(IEnumerable<Guid> dependentsGuids)
	{
		return false;
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.ResourceStateChanged:
		{
			ClusterResourceStateEventArgs args = e.EventArgument as ClusterResourceStateEventArgs;
			if (resourceState == args.State && args.Error == null)
			{
				return true;
			}
			resourceState = args.State;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("ResourceState");
				OnPropertyChanged("ApplicationStatus");
				this.StateChanged.SafeCall(this, args);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ResourceOwnerGroupChanged:
		{
			ClusterResourceOwnerGroupEventArgs args10 = e.EventArgument as ClusterResourceOwnerGroupEventArgs;
			if (ownerGroup != null && ownerGroup.Id == args10.GroupId && args10.Error == null)
			{
				return true;
			}
			ManualResetEventSlim resetEvent = args10.WaitEvent();
			Group.Get(base.Cluster, args10.GroupId, delegate(OperationResult<Group> groupOpGet)
			{
				bool updated = false;
				try
				{
					if (groupOpGet.Error is ClusterObjectNotFoundException)
					{
						return;
					}
					if (groupOpGet.Error != null)
					{
						Error = groupOpGet.Error;
						return;
					}
					this.ExecuteMethod(delegate(ILockable resourceLock)
					{
						if (resourceLock != null)
						{
							ownerGroup = groupOpGet.Result;
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
					childrenWeak = null;
					UIHelper.ExecuteOnDispatcher(delegate
					{
						OnPropertyChanged("OwnerGroup");
						OnPropertyChanged("OwnerNodeName");
						OnPropertyChanged("Children");
						this.OwnerGroupChanged.SafeCall(this, args10);
					}, OperationType.Async);
				}
			}, OperationType.Async);
			return true;
		}
		case EventType.ResourceFlagsChanged:
		{
			ClusterResourceFlagsEventArgs args3 = e.EventArgument as ClusterResourceFlagsEventArgs;
			if (flags == args3.Flags && args3.Error == null)
			{
				return true;
			}
			flags = args3.Flags;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("Flags");
				this.FlagsChanged.SafeCall(this, args3);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ResourceCharacteristicsChanged:
		{
			ClusterCharacteristicsEventArgs args5 = e.EventArgument as ClusterCharacteristicsEventArgs;
			if (characteristics == args5.Characteristics && args5.Error == null)
			{
				return true;
			}
			characteristics = args5.Characteristics;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("Characteristics");
				this.CharacteristicsChanged.SafeCall(this, args5);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ResourceDependenciesChanged:
		{
			ClusterDependenciesEventArgs args4 = e.EventArgument as ClusterDependenciesEventArgs;
			if (args4.Dependencies == null || args4.Dependencies.Equals(dependencies) || args4.Error != null)
			{
				return true;
			}
			dependencies = new List<Guid>(args4.Dependencies);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("Dependencies");
				this.DependenciesChanged.SafeCall(this, args4);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ResourceDependentsChanged:
		{
			ClusterDependentsEventArgs args9 = e.EventArgument as ClusterDependentsEventArgs;
			if (args9.Dependents == null || args9.Dependents.Equals(dependents) || args9.Error != null)
			{
				return true;
			}
			dependents = new List<Guid>(args9.Dependents);
			isChildResource = args9.IsChild;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("Dependents");
				OnPropertyChanged("IsChild");
				this.DependentsChanged.SafeCall(this, args9);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ResourceDependencyRelationshipChanged:
		{
			ClusterDependencyRelationshipEventArgs args7 = e.EventArgument as ClusterDependencyRelationshipEventArgs;
			if (args7.Error != null || args7.DependencyRelationship == null || (dependencyRelationship != null && dependencyRelationship.ToString() == args7.DependencyRelationship))
			{
				UIHelper.ExecuteOnDispatcher(delegate
				{
					this.DependencyRelationshipChanged.SafeCall(this, args7);
				}, OperationType.Async);
				return true;
			}
			Worker.Start(delegate
			{
				dependencyRelationship = DependencyRelationship.Parse(this, args7.DependencyRelationship);
				UIHelper.ExecuteOnDispatcher(delegate
				{
					OnPropertyChanged("DependencyRelationship");
					this.DependencyRelationshipChanged.SafeCall(this, args7);
				}, OperationType.Async);
			});
			return true;
		}
		case EventType.ResourceRequiredDependenciesChanged:
		{
			ClusterRequiredDependenciesEventArgs args2 = e.EventArgument as ClusterRequiredDependenciesEventArgs;
			if (args2.Error != null || args2.RequiredDependencies == null)
			{
				return true;
			}
			requiredDependencies = args2.RequiredDependencies;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("RequiredDependencies");
				this.RequiredDependenciesChanged.SafeCall(this, args2);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ResourcePossibleOwnersChanged:
		{
			ClusterResourcePossibleOwnersChangedEventArgs args6 = e.EventArgument as ClusterResourcePossibleOwnersChangedEventArgs;
			if (args6.Error != null || args6.PossibleNodes == null)
			{
				possibleOwners = null;
				break;
			}
			possibleOwners = NodesFromNodeIds(base.Cluster, args6.PossibleNodes, subscribeToEvents: false);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("PossibleOwners");
				this.PossibleOwnersChanged.SafeCall(this, args6);
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
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
		case EventType.ResourceIsQuorumChanged:
		{
			ClusterResourceIsQuorumChangedEventArgs args8 = e.EventArgument as ClusterResourceIsQuorumChangedEventArgs;
			if (args8 != null && args8.Error == null)
			{
				IsQuorum = args8.IsQuorum;
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("IsQuorum");
					this.IsQuorumChanged.SafeCall(this, args8);
				}, OperationType.Async, queueOnDispatcher);
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
		ObservableCollection<Node> possibleOwners = new ObservableCollection<Node>();
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			foreach (Guid item in nodesId)
			{
				using ClusterLock clusterLock = ((PCluster)lockObject.Owner).CacheManager.Get(item, ClusterIdentityType.Node, LockAccess.Reader);
				if (clusterLock != null)
				{
					Node proxy = ((PNode)clusterLock.Owner).GetProxy();
					possibleOwners.Add(proxy);
				}
			}
		}, LockAccess.Reader);
		return new ReadOnlyObservableCollection<Node>(possibleOwners);
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		resourceState = null;
		subStatus = null;
		ownerGroup = null;
		flags = null;
		characteristics = null;
		dependencies = null;
		dependents = null;
		dependencyRelationship = null;
		requiredDependencies = null;
		possibleOwners = null;
		lastOperationStatusCode = null;
		childrenWeak = null;
		moveTargetsWeak = null;
		isChildResource = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("ResourceState");
			OnPropertyChanged("ApplicationStatus");
			OnPropertyChanged("ResourceSubStatus");
			OnPropertyChanged("OwnerGroup");
			OnPropertyChanged("OwnerNodeName");
			OnPropertyChanged("Flags");
			OnPropertyChanged("Characteristics");
			OnPropertyChanged("Dependencies");
			OnPropertyChanged("Dependents");
			OnPropertyChanged("DependencyRelationship");
			OnPropertyChanged("RequiredDependencies");
			OnPropertyChanged("PossibleOwners");
			OnPropertyChanged("DiskInfo");
			OnPropertyChanged("DiskCapacity");
			OnPropertyChanged("Children");
			OnPropertyChanged("IsChild");
			OnPropertyChanged("LastOperationStatusCode");
			this.StateChanged.SafeCall(this, null);
			this.ApplicationStatusChanged.SafeCall(this, null);
			this.OwnerGroupChanged.SafeCall(this, null);
			this.FlagsChanged.SafeCall(this, null);
			this.CharacteristicsChanged.SafeCall(this, null);
			this.DependenciesChanged.SafeCall(this, null);
			this.DependentsChanged.SafeCall(this, null);
			this.DependencyRelationshipChanged.SafeCall(this, null);
			this.RequiredDependenciesChanged.SafeCall(this, null);
			this.PossibleOwnersChanged.SafeCall(this, null);
		}, OperationType.Async);
	}
}

