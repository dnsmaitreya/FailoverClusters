using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;

namespace KDDSL.ServerClusters;

public class ClusterNetwork : ClusterItem
{
	private volatile string m_name;

	private volatile string m_lastLoadedName;

	private volatile bool m_loadingName;

	private bool m_closed;

	private NetworkState m_state;

	private Guid m_Id;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private Cluster m_cluster;

	private SafeNetworkHandle m_hNetwork;

	private volatile ClusterNetworkInterfaceCollection m_networkInterfaces;

	private object m_networkInterfacesLock;

	private object m_nameLockObject;

	private object m_stateLock;

	private static object m_creationLockObject = new object();

	private EventHandler _003Cbacking_store_003EStateChanged;

	private EventHandler _003Cbacking_store_003EPropertiesChanged;

	private EventHandler _003Cbacking_store_003EDeleted;

	public Cluster Cluster => m_cluster;

	public NetworkRole Role => GetNetworkRole();

	public NetworkState State
	{
		get
		{
			ThreadWatchdog.PerformUIThreadCheck();
			NetworkState networkState = m_state;
			if (ClusterItem.CachingDisabled || IsDeleted)
			{
				networkState = NetworkState.Unknown;
			}
			if (networkState == NetworkState.Unknown)
			{
				while (!IsDeleted)
				{
					LoadState();
					networkState = m_state;
					if (networkState != NetworkState.Unknown)
					{
						break;
					}
				}
			}
			return networkState;
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

	internal unsafe _HNETWORK* Handle
	{
		get
		{
			m_lifetimeHelper.CheckObjectState();
			return m_hNetwork.DangerousGetNetworkHandle();
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

	private ClusterNetwork(Cluster cluster, SafeNetworkHandle hNetwork, Guid id, string networkName)
	{
		try
		{
			if (cluster == null)
			{
				throw new ArgumentNullException("cluster");
			}
			m_hNetwork = hNetwork;
			m_nameLockObject = new object();
			m_stateLock = new object();
			m_lastLoadedName = networkName;
			m_name = networkName;
			m_cluster = cluster;
			m_state = NetworkState.Unknown;
			m_Id = id;
			m_lifetimeHelper = new ObjectLifetimeHelper();
			m_networkInterfaces = null;
			m_networkInterfacesLock = new object();
			m_closed = false;
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

	private static SafeNetworkHandle OpenNetwork(Cluster cluster, string networkName)
	{
		return NativeMethods.OpenClusterNetwork(cluster, networkName);
	}

	private static Guid GetId(Cluster cluster, SafeNetworkHandle hNetwork)
	{
		NetworkControlExecutor networkControlExecutor = new NetworkControlExecutor(hNetwork, cluster);
		return networkControlExecutor.GetId(networkControlExecutor);
	}

	private void ResetState()
	{
		m_state = NetworkState.Unknown;
	}

	private unsafe void LoadState()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (m_state != NetworkState.Unknown && !ClusterItem.CachingDisabled)
		{
			return;
		}
		Monitor.Enter(m_stateLock);
		try
		{
			if (m_hNetwork != null && (m_state == NetworkState.Unknown || ClusterItem.CachingDisabled))
			{
				NetworkState clusterNetworkState = (NetworkState)global::_003CModule_003E.GetClusterNetworkState(m_hNetwork.DangerousGetNetworkHandle());
				if (clusterNetworkState == NetworkState.Unknown)
				{
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)global::_003CModule_003E.GetLastError(), Resources.Network_LoadState_Fail_Text, Name);
				}
				m_state = clusterNetworkState;
			}
		}
		finally
		{
			Monitor.Exit(m_stateLock);
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
				LoadName();
				text = m_name;
			}
			while (text == null);
		}
		return text;
	}

	private void ResetName()
	{
		m_name = null;
	}

	private void LoadName()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_nameLockObject);
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
			Monitor.Exit(m_nameLockObject);
		}
	}

	private NetworkRole GetNetworkRole()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
		string name = "Role";
		uint num = (uint)commonProperties.GetProperty(name).Value;
		return (NetworkRole)Enum.Parse(typeof(NetworkRole), num.ToString(CultureInfo.InvariantCulture));
	}

	private long GetItemCount(NetworkEnumType enumType)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		using SafeNetworkEnumHandle safeNetworkEnumHandle = new SafeNetworkEnumHandle(m_cluster, m_hNetwork, enumType);
		return safeNetworkEnumHandle.GetCount();
	}

	private AsyncEnumeration<ClusterNetworkInterface> BuildNetworkInterfaceAsyncEnum()
	{
		m_lifetimeHelper.CheckObjectState();
		SafeNetworkEnumHandle enumHandle = new SafeNetworkEnumHandle(m_cluster, m_hNetwork, NetworkEnumType.NetworkInterface);
		return new AsyncEnumeration<ClusterNetworkInterface>(m_cluster.GetNetworkInterface, enumHandle);
	}

	private void OnNetworkInterfaceAsyncEnumCompleted(object sender, EventArgs e)
	{
		AsyncEnumeration<ClusterNetworkInterface> asyncEnumeration = (AsyncEnumeration<ClusterNetworkInterface>)sender;
		if (asyncEnumeration.EnumeratedItems == null)
		{
			return;
		}
		ClusterNetworkInterfaceCollection clusterNetworkInterfaceCollection = new ClusterNetworkInterfaceCollection();
		foreach (ClusterNetworkInterface enumeratedItem in asyncEnumeration.EnumeratedItems)
		{
			clusterNetworkInterfaceCollection.InternalAdd(enumeratedItem);
		}
		m_networkInterfaces = clusterNetworkInterfaceCollection;
	}

	internal static ClusterNetwork CreateObject(Cluster cluster, string networkName, Guid networkId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		SafeNetworkHandle safeNetworkHandle = NativeMethods.OpenClusterNetwork(cluster, networkName);
		Guid id;
		if (networkId != Guid.Empty)
		{
			id = networkId;
		}
		else
		{
			NetworkControlExecutor networkControlExecutor = new NetworkControlExecutor(safeNetworkHandle, cluster);
			id = networkControlExecutor.GetId(networkControlExecutor);
		}
		Monitor.Enter(m_creationLockObject);
		ClusterNetwork clusterNetwork = null;
		try
		{
			clusterNetwork = cluster.ObjectMgr.GetNetworkInstance(id);
			if (clusterNetwork == null)
			{
				clusterNetwork = new ClusterNetwork(cluster, safeNetworkHandle, id, networkName);
			}
			else
			{
				SafeNetworkHandle safeNetworkHandle2 = safeNetworkHandle;
				IDisposable disposable = safeNetworkHandle;
				((IDisposable)safeNetworkHandle)?.Dispose();
			}
		}
		finally
		{
			Monitor.Exit(m_creationLockObject);
		}
		return clusterNetwork;
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
			m_state = NetworkState.Unknown;
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
				ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event PropertiesChanged");
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
		//Discarded unreachable code: IL_0040
		Exception ex = null;
		try
		{
			m_networkInterfaces = null;
			m_state = NetworkState.Unknown;
			m_name = null;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Network_Refresh_Fail_Text,
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
				SafeNetworkHandle hNetwork = m_hNetwork;
				IDisposable disposable = hNetwork;
				((IDisposable)hNetwork)?.Dispose();
				m_hNetwork = null;
				ClusterNetworkInterfaceCollection networkInterfaces = m_networkInterfaces;
				if (networkInterfaces is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
				m_networkInterfaces = null;
			}
		}
		finally
		{
			Monitor.Exit(m_stateLock);
		}
	}

	internal void InvalidateNetworkInterfaces()
	{
		m_networkInterfaces = null;
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

	public ClusterNetworkInterfaceCollection GetNetworkInterfaces()
	{
		//Discarded unreachable code: IL_007d, IL_007f
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterNetworkInterfaceCollection clusterNetworkInterfaceCollection = m_networkInterfaces;
		if (clusterNetworkInterfaceCollection == null || ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_networkInterfacesLock);
			try
			{
				clusterNetworkInterfaceCollection = m_networkInterfaces;
				if (clusterNetworkInterfaceCollection == null || ClusterItem.CachingDisabled)
				{
					clusterNetworkInterfaceCollection = (m_networkInterfaces = new ClusterNetworkInterfaceCollection(BuildNetworkInterfaceAsyncEnum()));
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Network_GetNetworkInterfaces_Fail_Text,
					m_name
				});
			}
			finally
			{
				Monitor.Exit(m_networkInterfacesLock);
			}
		}
		return clusterNetworkInterfaceCollection;
	}

	public AsyncEnumerationStatus GetNetworkInterfacesAsync(AsyncEnumerationCallback<ClusterNetworkInterface> callback)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterNetworkInterfaceCollection networkInterfaces = m_networkInterfaces;
		AsyncEnumeration<ClusterNetworkInterface> asyncEnumeration;
		if (networkInterfaces != null && !ClusterItem.CachingDisabled)
		{
			asyncEnumeration = new AsyncEnumeration<ClusterNetworkInterface>(networkInterfaces);
		}
		else
		{
			asyncEnumeration = BuildNetworkInterfaceAsyncEnum();
			asyncEnumeration.EnumerationComplete += OnNetworkInterfaceAsyncEnumCompleted;
		}
		asyncEnumeration.SetCallback(callback);
		return asyncEnumeration.StartEnumeration(useDifferentThread: true);
	}

	public long GetNetworkInterfaceCount()
	{
		//Discarded unreachable code: IL_002b, IL_002d
		try
		{
			return GetItemCount(NetworkEnumType.NetworkInterface);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Network_GetNetworkInterfaceCount_Fail_Text,
				m_name
			});
		}
	}

	public override ControlExecutor GetControlExecutor()
	{
		Cluster cluster = m_cluster;
		return new NetworkControlExecutor(this, cluster);
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
				Resources.Network_GetCommonProperties_Fail_Text,
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
				Resources.Network_GetPrivateProperties_Fail_Text,
				m_name
			});
		}
	}

	public unsafe void Rename(string newName)
	{
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		if (newName == null)
		{
			throw new ArgumentNullException("newName");
		}
		try
		{
			ptr = InteropHelp.StringToWstr(newName);
			uint num = global::_003CModule_003E.SetClusterNetworkName(Handle, ptr);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RenameNetworkFail_Text, Name, newName);
			}
			m_lastLoadedName = newName;
			m_name = newName;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
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
		HKEY__* clusterNetworkKey = global::_003CModule_003E.GetClusterNetworkKey(Handle, ClusterRegistryKey.RegistryRightsToRegSam(rights));
		uint lastError = global::_003CModule_003E.GetLastError();
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterNetworkKey);
		if (safeRegistryHandle.IsInvalid)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)lastError, Resources.ClusterNetwork_GetRegistryKeyFailed_Text, Name);
		}
		return new ClusterRegistryKey(m_cluster, safeRegistryHandle);
	}
}
