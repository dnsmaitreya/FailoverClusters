using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public class ClusterDisk : IComparable<ClusterDisk>
{
	private ClusterDiskId m_diskId;

	private string m_id;

	private int m_number = -1;

	private ulong m_size;

	private Cluster m_cluster;

	private Collection<ClusterDiskPartition> m_partitions;

	private string m_diskName;

	private string m_node;

	private string wmiSuggestedResourceName;

	private string m_virtualDiskId;

	private string m_poolId;

	private const string WmiAvailablePartitionClassName = "MSCluster_AvailableDiskPartition";

	private Guid m_key;

	private const string WmiResourceClassName = "MSCluster_Resource";

	private const string AttachStorageDeviceMethod = "AttachStorageDevice";

	private const string AttachStorageDiskParam = "StorageDevice";

	public int PartitionCount => m_partitions.Count;

	public ICollection<ClusterDiskPartition> Partitions => m_partitions;

	public string VirtualDiskId
	{
		get
		{
			return m_virtualDiskId;
		}
		internal set
		{
			m_virtualDiskId = value;
		}
	}

	public string PoolId
	{
		get
		{
			return m_poolId;
		}
		internal set
		{
			m_poolId = value;
		}
	}

	public ulong Size
	{
		get
		{
			return m_size;
		}
		internal set
		{
			m_size = value;
		}
	}

	public int DiskNumber
	{
		get
		{
			return m_number;
		}
		internal set
		{
			m_number = value;
		}
	}

	public ClusterDiskId DiskId => m_diskId;

	public string Node => m_node;

	public string Name
	{
		get
		{
			return m_diskName;
		}
		internal set
		{
			m_diskName = value;
		}
	}

	public string Id
	{
		get
		{
			if (m_id != null)
			{
				return m_id;
			}
			return m_diskId?.ToString();
		}
	}

	private void Construct(Cluster cluster, string node)
	{
		m_cluster = cluster;
		m_size = 0uL;
		m_diskName = null;
		m_node = node;
		m_partitions = new Collection<ClusterDiskPartition>();
	}

	internal ClusterResource CreateDiskResource(ManagementScope wmiScope, Guid key)
	{
		string diskName = m_diskName;
		string resourceName = ResourceHelp.GenerateResourceName(m_cluster, null, diskName, null);
		return CreateDiskResource(m_cluster, wmiScope, m_diskId, resourceName, key);
	}

	private ClusterResource CreateDiskResource(Cluster cluster, ManagementScope wmiScope, ClusterDiskId diskId, string resourceName, Guid key)
	{
		//Discarded unreachable code: IL_011b, IL_012d, IL_01e7, IL_01eb, IL_0223, IL_0225
		try
		{
			if (cluster.CurrentVersion > ClusterVersion.Windows7)
			{
				ManagementPath path = new ManagementPath(string.Format("{0}.Id='{1}'", "MSCluster_AvailableDisk", m_id.ToString()));
				ObjectGetOptions objectGetOptions = new ObjectGetOptions();
				objectGetOptions.Context.Add(ClusterableDisks.WmiDiskCacheKey, key.ToString());
				ManagementObject managementObject = new ManagementObject(wmiScope, path, objectGetOptions);
				try
				{
					ManagementBaseObject methodParameters = managementObject.GetMethodParameters("CreateStorageResource");
					try
					{
						methodParameters["ResourceName"] = wmiSuggestedResourceName;
						DebugLog.LogInfo("Creating disk with id {0}", m_id);
						InvokeMethodOptions invokeMethodOptions = new InvokeMethodOptions();
						invokeMethodOptions.Context.Add(ClusterableDisks.WmiDiskCacheKey, key.ToString());
						string obj = (string)managementObject.InvokeMethod("CreateStorageResource", methodParameters, invokeMethodOptions)["Path"];
						char[] trimChars = new char[1] { '"' };
						string resourceName2 = obj.Replace("MSCluster_Resource.Name=", "").Trim(trimChars);
						return cluster.GetResource(resourceName2);
					}
					finally
					{
						ManagementBaseObject managementBaseObject = methodParameters;
						IDisposable disposable = methodParameters;
						((IDisposable)methodParameters)?.Dispose();
					}
				}
				finally
				{
					ManagementObject managementObject2 = managementObject;
					IDisposable disposable2 = managementObject;
					((IDisposable)managementObject)?.Dispose();
				}
			}
			ClusterResource clusterResource = null;
			ClusterGroup availableStorageGroup = cluster.GetAvailableStorageGroup();
			try
			{
				DebugLog.LogInfo("Creating disk resource {0}", resourceName);
				string resourceTypeName = "Physical Disk";
				clusterResource = availableStorageGroup.CreateResource(resourceName, resourceTypeName);
				DebugLog.LogInfo("Saving disk properties for {0}", resourceName);
				ConfigureDiskResource(diskId, clusterResource, key);
				DebugLog.LogInfo("Disk properties for {0} succesfully saved", resourceName);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error creating & configuring disk {0}", resourceName);
				if (clusterResource != null)
				{
					DebugLog.LogInfo("Deleting disk {0}", resourceName);
					clusterResource.Delete();
				}
				ClusterResource clusterResource2 = clusterResource;
				IDisposable disposable3 = clusterResource;
				((IDisposable)clusterResource)?.Dispose();
				throw;
			}
			finally
			{
				ClusterGroup clusterGroup = availableStorageGroup;
				IDisposable disposable4 = availableStorageGroup;
				((IDisposable)availableStorageGroup)?.Dispose();
			}
			return clusterResource;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.CreateDiskResource_Failed_Text,
				resourceName
			});
		}
	}

	private ClusterResource CreateDiskResource(Cluster cluster, ClusterDiskId diskId, string resourceName, Guid key)
	{
		//Discarded unreachable code: IL_003d
		ManagementScope clusterOrNodeConnection = WmiHelper.GetClusterOrNodeConnection(m_cluster.FqdnName, m_cluster.GetCurrentNodeNames(NodeState.Up));
		try
		{
			return CreateDiskResource(cluster, clusterOrNodeConnection, diskId, resourceName, key);
		}
		finally
		{
			if (clusterOrNodeConnection is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	public void ConfigureDiskResource(ClusterResource diskResource)
	{
		ConfigureDiskResource(m_diskId, diskResource, m_key);
	}

	private void ConfigureDiskResource(ClusterDiskId diskId, ClusterResource diskResource, Guid key)
	{
		if (diskResource.Cluster.CurrentVersion > ClusterVersion.Windows7)
		{
			ManagementPath path = new ManagementPath(string.Format("{0}.Name='{1}'", "MSCluster_Resource", diskResource.Id.ToString()));
			ObjectGetOptions objectGetOptions = new ObjectGetOptions();
			objectGetOptions.Context.Add(ClusterableDisks.WmiDiskCacheKey, key.ToString());
			using ManagementObject managementObject = new ManagementObject(WmiHelper.GetClusterWmiConnection(diskResource.Cluster.ConnectedTo), path, objectGetOptions);
			ManagementBaseObject methodParameters = managementObject.GetMethodParameters("AttachStorageDevice");
			try
			{
				methodParameters["StorageDevice"] = string.Format("{0}.Id='{1}'", "MSCluster_AvailableDisk", m_id);
				DebugLog.LogInfo("Attaching disk with id {0}", m_id);
				InvokeMethodOptions invokeMethodOptions = new InvokeMethodOptions();
				invokeMethodOptions.Context.Add(ClusterableDisks.WmiDiskCacheKey, key.ToString());
				managementObject.InvokeMethod("AttachStorageDevice", methodParameters, invokeMethodOptions);
				return;
			}
			finally
			{
				if (methodParameters != null)
				{
					ManagementBaseObject managementBaseObject = methodParameters;
					IDisposable disposable = methodParameters;
					((IDisposable)methodParameters).Dispose();
				}
			}
		}
		PropertyCollection privateProperties = diskResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		switch (diskId.IdType)
		{
		case DiskIdType.Guid:
		{
			Guid diskGuid = ((ClusterDiskGuidId)diskId).DiskGuid;
			string name3 = "DiskIdGuid";
			privateProperties.GetProperty(name3).Value = diskGuid.ToString("B");
			if (!(diskResource.Cluster.CurrentVersion == ClusterVersion.WindowsServer2008))
			{
				string name4 = "DiskSignature";
				privateProperties.GetProperty(name4).Value = Convert.ToUInt32(0);
			}
			break;
		}
		case DiskIdType.Signature:
		{
			ClusterDiskSignatureId clusterDiskSignatureId = (ClusterDiskSignatureId)diskId;
			string name = "DiskSignature";
			privateProperties.GetProperty(name).Value = clusterDiskSignatureId.Signature;
			if (!(diskResource.Cluster.CurrentVersion == ClusterVersion.WindowsServer2008))
			{
				string name2 = "DiskIdGuid";
				privateProperties.GetProperty(name2).Value = string.Empty;
			}
			break;
		}
		}
		privateProperties.SaveChanges();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsClustered(Cluster cluster, ClusterDiskId diskId)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		if (diskId == null)
		{
			throw new ArgumentNullException("diskId");
		}
		return GetResource(cluster, diskId) != null;
	}

	public static ClusterResource GetResource(Cluster cluster, ClusterDiskId diskId)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		if (diskId == null)
		{
			throw new ArgumentNullException("diskId");
		}
		ClusterResourceTypeCollection clusterResourceTypeCollection = cluster.GetStorageClassResourceTypes();
		try
		{
			IEnumerator<ClusterResourceType> enumerator = clusterResourceTypeCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResourceType current = enumerator.Current;
					IEnumerator<ClusterResource> enumerator2 = cluster.GetResources(current.Name).GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ClusterResource current2 = enumerator2.Current;
							if (current2.State != ResourceState.Online)
							{
								continue;
							}
							ClusterDisk clusterDisk = current2.Storage_GetDiskInfo(includeMountPoints: false);
							switch (diskId.IdType)
							{
							case DiskIdType.Guid:
							{
								ClusterDiskGuidId clusterDiskGuidId = (ClusterDiskGuidId)diskId;
								ClusterDiskId diskId3 = clusterDisk.m_diskId;
								if (diskId3 is ClusterDiskGuidId clusterDiskGuidId2)
								{
									Guid diskGuid = clusterDiskGuidId2.DiskGuid;
									if (clusterDiskGuidId.DiskGuid.Equals(diskGuid))
									{
										return current2;
									}
								}
								break;
							}
							case DiskIdType.Signature:
							{
								ClusterDiskSignatureId clusterDiskSignatureId = (ClusterDiskSignatureId)diskId;
								ClusterDiskId diskId2 = clusterDisk.m_diskId;
								if (diskId2 is ClusterDiskSignatureId clusterDiskSignatureId2 && clusterDiskSignatureId.Signature == clusterDiskSignatureId2.Signature)
								{
									return current2;
								}
								break;
							}
							}
						}
					}
					finally
					{
						IEnumerator<ClusterResource> enumerator3 = enumerator2;
						IDisposable disposable = enumerator2;
						enumerator2?.Dispose();
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResourceType> enumerator4 = enumerator;
				IDisposable disposable2 = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable3 = clusterResourceTypeCollection as IDisposable;
			if (disposable3 != null)
			{
				disposable3.Dispose();
			}
		}
		return null;
	}

	public static ClusterGroup GetOwnerGroup(Cluster cluster, ClusterDiskId diskId)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		if (diskId == null)
		{
			throw new ArgumentNullException("diskId");
		}
		return GetResource(cluster, diskId)?.GetOwnerGroup();
	}

	public virtual int CompareTo(ClusterDisk other)
	{
		if (other == null)
		{
			return -1;
		}
		string diskName = other.m_diskName;
		return m_diskName.CompareTo(diskName);
	}

	internal unsafe static Collection<ClusterDisk> ParseClusterableDisks(Cluster cluster, string sourceNode, CClusPropValueList* listDisks)
	{
		//IL_0015: Expected I, but got I8
		//IL_0065: Expected I, but got I8
		//IL_0096: Expected I, but got I8
		//IL_00bf: Expected I, but got I8
		Collection<ClusterDisk> collection = new Collection<ClusterDisk>();
		ClusterDisk clusterDisk = null;
		global::_003CModule_003E.CClusPropValueList_002EScMoveToFirstValue(listDisks);
		CClusPropValueList* ptr = (CClusPropValueList*)((ulong)(nint)listDisks + 8uL);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
		uint num;
		do
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER, ptr, 8);
			switch ((uint)(*(int*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER))))
			{
			case 851969u:
			{
				ClusterDiskPartition item2 = new ClusterDiskPartition((CLUS_PARTITION_INFO_EX*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
				clusterDisk.m_partitions.Add(item2);
				break;
			}
			case 786438u:
				clusterDisk.m_size = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>((void*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
				break;
			case 720899u:
			{
				string g = InteropHelp.WstrToString((ushort*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
				Guid guid = new Guid(g);
				clusterDisk = new ClusterDisk(cluster, guid, sourceNode);
				collection.Add(clusterDisk);
				break;
			}
			case 524289u:
			{
				ClusterDiskPartition item = new ClusterDiskPartition((CLUS_PARTITION_INFO*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
				clusterDisk.m_partitions.Add(item);
				break;
			}
			case 458754u:
				clusterDisk.m_number = *(int*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8);
				break;
			case 327682u:
				clusterDisk = new ClusterDisk(cluster, *(uint*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8), sourceNode);
				collection.Add(clusterDisk);
				break;
			}
			num = global::_003CModule_003E.CClusPropValueList_002EScMoveToNextValue(listDisks);
		}
		while (num != 259 && num == 0);
		return collection;
	}

	internal static ClusterDisk Create(Cluster cluster, ManagementScope wmiScope, ManagementObject managementObject, Guid key)
	{
		uint num = ((managementObject["Signature"] != null) ? ((uint)managementObject["Signature"]) : 0u);
		string text = ((managementObject["GptGuid"] == null) ? null : ((string)managementObject["GptGuid"]));
		ClusterDisk clusterDisk = ((num != 0) ? new ClusterDisk(cluster, (string)managementObject["Id"], num, (string)managementObject["Node"], (string)managementObject["ResourceName"]) : ((!(text != null)) ? new ClusterDisk(cluster, (string)managementObject["Id"], (string)managementObject["Node"], (string)managementObject["ResourceName"]) : new ClusterDisk(gptGuid: new Guid(text), cluster: cluster, id: (string)managementObject["Id"], node: (string)managementObject["Node"], wmiSuggestedResourceName: (string)managementObject["ResourceName"])));
		ulong size = ((managementObject["Size"] == null) ? 0 : ((ulong)managementObject["Size"]));
		clusterDisk.m_size = size;
		if (managementObject["Number"] != null)
		{
			int number = (int)(uint)managementObject["Number"];
			clusterDisk.m_number = number;
		}
		string poolId = ((managementObject["StoragePoolId"] == null) ? null : ((string)managementObject["StoragePoolId"]));
		clusterDisk.m_poolId = poolId;
		string virtualDiskId = ((managementObject["VirtualDiskId"] == null) ? null : ((string)managementObject["VirtualDiskId"]));
		clusterDisk.m_virtualDiskId = virtualDiskId;
		clusterDisk.m_key = key;
		ObjectQuery query = new ObjectQuery(string.Format(CultureInfo.CurrentCulture, "Associators of {{{0}}} where ResultClass={1}", managementObject.Path, "MSCluster_AvailableDiskPartition"));
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wmiScope, query);
		managementObjectSearcher.Options.Context.Add(ClusterableDisks.WmiDiskCacheKey, key.ToString());
		ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
		try
		{
			ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ManagementObject managementObject2 = (ManagementObject)enumerator.Current;
					clusterDisk.AddPartition(new ClusterDiskPartition(managementObject2));
				}
				return clusterDisk;
			}
			finally
			{
				ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = enumerator;
				IDisposable disposable = enumerator;
				((IDisposable)enumerator)?.Dispose();
			}
		}
		finally
		{
			ManagementObjectCollection managementObjectCollection2 = managementObjectCollection;
			IDisposable disposable2 = managementObjectCollection;
			((IDisposable)managementObjectCollection)?.Dispose();
		}
	}

	internal static ClusterDisk Create(Cluster cluster, ManagementObject managementObject, Guid key)
	{
		//Discarded unreachable code: IL_0030
		ManagementScope clusterOrNodeConnection = WmiHelper.GetClusterOrNodeConnection(cluster.FqdnName, cluster.GetCurrentNodeNames(NodeState.Up));
		try
		{
			return Create(cluster, clusterOrNodeConnection, managementObject, key);
		}
		finally
		{
			if (clusterOrNodeConnection is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	internal void AddPartition(ClusterDiskPartition partition)
	{
		m_partitions.Add(partition);
	}

	internal ClusterDisk(Cluster cluster, string id, string node, string wmiSuggestedResourceName)
	{
		Construct(cluster, node);
		m_id = id;
		Guid guid = new Guid(id);
		m_diskId = ClusterDiskId.CreateDiskGuid(guid);
		this.wmiSuggestedResourceName = wmiSuggestedResourceName;
	}

	internal ClusterDisk(Cluster cluster, string id, Guid gptGuid, string node, string wmiSuggestedResourceName)
	{
		Construct(cluster, node);
		m_id = id;
		m_diskId = ClusterDiskId.CreateDiskGuid(gptGuid);
		this.wmiSuggestedResourceName = wmiSuggestedResourceName;
	}

	internal ClusterDisk(Cluster cluster, string id, uint signature, string node, string wmiSuggestedResourceName)
	{
		Construct(cluster, node);
		m_id = id;
		m_diskId = ClusterDiskId.CreateDiskSignature(signature);
		this.wmiSuggestedResourceName = wmiSuggestedResourceName;
	}

	internal ClusterDisk(Cluster cluster, Guid guid, string node)
	{
		Construct(cluster, node);
		m_diskId = ClusterDiskId.CreateDiskGuid(guid);
	}

	internal ClusterDisk(Cluster cluster, uint signature, string node)
	{
		Construct(cluster, node);
		m_diskId = ClusterDiskId.CreateDiskSignature(signature);
	}
}
