using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public class ClusterableDisks : IDisposable
{
	private class ClusterableDiskInfo
	{
		private ClusterDisk m_disk;

		private ClusterNode m_node;

		public ClusterNode Node => m_node;

		public ClusterDisk Disk => m_disk;

		public ClusterableDiskInfo(ClusterDisk disk, ClusterNode node)
		{
			m_disk = disk;
			m_node = node;
		}
	}

	[DefaultMember("Item")]
	private class ClusterableDiskInfoCollection : Collection<ClusterableDiskInfo>
	{
	}

	private class AvailableDriveLetterInput
	{
		private uint m_existingDriveLetterMask;

		private ClusterNode m_filterNode;

		public ClusterNode FilterNode => m_filterNode;

		public uint FilterDriveLetterMask => m_existingDriveLetterMask;

		public AvailableDriveLetterInput(uint existingDriveLetterMask, ClusterNode filterNode)
		{
			m_existingDriveLetterMask = existingDriveLetterMask;
			m_filterNode = filterNode;
		}
	}

	private Collection<ClusterDisk> m_clusterableDisks;

	private Dictionary<string, ClusterableDiskInfo> m_clusterDiskInfo;

	private Cluster m_cluster;

	private bool m_continueOnError;

	private List<Exception> m_errors;

	private const string WmiDiskColumns = "Id, Name, GptGuid, Number, ScsiBus, ScsiLun, ScsiPort, ScsiTargetId, Signature, Size, ResourceName, Node, VirtualDiskId, StoragePoolId";

	private Guid m_key;

	private static uint NeverAvailableDisks = 7u;

	private EventHandler<OperationProgressEventArgs> _003Cbacking_store_003ECreateDiskResourcesProgress;

	internal const string WmiAvailableDiskClassName = "MSCluster_AvailableDisk";

	internal const string WmiAddAvailableDiskMethod = "CreateStorageResource";

	private const string WmiClusterNamespace = "\\\\{0}\\root\\mscluster";

	internal static string WmiAllDisksKey => "ALL_AVAILABLE_DISKS_KEY";

	internal static string WmiDiskCacheKey => "AVAILABLE_DISKS_KEY";

	public ICollection<Exception> Errors => m_errors;

	public ICollection<ClusterDisk> AvailableDisks => m_clusterableDisks;

	public bool ContinueCreateDisksOnError
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_continueOnError;
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			m_continueOnError = value;
		}
	}

	[SpecialName]
	public event EventHandler<OperationProgressEventArgs> CreateDiskResourcesProgress
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ECreateDiskResourcesProgress = (EventHandler<OperationProgressEventArgs>)Delegate.Combine(_003Cbacking_store_003ECreateDiskResourcesProgress, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ECreateDiskResourcesProgress = (EventHandler<OperationProgressEventArgs>)Delegate.Remove(_003Cbacking_store_003ECreateDiskResourcesProgress, value);
		}
	}

	private IDictionary<int, uint> GetNewDriveLettersForDisk(ClusterDisk disk, ClusterNode diskNode)
	{
		Dictionary<int, uint> dictionary = new Dictionary<int, uint>();
		foreach (ClusterDiskPartition partition in disk.Partitions)
		{
			uint driveLetterMask = partition.DriveLetterMask;
			DebugLog.LogInfo("Disk {0}\\{1} - drive letter mask 0x{2:x}", disk.Name, partition.Name, driveLetterMask);
			if (driveLetterMask != 0)
			{
				uint availableDriveLetterMask = GetAvailableDriveLetterMask(driveLetterMask, diskNode);
				driveLetterMask = DetermineDriveLetterMask(driveLetterMask, availableDriveLetterMask);
				dictionary.Add((int)partition.PartitionNumber, driveLetterMask);
			}
		}
		return dictionary;
	}

	private uint GetAvailableDriveLetterMask(uint existingDriveLetterMask, ClusterNode filterNode)
	{
		uint num = 1073741823u;
		AvailableDriveLetterInput inputData = new AvailableDriveLetterInput(existingDriveLetterMask, filterNode);
		ClusterNodeCollection nodes = m_cluster.GetNodes();
		foreach (uint item in new ActionMultiplexor<AvailableDriveLetterInput, ClusterNode, uint>(GetNodeAvailableDriveLetters).Execute(inputData, nodes))
		{
			num &= item;
		}
		num &= ~NeverAvailableDisks;
		DebugLog.LogInfo("Available drive letters 0x{0:x}", num);
		return num;
	}

	private uint DetermineDriveLetterMask(uint existingDriveLetterMask, uint availableDriveLetterMask)
	{
		uint result;
		if (0 != (existingDriveLetterMask & availableDriveLetterMask))
		{
			result = existingDriveLetterMask;
		}
		else
		{
			uint num = existingDriveLetterMask;
			if (existingDriveLetterMask != 0)
			{
				while (0 == (num & availableDriveLetterMask))
				{
					num <<= 1;
					if (num == 0)
					{
						break;
					}
				}
			}
			result = num;
			if (num == 0)
			{
				uint num2 = 1u;
				while (0 == (num2 & availableDriveLetterMask))
				{
					num2 <<= 1;
					if (num2 == 0)
					{
						break;
					}
				}
				result = num2;
			}
		}
		return result;
	}

	private uint FindNextAvailableDriveLetter(uint driveLetterMask, uint availableDriveLetterMask)
	{
		if (driveLetterMask != 0)
		{
			while (0 == (driveLetterMask & availableDriveLetterMask))
			{
				driveLetterMask <<= 1;
				if (driveLetterMask == 0)
				{
					break;
				}
			}
		}
		return driveLetterMask;
	}

	private void AssignDiskNames()
	{
		Dictionary<Guid, string> currentResourceNames = m_cluster.GetCurrentResourceNames();
		int num = 1;
		foreach (ClusterDisk clusterableDisk in m_clusterableDisks)
		{
			string text = null;
			do
			{
				int num2 = num;
				num++;
				text = string.Format(CultureInfo.InvariantCulture, Resources.DiskNameFormatter_Text, num2);
			}
			while (currentResourceNames.ContainsValue(text));
			clusterableDisk.Name = text;
		}
	}

	private void BuildClusterableDiskCollection()
	{
		m_clusterableDisks = new Collection<ClusterDisk>();
		Dictionary<string, ClusterableDiskInfo>.ValueCollection.Enumerator enumerator = m_clusterDiskInfo.Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				ClusterableDiskInfo current = enumerator.Current;
				m_clusterableDisks.Add(current.Disk);
			}
			while (enumerator.MoveNext());
		}
	}

	private int GetNumberOfDriveLetters(ClusterDisk disk)
	{
		int num = 0;
		foreach (ClusterDiskPartition partition in disk.Partitions)
		{
			if (partition.DriveLetter != null)
			{
				num++;
			}
		}
		return num;
	}

	private Dictionary<string, ClusterableDiskInfoCollection> GetDisksFromNodes(ClusterNodeCollection nodes)
	{
		ClusterableDiskInfoCollection clusterableDiskInfoCollection = null;
		Dictionary<string, ClusterableDiskInfoCollection> dictionary = new Dictionary<string, ClusterableDiskInfoCollection>(StringComparer.OrdinalIgnoreCase);
		foreach (ICollection<ClusterableDiskInfo> item in new ActionMultiplexor<object, ClusterNode, ICollection<ClusterableDiskInfo>>(GetDisksFromNode).Execute(null, nodes))
		{
			IEnumerator<ClusterableDiskInfo> enumerator2 = item.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					ClusterableDiskInfo current = enumerator2.Current;
					clusterableDiskInfoCollection = null;
					if (!dictionary.TryGetValue(current.Disk.DiskId.ToString(), out clusterableDiskInfoCollection))
					{
						clusterableDiskInfoCollection = new ClusterableDiskInfoCollection();
						dictionary.Add(current.Disk.DiskId.ToString(), clusterableDiskInfoCollection);
					}
					clusterableDiskInfoCollection.Add(current);
				}
			}
			finally
			{
				IEnumerator<ClusterableDiskInfo> enumerator3 = enumerator2;
				IDisposable disposable = enumerator2;
				enumerator2?.Dispose();
			}
		}
		return dictionary;
	}

	private void OnCreateDiskResourcesProgress(OperationProgressWarningLevel warningLevel, int workDone, int totalWork, string message)
	{
		OperationProgressEventArgs e = new OperationProgressEventArgs(warningLevel, message, workDone, totalWork);
		_003Cbacking_store_003ECreateDiskResourcesProgress?.Invoke(this, e);
	}

	private void OnCreateDiskResourcesProgress(OperationProgressWarningLevel warningLevel, int workDone, int totalWork, string format, object arg0)
	{
		string message = string.Format(CultureInfo.CurrentCulture, format, arg0);
		OperationProgressEventArgs e = new OperationProgressEventArgs(warningLevel, message, workDone, totalWork);
		_003Cbacking_store_003ECreateDiskResourcesProgress?.Invoke(this, e);
	}

	private ICollection<ClusterableDiskInfo> GetDisksFromNode(object none, ClusterNode node)
	{
		Collection<ClusterableDiskInfo> collection = new Collection<ClusterableDiskInfo>();
		if (node.CanProcessClusterServiceCommands)
		{
			Collection<ClusterDisk> clusterableDisks = node.GetClusterableDisks();
			DebugLog.LogInfo("Node {0} reports {1} clusterable disks", node.Name, clusterableDisks.Count);
			foreach (ClusterDisk item2 in clusterableDisks)
			{
				DebugLog.LogInfo("Node {0} has clusterable disk {1}", node.Name, item2.DiskId);
				ClusterableDiskInfo item = new ClusterableDiskInfo(item2, node);
				collection.Add(item);
			}
		}
		else
		{
			DebugLog.LogInfo("Node {0} not being asked for clusterable disks due to its state {1}", node.Name, node.State);
		}
		return collection;
	}

	private int GetNumberOfUpNodes(ClusterNodeCollection nodes)
	{
		int num = 0;
		foreach (ClusterNode node in nodes)
		{
			if (node.CanProcessClusterServiceCommands)
			{
				num++;
			}
		}
		return num;
	}

	private uint GetNodeAvailableDriveLetters(AvailableDriveLetterInput input, ClusterNode node)
	{
		uint num = uint.MaxValue;
		if (node.CanProcessClusterServiceCommands)
		{
			num = node.GetAvailableDriveLetters();
			if (0 == string.Compare(node.Name, input.FilterNode.Name, StringComparison.OrdinalIgnoreCase))
			{
				DebugLog.LogInfo("Marking drive letter 0x{0:x} available drive letters on {1}", input.FilterDriveLetterMask, node.Name);
				num |= input.FilterDriveLetterMask;
			}
		}
		DebugLog.LogInfo("Available drive letters on {0} are 0x{1:x}", node.Name, num);
		return num;
	}

	private Dictionary<string, ClusterableDiskInfo> GetAvailableClusterDisksFromCluster(ManagementScope wmiScope, Guid poolId, [MarshalAs(UnmanagedType.U1)] bool all)
	{
		Dictionary<string, ClusterableDiskInfo> dictionary = new Dictionary<string, ClusterableDiskInfo>(StringComparer.OrdinalIgnoreCase);
		_ = string.Empty;
		ObjectQuery query = new ObjectQuery((!(poolId == Guid.Empty)) ? string.Format(CultureInfo.CurrentCulture, "SELECT {0} FROM {1} WHERE StoragePoolId = '{2}'", "Id, Name, GptGuid, Number, ScsiBus, ScsiLun, ScsiPort, ScsiTargetId, Signature, Size, ResourceName, Node, VirtualDiskId, StoragePoolId", "MSCluster_AvailableDisk", poolId.ToString("B")) : string.Format(CultureInfo.CurrentCulture, "SELECT {0} FROM {1}", "Id, Name, GptGuid, Number, ScsiBus, ScsiLun, ScsiPort, ScsiTargetId, Signature, Size, ResourceName, Node, VirtualDiskId, StoragePoolId", "MSCluster_AvailableDisk"));
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wmiScope, query);
		string name = "AVAILABLE_DISKS_KEY";
		managementObjectSearcher.Options.Context.Add(name, m_key.ToString());
		if (all)
		{
			bool flag = all;
			string name2 = "ALL_AVAILABLE_DISKS_KEY";
			managementObjectSearcher.Options.Context.Add(name2, flag.ToString());
		}
		ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
		try
		{
			ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ManagementObject managementObject = (ManagementObject)enumerator.Current;
					ClusterableDiskInfo clusterableDiskInfo = new ClusterableDiskInfo(ClusterDisk.Create(m_cluster, wmiScope, managementObject, m_key), null);
					if (!dictionary.ContainsKey(clusterableDiskInfo.Disk.Id))
					{
						dictionary.Add(clusterableDiskInfo.Disk.Id, clusterableDiskInfo);
					}
				}
				return dictionary;
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

	private Dictionary<string, ClusterableDiskInfo> GetAvailableClusterDisksFromCluster(Guid poolId, [MarshalAs(UnmanagedType.U1)] bool all)
	{
		//Discarded unreachable code: IL_003a
		ManagementScope clusterOrNodeConnection = WmiHelper.GetClusterOrNodeConnection(m_cluster.FqdnName, m_cluster.GetCurrentNodeNames(NodeState.Up));
		try
		{
			return GetAvailableClusterDisksFromCluster(clusterOrNodeConnection, poolId, all);
		}
		finally
		{
			if (clusterOrNodeConnection is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	public ClusterableDisks()
	{
		m_clusterableDisks = null;
		m_clusterDiskInfo = null;
		m_cluster = null;
		m_continueOnError = false;
		m_errors = new List<Exception>();
		Guid key = Guid.NewGuid();
		m_key = key;
	}

	private void _007EClusterableDisks()
	{
		m_clusterableDisks = null;
		m_cluster = null;
		Dictionary<string, ClusterableDiskInfo> clusterDiskInfo = m_clusterDiskInfo;
		if (clusterDiskInfo == null)
		{
			return;
		}
		Dictionary<string, ClusterableDiskInfo>.ValueCollection.Enumerator enumerator = clusterDiskInfo.Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				((IDisposable)enumerator.Current.Node)?.Dispose();
			}
			while (enumerator.MoveNext());
		}
		m_clusterDiskInfo = null;
	}

	public void DetermineClusterableDisks(Cluster cluster, Guid poolId, [MarshalAs(UnmanagedType.U1)] bool all)
	{
		//Discarded unreachable code: IL_01d2
		m_clusterableDisks = null;
		m_cluster = cluster;
		try
		{
			if (cluster.CurrentVersion > ClusterVersion.Windows7)
			{
				m_clusterDiskInfo = GetAvailableClusterDisksFromCluster(poolId, all);
			}
			else
			{
				ClusterNodeCollection nodes = m_cluster.GetNodes();
				int numberOfUpNodes = GetNumberOfUpNodes(nodes);
				Dictionary<string, ClusterableDiskInfoCollection> disksFromNodes = GetDisksFromNodes(nodes);
				m_clusterDiskInfo = new Dictionary<string, ClusterableDiskInfo>(StringComparer.OrdinalIgnoreCase);
				Dictionary<string, ClusterableDiskInfoCollection>.ValueCollection.Enumerator enumerator = disksFromNodes.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ClusterableDiskInfoCollection current = enumerator.Current;
					if (current.Count == numberOfUpNodes)
					{
						ClusterableDiskInfo clusterableDiskInfo = null;
						int num = -1;
						IEnumerator<ClusterableDiskInfo> enumerator2 = current.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								ClusterableDiskInfo current2 = enumerator2.Current;
								int numberOfDriveLetters = GetNumberOfDriveLetters(current2.Disk);
								if (numberOfDriveLetters > num)
								{
									num = numberOfDriveLetters;
									clusterableDiskInfo = current2;
								}
							}
						}
						finally
						{
							IEnumerator<ClusterableDiskInfo> enumerator3 = enumerator2;
							IDisposable disposable = enumerator2;
							enumerator2?.Dispose();
						}
						DebugLog.LogInfo("Disk {0} is clusterable", clusterableDiskInfo.Disk.DiskId);
						m_clusterDiskInfo.Add(clusterableDiskInfo.Disk.DiskId.ToString(), clusterableDiskInfo);
					}
					else
					{
						ClusterableDiskInfo clusterableDiskInfo2 = current[0];
						DebugLog.LogInfo("Disk {0} not clusterable since only {1} nodes can see it", clusterableDiskInfo2.Disk.DiskId, current.Count);
					}
				}
			}
			DebugLog.LogInfo("Total clusterable disks found: {0}", m_clusterDiskInfo.Count);
			BuildClusterableDiskCollection();
			AssignDiskNames();
			List<ClusterDisk> list = new List<ClusterDisk>(m_clusterableDisks);
			list.Sort();
			m_clusterableDisks = new Collection<ClusterDisk>(list);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.DetermineClusterableDisks_Failed_Text,
				m_cluster.Name
			});
		}
	}

	public void DetermineClusterableDisks(Cluster cluster, Guid poolId)
	{
		DetermineClusterableDisks(cluster, poolId, all: false);
	}

	public void DetermineClusterableDisks(Cluster cluster, [MarshalAs(UnmanagedType.U1)] bool all)
	{
		DetermineClusterableDisks(cluster, Guid.Empty, all);
	}

	public void DetermineClusterableDisks(Cluster cluster)
	{
		DetermineClusterableDisks(cluster, Guid.Empty, all: false);
	}

	public IEnumerable<ClusterResource> CreateDiskResources(IEnumerable<ClusterDisk> clusterableDisks)
	{
		int num = 0;
		int totalWork = m_clusterableDisks.Count * 4;
		int num2 = 1;
		m_errors.Clear();
		List<ClusterResource> list = new List<ClusterResource>();
		ManagementScope clusterOrNodeConnection = WmiHelper.GetClusterOrNodeConnection(m_cluster.FqdnName, m_cluster.GetCurrentNodeNames(NodeState.Up));
		try
		{
			IEnumerator<ClusterDisk> enumerator = clusterableDisks.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterDisk current = enumerator.Current;
					num++;
					ClusterNode diskNode = null;
					Dictionary<string, ClusterableDiskInfo>.ValueCollection.Enumerator enumerator2 = m_clusterDiskInfo.Values.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						ClusterableDiskInfo current2 = enumerator2.Current;
						if (current2.Disk.DiskId.Equals(current.DiskId))
						{
							diskNode = current2.Node;
						}
					}
					try
					{
						IDictionary<int, uint> newDriveLettersForDisk = GetNewDriveLettersForDisk(current, diskNode);
						int workDone = num2;
						num2++;
						OnCreateDiskResourcesProgress(OperationProgressWarningLevel.Information, workDone, totalWork, Resources.ClusterableDisks_CreateDiskResource_Text, current.Name);
						ClusterResource clusterResource = current.CreateDiskResource(clusterOrNodeConnection, m_key);
						list.Add(clusterResource);
						try
						{
							int workDone2 = num2;
							num2++;
							OnCreateDiskResourcesProgress(OperationProgressWarningLevel.Information, workDone2, totalWork, Resources.ClusterableDisks_BringOnline_Text, current.Name);
							clusterResource.BringOnline();
							int workDone3 = num2;
							num2++;
							OnCreateDiskResourcesProgress(OperationProgressWarningLevel.Information, workDone3, totalWork, Resources.ClusterableDisks_AssignDriveLetters_Text, current.Name);
							IEnumerator<KeyValuePair<int, uint>> enumerator3 = newDriveLettersForDisk.GetEnumerator();
							try
							{
								while (enumerator3.MoveNext())
								{
									ValueType valueType = enumerator3.Current;
									if (((KeyValuePair<int, uint>)valueType).Value == 0)
									{
										OnCreateDiskResourcesProgress(OperationProgressWarningLevel.Warning, num2, totalWork, Resources.ClusterableDisks_NoDriveLetter_Text, current.Name);
										continue;
									}
									DebugLog.LogInfo("Disk {0}\\{1} - set drive letter 0x{2:x}", current.Name, ((KeyValuePair<int, uint>)valueType).Key, ((KeyValuePair<int, uint>)valueType).Value);
									clusterResource.Storage_SetDriveLetter((uint)((KeyValuePair<int, uint>)valueType).Key, ((KeyValuePair<int, uint>)valueType).Value);
								}
							}
							finally
							{
								IEnumerator<KeyValuePair<int, uint>> enumerator4 = enumerator3;
								IDisposable disposable = enumerator3;
								enumerator3?.Dispose();
							}
							int workDone4 = num2;
							num2++;
							OnCreateDiskResourcesProgress(OperationProgressWarningLevel.Information, workDone4, totalWork, Resources.ClusterableDisks_FinishedWithDisk_Text, current.Name);
						}
						finally
						{
							ClusterResource clusterResource2 = clusterResource;
							IDisposable disposable2 = clusterResource;
							((IDisposable)clusterResource)?.Dispose();
						}
					}
					catch (ApplicationException ex)
					{
						Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
						if (firstException == null || firstException.NativeErrorCode != -2147018933)
						{
							num2 = num * 4;
							OnCreateDiskResourcesProgress(OperationProgressWarningLevel.Error, num2, totalWork, Resources.ClusterableDisks_ErrorWithDisk_Text, current.Name);
							if (!m_continueOnError)
							{
								throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
								{
									Resources.CreateDiskResources_Failed_Text,
									m_cluster.Name
								});
							}
							ExceptionHelp.LogException(ex, "Error while clustering disks");
							m_errors.Add(ex);
							string[] args = new string[0];
							string exceptionReportMessage = ExceptionHelp.GetExceptionReportMessage(ex, args);
							ClusterLog.AdminEvents.WriteDiskCreationErrorEvent(exceptionReportMessage);
						}
					}
				}
				return list;
			}
			finally
			{
				IEnumerator<ClusterDisk> enumerator5 = enumerator;
				IDisposable disposable3 = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			if (clusterOrNodeConnection is IDisposable disposable4)
			{
				disposable4.Dispose();
			}
		}
	}

	public IEnumerable<ClusterResource> CreateDiskResources()
	{
		return CreateDiskResources(m_clusterableDisks);
	}

	[SpecialName]
	protected void raise_CreateDiskResourcesProgress(object value0, OperationProgressEventArgs value1)
	{
		_003Cbacking_store_003ECreateDiskResourcesProgress?.Invoke(value0, value1);
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EClusterableDisks();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
