using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class WmiVmAdapter : IVirtualizationAdapter
{
	private enum RequestedState
	{
		Enabled = 2,
		Disabled = 3,
		Reset = 11,
		Paused = 9
	}

	private static class ResourceType
	{
		public const ushort Other = 1;

		public const ushort ComputerSystem = 2;

		public const ushort Processor = 3;

		public const ushort Memory = 4;

		public const ushort IDEController = 5;

		public const ushort ParallelSCSIHBA = 6;

		public const ushort FCHBA = 7;

		public const ushort ISCSIHBA = 8;

		public const ushort IBHCA = 9;

		public const ushort EthernetAdapter = 10;

		public const ushort OtherNetworkAdapter = 11;

		public const ushort IOSlot = 12;

		public const ushort IODevice = 13;

		public const ushort FloppyDrive = 14;

		public const ushort CDDrive = 15;

		public const ushort DVDdrive = 16;

		public const ushort Serialport = 17;

		public const ushort Parallelport = 18;

		public const ushort USBController = 19;

		public const ushort GraphicsController = 20;

		public const ushort StorageExtent = 21;

		public const ushort Disk = 22;

		public const ushort Tape = 23;

		public const ushort OtherStorageDevice = 24;

		public const ushort FirewireController = 25;

		public const ushort PartitionableUnit = 26;

		public const ushort BasePartitionableUnit = 27;

		public const ushort PowerSupply = 28;

		public const ushort CoolingDevice = 29;

		public const ushort DisketteController = 1;
	}

	private static class JobState
	{
		public const ushort New = 2;

		public const ushort Starting = 3;

		public const ushort Running = 4;

		public const ushort Suspended = 5;

		public const ushort ShuttingDown = 6;

		public const ushort Completed = 7;

		public const ushort Terminated = 8;

		public const ushort Killed = 9;

		public const ushort Exception = 10;

		public const ushort Service = 11;
	}

	private static class ResourceSubType
	{
		public const string DisketteController = null;

		public const string DisketteDrive = "Synthetic Diskette Drive";

		public const string ParallelSCSIHBA = "Synthetic SCSI Controller";

		public const string IDEController = "Emulated IDE Controller";

		public const string DiskSynthetic = "Synthetic Disk Drive";

		public const string DiskPhysical = "Physical Disk Drive";

		public const string DVDPhysical = "Physical DVD Drive";

		public const string DVDSynthetic = "Synthetic DVD Drive";

		public const string CDROMPhysical = "Physical CD Drive";

		public const string CDROMSynthetic = "Synthetic CD Drive";

		public const string EthernetSynthetic = "Synthetic Ethernet Port";

		public const string DVDLogical = "Virtual CD/DVD Disk";

		public const string ISOImage = "ISO Image";

		public const string VHD = "Virtual Hard Disk";

		public const string DVD = "Virtual DVD Disk";

		public const string VFD = "Virtual Floppy Disk";

		public const string VideoSynthetic = "Synthetic Display Controller";
	}

	private static class OtherResourceType
	{
		public const string DisketteController = "Virtual Diskette Controller";
	}

	private static class Utility
	{
		private enum ValueRole
		{
			Default,
			Minimum,
			Maximum,
			Increment
		}

		private enum ValueRange
		{
			Default,
			Minimum,
			Maximum,
			Increment
		}

		public const string VhdResourceType = "31";

		public const string VhdResourceSubType = "Microsoft:Hyper-V:Virtual Hard Disk";

		public const string SelectResourcePoolQuery = "SELECT * FROM CIM_ResourcePool WHERE ResourceType=\"{0}\" AND ResourceSubType=\"{1}\" AND PoolId=\"{2}\"";

		public static ManagementObject GetServiceObject(ManagementScope scope, string serviceName)
		{
			scope.Connect();
			ManagementPath path = new ManagementPath(serviceName);
			using ManagementClass managementClass = new ManagementClass(scope, path, null);
			ManagementObjectCollection instances = managementClass.GetInstances();
			ManagementObject managementObject = null;
			foreach (ManagementObject item in instances)
			{
				managementObject?.Dispose();
				managementObject = item;
			}
			return managementObject;
		}

		public static ManagementObject GetHostSystemDevice(string deviceClassName, string deviceObjectElementName, ManagementScope scope)
		{
			string machineName = Environment.MachineName;
			return GetSystemDevice(deviceClassName, deviceObjectElementName, machineName, scope);
		}

		public static ManagementObject GetSystemDevice(string deviceClassName, string deviceObjectElementName, string vmName, ManagementScope scope)
		{
			ManagementObject result = null;
			foreach (ManagementObject item in GetTargetComputer(vmName, scope).GetRelated(deviceClassName, "Msvm_SystemDevice", null, null, "PartComponent", "GroupComponent", classDefinitionsOnly: false, null))
			{
				if (item["ElementName"].ToString().Equals(deviceObjectElementName, StringComparison.OrdinalIgnoreCase))
				{
					result = item;
					break;
				}
			}
			return result;
		}

		public static int JobCompleted(ManagementBaseObject outParams, ManagementScope scope)
		{
			string path = (string)outParams["Job"];
			using ManagementObject managementObject = new ManagementObject(scope, new ManagementPath(path), null);
			managementObject.Get();
			while ((ushort)managementObject["JobState"] == 3 || (ushort)managementObject["JobState"] == 4)
			{
				Thread.Sleep(500);
				managementObject.Get();
			}
			if ((ushort)managementObject["JobState"] != 7)
			{
				return (ushort)managementObject["ErrorCode"];
			}
			return 0;
		}

		public static ManagementObject GetTargetComputer(string vmElementName, ManagementScope scope)
		{
			string query = "select * from Msvm_ComputerSystem Where ElementName = '{0}'".FormatInvariantCulture(vmElementName);
			using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, new ObjectQuery(query));
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			ManagementObject result = null;
			using (ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectCollection.GetEnumerator())
			{
				if (managementObjectEnumerator.MoveNext())
				{
					result = (ManagementObject)managementObjectEnumerator.Current;
				}
			}
			return result;
		}

		public static ManagementObject GetVirtualMachineSettings(ManagementObjectCollection virtualSystemSettings)
		{
			foreach (ManagementObject virtualSystemSetting in virtualSystemSettings)
			{
				if (string.Compare(virtualSystemSetting["VirtualSystemType"].ToString(), "Microsoft:Hyper-V:System:Realized", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return virtualSystemSetting;
				}
			}
			return null;
		}

		public static ManagementObject GetResourceAllocationsettingDataDefault(ManagementScope scope, ushort resourceType, string resourceSubType, string otherResourceType)
		{
			ManagementObject result = null;
			string text = "select * from Msvm_ResourcePool where ResourceType = '{0}' and ResourceSubType ='{1}' and OtherResourceType = '{2}'".FormatInvariantCulture(resourceType, resourceSubType, otherResourceType);
			text = ((resourceType != 1) ? "select * from Msvm_ResourcePool where ResourceType = '{0}' and ResourceSubType ='{1}' and OtherResourceType = null".FormatInvariantCulture(resourceType, resourceSubType) : "select * from Msvm_ResourcePool where ResourceType = '{0}' and ResourceSubType = null and OtherResourceType = {1}".FormatInvariantCulture(resourceType, otherResourceType));
			ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher(scope, new ObjectQuery(text)).Get();
			if (managementObjectCollection.Count == 1)
			{
				foreach (ManagementObject item in managementObjectCollection)
				{
					foreach (ManagementObject item2 in item.GetRelated("Msvm_AllocationCapabilities"))
					{
						foreach (ManagementObject relationship in item2.GetRelationships("Msvm_SettingsDefineCapabilities"))
						{
							if (Convert.ToInt16(relationship["ValueRole"], CultureInfo.InvariantCulture) == 0)
							{
								result = new ManagementObject(relationship["PartComponent"].ToString());
								break;
							}
						}
					}
				}
			}
			return result;
		}
	}

	private static readonly Size ThumbnailResolution = new Size(128, 96);

	private PCluster cluster;

	public WmiVmAdapter(PCluster cluster)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		this.cluster = cluster;
	}

	public VirtualMachineKeyValuePairs GetKeyValuePairs(string virtualMachineId, string hostName)
	{
		VirtualMachineKeyValuePairs kvp = null;
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			try
			{
				using ManagementObject managementObject = GetVirtualMachineKvpExchangeComponent(virtualMachineWmiConnection, virtualMachineId);
				if (managementObject != null)
				{
					kvp = new VirtualMachineKeyValuePairs(managementObject, VirtualMachineKeyValuePairs.GetRequestedInformationArray(VirtualMachineKeyValuePairRequest.Basic));
				}
			}
			catch (ManagementException ex)
			{
				if (ex.ErrorCode != ManagementStatus.NotFound)
				{
					throw;
				}
			}
		}, virtualMachineId);
		return kvp;
	}

	public VirtualMachineStorageInformation GetStorageInformation(string virtualMachineId, string hostName)
	{
		return ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			ManagementObject managementObject = null;
			List<VirtualHardDisk> list = new List<VirtualHardDisk>();
			IEnumerable<VirtualMachineResourcePool> storagePools = Enumerable.Empty<VirtualMachineResourcePool>();
			using (ManagementObject rootObject = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId))
			{
				using ManagementObjectCollection virtualSystemSettings = FollowAssociator(virtualMachineWmiConnection, rootObject, "Msvm_VirtualSystemSettingData");
				managementObject = Utility.GetVirtualMachineSettings(virtualSystemSettings);
				if (managementObject != null)
				{
					using ManagementObjectCollection source = FollowAssociator(virtualMachineWmiConnection, managementObject, "Msvm_StorageAllocationSettingData");
					list.AddRange(from ManagementObject storageAllocationSettingData in source
						where ContainsProperty(storageAllocationSettingData.Properties, "ResourceType") && ContainsProperty(storageAllocationSettingData.Properties, "HostResource")
						where (ushort)storageAllocationSettingData["ResourceType"] == 31
						from path in (string[])storageAllocationSettingData["HostResource"]
						where !string.IsNullOrEmpty(path)
						select new VirtualHardDisk(path, (string)storageAllocationSettingData["PoolId"], ContainsProperty(storageAllocationSettingData.Properties, "PersistentReservationsSupported") && storageAllocationSettingData["PersistentReservationsSupported"] != null && (bool)storageAllocationSettingData["PersistentReservationsSupported"]));
				}
			}
			if (list.Count > 0)
			{
				using (ManagementObject managementObject2 = GetImageManagementService(virtualMachineWmiConnection))
				{
					foreach (VirtualHardDisk item in list)
					{
						object[] array = new object[3] { item.Path, null, null };
						managementObject2.InvokeMethod("GetVirtualHardDiskState", array);
						if (array[1] is string xml)
						{
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.LoadXml(xml);
							XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/INSTANCE/@CLASSNAME");
							if (xmlNodeList != null && xmlNodeList.Count == 1 && !(xmlNodeList[0].Value != "Msvm_VirtualHardDiskState"))
							{
								xmlNodeList = xmlDocument.SelectNodes("//PROPERTY[@NAME = 'FileSize']/VALUE/child::text()");
								if (xmlNodeList != null && xmlNodeList.Count == 1)
								{
									item.Size = ulong.Parse(xmlNodeList[0].Value, CultureInfo.InvariantCulture);
								}
							}
						}
					}
				}
				string empty = string.Empty;
				storagePools = GetChildPools(GetPool(virtualMachineWmiConnection, empty), hostName);
			}
			return new VirtualMachineStorageInformation(managementObject, list, storagePools);
		}, virtualMachineId);
	}

	public VirtualMachineCheckpointInformation GetCheckpointInformation(string virtualMachineId, string hostName, PVirtualMachineResource virtualMachineResource)
	{
		return ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			List<Checkpoint> checkpoints = new List<Checkpoint>();
			using (ManagementObject rootObject = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId))
			{
				using ManagementObjectCollection associates = FollowAssociator(virtualMachineWmiConnection, rootObject, "Msvm_VirtualSystemSettingData");
				checkpoints = GetCheckpointTree(associates, virtualMachineResource);
			}
			return new VirtualMachineCheckpointInformation(checkpoints);
		}, virtualMachineId);
	}

	public VirtualMachineSummaryInformation GetSummaryInformation(string virtualMachineId, string hostName)
	{
		return ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject managementObject = GetVirtualSystemSettings(virtualMachineWmiConnection, virtualMachineId);
			if (managementObject == null)
			{
				return null;
			}
			return GetSummaryInformation(virtualMachineWmiConnection, managementObject, VirtualMachineSummaryInformation.GetRequestedInformationArray(VirtualMachineSummaryInformationRequest.Basic));
		}, virtualMachineId);
	}

	private static VirtualMachineSummaryInformation GetSummaryInformation(ManagementScope scope, ManagementObject virtualSystemSettings, uint[] requestedInformation)
	{
		using ManagementObject managementObject = GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
		using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("GetSummaryInformation");
		managementBaseObject["SettingData"] = new string[1] { virtualSystemSettings.Path.Path };
		managementBaseObject["RequestedInformation"] = requestedInformation;
		ManagementBaseObject managementBaseObject2 = managementObject.InvokeMethod("GetSummaryInformation", managementBaseObject, null);
		VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
		if (virtualMachineJobErrors == VirtualMachineJobErrors.Completed)
		{
			using (ManagementBaseObject summaryInformation = ((ManagementBaseObject[])managementBaseObject2["SummaryInformation"])[0])
			{
				return new VirtualMachineSummaryInformation(summaryInformation, virtualSystemSettings);
			}
		}
		throw new ClusterVirtualMachineGetSummaryInformationException((string)virtualSystemSettings["ElementName"], (int)virtualMachineJobErrors);
	}

	public VirtualMachineReplicationInformation GetReplicationInformation(string virtualMachineId, string hostName, ReplicationRelationshipType type)
	{
		return ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			VirtualMachineReplicationInformation virtualMachineReplicationInformation = null;
			try
			{
				using ManagementObject virtualComputerReplicationSettings = GetVirtualMachineReplicationSettings(virtualMachineWmiConnection, virtualMachineId, type);
				if (cluster.OSVersion >= OSVersion.Windows2012R2)
				{
					using ManagementObject virtualComputerReplicationRelationship = GetVirtualMachineReplicationRelationship(virtualMachineWmiConnection, virtualMachineId, type);
					virtualMachineReplicationInformation = new VirtualMachineReplicationInformation(virtualComputerReplicationRelationship, virtualComputerReplicationSettings)
					{
						RelationshipType = type
					};
				}
				else
				{
					using ManagementObject managementObject = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
					if (managementObject != null)
					{
						virtualMachineReplicationInformation = new VirtualMachineReplicationInformation(managementObject, virtualComputerReplicationSettings)
						{
							RelationshipType = type
						};
					}
				}
			}
			catch (ManagementException ex)
			{
				if (ex.ErrorCode != ManagementStatus.NotFound)
				{
					throw;
				}
			}
			if (virtualMachineReplicationInformation != null)
			{
				ManagementBaseObject replicationTaskObject = GetReplicationTaskObject(virtualMachineWmiConnection, virtualMachineId);
				if (replicationTaskObject != null)
				{
					virtualMachineReplicationInformation.ReplicationTaskProgress = (ushort)replicationTaskObject["PercentComplete"];
					virtualMachineReplicationInformation.ReplicationTaskName = (string)replicationTaskObject["ElementName"];
				}
				else
				{
					virtualMachineReplicationInformation.ReplicationTaskName = string.Empty;
				}
			}
			return virtualMachineReplicationInformation;
		}, virtualMachineId);
	}

	public Guid GetTestFailoverVirtualMachineId(string virtualMachineId, string hostName)
	{
		return ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			Guid result = default(Guid);
			using (ManagementObject managementObject = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId))
			{
				ManagementObjectCollection related = managementObject.GetRelated("Msvm_ComputerSystem", "Msvm_ReplicaSystemDependency", null, null, null, null, classDefinitionsOnly: false, null);
				if (related.Count > 1)
				{
					ClusterLog.LogError("More than one replica system dependency found for virtual machine");
				}
				else
				{
					foreach (ManagementObject item in related)
					{
						string g = (string)item["Name"];
						result = new Guid(g);
					}
				}
			}
			return result;
		}, virtualMachineId);
	}

	public Bitmap GetDesktopThumbnailImage(string virtualMachineId, string hostName)
	{
		return ExecuteAndCatchWmiExceptions(delegate
		{
			byte[] thumbnailImageData = GetThumbnailImageData(WmiHelper.GetVirtualMachineWmiConnection(hostName), virtualMachineId, ThumbnailResolution.Width, ThumbnailResolution.Height);
			return CreateBitmap(ThumbnailResolution.Width, ThumbnailResolution.Height, thumbnailImageData);
		}, virtualMachineId);
	}

	public void DeleteSaveState(string virtualMachineId, string hostName)
	{
		RequestState(virtualMachineId, hostName, RequestedState.Disabled);
	}

	public void Pause(string virtualMachineId, string hostName)
	{
		RequestState(virtualMachineId, hostName, RequestedState.Paused);
	}

	public void Resume(string virtualMachineId, string hostName)
	{
		RequestState(virtualMachineId, hostName, RequestedState.Enabled);
	}

	public void Reset(string virtualMachineId, string hostName)
	{
		RequestState(virtualMachineId, hostName, RequestedState.Reset);
	}

	public void TakeCheckpoint(string virtualMachineId, string hostName)
	{
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject managementObject = GetVirtualSystemSnapshotService(virtualMachineWmiConnection);
			using ManagementObject managementObject2 = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
			using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("CreateSnapshot");
			managementBaseObject["AffectedSystem"] = managementObject2.Path.Path;
			managementBaseObject["SnapshotType"] = SnapshotType.Full;
			using ManagementBaseObject managementBaseObject2 = managementObject.InvokeMethod("CreateSnapshot", managementBaseObject, null);
			string virtualMachineName = (string)managementObject2["ElementName"];
			VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
			switch (virtualMachineJobErrors)
			{
			case VirtualMachineJobErrors.Started:
			{
				int num = Utility.JobCompleted(managementBaseObject2, virtualMachineWmiConnection);
				if (num != 0)
				{
					throw new ClusterVirtualMachineTakeCheckpointException(virtualMachineName, num);
				}
				break;
			}
			case VirtualMachineJobErrors.Completed:
				break;
			default:
				throw new ClusterVirtualMachineTakeCheckpointException(virtualMachineName, EnumHelper.Translate(virtualMachineJobErrors));
			}
		}, virtualMachineId);
	}

	public void RevertCheckpoint(string virtualMachineId, string hostName, PVirtualMachineResource virtualMachineResource)
	{
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject virtualSystemManagementService = GetVirtualSystemSnapshotService(virtualMachineWmiConnection);
			using ManagementObject managementObject = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
			string virtualMachineName;
			string checkpointId;
			using (ManagementObjectCollection associates = FollowAssociator(virtualMachineWmiConnection, managementObject, "Msvm_VirtualSystemSettingData"))
			{
				virtualMachineName = (string)managementObject["ElementName"];
				Checkpoint checkpoint = FindCurrentVirtualMachineCheckpoint(GetCheckpointTree(associates, virtualMachineResource));
				if (checkpoint == null)
				{
					return;
				}
				checkpointId = checkpoint.Parent.Id.ToString();
			}
			using ManagementObject checkpointManagementObject = GetCheckPoint(virtualMachineWmiConnection, checkpointId);
			try
			{
				PerformCheckpointApplication(virtualSystemManagementService, checkpointManagementObject, managementObject, virtualMachineWmiConnection);
			}
			catch (ClusterVirtualMachineApplyCheckpointException innerException)
			{
				throw new ClusterVirtualMachineRevertCheckpointException(virtualMachineName, innerException);
			}
		}, virtualMachineId);
	}

	private void RequestState(string virtualMachineId, string hostName, RequestedState requestedState)
	{
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject managementObject = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
			string virtualMachineName = (string)managementObject["ElementName"];
			using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("RequestStateChange");
			managementBaseObject["RequestedState"] = requestedState;
			using ManagementBaseObject managementBaseObject2 = managementObject.InvokeMethod("RequestStateChange", managementBaseObject, null);
			VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
			switch (virtualMachineJobErrors)
			{
			case VirtualMachineJobErrors.Started:
			{
				int num = Utility.JobCompleted(managementBaseObject2, virtualMachineWmiConnection);
				if (num != 0)
				{
					throw new ClusterVirtualMachineStateException(virtualMachineName, num);
				}
				break;
			}
			case VirtualMachineJobErrors.Completed:
				break;
			default:
				throw new ClusterVirtualMachineStateException(virtualMachineName, EnumHelper.Translate(virtualMachineJobErrors));
			}
		}, virtualMachineId);
	}

	public void ApplyCheckpoint(string virtualMachineId, string hostName, string checkpointId)
	{
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject virtualSystemManagementService = GetVirtualSystemSnapshotService(virtualMachineWmiConnection);
			using ManagementObject virtualMachineManagementObject = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
			using ManagementObject checkpointManagementObject = GetCheckPoint(virtualMachineWmiConnection, checkpointId);
			PerformCheckpointApplication(virtualSystemManagementService, checkpointManagementObject, virtualMachineManagementObject, virtualMachineWmiConnection);
		}, virtualMachineId);
	}

	private static void PerformCheckpointApplication(ManagementObject virtualSystemManagementService, ManagementObject checkpointManagementObject, ManagementObject virtualMachineManagementObject, ManagementScope scope)
	{
		using ManagementBaseObject managementBaseObject = virtualSystemManagementService.GetMethodParameters("ApplySnapshot");
		managementBaseObject["Snapshot"] = checkpointManagementObject.Path.Path;
		using ManagementBaseObject managementBaseObject2 = virtualSystemManagementService.InvokeMethod("ApplySnapshot", managementBaseObject, null);
		string virtualMachineName = (string)virtualMachineManagementObject["ElementName"];
		string checkpointName = (string)checkpointManagementObject["ElementName"];
		VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
		switch (virtualMachineJobErrors)
		{
		case VirtualMachineJobErrors.Started:
		{
			int num = Utility.JobCompleted(managementBaseObject2, scope);
			if (num == 0)
			{
				break;
			}
			throw new ClusterVirtualMachineApplyCheckpointException(virtualMachineName, checkpointName, num);
		}
		case VirtualMachineJobErrors.Completed:
			break;
		default:
			throw new ClusterVirtualMachineApplyCheckpointException(virtualMachineName, checkpointName, EnumHelper.Translate(virtualMachineJobErrors));
		}
	}

	public void DeleteCheckpoint(string virtualMachineId, string hostName, string checkpointId)
	{
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject managementObject = GetVirtualSystemSnapshotService(virtualMachineWmiConnection);
			using ManagementObject managementObject3 = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
			using ManagementObject managementObject2 = GetCheckPoint(virtualMachineWmiConnection, checkpointId);
			using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("DestroySnapshot");
			managementBaseObject["AffectedSnapshot"] = managementObject2.Path.Path;
			string checkpointName = (string)managementObject2["ElementName"];
			using ManagementBaseObject managementBaseObject2 = managementObject.InvokeMethod("DestroySnapshot", managementBaseObject, null);
			string virtualMachineName = (string)managementObject3["ElementName"];
			VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
			switch (virtualMachineJobErrors)
			{
			case VirtualMachineJobErrors.Started:
			{
				int num = Utility.JobCompleted(managementBaseObject2, virtualMachineWmiConnection);
				if (num != 0)
				{
					throw new ClusterVirtualMachineDeleteCheckpointException(virtualMachineName, checkpointName, num);
				}
				break;
			}
			case VirtualMachineJobErrors.Completed:
				break;
			default:
				throw new ClusterVirtualMachineDeleteCheckpointException(virtualMachineName, checkpointName, EnumHelper.Translate(virtualMachineJobErrors));
			}
		}, virtualMachineId);
	}

	public void DeleteCheckpointTree(string virtualMachineId, string hostName, string checkpointId)
	{
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject managementObject = GetVirtualSystemSnapshotService(virtualMachineWmiConnection);
			using ManagementObject managementObject3 = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
			using ManagementObject managementObject2 = GetCheckPoint(virtualMachineWmiConnection, checkpointId);
			using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("DestroySnapshotTree");
			managementBaseObject["SnapshotSettingData"] = managementObject2.Path.Path;
			using ManagementBaseObject managementBaseObject2 = managementObject.InvokeMethod("DestroySnapshotTree", managementBaseObject, null);
			string virtualMachineName = (string)managementObject3["ElementName"];
			string checkpointName = (string)managementObject2["ElementName"];
			VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
			switch (virtualMachineJobErrors)
			{
			case VirtualMachineJobErrors.Started:
			{
				int num = Utility.JobCompleted(managementBaseObject2, virtualMachineWmiConnection);
				if (num != 0)
				{
					throw new ClusterVirtualMachineDeleteCheckpointTreeException(virtualMachineName, checkpointName, num);
				}
				break;
			}
			case VirtualMachineJobErrors.Completed:
				break;
			default:
				throw new ClusterVirtualMachineDeleteCheckpointTreeException(virtualMachineName, checkpointName, EnumHelper.Translate(virtualMachineJobErrors));
			}
		}, virtualMachineId);
	}

	public void RenameCheckpoint(string virtualMachineId, string hostName, string checkpointId, string checkpointName)
	{
		ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope virtualMachineWmiConnection = WmiHelper.GetVirtualMachineWmiConnection(hostName);
			using ManagementObject managementObject = GetServiceObject(virtualMachineWmiConnection, "Msvm_VirtualSystemManagementService");
			using ManagementObject managementObject3 = GetVirtualMachineObject(virtualMachineWmiConnection, virtualMachineId);
			using ManagementObject managementObject2 = GetCheckPoint(virtualMachineWmiConnection, checkpointId);
			using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("ModifySystemSettings");
			string checkpointName2 = (string)managementObject2["ElementName"];
			managementObject2["ElementName"] = checkpointName;
			managementBaseObject["SystemSettings"] = managementObject2.GetText(TextFormat.WmiDtd20);
			using ManagementBaseObject managementBaseObject2 = managementObject.InvokeMethod("ModifySystemSettings", managementBaseObject, null);
			string virtualMachineName = (string)managementObject3["ElementName"];
			VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
			switch (virtualMachineJobErrors)
			{
			case VirtualMachineJobErrors.Started:
			{
				int num = Utility.JobCompleted(managementBaseObject2, virtualMachineWmiConnection);
				if (num != 0)
				{
					throw new ClusterVirtualMachineRenameCheckpointException(virtualMachineName, checkpointName2, num);
				}
				break;
			}
			case VirtualMachineJobErrors.Completed:
				break;
			default:
				throw new ClusterVirtualMachineRenameCheckpointException(virtualMachineName, checkpointName, EnumHelper.Translate(virtualMachineJobErrors));
			}
		}, virtualMachineId);
	}

	public void Collect()
	{
	}

	private static ManagementObject GetPool(ManagementScope scope, string poolId)
	{
		SelectQuery query = new SelectQuery(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM CIM_ResourcePool WHERE ResourceType=\"{0}\" AND ResourceSubType=\"{1}\" AND PoolId=\"{2}\"", "31", "Microsoft:Hyper-V:Virtual Hard Disk", poolId));
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query);
		using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
		if (managementObjectCollection.Count != 1)
		{
			throw new ManagementException("A single CIM_ResourcePool derived instance could not be found for " + "ResourceType \"{0}\", ResourceSubtype \"{1}\" and PoolId \"{2}\"".FormatCurrentCulture("31", "Microsoft:Hyper-V:Virtual Hard Disk", poolId));
		}
		return managementObjectCollection.GetFirst();
	}

	private static IEnumerable<VirtualMachineResourcePool> GetChildPools(ManagementObject parentPool, string hostName)
	{
		using ManagementObjectCollection childPoolCollection = parentPool.GetRelated("Msvm_ResourcePool", "Msvm_ElementAllocatedFromPool", null, null, "Dependent", "Antecedent", classDefinitionsOnly: false, null);
		foreach (ManagementObject item in childPoolCollection)
		{
			using ManagementObject rasd = GetPoolAssociatedResourceAllocationSettingData(item);
			yield return new VirtualMachineResourcePool(item, rasd, hostName)
			{
				ChildPools = GetChildPools(item, hostName)
			};
		}
	}

	internal static ManagementObject GetPoolAssociatedResourceAllocationSettingData(ManagementObject pool)
	{
		if ((bool)pool.GetPropertyValue("Primordial"))
		{
			return null;
		}
		using ManagementObjectCollection managementObjectCollection = pool.GetRelated("Msvm_StorageAllocationSettingData", "Msvm_SettingsDefineState", null, null, "SettingData", "ManagedElement", classDefinitionsOnly: false, null);
		if (managementObjectCollection.Count != 1)
		{
			throw new ManagementException(string.Format(CultureInfo.CurrentCulture, "A single Msvm_StorageAllocationSettingData derived instance could not be found for PoolId \"{0}\"", (string)pool["PoolID"]));
		}
		return managementObjectCollection.GetFirst();
	}

	private static bool ContainsProperty(PropertyDataCollection properties, string name)
	{
		return properties.Cast<PropertyData>().Any((PropertyData property) => property.Name == name);
	}

	private static ManagementObjectCollection FollowAssociator(ManagementScope scope, ManagementObject rootObject, string desiredResultClass)
	{
		if (rootObject == null)
		{
			throw new ArgumentNullException("rootObject");
		}
		return ExecuteQuery(scope, "Associators of {{{0}}} where ResultClass={1}", rootObject.Path, desiredResultClass);
	}

	private static ManagementBaseObject GetReplicationTaskObject(ManagementScope scope, string virtualMachineId)
	{
		using ManagementObject managementObject = GetVirtualSystemSettings(scope, virtualMachineId);
		if (managementObject == null)
		{
			return null;
		}
		uint[] requestedInformation = new uint[1] { 108u };
		ManagementBaseObject[] virtualMachineAsynchronousTasks = GetVirtualMachineAsynchronousTasks(scope, managementObject, requestedInformation);
		if (virtualMachineAsynchronousTasks != null)
		{
			ManagementBaseObject[] array = virtualMachineAsynchronousTasks;
			foreach (ManagementBaseObject managementBaseObject in array)
			{
				ushort taskId = (ushort)managementBaseObject["JobType"];
				ushort jobState = (ushort)managementBaseObject["JobState"];
				if (IsReplicationTask(taskId) && !IsCompleted(jobState))
				{
					return managementBaseObject;
				}
			}
		}
		return null;
	}

	private static bool IsReplicationTask(int taskId)
	{
		if (taskId != 113 && taskId != 94 && taskId != 95 && taskId != 97 && taskId != 120 && taskId != 123 && taskId != 117 && taskId != 118 && taskId != 116 && taskId != 100 && taskId != 106 && taskId != 107 && taskId != 105)
		{
			return taskId == 121;
		}
		return true;
	}

	private static bool IsCompleted(int jobState)
	{
		if (jobState != 7 && jobState != 32768 && jobState != 8 && jobState != 9)
		{
			return jobState == 10;
		}
		return true;
	}

	private static ManagementBaseObject[] GetVirtualMachineAsynchronousTasks(ManagementScope scope, ManagementObject virtualSystemSettings, uint[] requestedInformation)
	{
		using ManagementObject managementObject = GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
		using ManagementBaseObject managementBaseObject = managementObject.GetMethodParameters("GetSummaryInformation");
		managementBaseObject["SettingData"] = new string[1] { virtualSystemSettings.Path.Path };
		managementBaseObject["RequestedInformation"] = requestedInformation;
		ManagementBaseObject managementBaseObject2 = managementObject.InvokeMethod("GetSummaryInformation", managementBaseObject, null);
		VirtualMachineJobErrors virtualMachineJobErrors = (VirtualMachineJobErrors)(uint)managementBaseObject2["ReturnValue"];
		if (virtualMachineJobErrors == VirtualMachineJobErrors.Completed)
		{
			using (ManagementBaseObject managementBaseObject3 = ((ManagementBaseObject[])managementBaseObject2["SummaryInformation"])[0])
			{
				return (ManagementBaseObject[])managementBaseObject3.GetPropertyValue("AsynchronousTasks");
			}
		}
		throw new ClusterVirtualMachineGetSummaryInformationException((string)virtualSystemSettings["ElementName"], (int)virtualMachineJobErrors);
	}

	private static ManagementObject GetVirtualSystemSettings(ManagementScope scope, string virtualMachineId)
	{
		using ManagementObject managementObject = GetVirtualMachineObject(scope, virtualMachineId);
		using ManagementObjectCollection virtualSystemSettings = managementObject.GetRelated("Msvm_VirtualSystemSettingData", "Msvm_SettingsDefineState", null, null, "SettingData", "ManagedElement", classDefinitionsOnly: false, null);
		return Utility.GetVirtualMachineSettings(virtualSystemSettings);
	}

	private static byte[] GetThumbnailImageData(ManagementScope scope, string virtualMachineId, int widthPixels, int heightPixels)
	{
		byte[] result = null;
		try
		{
			using ManagementObject managementObject = GetVirtualMachineObject(scope, virtualMachineId);
			using ManagementObjectCollection source = managementObject.GetRelated("Msvm_VirtualSystemManagementService");
			ManagementObject managementObject2 = source.Cast<ManagementObject>().FirstOrDefault();
			if (managementObject2 != null)
			{
				using (managementObject2)
				{
					using ManagementBaseObject managementBaseObject = managementObject2.GetMethodParameters("GetVirtualSystemThumbnailImage");
					using ManagementObjectCollection virtualSystemSettings = managementObject.GetRelated("Msvm_VirtualSystemSettingData", "Msvm_SettingsDefineState", null, null, "SettingData", "ManagedElement", classDefinitionsOnly: false, null);
					ManagementObject virtualMachineSettings = Utility.GetVirtualMachineSettings(virtualSystemSettings);
					if (virtualMachineSettings != null)
					{
						using (virtualMachineSettings)
						{
							managementBaseObject["TargetSystem"] = virtualMachineSettings.Path.Path;
						}
						managementBaseObject["HeightPixels"] = heightPixels;
						managementBaseObject["WidthPixels"] = widthPixels;
						using ManagementBaseObject managementBaseObject2 = managementObject2.InvokeMethod("GetVirtualSystemThumbnailImage", managementBaseObject, null);
						if ((uint)managementBaseObject2["ReturnValue"] == 0)
						{
							result = managementBaseObject2["ImageData"] as byte[];
						}
						return result;
					}
				}
			}
		}
		catch (ManagementException ex)
		{
			if (ex.ErrorCode != ManagementStatus.NotFound)
			{
				throw;
			}
		}
		return result;
	}

	private static Bitmap CreateBitmap(int width, int height, byte[] imageData)
	{
		Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format16bppRgb565);
		if (imageData != null)
		{
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppRgb565);
			try
			{
				Marshal.Copy(imageData, 0, bitmapData.Scan0, width * height * 2);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}
		else
		{
			using Graphics graphics = Graphics.FromImage(bitmap);
			graphics.FillRectangle(Brushes.LightGray, 0, 0, width, height);
		}
		return bitmap;
	}

	private static ManagementObject GetServiceObject(ManagementScope scope, string serviceName)
	{
		ManagementPath path = new ManagementPath(serviceName);
		using ManagementClass managementClass = new ManagementClass(scope, path, null);
		using ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
		ManagementObject result = null;
		foreach (ManagementObject item in managementObjectCollection)
		{
			result = item;
		}
		return result;
	}

	private static ManagementObject GetSingleObject(ManagementScope scope, ManagementPath path, ObjectGetOptions op)
	{
		return new ManagementObject(scope, path, op);
	}

	private ManagementObject GetVirtualMachineReplicationSettings(ManagementScope scope, string virtualMachineId, ReplicationRelationshipType type)
	{
		string query = ((!(cluster.OSVersion >= OSVersion.Windows2012R2)) ? ("Select * from Msvm_ReplicationSettingData where VirtualSystemIdentifier='" + virtualMachineId + "'") : ("Select * from Msvm_ReplicationSettingData where InstanceID='" + GetReplicationObjectIdentifier(virtualMachineId, type) + "'"));
		using ManagementObjectCollection source = ExecuteQuery(scope, query);
		return source.Cast<ManagementObject>().FirstOrDefault();
	}

	private static ManagementObject GetVirtualMachineReplicationRelationship(ManagementScope scope, string virtualMachineId, ReplicationRelationshipType type)
	{
		using ManagementObjectCollection source = ExecuteQuery(scope, "Select * from Msvm_ReplicationRelationship where InstanceID='{0}'", GetReplicationObjectIdentifier(virtualMachineId, type));
		return source.Cast<ManagementObject>().FirstOrDefault();
	}

	private static string GetReplicationObjectIdentifier(string virtualMachineId, ReplicationRelationshipType type)
	{
		string text = "Microsoft:" + virtualMachineId + "\\\\HVR\\\\";
		if (type == ReplicationRelationshipType.Primary)
		{
			return text + "0";
		}
		return text + "1";
	}

	private static ManagementObject GetVirtualMachineObject(ManagementScope scope, string virtualMachineId, ObjectGetOptions op = null)
	{
		ManagementPath path = new ManagementPath("Msvm_ComputerSystem.CreationClassName=\"Msvm_ComputerSystem\",Name='{0}'".FormatInvariantCulture(virtualMachineId));
		return GetSingleObject(scope, path, op);
	}

	private static ManagementObject GetImageManagementService(ManagementScope scope, ObjectGetOptions op = null)
	{
		using ManagementObjectCollection source = ExecuteQuery(scope, "SELECT * FROM Msvm_ImageManagementService");
		return source.Cast<ManagementObject>().FirstOrDefault();
	}

	private static ManagementObject GetVirtualSystemSnapshotService(ManagementScope scope, ObjectGetOptions op = null)
	{
		using ManagementObjectCollection source = ExecuteQuery(scope, "SELECT * FROM Msvm_VirtualSystemSnapshotService");
		return source.Cast<ManagementObject>().FirstOrDefault();
	}

	private ManagementObject GetVirtualMachineKvpExchangeComponent(ManagementScope scope, string virtualMachineId)
	{
		using ManagementObject managementObject = GetVirtualMachineObject(scope, virtualMachineId);
		using ManagementObjectCollection source = managementObject.GetRelated("Msvm_KvpExchangeComponent");
		return source.Cast<ManagementObject>().FirstOrDefault();
	}

	private ManagementObject GetCheckPoint(ManagementScope scope, string checkpointId)
	{
		ManagementPath path = new ManagementPath("Msvm_VirtualSystemSettingData.InstanceId=\"Microsoft:{0}\"".FormatInvariantCulture(checkpointId));
		return GetSingleObject(scope, path, null);
	}

	private static ManagementObjectCollection ExecuteQuery(ManagementScope scope, string queryFormat, params object[] args)
	{
		string query = string.Format(CultureInfo.InvariantCulture, queryFormat, args);
		return ExecuteQuery(scope, query);
	}

	private static ManagementObjectCollection ExecuteQuery(ManagementScope scope, string query)
	{
		ObjectQuery query2 = new ObjectQuery(query);
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query2);
		return managementObjectSearcher.Get();
	}

	private T ExecuteAndCatchWmiExceptions<T>(Func<T> action, string objectName)
	{
		try
		{
			return action.SafeCall();
		}
		catch (ManagementException exception)
		{
			ClusterDialogException ex = ConvertException(exception, objectName);
			if (ex != null)
			{
				throw ex;
			}
			throw;
		}
		catch (COMException exception2)
		{
			ClusterDialogException ex2 = ConvertException(exception2, objectName);
			if (ex2 != null)
			{
				throw ex2;
			}
			throw;
		}
		catch (UnauthorizedAccessException exception3)
		{
			ClusterDialogException ex3 = ConvertException(exception3, objectName);
			if (ex3 != null)
			{
				throw ex3;
			}
			throw;
		}
	}

	private void ExecuteAndCatchWmiExceptions(Action action, string objectName)
	{
		ExecuteAndCatchWmiExceptions((Func<object>)delegate
		{
			action.SafeCall();
			return null;
		}, objectName);
	}

	private ClusterDialogException ConvertException(Exception exception, string objectName = null)
	{
		if (exception is ManagementException ex)
		{
			return new ClusterDefaultException(new ClusterWmiWin32Exception((int)ex.ErrorCode, ex.Message, ex.StackTrace));
		}
		if (exception is COMException ex2)
		{
			return new ClusterDefaultException(new ClusterWmiWin32Exception(ex2.ErrorCode, exception.StackTrace));
		}
		if (exception is UnauthorizedAccessException)
		{
			return new ClusterDefaultException(exception);
		}
		return null;
	}

	private List<Checkpoint> GetCheckpointTree(ManagementObjectCollection associates, PVirtualMachineResource virtualMachineResource)
	{
		Dictionary<string, Tuple<Checkpoint, string>> dictionary = new Dictionary<string, Tuple<Checkpoint, string>>(StringComparer.OrdinalIgnoreCase);
		List<Checkpoint> list = new List<Checkpoint>();
		foreach (ManagementObject associate in associates)
		{
			string key = associate.Path.ToString();
			if (!dictionary.ContainsKey(key))
			{
				Checkpoint checkpoint = new Checkpoint(associate, virtualMachineResource);
				object obj = associate["Parent"];
				string item = null;
				if (obj == null || string.IsNullOrEmpty(obj.ToString()))
				{
					list.Add(checkpoint);
				}
				else
				{
					item = obj.ToString();
				}
				dictionary.Add(key, new Tuple<Checkpoint, string>(checkpoint, item));
			}
		}
		foreach (Tuple<Checkpoint, string> value in dictionary.Values)
		{
			if (!string.IsNullOrEmpty(value.Item2))
			{
				Checkpoint item2 = dictionary[value.Item2].Item1;
				value.Item1.Parent = item2;
				item2.Children.Add(value.Item1);
			}
		}
		return list;
	}

	private static Checkpoint FindCurrentVirtualMachineCheckpoint(IList<Checkpoint> checkpoints)
	{
		foreach (Checkpoint checkpoint in checkpoints)
		{
			if (checkpoint.IsCurrentVirtualMachine)
			{
				return checkpoint;
			}
			if (checkpoint.Children.Count > 0)
			{
				return FindCurrentVirtualMachineCheckpoint(checkpoint.Children);
			}
		}
		return null;
	}
}

