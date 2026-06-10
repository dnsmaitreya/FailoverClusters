using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;

namespace KDDSL.ServerClusters;

public class ClusterNetworkInterface : ClusterItem
{
	private volatile string m_name;

	private volatile string m_lastLoadedName;

	private volatile bool m_loadingName;

	private NetworkInterfaceState m_state;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private volatile ClusterNetwork m_network;

	private object m_stateLock;

	private Guid m_Id;

	private bool m_closed;

	private Cluster m_cluster;

	private SafeNetworkInterfaceHandle m_hNetInterface;

	private object m_propertiesLockObject;

	private static object m_creationLockObject = new object();

	private EventHandler _003Cbacking_store_003EStateChanged;

	private EventHandler _003Cbacking_store_003EPropertiesChanged;

	private EventHandler _003Cbacking_store_003EDeleted;

	public Cluster Cluster => m_cluster;

	public NetworkInterfaceState State
	{
		get
		{
			ThreadWatchdog.PerformUIThreadCheck();
			NetworkInterfaceState networkInterfaceState = m_state;
			if (ClusterItem.CachingDisabled || IsDeleted)
			{
				networkInterfaceState = NetworkInterfaceState.Unknown;
			}
			if (networkInterfaceState == NetworkInterfaceState.Unknown)
			{
				while (!IsDeleted)
				{
					LoadState();
					networkInterfaceState = m_state;
					if (networkInterfaceState != NetworkInterfaceState.Unknown)
					{
						break;
					}
				}
			}
			return networkInterfaceState;
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

	internal unsafe _HNETINTERFACE* Handle
	{
		get
		{
			m_lifetimeHelper.CheckObjectState();
			return m_hNetInterface.DangerousGetNetworkInterfaceHandle();
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

	private ClusterNetworkInterface(Cluster cluster, SafeNetworkInterfaceHandle hNetworkInterface, Guid id, string networkInterfaceName)
	{
		try
		{
			if (cluster == null)
			{
				throw new ArgumentNullException("cluster");
			}
			m_hNetInterface = hNetworkInterface;
			m_propertiesLockObject = new object();
			m_stateLock = new object();
			m_lastLoadedName = networkInterfaceName;
			m_name = networkInterfaceName;
			m_cluster = cluster;
			m_state = NetworkInterfaceState.Unknown;
			m_lifetimeHelper = new ObjectLifetimeHelper();
			m_Id = id;
			m_closed = false;
			UpdateProperties();
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

	private static SafeNetworkInterfaceHandle OpenNetworkInterface(Cluster cluster, string networkInterfaceName)
	{
		return NativeMethods.OpenClusterNetworkInterface(cluster, networkInterfaceName);
	}

	private static Guid GetId(Cluster cluster, SafeNetworkInterfaceHandle hNetworkInterface)
	{
		NetworkInterfaceControlExecutor networkInterfaceControlExecutor = new NetworkInterfaceControlExecutor(hNetworkInterface, cluster);
		return networkInterfaceControlExecutor.GetId(networkInterfaceControlExecutor);
	}

	private void ResetState()
	{
		m_state = NetworkInterfaceState.Unknown;
	}

	private void ResetProperties()
	{
		m_name = null;
	}

	private unsafe void LoadState()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (m_state != NetworkInterfaceState.Unknown && !ClusterItem.CachingDisabled)
		{
			return;
		}
		Monitor.Enter(m_stateLock);
		try
		{
			if (m_hNetInterface != null && (m_state == NetworkInterfaceState.Unknown || ClusterItem.CachingDisabled))
			{
				NetworkInterfaceState clusterNetInterfaceState = (NetworkInterfaceState)global::_003CModule_003E.GetClusterNetInterfaceState(m_hNetInterface.DangerousGetNetworkInterfaceHandle());
				if (clusterNetInterfaceState == NetworkInterfaceState.Unknown)
				{
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)global::_003CModule_003E.GetLastError(), Resources.NetworkInterface_LoadState_Fail_Text, Name);
				}
				m_state = clusterNetInterfaceState;
			}
		}
		finally
		{
			Monitor.Exit(m_stateLock);
		}
	}

	private void UpdateProperties()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_propertiesLockObject);
		try
		{
			if (!(m_name == null) && !ClusterItem.CachingDisabled)
			{
				return;
			}
			if (m_loadingName)
			{
				m_name = m_lastLoadedName;
				return;
			}
			m_loadingName = true;
			try
			{
				Property property = GetCommonProperties(PropertyCollectionSet.ReadOnly)["Name"];
				m_name = (string)property.Value;
			}
			finally
			{
				m_loadingName = false;
			}
		}
		finally
		{
			Monitor.Exit(m_propertiesLockObject);
		}
	}

	private string GetName()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string text = m_name;
		if (ClusterItem.CachingDisabled && !IsDeleted)
		{
			text = null;
		}
		if (text == null)
		{
			do
			{
				UpdateProperties();
				text = m_name;
			}
			while (text == null);
		}
		return text;
	}

