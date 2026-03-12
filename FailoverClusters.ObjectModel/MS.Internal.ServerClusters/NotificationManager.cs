using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

internal class NotificationManager : IDisposable
{
	private Cluster m_cluster;

	private NotificationQueue m_queue;

	private Thread m_harvestThread;

	private Thread[] m_dispatchThreads;

	private ManualResetEvent m_finishEvent;

	private NotificationsEventHandler m_notificationMonitor;

	private object m_lockObject;

	private SafeChangeHandle m_hNotifyPort;

	private unsafe _HCHANGE* Handle => m_hNotifyPort.DangerousGetChangeHandle();

	private void WaitForThreadShutdown()
	{
		bool flag = true;
		flag = m_harvestThread.Join(global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EThreadShutdownTimeout) && flag;
		Thread[] dispatchThreads = m_dispatchThreads;
		int num = 0;
		if (0 < (nint)dispatchThreads.LongLength)
		{
			do
			{
				Thread thread = dispatchThreads[num];
				if (thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
				{
					flag = thread.Join(global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EThreadShutdownTimeout) && flag;
				}
				num++;
			}
			while (num < (nint)dispatchThreads.LongLength);
		}
		if (flag)
		{
			((IDisposable)m_finishEvent)?.Dispose();
		}
	}

	private void HarvestThreadProc()
	{
		try
		{
			HarvestNotifications();
		}
		catch (ThreadAbortException)
		{
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown in notification harvest proc");
		}
	}

	private unsafe void HarvestNotifications()
	{
		//IL_002d: Expected I4, but got I8
		//IL_00cc: Expected I4, but got I8
		uint num = 0u;
		uint num2 = 128u;
		ulong dwNotifyKey = 0uL;
		uint dwFilterType = 0u;
		try
		{
			ushort* ptr = (ushort*)global::_003CModule_003E.new_005B_005D(256uL);
			ushort* ptr2 = ptr;
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ptr, 0, 256);
			while (!m_finishEvent.WaitOne(0, exitContext: false))
			{
				Monitor.Enter(m_lockObject);
				try
				{
					if (m_hNotifyPort.IsInvalid)
					{
						ClusterLog.LogError("The notification port has already been closed.");
						break;
					}
					num = global::_003CModule_003E.GetClusterNotify(Handle, &dwNotifyKey, &dwFilterType, ptr2, &num2, 10000u);
				}
				finally
				{
					Monitor.Exit(m_lockObject);
				}
				switch (num)
				{
				case 258u:
					break;
				case 234u:
				{
					num2++;
					uint num3 = num2;
					ushort* ptr3 = ptr2;
					global::_003CModule_003E.delete_005B_005D(ptr2);
					ulong num4 = (ulong)num3 * 2uL;
					ushort* ptr4 = (ushort*)global::_003CModule_003E.new_005B_005D(num4);
					ptr2 = ptr4;
					// IL initblk instruction
					System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ptr4, 0, num4);
					break;
				}
				default:
				{
					string name = InteropHelp.WstrToString(ptr2);
					string text = "Harvested";
					m_queue.Enqueue(dwNotifyKey, name, dwFilterType);
					break;
				}
				case 6u:
					return;
				}
			}
		}
		finally
		{
			Global.WriteLineThreadTerminated();
		}
	}

	private void DispatchThreadProc()
	{
		//Discarded unreachable code: IL_0100, IL_0110
		try
		{
			int num = 0;
			int num2 = 1;
			WaitHandle[] waitHandles = new WaitHandle[2] { m_finishEvent, m_queue.WaitHandle };
			while (!m_finishEvent.SafeWaitHandle.IsClosed && WaitHandle.WaitAny(waitHandles) != 0)
			{
				int notificationsDiscarded = 0;
				NotificationPayload notificationPayload = m_queue.Dequeue(ref notificationsDiscarded);
				if (notificationPayload == null)
				{
					continue;
				}
				string text = "Dispatching";
				uint filterType = notificationPayload.FilterType;
				string name = notificationPayload.Name;
				_ = notificationPayload.NotifyKey;
				try
				{
					NotificationsEventHandler notificationMonitor = m_notificationMonitor;
					if (notificationMonitor != null)
					{
						try
						{
							string name2 = FriendlyName((int)notificationPayload.NotifyKey, notificationPayload.Name, (int)notificationPayload.FilterType);
							notificationMonitor(this, new NotificationEventArgs(m_queue.Count, notificationsDiscarded, (int)notificationPayload.NotifyKey, name2, GetFilterName(notificationPayload.FilterType)));
						}
						catch (Exception)
						{
						}
					}
					DispatchNotification(notificationPayload.NotifyKey, notificationPayload.Name, notificationPayload.FilterType);
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception caughtException)
				{
					ExceptionHelp.LogException(caughtException, "Exception thrown while dispatching notification");
				}
			}
		}
		catch (Exception caughtException2)
		{
			ExceptionHelp.LogException(caughtException2, "Exception thrown in notification dispatch proc");
		}
		finally
		{
			Global.WriteLineThreadTerminated();
		}
	}

	private void DispatchNotification(ulong dwNotifyKey, string name, uint dwFilterType)
	{
		if (dwFilterType <= 65536)
		{
			if (dwFilterType != 65536)
			{
				if (dwFilterType <= 256)
				{
					if (dwFilterType != 256)
					{
						if (dwFilterType == 0)
						{
							return;
						}
						if (dwFilterType > 2)
						{
							if (dwFilterType == 4)
							{
								goto IL_01f7;
							}
							switch (dwFilterType)
							{
							case 16u:
							case 32u:
							case 64u:
							case 128u:
								OnRegistryNotification(dwNotifyKey, name, dwFilterType);
								return;
							case 8u:
								break;
							default:
								return;
							}
							m_cluster.ObjectMgr.SyncNodeRename(dwNotifyKey, name);
						}
						OnNodeNotification(dwNotifyKey, name, dwFilterType);
						return;
					}
				}
				else if (dwFilterType != 512)
				{
					if (dwFilterType == 1024)
					{
						goto IL_01f7;
					}
					if (dwFilterType != 2048)
					{
						if (dwFilterType != 4096)
						{
							if (dwFilterType != 8192)
							{
								if (dwFilterType == 16384)
								{
									goto IL_01f7;
								}
								if (dwFilterType != 32768)
								{
									return;
								}
								m_cluster.ObjectMgr.SyncGroupRename(dwNotifyKey, name);
							}
							OnGroupNotification(dwNotifyKey, name, dwFilterType);
						}
						else
						{
							OnClusterNotification(dwNotifyKey, name, 4096u);
						}
						return;
					}
					m_cluster.ObjectMgr.SyncResourceRename(dwNotifyKey, name);
				}
				OnResourceNotification(dwNotifyKey, name, dwFilterType);
				return;
			}
			goto IL_01a6;
		}
		if (dwFilterType <= 16777216)
		{
			if (dwFilterType != 16777216)
			{
				if (dwFilterType != 131072)
				{
					if (dwFilterType == 262144)
					{
						m_cluster.ObjectMgr.SyncResourceTypeRename(dwNotifyKey, name);
						goto IL_01a6;
					}
					if (dwFilterType != 524288)
					{
						if (dwFilterType != 1048576 && dwFilterType != 2097152)
						{
							if (dwFilterType == 4194304)
							{
								goto IL_01f7;
							}
							if (dwFilterType != 8388608)
							{
								return;
							}
							m_cluster.ObjectMgr.SyncNetworkRename(dwNotifyKey, name);
						}
						OnNetworkNotification(dwNotifyKey, name, dwFilterType);
						return;
					}
				}
				goto IL_01f7;
			}
		}
		else if (dwFilterType != 33554432)
		{
			if (dwFilterType != 67108864)
			{
				if (dwFilterType == 134217728)
				{
					m_cluster.ObjectMgr.SyncNetworkInterfaceRename(dwNotifyKey, name);
					goto IL_0202;
				}
				if (dwFilterType != 268435456 && dwFilterType != 536870912 && dwFilterType != 1073741824)
				{
					return;
				}
			}
			goto IL_01f7;
		}
		goto IL_0202;
		IL_01f7:
		OnClusterNotification(dwNotifyKey, name, dwFilterType);
		return;
		IL_01a6:
		OnResourceTypeNotification(dwNotifyKey, name, dwFilterType);
		return;
		IL_0202:
		OnNetworkInterfaceNotification(dwNotifyKey, name, dwFilterType);
	}

	private void OnClusterNotification(ulong dwNotifyKey, string name, uint dwFilterType)
	{
		switch (dwFilterType)
		{
		case 131072u:
			m_cluster.OnResourceTypesChanged(name, Guid.Empty, ClusterObjectEventType.Added);
			break;
		case 16384u:
			m_cluster.OnGroupsChanged(name, Guid.Empty, ClusterObjectEventType.Added);
			break;
		case 4096u:
			m_cluster.OnGroupStateChanged(name);
			break;
		case 1024u:
			m_cluster.OnResourcesChanged(name, Guid.Empty, ClusterObjectEventType.Added);
			break;
		case 4u:
			m_cluster.OnNodesChanged(name, Guid.Empty, ClusterObjectEventType.Added);
			break;
		case 524288u:
			m_cluster.OnConnectionChanged(ClusterConnectionState.Reconnected);
			break;
		case 1073741824u:
			m_cluster.OnPropertiesChanged();
			break;
		case 536870912u:
			m_cluster.OnConnectionChanged(ClusterConnectionState.Disconnected);
			break;
		case 268435456u:
			m_cluster.OnQuorumChanged(name);
			break;
		case 67108864u:
		{
			ClusterNetworkInterface networkInterface = m_cluster.GetNetworkInterface(name);
			networkInterface.GetNetwork().InvalidateNetworkInterfaces();
			Guid id = networkInterface.Id;
			m_cluster.OnNetworkInterfacesChanged(name, id, ClusterObjectEventType.Added);
			break;
		}
		case 4194304u:
			m_cluster.OnNetworksChanged(name, Guid.Empty, ClusterObjectEventType.Added);
			break;
		}
	}

	private void OnRegistryNotification(ulong dwNotifyKey, string name, uint dwFilterType)
	{
		ClusterRegistryChangeType type = ClusterRegistryChangeType.None;
		switch (dwFilterType)
		{
		case 128u:
			type = ClusterRegistryChangeType.Subtree;
			break;
		case 64u:
			type = ClusterRegistryChangeType.Value;
			break;
		case 32u:
			type = ClusterRegistryChangeType.Attributes;
			break;
		case 16u:
			type = ClusterRegistryChangeType.Name;
			break;
		}
		m_cluster.OnRegistryChanged(name, type);
	}

	private void OnGroupNotification(ulong dwNotifyKey, string groupName, uint dwFilterType)
	{
		Exception ex = null;
		ClusterGroup groupInstance = m_cluster.ObjectMgr.GetGroupInstance(dwNotifyKey);
		if (groupInstance == null)
		{
			groupInstance = m_cluster.ObjectMgr.GetGroupInstance(groupName);
		}
		try
		{
			if (groupInstance != null)
			{
				switch (dwFilterType)
				{
				case 32768u:
					groupInstance.OnPropertiesChanged();
					break;
				case 8192u:
					groupInstance.OnDeleted();
					break;
				case 4096u:
					groupInstance.OnStateChanged();
					break;
				}
			}
		}
		catch (ClusterObjectDeletedException)
		{
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error raising NotificationManager::OnGroupNotification");
		}
		if (dwFilterType == 8192)
		{
			Guid id = groupInstance?.Id ?? Guid.Empty;
			m_cluster.OnGroupsChanged(groupName, id, ClusterObjectEventType.Deleted);
		}
	}

	private void OnResourceNotification(ulong dwNotifyKey, string resourceName, uint dwFilterType)
	{
		ClusterResource resourceInstance = m_cluster.ObjectMgr.GetResourceInstance(dwNotifyKey);
		if (resourceInstance == null)
		{
			resourceInstance = m_cluster.ObjectMgr.GetResourceInstance(resourceName);
			if (resourceInstance == null)
			{
				goto IL_0065;
			}
		}
		if (dwFilterType != 256)
		{
			if (dwFilterType == 512)
			{
				resourceInstance.OnDeleted();
				goto IL_006d;
			}
			if (dwFilterType == 2048)
			{
				m_cluster.OnResourcePropertyChanged(resourceName, resourceInstance);
				resourceInstance.OnPropertiesChanged();
				return;
			}
		}
		else
		{
			resourceInstance.OnStateChanged();
		}
		goto IL_0065;
		IL_0065:
		if (dwFilterType != 512)
		{
			return;
		}
		goto IL_006d;
		IL_006d:
		Guid id = resourceInstance?.Id ?? Guid.Empty;
		m_cluster.OnResourcesChanged(resourceName, id, ClusterObjectEventType.Deleted);
	}

	private void OnResourceTypeNotification(ulong dwNotifyKey, string resourceTypeName, uint dwFilterType)
	{
		ClusterResourceType resourceType = m_cluster.GetResourceType(resourceTypeName);
		switch (dwFilterType)
		{
		case 262144u:
			resourceType.OnPropertiesChanged();
			return;
		case 65536u:
			resourceType.OnDeleted();
			break;
		}
		if (dwFilterType == 65536)
		{
			Guid id = resourceType?.Id ?? Guid.Empty;
			m_cluster.OnResourceTypesChanged(resourceTypeName, id, ClusterObjectEventType.Deleted);
		}
	}

	private void OnNetworkNotification(ulong dwNotifyKey, string networkName, uint dwFilterType)
	{
		ClusterNetwork networkInstance = m_cluster.ObjectMgr.GetNetworkInstance(dwNotifyKey);
		if (networkInstance == null)
		{
			networkInstance = m_cluster.ObjectMgr.GetNetworkInstance(networkName);
			if (networkInstance == null)
			{
				goto IL_0058;
			}
		}
		if (dwFilterType != 1048576)
		{
			if (dwFilterType == 2097152)
			{
				networkInstance.OnDeleted();
				goto IL_0060;
			}
			if (dwFilterType == 8388608)
			{
				networkInstance.OnPropertiesChanged();
				return;
			}
		}
		else
		{
			networkInstance.OnStateChanged();
		}
		goto IL_0058;
		IL_0058:
		if (dwFilterType != 2097152)
		{
			return;
		}
		goto IL_0060;
		IL_0060:
		Guid id = networkInstance?.Id ?? Guid.Empty;
		m_cluster.OnNetworksChanged(networkName, id, ClusterObjectEventType.Deleted);
	}

	private void OnNetworkInterfaceNotification(ulong dwNotifyKey, string netInterfaceName, uint dwFilterType)
	{
		ClusterNetworkInterface networkInterfaceInstance = m_cluster.ObjectMgr.GetNetworkInterfaceInstance(dwNotifyKey);
		if (networkInterfaceInstance == null)
		{
			networkInterfaceInstance = m_cluster.ObjectMgr.GetNetworkInterfaceInstance(netInterfaceName);
			if (networkInterfaceInstance == null)
			{
				goto IL_0058;
			}
		}
		if (dwFilterType != 16777216)
		{
			if (dwFilterType == 33554432)
			{
				networkInterfaceInstance.OnDeleted();
				goto IL_0060;
			}
			if (dwFilterType == 134217728)
			{
				networkInterfaceInstance.OnPropertiesChanged();
				return;
			}
		}
		else
		{
			networkInterfaceInstance.OnStateChanged();
		}
		goto IL_0058;
		IL_00ac:
		Guid id;
		m_cluster.OnNetworkInterfacesChanged(netInterfaceName, id, ClusterObjectEventType.Deleted);
		return;
		IL_0058:
		if (dwFilterType != 33554432)
		{
			return;
		}
		goto IL_0060;
		IL_0060:
		if (networkInterfaceInstance != null)
		{
			foreach (ClusterNetwork network in networkInterfaceInstance.Cluster.ObjectMgr.GetNetworks())
			{
				network.InvalidateNetworkInterfaces();
			}
			if (networkInterfaceInstance != null)
			{
				id = networkInterfaceInstance.Id;
				goto IL_00ac;
			}
		}
		id = Guid.Empty;
		goto IL_00ac;
	}

	private void OnNodeNotification(ulong dwNotifyKey, string nodeName, uint dwFilterType)
	{
		ClusterNode nodeInstance = m_cluster.ObjectMgr.GetNodeInstance(dwNotifyKey);
		if (nodeInstance == null)
		{
			nodeInstance = m_cluster.ObjectMgr.GetNodeInstance(nodeName);
			if (nodeInstance == null)
			{
				goto IL_004c;
			}
		}
		if (dwFilterType != 1)
		{
			if (dwFilterType == 2)
			{
				nodeInstance.OnDeleted();
				goto IL_0050;
			}
			if (dwFilterType == 8)
			{
				nodeInstance.OnPropertiesChanged();
				return;
			}
		}
		else
		{
			nodeInstance.OnStateChanged();
		}
		goto IL_004c;
		IL_004c:
		if (dwFilterType != 2)
		{
			return;
		}
		goto IL_0050;
		IL_0050:
		Guid id = nodeInstance?.Id ?? Guid.Empty;
		m_cluster.OnNodesChanged(nodeName, id, ClusterObjectEventType.Deleted);
	}

	private unsafe void OnGroupLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterGroup> e)
	{
		if (!m_finishEvent.WaitOne(0, exitContext: false) && e.Lifetime == ObjectLifetime.Start)
		{
			uint num = global::_003CModule_003E.RegisterClusterNotify(m_hNotifyPort.DangerousGetChangeHandle(), 36864u, e.ClusterObject.Handle, e.Id);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RegisterClusterNotifyFail_Text, e.ClusterObject.Name);
			}
		}
	}

	private unsafe void OnResourceLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterResource> e)
	{
		if (!m_finishEvent.WaitOne(0, exitContext: false) && e.Lifetime == ObjectLifetime.Start)
		{
			uint num = global::_003CModule_003E.RegisterClusterNotify(m_hNotifyPort.DangerousGetChangeHandle(), 2304u, e.ClusterObject.Handle, e.Id);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RegisterClusterNotifyFail_Text, e.ClusterObject.Name);
			}
		}
	}

	private unsafe void OnNetworkLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterNetwork> e)
	{
		if (!m_finishEvent.WaitOne(0, exitContext: false) && e.Lifetime == ObjectLifetime.Start)
		{
			uint num = global::_003CModule_003E.RegisterClusterNotify(m_hNotifyPort.DangerousGetChangeHandle(), 11534336u, e.ClusterObject.Handle, e.Id);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RegisterClusterNotifyFail_Text, e.ClusterObject.Name);
			}
		}
	}

	private unsafe void OnNetworkInterfaceLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterNetworkInterface> e)
	{
		if (!m_finishEvent.WaitOne(0, exitContext: false) && e.Lifetime == ObjectLifetime.Start)
		{
			uint num = global::_003CModule_003E.RegisterClusterNotify(m_hNotifyPort.DangerousGetChangeHandle(), 184549376u, e.ClusterObject.Handle, e.Id);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RegisterClusterNotifyFail_Text, e.ClusterObject.Name);
			}
		}
	}

	private unsafe void OnNodeLifetimeChanged(object sender, ObjectLifetimeEventArgs<ClusterNode> e)
	{
		if (!m_finishEvent.WaitOne(0, exitContext: false) && e.Lifetime == ObjectLifetime.Start)
		{
			uint num = global::_003CModule_003E.RegisterClusterNotify(m_hNotifyPort.DangerousGetChangeHandle(), 11u, e.ClusterObject.Handle, e.Id);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RegisterClusterNotifyFail_Text, e.ClusterObject.Name);
			}
		}
	}

	private static string GetFilterName(uint dwFilterType)
	{
		return dwFilterType switch
		{
			128u => "CLUSTER_CHANGE_REGISTRY_SUBTREE", 
			64u => "CLUSTER_CHANGE_REGISTRY_VALUE", 
			32u => "CLUSTER_CHANGE_REGISTRY_ATTRIBUTES", 
			16u => "CLUSTER_CHANGE_REGISTRY_NAME", 
			8u => "CLUSTER_CHANGE_NODE_PROPERTY", 
			4u => "CLUSTER_CHANGE_NODE_ADDED", 
			2u => "CLUSTER_CHANGE_NODE_DELETED", 
			1u => "CLUSTER_CHANGE_NODE_STATE", 
			256u => "CLUSTER_CHANGE_RESOURCE_STATE", 
			32768u => "CLUSTER_CHANGE_GROUP_PROPERTY", 
			16384u => "CLUSTER_CHANGE_GROUP_ADDED", 
			8192u => "CLUSTER_CHANGE_GROUP_DELETED", 
			4096u => "CLUSTER_CHANGE_GROUP_STATE", 
			2048u => "CLUSTER_CHANGE_RESOURCE_PROPERTY", 
			1024u => "CLUSTER_CHANGE_RESOURCE_ADDED", 
			512u => "CLUSTER_CHANGE_RESOURCE_DELETED", 
			65536u => "CLUSTER_CHANGE_RESOURCE_TYPE_DELETED", 
			8388608u => "CLUSTER_CHANGE_NETWORK_PROPERTY", 
			4194304u => "CLUSTER_CHANGE_NETWORK_ADDED", 
			2097152u => "CLUSTER_CHANGE_NETWORK_DELETED", 
			1048576u => "CLUSTER_CHANGE_NETWORK_STATE", 
			524288u => "CLUSTER_CHANGE_CLUSTER_RECONNECT", 
			262144u => "CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY", 
			131072u => "CLUSTER_CHANGE_RESOURCE_TYPE_ADDED", 
			16777216u => "CLUSTER_CHANGE_NETINTERFACE_STATE", 
			2147483648u => "CLUSTER_CHANGE_HANDLE_CLOSE", 
			1073741824u => "CLUSTER_CHANGE_CLUSTER_PROPERTY", 
			536870912u => "CLUSTER_CHANGE_CLUSTER_STATE", 
			268435456u => "CLUSTER_CHANGE_QUORUM_STATE", 
			134217728u => "CLUSTER_CHANGE_NETINTERFACE_PROPERTY", 
			67108864u => "CLUSTER_CHANGE_NETINTERFACE_ADDED", 
			33554432u => "CLUSTER_CHANGE_NETINTERFACE_DELETED", 
			_ => "UNKNOWN", 
		};
	}

	private string FriendlyName(int key, string id, int filterType)
	{
		string result = id;
		try
		{
			switch (filterType)
			{
			case 1048576:
			case 2097152:
			case 8388608:
			{
				ClusterNetwork networkInstance = m_cluster.ObjectMgr.GetNetworkInstance((ulong)key);
				result = ((networkInstance == null) ? id : networkInstance.Name);
				break;
			}
			case 256:
			case 512:
			case 2048:
			{
				ClusterResource resourceInstance = m_cluster.ObjectMgr.GetResourceInstance((ulong)key);
				result = ((resourceInstance == null) ? id : resourceInstance.Name);
				break;
			}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	internal unsafe NotificationManager(Cluster cluster)
	{
		//IL_00db: Expected I, but got I8
		m_cluster = cluster;
		m_queue = new NotificationQueue();
		m_finishEvent = new ManualResetEvent(initialState: false);
		m_lockObject = new object();
		m_cluster.ObjectMgr.GroupLifetimeChanged += OnGroupLifetimeChanged;
		m_cluster.ObjectMgr.ResourceLifetimeChanged += OnResourceLifetimeChanged;
		m_cluster.ObjectMgr.NetworkLifetimeChanged += OnNetworkLifetimeChanged;
		m_cluster.ObjectMgr.NetworkInterfaceLifetimeChanged += OnNetworkInterfaceLifetimeChanged;
		m_cluster.ObjectMgr.NodeLifetimeChanged += OnNodeLifetimeChanged;
		if ((m_hNotifyPort = new SafeChangeHandle(global::_003CModule_003E.CreateClusterNotifyPort((_HCHANGE*)ulong.MaxValue, m_cluster.Handle, 1951344372u, 0uL))).IsInvalid)
		{
			int lastError = (int)global::_003CModule_003E.GetLastError();
			NativeMethods.AnalyzeAndThrow((lastError > 0) ? ((lastError & 0xFFFF) | -2147024896) : lastError, cluster.Name);
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)global::_003CModule_003E.GetLastError(), Resources.CreateClusterNotifyPortFail_Text, cluster.Name);
		}
		(m_harvestThread = new Thread(HarvestThreadProc)).Priority = ThreadPriority.Normal;
		m_harvestThread.IsBackground = true;
		m_harvestThread.Name = "NotificationHarvest";
		m_harvestThread.Start();
		m_dispatchThreads = new Thread[1];
		m_dispatchThreads[0] = new Thread(DispatchThreadProc);
		m_dispatchThreads[0].Priority = ThreadPriority.Normal;
		m_dispatchThreads[0].IsBackground = true;
		m_dispatchThreads[0].Name = string.Format(CultureInfo.InvariantCulture, "NotificationDispatch {0}", 0);
		m_dispatchThreads[0].Start();
	}

	private void _007ENotificationManager()
	{
		m_finishEvent.Set();
		Monitor.Enter(m_lockObject);
		try
		{
			m_hNotifyPort.Close();
		}
		finally
		{
			Monitor.Exit(m_lockObject);
		}
		m_cluster.ObjectMgr.GroupLifetimeChanged -= OnGroupLifetimeChanged;
		m_cluster.ObjectMgr.ResourceLifetimeChanged -= OnResourceLifetimeChanged;
		m_cluster.ObjectMgr.NetworkLifetimeChanged -= OnNetworkLifetimeChanged;
		m_cluster.ObjectMgr.NetworkInterfaceLifetimeChanged -= OnNetworkInterfaceLifetimeChanged;
		m_cluster.ObjectMgr.NodeLifetimeChanged -= OnNodeLifetimeChanged;
		WaitForThreadShutdown();
	}

	internal static void LogNotification(ulong dwNotifyKey, string name, uint dwFilterType, string extraMessage)
	{
	}

	internal static void LogNotification(NotificationPayload payload, string extraMessage)
	{
		_ = payload.FilterType;
		_ = payload.Name;
		_ = payload.NotifyKey;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool ShouldRaiseNotificationEvent(Exception e, ref bool rethrowException)
	{
		bool result = true;
		rethrowException = false;
		if (ExceptionHelp.IsFirstExceptionFound<ObjectDisposedException>(e))
		{
			result = false;
		}
		else if (ExceptionHelp.IsFirstExceptionFound<ClusterObjectDeletedException>(e))
		{
			ExceptionHelp.LogException(e, "Exception encounted while refreshing due to a notification");
			result = false;
		}
		else if (ExceptionHelp.IsFirstExceptionFound<Win32Exception>(e))
		{
			ExceptionHelp.LogException(e, "Exception encounted while refreshing due to a notification");
			result = false;
		}
		else
		{
			rethrowException = true;
		}
		return result;
	}

	public void RegisterMonitor(NotificationsEventHandler monitor)
	{
		m_notificationMonitor = monitor;
	}

	public void UnregisterMonitor()
	{
		m_notificationMonitor = null;
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007ENotificationManager();
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
