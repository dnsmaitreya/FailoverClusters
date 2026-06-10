using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class Cluster : ClusterObject, IFrameworkCluster
{
	private ClusterAdapterType adapter;

	private Icon2 clusterIcon;

	private static readonly SemaphoreSlim ResourceTypeSempahore;

	private bool fatalErrorOccurred;

	private uint? dynamicQuorum;

	private uint? fixQuorum;

	private DateTime? recentEventsResetTime;

	private WeakReferenceEx groupsWeak;

	private WeakReferenceEx nodesWeak;

	private WeakReferenceEx networksWeak;

	private WeakReferenceEx allUpNetworksWeak;

	private WeakReferenceEx networkInterfacesWeak;

	private WeakReferenceEx allUpNetworkInterfacesWeak;

	private WeakReferenceEx allUpNodesWeak;

	private WeakReferenceEx downNodesWeak;

	private WeakReferenceEx healthyRolesWeak;

	private WeakReferenceEx failedRolesWeak;

	private WeakReferenceEx resorceTypesWeak;

	private WeakReferenceEx allRolesWeak;

	private QuorumConfiguration quorumConfiguration;

	private string cauResourceName;

	private DateTime? cauLastRunCompleted;

	internal static readonly ConcurrentDictionary<Guid, ObservableCollection<AddResourceInputParameter>> ResourcesTypeNoWizardDict;

	internal static readonly ConcurrentDictionary<Guid, ObservableCollection<AddResourceInputParameter>> ResourcesTypeWizardDict;

	private readonly IClusterLinqProvider linqProvider;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.Cluster;

	public ClusterAdapterType Adapter
	{
		get
		{
			return adapter;
		}
		set
		{
			if (base.IsOpen)
			{
				throw new InvalidOperationException("Adapter cannot be changed while the cluster is open");
			}
			adapter = value;
		}
	}

	public Group AvailableStorage { get; private set; }

	public Group CoreGroup { get; private set; }

	public string FullyQualifiedDomainName { get; private set; }

	public ManagementPointType ManagementPointType { get; internal set; }

	public string ConnectedTo { get; private set; }

	public string HostNode { get; private set; }

	public OSVersion OsVersion { get; private set; }

	public uint FunctionalLevel { get; private set; }

	public string ClusterSharedVolumeRootPath { get; private set; }

	public bool DynamicQuorumEnabled
	{
		get
		{
			if (dynamicQuorum.HasValue)
			{
				return dynamicQuorum.Value == 1;
			}
			return false;
		}
	}

	public bool FixQuorum
	{
		get
		{
			if (fixQuorum.HasValue)
			{
				return fixQuorum.Value == 1;
			}
			return false;
		}
	}

	public override ClusterException Error
	{
		get
		{
			return base.Error;
		}
		set
		{
			base.Error = value;
			if (value is ClusterDialogException)
			{
				ClusterDialogException.ShowTaskDialogAsync(value);
			}
			else if (value is ClusterClosedException)
			{
				ClusterLog.LogException(value, "The cluster was closed unexpectedly and the UI will display the down cluster page.");
				UIHelper.ExecuteOnDispatcher(delegate
				{
					this.Disconnected.SafeCall(this, new ClusterDisconnectedEventArgs(Id, value));
				}, OperationType.Async);
			}
			else if (value != null && !fatalErrorOccurred && !IgnorableError(value))
			{
				fatalErrorOccurred = true;
				ClusterLog.LogException(value, "An unrecoverable error has occurred and Failover Cluster snap-in will be closed");
				UIHelper.ExecuteOnDispatcher(delegate
				{
					this.FatalError.SafeCall(this, new ClusterEventArgs(Id, value));
				}, OperationType.Async);
			}
		}
	}

	public IClusterList<Group> Groups
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref groupsWeak, () => (ClusterList<Group>)new ClusterList<Group>(this)
			{
				Name = "All Groups"
			}.Select((Group g) => g));
		}
	}

	public IClusterList<Group> HealthyRoles
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref healthyRolesWeak, delegate
			{
				ApplicationStatus roleState = ApplicationStatus.Running;
				ClusterList<Group> obj = (ClusterList<Group>)(from g in Groups
					where g.IsCore == (bool?)false && (int)g.GroupType != 4 && (int)g.GroupType != 5 && (int)g.GroupType != 116 && (int)g.GroupType != 117 && (int)g.GroupType != 120 && (int)g.ApplicationStatus == (int)roleState && g.IsHidden == false
					orderby g.Name
					select g);
				obj.Name = "All Healthy roles";
				return obj;
			});
		}
	}

	public IClusterList<Group> FailedRoles
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref failedRolesWeak, delegate
			{
				ApplicationStatus roleState = ApplicationStatus.Failed;
				ClusterList<Group> obj = (ClusterList<Group>)(from g in Groups
					where g.IsCore == (bool?)false && (int)g.GroupType != 4 && (int)g.GroupType != 5 && (int)g.GroupType != 116 && (int)g.GroupType != 117 && (int)g.GroupType != 120 && (int)g.ApplicationStatus == (int)roleState && g.IsHidden == false
					orderby g.Name
					select g);
				obj.Name = "All failed roles";
				return obj;
			});
		}
	}

	public IClusterList<Group> AllRoles
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref allRolesWeak, () => (ClusterList<Group>)(from g in new ClusterList<Group>(this)
				{
					Name = "All Roles"
				}
				where g.IsCore == (bool?)false && (int)g.GroupType != 4 && (int)g.GroupType != 5 && (int)g.GroupType != 116 && (int)g.GroupType != 117 && (int)g.GroupType != 120 && g.IsHidden == false
				orderby g.Name
				select g));
		}
	}

	public IClusterList<ResourceType> ResourceTypes
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref resorceTypesWeak, () => (ClusterList<ResourceType>)new ClusterList<ResourceType>(this)
			{
				Name = "All Resource Types"
			}.Select((ResourceType n) => n));
		}
	}

	public IClusterList<Node> Nodes
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref nodesWeak, () => (ClusterList<Node>)new ClusterList<Node>(this)
			{
				Name = "All Nodes"
			}.Select((Node n) => n));
		}
	}

	public IClusterList<Network> AllUpNetworks
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref allUpNetworksWeak, () => (ClusterList<Network>)new ClusterList<Network>(this)
			{
				Name = "All Up Networks"
			}.Where((Network n) => (int)n.State == 3));
		}
	}

	public IClusterList<Network> Networks
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref networksWeak, () => (ClusterList<Network>)new ClusterList<Network>(this)
			{
				Name = "All Networks"
			}.Select((Network n) => n));
		}
	}

	public IClusterList<NetworkInterface> AllUpNetworkInterfaces
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref allUpNetworkInterfacesWeak, () => (ClusterList<NetworkInterface>)new ClusterList<NetworkInterface>(this)
			{
				Name = "All Up Network Interfaces"
			}.Where((NetworkInterface n) => (int)n.State == 3));
		}
	}

	public IClusterList<NetworkInterface> NetworkInterfaces
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref networkInterfacesWeak, () => (ClusterList<NetworkInterface>)new ClusterList<NetworkInterface>(this)
			{
				Name = "All Network Interfaces"
			}.Select((NetworkInterface n) => n));
		}
	}

	public ClusterList<Node> AllUpNodes
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref allUpNodesWeak, () => (ClusterList<Node>)(from n in new ClusterList<Node>(this)
				{
					Name = "All Up Nodes"
				}
				orderby n.Name
				where (int)n.State == 0
				select n));
		}
	}

	public IClusterList<Node> AllDownNodes
	{
		get
		{
			if (!base.IsOpen)
			{
				throw new ClusterNotOpenedException();
			}
			return WeakReferenceEx.ReturnInstance(ref downNodesWeak, () => (ClusterList<Node>)(from n in new ClusterList<Node>(this)
				{
					Name = "All down Nodes"
				}
				orderby n.Name
				where (int)n.State == 1
				select n));
		}
	}

	public DateTime RecentEventsResetTime
	{
		get
		{
			DateTime? dateTime = recentEventsResetTime;
			if (!dateTime.HasValue)
			{
				return DateTime.MinValue;
			}
			return dateTime.Value;
		}
	}

	public QuorumConfiguration QuorumConfiguration => quorumConfiguration;

	public string CauResourceName => cauResourceName;

	public DateTime CauLastRunCompleted
	{
		get
		{
			DateTime? dateTime = cauLastRunCompleted;
			if (!dateTime.HasValue)
			{
				return DateTime.MinValue;
			}
			return dateTime.Value;
		}
	}

	public override Icon2 Icon => ReturnInstance(ref clusterIcon, () => new Icon2(InvariantResources.Cluster_Icon));

	internal override Type OwnerType => typeof(PCluster);

	public Guid CacheId { get; internal set; }

	internal IClusterLinqProvider Provider => linqProvider;

	public event EventHandler<ClusterDisconnectedEventArgs> Disconnected;

	public event EventHandler<ClusterEventArgs> FatalError;

	public event EventHandler<ClusterRefreshedEventArgs> Refreshed;

	protected override void InitializeCommands(CommandCollection collection)
	{
	}

	static Cluster()
	{
		ResourceTypeSempahore = new SemaphoreSlim(1);
		ResourcesTypeNoWizardDict = new ConcurrentDictionary<Guid, ObservableCollection<AddResourceInputParameter>>();
		ResourcesTypeWizardDict = new ConcurrentDictionary<Guid, ObservableCollection<AddResourceInputParameter>>();
		PerformanceCounters.InstallPerformanceCounters();
	}

	public Cluster()
		: this(ClusterAdapterType.Unknown)
	{
	}

	public Cluster(ClusterAdapterType adapter)
		: base(null)
	{
		this.adapter = adapter;
		base.Cluster = this;
		linqProvider = new ClusterLinqProvider(this);
	}

	public void AddNode(string name)
	{
		AddNode(name, base.SetLastError);
	}

	public void AddNode(string name, Action<OperationResult<Node>> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			Node newNode = null;
			ClusterObject.ProtectedScope(delegate
			{
				PNode pNode = ((PCluster)lockObject.Owner).AddNode(name);
				newNode = pNode.GetProxy();
			}, delegate(ClusterException ex)
			{
				OperationResult<Node> obj = new OperationResult<Node>(this, newNode, ex);
				operationResult(obj);
			});
		}, OperationType.Async, LockAccess.Reader, setErrorOnObject: false);
	}

	public void AddVirtualMachine(Guid virtualMachineId, string ownerNodeName)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).AddVirtualMachine(virtualMachineId, ownerNodeName);
		}, null, LockAccess.Reader, setErrorOnObject: false);
	}

	public Group GetGroup(string groupName)
	{
		Group toReturn = null;
		GetGroup(groupName, delegate(OperationResult<Group> operationResult)
		{
			if (operationResult.Error != null)
			{
				throw operationResult.Error;
			}
			toReturn = operationResult.Result;
		}, OperationType.Sync);
		if (toReturn == null)
		{
			throw new ClusterObjectNotFoundException(groupName, Guid.Empty, typeof(Group));
		}
		return toReturn;
	}

	public void GetGroup(Guid groupId, Action<OperationResult<Group>> operationResult, OperationType operationType)
	{
		Group.Get(this, groupId, operationResult, operationType);
	}

	public void GetGroup(string groupName, Action<OperationResult<Group>> operationResult, OperationType operationType)
	{
		Group.Get(this, groupName, operationResult, operationType);
	}

	public void CreateGroup(string name, GroupType groupType)
	{
		CreateGroup(name, groupType, base.SetLastError);
	}

	public void CreateGroup(string name, GroupType groupType, Action<OperationResult<Group>> operationResult)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			Group newGroup = null;
			ClusterObject.ProtectedScope(delegate
			{
				PGroup pGroup = ((PCluster)lockObject.Owner).CreateGroup(name, groupType);
				newGroup = pGroup.GetProxy();
			}, delegate(ClusterException ex)
			{
				OperationResult<Group> obj = new OperationResult<Group>(this, newGroup, ex);
				operationResult(obj);
			});
		}, OperationType.Async, LockAccess.Reader, setErrorOnObject: false);
	}

	public Resource GetResource(string resourceName)
	{
		Resource toReturn = null;
		GetResource(resourceName, delegate(OperationResult<Resource> operationResult)
		{
			if (operationResult.Error != null)
			{
				throw operationResult.Error;
			}
			toReturn = operationResult.Result;
		}, OperationType.Sync);
		if (toReturn == null)
		{
			throw new ClusterObjectNotFoundException(resourceName, Guid.Empty, typeof(Resource));
		}
		return toReturn;
	}

	public void GetResource(Guid resourceId, Action<OperationResult<Resource>> operationResult, OperationType operationType)
	{
		Resource.Get(this, resourceId, operationResult, operationType);
	}

	public void GetResource(string resourceName, Action<OperationResult<Resource>> operationResult, OperationType operationType)
	{
		Resource.Get(this, resourceName, operationResult, operationType);
	}

	public Node GetNode(string nodeName)
	{
		Node toReturn = null;
		GetNode(nodeName, delegate(OperationResult<Node> operationResult)
		{
			if (operationResult.Error != null)
			{
				throw operationResult.Error;
			}
			toReturn = operationResult.Result;
		}, OperationType.Sync);
		if (toReturn == null)
		{
			throw new ClusterObjectNotFoundException(nodeName, Guid.Empty, typeof(Node));
		}
		return toReturn;
	}

	public void GetNode(Guid nodeId, Action<OperationResult<Node>> operationResult, OperationType operationType)
	{
		Node.Get(this, nodeId, operationResult, operationType);
	}

	public void GetNode(string nodeName, Action<OperationResult<Node>> operationResult, OperationType operationType)
	{
		Node.Get(this, nodeName, operationResult, operationType);
	}

	public Network GetNetwork(string networkName)
	{
		Network toReturn = null;
		GetNetwork(networkName, delegate(OperationResult<Network> operationResult)
		{
			if (operationResult.Error != null)
			{
				throw operationResult.Error;
			}
			toReturn = operationResult.Result;
		}, OperationType.Sync);
		if (toReturn == null)
		{
			throw new ClusterObjectNotFoundException(networkName, Guid.Empty, typeof(Network));
		}
		return toReturn;
	}

	public void GetNetwork(Guid networkId, Action<OperationResult<Network>> operationResult, OperationType operationType)
	{
		Network.Get(this, networkId, operationResult, operationType);
	}

	public void GetNetwork(string networkName, Action<OperationResult<Network>> operationResult, OperationType operationType)
	{
		Network.Get(this, networkName, operationResult, operationType);
	}

	public NetworkInterface GetNetworkInterface(string networkInterfaceName)
	{
		NetworkInterface toReturn = null;
		GetNetworkInterface(networkInterfaceName, delegate(OperationResult<NetworkInterface> operationResult)
		{
			if (operationResult.Error != null)
			{
				throw operationResult.Error;
			}
			toReturn = operationResult.Result;
		}, OperationType.Sync);
		if (toReturn == null)
		{
			throw new ClusterObjectNotFoundException(networkInterfaceName, Guid.Empty, typeof(Network));
		}
		return toReturn;
	}

	public void GetNetworkInterface(Guid networkInterfaceId, Action<OperationResult<NetworkInterface>> operationResult, OperationType operationType)
	{
		NetworkInterface.Get(this, networkInterfaceId, operationResult, operationType);
	}

	public void GetNetworkInterface(string networkInterfaceName, Action<OperationResult<NetworkInterface>> operationResult, OperationType operationType)
	{
		NetworkInterface.Get(this, networkInterfaceName, operationResult, operationType);
	}

	public void GetResourceType(string resourceTypeName, Action<OperationResult<ResourceType>> operationResult, OperationType operationType)
	{
		ResourceType.Get(this, resourceTypeName, operationResult, operationType);
	}

	public void Collect()
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			PCluster pCluster = (PCluster)lockObject.Owner;
			if (pCluster.ClusterGc == null)
			{
				ClusterLog.LogError("ClusterGC instance is not set, Cluster.ClusterGC.Collect() could not be done");
			}
			else
			{
				pCluster.ClusterGc.Collect();
			}
		}, LockAccess.Reader);
	}

	public void Close()
	{
		Close(null);
	}

	public void Close(Action<OperationResult<bool>> operationResult)
	{
		if (!(Id == Guid.Empty))
		{
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				PCluster obj = (PCluster)lockObject.Owner;
				CacheManager.RemoveCluster(obj);
				bool result = obj.Close();
				ClearReferences();
				ResourcesTypeNoWizardDict.TryRemove(CacheId, out var value);
				ResourcesTypeWizardDict.TryRemove(CacheId, out value);
				return result;
			}, operationResult, LockAccess.Reader);
		}
	}

	public virtual void Open(string name, params object[] parameters)
	{
		if (base.Cluster == null)
		{
			throw new ClusterNotOpenedException();
		}
		if (parameters == null)
		{
			throw new ArgumentNullException("parameters");
		}
		if (parameters.Length < 3)
		{
			throw new ArgumentException("Parameters length is less than 3.  This method expects the parameters array to contain the cluster handle, cluster id and cluster apiaccesslevel.");
		}
		this.CreateObject((Guid)parameters[1]);
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).Open(name, parameters);
			TransferInternalData(lockObject.Owner, subscribeToEvents: false);
		}, LockAccess.Writer, setErrorOnObject: false);
	}

	public void Open(string name, Action<OperationResult> operationResult, params object[] parameters)
	{
		if (base.Cluster == null)
		{
			throw new ClusterNotOpenedException();
		}
		this.CreateObject(Guid.Empty);
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).Open(name, parameters);
			TransferInternalData(lockObject.Owner, subscribeToEvents: false);
		}, operationResult, LockAccess.Writer);
	}

	public void Shutdown()
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).Shutdown();
		}, null, LockAccess.Reader);
	}

	public void AddStoragePools(IList<ClusterableStoragePool> clusterableStoragePools, Action<ClusterableStoragePool> onNextAdded, Action onCompleted, CancellationToken cancellationToken)
	{
		Exceptions.ThrowIfNull(clusterableStoragePools, "clusterableStoragePools");
		int count = 0;
		int totalCount = clusterableStoragePools.Count();
		object syncLockOnCount = new object();
		bool raiseDoneEvent = false;
		Action doneAction = delegate
		{
			lock (syncLockOnCount)
			{
				count++;
				if (count >= totalCount)
				{
					raiseDoneEvent = true;
				}
			}
			if ((cancellationToken.IsCancellationRequested || raiseDoneEvent) && onCompleted != null)
			{
				UIHelper.ExecuteOnDispatcher(onCompleted, OperationType.Async);
			}
		};
		foreach (ClusterableStoragePool clusterableStoragePool in clusterableStoragePools)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				doneAction();
				break;
			}
			clusterableStoragePool.State = ClusterableStoragePoolAddOperationState.InProgress;
			ClusterableStoragePool localClusterableStoragePool = clusterableStoragePool;
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				((PCluster)lockObject.Owner).AddStoragePool(localClusterableStoragePool, delegate(Exception ex)
				{
					UIHelper.ExecuteOnDispatcher(delegate(ClusterableStoragePool pool)
					{
						pool.Error = ex;
						pool.State = ClusterableStoragePoolAddOperationState.Fail;
						if (onNextAdded != null)
						{
							onNextAdded(localClusterableStoragePool);
						}
					}, OperationType.Async, localClusterableStoragePool);
					doneAction();
				}, delegate
				{
					UIHelper.ExecuteOnDispatcher(delegate(ClusterableStoragePool pool)
					{
						pool.State = ClusterableStoragePoolAddOperationState.Success;
						if (onNextAdded != null)
						{
							onNextAdded(localClusterableStoragePool);
						}
					}, OperationType.Async, localClusterableStoragePool);
					doneAction();
				});
				return true;
			}, null, LockAccess.Reader);
		}
	}

	public ClusterableStoragePoolsCollection GetClusterablePools(Action<OperationResult> operationResult, Action onCompleted)
	{
		ClusterableStoragePoolsCollection clusterableStoragePoolsCollection = new ClusterableStoragePoolsCollection();
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).GetClusterablePools(delegate(ClusterableStoragePool pool)
			{
				UIHelper.ExecuteOnDispatcher(clusterableStoragePoolsCollection.Add, OperationType.Async, pool);
			}, delegate(Exception error)
			{
				UIHelper.ExecuteOnDispatcher(delegate
				{
					clusterableStoragePoolsCollection.Error = error;
					clusterableStoragePoolsCollection.IsLoaded = true;
					onCompleted.SafeCall();
				}, OperationType.Async);
			}, delegate
			{
				UIHelper.ExecuteOnDispatcher(delegate
				{
					clusterableStoragePoolsCollection.IsLoaded = true;
					onCompleted.SafeCall();
				}, OperationType.Async);
			});
		}, operationResult, LockAccess.Reader);
		return clusterableStoragePoolsCollection;
	}

	public override void Delete(bool askConfirmation = false)
	{
		Delete(null, askConfirmation);
	}

	public override void Delete(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).Delete();
		}, operationResult, LockAccess.Writer);
	}

	~Cluster()
	{
		Init();
	}

	private void Init()
	{
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.IdChanged:
			ResourcesTypeNoWizardDict.TryAdd(base.Cluster.CacheId, new ObservableCollection<AddResourceInputParameter>());
			ResourcesTypeWizardDict.TryAdd(base.Cluster.CacheId, new ObservableCollection<AddResourceInputParameter>());
			return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
		case EventType.ClusterRefresh:
		{
			ClusterLog.LogVerbose(LogSubcategory.FxCoreNotification, "Refresh in progress");
			ClusterRefreshedEventArgs clusterRefreshedEventArgs = e.EventArgument as ClusterRefreshedEventArgs;
			OnPostRefresh(clusterRefreshedEventArgs.Targeted);
			return true;
		}
		case EventType.ConnectedToNodeChanged:
		{
			ClusterConnectedToNodeChangedEventArgs clusterConnectedToNodeChangedEventArgs = e.EventArgument as ClusterConnectedToNodeChangedEventArgs;
			ConnectedTo = clusterConnectedToNodeChangedEventArgs.ConnectedTo;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("ConnectedTo");
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ClusterHostNodeChanged:
		{
			ClusterHostNodeChangedEventArgs clusterHostNodeChangedEventArgs = e.EventArgument as ClusterHostNodeChangedEventArgs;
			HostNode = clusterHostNodeChangedEventArgs.HostNode;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("HostNode");
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.FullyQualifiedDomainNameChanged:
		{
			ClusterFullyQualifiedDomainChangedEventArgs clusterFullyQualifiedDomainChangedEventArgs = e.EventArgument as ClusterFullyQualifiedDomainChangedEventArgs;
			FullyQualifiedDomainName = clusterFullyQualifiedDomainChangedEventArgs.NewFullyQualifiedDomainName;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("FullyQualifiedDomainName");
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.Disconnected:
		{
			ClusterLog.LogVerbose(LogSubcategory.FxCoreNotification, "Disconnection in progress");
			ClusterDisconnectedEventArgs args = e.EventArgument as ClusterDisconnectedEventArgs;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				this.Disconnected.SafeCall(this, args);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.FatalError:
			Error = e.EventArgument.Error;
			return true;
		case EventType.Opened:
		{
			base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
			ClusterOpenEventArgs clusterOpenEventArgs = e.EventArgument as ClusterOpenEventArgs;
			base.IsOpen = clusterOpenEventArgs.IsOpen;
			if (!clusterOpenEventArgs.IsOpen || clusterOpenEventArgs.Error != null)
			{
				return true;
			}
			LoadResourceTypes();
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				TransferInternalData(lockObject.Owner, subscribeToEvents: false);
				SetNameInternal(lockObject.Owner.Name);
			}, LockAccess.Reader);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("FullyQualifiedDomainName");
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.QuorumConfigurationChanged:
			if (e.EventArgument is ClusterQuorumChangedEventArgs clusterQuorumChangedEventArgs)
			{
				quorumConfiguration = ((clusterQuorumChangedEventArgs.QuorumConfiguration != null) ? new QuorumConfiguration(this, clusterQuorumChangedEventArgs.QuorumConfiguration) : null);
				UIHelper.ExecuteOnDispatcher((Action)delegate
				{
					OnPropertyChanged("QuorumConfiguration");
				}, OperationType.Async, queueOnDispatcher);
			}
			return true;
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
				return true;
			}
			break;
		}
		case EventType.ResourceTypeChanged:
			resorceTypesWeak = null;
			LoadResourceTypes(delegate
			{
				base.Cluster.ExecuteMethod(delegate(ILockable clusterLock)
				{
					((PCluster)clusterLock.Owner).CacheManager.SendEventToProxyGroups(new ClusterWrapperEventArgs(EventType.ResourceTypeChanged, e.EventArgument));
				}, LockAccess.Reader);
			});
			UIHelper.ExecuteOnDispatcher(delegate
			{
				OnPropertyChanged("ResourceTypes");
			}, OperationType.Async);
			return true;
		case EventType.DestroyClusterUpdate:
			if (e.EventArgument is ClusterDestroyClusterProgressEventArgs clusterDestroyClusterProgressEventArgs)
			{
				if (clusterDestroyClusterProgressEventArgs.Error == null)
				{
					Information = clusterDestroyClusterProgressEventArgs.DestroyProgress.ToString();
					return true;
				}
				Error = clusterDestroyClusterProgressEventArgs.Error;
			}
			break;
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		ParseProperty(properties, "DynamicQuorumEnabled", ref dynamicQuorum, list);
		ParseProperty(properties, "FixQuorum", ref fixQuorum, list);
		ParseProperty(properties, "RecentEventsResetTime", ref recentEventsResetTime, list);
		if (properties.PrivatePropertiesLoaded)
		{
			string memberVariable = null;
			if (!ParseProperty(properties, "CauResourceName", ref memberVariable, list) || memberVariable == null)
			{
				cauResourceName = string.Empty;
			}
			else
			{
				cauResourceName = memberVariable;
			}
			ulong? memberVariable2 = null;
			if (!ParseProperty(properties, "CauLastRunCompleted", ref memberVariable2, list) || !memberVariable2.HasValue)
			{
				cauLastRunCompleted = DateTime.MinValue;
			}
			else
			{
				cauLastRunCompleted = DateTime.FromFileTime((long)memberVariable2.Value);
			}
		}
		return list;
	}

	private void LoadResourceTypes(Action loaded = null)
	{
		base.Cluster.ResourceTypes.ExecuteQuery(delegate(OperationResult<IClusterList<ResourceType>> param)
		{
			ResourceTypesQuery(param, loaded.SafeCall);
		});
	}

	private void ResourceTypesQuery(OperationResult<IClusterList<ResourceType>> resourceTypeResult, Action finishLoad)
	{
		ResourceTypeSempahore.Wait();
		ResourcesTypeNoWizardDict.AddOrUpdate(base.Cluster.CacheId, new ObservableCollection<AddResourceInputParameter>(), (Guid key, ObservableCollection<AddResourceInputParameter> oldValue) => new ObservableCollection<AddResourceInputParameter>());
		ResourcesTypeWizardDict.AddOrUpdate(base.Cluster.CacheId, new ObservableCollection<AddResourceInputParameter>(), (Guid key, ObservableCollection<AddResourceInputParameter> oldValue) => new ObservableCollection<AddResourceInputParameter>());
		ObservableCollection<AddResourceInputParameter> resourcesTypeNoWizard = ResourcesTypeNoWizardDict[base.Cluster.CacheId];
		ObservableCollection<AddResourceInputParameter> resourcesTypeWizard = ResourcesTypeWizardDict[base.Cluster.CacheId];
		ConcurrentBag<AddResourceInputParameter> resourcesTypeNoWizardList = new ConcurrentBag<AddResourceInputParameter>();
		ConcurrentBag<AddResourceInputParameter> resourcesTypeWizardList = new ConcurrentBag<AddResourceInputParameter>();
		if (resourceTypeResult.Error != null)
		{
			ResourceTypeSempahore.Release();
			ClusterLog.LogException(resourceTypeResult.Error, "Error to get the resource types from the cluster");
			return;
		}
		IEnumerable<ResourceType> clusterObjects = new List<ResourceType>(resourceTypeResult.Result).Where(IsResourceTypeSupportedForAddingToGroup);
		LoadAsync(clusterObjects, delegate(ClusterLoadedEventArgs typeLoadResult)
		{
			if (typeLoadResult.Error != null)
			{
				ClusterLog.LogException(typeLoadResult.Error, "Error to load resource type");
			}
			else
			{
				ResourceType resourceType = typeLoadResult.Sender as ResourceType;
				if (resourceType.ResourceKind == ResourceKind.GenericApplication || resourceType.ResourceKind == ResourceKind.GenericScript || resourceType.ResourceKind == ResourceKind.GenericService || resourceType.ResourceKind == ResourceKind.NetworkName || resourceType.ResourceKind == ResourceKind.DistributedFileSystem)
				{
					resourcesTypeWizardList.Add(new AddResourceInputParameter(CommandResources.Group_NewResource_Value.FormatCurrentCulture(resourceType.ResourceKind.Translate()), resourceType.ResourceKind.Translate(), resourceType));
				}
				else if (resourceType.ResourceKind != ResourceKind.Other)
				{
					resourcesTypeNoWizardList.Add(new AddResourceInputParameter(CommandResources.Group_NewResource_Value.FormatCurrentCulture(resourceType.ResourceKind.Translate()), resourceType.ResourceKind.Translate(), resourceType));
				}
				else
				{
					resourcesTypeNoWizardList.Add(new AddResourceInputParameter(CommandResources.Group_NewResource_Value.FormatCurrentCulture(resourceType.DisplayName), resourceType.DisplayName, resourceType));
				}
			}
		}, delegate(OperationResult loadComplete)
		{
			try
			{
				if (loadComplete.Error != null)
				{
					ClusterLog.LogException(loadComplete.Error, "Failed to load resource types");
				}
				else
				{
					resourcesTypeNoWizard.AddAll(resourcesTypeNoWizardList.OrderBy((AddResourceInputParameter item) => item.SuggestedName));
					resourcesTypeWizard.AddAll(resourcesTypeWizardList.OrderBy((AddResourceInputParameter item) => item.SuggestedName));
				}
			}
			finally
			{
				try
				{
					finishLoad.SafeCall();
				}
				finally
				{
					ResourceTypeSempahore.Release();
				}
			}
		});
	}

	public void LoadPropertiesAsync(int loadSelection, Action<OperationResult> onLoadComplete)
	{
		Worker.Start(delegate
		{
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				pCluster.LoadedSelection &= ~loadSelection;
				pCluster.LoadObject(loadSelection);
				Properties = pCluster.Properties;
			}, LockAccess.Reader);
			onLoadComplete(new OperationResult(this, Error));
		});
	}

	public void LoadAsync(IEnumerable<ClusterObject> clusterObjects, Action<ClusterLoadedEventArgs> onSingleLoad, Action<OperationResult> onLoadComplete)
	{
		LoadAsync(clusterObjects, onSingleLoad, onLoadComplete, 268435456);
	}

	public void LoadAsync(IEnumerable<ClusterObject> clusterObjects, Action<ClusterLoadedEventArgs> onSingleLoad, Action<OperationResult> onLoadComplete, int loadSelection)
	{
		Worker.Start(delegate
		{
			CountdownEvent countDown = new CountdownEvent(1);
			foreach (ClusterObject clusterObject in clusterObjects)
			{
				countDown.TryAddCount();
				clusterObject.LoadAsync(delegate(ClusterLoadedEventArgs loadedArgs)
				{
					try
					{
						onSingleLoad.SafeCall(loadedArgs);
					}
					catch (Exception innerException3)
					{
						loadedArgs.Sender.Error = new ClusterDefaultException(innerException3);
					}
					finally
					{
						countDown.Signal();
					}
				}, loadSelection);
			}
			countDown.Signal();
			countDown.Wait();
			try
			{
				onLoadComplete.SafeCall(new OperationResult(null, null));
			}
			catch (Exception innerException2)
			{
				Error = new ClusterDefaultException(innerException2);
			}
		}, delegate(ClusterException exception)
		{
			try
			{
				onLoadComplete.SafeCall(new OperationResult(null, exception));
			}
			catch (Exception innerException)
			{
				Error = new ClusterDefaultException(innerException);
			}
		});
	}

	internal void MonitorCollectionStart<T>(ClusterList<T> clusterList) where T : ClusterObject
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).RealtimeCollections.Add(clusterList);
		}, LockAccess.Reader);
	}

	internal void MonitorCollectionEnd<T>(ClusterList<T> clusterList) where T : ClusterObject
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).RealtimeCollections.Remove(clusterList);
		}, LockAccess.Reader);
	}

	internal static Cluster Create(PCluster privateCluster, bool subscribeToEvents)
	{
		Cluster cluster = new Cluster();
		cluster.TransferInternalData(privateCluster, subscribeToEvents);
		return cluster;
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents: false, ignorePossibleOwners);
		PCluster pCluster = (PCluster)privateObject;
		CacheId = pCluster.CacheId;
		SetIdInternal(pCluster.Id);
		SetNameInternal(pCluster.Name);
		bool isOpen = base.IsOpen;
		base.IsOpen = false;
		base.IsOpen = isOpen;
		FullyQualifiedDomainName = pCluster.FqdnName;
		ManagementPointType = pCluster.ManagementPointType;
		base.IsOpen = pCluster.IsOpen;
		base.IsRemoved = pCluster.IsRemoved;
		base.LoadSelection = pCluster.LoadedSelection;
		Properties = pCluster.Properties;
		AvailableStorage = ((pCluster.AvailableStorage != null) ? pCluster.AvailableStorage.GetProxy() : null);
		ConnectedTo = pCluster.ConnectedTo;
		CoreGroup = ((pCluster.CoreGroup != null) ? pCluster.CoreGroup.GetProxy() : null);
		quorumConfiguration = ((pCluster.QuorumConfiguration != null) ? new QuorumConfiguration(this, pCluster.QuorumConfiguration) : null);
		HostNode = pCluster.HostNode;
		OsVersion = pCluster.OSVersion;
		FunctionalLevel = pCluster.FunctionalLevel;
		ClusterSharedVolumeRootPath = pCluster.SharedVolumesRootPath;
		ParseProperties(pCluster.Properties, trackChanges: false);
		if (subscribeToEvents)
		{
			SubscribeToEvents(pCluster);
		}
	}

	internal void WillVoterLossCauseQuorumLoss(QuorumVoterActionCheck voterActionCheck, string id, Action<OperationResult<bool>> operationResult)
	{
		this.ExecuteMethod((ILockable lockObject) => ((PCluster)lockObject.Owner).WillVoterLossCauseQuorumLoss(voterActionCheck, id), operationResult, LockAccess.Reader);
	}

	internal void EnqueueNotification(Notification notification)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PCluster)lockObject.Owner).Server.EnqueueNotification(notification);
		}, LockAccess.Reader);
	}

	private static bool IsResourceTypeSupportedForAddingToGroup(ResourceType resType)
	{
		if (resType.ResourceKind != ResourceKind.ClusterFileSystem && resType.ResourceKind != ResourceKind.PhysicalDisk && resType.ResourceKind != ResourceKind.FileShareWitness && resType.ResourceKind != ResourceKind.CloudWitness && resType.ResourceKind != ResourceKind.VirtualMachine && resType.ResourceKind != ResourceKind.VirtualMachineConfiguration && resType.ResourceKind != ResourceKind.FileServer && resType.ResourceKind != ResourceKind.DfsReplicatedFolder && resType.ResourceKind != ResourceKind.DistributedNetworkName && resType.ResourceKind != ResourceKind.ScaleOutFileServer && resType.ResourceKind != ResourceKind.VirtualMachineReplicationBroker && resType.ResourceKind != ResourceKind.NfsShare && resType.ResourceKind != ResourceKind.NetworkFileSystem && resType.ResourceKind != ResourceKind.StoragePool && resType.ResourceKind != ResourceKind.TaskScheduler && resType.ResourceKind != ResourceKind.ClusterAwareUpdating && resType.ResourceKind != ResourceKind.HyperVNetworkVirtualizationProviderAddress && resType.ResourceKind != ResourceKind.NetworkAddressTranslator && resType.ResourceKind != ResourceKind.DisjointIPv4Address && resType.ResourceKind != ResourceKind.DisjointIPv6Address && resType.ResourceKind != ResourceKind.StorageReplica && resType.ResourceKind != ResourceKind.HealthService && resType.ResourceKind != ResourceKind.SDDCManagement && resType.ResourceKind != ResourceKind.StorageQoS && resType.ResourceKind != ResourceKind.HyperVClusterWmi && resType.ResourceKind != ResourceKind.VirtualMachineReplicationCoordinator && resType.ResourceKind != ResourceKind.CrossClusterOrchestrator)
		{
			return resType.ResourceKind != ResourceKind.VolumeShadowCopyServiceTask;
		}
		return false;
	}

	private void OnPostRefresh(bool targeted)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			TransferInternalData(lockObject.Owner, subscribeToEvents: false);
		}, LockAccess.Reader);
		UIHelper.ExecuteOnDispatcher(delegate
		{
			this.Refreshed.SafeCall(this, new ClusterRefreshedEventArgs(Id, base.Name, targeted));
		}, OperationType.Async);
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		ClearReferences();
		ClearReferencesNotify();
	}

	private void ClearReferences()
	{
		dynamicQuorum = null;
		fixQuorum = null;
		groupsWeak = null;
		nodesWeak = null;
		networksWeak = null;
		allUpNetworksWeak = null;
		networkInterfacesWeak = null;
		allUpNetworkInterfacesWeak = null;
		allUpNodesWeak = null;
		downNodesWeak = null;
		allRolesWeak = null;
		healthyRolesWeak = null;
		failedRolesWeak = null;
		resorceTypesWeak = null;
		quorumConfiguration = null;
		cauResourceName = null;
		cauLastRunCompleted = null;
	}

	private void ClearReferencesNotify()
	{
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("DynamicQuorumEnabled");
			OnPropertyChanged("FixQuorum");
			OnPropertyChanged("Groups");
			OnPropertyChanged("Nodes");
			OnPropertyChanged("Networks");
			OnPropertyChanged("AllUpNetworks");
			OnPropertyChanged("NetworkInterfaces");
			OnPropertyChanged("AllUpNetworkInterfaces");
			OnPropertyChanged("AllUpNodes");
			OnPropertyChanged("AllDownNodes");
			OnPropertyChanged("HealthyRoles");
			OnPropertyChanged("FailedRoles");
			OnPropertyChanged("ResourceTypes");
			OnPropertyChanged("AllRoles");
			OnPropertyChanged("QuorumConfiguration");
			OnPropertyChanged("CauResourceName");
			OnPropertyChanged("CauLastRunCompleted");
			OnPropertyChanged("ConnectedTo");
			OnPropertyChanged("HostNode");
			OnPropertyChanged("ManagementPointType");
			OnPropertyChanged("ResourceTypes");
		}, OperationType.Async);
	}

	internal static bool IgnorableError(Exception exception)
	{
		Exception ex = exception;
		do
		{
			if (ex is ClusterObjectLoadFailedException || ex is ClusterObjectNotFoundException || ex is ClusterObjectDeletingException)
			{
				return true;
			}
		}
		while ((ex = ex.InnerException) != null);
		return false;
	}

	internal static ClusterIdentityType IdentityFromType(Type clusterObjectType)
	{
		if (clusterObjectType.IsAssignableFrom(typeof(Cluster)) || clusterObjectType.IsAssignableFrom(typeof(PCluster)))
		{
			return ClusterIdentityType.Cluster;
		}
		if (clusterObjectType.IsAssignableFrom(typeof(Group)) || clusterObjectType.IsAssignableFrom(typeof(PGroup)))
		{
			return ClusterIdentityType.Group;
		}
		if (clusterObjectType.IsAssignableFrom(typeof(Resource)) || clusterObjectType.IsAssignableFrom(typeof(PResource)))
		{
			return ClusterIdentityType.Resource;
		}
		if (clusterObjectType.IsAssignableFrom(typeof(Node)) || clusterObjectType.IsAssignableFrom(typeof(PNode)))
		{
			return ClusterIdentityType.Node;
		}
		if (clusterObjectType.IsAssignableFrom(typeof(ResourceType)) || clusterObjectType.IsAssignableFrom(typeof(PResourceType)))
		{
			return ClusterIdentityType.ResourceType;
		}
		if (clusterObjectType.IsAssignableFrom(typeof(Network)) || clusterObjectType.IsAssignableFrom(typeof(PNetwork)))
		{
			return ClusterIdentityType.Network;
		}
		if (clusterObjectType.IsAssignableFrom(typeof(NetworkInterface)) || clusterObjectType.IsAssignableFrom(typeof(PNetworkInterface)))
		{
			return ClusterIdentityType.NetworkInterface;
		}
		throw new NotSupportedException("Identity for type {0} not supported".FormatCurrentCulture(clusterObjectType.Name));
	}
}

