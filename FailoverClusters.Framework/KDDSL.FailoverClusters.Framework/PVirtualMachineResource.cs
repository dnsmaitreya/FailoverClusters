using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class PVirtualMachineResource : PResource
{
	private VirtualMachineState memberVirtualMachineState;

	private ulong? resourceSpecificData1;

	private ulong? resourceSpecificData2;

	private ulong? lastOperationStatusCode;

	public ulong? AssignedMemory { get; private set; }

	public ulong? AvailableMemory { get; private set; }

	public ulong? MemoryDemand { get; private set; }

	public ushort? GuestCpuUsage { get; private set; }

	public VirtualMachineHeartbeatStatus? HeartbeatStatus { get; private set; }

	public TimeSpan? GuestUpTime { get; private set; }

	public DateTime? GuestCreationTime { get; private set; }

	public string GuestOperatingSystem { get; private set; }

	public string GuestComputerName { get; private set; }

	public string IntegrationServicesVersion { get; private set; }

	public int GuestOsMajorVersion { get; private set; }

	public int GuestOsMinorVersion { get; private set; }

	public int GuestOsBuildNumber { get; private set; }

	public OSProductType GuestOsProductType { get; private set; }

	public string GuestNotes { get; private set; }

	public string GuestStatus { get; private set; }

	public VirtualMachineReplicationData PrimaryReplicationData { get; private set; }

	public VirtualMachineReplicationData ExtendedReplicationData { get; private set; }

	public VirtualMachineStorageInformation StorageInformation { get; private set; }

	public VirtualMachineCheckpointInformation CheckpointInformation { get; private set; }

	public Bitmap DesktopThumbnail { get; private set; }

	public VirtualMachineIntegrationServicesStatus? IntegrationServicesStatus { get; private set; }

	public string VmVersion { get; private set; }

	private bool IsRunning
	{
		get
		{
			if (base.Properties.PrivatePropertiesLoaded)
			{
				VirtualMachineState typedValue = (VirtualMachineState)((ClusterPropertyUInt)base.Properties["VmState"]).TypedValue;
				if (typedValue != VirtualMachineState.PowerOff && typedValue != VirtualMachineState.Saved)
				{
					return true;
				}
			}
			return false;
		}
	}

	private bool IsPowerOff
	{
		get
		{
			if (base.Properties.PrivatePropertiesLoaded && ((ClusterPropertyUInt)base.Properties["VmState"]).TypedValue == 3)
			{
				return true;
			}
			return false;
		}
	}

	public PVirtualMachineResource(PCluster cluster, Guid id, string name)
		: base(cluster, id, name, new PResourceType(cluster, ResourceKind.VirtualMachine))
	{
	}

	public void TurnOff()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.VirtualMachineTurnOff(this);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineTurnoffException(base.Name, ex);
			}
			return ex;
		});
	}

	public void TakeCheckpoint()
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.TakeCheckpoint(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineTakeCheckpointException(base.Name, ex);
			}
			return ex;
		});
	}

	public void RevertCheckpoint()
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.RevertCheckpoint(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name, this);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineRevertCheckpointException(base.Name, ex);
			}
			return ex;
		});
	}

	public void DeleteSavedState()
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.DeleteSaveState(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineDeleteStateException(base.Name, ex);
			}
			return ex;
		});
	}

	public void Pause()
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.Pause(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachinePauseException(base.Name, ex);
			}
			return ex;
		});
	}

	public void Resume()
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.Resume(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineResumeException(base.Name, ex);
			}
			return ex;
		});
	}

	public void Reset()
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.Reset(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineResetException(base.Name, ex);
			}
			return ex;
		});
	}

	public void Start()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.Online(base.Id);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null && !(ex is ClusterVirtualMachineStartReplicaException))
			{
				ex = new ClusterVirtualMachineStartException(base.Name, ex);
			}
			return ex;
		});
	}

	public void Save()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.VirtualMachineSave(this);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineSaveException(base.Name, ex);
			}
			return ex;
		});
	}

	public void Shutdown()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.VirtualMachineShutdown(this);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineShutdownException(base.Name, ex);
			}
			return ex;
		});
	}

	public void ApplyCheckpoint(Checkpoint checkpoint)
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.ApplyCheckpoint(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name, checkpoint.Id.ToString());
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineApplyCheckpointException(base.Name, checkpoint.DisplayName, ex);
			}
			return ex;
		});
	}

	public void DeleteCheckpoint(Checkpoint checkpoint)
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.DeleteCheckpoint(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name, checkpoint.Id.ToString());
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineDeleteCheckpointException(base.Name, checkpoint.DisplayName, ex);
			}
			return ex;
		});
	}

	public void DeleteCheckpointTree(Checkpoint checkpoint)
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.DeleteCheckpointTree(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name, checkpoint.Id.ToString());
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineDeleteCheckpointTreeException(base.Name, checkpoint.DisplayName, ex);
			}
			return ex;
		});
	}

	public void RenameCheckpoint(Checkpoint checkpoint, string newCheckpointName)
	{
		ProtectedScope(delegate
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
			if (clusterPropertyString == null)
			{
				throw new ClusterVirtualMachineIdNotFoundException(base.Name);
			}
			base.Cluster.Virtualization.RenameCheckpoint(clusterPropertyString.TypedValue, base.OwnerGroup.OwnerNode.Name, checkpoint.Id.ToString(), newCheckpointName);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ex = new ClusterVirtualMachineRenameCheckpointException(base.Name, checkpoint.DisplayName, ex);
			}
			return ex;
		});
	}

	public void MoveStorage(VirtualMachineStorageMoveParameters virtualMachineStorageMoveParameters)
	{
		ProtectedScope(delegate
		{
			SetInformation(CommonResources.VirtualMachineStorageMigrationStart);
			ClusterInformationEventArgs payload = new ClusterInformationEventArgs(base.OwnerGroup.Id, CommonResources.VirtualMachineStorageMigrationStart);
			base.Cluster.Server.EnqueueNotification(new GroupNotification(payload));
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.VirtualMachineMoveStorage(base.Id, base.OwnerGroup.OwnerNode.Name, virtualMachineStorageMoveParameters);
			});
		}, delegate(ClusterException ex)
		{
			SetInformation(string.Empty);
			base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterInformationEventArgs(base.OwnerGroup.Id, string.Empty)));
			if (ex != null)
			{
				ex = new ClusterVirtualMachineMoveStorageException(base.Name, ex);
				SetError(ex);
				base.OwnerGroup.SetError(ex);
				ClusterLog.LogException(ex, "Error moving storage.");
			}
			return ex;
		});
	}

	public Guid GetTestFailoverVirtualMachineId()
	{
		Guid testFailoverVmId = Guid.Empty;
		ProtectedScope(delegate
		{
			base.Properties.Get("vmId", delegate(ClusterPropertyString virtualMachineIdProperty)
			{
				testFailoverVmId = base.Cluster.Virtualization.GetTestFailoverVirtualMachineId(virtualMachineIdProperty.TypedValue, base.OwnerGroup.OwnerNode.Name);
			});
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ClusterLog.LogException(ex, "Failed to get test failover virtual machine id.");
			}
			return ex;
		});
		return testFailoverVmId;
	}

	public string GetVirtualMachineOwnerGroup(Guid vmId)
	{
		string ownerGroup = string.Empty;
		ProtectedScope(delegate
		{
			ownerGroup = base.Cluster.Server.Resource.GetVirtualMachineOwnerGroup(vmId);
		}, delegate(ClusterException ex)
		{
			if (ex != null)
			{
				ClusterLog.LogException(ex, "Failed to get owner group for test vm.");
			}
			return ex;
		});
		return ownerGroup;
	}

	protected override void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
		ResourceLoadSelection resourceLoadSelection = ResourceLoadSelection.None;
		if ((base.LoadedSelection & 1) != 1)
		{
			resourceLoadSelection |= ResourceLoadSelection.Basic;
		}
		if ((base.LoadedSelection & 2) != 2)
		{
			resourceLoadSelection |= ResourceLoadSelection.CommonProperties;
		}
		if ((base.LoadedSelection & 4) != 4)
		{
			resourceLoadSelection |= ResourceLoadSelection.PrivateProperties;
		}
		if (resourceLoadSelection != 0)
		{
			LoadObject((int)resourceLoadSelection);
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
			});
		}
		e.Properties.Get("VmState", delegate(ClusterPropertyUInt virtualMachineStateProperty)
		{
			VirtualMachineState typedValue = (VirtualMachineState)virtualMachineStateProperty.TypedValue;
			if (typedValue != memberVirtualMachineState)
			{
				memberVirtualMachineState = typedValue;
				base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, e, 2)));
			}
		});
		bool forwardedChanges = false;
		e.Properties.Get("ResourceSpecificData1", delegate(ClusterPropertyULong resourceSpecificData1Property)
		{
			if (resourceSpecificData1Property.TypedValue != resourceSpecificData1)
			{
				resourceSpecificData1 = resourceSpecificData1Property.TypedValue;
				forwardedChanges = true;
			}
		});
		e.Properties.Get("ResourceSpecificData2", delegate(ClusterPropertyULong resourceSpecificData2Property)
		{
			if (resourceSpecificData2Property.TypedValue != resourceSpecificData2)
			{
				resourceSpecificData2 = resourceSpecificData2Property.TypedValue;
				forwardedChanges = true;
			}
		});
		e.Properties.Get("LastOperationStatusCode", delegate(ClusterPropertyULong lastOperationStatusCodeProperty)
		{
			if (lastOperationStatusCodeProperty.TypedValue != lastOperationStatusCode)
			{
				lastOperationStatusCode = lastOperationStatusCodeProperty.TypedValue;
				forwardedChanges = true;
			}
		});
		if (forwardedChanges)
		{
			base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, e, 3)));
		}
		base.OnPropertiesChanged(sender, e);
	}

	public override void Refresh(bool targeted)
	{
		resourceSpecificData1 = null;
		resourceSpecificData2 = null;
		lastOperationStatusCode = null;
		base.Refresh(targeted);
	}

	private void BroadcastChanges(VirtualMachineResourceLoadSelections loadSelection)
	{
		List<ClusterWrapperEventArgs> list = new List<ClusterWrapperEventArgs>(10);
		if ((loadSelection & (VirtualMachineResourceLoadSelections)2) == (VirtualMachineResourceLoadSelections)2 && base.OwnerGroup != null)
		{
			ClusterPropertiesEventArgs forwardedPayload = new ClusterPropertiesEventArgs(base.Id, base.Name, (int)base.ResourceType.ResourceKind, null)
			{
				Properties = base.Properties
			};
			base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, forwardedPayload, 3)));
		}
		if ((loadSelection & (VirtualMachineResourceLoadSelections)4) == (VirtualMachineResourceLoadSelections)4 && base.OwnerGroup != null)
		{
			ClusterPropertiesEventArgs forwardedPayload2 = new ClusterPropertiesEventArgs(base.Id, base.Name, (int)base.ResourceType.ResourceKind, null)
			{
				Properties = base.Properties
			};
			base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, forwardedPayload2, 2)));
		}
		if ((loadSelection & VirtualMachineResourceLoadSelections.Thumbnail) == VirtualMachineResourceLoadSelections.Thumbnail)
		{
			list.Add(new ClusterWrapperEventArgs(EventType.VirtualMachineDesktopThumbnailChanged, new ClusterResourceImageEventArgs(base.Id, DesktopThumbnail, null)));
		}
		if ((loadSelection & VirtualMachineResourceLoadSelections.Status) == VirtualMachineResourceLoadSelections.Status)
		{
			list.Add(new ClusterWrapperEventArgs(EventType.VirtualMachineGuestStatusChanged, new ClusterResourceVirtualMachineGuestStatusEventArgs(base.Id, AssignedMemory, AvailableMemory, MemoryDemand, GuestCpuUsage, GuestUpTime, GuestCreationTime, GuestNotes, HeartbeatStatus, GuestStatus, null)));
		}
		if ((loadSelection & VirtualMachineResourceLoadSelections.Replication) == VirtualMachineResourceLoadSelections.Replication)
		{
			list.Add(new ClusterWrapperEventArgs(EventType.VirtualMachineReplicationSummaryChanged, new ClusterResourceVirtualMachineReplicationEventArgs(base.Id, PrimaryReplicationData, null)));
		}
		if ((loadSelection & VirtualMachineResourceLoadSelections.ExtendedReplication) == VirtualMachineResourceLoadSelections.ExtendedReplication)
		{
			list.Add(new ClusterWrapperEventArgs(EventType.VirtualMachineExtendedReplicationSummaryChanged, new ClusterResourceVirtualMachineReplicationEventArgs(base.Id, ExtendedReplicationData, null)));
		}
		if ((loadSelection & VirtualMachineResourceLoadSelections.Storage) == VirtualMachineResourceLoadSelections.Storage)
		{
			list.Add(new ClusterWrapperEventArgs(EventType.VirtualMachineStorageSummaryChanged, new ClusterResourceVirtualMachineStorageSummaryEventArgs(base.Id, StorageInformation, null)));
		}
		if ((loadSelection & VirtualMachineResourceLoadSelections.Checkpoints) == VirtualMachineResourceLoadSelections.Checkpoints)
		{
			list.Add(new ClusterWrapperEventArgs(EventType.VirtualMachineCheckpointSummaryChanged, new ClusterResourceVirtualMachineCheckpointSummaryEventArgs(base.Id, CheckpointInformation, null)));
		}
		if ((loadSelection & VirtualMachineResourceLoadSelections.Summary) == VirtualMachineResourceLoadSelections.Summary)
		{
			list.Add(new ClusterWrapperEventArgs(EventType.VirtualMachineGuestSummaryChanged, new ClusterResourceVirtualMachineGuestSummaryEventArgs(base.Id, GuestOperatingSystem, GuestComputerName, IntegrationServicesVersion, IntegrationServicesStatus, null, GuestOsProductType, GuestOsMajorVersion, GuestOsMinorVersion, GuestOsBuildNumber, VmVersion)));
		}
		RouteEvent(new ClusterWrapperEventArgs(EventType.BatchChanges, new ClusterBatchChangesEventArgs(base.Id, list)));
	}

	private void GetVirtualMachineIdAndOwnerNode(out string virtualMachineId, out string ownerNodeName)
	{
		ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["vmId"];
		if (clusterPropertyString == null)
		{
			ClusterLog.LogError("Failed to get vmId property from the virtual machine resource {0}", base.Name);
			throw new ClusterObjectLoadFailedException(base.Name);
		}
		PGroup ownerGroup = base.OwnerGroup;
		if (ownerGroup == null)
		{
			ClusterLog.LogError("Failed to get ownergroup for the virtual machine resource {0}", base.Name);
			throw new ClusterObjectLoadFailedException(base.Name);
		}
		PNode ownerNode = ownerGroup.OwnerNode;
		if (ownerNode == null)
		{
			ClusterLog.LogError("Failed to get ownernode for the virtual machine resource {0}", base.Name);
			throw new ClusterObjectLoadFailedException(base.Name);
		}
		virtualMachineId = clusterPropertyString.TypedValue;
		ownerNodeName = ownerNode.Name;
	}

	public override ClusterLoadedEventArgs LoadObject(int loadSelectionNeutral)
	{
		bool flag = false;
		if (loadSelectionNeutral == 0)
		{
			return null;
		}
		if ((loadSelectionNeutral & 0x20000000) == 536870912)
		{
			loadSelectionNeutral ^= 0x20000000;
			base.LoadedSelection &= ~loadSelectionNeutral;
		}
		if ((loadSelectionNeutral & 0x1000) == 4096 || (loadSelectionNeutral & 0x10000) == 65536 || (loadSelectionNeutral & 0x40000) == 262144 || (loadSelectionNeutral & 0x2000) == 8192 || (loadSelectionNeutral & 0x4000) == 16384 || (loadSelectionNeutral & 0x8000) == 32768 || (loadSelectionNeutral & 0x20000) == 131072)
		{
			if ((base.LoadedSelection & 1) != 1)
			{
				loadSelectionNeutral |= 1;
			}
			if ((base.LoadedSelection & 2) != 2)
			{
				loadSelectionNeutral |= 2;
			}
			if ((base.LoadedSelection & 4) != 4)
			{
				loadSelectionNeutral |= 4;
			}
			flag = true;
		}
		int previousSelection = base.LoadedSelection;
		if (((uint)loadSelectionNeutral & 0xFFFu) != 0)
		{
			base.LoadObject(loadSelectionNeutral);
		}
		string virtualMachineId = null;
		string ownerNodeName = null;
		if (flag)
		{
			GetVirtualMachineIdAndOwnerNode(out virtualMachineId, out ownerNodeName);
		}
		ClusterLoadedEventArgs loadedArgs = null;
		int currentSelection = base.LoadedSelection;
		VirtualMachineResourceLoadSelections loadSelection = (VirtualMachineResourceLoadSelections)loadSelectionNeutral;
		ProtectedScope(delegate
		{
			if ((loadSelection & VirtualMachineResourceLoadSelections.Thumbnail) == VirtualMachineResourceLoadSelections.Thumbnail)
			{
				try
				{
					DesktopThumbnail = ((!IsPowerOff) ? base.Cluster.Virtualization.GetDesktopThumbnailImage(virtualMachineId, ownerNodeName) : InvariantResources.VirtualMachineDesktopThumbnail);
					base.LoadedSelection |= 0x1000;
				}
				catch (ClusterException innerException)
				{
					throw new ClusterVirtualMachineGetDesktopThumbnailException(base.Name, innerException);
				}
			}
			if ((loadSelection & VirtualMachineResourceLoadSelections.Storage) == VirtualMachineResourceLoadSelections.Storage)
			{
				try
				{
					StorageInformation = base.Cluster.Virtualization.GetStorageInformation(virtualMachineId, ownerNodeName);
					base.LoadedSelection |= 0x10000;
				}
				catch (ClusterException innerException2)
				{
					throw new ClusterVirtualMachineGetStorageException(base.Name, innerException2);
				}
			}
			if ((loadSelection & VirtualMachineResourceLoadSelections.Checkpoints) == VirtualMachineResourceLoadSelections.Checkpoints)
			{
				try
				{
					CheckpointInformation = base.Cluster.Virtualization.GetCheckpointInformation(virtualMachineId, ownerNodeName, this);
					base.LoadedSelection |= 0x40000;
				}
				catch (ClusterException innerException3)
				{
					throw new ClusterVirtualMachineGetCheckpointException(base.Name, innerException3);
				}
			}
			if ((loadSelection & VirtualMachineResourceLoadSelections.Status) == VirtualMachineResourceLoadSelections.Status)
			{
				try
				{
					VirtualMachineSummaryInformation virtualMachineSummaryInformation = null;
					if (IsRunning)
					{
						virtualMachineSummaryInformation = base.Cluster.Virtualization.GetSummaryInformation(virtualMachineId, ownerNodeName);
					}
					if (virtualMachineSummaryInformation != null)
					{
						AssignedMemory = virtualMachineSummaryInformation.AssignedMemory;
						AvailableMemory = virtualMachineSummaryInformation.AvailableMemory;
						MemoryDemand = virtualMachineSummaryInformation.MemoryDemand;
						GuestCpuUsage = virtualMachineSummaryInformation.ProcessorLoad;
						GuestUpTime = virtualMachineSummaryInformation.Uptime;
						GuestCreationTime = virtualMachineSummaryInformation.CreationTime;
						GuestNotes = virtualMachineSummaryInformation.Notes;
						GuestStatus = virtualMachineSummaryInformation.GuestStatus;
						HeartbeatStatus = virtualMachineSummaryInformation.Heartbeat;
						VmVersion = virtualMachineSummaryInformation.Version;
					}
					else
					{
						AssignedMemory = 0uL;
						AvailableMemory = 0uL;
						MemoryDemand = 0uL;
						GuestCpuUsage = 0;
						GuestUpTime = new TimeSpan(0L);
						GuestCreationTime = new DateTime(0L);
						GuestNotes = string.Empty;
						GuestStatus = string.Empty;
						HeartbeatStatus = VirtualMachineHeartbeatStatus.NoContact;
						VmVersion = string.Empty;
					}
				}
				catch (ClusterException innerException4)
				{
					throw new ClusterVirtualMachineGetGuestStatusException(base.Name, innerException4);
				}
				base.LoadedSelection |= 0x2000;
			}
			if ((loadSelection & VirtualMachineResourceLoadSelections.Replication) == VirtualMachineResourceLoadSelections.Replication)
			{
				try
				{
					VirtualMachineReplicationInformation replicationInformation = base.Cluster.Virtualization.GetReplicationInformation(virtualMachineId, ownerNodeName, ReplicationRelationshipType.Primary);
					PrimaryReplicationData = GetReplicationData(replicationInformation);
				}
				catch (ClusterException exception)
				{
					ClusterLog.LogException(exception, "Failed to get replication information for virtual machine {0}".FormatCurrentCulture(base.Name));
				}
				base.LoadedSelection |= 0x8000;
			}
			if ((loadSelection & VirtualMachineResourceLoadSelections.ExtendedReplication) == VirtualMachineResourceLoadSelections.ExtendedReplication)
			{
				try
				{
					VirtualMachineReplicationInformation replicationInformation2 = base.Cluster.Virtualization.GetReplicationInformation(virtualMachineId, ownerNodeName, ReplicationRelationshipType.Extended);
					ExtendedReplicationData = GetReplicationData(replicationInformation2);
				}
				catch (ClusterException exception2)
				{
					ClusterLog.LogException(exception2, "Failed to get replication information for virtual machine {0}".FormatCurrentCulture(base.Name));
				}
				base.LoadedSelection |= 0x20000;
			}
			if ((loadSelection & VirtualMachineResourceLoadSelections.Summary) == VirtualMachineResourceLoadSelections.Summary)
			{
				try
				{
					VirtualMachineKeyValuePairs virtualMachineKeyValuePairs = null;
					if (IsRunning)
					{
						virtualMachineKeyValuePairs = base.Cluster.Virtualization.GetKeyValuePairs(virtualMachineId, ownerNodeName);
					}
					if (virtualMachineKeyValuePairs != null && virtualMachineKeyValuePairs.IntegrationServicesStatus == VirtualMachineIntegrationServicesStatus.Installed)
					{
						GuestOperatingSystem = virtualMachineKeyValuePairs.OSName;
						GuestComputerName = virtualMachineKeyValuePairs.FullyQualifiedDomainName;
						IntegrationServicesVersion = virtualMachineKeyValuePairs.IntegrationServicesVersion;
						IntegrationServicesStatus = virtualMachineKeyValuePairs.IntegrationServicesStatus;
						GuestOsProductType = OSProductType.Unknown;
						if (Enum.TryParse<OSProductType>(virtualMachineKeyValuePairs.ProductType, ignoreCase: true, out var result))
						{
							GuestOsProductType = result;
						}
						if (int.TryParse(virtualMachineKeyValuePairs.OSMajorVersion, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result2))
						{
							GuestOsMajorVersion = result2;
						}
						if (int.TryParse(virtualMachineKeyValuePairs.OSMinorVersion, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result3))
						{
							GuestOsMinorVersion = result3;
						}
						if (int.TryParse(virtualMachineKeyValuePairs.OSBuildNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result4))
						{
							GuestOsBuildNumber = result4;
						}
					}
					else
					{
						GuestOperatingSystem = string.Empty;
						GuestComputerName = string.Empty;
						IntegrationServicesVersion = string.Empty;
						IntegrationServicesStatus = VirtualMachineIntegrationServicesStatus.Unknown;
						GuestOsProductType = OSProductType.Unknown;
						GuestOsMajorVersion = 0;
						GuestOsMinorVersion = 0;
						GuestOsBuildNumber = 0;
					}
					base.LoadedSelection |= 0x4000;
				}
				catch (ClusterException ex2)
				{
					ClusterLog.LogException(ex2, "Could not get the virtual machine summary information");
					throw new ClusterVirtualMachineGetGuestSummaryException(base.Name, ex2);
				}
			}
		}, delegate(ClusterException ex)
		{
			if (ex == null)
			{
				int num = currentSelection ^ base.LoadedSelection;
				if ((previousSelection & 4) != 4 && (currentSelection & 4) == 4)
				{
					num |= 4;
				}
				if ((previousSelection & 2) != 2 && (currentSelection & 2) == 2)
				{
					num |= 2;
				}
				BroadcastChanges((VirtualMachineResourceLoadSelections)num);
			}
			loadedArgs = new ClusterLoadedEventArgs(base.Id, ex == null, (int)loadSelection, ex);
			RouteEvent(new ClusterWrapperEventArgs(EventType.Loaded, loadedArgs));
		}, resetIsProcessing: true, affectsIsProcessing: false);
		return loadedArgs;
	}

	private VirtualMachineReplicationData GetReplicationData(VirtualMachineReplicationInformation replicationInformation)
	{
		VirtualMachineReplicationData virtualMachineReplicationData = new VirtualMachineReplicationData();
		if (replicationInformation != null)
		{
			virtualMachineReplicationData.PrimaryServerName = replicationInformation.PrimaryServerName;
			virtualMachineReplicationData.PrimaryConnectionPoint = replicationInformation.PrimaryConnectionPoint;
			virtualMachineReplicationData.RecoveryServerName = replicationInformation.RecoveryServerName;
			virtualMachineReplicationData.RecoveryConnectionPoint = replicationInformation.RecoveryConnectionPoint;
			virtualMachineReplicationData.LastReplicaTime = replicationInformation.LastReplicaTime;
			virtualMachineReplicationData.ReplicationTaskName = replicationInformation.ReplicationTaskName;
			virtualMachineReplicationData.ReplicationTaskProgress = replicationInformation.ReplicationTaskProgress;
			virtualMachineReplicationData.RelationshipType = replicationInformation.RelationshipType;
		}
		return virtualMachineReplicationData;
	}
}

