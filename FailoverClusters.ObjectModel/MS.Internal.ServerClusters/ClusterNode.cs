using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public class ClusterNode : ClusterItem
{
	private volatile string m_name;

	private volatile string m_lastLoadedName;

	private Guid m_Id;

	private NodeState m_state;

	private ClusterNodeDrainStatus m_drainStatus;

	private ClusterNodeStatus m_Status;

	private Cluster m_cluster;

	private SafeNodeHandle m_hNode;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private ServiceController m_nodeServiceController;

	private volatile string m_nodeId;

	private bool m_closed;

	private volatile bool m_deleting;

	private static object m_creationLockObject = new object();

	private object m_stateLock;

	private object m_drainStatusLock;

	private object m_StatusLock;

	private object m_nodeFqdnLock;

	private string m_fqdnName;

	private Random rnd = new Random();

	private ManagementObject m_clusterSubSystem = null;

	private const string WmiStorageSubSystemClassName = "MSFT_StorageSubSystem";

	private const string WmiVirtualDiskClassName = "MSFT_VirtualDisk";

	private const string WmiPhysicalDiskClassName = "MSFT_PhysicalDisk";

	private const string WmiStorageNodeClassName = "MSFT_StorageNode";

	private const string WmiStoragePoolClassName = "MSFT_StoragePool";

	private const string WmiStorageNodeToPhysicalDiskClassName = "MSFT_StorageNodeToPhysicalDisk";

	private const string WmiVirtualDiskHealtyPropertyName = "HealthStatus";

	private const string WmiUniqueIdPropertyName = "UniqueId";

	private const string WmiNamePropertyName = "Name";

	private const string WmiIsPrimordialPropertyName = "IsPrimordial";

	private const ushort VirtualDiskHealthyStatusHealthy = 0;

	private volatile ClusterGroupCollection m_groups;

	private object m_groupsLock;

	private EventHandler _003Cbacking_store_003EStateChanged;

	private EventHandler _003Cbacking_store_003EPropertiesChanged;

	private EventHandler<DeletingEventArgs> _003Cbacking_store_003EDeleting;

	private EventHandler _003Cbacking_store_003EDeleted;

	public static string ServiceName = "ClusSvc";

	public static TimeSpan DefaultEvictTimeout => TimeSpan.FromMinutes(3.0);

	public int NodeWeight => GetNodeWeight();

	public int DynamicWeight => GetDynamicWeight();

	public ClusterNodeDrainStatus DrainStatus
	{
		get
		{
			if (!ClusterItem.CachingDisabled)
			{
				_ = IsDeleted;
			}
			LoadDrainStatus();
			return m_drainStatus;
		}
	}

	public Cluster Cluster => m_cluster;

	public NodeClusterState ClusterState
	{
		get
		{
			ThreadWatchdog.PerformUIThreadCheck();
			return GetClusterState(m_name);
		}
	}

	public bool IsNodeJoined
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CanProcessClusterServiceCommands;
		}
	}

	public bool CanProcessClusterServiceCommands
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			if (State != 0)
			{
				TimeSpan timeSpan = new TimeSpan(0, 2, 0);
				TimeSpan timeSpan2 = new TimeSpan(0, 0, 1);
				TimeSpan timeSpan3 = default(TimeSpan);
				Refresh();
				NodeState state = State;
				if (state == NodeState.Joining)
				{
					while (timeSpan3 < timeSpan)
					{
						Thread.Sleep(timeSpan2);
						timeSpan3 += timeSpan2;
						Refresh();
						state = State;
						if (state != NodeState.Joining)
						{
							break;
						}
					}
				}
				if (state != 0 && state != NodeState.Paused)
				{
					return false;
				}
			}
			return true;
		}
	}

	public NodeState SafeNodeState
	{
		get
		{
			ThreadWatchdog.PerformUIThreadCheck();
			NodeState nodeState = m_state;
			if (ClusterItem.CachingDisabled || IsDeleted)
			{
				nodeState = NodeState.Unknown;
			}
			try
			{
				while (nodeState == NodeState.Unknown && !IsDeleted)
				{
					LoadState();
					nodeState = m_state;
				}
			}
			catch (Exception caughtException)
			{
				nodeState = NodeState.Unknown;
				ExceptionHelp.LogException(caughtException, "An exception occurred while getting node {0} state. Defaulting to NodeState::Unknown.", m_name);
			}
			return nodeState;
		}
	}

	public NodeState State
	{
		get
		{
			ThreadWatchdog.PerformUIThreadCheck();
			NodeState nodeState = m_state;
			if (ClusterItem.CachingDisabled || IsDeleted)
			{
				nodeState = NodeState.Unknown;
			}
			if (nodeState == NodeState.Unknown)
			{
				while (!IsDeleted)
				{
					LoadState();
					nodeState = m_state;
					if (nodeState != NodeState.Unknown)
					{
						break;
					}
				}
			}
			return nodeState;
		}
	}

	public WindowsServiceState ServiceState => GetServiceState();

	public ClusterNodeStatus NodeStatus
	{
		get
		{
			if (!ClusterItem.CachingDisabled)
			{
				_ = IsDeleted;
			}
			LoadStatus();
			return m_Status;
		}
	}

	public string FqdnName
	{
		get
		{
			//Discarded unreachable code: IL_0047
			Monitor.Enter(m_nodeFqdnLock);
			try
			{
				if (ClusterItem.CachingDisabled)
				{
					m_fqdnName = null;
				}
				if (string.IsNullOrEmpty(m_fqdnName))
				{
					m_fqdnName = GetFqdnName();
				}
				return m_fqdnName;
			}
			finally
			{
				Monitor.Exit(m_nodeFqdnLock);
			}
		}
	}

	public int MaximumNumberOfNodes => GetMaximumNumberOfNodes(m_name);

	public bool IsDeleting
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_deleting;
		}
	}

	public override bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_lifetimeHelper.IsDeleted;
		}
	}

	public override Guid Id => m_Id;

	public string NodeId
	{
		get
		{
			if (null == m_nodeId)
			{
				m_nodeId = GetNodeId();
			}
			return m_nodeId;
		}
	}

	public override string Name
	{
		get
		{
			if (IsDeleted)
			{
				return m_lastLoadedName;
			}
			return GetName();
		}
	}

	internal unsafe _HNODE* Handle
	{
		get
		{
			m_lifetimeHelper.CheckObjectState();
			return m_hNode.DangerousGetNodeHandle();
		}
	}

	[SpecialName]
	public event EventHandler Deleted
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EDeleted = (EventHandler)Delegate.Combine(_003Cbacking_store_003EDeleted, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EDeleted = (EventHandler)Delegate.Remove(_003Cbacking_store_003EDeleted, value);
		}
	}

	[SpecialName]
	public event EventHandler<DeletingEventArgs> Deleting
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EDeleting = (EventHandler<DeletingEventArgs>)Delegate.Combine(_003Cbacking_store_003EDeleting, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EDeleting = (EventHandler<DeletingEventArgs>)Delegate.Remove(_003Cbacking_store_003EDeleting, value);
		}
	}

	[SpecialName]
	public event EventHandler PropertiesChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EPropertiesChanged = (EventHandler)Delegate.Combine(_003Cbacking_store_003EPropertiesChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EPropertiesChanged = (EventHandler)Delegate.Remove(_003Cbacking_store_003EPropertiesChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler StateChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EStateChanged = (EventHandler)Delegate.Combine(_003Cbacking_store_003EStateChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EStateChanged = (EventHandler)Delegate.Remove(_003Cbacking_store_003EStateChanged, value);
		}
	}

	private ClusterNode(Cluster cluster, SafeNodeHandle hNode, Guid id, string nodeName)
	{
		try
		{
			m_lastLoadedName = nodeName;
			m_name = nodeName;
			m_cluster = cluster;
			m_stateLock = new object();
			m_drainStatusLock = new object();
			m_StatusLock = new object();
			m_state = NodeState.Unknown;
			ResetDrainStatus();
			m_lifetimeHelper = new ObjectLifetimeHelper();
			m_hNode = hNode;
			m_Id = id;
			m_nodeId = null;
			m_groups = null;
			m_groupsLock = new object();
			m_nodeServiceController = null;
			m_nodeFqdnLock = new object();
			m_closed = false;
			if (!ClusterItem.CachingDisabled)
			{
				m_cluster.GroupsChanged += OnGroupsChanged;
				m_cluster.GroupStateChanged += OnGroupsStateChanged;
			}
			m_cluster.ObjectMgr.RegisterInstance(this);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(A_0: true);
			throw;
		}
	}

	private static SafeNodeHandle OpenNode(Cluster cluster, string name)
	{
		return NativeMethods.OpenClusterNode(cluster, name);
	}

	private static Guid GetId(Cluster cluster, SafeNodeHandle hNode)
	{
		ControlExecutor controlExecutor = new NodeControlExecutor(hNode, cluster);
		PropertyCollection propertyCollection = new PropertyCollection(cluster, controlExecutor, ClusterPropertyScope.Common, PropertyCollectionSet.ReadOnly);
		string name = "NodeInstanceID";
		string g = (string)propertyCollection.GetProperty(name).Value;
		return new Guid(g);
	}

	private unsafe string GetName()
	{
		if (m_name == null)
		{
			ThreadWatchdog.PerformUIThreadCheck();
			UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
			unmanagedBuffer.Allocate(1024uL);
			try
			{
				NodeControlExecutor nodeControlExecutor = new NodeControlExecutor(this, m_cluster);
				nodeControlExecutor.ExecuteOutControl(nodeControlExecutor.GetNameCode, unmanagedBuffer);
				m_name = InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
			}
			finally
			{
				unmanagedBuffer.Free();
			}
		}
		if (m_name != null)
		{
			m_lastLoadedName = m_name;
		}
		return m_name;
	}

	private void ResetState()
	{
		m_state = NodeState.Unknown;
	}

	private unsafe void LoadState()
	{
		//Discarded unreachable code: IL_00b3, IL_00d6, IL_00d8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (m_state != NodeState.Unknown && !ClusterItem.CachingDisabled)
		{
			return;
		}
		Monitor.Enter(m_stateLock);
		try
		{
			if (m_hNode != null && (m_state == NodeState.Unknown || ClusterItem.CachingDisabled))
			{
				NodeState clusterNodeState = (NodeState)global::_003CModule_003E.GetClusterNodeState(m_hNode.DangerousGetNodeHandle());
				if (clusterNodeState == NodeState.Unknown)
				{
					Cluster cluster = m_cluster;
					ClusApiExceptionFactory.CreateAndThrow(cluster, (int)global::_003CModule_003E.GetLastError());
				}
				m_state = clusterNodeState;
			}
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019854)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Node_LoadState_Fail_Text,
				m_name
			});
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_LoadState_Fail_Text,
				m_name
			});
		}
		finally
		{
			Monitor.Exit(m_stateLock);
		}
	}

	private long GetItemCount(NodeEnumType enumType, SafeNodeEnumHandleOptions options)
	{
		m_lifetimeHelper.CheckObjectState();
		using SafeNodeEnumHandle safeNodeEnumHandle = NativeMethods.ClusterNodeOpenEnum(this, enumType, options);
		return safeNodeEnumHandle.GetCount();
	}

	private long GetItemCount(NodeEnumType enumType)
	{
		m_lifetimeHelper.CheckObjectState();
		using SafeNodeEnumHandle safeNodeEnumHandle = NativeMethods.ClusterNodeOpenEnum(this, enumType, SafeNodeEnumHandleOptions.None);
		return safeNodeEnumHandle.GetCount();
	}

	private unsafe void ValidatePath(string path, ControlExecutor controlExecutor, uint validateCode)
	{
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		int num = (int)(((long)(path.Length + 1) + 1L) * 2);
		try
		{
			unmanagedBuffer.Allocate((ulong)num);
			byte* pointer = (byte*)unmanagedBuffer.Pointer;
			_CLUSTER_VALIDATE_PATH* ptr = (_CLUSTER_VALIDATE_PATH*)pointer;
			WriteStringToBuffer(path, (ushort*)pointer);
			controlExecutor.ExecuteInControl(this, validateCode, unmanagedBuffer);
		}
		finally
		{
			((IDisposable)unmanagedBuffer)?.Dispose();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool WaitForClusterService(string machineName, int timeout, WindowsServiceState desiredState)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		using (WindowsService windowsService = GetClusterService(machineName))
		{
			WindowsServiceState windowsServiceState = (WindowsServiceState)0;
			int num = 500;
			for (int i = 0; i <= timeout; i += num)
			{
				windowsServiceState = windowsService.GetState();
				if (windowsServiceState != desiredState)
				{
					Thread.Sleep(num);
					continue;
				}
				return true;
			}
		}
		return false;
	}

	private unsafe void WriteStringToBuffer(string inputString, ushort* pBuffer)
	{
		//IL_0021: Expected I, but got I8
		int num = 0;
		int num2 = 0;
		if (0 < inputString.Length)
		{
			ushort* ptr = pBuffer;
			do
			{
				*ptr = inputString[num2];
				num++;
				ptr = (ushort*)((ulong)(nint)ptr + 2uL);
				num2++;
			}
			while (num2 < inputString.Length);
		}
		*(short*)((long)num * 2L + (nint)pBuffer) = 0;
	}

	private void OnGroupsChanged(object sender, ClusterObjectEventArgs e)
	{
		Monitor.Enter(m_groupsLock);
		try
		{
			m_groups = null;
		}
		finally
		{
			Monitor.Exit(m_groupsLock);
		}
	}

	private void OnGroupsStateChanged(object sender, ClusterGroupEventArgs e)
	{
		Monitor.Enter(m_groupsLock);
		try
		{
			m_groups = null;
		}
		finally
		{
			Monitor.Exit(m_groupsLock);
		}
	}

	private void OnDeleting(DeletingStage stage)
	{
		Exception ex = null;
		if (stage == DeletingStage.Start)
		{
			m_deleting = true;
		}
		else
		{
			m_deleting = false;
			if (stage == DeletingStage.Error || stage == DeletingStage.Canceled)
			{
				Refresh();
			}
		}
		try
		{
			raise_Deleting(this, new DeletingEventArgs(stage));
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event Deleting");
		}
	}

	private static WindowsService GetClusterService(string machineName)
	{
		//Discarded unreachable code: IL_0035
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		WindowsService windowsService = null;
		try
		{
			return new WindowsService(ServiceName, machineName, WindowsServicePermissions.Query | WindowsServicePermissions.ChangeState, NodeState.Unknown);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterService_OpenFailed_Text,
				machineName
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe static bool IsDiskIdValid(_CLUSDSK_DISKID* clusDiskId)
	{
		//IL_002e: Expected I, but got I8
		int num = *(int*)clusDiskId;
		switch (num)
		{
		case 5000:
			return false;
		case 0:
			if (*(int*)((ulong)(nint)clusDiskId + 4uL) == 0)
			{
				return false;
			}
			break;
		}
		if (num == 1)
		{
			byte[] array = new byte[16];
			Marshal.Copy((IntPtr)(void*)((ulong)(nint)clusDiskId + 4uL), array, 0, 16);
			ValueType valueType = default(Guid);
			(Guid)valueType = new Guid(array);
			return (byte)((valueType != (object)Guid.Empty) ? 1u : 0u) != 0;
		}
		return true;
	}

	private AsyncEnumeration<ClusterGroup> BuildGroupAsyncEnum()
	{
		SafeNodeEnumHandle enumHandle = NativeMethods.ClusterNodeOpenEnum(this, NodeEnumType.Groups, SafeNodeEnumHandleOptions.NoCoreGroups);
		return new AsyncEnumeration<ClusterGroup>(m_cluster.GetGroup, enumHandle);
	}

	private void OnGroupAsyncEnumCompleted(object sender, EventArgs e)
	{
		AsyncEnumeration<ClusterGroup> asyncEnumeration = (AsyncEnumeration<ClusterGroup>)sender;
		if (asyncEnumeration.EnumeratedItems == null)
		{
			return;
		}
		ClusterGroupCollection clusterGroupCollection = new ClusterGroupCollection();
		foreach (ClusterGroup enumeratedItem in asyncEnumeration.EnumeratedItems)
		{
			clusterGroupCollection.InternalAdd(enumeratedItem);
		}
		m_groups = clusterGroupCollection;
	}

	private void ResetDrainStatus()
	{
		Monitor.Enter(m_drainStatusLock);
		try
		{
			m_drainStatus = ClusterNodeDrainStatus.Unknown;
		}
		finally
		{
			Monitor.Exit(m_drainStatusLock);
		}
	}

	private void LoadDrainStatus()
	{
		//Discarded unreachable code: IL_0096, IL_0098
		Property property = null;
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_drainStatusLock);
		try
		{
			if (m_hNode != null && (m_drainStatus == ClusterNodeDrainStatus.Unknown || ClusterItem.CachingDisabled))
			{
				property = null;
				if (GetCommonProperties(PropertyCollectionSet.ReadOnly).TryGetProperty("NodeDrainStatus", out property))
				{
					m_drainStatus = (ClusterNodeDrainStatus)(uint)property.Value;
				}
			}
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019854)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Node_LoadNodeEvacuationStatus_Fail_Text,
				m_name
			});
		}
		finally
		{
			Monitor.Exit(m_drainStatusLock);
		}
	}

	private void LoadStatus()
	{
		//Discarded unreachable code: IL_00a3, IL_00a5
		Property property = null;
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_StatusLock);
		try
		{
			if (m_hNode != null && (m_Status == ClusterNodeStatus.Unknown || ClusterItem.CachingDisabled))
			{
				property = null;
				if (GetCommonProperties(PropertyCollectionSet.ReadOnly).TryGetProperty("StatusInformation", out property))
				{
					m_Status = (ClusterNodeStatus)(uint)property.Value;
				}
				else
				{
					m_Status = ClusterNodeStatus.None;
				}
			}
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019854)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Node_LoadNodeStatus_Fail_Text,
				m_name
			});
		}
		finally
		{
			Monitor.Exit(m_StatusLock);
		}
	}

	private int GetDynamicWeight()
	{
		Property property = null;
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
		property = null;
		uint result = 0u;
		if (commonProperties.TryGetProperty("DynamicWeight", out property))
		{
			result = (uint)property.Value;
		}
		return (int)result;
	}

	private int GetNodeWeight()
	{
		Property property = null;
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
		property = null;
		uint result = 0u;
		if (commonProperties.TryGetProperty("NodeWeight", out property))
		{
			result = (uint)property.Value;
		}
		return (int)result;
	}

	private unsafe WindowsServiceState GetServiceState()
	{
		//IL_0009: Expected I8, but got I
		//IL_006b: Expected I, but got I8
		//IL_006b: Expected I, but got I8
		//IL_007c: Expected I, but got I8
		//IL_00bc: Expected I, but got I8
		long num = (nint)stackalloc byte[global::_003CModule_003E.__CxxQueryExceptionSize()];
		if (NetworkHelper.CanPing(m_name))
		{
			try
			{
				ServiceController nodeServiceController = m_nodeServiceController;
				if (nodeServiceController == null)
				{
					m_nodeServiceController = new ServiceController(ServiceName, m_name);
				}
				else
				{
					nodeServiceController.Refresh();
				}
				return (WindowsServiceState)m_nodeServiceController.Status;
			}
			catch when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				uint exceptionCode = (uint)Marshal.GetExceptionCode();
				return (byte)global::_003CModule_003E.__CxxExceptionFilter((void*)Marshal.GetExceptionPointers(), null, 0, null) != 0;
			}).Invoke())
			{
				uint num2 = 0u;
				global::_003CModule_003E.__CxxRegisterExceptionObject((void*)Marshal.GetExceptionPointers(), (void*)num);
				try
				{
					try
					{
						ServiceController nodeServiceController2 = m_nodeServiceController;
						if (nodeServiceController2 != null)
						{
							ServiceController serviceController = nodeServiceController2;
							IDisposable disposable = serviceController;
							((IDisposable)serviceController).Dispose();
							m_nodeServiceController = null;
						}
					}
					catch when (((Func<bool>)delegate
					{
						// Could not convert BlockContainer to single expression
						num2 = (uint)global::_003CModule_003E.__CxxDetectRethrow((void*)Marshal.GetExceptionPointers());
						return (byte)num2 != 0;
					}).Invoke())
					{
						goto IL_00ad;
					}
					goto end_IL_007d;
					IL_00ad:
					if (num2 != 0)
					{
						throw;
					}
					end_IL_007d:;
				}
				finally
				{
					global::_003CModule_003E.__CxxUnregisterExceptionObject((void*)num, (int)num2);
				}
			}
		}
		return WindowsServiceState.Stopped;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool NeedsPreventQuorum()
	{
		Property property = null;
		if (GetCommonProperties(PropertyCollectionSet.ReadOnly).TryGetProperty("NeedsPreventQuorum", out property) && (uint)property.Value != 0)
		{
			return true;
		}
		return false;
	}

	internal static ClusterNode CreateObject(Cluster cluster, SafeNodeHandle hNode, string nodeName, Guid nodeId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Guid id = GetId(cluster, hNode);
		Monitor.Enter(m_creationLockObject);
		ClusterNode clusterNode = null;
		try
		{
			clusterNode = cluster.ObjectMgr.GetNodeInstance(id);
			if (clusterNode == null)
			{
				clusterNode = new ClusterNode(cluster, hNode, id, nodeName);
			}
			else
			{
				((IDisposable)hNode)?.Dispose();
			}
		}
		finally
		{
			Monitor.Exit(m_creationLockObject);
		}
		return clusterNode;
	}

	internal static ClusterNode CreateObject(Cluster cluster, string nodeName, Guid nodeId)
	{
		string text = null;
		string text2 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		text = null;
		text2 = null;
		if (TryParseFQDN(nodeName, ref text, ref text2))
		{
			if (string.Compare(cluster.Domain, text2, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>(new string[3]
				{
					Resources.InvalidNodeDomainFormat_Text,
					text2,
					cluster.Domain
				});
			}
		}
		else
		{
			text = nodeName;
		}
		SafeNodeHandle hNode = NativeMethods.OpenClusterNode(cluster, text);
		return CreateObject(cluster, hNode, text, nodeId);
	}

	internal static ClusterNode CreateObject(Cluster cluster, SafeNodeHandle hNode, string nodeName)
	{
		return CreateObject(cluster, hNode, nodeName, Guid.Empty);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool TryParseFQDN(string nodeName, ref string shortName, ref string domainName)
	{
		Match match = new Regex(new string((char*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1EI_0040PEPOGKKC_0040_003F_0024AA_003F_0024FO_003F_0024AA_003F_0024CI_003F_0024AA_003F_0024DP_003F_0024AA_003F_0024DM_003F_0024AAn_003F_0024AAo_003F_0024AAd_003F_0024AAe_003F_0024AA_003F_0024DO_003F_0024AA_003F_0024FL_003F_0024AA_003F_0024FO_003F_0024AA_003F2_003F_0024AA_003F4_003F_0024AA_003F_0024FN_003F_0024AA_003F_0024CL_0040))).Match(nodeName);
		if (match == Match.Empty)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.InvalidNodeNameFormat_Text,
				nodeName
			});
		}
		shortName = match.Groups["node"].Value;
		return !string.IsNullOrEmpty(domainName = match.Groups["domain"].Value);
	}

	internal void OnStateChanged()
	{
		Exception ex = null;
		Exception ex2 = null;
		bool flag = true;
		m_lifetimeHelper.AquireDisposeLock();
		try
		{
			m_lifetimeHelper.CheckObjectState();
			m_state = NodeState.Unknown;
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
		}
		finally
		{
			m_lifetimeHelper.ReleaseDisposeLock();
		}
		if (flag)
		{
			try
			{
				raise_StateChanged(this, EventArgs.Empty);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event StateChanged");
			}
		}
	}

	internal void OnPropertiesChanged()
	{
		Exception ex = null;
		Exception ex2 = null;
		bool flag;
		try
		{
			m_lifetimeHelper.CheckObjectState();
			ResetDrainStatus();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_0028;
		}
		goto IL_002c;
		IL_0028:
		if (!flag)
		{
			return;
		}
		goto IL_002c;
		IL_002c:
		try
		{
			raise_PropertiesChanged(this, EventArgs.Empty);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event PropertiesChanged");
		}
	}

	internal void OnDeleted()
	{
		Exception ex = null;
		Exception ex2 = null;
		bool flag;
		try
		{
			m_cluster.ObjectMgr.UnregisterInstance(this);
			m_lifetimeHelper.CheckObjectState();
			m_lifetimeHelper.MarkAsDeleted();
			Close();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_0044;
		}
		goto IL_0048;
		IL_0044:
		if (!flag)
		{
			return;
		}
		goto IL_0048;
		IL_0048:
		try
		{
			raise_Deleted(this, EventArgs.Empty);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event Deleted - {0}");
		}
	}

	internal override void Refresh()
	{
		m_state = NodeState.Unknown;
		ResetDrainStatus();
	}

	public override void Close()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Monitor.Enter(m_stateLock);
		try
		{
			if (!m_closed)
			{
				m_closed = true;
				SafeNodeHandle hNode = m_hNode;
				IDisposable disposable = hNode;
				((IDisposable)hNode)?.Dispose();
				m_hNode = null;
				ClusterGroupCollection groups = m_groups;
				if (groups is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
				m_groups = null;
			}
		}
		finally
		{
			Monitor.Exit(m_stateLock);
		}
	}

	[SpecialName]
	protected void raise_StateChanged(object value0, EventArgs value1)
	{
		_003Cbacking_store_003EStateChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_PropertiesChanged(object value0, EventArgs value1)
	{
		_003Cbacking_store_003EPropertiesChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_Deleting(object value0, DeletingEventArgs value1)
	{
		_003Cbacking_store_003EDeleting?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_Deleted(object value0, EventArgs value1)
	{
		_003Cbacking_store_003EDeleted?.Invoke(value0, value1);
	}

	public unsafe static NodeClusterState GetClusterState(string nodeName)
	{
		//IL_0009: Expected I, but got I8
		NodeClusterState nodeClusterState = NodeClusterState.None;
		ushort* ptr = null;
		if (nodeName != null)
		{
			ThreadWatchdog.PerformUIThreadCheck();
			ptr = InteropHelp.StringToWstr(nodeName);
		}
		try
		{
			uint result = 0u;
			uint nodeClusterState2 = global::_003CModule_003E.GetNodeClusterState(ptr, &result);
			if (nodeClusterState2 != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)nodeClusterState2, new string[2]
				{
					Resources.GetNodeClusterStateFail_Text,
					nodeName
				});
			}
			return (NodeClusterState)result;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public unsafe static Guid GetNodeClusterId(string nodeName)
	{
		SafeClusterHandle safeClusterHandle = null;
		try
		{
			safeClusterHandle = NativeMethods.OpenCluster(nodeName);
		}
		catch (ClusterBaseException)
		{
			return Guid.Empty;
		}
		try
		{
			SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(global::_003CModule_003E.GetClusterKey((_HCLUSTER*)safeClusterHandle.DangerousGetHandle().ToPointer(), ClusterRegistryKey.RegistryRightsToRegSam(RegistryRights.ExecuteKey)));
			if (!safeRegistryHandle.IsInvalid)
			{
				try
				{
					uint num = 74u;
					byte* ptr = (byte*)global::_003CModule_003E.LocalAlloc(64u, 148uL);
					if (ptr != null)
					{
						try
						{
							System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
							if (global::_003CModule_003E.ClusterRegQueryValue(safeRegistryHandle.DangerousGetRegistryHandle(), (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CE_0040OHKFAEAN_0040_003F_0024AAC_003F_0024AAl_003F_0024AAu_003F_0024AAs_003F_0024AAt_003F_0024AAe_003F_0024AAr_003F_0024AAI_003F_0024AAn_003F_0024AAs_003F_0024AAt_003F_0024AAa_003F_0024AAn_003F_0024AAc_003F_0024AAe_0040), &num2, ptr, &num) == 0 && num2 == 1)
							{
								return new Guid(InteropHelp.WstrToString((ushort*)ptr));
							}
						}
						finally
						{
							global::_003CModule_003E.LocalFree(ptr);
						}
					}
				}
				finally
				{
					SafeRegistryHandle safeRegistryHandle2 = safeRegistryHandle;
					IDisposable disposable = safeRegistryHandle;
					((IDisposable)safeRegistryHandle)?.Dispose();
				}
			}
		}
		finally
		{
			if (!safeClusterHandle.IsInvalid)
			{
				((IDisposable)safeClusterHandle)?.Dispose();
			}
		}
		return Guid.Empty;
	}

	public string GetFqdnName()
	{
		return WmiHelper.GetNodeFullyQualifiedDomainName(Name);
	}

	public ManagementObject GetClusteredStorageSubsystem()
	{
		if (m_clusterSubSystem == null)
		{
			ManagementScope storageWmiConnection = WmiHelper.GetStorageWmiConnection(Name);
			ObjectQuery query = new ObjectQuery(string.Format("SELECT * FROM {0}", "MSFT_StorageSubSystem"));
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(storageWmiConnection, query);
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			try
			{
				string storageSubSystemId = m_cluster.StorageSubSystemId;
				ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						if (managementObject["SerialNumber"] != null)
						{
							string b = (string)managementObject["SerialNumber"];
							if (string.Equals(storageSubSystemId, b))
							{
								m_clusterSubSystem = managementObject;
							}
						}
						if (m_clusterSubSystem == null)
						{
							ManagementObject managementObject2 = managementObject;
							IDisposable disposable = managementObject;
							((IDisposable)managementObject)?.Dispose();
							continue;
						}
						break;
					}
				}
				finally
				{
					ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = enumerator;
					IDisposable disposable2 = enumerator;
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)managementObjectSearcher)?.Dispose();
				((IDisposable)managementObjectCollection)?.Dispose();
			}
		}
		return m_clusterSubSystem;
	}

	public ManagementObjectCollection GetClusteredStorageObject(string className)
	{
		ManagementObject clusteredStorageSubsystem = GetClusteredStorageSubsystem();
		object result = null;
		if (clusteredStorageSubsystem != null)
		{
			result = clusteredStorageSubsystem.GetRelated(className);
		}
		return (ManagementObjectCollection)result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsClusteredVirtualDiskHealthy()
	{
		bool result = true;
		try
		{
			ManagementObjectCollection clusteredStorageObject = GetClusteredStorageObject("MSFT_VirtualDisk");
			if (clusteredStorageObject != null)
			{
				try
				{
					ManagementObjectCollection.ManagementObjectEnumerator enumerator = clusteredStorageObject.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ManagementObject managementObject = (ManagementObject)enumerator.Current;
							try
							{
								if (managementObject["HealthStatus"] == null)
								{
									throw ExceptionHelp.Build<ManagementException>(new string[3]
									{
										Resources.Wmi_PropertyQuery_Failed_Text,
										"HealthStatus",
										"MSFT_VirtualDisk"
									});
								}
								if ((ushort)managementObject["HealthStatus"] != 0)
								{
									result = false;
									break;
								}
							}
							finally
							{
								ManagementObject managementObject2 = managementObject;
								IDisposable disposable = managementObject;
								((IDisposable)managementObject)?.Dispose();
							}
						}
					}
					finally
					{
						ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = enumerator;
						IDisposable disposable2 = enumerator;
						((IDisposable)enumerator)?.Dispose();
					}
				}
				finally
				{
					ManagementObjectCollection managementObjectCollection = clusteredStorageObject;
					IDisposable disposable3 = clusteredStorageObject;
					((IDisposable)clusteredStorageObject)?.Dispose();
				}
			}
		}
		finally
		{
			ManagementObject clusterSubSystem = m_clusterSubSystem;
			if (clusterSubSystem != null)
			{
				((IDisposable)clusterSubSystem).Dispose();
				m_clusterSubSystem = null;
			}
		}
		return result;
	}

	public string GetClusteredStorageNodeUniqueId()
	{
		string relatedClass = "MSFT_StorageNode";
		ManagementObject clusteredStorageSubsystem = GetClusteredStorageSubsystem();
		ManagementObjectCollection managementObjectCollection = null;
		if (clusteredStorageSubsystem != null)
		{
			managementObjectCollection = clusteredStorageSubsystem.GetRelated(relatedClass);
		}
		ManagementObjectCollection managementObjectCollection2 = managementObjectCollection;
		string result = null;
		if (managementObjectCollection != null)
		{
			try
			{
				ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						try
						{
							if (managementObject["Name"] == null)
							{
								throw ExceptionHelp.Build<ManagementException>(new string[3]
								{
									Resources.Wmi_PropertyQuery_Failed_Text,
									"Name",
									"MSFT_StorageNode"
								});
							}
							if (string.Compare(FqdnName, (string)managementObject["Name"], StringComparison.OrdinalIgnoreCase) == 0)
							{
								if (managementObject["UniqueId"] == null)
								{
									throw ExceptionHelp.Build<ManagementException>(new string[3]
									{
										Resources.Wmi_PropertyQuery_Failed_Text,
										"UniqueId",
										"MSFT_StorageNode"
									});
								}
								result = (string)managementObject["UniqueId"];
								break;
							}
						}
						finally
						{
							ManagementObject managementObject2 = managementObject;
							IDisposable disposable = managementObject;
							((IDisposable)managementObject)?.Dispose();
						}
					}
				}
				finally
				{
					ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = enumerator;
					IDisposable disposable2 = enumerator;
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)managementObjectCollection2)?.Dispose();
			}
		}
		return result;
	}

	public Hashtable GetDisksIdInClusteredStoragePools()
	{
		string relatedClass = "MSFT_StoragePool";
		ManagementObject clusteredStorageSubsystem = GetClusteredStorageSubsystem();
		ManagementObjectCollection managementObjectCollection = null;
		if (clusteredStorageSubsystem != null)
		{
			managementObjectCollection = clusteredStorageSubsystem.GetRelated(relatedClass);
		}
		ManagementObjectCollection managementObjectCollection2 = managementObjectCollection;
		Hashtable hashtable = null;
		if (managementObjectCollection != null)
		{
			try
			{
				ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						try
						{
							if (managementObject["IsPrimordial"] == null)
							{
								throw ExceptionHelp.Build<ManagementException>(new string[3]
								{
									Resources.Wmi_PropertyQuery_Failed_Text,
									"IsPrimordial",
									"MSFT_StoragePool"
								});
							}
							if ((bool)managementObject["IsPrimordial"])
							{
								continue;
							}
							ManagementObjectCollection.ManagementObjectEnumerator enumerator2 = managementObject.GetRelated("MSFT_PhysicalDisk").GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									ManagementObject managementObject2 = (ManagementObject)enumerator2.Current;
									try
									{
										if (managementObject2["UniqueId"] == null)
										{
											throw ExceptionHelp.Build<ManagementException>(new string[3]
											{
												Resources.Wmi_PropertyQuery_Failed_Text,
												"UniqueId",
												"MSFT_PhysicalDisk"
											});
										}
										if (hashtable == null)
										{
											hashtable = new Hashtable();
										}
										hashtable.Add((string)managementObject2["UniqueId"], (string)managementObject2["UniqueId"]);
									}
									finally
									{
										ManagementObject managementObject3 = managementObject2;
										IDisposable disposable = managementObject2;
										((IDisposable)managementObject2)?.Dispose();
									}
								}
							}
							finally
							{
								ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = enumerator2;
								IDisposable disposable2 = enumerator2;
								((IDisposable)enumerator2)?.Dispose();
							}
						}
						finally
						{
							ManagementObject managementObject4 = managementObject;
							IDisposable disposable3 = managementObject;
							((IDisposable)managementObject)?.Dispose();
						}
					}
				}
				finally
				{
					ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator2 = enumerator;
					IDisposable disposable4 = enumerator;
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)managementObjectCollection2)?.Dispose();
			}
		}
		return hashtable;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool HasClusteredPoolDisks()
	{
		//Discarded unreachable code: IL_025c, IL_025e
		bool flag = false;
		try
		{
			Hashtable disksIdInClusteredStoragePools = GetDisksIdInClusteredStoragePools();
			if (disksIdInClusteredStoragePools == null)
			{
				return false;
			}
			string clusteredStorageNodeUniqueId = GetClusteredStorageNodeUniqueId();
			if (clusteredStorageNodeUniqueId == null)
			{
				return false;
			}
			ManagementScope storageWmiConnection = WmiHelper.GetStorageWmiConnection(Name);
			ObjectQuery query = new ObjectQuery(string.Format("SELECT * FROM {0} WHERE IsPhysicallyConnected = TRUE", "MSFT_StorageNodeToPhysicalDisk"));
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(storageWmiConnection, query);
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			try
			{
				ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						try
						{
							if (managementObject["StorageNode"] == null)
							{
								throw ExceptionHelp.Build<ManagementException>(new string[3]
								{
									Resources.Wmi_PropertyQuery_Failed_Text,
									"StorageNode",
									"MSFT_StorageNodeToPhysicalDisk"
								});
							}
							if (managementObject["PhysicalDisk"] == null)
							{
								throw ExceptionHelp.Build<ManagementException>(new string[3]
								{
									Resources.Wmi_PropertyQuery_Failed_Text,
									"PhysicalDisk",
									"MSFT_StorageNodeToPhysicalDisk"
								});
							}
							ManagementObject managementObject2 = new ManagementObject(storageWmiConnection, new ManagementPath((string)managementObject["StorageNode"]), new ObjectGetOptions());
							ManagementObject managementObject3 = new ManagementObject(storageWmiConnection, new ManagementPath((string)managementObject["PhysicalDisk"]), new ObjectGetOptions());
							try
							{
								if (managementObject2["UniqueId"] == null)
								{
									throw ExceptionHelp.Build<ManagementException>(new string[3]
									{
										Resources.Wmi_PropertyQuery_Failed_Text,
										"UniqueId",
										"MSFT_StorageNode"
									});
								}
								if (managementObject3["UniqueId"] == null)
								{
									throw ExceptionHelp.Build<ManagementException>(new string[3]
									{
										Resources.Wmi_PropertyQuery_Failed_Text,
										"UniqueId",
										"MSFT_PhysicalDisk"
									});
								}
								if (string.Compare(clusteredStorageNodeUniqueId, (string)managementObject2["UniqueId"], StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag = disksIdInClusteredStoragePools.ContainsKey((string)managementObject3["UniqueId"]) || flag;
								}
							}
							finally
							{
								ManagementObject managementObject4 = managementObject2;
								IDisposable disposable = managementObject2;
								((IDisposable)managementObject2)?.Dispose();
								ManagementObject managementObject5 = managementObject3;
								IDisposable disposable2 = managementObject3;
								((IDisposable)managementObject3)?.Dispose();
							}
						}
						finally
						{
							ManagementObject managementObject6 = managementObject;
							IDisposable disposable3 = managementObject;
							((IDisposable)managementObject)?.Dispose();
						}
					}
					return flag;
				}
				finally
				{
					ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = enumerator;
					IDisposable disposable4 = enumerator;
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				ManagementObjectSearcher managementObjectSearcher2 = managementObjectSearcher;
				IDisposable disposable5 = managementObjectSearcher;
				((IDisposable)managementObjectSearcher)?.Dispose();
				ManagementObjectCollection managementObjectCollection2 = managementObjectCollection;
				IDisposable disposable6 = managementObjectCollection;
				((IDisposable)managementObjectCollection)?.Dispose();
			}
		}
		catch (ManagementException ex)
		{
			throw ex;
		}
		finally
		{
			ManagementObject clusterSubSystem = m_clusterSubSystem;
			if (clusterSubSystem != null)
			{
				((IDisposable)clusterSubSystem).Dispose();
				m_clusterSubSystem = null;
			}
		}
	}

	public unsafe void Pause()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		m_state = NodeState.Unknown;
		uint num = global::_003CModule_003E.PauseClusterNode(Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.NodePauseFail_Text, Name);
		}
	}

	public unsafe void PauseEx([MarshalAs(UnmanagedType.U1)] bool drain, string target, ClusterNodePauseExFlags flags)
	{
		//IL_0020: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		m_state = NodeState.Unknown;
		ResetDrainStatus();
		_HNODE* ptr = null;
		if (target != null)
		{
			ptr = m_cluster.GetNode(target).Handle;
		}
		uint num = global::_003CModule_003E.PauseClusterNodeEx(Handle, drain ? 1 : 0, (uint)flags, ptr);
		if (num != 0 && num != 997)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.NodePauseFail_Text, Name);
		}
	}

	public unsafe void Resume()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		m_state = NodeState.Unknown;
		uint num = global::_003CModule_003E.ResumeClusterNode(Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.NodeResumeFail_Text, Name);
		}
	}

	public unsafe void ResumeEx(ClusterNodeResumeFailbackType failbackType)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		m_state = NodeState.Unknown;
		ResetDrainStatus();
		uint num = global::_003CModule_003E.ResumeClusterNodeEx(Handle, (CLUSTER_NODE_RESUME_FAILBACK_TYPE)failbackType, 0u);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.NodeResumeFail_Text, Name);
		}
	}

	public unsafe void Evict(TimeSpan timeout)
	{
		//Discarded unreachable code: IL_00f0, IL_00f2
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		int resultCode = 0;
		uint num = (uint)timeout.TotalMilliseconds;
		uint num2 = ((State == NodeState.Up || State == NodeState.Paused) ? num : 4294967294u);
		OnDeleting(DeletingStage.Start);
		try
		{
			uint num3 = global::_003CModule_003E.EvictClusterNodeEx(Handle, num2, &resultCode);
			switch (num3)
			{
			case 258u:
				throw new System.TimeoutException(string.Format(CultureInfo.CurrentCulture, Resources.NodeEvictTimeout_Text, Name));
			case 5896u:
				throw ExceptionHelp.Build<ClusterFailedCleanupEvictNodeException>(args: new string[1] { Name }, resultCode: resultCode);
			default:
			{
				Cluster cluster = m_cluster;
				ClusApiExceptionFactory.CreateAndThrow(cluster, (int)num3, Resources.NodeEvictFail_Text, Name);
				break;
			}
			case 0u:
				break;
			}
			Cluster cluster2 = m_cluster;
			cluster2.RefreshNodes();
		}
		catch (Exception ex)
		{
			if (ex.GetType() == typeof(ClusterFailedCleanupEvictNodeException))
			{
				Cluster.RefreshNodes();
				OnDeleting(DeletingStage.Complete);
			}
			else
			{
				OnDeleting(DeletingStage.Error);
			}
			throw;
		}
		finally
		{
			OnDeleting(DeletingStage.Complete);
		}
	}

	public void StartService()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		StartClusterService(serviceArgs: (!NeedsPreventQuorum()) ? new string[0] : new string[1] { "-pq" }, machineName: (State != NodeState.Down) ? FqdnName : Name);
	}

	public void StopService()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		StopClusterService((State != NodeState.Down) ? FqdnName : Name);
	}

	private void _007EClusterNode()
	{
		ServiceController nodeServiceController = m_nodeServiceController;
		if (nodeServiceController != null)
		{
			((IDisposable)nodeServiceController).Dispose();
			m_nodeServiceController = null;
		}
		ManagementObject clusterSubSystem = m_clusterSubSystem;
		if (clusterSubSystem != null)
		{
			((IDisposable)clusterSubSystem).Dispose();
			m_clusterSubSystem = null;
		}
	}

	public static void StartClusterService(string machineName, params string[] serviceArgs)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		using WindowsService windowsService = GetClusterService(machineName);
		if (windowsService.GetStartMode() == WindowsServiceStart.Disabled)
		{
			throw ExceptionHelp.Build(Resources.StartService_ServiceDisabled_Text, machineName);
		}
		windowsService.Start(serviceArgs);
	}

	public static void StartClusterService(string machineName)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		StartClusterService(machineName, new string[0]);
	}

	public static void StopClusterService(string machineName)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		using WindowsService windowsService = GetClusterService(machineName);
		windowsService.Stop();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsClusterServiceRunning(string machineName)
	{
		//Discarded unreachable code: IL_0027
		ThreadWatchdog.PerformUIThreadCheck();
		using WindowsService windowsService = GetClusterService(machineName);
		if (windowsService.GetState() == WindowsServiceState.Running)
		{
			return true;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool WaitForStartingClusterService(string machineName, int timeout)
	{
		return WaitForClusterService(machineName, timeout, WindowsServiceState.Running);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool WaitForStoppingClusterService(string machineName, int timeout)
	{
		return WaitForClusterService(machineName, timeout, WindowsServiceState.Stopped);
	}

	public unsafe static int GetMaximumNumberOfNodes(string machineName)
	{
		//Discarded unreachable code: IL_0044
		//IL_0008: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		try
		{
			uint result = 0u;
			uint num = 0u;
			ptr = InteropHelp.StringToWstr(machineName);
			uint maxNodeCount = global::_003CModule_003E.GetMaxNodeCount(ptr, &result);
			if (maxNodeCount != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)maxNodeCount, new string[1] { Resources.Cluster_GetMaxNumberOfNodes_Failed_Text });
			}
			return (int)result;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public unsafe string GetNodeId()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (m_nodeId == null)
		{
			UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
			unmanagedBuffer.Allocate(1024uL);
			try
			{
				NodeControlExecutor nodeControlExecutor = new NodeControlExecutor(this, m_cluster);
				nodeControlExecutor.ExecuteOutControl(nodeControlExecutor.GetIdCode, unmanagedBuffer);
				m_nodeId = InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
			}
			finally
			{
				unmanagedBuffer.Free();
			}
		}
		return m_nodeId;
	}

	public ClusterGroupCollection GetGroups()
	{
		//Discarded unreachable code: IL_007d, IL_007f
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterGroupCollection clusterGroupCollection = m_groups;
		if (clusterGroupCollection == null || ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_groupsLock);
			try
			{
				clusterGroupCollection = m_groups;
				if (clusterGroupCollection == null || ClusterItem.CachingDisabled)
				{
					clusterGroupCollection = (m_groups = new ClusterGroupCollection(BuildGroupAsyncEnum()));
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetGroups_Failed_Text,
					m_name
				});
			}
			finally
			{
				Monitor.Exit(m_groupsLock);
			}
		}
		return clusterGroupCollection;
	}

	public IEnumerable<Tuple<Guid, string>> GetGroupsGuidName()
	{
		//Discarded unreachable code: IL_0086, IL_00a9, IL_00cc, IL_00ce, IL_00d0, IL_00dc
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeNodeEnumHandle safeNodeEnumHandle = null;
		List<Tuple<Guid, string>> list = new List<Tuple<Guid, string>>();
		try
		{
			safeNodeEnumHandle = NativeMethods.ClusterNodeOpenEnum(this, NodeEnumType.Groups, SafeNodeEnumHandleOptions.None);
			while (safeNodeEnumHandle.MoveNext())
			{
				ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeNodeEnumHandle.Current;
				if (clusterEnumItem != null)
				{
					list.Add(new Tuple<Guid, string>(clusterEnumItem.ID, clusterEnumItem.Name));
				}
			}
			return list;
		}
		catch (ApplicationException innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetGroups_Failed_Text,
				m_name
			});
		}
		catch (Win32Exception innerException2)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException2, new string[2]
			{
				Resources.Cluster_GetGroups_Failed_Text,
				m_name
			});
		}
		catch (ClusterBaseException innerException3)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException3, new string[2]
			{
				Resources.Cluster_GetGroups_Failed_Text,
				m_name
			});
		}
		finally
		{
			safeNodeEnumHandle?.Close();
		}
	}

	public unsafe IEnumerable<Tuple<Guid, string>> GetResourcesGuidName()
	{
		//Discarded unreachable code: IL_01cc, IL_01f3, IL_021a, IL_021c, IL_021e, IL_0233
		string text = null;
		string text2 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeResourceEnumExHandle safeResourceEnumExHandle = null;
		SafeClusterEnumHandle safeClusterEnumHandle = null;
		try
		{
			List<Tuple<Guid, string>> list = new List<Tuple<Guid, string>>();
			int num = ((m_cluster.CurrentVersion > ClusterVersion.Windows7) ? 1 : 0);
			if ((byte)num != 0)
			{
				HashSet<Guid> hashSet = new HashSet<Guid>(new HashSet<Guid>());
				IEnumerator<Tuple<Guid, string>> enumerator = GetGroupsGuidName().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Tuple<Guid, string> current = enumerator.Current;
						Guid item = current.Item1;
						if (!hashSet.Contains(item))
						{
							Guid item2 = current.Item1;
							hashSet.Add(item2);
						}
					}
				}
				finally
				{
					IEnumerator<Tuple<Guid, string>> enumerator2 = enumerator;
					IDisposable disposable = enumerator;
					enumerator?.Dispose();
				}
				safeResourceEnumExHandle = new SafeResourceEnumExHandle(m_cluster);
				while (safeResourceEnumExHandle.MoveNext())
				{
					ClusterResourceExEnumItem clusterResourceExEnumItem = (ClusterResourceExEnumItem)safeResourceEnumExHandle.Current;
					if (clusterResourceExEnumItem != null && hashSet.Contains(clusterResourceExEnumItem.groupGuid))
					{
						list.Add(new Tuple<Guid, string>(clusterResourceExEnumItem.ID, clusterResourceExEnumItem.Name));
					}
				}
			}
			else
			{
				safeClusterEnumHandle = NativeMethods.ClusterOpenEnum(m_cluster, ClusterEnumType.Resource, SafeClusterEnumHandleOptions.None);
				string name = GetName();
				while (safeClusterEnumHandle.MoveNext())
				{
					ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeClusterEnumHandle.Current;
					if (clusterEnumItem == null)
					{
						continue;
					}
					SafeResourceHandle safeResourceHandle = NativeMethods.OpenClusterResource(m_cluster, clusterEnumItem.Name);
					try
					{
						text = null;
						text2 = null;
						ClusterResource.GetResourceState(m_cluster, safeResourceHandle.DangerousGetResourceHandle(), ref text, ref text2);
						if (name == text)
						{
							list.Add(new Tuple<Guid, string>(clusterEnumItem.ID, clusterEnumItem.Name));
						}
					}
					finally
					{
						if (safeResourceHandle != null)
						{
							safeResourceHandle.Close();
							safeResourceHandle = null;
						}
					}
				}
			}
			return list;
		}
		catch (ApplicationException innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				m_name
			});
		}
		catch (Win32Exception innerException2)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException2, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				m_name
			});
		}
		catch (ClusterBaseException innerException3)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException3, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				m_name
			});
		}
		finally
		{
			safeResourceEnumExHandle?.Close();
			safeClusterEnumHandle?.Close();
		}
	}

	public Dictionary<Guid, ClusterGroup> GetCoreGroups()
	{
		Dictionary<Guid, ClusterGroup> dictionary = new Dictionary<Guid, ClusterGroup>();
		Dictionary<Guid, ClusterGroup>.ValueCollection.Enumerator enumerator = m_cluster.GetCoreGroups().Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				ClusterGroup current = enumerator.Current;
				if (string.Compare(current.OwnerNodeName, Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					Guid id = current.Id;
					dictionary.Add(id, current);
				}
			}
			while (enumerator.MoveNext());
		}
		return dictionary;
	}

	public Dictionary<Guid, string> GetCurrentGroupNames()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
		SafeNodeEnumHandle safeNodeEnumHandle = NativeMethods.ClusterNodeOpenEnum(this, NodeEnumType.Groups, SafeNodeEnumHandleOptions.NoCoreGroups);
		try
		{
			while (safeNodeEnumHandle.MoveNext())
			{
				ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeNodeEnumHandle.Current;
				if (clusterEnumItem == null)
				{
					continue;
				}
				if (clusterEnumItem.ID == Guid.Empty)
				{
					SafeGroupHandle safeGroupHandle = NativeMethods.OpenClusterGroup(m_cluster, clusterEnumItem.Name);
					try
					{
						Cluster cluster = m_cluster;
						GroupControlExecutor groupControlExecutor = new GroupControlExecutor(safeGroupHandle, cluster);
						Guid id = groupControlExecutor.GetId(groupControlExecutor);
						clusterEnumItem.ID = id;
					}
					finally
					{
						safeGroupHandle?.Close();
					}
				}
				dictionary.Add(clusterEnumItem.ID, clusterEnumItem.Name);
			}
			return dictionary;
		}
		finally
		{
			safeNodeEnumHandle?.Close();
		}
	}

	public AsyncEnumerationStatus GetGroupsAsync(AsyncEnumerationCallback<ClusterGroup> callback)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterGroupCollection groups = m_groups;
		AsyncEnumeration<ClusterGroup> asyncEnumeration;
		if (groups != null && !ClusterItem.CachingDisabled)
		{
			asyncEnumeration = new AsyncEnumeration<ClusterGroup>(groups);
		}
		else
		{
			asyncEnumeration = BuildGroupAsyncEnum();
			asyncEnumeration.EnumerationComplete += OnGroupAsyncEnumCompleted;
		}
		asyncEnumeration.SetCallback(callback);
		return asyncEnumeration.StartEnumeration(useDifferentThread: true);
	}

	public ClusterResourceCollection GetResources()
	{
		//Discarded unreachable code: IL_00a1, IL_00a3
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ClusterResourceCollection clusterResourceCollection = new ClusterResourceCollection();
			IEnumerator<ClusterGroup> enumerator = GetGroups().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IEnumerator<ClusterResource> enumerator2 = enumerator.Current.GetResources().GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ClusterResource current = enumerator2.Current;
							clusterResourceCollection.InternalAdd(current);
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
				IEnumerator<ClusterGroup> enumerator4 = enumerator;
				IDisposable disposable2 = enumerator;
				enumerator?.Dispose();
			}
			return clusterResourceCollection;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_GetResources_Fail_Text,
				m_name
			});
		}
	}

	public ClusterNetworkInterfaceCollection GetNetworkInterfaces()
	{
		//Discarded unreachable code: IL_0059, IL_005b
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			SafeNodeEnumHandle enumHandle = NativeMethods.ClusterNodeOpenEnum(this, NodeEnumType.NetworkInterface, SafeNodeEnumHandleOptions.None);
			return new ClusterNetworkInterfaceCollection(new AsyncEnumeration<ClusterNetworkInterface>(m_cluster.GetNetworkInterface, enumHandle));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_GetNetworkInterfaces_Fail_Text,
				m_name
			});
		}
	}

	public AsyncEnumerationStatus GetNetworkInterfacesAsync(AsyncEnumerationCallback<ClusterNetworkInterface> callback)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeNodeEnumHandle enumHandle = NativeMethods.ClusterNodeOpenEnum(this, NodeEnumType.NetworkInterface, SafeNodeEnumHandleOptions.None);
		AsyncEnumeration<ClusterNetworkInterface> asyncEnumeration = new AsyncEnumeration<ClusterNetworkInterface>(m_cluster.GetNetworkInterface, enumHandle);
		asyncEnumeration.SetCallback(callback);
		return asyncEnumeration.StartEnumeration(useDifferentThread: true);
	}

	public string GetSystemDrive()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return Utilities.GetSystemDrive(Name);
	}

	public long GetNetworkInterfaceCount()
	{
		//Discarded unreachable code: IL_0030, IL_0032
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return GetItemCount(NodeEnumType.NetworkInterface);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_GetNetworkInterfaceCount_Fail_Text,
				m_name
			});
		}
	}

	public long GetGroupCount()
	{
		//Discarded unreachable code: IL_0031, IL_0033
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return GetItemCount(NodeEnumType.Groups, SafeNodeEnumHandleOptions.None);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetGroupCount_Failed_Text,
				m_name
			});
		}
	}

	public unsafe Collection<ClusterDisk> GetClusterableDisks()
	{
		//IL_0039: Expected I, but got I8
		//IL_0069: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Collection<ClusterDisk> result = new Collection<ClusterDisk>();
		if (!CanProcessClusterServiceCommands)
		{
			return result;
		}
		CClusPropValueList* ptr = (CClusPropValueList*)global::_003CModule_003E.@new(48uL);
		CClusPropValueList* ptr2;
		try
		{
			ptr2 = ((ptr == null) ? null : global::_003CModule_003E.CClusPropValueList_002E_007Bctor_007D(ptr));
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		CClusPropValueList* ptr3 = ptr2;
		try
		{
			uint num = global::_003CModule_003E.CClusPropValueList_002EScGetResourceTypeValueList(ptr2, m_cluster.Handle, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BM_0040PLDPFCNA_0040_003F_0024AAP_003F_0024AAh_003F_0024AAy_003F_0024AAs_003F_0024AAi_003F_0024AAc_003F_0024AAa_003F_0024AAl_003F_0024AA_003F5_003F_0024AAD_003F_0024AAi_003F_0024AAs_003F_0024AAk_0040), 33554933u, Handle, null, 0uL);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.GetClusterableDisksFail_Text, Name);
			}
			return ClusterDisk.ParseClusterableDisks(m_cluster, Name, ptr2);
		}
		finally
		{
			if (ptr3 != null)
			{
				global::_003CModule_003E.CClusPropValueList_002EDeleteValueList(ptr3);
				global::_003CModule_003E.delete(ptr3);
			}
		}
	}

	public unsafe uint GetAvailableDriveLetters()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceTypeCollection storageClassResourceTypes = m_cluster.GetStorageClassResourceTypes();
		uint num = 0u;
		try
		{
			IEnumerator<ClusterResourceType> enumerator = storageClassResourceTypes.GetEnumerator();
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUS_STORAGE_GET_AVAILABLE_DRIVELETTERS cLUS_STORAGE_GET_AVAILABLE_DRIVELETTERS);
				while (enumerator.MoveNext())
				{
					ClusterResourceType current = enumerator.Current;
					try
					{
						ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(current.Name, m_cluster);
						UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&cLUS_STORAGE_GET_AVAILABLE_DRIVELETTERS, 4uL);
						resourceTypeControlExecutor.ExecuteOutControl(this, 33554925u, outputBuffer);
						num = *(uint*)(&cLUS_STORAGE_GET_AVAILABLE_DRIVELETTERS);
						if (*(int*)(&cLUS_STORAGE_GET_AVAILABLE_DRIVELETTERS) != 0)
						{
							break;
						}
					}
					catch (Exception innerException)
					{
						Exception ex = ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
						{
							Resources.Node_StorageType_GetAvailableDriveLetters_Fail_Text,
							m_name,
							current.Name
						});
						string[] args = new string[0];
						string exceptionReportMessage = ExceptionHelp.GetExceptionReportMessage(ex, args);
						ExceptionHelp.LogException(ex, exceptionReportMessage);
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResourceType> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			if (storageClassResourceTypes is IDisposable disposable2)
			{
				disposable2.Dispose();
			}
		}
		if (num == 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.Node_GetAvailableDriveLetters_Fail_Text,
				m_name
			});
		}
		return num;
	}

	public unsafe StorageNodeState GetStorageNodeState(string storageNodeName)
	{
		//Discarded unreachable code: IL_00a5, IL_00a7
		//IL_0042: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = null;
		UnmanagedBuffer unmanagedBuffer2 = null;
		StorageNodeState storageNodeState = StorageNodeState.Unknown;
		try
		{
			unmanagedBuffer2 = new UnmanagedBuffer();
			unmanagedBuffer2.Allocate(1024uL);
			unmanagedBuffer = new UnmanagedBuffer();
			unmanagedBuffer.Allocate(524uL);
			byte* pointer = (byte*)unmanagedBuffer.Pointer;
			_CLUS_GET_STORAGE_NODE_STATE_IN* ptr = (_CLUS_GET_STORAGE_NODE_STATE_IN*)pointer;
			*(int*)pointer = 1;
			WriteStringToBuffer(storageNodeName, (ushort*)((ulong)(nint)pointer + 4uL));
			NodeControlExecutor nodeControlExecutor = new NodeControlExecutor(this, m_cluster);
			try
			{
				nodeControlExecutor.ExecuteInOutControl(nodeControlExecutor.GetStorageNodeStateCode, unmanagedBuffer, unmanagedBuffer2);
			}
			catch (Exception)
			{
				storageNodeState = StorageNodeState.Down;
			}
			if (storageNodeState == StorageNodeState.Unknown)
			{
				pointer = (byte*)unmanagedBuffer2.Pointer;
				_CLUS_GET_STORAGE_NODE_STATE_OUT* ptr2 = (_CLUS_GET_STORAGE_NODE_STATE_OUT*)pointer;
				if (*(int*)pointer == 1)
				{
					storageNodeState = *(StorageNodeState*)((ulong)(nint)pointer + 4uL);
				}
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_LoadNodeStatus_Fail_Text,
				m_name
			});
		}
		finally
		{
			((IDisposable)unmanagedBuffer)?.Dispose();
			((IDisposable)unmanagedBuffer2)?.Dispose();
		}
		return storageNodeState;
	}

	public unsafe ClusterDiskId GetDiskId(string win32Path)
	{
		//Discarded unreachable code: IL_0154, IL_015d
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterDiskId clusterDiskId = null;
		ClusterResourceTypeCollection clusterResourceTypeCollection = m_cluster.GetStorageClassResourceTypes();
		try
		{
			IEnumerator<ClusterResourceType> enumerator = clusterResourceTypeCollection.GetEnumerator();
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUSDSK_DISKID diskId);
				while (enumerator.MoveNext())
				{
					ClusterResourceType current = enumerator.Current;
					UnmanagedBuffer unmanagedBuffer = null;
					try
					{
						DebugLog.LogInfo("Getting the diskId for disk that contains path '{0}' on node '{1}' for resource type '{2}'.", win32Path, Name, current.DisplayName);
						ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(current.Name, m_cluster);
						UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&diskId, 20uL);
						unmanagedBuffer = UnmanagedBuffer.Create(win32Path);
						resourceTypeControlExecutor.ExecuteInOutControl(this, 33554949u, unmanagedBuffer, outputBuffer);
						if (!IsDiskIdValid(&diskId))
						{
							DebugLog.LogInfo("Did not find diskId for disk that contains path '{0}' on node '{1}' for resource type '{2}'.", win32Path, Name, current.DisplayName);
							continue;
						}
						clusterDiskId = ClusterDiskId.CreateDiskId(diskId);
						if (clusterDiskId != null)
						{
							DebugLog.LogInfo("Found the diskId for disk that contains path '{0}' on node '{1}' for resource type '{2}'.", win32Path, Name, current.DisplayName);
							break;
						}
					}
					catch (ApplicationException exception)
					{
						string[] args = new string[0];
						string exceptionReportMessage = ExceptionHelp.GetExceptionReportMessage(exception, args);
						ClusterLog.AdminEvents.WriteDiskCreationErrorEvent(exceptionReportMessage);
					}
					catch (Win32Exception exception2)
					{
						string[] args2 = new string[0];
						string exceptionReportMessage2 = ExceptionHelp.GetExceptionReportMessage(exception2, args2);
						ClusterLog.AdminEvents.WriteDiskCreationErrorEvent(exceptionReportMessage2);
					}
					finally
					{
						unmanagedBuffer?.Free();
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResourceType> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterResourceTypeCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
		if (clusterDiskId == null)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[3]
			{
				Resources.Node_GetDiskId_Fail_Text,
				m_name,
				win32Path
			});
		}
		return clusterDiskId;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool TryGetDiskId(string win32Path, out ClusterDiskId diskId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ClusterDiskId clusterDiskId = null;
			clusterDiskId = GetDiskId(win32Path);
			diskId = clusterDiskId;
			return true;
		}
		catch (Exception exception)
		{
			DebugLog.LogException(exception, string.Format(CultureInfo.CurrentCulture, "There was an error getting the diskId for for volume in path '{0}' on node '{1}'. This error may be expected...", win32Path, Name));
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool IsDiskClusterable(ClusterDiskId diskId)
	{
		//Discarded unreachable code: IL_0166
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceTypeCollection clusterResourceTypeCollection = m_cluster.GetStorageClassResourceTypes();
		try
		{
			IEnumerator<ClusterResourceType> enumerator = clusterResourceTypeCollection.GetEnumerator();
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUSDSK_DISKID cLUSDSK_DISKID);
				while (enumerator.MoveNext())
				{
					ClusterResourceType current = enumerator.Current;
					DebugLog.LogInfo("Checking that diskId '{0}' on node '{1}' for resource type '{2}' can be added to the cluster.", diskId.ToString(), Name, current.DisplayName);
					ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(current.Name, m_cluster);
					diskId.GetClusDiskId(&cLUSDSK_DISKID);
					UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&cLUSDSK_DISKID, 20uL);
					try
					{
						resourceTypeControlExecutor.ExecuteInControl(this, 33554953u, inputBuffer);
						return true;
					}
					catch (Exception ex)
					{
						ExceptionHelp.LogException(ex, "There was an error checking if diskId '{0}' on node '{1}' for resource type '{2}' can be added to the cluster.", diskId.ToString(), Name, current.DisplayName);
						Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
						if (firstException == null)
						{
							continue;
						}
						int num = -2147024862;
						if (firstException.NativeErrorCode != -2147024862)
						{
							int num2 = -2147024891;
							if (firstException.NativeErrorCode != -2147024891)
							{
								ApplicationException exception = ExceptionHelp.Build<ApplicationException>(ex, new string[4]
								{
									Resources.Node_StorageType_IsClusterable_Fail_Text,
									m_name,
									diskId.ToString(),
									current.Name
								});
								string[] args = new string[0];
								string exceptionReportMessage = ExceptionHelp.GetExceptionReportMessage(exception, args);
								ClusterLog.AdminEvents.WriteDiskCreationErrorEvent(exceptionReportMessage);
							}
						}
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResourceType> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterResourceTypeCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool IsFileOnClusterSharedVolume(string path)
	{
		//Discarded unreachable code: IL_00f8, IL_0133, IL_0135
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			string resourceTypeName = "Physical Disk";
			ClusterResourceType resourceType = m_cluster.GetResourceType(resourceTypeName);
			try
			{
				UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
				ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(resourceType.Name, m_cluster);
				int num = 2;
				int num2 = (int)(((long)(path.Length + 1) + 1L) * 2);
				unmanagedBuffer.Allocate((ulong)num2);
				byte* pointer = (byte*)unmanagedBuffer.Pointer;
				_CLUSTER_VALIDATE_CSV_FILENAME* ptr = (_CLUSTER_VALIDATE_CSV_FILENAME*)pointer;
				WriteStringToBuffer(path, (ushort*)pointer);
				bool result;
				try
				{
					resourceTypeControlExecutor.ExecuteInControl(this, 16777769u, unmanagedBuffer);
				}
				catch (Exception caughtException)
				{
					Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
					if (firstException == null)
					{
						goto IL_00f6;
					}
					int num3 = -2147018951;
					if (firstException.NativeErrorCode != -2147018951)
					{
						int num4 = -2147024894;
						if (firstException.NativeErrorCode != -2147024894)
						{
							int num5 = -2147024895;
							if (firstException.NativeErrorCode != -2147024895)
							{
								int num6 = -2147024893;
								if (firstException.NativeErrorCode != -2147024893)
								{
									int num7 = -2147019818;
									if (firstException.NativeErrorCode != -2147019818)
									{
										goto IL_00f6;
									}
								}
							}
						}
					}
					result = false;
					goto IL_00fc;
					IL_00f6:
					throw;
				}
				goto end_IL_0025;
				IL_00fc:
				return result;
				end_IL_0025:;
			}
			finally
			{
				ClusterResourceType clusterResourceType = resourceType;
				IDisposable disposable = resourceType;
				((IDisposable)resourceType)?.Dispose();
			}
			return true;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_IsFileOnSharedVolume_Fail_Text,
				path
			});
		}
	}

	public unsafe void RemoveDiskOwnership(ClusterDiskId diskId)
	{
		//Discarded unreachable code: IL_0096
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			string resourceTypeName = "Physical Disk";
			ClusterResourceType resourceType = m_cluster.GetResourceType(resourceTypeName);
			try
			{
				ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(resourceType.Name, m_cluster);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUSDSK_DISKID cLUSDSK_DISKID);
				diskId.GetClusDiskId(&cLUSDSK_DISKID);
				UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&cLUSDSK_DISKID, 20uL);
				resourceTypeControlExecutor.ExecuteInControl(this, 37749262u, inputBuffer);
			}
			finally
			{
				ClusterResourceType clusterResourceType = resourceType;
				IDisposable disposable = resourceType;
				((IDisposable)resourceType)?.Dispose();
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Node_RemoveOwnership_Fail_Text,
				m_name,
				diskId.ToString()
			});
		}
	}

	public void ValidateGenericApplicationPath(string path)
	{
		//Discarded unreachable code: IL_0065, IL_0067
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string resourceTypeName = "Generic Application";
		using ClusterResourceType clusterResourceType = m_cluster.GetResourceType(resourceTypeName);
		try
		{
			ResourceTypeControlExecutor controlExecutor = new ResourceTypeControlExecutor(clusterResourceType.Name, m_cluster);
			ValidatePath(path, controlExecutor, 33554993u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_ValidateGenericApplicationPath_Fail_Text,
				m_name
			});
		}
	}

	public unsafe void ValidateGenericApplicationCurrentDirectory(string path)
	{
		//Discarded unreachable code: IL_009d, IL_009f
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string resourceTypeName = "Generic Application";
		ClusterResourceType resourceType = m_cluster.GetResourceType(resourceTypeName);
		UnmanagedBuffer unmanagedBuffer = null;
		try
		{
			unmanagedBuffer = new UnmanagedBuffer();
			ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(resourceType.Name, m_cluster);
			int num = 2;
			int num2 = (int)(((long)(path.Length + 1) + 1L) * 2);
			unmanagedBuffer.Allocate((ulong)num2);
			byte* pointer = (byte*)unmanagedBuffer.Pointer;
			_CLUSTER_VALIDATE_DIRECTORY* ptr = (_CLUSTER_VALIDATE_DIRECTORY*)pointer;
			WriteStringToBuffer(path, (ushort*)pointer);
			resourceTypeControlExecutor.ExecuteInControl(this, 33555001u, unmanagedBuffer);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_ValidateGenericApplicationCurrentDirectory_Fail_Text,
				m_name
			});
		}
		finally
		{
			((IDisposable)resourceType)?.Dispose();
			((IDisposable)unmanagedBuffer)?.Dispose();
		}
	}

	public void ValidateGenericScriptPath(string path)
	{
		//Discarded unreachable code: IL_0065, IL_0067
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string resourceTypeName = "Generic Script";
		using ClusterResourceType clusterResourceType = m_cluster.GetResourceType(resourceTypeName);
		try
		{
			ResourceTypeControlExecutor controlExecutor = new ResourceTypeControlExecutor(clusterResourceType.Name, m_cluster);
			ValidatePath(path, controlExecutor, 33554993u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_ValidateGenericScriptPath_Fail_Text,
				m_name
			});
		}
	}

	public unsafe void ValidateNetworkName(string netName)
	{
		//Discarded unreachable code: IL_009d, IL_009f
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string resourceTypeName = "Network Name";
		ClusterResourceType resourceType = m_cluster.GetResourceType(resourceTypeName);
		UnmanagedBuffer unmanagedBuffer = null;
		try
		{
			unmanagedBuffer = new UnmanagedBuffer();
			ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(resourceType.Name, m_cluster);
			int num = 2;
			int num2 = (int)(((long)(netName.Length + 1) + 1L) * 2);
			unmanagedBuffer.Allocate((ulong)num2);
			byte* pointer = (byte*)unmanagedBuffer.Pointer;
			_CLUSTER_VALIDATE_NETNAME* ptr = (_CLUSTER_VALIDATE_NETNAME*)pointer;
			WriteStringToBuffer(netName, (ushort*)pointer);
			resourceTypeControlExecutor.ExecuteInControl(this, 33554997u, unmanagedBuffer);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_ValidateNetworkName_Fail_Text,
				m_name
			});
		}
		finally
		{
			((IDisposable)resourceType)?.Dispose();
			((IDisposable)unmanagedBuffer)?.Dispose();
		}
	}

	public override ControlExecutor GetControlExecutor()
	{
		return new NodeControlExecutor(this, m_cluster);
	}

	public override PropertyCollection GetCommonProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0056, IL_0079, IL_007b
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Common, propSet);
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019854)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Node_GetCommonProperties_Fail_Text,
				m_name
			});
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_GetCommonProperties_Fail_Text,
				m_name
			});
		}
	}

	public override PropertyCollection GetPrivateProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0056, IL_0079, IL_007b
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Private, propSet);
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019854)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Node_GetPrivateProperties_Fail_Text,
				m_name
			});
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Node_GetPrivateProperties_Fail_Text,
				m_name
			});
		}
	}

	public override string ToString()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return Name;
	}

	public unsafe ClusterRegistryKey GetRegistryKey(RegistryRights rights)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		HKEY__* clusterNodeKey = global::_003CModule_003E.GetClusterNodeKey(Handle, ClusterRegistryKey.RegistryRightsToRegSam(rights));
		uint lastError = global::_003CModule_003E.GetLastError();
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterNodeKey);
		if (safeRegistryHandle.IsInvalid)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)lastError, Resources.ClusterNode_GetRegistryKeyFailed_Text, Name);
		}
		return new ClusterRegistryKey(m_cluster, safeRegistryHandle);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool WillEvictLoseQuorum()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (CanProcessClusterServiceCommands)
		{
			return m_cluster.WillVoterLoseQuorum(117440581u, GetNodeId());
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool WillDownLoseQuorum()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (CanProcessClusterServiceCommands)
		{
			return m_cluster.WillVoterLoseQuorum(117440585u, GetNodeId());
		}
		return false;
	}

	public void DeleteCanceled()
	{
		OnDeleting(DeletingStage.Canceled);
	}

	[HandleProcessCorruptedStateExceptions]
	protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			try
			{
				_007EClusterNode();
				return;
			}
			finally
			{
				base.Dispose(A_0: true);
			}
		}
		base.Dispose(A_0: false);
	}
}
