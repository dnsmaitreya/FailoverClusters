using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ResourceType : ClusterObject
{
	private ResourceClass? resourceClass;

	private ResourceSubclass? resourceSubclass;

	private Characteristics? characteristics;

	private bool? isStorage;

	private ReadOnlyObservableCollection<Node> possibleOwners;

	private string displayName;

	public override ClusterIdentityType IdentityType => ClusterIdentityType.ResourceType;

	public ResourceType ActualResourceType { get; internal set; }

	public ResourceKind ResourceKind { get; private set; }

	public override string DisplayName
	{
		get
		{
			string text = displayName;
			if (text == null)
			{
				LoadAsync(displayName, 3);
				return base.Name;
			}
			return text;
		}
	}

	[Column(Name = "Id", AutoSync = AutoSync.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Column(Name = "ResourceTypeProperties")]
	public override ClusterPropertyCollection Properties => base.Properties;

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

	public bool? IsStorage => LoadAsync(isStorage, 1);

	public ResourceClass Class => LoadAsync<ResourceClass, ResourceClass>(resourceClass, 1);

	public ResourceSubclass Subclass => LoadAsync<ResourceSubclass, ResourceSubclass>(resourceSubclass, 1);

	public Characteristics Characteristics => LoadAsync<Characteristics, Characteristics>(characteristics, 1);

	internal override Type OwnerType => typeof(PResourceType);

	public event EventHandler<ClusterResourceTypeIsStorageEventArgs> IsStorageChanged;

	public event EventHandler<ClusterResourceTypePossibleOwnersChangedEventArgs> PossibleOwnersChanged;

	public ResourceType(Cluster cluster, ResourceKind resourceKind, ResourceType actualResourceType = null)
		: base(cluster)
	{
		ResourceKind = resourceKind;
		SetNameInternal(PResourceType.ResourceKindToString(resourceKind));
		SetIdInternal(FormatHelper.UIntToGuid(FormatHelper.StringHash(base.Name.ToLowerInvariant())));
		ActualResourceType = actualResourceType;
	}

	public ResourceType Init(params object[] operations)
	{
		return this;
	}

	public override void Delete(bool askConfirmation = false)
	{
		Delete(base.SetLastError, askConfirmation);
	}

	public override void Delete(Action<OperationResult> operationResult, bool askConfirmation = false)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			lockObject.Owner.Delete();
		}, operationResult, LockAccess.Writer);
	}

	public override string ToString()
	{
		return string.Concat("ResourceType:", Id, ":", base.Name, ":", base.IsOpen.ToString());
	}

	public static void Get(Cluster cluster, string resourceTypeName, Action<OperationResult<ResourceType>> operationResult, OperationType operationType)
	{
		ResourceType resourceType = null;
		cluster.ExecuteMethod(delegate(ILockable lockObject)
		{
			ClusterObject.ProtectedScope(delegate
			{
				PCluster pCluster = (PCluster)lockObject.Owner;
				using ClusterLock clusterLock = pCluster.CacheManager.Get(resourceTypeName, ClusterIdentityType.ResourceType, LockAccess.Reader);
				if (clusterLock != null)
				{
					resourceType = ((PResourceType)clusterLock.Owner).GetProxy();
				}
				else
				{
					PResourceType pResourceType = (PResourceType)pCluster.CacheManager.AddObject(pCluster, ClusterIdentityType.ResourceType, resourceTypeName);
					try
					{
						pResourceType.LoadObject(0);
					}
					catch (ClusterObjectNotFoundException)
					{
						pCluster.CacheManager.RemoveObject(pResourceType);
						throw;
					}
					resourceType = pResourceType.GetProxy();
				}
			}, delegate(ClusterException ex)
			{
				OperationResult<ResourceType> obj = new OperationResult<ResourceType>(cluster, resourceType, ex);
				operationResult(obj);
			});
		}, operationType, LockAccess.Reader);
	}

	internal ResourceType(Cluster cluster, ResourceKind resourceKind)
		: base(cluster)
	{
		ResourceKind = resourceKind;
		SetNameInternal(PResourceType.ResourceKindToString(resourceKind));
		SetIdInternal(PResourceType.IdFromName(base.Name));
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents, ignorePossibleOwners);
		PResourceType pResourceType = (PResourceType)privateObject;
		SetIdInternal(pResourceType.Id);
		SetNameInternal(pResourceType.Name);
		resourceClass = pResourceType.Class;
		resourceSubclass = pResourceType.Subclass;
		characteristics = pResourceType.Characteristics;
		isStorage = pResourceType.IsStorage;
		base.LoadSelection = pResourceType.LoadedSelection;
		possibleOwners = (ignorePossibleOwners ? null : NodesFromNodeIds(base.Cluster, pResourceType.PossibleOwners, subscribeToEvents: false));
		Properties = pResourceType.Properties;
		ParseProperties(pResourceType.Properties, trackChanges: false);
		if (subscribeToEvents)
		{
			SubscribeToEvents(pResourceType);
		}
	}

	private IEnumerable<string> ParseProperties(ClusterPropertyCollection properties, bool trackChanges)
	{
		List<string> list = (trackChanges ? new List<string>() : null);
		ParseProperty(properties, "Name", ref displayName, list);
		return list;
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
		case EventType.ResourceTypeIsStorageChanged:
		{
			ClusterResourceTypeIsStorageEventArgs args = e.EventArgument as ClusterResourceTypeIsStorageEventArgs;
			if (isStorage == args.IsStorage && args.Error == null)
			{
				return true;
			}
			isStorage = args.IsStorage;
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("IsStorage");
				this.IsStorageChanged.SafeCall(this, args);
			}, OperationType.Async, queueOnDispatcher);
			return true;
		}
		case EventType.ResourceTypePossibleOwnersChanged:
		{
			ClusterResourceTypePossibleOwnersChangedEventArgs args2 = e.EventArgument as ClusterResourceTypePossibleOwnersChangedEventArgs;
			if (args2.Error != null || args2.PossibleNodes == null)
			{
				possibleOwners = null;
				break;
			}
			possibleOwners = NodesFromNodeIds(base.Cluster, args2.PossibleNodes, subscribeToEvents: false);
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				OnPropertyChanged("PossibleOwners");
				this.PossibleOwnersChanged.SafeCall(this, args2);
			}, OperationType.Async, queueOnDispatcher);
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

	public static bool operator ==(ResourceType resourceType1, ResourceType resourceType2)
	{
		return resourceType1?.Equals(resourceType2) ?? ((object)resourceType2 == null);
	}

	public static bool operator !=(ResourceType resourceType1, ResourceType resourceType2)
	{
		return !(resourceType1 == resourceType2);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		characteristics = null;
		isStorage = null;
		possibleOwners = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("Class");
			OnPropertyChanged("Characteristics");
			OnPropertyChanged("IsStorage");
			OnPropertyChanged("PossibleOwners");
			this.PossibleOwnersChanged.SafeCall(this, null);
			this.IsStorageChanged.SafeCall(this, null);
		}, OperationType.Async);
	}
}

