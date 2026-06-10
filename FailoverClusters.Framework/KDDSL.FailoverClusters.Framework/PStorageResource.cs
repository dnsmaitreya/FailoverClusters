using System;
using System.Collections.Generic;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class PStorageResource : PResource
{
	private byte[] diskVolumeInfo;

	private uint? maintenanceMode;

	private uint? diskRunChkDsk;

	private PoolInfo poolInfo;

	private ReplicationInfo replicationInfo;

	private ReplicationDiskType replicationDiskType;

	private IList<ReplicationStatusInfo> replicationStatus;

	private string cachedOwnerNodeFqdnName = string.Empty;

	public ReplicationDiskType ReplicationDiskType
	{
		get
		{
			return replicationDiskType;
		}
		internal set
		{
			replicationDiskType = value;
			RouteEvent(new ClusterWrapperEventArgs(EventType.StorageReplicationDiskTypeChanged, new ClusterStorageReplicationDiskTypeChangedEventArgs(base.Id, replicationDiskType)));
		}
	}

	public string StorageType { get; internal set; }

	public StorageCaps StorageCaps { get; internal set; }

	public ReplicationInfo ReplicationInfo
	{
		get
		{
			return replicationInfo;
		}
		internal set
		{
			replicationInfo = value;
			RouteEvent(new ClusterWrapperEventArgs(EventType.StorageReplicationInfoChanged, new ClusterStorageReplicationInfoChangedEventArgs(base.Id, replicationInfo)));
		}
	}

	public IList<ReplicationStatusInfo> ReplicationStatus
	{
		get
		{
			return replicationStatus;
		}
		internal set
		{
			replicationStatus = value;
			RouteEvent(new ClusterWrapperEventArgs(EventType.StorageReplicationStatusChanged, new ClusterStorageReplicationStatusChangedEventArgs(base.Id, replicationStatus)));
		}
	}

	public PoolInfo PoolInfo
	{
		get
		{
			return poolInfo;
		}
		internal set
		{
			poolInfo = value;
			RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceStoragePoolInfoChanged, new ClusterStoragePoolInfoChangedEventArgs(base.Id, poolInfo)));
		}
	}

	public PStorageResource(PCluster cluster, Guid id, string name)
		: this(cluster, id, name, new PResourceType(cluster, ResourceKind.PhysicalDisk))
	{
	}

	public PStorageResource(PCluster cluster, Guid id, string name, PResourceType resourceType)
		: base(cluster, id, name, resourceType)
	{
	}

	public Guid GetPoolId()
	{
		return base.Properties.GetOrNull("PoolId", (ClusterPropertyString clusterProperty) => (clusterProperty == null || string.IsNullOrEmpty(clusterProperty.TypedValue)) ? Guid.Empty : new Guid(clusterProperty.TypedValue));
	}

	protected override void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
		base.OnPropertiesChanged(sender, e);
		bool? flag = UpdateStorageFields(e.Properties);
		if (flag.HasValue)
		{
			if (flag == true)
			{
				base.LoadedSelection &= -257;
			}
			ClusterStoragePropertiesChangedEventArgs forceLoadArgs = new ClusterStoragePropertiesChangedEventArgs(base.Id, null, flag.Value, null);
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceStoragePropertiesChanged, forceLoadArgs));
			});
		}
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		int num = loadSelectionNeutral;
		int num2 = 0;
		if ((loadSelectionNeutral & 4) == 4)
		{
			num &= -5;
		}
		base.LoadObject(num);
		num2 |= num;
		if ((num & 0x100) == 256)
		{
			UpdateStorageFields(base.Properties);
		}
		if ((loadSelectionNeutral & 4) == 4)
		{
			try
			{
				base.LoadObject(4);
			}
			catch (ClusterObjectLoadFailedException)
			{
				base.Properties.Add(new ClusterPropertyUInt("VirtualDiskHealth", null, ClusterPropertyKind.Private, readOnly: true, 0u));
				base.Properties.Add(new ClusterPropertyUInt("VirtualDiskState", null, ClusterPropertyKind.Private, readOnly: true, 0u));
				base.LoadedSelection |= 4;
				RouteEvent(new ClusterWrapperEventArgs(EventType.PropertiesChanged, new ClusterPropertiesEventArgs(base.Id, base.Name, null, null)
				{
					Properties = base.Properties
				}));
			}
			num2 |= 4;
		}
		return new ClusterLoadedEventArgs(base.Id, loaded: true, num2, null);
	}

	public override void BroadcastChanges(int loadSelection, bool raiseLoadedEvent = false)
	{
		base.BroadcastChanges(loadSelection, raiseLoadedEvent);
		if (((ResourceLoadSelection)loadSelection).HasFlag(ResourceLoadSelection.StorageReplicationInfo))
		{
			RouteEvent(new ClusterWrapperEventArgs(EventType.StorageReplicationInfoChanged, new ClusterStorageReplicationInfoChangedEventArgs(base.Id, ReplicationInfo)));
		}
	}

	public override void TransferFrom(PClusterObject source, bool cacheIsLocked, int loadSelection)
	{
		PResource sourceObject = source as PResource;
		if (sourceObject == null)
		{
			throw new InvalidOperationException("Source and Target must be the same type: ".FormatCurrentCulture(GetType()));
		}
		base.TransferFrom(source, cacheIsLocked, loadSelection);
		LockTransferAndBroadCast(source, loadSelection, delegate
		{
			ResourceLoadSelection resourceLoadSelection = (ResourceLoadSelection)loadSelection;
			if (resourceLoadSelection.HasFlag(ResourceLoadSelection.StorageReplicationInfo))
			{
				base.PossibleOwners = sourceObject.PossibleOwners;
				base.LoadedSelection |= 0x800;
			}
		});
	}

	public void ReloadReplicationInfo()
	{
		LoadObject(536872960);
	}

	public void RemoveReplication(bool fullCleanUp)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Storage.RemoveReplication(this, fullCleanUp);
			});
		});
	}

	internal void SetReplicationLogSize(long logSize)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Storage.SetReplicationLogSize(this, logSize);
			});
		});
	}

	public void AssignStorageCaps()
	{
		if (base.Class.HasFlag(ResourceClass.Storage) && base.Subclass.HasFlag(ResourceSubclass.Compliant))
		{
			StorageCaps |= StorageCaps.VolumeInfo;
		}
		if (base.ResourceType.ResourceKind == ResourceKind.PhysicalDisk)
		{
			StorageCaps |= StorageCaps.PhysicalDiskResource;
		}
		else if (base.ResourceType.ResourceKind == ResourceKind.ClusterFileSystem)
		{
			StorageCaps |= StorageCaps.CsvResource;
		}
	}

	public override List<Action> ProcessNotification(Notification notification)
	{
		List<Action> list = base.ProcessNotification(notification);
		if (notification.Payload is ClusterResourceStateEventArgs clusterResourceStateEventArgs)
		{
			bool flag = ReloadStorageOnOwnerNodeAndResourceStateChanged(clusterResourceStateEventArgs.State);
			if (flag)
			{
				base.LoadedSelection &= -257;
			}
			ClusterStoragePropertiesChangedEventArgs forceLoadArgs = new ClusterStoragePropertiesChangedEventArgs(base.Id, null, flag, null);
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceStoragePropertiesChanged, forceLoadArgs));
			});
		}
		ClusterStoragePoolInfoChangedEventArgs poolInfoChangedEventArgs = notification.Payload as ClusterStoragePoolInfoChangedEventArgs;
		if (poolInfoChangedEventArgs != null)
		{
			poolInfo = poolInfoChangedEventArgs.PoolInfo;
			list.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.ResourceStoragePoolInfoChanged, poolInfoChangedEventArgs));
			});
		}
		return list;
	}

	internal void AddToClusterSharedVolumes()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.AddToClusterSharedVolumes(this);
			});
		});
	}

	internal void SetMaintenanceMode(bool maintMode)
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.SetMaintenanceMode(this, maintMode);
			});
		});
	}

	public override void Refresh(bool targeted)
	{
		diskVolumeInfo = null;
		maintenanceMode = null;
		diskRunChkDsk = null;
		poolInfo = null;
		replicationInfo = null;
		replicationStatus = null;
		base.Refresh(targeted);
	}

	private bool ReloadStorageOnOwnerNodeAndResourceStateChanged(ResourceState? resourceState)
	{
		bool result = false;
		if (resourceState == FailoverClusters.Framework.ResourceState.Online)
		{
			PNode pNode = null;
			PGroup ownerGroup = base.OwnerGroup;
			if (ownerGroup != null)
			{
				pNode = ownerGroup.OwnerNode;
			}
			string b = string.Empty;
			if (pNode != null)
			{
				b = pNode.Fqdn;
			}
			if (!string.Equals(cachedOwnerNodeFqdnName, b, StringComparison.OrdinalIgnoreCase))
			{
				result = true;
			}
			cachedOwnerNodeFqdnName = b;
		}
		return result;
	}

	private bool? UpdateStorageFields(ClusterPropertyCollection propertyCollection)
	{
		bool? updateStorageAndReload = null;
		propertyCollection.Get("DiskVolumeInfo", delegate(ClusterPropertyBinary diskVolumeInfoProperty)
		{
			if (!diskVolumeInfoProperty.TypedValue.SequenceEqual(diskVolumeInfo))
			{
				updateStorageAndReload = diskVolumeInfo != null;
				diskVolumeInfo = diskVolumeInfoProperty.TypedValue;
			}
		});
		propertyCollection.Get("MaintenanceMode", delegate(ClusterPropertyUInt maintenanceModeProperty)
		{
			if (maintenanceModeProperty.TypedValue != maintenanceMode)
			{
				updateStorageAndReload = maintenanceMode.HasValue;
				maintenanceMode = maintenanceModeProperty.TypedValue;
			}
		});
		propertyCollection.Get("DiskRunChkDsk", delegate(ClusterPropertyUInt diskRunChkDskProperty)
		{
			if (diskRunChkDskProperty.TypedValue != diskRunChkDsk)
			{
				updateStorageAndReload = diskRunChkDsk.HasValue;
				diskRunChkDsk = diskRunChkDskProperty.TypedValue;
			}
		});
		return updateStorageAndReload;
	}
}