	internal static ClusterNetworkInterface CreateObject(Cluster cluster, string networkInterfaceName, Guid networkInterfaceId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		SafeNetworkInterfaceHandle safeNetworkInterfaceHandle = NativeMethods.OpenClusterNetworkInterface(cluster, networkInterfaceName);
		Guid id;
		if (networkInterfaceId != Guid.Empty)
		{
			id = networkInterfaceId;
		}
		else
		{
			NetworkInterfaceControlExecutor networkInterfaceControlExecutor = new NetworkInterfaceControlExecutor(safeNetworkInterfaceHandle, cluster);
			id = networkInterfaceControlExecutor.GetId(networkInterfaceControlExecutor);
		}
		Monitor.Enter(m_creationLockObject);
		ClusterNetworkInterface clusterNetworkInterface = null;
		try
		{
			clusterNetworkInterface = cluster.ObjectMgr.GetNetworkInterfaceInstance(id);
			if (clusterNetworkInterface == null)
			{
				clusterNetworkInterface = new ClusterNetworkInterface(cluster, safeNetworkInterfaceHandle, id, networkInterfaceName);
			}
			else
			{
				SafeNetworkInterfaceHandle safeNetworkInterfaceHandle2 = safeNetworkInterfaceHandle;
				IDisposable disposable = safeNetworkInterfaceHandle;
				((IDisposable)safeNetworkInterfaceHandle)?.Dispose();
			}
		}
		finally
		{
			Monitor.Exit(m_creationLockObject);
		}
		return clusterNetworkInterface;
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
			m_state = NetworkInterfaceState.Unknown;
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
		bool flag = true;
		m_lifetimeHelper.AquireDisposeLock();
		try
		{
			m_lifetimeHelper.CheckObjectState();
			m_name = null;
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
				raise_PropertiesChanged(this, EventArgs.Empty);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event PropertiesChanged - {0}");
			}
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
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event Deleted");
		}
	}

	internal override void Refresh()
	{
		//Discarded unreachable code: IL_0037
		Exception ex = null;
		try
		{
			m_state = NetworkInterfaceState.Unknown;
			m_name = null;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.NetworkInterface_Refresh_Fail_Text,
				m_name
			});
		}
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
				SafeNetworkInterfaceHandle hNetInterface = m_hNetInterface;
				IDisposable disposable = hNetInterface;
				((IDisposable)hNetInterface)?.Dispose();
				m_hNetInterface = null;
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
	protected void raise_Deleted(object value0, EventArgs value1)
	{
		_003Cbacking_store_003EDeleted?.Invoke(value0, value1);
	}

	public ClusterNetwork GetNetwork()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (m_network == null)
		{
			PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
			string name = "Network";
			string networkName = (string)commonProperties.GetProperty(name).Value;
			Cluster cluster = m_cluster;
			m_network = cluster.GetNetwork(networkName);
		}
		return m_network;
	}

	public override ControlExecutor GetControlExecutor()
	{
		Cluster cluster = m_cluster;
		return new NetworkInterfaceControlExecutor(this, cluster);
	}

	public override PropertyCollection GetCommonProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_003c, IL_003e
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Common, propSet);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.NetworkInterface_GetCommonProperties_Fail_Text,
				m_name
			});
		}
	}

	public override PropertyCollection GetPrivateProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_003c, IL_003e
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Private, propSet);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.NetworkInterface_GetPrivateProperties_Fail_Text,
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
		HKEY__* clusterNetInterfaceKey = global::_003CModule_003E.GetClusterNetInterfaceKey(Handle, ClusterRegistryKey.RegistryRightsToRegSam(rights));
		uint lastError = global::_003CModule_003E.GetLastError();
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterNetInterfaceKey);
		if (safeRegistryHandle.IsInvalid)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)lastError, Resources.ClusterNetworkInterface_GetRegistryKeyFailed_Text, Name);
		}
		return new ClusterRegistryKey(m_cluster, safeRegistryHandle);
	}
}
