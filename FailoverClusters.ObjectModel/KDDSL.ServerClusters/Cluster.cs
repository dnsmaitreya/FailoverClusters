#define DEBUG
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Threading;
using _003CCppImplementationDetails_003E;
using FailoverClusters.UI.Common;

namespace KDDSL.ServerClusters;

public class Cluster : ClusterItem
{
	private int m_disposed;

	private volatile string m_name;

	private ushort m_majorVersion;

	private ushort m_minorVersion;

	private ushort m_buildNumber;

	private string m_vendorId;

	private string m_servicePack;

	private uint m_clusterHighestVersion;

	private uint m_clusterLowestVersion;

	private bool m_mixedMode;

	private string m_domain;

	private ClusterVersion m_curVersion;

	private volatile QuorumSettings m_quorumSettings;

	private SafeClusterHandle m_hCluster;

	private ReaderWriterLockSlim m_hClusterLock;

	private ObjectManager m_objectManager;

	private NotificationManager m_notificationManager;

	private bool proxyCluster;

	private unsafe _HCLUSTER* proxyHandle;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private Guid m_id;

	private object m_closeLockObject;

	private bool m_isClosed;

	private bool m_closed;

	private ClusterGroup m_coreClusterGroup;

	private ClusterGroup m_availableStorageGroup;

	private ClusterResource m_coreClusterNetName;

	private ClusterAccessRights m_apiAccessLevel;

	private IFrameworkCluster frameworkCluster;

	private AdminAccessPoint? m_AdminAccessPoint;

	private uint m_clusterFunctionalLevel;

	private uint m_clusterUpgradeVersion;

	private Dictionary<Guid, ClusterResource> m_clusterSharedVolumeResources;

	private ReaderWriterLock m_clusterSharedVolumeResourcesLock;

	private volatile ClusterNodeCollection m_nodes;

	private volatile ClusterStorageOnlyNodeCollection m_storageOnlyNodes;

	private volatile ClusterGroupCollection m_groups;

	private volatile ClusterResourceCollection m_resources;

	private volatile ClusterResourceTypeCollection m_resourceTypes;

	private volatile ClusterNetworkCollection m_networks;

	private object m_nodesLock;

	private object m_storageOnlyNodeLock;

	private object m_groupsLock;

	private object m_resourcesLock;

	private object m_resourceTypesLock;

	private object m_networksLock;

	private object m_clusterSharedVolumesStorageLock;

	private static string AvailableStoragePrivateProperty = "StorageGroup";

	private ControlCodesEventHandler m_controlCodesMonitor;

	internal ReaderWriterLock m_groupsCreateObjectLock;

	internal static ClusterActionCallback m_Callback = null;

	public const uint LogLevelMax = 5u;

	public const uint LogSizeDefault = 1536u;

	public const uint CoreGroupCount = 2u;

	private EventHandler _003Cbacking_store_003EPropertiesChanged;

	private EventHandler<ClusterObjectEventArgs> _003Cbacking_store_003EGroupsChanged;

	private EventHandler<ClusterGroupEventArgs> _003Cbacking_store_003EGroupStateChanged;

	private EventHandler<ClusterObjectEventArgs> _003Cbacking_store_003ENetworksChanged;

	private EventHandler<ClusterObjectEventArgs> _003Cbacking_store_003ENetworkInterfacesChanged;

	private EventHandler<ClusterObjectEventArgs> _003Cbacking_store_003EResourcesChanged;

	private EventHandler<ClusterObjectEventArgs> _003Cbacking_store_003EResourceTypesChanged;

	private EventHandler<ClusterObjectEventArgs> _003Cbacking_store_003ENodesChanged;

	private EventHandler<ClusterConnectionEventArgs> _003Cbacking_store_003EConnectionChanged;

	private EventHandler<ClusterQuorumEventArgs> _003Cbacking_store_003EQuorumChanged;

	private EventHandler<ClusterRegistryEventArgs> _003Cbacking_store_003ERegistryChanged;

	private EventHandler<ClusterResourceEventArgs> _003Cbacking_store_003EResourcePropertyChanged;

	private EventHandler<ClusterResourceEventArgs> _003Cbacking_store_003EClusterSharedVolumeAdded;

	private EventHandler<ClusterResourceEventArgs> _003Cbacking_store_003EClusterSharedVolumeRemoved;

	private NotificationsEventHandler _003Cbacking_store_003ENotifications;

	public ClusterAccessRights ApiAccessLevel => m_apiAccessLevel;

	public unsafe int MaximumNumberOfNodes
	{
		get
		{
			ThreadWatchdog.PerformUIThreadCheck();
			uint result = 0u;
			uint num = global::_003CModule_003E.ComputeMaximumNumberOfNodes(Handle, &result);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)num, Resources.Cluster_GetMaxNumberOfNodes_Failed_Text, m_name);
			}
			return (int)result;
		}
	}

	public string FqdnName => string.IsNullOrEmpty(Domain) ? Name : string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Name, Domain);

	public string Domain
	{
		get
		{
			if (m_domain == null || ClusterItem.CachingDisabled)
			{
				GetDomainInfo();
			}
			return m_domain;
		}
	}

	public string ServicePack
	{
		get
		{
			if (m_id == Guid.Empty)
			{
				LoadClusterInfo();
			}
			return m_servicePack;
		}
	}

	public string VendorId
	{
		get
		{
			if (m_id == Guid.Empty)
			{
				LoadClusterInfo();
			}
			return m_vendorId;
		}
	}

	public ClusterVersion CurrentVersion
	{
		get
		{
			if (m_curVersion == ClusterVersion.Unsupported)
			{
				if (m_majorVersion == 0)
				{
					LoadClusterInfo();
				}
				uint clusterFunctionalLevel = m_clusterFunctionalLevel;
				if (clusterFunctionalLevel != 0)
				{
					m_curVersion = ConvertFunctionalLevelToClusterVersion(clusterFunctionalLevel, m_clusterUpgradeVersion);
				}
				else
				{
					m_curVersion = ConvertMajorMinorVersionsToClusterVersion(m_majorVersion, m_minorVersion);
				}
			}
			return m_curVersion;
		}
	}

	public string Version
	{
		get
		{
			if (m_majorVersion == 0)
			{
				LoadClusterInfo();
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", m_majorVersion, m_minorVersion, m_buildNumber);
		}
	}

	public IFrameworkCluster FrameworkCluster
	{
		get
		{
			return frameworkCluster;
		}
		set
		{
			frameworkCluster = value;
		}
	}

	public bool IsConnected
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			//Discarded unreachable code: IL_0029
			m_lifetimeHelper.AquireDisposeLock();
			try
			{
				return !m_lifetimeHelper.IsDisposed;
			}
			finally
			{
				m_lifetimeHelper.ReleaseDisposeLock();
			}
		}
	}

	public AdminAccessPoint ManagementPointType
	{
		get
		{
			//Discarded unreachable code: IL_0038
			LockAccessToHandle(writeAccess: false);
			try
			{
				if (!m_AdminAccessPoint.HasValue || ClusterItem.CachingDisabled)
				{
					LoadAdminAccessPoint();
				}
				return m_AdminAccessPoint.Value;
			}
			finally
			{
				UnlockAccessToHandle(writeAccess: false);
			}
		}
	}

	public override Guid Id
	{
		get
		{
			if (m_id == Guid.Empty)
			{
				LoadClusterInfo();
			}
			return m_id;
		}
	}

	public string CachedName
	{
		get
		{
			if (m_name == null)
			{
				return string.Empty;
			}
			return m_name;
		}
	}

	public override string Name
	{
		get
		{
			if (m_name == null || ClusterItem.CachingDisabled)
			{
				LoadClusterInfo();
			}
			return m_name;
		}
	}

	public bool IsGreatherThanWindowsBlueCluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion > ClusterVersion.WindowsBlue;
		}
	}

	public bool IsGreaterThanWindows8Cluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion > ClusterVersion.Windows8;
		}
	}

	public bool IsGreaterThanWindows7Cluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion > ClusterVersion.Windows7;
		}
	}

	public bool IsGreaterThanWindows2008Cluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion > ClusterVersion.WindowsServer2008;
		}
	}

	public bool IsWindows2008Cluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion == ClusterVersion.WindowsServer2008;
		}
	}

	public bool IsWindows7Cluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion == ClusterVersion.Windows7;
		}
	}

	public bool IsWindowsBlueCluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion == ClusterVersion.WindowsBlue;
		}
	}

	public bool IsWindows8Cluster
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return CurrentVersion == ClusterVersion.Windows8;
		}
	}

	public ClusterVersion CurrentClientVersion => ConvertFunctionalLevelToClusterVersion(11u, 1u);

	public ClusterFunctionalLevel FunctionalLevel => GetFunctionalLevel();

	public bool WitnessDynamicWeight
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return GetWitnessDynamicWeight();
		}
	}

	public bool DynamicQuorumEnabled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return GetDynamicQuorumEnabled();
		}
	}

	public bool IsSharedVolumesEnabled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return GetSharedVolumeEnabledState();
		}
	}

	public string ConnectedTo => NativeMethods.GetNodeConnectedTo(this);

	public static uint LogLevelDefault => 3u;

	public unsafe string StorageSubSystemId => (string)GetRegistryKey(RegistryRights.ExecuteKey).GetValue(InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BI_0040GIHAAKDN_0040_003F_0024AAS_003F_0024AAu_003F_0024AAb_003F_0024AAs_003F_0024AAy_003F_0024AAs_003F_0024AAt_003F_0024AAe_003F_0024AAm_003F_0024AAI_003F_0024AAd_0040)));

	public ControlCodesEventHandler ControlCodesMonitor
	{
		get
		{
			return m_controlCodesMonitor;
		}
		set
		{
			m_controlCodesMonitor = value;
		}
	}

	public override bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return false;
		}
	}

	public SafeHandle ClusterHandle
	{
		get
		{
			//Discarded unreachable code: IL_0018
			LockAccessToHandle(writeAccess: false);
			try
			{
				return m_hCluster;
			}
			finally
			{
				UnlockAccessToHandle(writeAccess: false);
			}
		}
	}

	internal ObjectManager ObjectMgr => m_objectManager;

	internal unsafe _HCLUSTER* Handle
	{
		get
		{
			//Discarded unreachable code: IL_0030
			//IL_003a: Expected I, but got I8
			m_lifetimeHelper.CheckObjectState();
			LockAccessToHandle(writeAccess: false);
			try
			{
				return (_HCLUSTER*)m_hCluster.DangerousGetHandle().ToPointer();
			}
			finally
			{
				UnlockAccessToHandle(writeAccess: false);
			}
		}
	}

	public List<ClusterResource> ClusterSharedVolumeResources
	{
		get
		{
			//Discarded unreachable code: IL_0033
			m_clusterSharedVolumeResourcesLock.AcquireReaderLock(-1);
			try
			{
				List<ClusterResource> list = new List<ClusterResource>();
				list.AddRange(m_clusterSharedVolumeResources.Values);
				return list;
			}
			finally
			{
				m_clusterSharedVolumeResourcesLock.ReleaseReaderLock();
			}
		}
	}

	[SpecialName]
	public event NotificationsEventHandler Notifications
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ENotifications = (NotificationsEventHandler)Delegate.Combine(_003Cbacking_store_003ENotifications, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ENotifications = (NotificationsEventHandler)Delegate.Remove(_003Cbacking_store_003ENotifications, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterResourceEventArgs> ClusterSharedVolumeRemoved
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EClusterSharedVolumeRemoved = (EventHandler<ClusterResourceEventArgs>)Delegate.Combine(_003Cbacking_store_003EClusterSharedVolumeRemoved, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EClusterSharedVolumeRemoved = (EventHandler<ClusterResourceEventArgs>)Delegate.Remove(_003Cbacking_store_003EClusterSharedVolumeRemoved, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterResourceEventArgs> ClusterSharedVolumeAdded
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EClusterSharedVolumeAdded = (EventHandler<ClusterResourceEventArgs>)Delegate.Combine(_003Cbacking_store_003EClusterSharedVolumeAdded, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EClusterSharedVolumeAdded = (EventHandler<ClusterResourceEventArgs>)Delegate.Remove(_003Cbacking_store_003EClusterSharedVolumeAdded, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterResourceEventArgs> ResourcePropertyChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EResourcePropertyChanged = (EventHandler<ClusterResourceEventArgs>)Delegate.Combine(_003Cbacking_store_003EResourcePropertyChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EResourcePropertyChanged = (EventHandler<ClusterResourceEventArgs>)Delegate.Remove(_003Cbacking_store_003EResourcePropertyChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterRegistryEventArgs> RegistryChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ERegistryChanged = (EventHandler<ClusterRegistryEventArgs>)Delegate.Combine(_003Cbacking_store_003ERegistryChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ERegistryChanged = (EventHandler<ClusterRegistryEventArgs>)Delegate.Remove(_003Cbacking_store_003ERegistryChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterQuorumEventArgs> QuorumChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EQuorumChanged = (EventHandler<ClusterQuorumEventArgs>)Delegate.Combine(_003Cbacking_store_003EQuorumChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EQuorumChanged = (EventHandler<ClusterQuorumEventArgs>)Delegate.Remove(_003Cbacking_store_003EQuorumChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterConnectionEventArgs> ConnectionChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EConnectionChanged = (EventHandler<ClusterConnectionEventArgs>)Delegate.Combine(_003Cbacking_store_003EConnectionChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EConnectionChanged = (EventHandler<ClusterConnectionEventArgs>)Delegate.Remove(_003Cbacking_store_003EConnectionChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterObjectEventArgs> NodesChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ENodesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Combine(_003Cbacking_store_003ENodesChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ENodesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Remove(_003Cbacking_store_003ENodesChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterObjectEventArgs> ResourceTypesChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EResourceTypesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Combine(_003Cbacking_store_003EResourceTypesChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EResourceTypesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Remove(_003Cbacking_store_003EResourceTypesChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterObjectEventArgs> ResourcesChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EResourcesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Combine(_003Cbacking_store_003EResourcesChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EResourcesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Remove(_003Cbacking_store_003EResourcesChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterObjectEventArgs> NetworkInterfacesChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ENetworkInterfacesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Combine(_003Cbacking_store_003ENetworkInterfacesChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ENetworkInterfacesChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Remove(_003Cbacking_store_003ENetworkInterfacesChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterObjectEventArgs> NetworksChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003ENetworksChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Combine(_003Cbacking_store_003ENetworksChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003ENetworksChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Remove(_003Cbacking_store_003ENetworksChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterGroupEventArgs> GroupStateChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EGroupStateChanged = (EventHandler<ClusterGroupEventArgs>)Delegate.Combine(_003Cbacking_store_003EGroupStateChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EGroupStateChanged = (EventHandler<ClusterGroupEventArgs>)Delegate.Remove(_003Cbacking_store_003EGroupStateChanged, value);
		}
	}

	[SpecialName]
	public event EventHandler<ClusterObjectEventArgs> GroupsChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EGroupsChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Combine(_003Cbacking_store_003EGroupsChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EGroupsChanged = (EventHandler<ClusterObjectEventArgs>)Delegate.Remove(_003Cbacking_store_003EGroupsChanged, value);
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

	private void Construct(SafeClusterHandle hCluster, ClusterAccessRights grantedAccess)
	{
		m_name = null;
		m_domain = null;
		m_hCluster = hCluster;
		m_hClusterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		m_objectManager = new ObjectManager(this);
		m_lifetimeHelper = new ObjectLifetimeHelper();
		m_id = Guid.Empty;
		m_closeLockObject = new object();
		m_isClosed = false;
		m_nodes = null;
		m_storageOnlyNodes = null;
		m_groups = null;
		m_resources = null;
		m_resourceTypes = null;
		m_networks = null;
		m_nodesLock = new object();
		m_storageOnlyNodeLock = new object();
		m_groupsLock = new object();
		m_resourcesLock = new object();
		m_resourceTypesLock = new object();
		m_networksLock = new object();
		m_clusterSharedVolumesStorageLock = new object();
		m_groupsCreateObjectLock = new ReaderWriterLock();
		m_clusterSharedVolumeResources = null;
		m_clusterSharedVolumeResourcesLock = new ReaderWriterLock();
		m_coreClusterGroup = null;
		m_availableStorageGroup = null;
		m_coreClusterNetName = null;
		m_majorVersion = 0;
		m_minorVersion = 0;
		m_buildNumber = 0;
		m_curVersion = ClusterVersion.Unsupported;
		m_apiAccessLevel = grantedAccess;
		if (ClusterItem.CachingDisabled)
		{
			m_notificationManager = null;
			return;
		}
		m_notificationManager = new NotificationManager(this);
		if (DebugLog.PrivateComponentsEnabled)
		{
			m_notificationManager.RegisterMonitor(OnNotifications);
		}
		if (!IsClusterConstructed())
		{
			m_notificationManager.UnregisterMonitor();
			((IDisposable)m_notificationManager)?.Dispose();
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.Cluster_NotConstructed_Text,
				m_name
			});
		}
		LoadClusterInfo();
		ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
		ThreadPool.QueueUserWorkItem(InitClusterSharedVolumeResourcesCache, manualResetEvent);
		manualResetEvent.WaitOne(-1);
	}

	private void LockAccessToHandle([MarshalAs(UnmanagedType.U1)] bool writeAccess)
	{
		if (writeAccess)
		{
			m_hClusterLock.EnterWriteLock();
			return;
		}
		m_hClusterLock.EnterReadLock();
		if (m_hCluster == null)
		{
			m_hClusterLock.ExitReadLock();
			ClusApiExceptionFactory.ThrowObjectDeletedException();
		}
	}

	private void UnlockAccessToHandle([MarshalAs(UnmanagedType.U1)] bool writeAccess)
	{
		if (writeAccess)
		{
			m_hClusterLock.ExitWriteLock();
		}
		else
		{
			m_hClusterLock.ExitReadLock();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsClusterConstructed()
	{
		//Discarded unreachable code: IL_0045
		try
		{
			if (null == GetCoreClusterGroup())
			{
				return false;
			}
			if (null == GetAvailableStorageGroup())
			{
				return false;
			}
		}
		catch (Exception caughtException)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null && (firstException.NativeErrorCode == -2147019883 || firstException.NativeErrorCode == -2147023728))
			{
				return false;
			}
			throw;
		}
		return true;
	}

	private unsafe void LoadClusterInfo()
	{
		//IL_0013: Expected I, but got I8
		//IL_0034: Expected I, but got I8
		//IL_0042: Expected I4, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		_HCLUSTER* ptr = null;
		LockAccessToHandle(writeAccess: false);
		try
		{
			ptr = Handle;
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
		uint num = 256u;
		ushort* ptr2 = null;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSTERVERSIONINFO cLUSTERVERSIONINFO);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref cLUSTERVERSIONINFO, 0, 284);
		*(int*)(&cLUSTERVERSIONINFO) = 284;
		ptr2 = InteropHelp.AllocateWCharArray(256u);
		try
		{
			uint clusterInformation = global::_003CModule_003E.GetClusterInformation(ptr, ptr2, &num, &cLUSTERVERSIONINFO);
			if (clusterInformation == 234)
			{
				num++;
				InteropHelp.ReallocateWCharArray(&ptr2, num);
				clusterInformation = global::_003CModule_003E.GetClusterInformation(ptr, ptr2, &num, &cLUSTERVERSIONINFO);
			}
			if (clusterInformation != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)clusterInformation, Resources.GetClusterInformationFail_Text, m_name);
			}
			m_majorVersion = System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 4));
			m_minorVersion = System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 6));
			m_buildNumber = System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 8));
			m_vendorId = InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 10)));
			m_servicePack = InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 138)));
			m_name = InteropHelp.WstrToString(ptr2);
			m_clusterHighestVersion = System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 268));
			m_clusterLowestVersion = System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 272));
			int mixedMode = ((System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 276)) == 1) ? 1 : 0);
			m_mixedMode = (byte)mixedMode != 0;
			uint num2 = (uint)System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 272)) >> 16;
			if (num2 >= 9)
			{
				m_clusterFunctionalLevel = num2;
				m_clusterUpgradeVersion = System.Runtime.CompilerServices.Unsafe.As<CLUSTERVERSIONINFO, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSTERVERSIONINFO, 272)) & 0xFFFFu;
			}
			else
			{
				m_clusterFunctionalLevel = 0u;
				m_clusterUpgradeVersion = 0u;
			}
			object value = GetRegistryKey(RegistryRights.ExecuteKey).GetValue("ClusterInstanceID");
			Guid id = new Guid((string)value);
			m_id = id;
		}
		finally
		{
			InteropHelp.FreeArray(ptr2);
		}
	}

	private void InitClusterSharedVolumeResourcesCache(object param)
	{
		//Discarded unreachable code: IL_011e, IL_0122
		ManualResetEvent manualResetEvent = null;
		SafeResourceHandle safeResourceHandle = null;
		ResourceControlExecutor resourceControlExecutor = null;
		Exception ex = null;
		ClusterResourceCollection clusterResourceCollection = null;
		Exception ex2 = null;
		try
		{
			m_clusterSharedVolumeResourcesLock.AcquireWriterLock(-1);
			try
			{
				m_clusterSharedVolumeResources = new Dictionary<Guid, ClusterResource>();
				if (param != null)
				{
					manualResetEvent = (ManualResetEvent)param;
					manualResetEvent.Set();
				}
				ClusterResourceCollection allStorage = GetAllStorage(includePoolResource: false);
				SafeClusterEnumHandle safeClusterEnumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.ClusterFileSystem, SafeClusterEnumHandleOptions.None);
				try
				{
					while (safeClusterEnumHandle.MoveNext())
					{
						ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeClusterEnumHandle.Current;
						if (clusterEnumItem == null)
						{
							continue;
						}
						if (clusterEnumItem.ID == Guid.Empty)
						{
							safeResourceHandle = NativeMethods.OpenClusterResource(this, clusterEnumItem.Name);
							try
							{
								resourceControlExecutor = new ResourceControlExecutor(safeResourceHandle, this);
								ResourceControlExecutor resourceControlExecutor2 = resourceControlExecutor;
								Guid id = resourceControlExecutor2.GetId(resourceControlExecutor2);
								clusterEnumItem.ID = id;
							}
							finally
							{
								safeResourceHandle?.Close();
							}
						}
						m_clusterSharedVolumeResources.Add(clusterEnumItem.ID, GetResource(clusterEnumItem.Name, clusterEnumItem.ID));
					}
				}
				finally
				{
					safeClusterEnumHandle?.Close();
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.GetClusterSharedVolumesStorageFailedFormat_Text,
					m_name
				});
			}
			finally
			{
				m_clusterSharedVolumeResourcesLock.ReleaseWriterLock();
			}
			clusterResourceCollection = GetAllStorage(includePoolResource: false);
		}
		catch (Exception exception)
		{
			DebugLog.LogException(exception, "Error to initialize CSV resources cache");
		}
		finally
		{
			if (param != null)
			{
				((ManualResetEvent)param).Set();
			}
		}
	}

	private unsafe void SetQuorum(_HRESOURCE* handle, string deviceName, uint maxLogSize)
	{
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		try
		{
			if (deviceName != null)
			{
				ptr = InteropHelp.StringToWstr(deviceName);
			}
			uint num = global::_003CModule_003E.SetClusterQuorumResource(handle, ptr, maxLogSize);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)num, Resources.SetQuorumResourceFail_Text, m_name);
			}
			m_quorumSettings = null;
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	private QuorumSettings BuildQuorumSettings(ClusterResource quorumResource, uint quorumSize)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (quorumResource == null)
		{
			return new MajorityQuorumSettings(this);
		}
		if (quorumResource.IsStorage)
		{
			if (quorumSize == 1024)
			{
				return StorageQuorumSettings.CreateStorageWitnessQuorumSettings(quorumResource);
			}
			return StorageQuorumSettings.CreateLegacyDiskQuorumSettings(quorumResource);
		}
		if (quorumSize == 1024)
		{
			string resourceType = "File Share Witness";
			if (quorumResource.IsResourceOfType(resourceType))
			{
				return new FileShareQuorumSettings(quorumResource);
			}
			string resourceType2 = "Cloud Witness";
			if (quorumResource.IsResourceOfType(resourceType2))
			{
				return new AzureWitnessQuorumSettings(quorumResource);
			}
		}
		return new UnknownQuorumSettings(quorumResource);
	}

	private long GetItemCount(ClusterEnumType enumType, SafeClusterEnumHandleOptions options)
	{
		m_lifetimeHelper.CheckObjectState();
		using SafeClusterEnumHandle safeClusterEnumHandle = NativeMethods.ClusterOpenEnum(this, enumType, options);
		return safeClusterEnumHandle.GetCount();
	}

	private long GetItemCount(ClusterEnumType enumType)
	{
		m_lifetimeHelper.CheckObjectState();
		using SafeClusterEnumHandle safeClusterEnumHandle = NativeMethods.ClusterOpenEnum(this, enumType, SafeClusterEnumHandleOptions.None);
		return safeClusterEnumHandle.GetCount();
	}

	private unsafe QuorumSettings InternalGetQuorumSettings()
	{
		//Discarded unreachable code: IL_0105, IL_0107
		//IL_0016: Expected I, but got I8
		//IL_001a: Expected I, but got I8
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		ushort* ptr2 = null;
		uint num = 128u;
		uint num2 = 512u;
		ClusterResource quorumResource = null;
		string text = null;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint quorumSize);
		try
		{
			ptr = InteropHelp.AllocateWCharArray(num);
			ptr2 = InteropHelp.AllocateWCharArray(num2);
			uint clusterQuorumResource = global::_003CModule_003E.GetClusterQuorumResource(Handle, ptr, &num, ptr2, &num2, &quorumSize);
			if (clusterQuorumResource == 234)
			{
				num++;
				InteropHelp.ReallocateWCharArray(&ptr, num);
				num2++;
				InteropHelp.ReallocateWCharArray(&ptr2, num2);
				clusterQuorumResource = global::_003CModule_003E.GetClusterQuorumResource(Handle, ptr, &num, ptr2, &num2, &quorumSize);
			}
			if (clusterQuorumResource != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)clusterQuorumResource, Resources.GetQuorumResourceFail_Text, m_name);
			}
			text = InteropHelp.WstrToString(ptr);
		}
		finally
		{
			InteropHelp.FreeArray(ptr);
			InteropHelp.FreeArray(ptr2);
		}
		try
		{
			if (text.Length != 0)
			{
				quorumResource = GetResource(text, Guid.Empty, null);
			}
			return BuildQuorumSettings(quorumResource, quorumSize);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.GetQuorumData_Failed_Text,
				m_name
			});
		}
	}

	private unsafe void RenameCluster(string newName)
	{
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		try
		{
			if (newName == null)
			{
				throw new ArgumentNullException("newName");
			}
			ptr = InteropHelp.StringToWstr(newName);
			uint num = global::_003CModule_003E.SetClusterName(Handle, ptr);
			if (num != 5024 && num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)num, Resources.SetClusterNameFail_Text, m_name);
			}
			m_name = newName;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	private unsafe void GetDomainInfo()
	{
		//Discarded unreachable code: IL_007f, IL_0081
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		UnmanagedBuffer unmanagedBuffer = null;
		try
		{
			unmanagedBuffer = GetControlExecutor().ExecuteOutControl(117440573u);
			string text = InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
			int num = text.IndexOf(".", StringComparison.OrdinalIgnoreCase);
			if (num <= 0)
			{
				m_domain = "";
			}
			else
			{
				m_domain = text.Substring(num + 1);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetDomain_Failed_Text,
				m_name
			});
		}
		finally
		{
			unmanagedBuffer?.Free();
		}
	}

	private unsafe ClusterResourceCollection ProcessDiskCollection(ClusterResourceCollection possibleResources)
	{
		//IL_0009: Expected I8, but got I
		//IL_0074: Expected I, but got I8
		//IL_0074: Expected I, but got I8
		//IL_0085: Expected I, but got I8
		//IL_00c3: Expected I, but got I8
		long num = (nint)stackalloc byte[global::_003CModule_003E.__CxxQueryExceptionSize()];
		ClusterResourceCollection clusterResourceCollection = new ClusterResourceCollection();
		try
		{
			IEnumerator<ClusterResource> enumerator = possibleResources.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					if (current.IsStorage)
					{
						clusterResourceCollection.InternalAdd(current);
						continue;
					}
					ClusterResource clusterResource = current;
					IDisposable disposable = current;
					((IDisposable)current)?.Dispose();
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable2 = enumerator;
				enumerator?.Dispose();
			}
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
					if (possibleResources is IDisposable disposable3)
					{
						disposable3.Dispose();
					}
					global::_003CModule_003E._CxxThrowException(null, null);
					goto end_IL_0086;
				}
				catch when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					num2 = (uint)global::_003CModule_003E.__CxxDetectRethrow((void*)Marshal.GetExceptionPointers());
					return (byte)num2 != 0;
				}).Invoke())
				{
				}
				if (num2 != 0)
				{
					throw;
				}
				end_IL_0086:;
			}
			finally
			{
				global::_003CModule_003E.__CxxUnregisterExceptionObject((void*)num, (int)num2);
			}
		}
		return clusterResourceCollection;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool MarkAsClosed()
	{
		bool result = false;
		Monitor.Enter(m_closeLockObject);
		try
		{
			if (!m_isClosed)
			{
				m_isClosed = true;
				result = true;
			}
		}
		finally
		{
			Monitor.Exit(m_closeLockObject);
		}
		return result;
	}

	private static object StartClusterNode(object inputData, string nodeName)
	{
		try
		{
			if (!(inputData is string[] serviceArgs))
			{
				ClusterNode.StartClusterService(nodeName);
			}
			else
			{
				ClusterNode.StartClusterService(nodeName, serviceArgs);
			}
			if (ClusterNode.WaitForStartingClusterService(nodeName, 10000))
			{
				return new object();
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool GetSharedVolumeEnabledState()
	{
		Property property = null;
		if (CurrentVersion > ClusterVersion.Windows7)
		{
			return true;
		}
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
		property = null;
		int num = ((commonProperties.TryGetProperty("EnableSharedVolumes", out property) && (uint)property.Value == 1) ? 1 : 0);
		return (byte)num != 0;
	}

	private void OnNotifications(object sender, NotificationEventArgs e)
	{
		try
		{
			raise_Notifications(this, e);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "An exception occurred calling Cluster::OnNotifications()");
		}
	}

	private AsyncEnumeration<ClusterNode> BuildNodeAsyncEnum()
	{
		SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.Node, SafeClusterEnumHandleOptions.None);
		return new AsyncEnumeration<ClusterNode>(GetNode, enumHandle);
	}

	private void OnNodeAsyncEnumCompleted(object sender, EventArgs e)
	{
		AsyncEnumeration<ClusterNode> asyncEnumeration = (AsyncEnumeration<ClusterNode>)sender;
		if (asyncEnumeration.EnumeratedItems == null)
		{
			return;
		}
		ClusterNodeCollection clusterNodeCollection = new ClusterNodeCollection();
		foreach (ClusterNode enumeratedItem in asyncEnumeration.EnumeratedItems)
		{
			clusterNodeCollection.InternalAdd(enumeratedItem);
		}
		m_nodes = clusterNodeCollection;
	}

	private AsyncEnumeration<ClusterGroup> BuildGroupAsyncEnum()
	{
		SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.Group, SafeClusterEnumHandleOptions.NoCoreGroups);
		return new AsyncEnumeration<ClusterGroup>(GetGroup, enumHandle);
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

	private AsyncEnumeration<ClusterResource> BuildResourceAsyncEnum()
	{
		SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.Resource, SafeClusterEnumHandleOptions.None);
		return new AsyncEnumeration<ClusterResource>(GetResource, enumHandle);
	}

	private void OnResourceAsyncEnumCompleted(object sender, EventArgs e)
	{
		AsyncEnumeration<ClusterResource> asyncEnumeration = (AsyncEnumeration<ClusterResource>)sender;
		if (asyncEnumeration.EnumeratedItems == null)
		{
			return;
		}
		ClusterResourceCollection clusterResourceCollection = new ClusterResourceCollection();
		foreach (ClusterResource enumeratedItem in asyncEnumeration.EnumeratedItems)
		{
			clusterResourceCollection.InternalAdd(enumeratedItem);
		}
		m_resources = clusterResourceCollection;
	}

	private AsyncEnumeration<ClusterResourceType> BuildResourceTypeAsyncEnum()
	{
		SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.ResourceType, SafeClusterEnumHandleOptions.None);
		return new AsyncEnumeration<ClusterResourceType>(GetResourceType, enumHandle);
	}

	private void OnResourceTypeAsyncEnumCompleted(object sender, EventArgs e)
	{
		AsyncEnumeration<ClusterResourceType> asyncEnumeration = (AsyncEnumeration<ClusterResourceType>)sender;
		if (asyncEnumeration.EnumeratedItems == null)
		{
			return;
		}
		ClusterResourceTypeCollection clusterResourceTypeCollection = new ClusterResourceTypeCollection();
		foreach (ClusterResourceType enumeratedItem in asyncEnumeration.EnumeratedItems)
		{
			clusterResourceTypeCollection.InternalAdd(enumeratedItem);
		}
		m_resourceTypes = clusterResourceTypeCollection;
	}

	private AsyncEnumeration<ClusterNetwork> BuildNetworkAsyncEnum()
	{
		SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.Network, SafeClusterEnumHandleOptions.None);
		return new AsyncEnumeration<ClusterNetwork>(GetNetwork, enumHandle);
	}

	private void OnNetworkAsyncEnumCompleted(object sender, EventArgs e)
	{
		AsyncEnumeration<ClusterNetwork> asyncEnumeration = (AsyncEnumeration<ClusterNetwork>)sender;
		if (asyncEnumeration.EnumeratedItems == null)
		{
			return;
		}
		ClusterNetworkCollection clusterNetworkCollection = new ClusterNetworkCollection();
		foreach (ClusterNetwork enumeratedItem in asyncEnumeration.EnumeratedItems)
		{
			clusterNetworkCollection.InternalAdd(enumeratedItem);
		}
		m_networks = clusterNetworkCollection;
	}

	private AsyncEnumeration<ClusterResource> BuildClusterSharedVolumesResourceAsyncEnum()
	{
		SafeClusterEnumHandle enumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.ClusterFileSystem, SafeClusterEnumHandleOptions.None);
		return new AsyncEnumeration<ClusterResource>(GetClusterSharedVolumeResource, enumHandle);
	}

	private unsafe ResourceCharacteristics GetResourceCharacteristics(string resourceName, Guid id)
	{
		//IL_0025: Expected I4, but got I8
		string resourceName2 = resourceName;
		if (id != Guid.Empty)
		{
			resourceName2 = id.ToString();
		}
		SafeResourceHandle resourceHandle = NativeMethods.OpenClusterResource(this, resourceName2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUS_RESOURCE_CLASS_INFO cLUS_RESOURCE_CLASS_INFO);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlock(ref cLUS_RESOURCE_CLASS_INFO, 0, 8);
		UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&cLUS_RESOURCE_CLASS_INFO, 8uL);
		ResourceControlExecutor resourceControlExecutor = new ResourceControlExecutor(resourceHandle, this);
		resourceControlExecutor.ExecuteOutControl(16777229u, outputBuffer);
		int resourceClass = *(int*)(&cLUS_RESOURCE_CLASS_INFO);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint characteristics);
		UnmanagedBuffer outputBuffer2 = new UnmanagedBuffer(&characteristics, 4uL);
		resourceControlExecutor.ExecuteOutControl(resourceControlExecutor.GetCharacteristicsCode, outputBuffer2);
		return new ResourceCharacteristics((ClusterResourceClass)resourceClass, characteristics);
	}

	public unsafe Cluster(IntPtr handle)
	{
		try
		{
			proxyCluster = true;
			proxyHandle = (_HCLUSTER*)handle.ToPointer();
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(A_0: true);
			throw;
		}
	}

	private Cluster(SafeClusterHandle hCluster, ClusterAccessRights grantedAccess)
	{
		try
		{
			Construct(hCluster, grantedAccess);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(A_0: true);
			throw;
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool DoesResourceDependOnClusterSharedVolume(ClusterResource resource, string sharedVolumeId)
	{
		foreach (string sharedVolumeDependencyId in ClusterResource.GetSharedVolumeDependencyIds(resource))
		{
			if (string.Compare(sharedVolumeDependencyId, sharedVolumeId, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
		}
		return false;
	}

	private void InternalSuspendClusterSharedVolume(ClusterResource sharedVolume, IList<ClusterGroup> groups, ClusterSharedVolumeInfo sharedVolumeInfo)
	{
		try
		{
			TakeGroupsThatDependOnClusterSharedVolumeOffline(sharedVolume, groups);
		}
		catch (Exception caughtException)
		{
			ClusterLog.AdminEvents.WriteCsvTakingDependentGroupsOfflineEvent(sharedVolume.DisplayName);
			ExceptionHelp.LogException(caughtException, "There was an error taking the groups offline when suspending the cluster shared volume '{0}'", sharedVolume.DisplayName);
		}
		sharedVolumeInfo.EnableMaintenance(sharedVolume);
	}

	private void InternalResumeClusterSharedVolume(ClusterResource sharedVolume, ClusterSharedVolumeInfo sharedVolumeInfo)
	{
		sharedVolumeInfo.DisableMaintenance(sharedVolume);
		if (sharedVolume.State != ResourceState.Online)
		{
			sharedVolume.BringOnline();
			if (sharedVolume.State != ResourceState.Online)
			{
				throw ExceptionHelp.Build<ClusterSharedVolumeNotOnlineException>(new string[1] { sharedVolume.DisplayName });
			}
		}
	}

	private void OnClusterSharedVolumeAdded(ClusterResource resource)
	{
		try
		{
			ClusterResourceEventArgs value = new ClusterResourceEventArgs(resource.DisplayName, resource);
			raise_ClusterSharedVolumeAdded(this, value);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event OnClusterSharedVolumeAdded");
		}
	}

	private void OnClusterSharedVolumeRemoved(ClusterResource resource)
	{
		try
		{
			ClusterResourceEventArgs value = new ClusterResourceEventArgs(resource.DisplayName, resource);
			raise_ClusterSharedVolumeRemoved(this, value);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event OnClusterSharedVolumeRemoved");
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool GetDynamicQuorumEnabled()
	{
		Property property = null;
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
		property = null;
		int num = ((commonProperties.TryGetProperty("DynamicQuorumEnabled", out property) && (uint)property.Value == 1) ? 1 : 0);
		return (byte)num != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool GetWitnessDynamicWeight()
	{
		Property property = null;
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
		property = null;
		int num = ((commonProperties.TryGetProperty("WitnessDynamicWeight", out property) && (uint)property.Value != 0) ? 1 : 0);
		return (byte)num != 0;
	}

	private unsafe static _HCLUSTER* OpenClusterInternal(string name)
	{
		//IL_0003: Expected I, but got I8
		_HCLUSTER* result = null;
		if (NetworkHelper.CanPing(name))
		{
			ushort* ptr = InteropHelp.StringToWstr(name);
			try
			{
				result = global::_003CModule_003E.OpenCluster(ptr);
			}
			finally
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
		return result;
	}

	private unsafe static _HNODE* OpenClusterNodeInternal(_HCLUSTER* hCluster, string nodeName)
	{
		//IL_0007: Expected I, but got I8
		string text = null;
		string text2 = null;
		_HNODE* result = null;
		if (NetworkHelper.CanPing(nodeName))
		{
			text = null;
			text2 = null;
			if (!ClusterNode.TryParseFQDN(nodeName, ref text, ref text2))
			{
				text = nodeName;
			}
			ushort* ptr = InteropHelp.StringToWstr(text);
			try
			{
				result = global::_003CModule_003E.OpenClusterNode(hCluster, ptr);
			}
			finally
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsClusterSharedVolumeResource(ClusterResource resource)
	{
		int num2;
		if (resource != null)
		{
			if (ClusterItem.CachingDisabled)
			{
				int num = ((resource.GetOwnerGroup().GroupType == GroupType.ClusterSharedVolume && (resource.GetCharacteristics() & 0x100) == 0) ? 1 : 0);
				return (byte)num != 0;
			}
			Guid id = resource.Id;
			if (IsClusterSharedVolumeResource(id))
			{
				num2 = 1;
				goto IL_0045;
			}
		}
		num2 = 0;
		goto IL_0045;
		IL_0045:
		return (byte)num2 != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsClusterSharedVolumeResource(Guid id)
	{
		//Discarded unreachable code: IL_004c
		if (ClusterItem.CachingDisabled)
		{
			return ClusterResource.CreateObject(this, id.ToString(), id, null).GetOwnerGroup().GroupType == GroupType.ClusterSharedVolume;
		}
		m_clusterSharedVolumeResourcesLock.AcquireReaderLock(-1);
		try
		{
			return m_clusterSharedVolumeResources.ContainsKey(id);
		}
		finally
		{
			m_clusterSharedVolumeResourcesLock.ReleaseReaderLock();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsMixedMode()
	{
		Property property = null;
		string nodeConnectedTo = NativeMethods.GetNodeConnectedTo(this);
		PropertyCollection commonProperties = GetNode(nodeConnectedTo, Guid.Empty).GetCommonProperties(PropertyCollectionSet.ReadOnly);
		property = null;
		if (!commonProperties.TryGetProperty("NodeHighestVersion", out property) || System.Runtime.CompilerServices.Unsafe.As<uint, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref (uint)property.Value, 2)) == 8)
		{
			foreach (ClusterNode node in GetNodes())
			{
				commonProperties = node.GetCommonProperties(PropertyCollectionSet.ReadOnly);
				if (commonProperties.TryGetProperty("NodeHighestVersion", out property) && (uint)property.Value != 533888)
				{
					return true;
				}
			}
			return false;
		}
		LoadClusterInfo();
		return m_mixedMode;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsS2DEnabled()
	{
		Property property = null;
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
		property = null;
		int num = ((commonProperties.TryGetProperty("S2DEnabled", out property) && (uint)property.Value == 1) ? 1 : 0);
		return (byte)num != 0;
	}

	public ClusterFunctionalLevel GetFunctionalLevel()
	{
		Property property = null;
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
		property = null;
		if (commonProperties.TryGetProperty("ClusterFunctionalLevel", out property))
		{
			return (ClusterFunctionalLevel)(uint)property.Value;
		}
		return ClusterFunctionalLevel.Blue;
	}

	public unsafe void UpdateFunctionalLevel([MarshalAs(UnmanagedType.U1)] bool whatIf)
	{
		//IL_0015: Expected I, but got I8
		//IL_0015: Expected I, but got I8
		int num = ((!whatIf) ? 1 : 0);
		uint num2 = global::_003CModule_003E.ClusterUpgradeFunctionalLevel(Handle, num, null, null);
		if (num2 != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(this, (int)num2, Resources.UpdateFunctionalLevel_Fail_Text);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsUpdateClusterFunctionalLevelPossible()
	{
		//Discarded unreachable code: IL_0073
		ControlExecutor controlExecutor = GetControlExecutor();
		try
		{
			controlExecutor.ExecuteControl(117440725u);
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null)
			{
				if (firstException.NativeErrorCode == -2147024895)
				{
					return false;
				}
				if (firstException.NativeErrorCode == -2147018923 || firstException.NativeErrorCode == -2147018922)
				{
					return false;
				}
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Cluster_ShutdownFailed_Text,
				m_name
			});
		}
		return true;
	}

	public unsafe string CreateInfrastructureScaleOutFileServer(string fileServerName)
	{
		//Discarded unreachable code: IL_009d, IL_009f
		//IL_000b: Expected I, but got I8
		//IL_0020: Expected I, but got I8
		//IL_005f: Expected I4, but got I8
		ControlExecutor controlExecutor = GetControlExecutor();
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(fileServerName);
			ushort* ptr2 = ptr;
			while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0)
			{
				ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
			}
			ulong num = (ulong)((nint)((byte*)ptr2 - (nuint)ptr) >> 1) * 2uL;
			if (num >= 32)
			{
				throw new ArgumentException(ExceptionHelp.FormatArgs(Resources.InvalidNetName_Failed_Too_Long_Text, fileServerName));
			}
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUS_CREATE_INFRASTRUCTURE_FILESERVER_INPUT cLUS_CREATE_INFRASTRUCTURE_FILESERVER_INPUT);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref cLUS_CREATE_INFRASTRUCTURE_FILESERVER_INPUT, ptr, num + 2);
			System.Runtime.CompilerServices.Unsafe.As<_CLUS_CREATE_INFRASTRUCTURE_FILESERVER_INPUT, short>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUS_CREATE_INFRASTRUCTURE_FILESERVER_INPUT, 30)) = 48;
			UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&cLUS_CREATE_INFRASTRUCTURE_FILESERVER_INPUT, 32uL);
			controlExecutor.ExecuteInControl(121644018u, inputBuffer);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.CreateGroup_Failed_Text,
				fileServerName
			});
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return "Infrastructure File Server";
	}

	public void RemoveInfrastructureScaleOutFileServer()
	{
		//Discarded unreachable code: IL_0032
		ControlExecutor controlExecutor = GetControlExecutor();
		try
		{
			controlExecutor.ExecuteControl(121644022u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_Delete_Fail_Text,
				"Infrastructure Scale Out File Server"
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsLocalHostAClusterNode()
	{
		ApplicationException ex = null;
		bool result = false;
		try
		{
			string machineName = Environment.MachineName;
			GetNode(machineName, Guid.Empty);
			result = true;
		}
		catch (ApplicationException caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Localhost '{0}' is not one of the cluster nodes.", Environment.MachineName);
		}
		return result;
	}

	public void RefreshSharedVolumeResources()
	{
		InitClusterSharedVolumeResourcesCache(null);
	}

	public IEnumerable<Tuple<Guid, string>> GetResourceGuidAndName([MarshalAs(UnmanagedType.U1)] bool unique)
	{
		//Discarded unreachable code: IL_0105, IL_0130, IL_0157, IL_0159
		SafeResourceHandle safeResourceHandle = null;
		ResourceControlExecutor resourceControlExecutor = null;
		ApplicationException ex = null;
		Win32Exception ex2 = null;
		ClusterBaseException ex3 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeClusterEnumHandle safeClusterEnumHandle = null;
		List<Tuple<Guid, string>> list = new List<Tuple<Guid, string>>();
		try
		{
			safeClusterEnumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.Resource, SafeClusterEnumHandleOptions.None);
			while (safeClusterEnumHandle.MoveNext())
			{
				ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeClusterEnumHandle.Current;
				if (clusterEnumItem == null)
				{
					continue;
				}
				if (clusterEnumItem.ID == Guid.Empty)
				{
					safeResourceHandle = NativeMethods.OpenClusterResource(this, clusterEnumItem.Name);
					try
					{
						resourceControlExecutor = new ResourceControlExecutor(safeResourceHandle, this);
						ResourceControlExecutor resourceControlExecutor2 = resourceControlExecutor;
						Guid id = resourceControlExecutor2.GetId(resourceControlExecutor2);
						clusterEnumItem.ID = id;
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
				list.Add(new Tuple<Guid, string>(item2: (!unique) ? clusterEnumItem.Name : clusterEnumItem.Name.ToLower(CultureInfo.CurrentCulture), item1: clusterEnumItem.ID));
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
			safeClusterEnumHandle?.Close();
		}
	}

	public unsafe IDictionary<ClusterResource, IList<ClusterNode>> GetDiskConnectivity(IList<ClusterResource> disks, IList<ClusterNode> nodes)
	{
		//Discarded unreachable code: IL_02bb, IL_02c9, IL_02d3
		//IL_0145: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		if (disks == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "disks" });
		}
		if (nodes == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "nodes" });
		}
		if (nodes.Count == 0)
		{
			throw ExceptionHelp.Build<ArgumentException>(new string[1] { "nodes" });
		}
		ulong num = 0uL;
		Dictionary<int, ClusterNode> dictionary = new Dictionary<int, ClusterNode>();
		foreach (ClusterNode node in nodes)
		{
			int num2 = int.Parse(node.NodeId);
			num2 += -1;
			dictionary.Add(num2, node);
			num |= (ulong)(1L << num2);
		}
		StringCollection stringCollection = new StringCollection();
		foreach (ClusterResource disk in disks)
		{
			if (disk.GetResourceClass() != ClusterResourceClass.Storage)
			{
				throw ExceptionHelp.Build<ArgumentException>(new string[3]
				{
					"nodes",
					Resources.DiskConnectivity_NotAStorageResource_Format,
					disk.DisplayName
				});
			}
			stringCollection.Add(disk.Id.ToString());
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _HCLUSPROPLIST* ptr);
		try
		{
			ptr = global::_003CModule_003E.CreatePropList(null, 0u);
			if (ptr == null)
			{
				string[] args = new string[0];
				throw ExceptionHelp.Build<Win32Exception>((int)global::_003CModule_003E.GetLastError(), args);
			}
			uint num3 = global::_003CModule_003E.AddBinaryProperty(ptr, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BC_0040LPPLIAJE_0040_003F_0024AAN_003F_0024AAo_003F_0024AAd_003F_0024AAe_003F_0024AAM_003F_0024AAa_003F_0024AAs_003F_0024AAk_0040), (byte*)(&num), 8u);
			if (num3 != 0)
			{
				string[] args2 = new string[0];
				throw ExceptionHelp.Build<Win32Exception>((int)num3, args2);
			}
			UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType(stringCollection);
			try
			{
				num3 = global::_003CModule_003E.AddMultiSzProperty(ptr, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BG_0040BCIBIFJD_0040_003F_0024AAR_003F_0024AAe_003F_0024AAs_003F_0024AAo_003F_0024AAu_003F_0024AAr_003F_0024AAc_003F_0024AAe_003F_0024AAI_003F_0024AAd_0040), (ushort*)unmanagedBuffer.Pointer);
			}
			finally
			{
				unmanagedBuffer.Free();
			}
			if (num3 != 0)
			{
				string[] args3 = new string[0];
				throw ExceptionHelp.Build<Win32Exception>((int)num3, args3);
			}
			uint num4 = global::_003CModule_003E.AddDwordProperty(ptr, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1M_0040OAJFFPML_0040_003F_0024AAF_003F_0024AAl_003F_0024AAa_003F_0024AAg_003F_0024AAs_0040), 2u);
			if (num4 != 0)
			{
				string[] args4 = new string[0];
				throw ExceptionHelp.Build<Win32Exception>((int)num4, args4);
			}
			ulong num5 = (ulong)disks.Count;
			ulong* ptr2 = (ulong*)global::_003CModule_003E.new_005B_005D((num5 > 2305843009213693951L) ? ulong.MaxValue : (num5 * 8));
			ulong* ptr3 = ptr2;
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_LIST* pMem);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num6);
				global::_003CModule_003E.GetPropertyListBuffer(ptr, &pMem, &num6);
				UnmanagedBuffer unmanagedBuffer2 = new UnmanagedBuffer(pMem, num6);
				UnmanagedBuffer outputBuffer = new UnmanagedBuffer(ptr2, (ulong)disks.Count * 8uL);
				try
				{
					GetControlExecutor().ExecuteInOutControl(117448737u, unmanagedBuffer2, outputBuffer);
					IDictionary<ClusterResource, IList<ClusterNode>> dictionary2 = new Dictionary<ClusterResource, IList<ClusterNode>>();
					for (int i = 0; i < disks.Count; i++)
					{
						List<ClusterNode> list = new List<ClusterNode>();
						ulong num7 = *(ulong*)((long)i * 8L + (nint)ptr3);
						for (int j = 0; j < 64; j++)
						{
							if (((num7 >> j) & 1) != 0L)
							{
								list.Add(dictionary[j]);
							}
						}
						dictionary2.Add(disks[i], list);
					}
					return dictionary2;
				}
				finally
				{
					unmanagedBuffer2.Free();
				}
			}
			finally
			{
				ulong* ptr4 = ptr3;
				global::_003CModule_003E.delete_005B_005D(ptr3);
			}
		}
		finally
		{
			global::_003CModule_003E.DestroyPropList(ptr);
		}
	}

	public static ClusterVersion ConvertMajorMinorVersionsToClusterVersion(ushort majorVersion, ushort minorVersion)
	{
		switch (majorVersion)
		{
		case 6:
			switch (minorVersion)
			{
			case 0:
				return ClusterVersion.WindowsServer2008;
			case 1:
				return ClusterVersion.Windows7;
			case 2:
				return ClusterVersion.Windows8;
			case 3:
				return ClusterVersion.WindowsBlue;
			case 4:
				return ClusterVersion.WindowsThreshold;
			}
			break;
		case 10:
			if (minorVersion == 0)
			{
				return ClusterVersion.WindowsThreshold;
			}
			break;
		}
		return ClusterVersion.Unsupported;
	}

	public static ClusterVersion ConvertFunctionalLevelToClusterVersion(uint ClusterFunctionalLevel, uint clusterUpgradeVersion)
	{
		int result;
		switch (ClusterFunctionalLevel)
		{
		default:
			return ClusterVersion.WindowsVb;
		case 0u:
		case 1u:
		case 2u:
		case 3u:
		case 4u:
		case 5u:
		case 6u:
		case 7u:
		case 8u:
		case 9u:
			result = 5;
			break;
		case 10u:
			result = 6;
			break;
		}
		return (ClusterVersion)result;
	}

	public unsafe ClusterStorageOnlyNode GetClusterStorageOnlyNode(string name)
	{
		//IL_0016: Expected I4, but got I8
		_HCLUSTER* handle = Handle;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _ClusStorageEnclosureInfo clusStorageEnclosureInfo);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref clusStorageEnclosureInfo, 0, 136);
		ClusterStorageOnlyNode clusterStorageOnlyNode = null;
		try
		{
			uint num = global::_003CModule_003E.ClusterGetClusterStorageEnclosureObject(handle, InteropHelp.StringToWstr(name), &clusStorageEnclosureInfo, 0u);
			if (num != 0)
			{
				object[] array = new object[2];
				string text = ((!(name != null)) ? string.Empty : name);
				array[0] = text;
				array[1] = num;
				DebugLog.LogError("There was an error querying for cluster storage enclosure '{0}'. The error is '{1}'.", array);
				ClusApiExceptionFactory.CreateAndThrow(this, (int)num);
			}
			return new ClusterStorageOnlyNode(this, &clusStorageEnclosureInfo);
		}
		finally
		{
			global::_003CModule_003E.FreeClusStorageEnclosureInfo(&clusStorageEnclosureInfo);
		}
	}

	public ClusterResource GetResource(Guid resourceId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return GetResource(resourceId.ToString(), resourceId, null);
	}

	public ClusterResource GetResource(string resourceName, Guid resourceId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return GetResource(resourceName, resourceId, null);
	}

	public ClusterResource GetResource(string resourceName)
	{
		return GetResource(resourceName, Guid.Empty, null);
	}

	internal ClusterResource GetResource(string resourceName, Guid resourceId, ClusterGroup group)
	{
		//Discarded unreachable code: IL_0041, IL_0043
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterResource.CreateObject(this, resourceName, resourceId, group);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Cluster_GetResource_Failed_Text,
				m_name,
				resourceName
			});
		}
	}

	internal ClusterResource GetResource(string resourceName, ClusterGroup group)
	{
		return GetResource(resourceName, Guid.Empty, group);
	}

	public ClusterNetworkInterface GetNetworkInterface(string networkInterfaceName)
	{
		return GetNetworkInterface(networkInterfaceName, Guid.Empty);
	}

	internal ClusterNetworkInterface GetNetworkInterface(string networkInterfaceName, Guid networkInterfaceId)
	{
		//Discarded unreachable code: IL_0040, IL_0042
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterNetworkInterface.CreateObject(this, networkInterfaceName, networkInterfaceId);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Cluster_GetNetworkInterface_Failed_Text,
				m_name,
				networkInterfaceName
			});
		}
	}

	internal void RefreshResources()
	{
		m_resources = null;
	}

	internal void RefreshGroups()
	{
		m_groups = null;
	}

	internal void RefreshNodes()
	{
		m_nodes = null;
		m_storageOnlyNodes = null;
	}

	internal void RefreshResourceTypes()
	{
		m_resourceTypes = null;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe bool WillVoterLoseQuorum(uint controlCode, string voterId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&num, 4uL);
		UnmanagedBuffer inputBuffer = UnmanagedBuffer.Create(voterId);
		GetControlExecutor().ExecuteInOutControl(controlCode, inputBuffer, outputBuffer);
		return (num != 0) ? true : false;
	}

	internal void TakeGroupsThatDependOnClusterSharedVolumeOffline(ClusterResource sharedVolume, ICollection<ClusterGroup> groups)
	{
		foreach (ClusterGroup group in groups)
		{
			if (group.State == GroupState.Online || group.State == GroupState.PartialOnline)
			{
				try
				{
					group.TakeOffline();
				}
				catch (Exception caughtException)
				{
					ClusterLog.AdminEvents.WriteCsvTakingDependentGroupOfflineEvent(group.Name, sharedVolume.DisplayName);
					ExceptionHelp.LogException(caughtException, "There was an error taking the dependent group '{0}' of cluster shared volume '{1}' offline.", group.Name, sharedVolume.DisplayName);
				}
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
			LoadClusterInfo();
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

	internal void OnGroupsChanged(string name, Guid id, ClusterObjectEventType type)
	{
		Exception ex = null;
		ClusterObjectEventArgs clusterObjectEventArgs = null;
		Exception ex2 = null;
		bool flag;
		try
		{
			m_groups = null;
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_002e;
		}
		goto IL_0032;
		IL_002e:
		if (!flag)
		{
			return;
		}
		goto IL_0032;
		IL_0032:
		try
		{
			clusterObjectEventArgs = new ClusterObjectEventArgs(name, id, type);
			raise_GroupsChanged(this, clusterObjectEventArgs);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event GroupsChanged");
		}
	}

	internal void OnNetworksChanged(string name, Guid id, ClusterObjectEventType type)
	{
		Exception ex = null;
		ClusterObjectEventArgs clusterObjectEventArgs = null;
		Exception ex2 = null;
		bool flag;
		try
		{
			m_networks = null;
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_002e;
		}
		goto IL_0032;
		IL_002e:
		if (!flag)
		{
			return;
		}
		goto IL_0032;
		IL_0032:
		try
		{
			clusterObjectEventArgs = new ClusterObjectEventArgs(name, id, type);
			raise_NetworksChanged(this, clusterObjectEventArgs);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event NetworksChanged");
		}
	}

	internal void OnNetworkInterfacesChanged(string name, Guid id, ClusterObjectEventType type)
	{
		bool flag;
		try
		{
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_001c;
		}
		goto IL_0020;
		IL_001c:
		if (!flag)
		{
			return;
		}
		goto IL_0020;
		IL_0020:
		try
		{
			ClusterObjectEventArgs value = new ClusterObjectEventArgs(name, id, type);
			raise_NetworkInterfacesChanged(this, value);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event NetworkInterfacesChanged");
		}
	}

	internal void OnResourcesChanged(string name, Guid id, ClusterObjectEventType type)
	{
		ResourceCharacteristics resourceCharacteristics = null;
		Exception ex = null;
		ClusterObjectEventArgs clusterObjectEventArgs = null;
		Exception ex2 = null;
		bool flag;
		try
		{
			m_resources = null;
			m_lifetimeHelper.CheckObjectState();
			if (type == ClusterObjectEventType.Added)
			{
				resourceCharacteristics = GetResourceCharacteristics(name, id);
				if (resourceCharacteristics.ResourceClass == ClusterResourceClass.Storage || resourceCharacteristics.ResourceClass == ClusterResourceClass.Network || (resourceCharacteristics.Characteristics & (true ? 1u : 0u)) != 0)
				{
					GetResource(name, id);
				}
			}
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_0062;
		}
		goto IL_0067;
		IL_0062:
		if (!flag)
		{
			return;
		}
		goto IL_0067;
		IL_0067:
		try
		{
			clusterObjectEventArgs = new ClusterObjectEventArgs(name, id, type);
			raise_ResourcesChanged(this, clusterObjectEventArgs);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event ResourcesChanged");
		}
	}

	internal void OnGroupStateChanged(string name)
	{
		bool flag = true;
		ClusterGroup clusterGroup = null;
		try
		{
			clusterGroup = ClusterGroup.CreateObject(this, name);
			m_lifetimeHelper.CheckObjectState();
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
		clusterGroup?.OnStateChanged();
		if (flag)
		{
			try
			{
				ClusterGroupEventArgs value = new ClusterGroupEventArgs(name, clusterGroup);
				raise_GroupStateChanged(this, value);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event GroupsChanged");
			}
		}
	}

	internal void OnResourceTypesChanged(string name, Guid id, ClusterObjectEventType type)
	{
		Exception ex = null;
		ClusterObjectEventArgs clusterObjectEventArgs = null;
		Exception ex2 = null;
		bool flag;
		try
		{
			m_resourceTypes = null;
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_002e;
		}
		goto IL_0032;
		IL_002e:
		if (!flag)
		{
			return;
		}
		goto IL_0032;
		IL_0032:
		try
		{
			clusterObjectEventArgs = new ClusterObjectEventArgs(name, id, type);
			raise_ResourceTypesChanged(this, clusterObjectEventArgs);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event ResourceTypesChanged");
		}
	}

	internal void OnNodesChanged(string name, Guid id, ClusterObjectEventType type)
	{
		Exception ex = null;
		ClusterObjectEventArgs clusterObjectEventArgs = null;
		Exception ex2 = null;
		bool flag;
		try
		{
			m_nodes = null;
			m_storageOnlyNodes = null;
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_0037;
		}
		goto IL_003b;
		IL_0037:
		if (!flag)
		{
			return;
		}
		goto IL_003b;
		IL_003b:
		try
		{
			clusterObjectEventArgs = new ClusterObjectEventArgs(name, id, type);
			raise_NodesChanged(this, clusterObjectEventArgs);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event NodesChanged");
		}
	}

	internal void OnConnectionChanged(ClusterConnectionState state)
	{
		ClusterLog.LogVerbose(LogSubcategory.ClusWinFx, "Cluster on connection changed.");
		bool flag = true;
		if (m_lifetimeHelper == null)
		{
			return;
		}
		try
		{
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_0031;
		}
		goto IL_0034;
		IL_0034:
		if (state == ClusterConnectionState.Disconnected)
		{
			ClusterLog.LogVerbose(LogSubcategory.ClusWinFx, "Cluster disconnected connection marked as closed");
			flag = MarkAsClosed();
		}
		if (flag)
		{
			try
			{
				ClusterConnectionEventArgs value = new ClusterConnectionEventArgs(state);
				ClusterLog.LogVerbose(LogSubcategory.ClusWinFx, "Cluster disconnected, firing event ConnectionChanged(Disconnected)");
				raise_ConnectionChanged(this, value);
				return;
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event ConnectionChanged");
				return;
			}
		}
		return;
		IL_0031:
		if (!flag)
		{
			return;
		}
		goto IL_0034;
	}

	internal void OnQuorumChanged(string data)
	{
		Exception ex = null;
		ClusterQuorumEventArgs clusterQuorumEventArgs = null;
		Exception ex2 = null;
		m_quorumSettings = null;
		bool flag;
		try
		{
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_002e;
		}
		goto IL_0032;
		IL_002e:
		if (!flag)
		{
			return;
		}
		goto IL_0032;
		IL_0032:
		try
		{
			clusterQuorumEventArgs = new ClusterQuorumEventArgs(data);
			raise_QuorumChanged(this, clusterQuorumEventArgs);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event QuorumChanged");
		}
	}

	internal void OnRegistryChanged(string registryName, ClusterRegistryChangeType type)
	{
		bool flag;
		try
		{
			m_lifetimeHelper.CheckObjectState();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_001c;
		}
		goto IL_0020;
		IL_001c:
		if (!flag)
		{
			return;
		}
		goto IL_0020;
		IL_0020:
		try
		{
			ClusterRegistryEventArgs value = new ClusterRegistryEventArgs(registryName, type);
			raise_RegistryChanged(this, value);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event RegistryChanged");
		}
	}

	internal void OnResourcePropertyChanged(string name, ClusterResource resource)
	{
		bool flag = true;
		try
		{
			if (resource != null && resource.IsStorage)
			{
				Guid id = resource.Id;
				Guid guid = id;
				if (resource.GetOwnerGroup().GroupType == GroupType.ClusterSharedVolume)
				{
					bool flag2 = false;
					m_clusterSharedVolumeResourcesLock.AcquireReaderLock(-1);
					try
					{
						if (!m_clusterSharedVolumeResources.ContainsKey(id))
						{
							LockCookie lockCookie = m_clusterSharedVolumeResourcesLock.UpgradeToWriterLock(-1);
							try
							{
								if (!m_clusterSharedVolumeResources.ContainsKey(id))
								{
									m_clusterSharedVolumeResources.Add(id, resource);
									flag2 = true;
								}
							}
							finally
							{
								m_clusterSharedVolumeResourcesLock.DowngradeFromWriterLock(ref lockCookie);
							}
						}
					}
					finally
					{
						m_clusterSharedVolumeResourcesLock.ReleaseReaderLock();
					}
					if (flag2)
					{
						OnClusterSharedVolumeAdded(resource);
					}
				}
				else
				{
					bool flag3 = false;
					m_clusterSharedVolumeResourcesLock.AcquireReaderLock(-1);
					try
					{
						if (m_clusterSharedVolumeResources.ContainsKey(id))
						{
							LockCookie lockCookie2 = m_clusterSharedVolumeResourcesLock.UpgradeToWriterLock(-1);
							try
							{
								m_clusterSharedVolumeResources.Remove(id);
								flag3 = true;
							}
							finally
							{
								m_clusterSharedVolumeResourcesLock.DowngradeFromWriterLock(ref lockCookie2);
							}
						}
					}
					finally
					{
						m_clusterSharedVolumeResourcesLock.ReleaseReaderLock();
					}
					if (flag3)
					{
						OnClusterSharedVolumeRemoved(resource);
					}
				}
			}
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
		if (flag)
		{
			try
			{
				ClusterResourceEventArgs value = new ClusterResourceEventArgs(name, resource);
				raise_ResourcePropertyChanged(this, value);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event OnResourcePropertyChanged");
			}
		}
	}

	internal unsafe string GetClusterNameOU()
	{
		//Discarded unreachable code: IL_008b
		ThreadWatchdog.PerformUIThreadCheck();
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		unmanagedBuffer.Allocate(4096uL);
		try
		{
			string unavailable_Text;
			try
			{
				string resourceTypeName = "Network Name";
				ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(resourceTypeName, this);
				resourceTypeControlExecutor.ExecuteOutControl(37749358u, unmanagedBuffer);
				return InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
			}
			catch (Win32Exception ex)
			{
				int num = -2147024895;
				if (ex.NativeErrorCode != -2147024895)
				{
					ClusterLog.LogException(ex, "An error occurred retrieving the name of the OU for cluster name - {0}.", m_name);
				}
				unavailable_Text = Resources.Unavailable_Text;
			}
			return unavailable_Text;
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private Dictionary<Guid, string> GetGroupNames(Dictionary<Guid, string> groupsIdNamePairs, [MarshalAs(UnmanagedType.U1)] bool unique)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeClusterEnumHandle safeClusterEnumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.Group, SafeClusterEnumHandleOptions.NoCoreGroups);
		try
		{
			while (safeClusterEnumHandle.MoveNext())
			{
				ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeClusterEnumHandle.Current;
				if (clusterEnumItem == null)
				{
					continue;
				}
				if (clusterEnumItem.ID == Guid.Empty)
				{
					SafeGroupHandle safeGroupHandle = NativeMethods.OpenClusterGroup(this, clusterEnumItem.Name);
					try
					{
						GroupControlExecutor groupControlExecutor = new GroupControlExecutor(safeGroupHandle, this);
						Guid id = groupControlExecutor.GetId(groupControlExecutor);
						clusterEnumItem.ID = id;
					}
					finally
					{
						safeGroupHandle?.Close();
					}
				}
				groupsIdNamePairs.Add(value: (!unique) ? clusterEnumItem.Name : clusterEnumItem.Name.ToLower(CultureInfo.CurrentCulture), key: clusterEnumItem.ID);
			}
			return groupsIdNamePairs;
		}
		finally
		{
			safeClusterEnumHandle?.Close();
		}
	}

	private Dictionary<Guid, string> GetResourceNames(Dictionary<Guid, string> resourcesIdNamePairs, [MarshalAs(UnmanagedType.U1)] bool unique)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		foreach (Tuple<Guid, string> item2 in GetResourceGuidAndName(unique))
		{
			Guid item = item2.Item1;
			resourcesIdNamePairs.Add(item, item2.Item2);
		}
		return resourcesIdNamePairs;
	}

	public unsafe void SetMajorityQuorum(ClusterResource resource, string deviceName)
	{
		//Discarded unreachable code: IL_0064
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			if (resource == null)
			{
				ClusterResource quorumResource = GetQuorumResource();
				if (quorumResource != null)
				{
					_HRESOURCE* handle = quorumResource.Handle;
					SetQuorum(handle, null, 0u);
				}
			}
			else
			{
				SetQuorum(resource.Handle, deviceName, 1024u);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.MajorityQuorum_Failed_Text,
				m_name
			});
		}
	}

	public unsafe void SetLegacyQuorum(ClusterResource resource, string deviceName)
	{
		//Discarded unreachable code: IL_0045
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			SetQuorum(resource.Handle, deviceName, 4194304u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.LegacyQuorum_Failed_Text,
				m_name
			});
		}
	}

	[SpecialName]
	protected void raise_PropertiesChanged(object value0, EventArgs value1)
	{
		_003Cbacking_store_003EPropertiesChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_GroupsChanged(object value0, ClusterObjectEventArgs value1)
	{
		_003Cbacking_store_003EGroupsChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_GroupStateChanged(object value0, ClusterGroupEventArgs value1)
	{
		_003Cbacking_store_003EGroupStateChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_NetworksChanged(object value0, ClusterObjectEventArgs value1)
	{
		_003Cbacking_store_003ENetworksChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_NetworkInterfacesChanged(object value0, ClusterObjectEventArgs value1)
	{
		_003Cbacking_store_003ENetworkInterfacesChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_ResourcesChanged(object value0, ClusterObjectEventArgs value1)
	{
		_003Cbacking_store_003EResourcesChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_ResourceTypesChanged(object value0, ClusterObjectEventArgs value1)
	{
		_003Cbacking_store_003EResourceTypesChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_NodesChanged(object value0, ClusterObjectEventArgs value1)
	{
		_003Cbacking_store_003ENodesChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_ConnectionChanged(object value0, ClusterConnectionEventArgs value1)
	{
		_003Cbacking_store_003EConnectionChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_QuorumChanged(object value0, ClusterQuorumEventArgs value1)
	{
		_003Cbacking_store_003EQuorumChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_RegistryChanged(object value0, ClusterRegistryEventArgs value1)
	{
		_003Cbacking_store_003ERegistryChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_ResourcePropertyChanged(object value0, ClusterResourceEventArgs value1)
	{
		_003Cbacking_store_003EResourcePropertyChanged?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_ClusterSharedVolumeAdded(object value0, ClusterResourceEventArgs value1)
	{
		_003Cbacking_store_003EClusterSharedVolumeAdded?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_ClusterSharedVolumeRemoved(object value0, ClusterResourceEventArgs value1)
	{
		_003Cbacking_store_003EClusterSharedVolumeRemoved?.Invoke(value0, value1);
	}

	[SpecialName]
	protected void raise_Notifications(object value0, NotificationEventArgs value1)
	{
		_003Cbacking_store_003ENotifications?.Invoke(value0, value1);
	}

	public unsafe static Cluster Create(string clusterName, StringCollection nodeNames, ICollection<IPAddressInfo> ipAddresses, ClusterActionCallback callback, AdminAccessPoint managementAccessPoint, AdminAccessPointResType CNOResType)
	{
		//Discarded unreachable code: IL_01de
		//IL_000e: Expected I, but got I8
		//IL_0012: Expected I, but got I8
		//IL_0015: Expected I, but got I8
		//IL_00ae: Expected I, but got I8
		//IL_00c8: Expected I8, but got I
		//IL_00ed: Expected I, but got I8
		//IL_0112: Expected I8, but got I
		//IL_0122: Expected I8, but got I
		//IL_0130: Expected I8, but got I
		//IL_0154: Expected I, but got I8
		//IL_01c2: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		uint num = 0u;
		uint num2 = 0u;
		ushort* ptr = null;
		ushort** ptr2 = null;
		_CLUSTER_IP_ENTRY* ptr3 = null;
		try
		{
			if (clusterName == null)
			{
				throw new ArgumentNullException("clusterName");
			}
			ptr = InteropHelp.StringToWstr(clusterName);
			if (nodeNames == null)
			{
				throw new ArgumentNullException("nodeNames");
			}
			if (nodeNames.Count == 0)
			{
				throw new ArgumentException(Resources.NodeNamesArgumentFail_Text, "nodeNames");
			}
			num = (uint)nodeNames.Count;
			InteropHelp.ConvertStringCollectionToPWSTRArray(nodeNames, &ptr2);
			if (ipAddresses != null && ipAddresses.Count > 0)
			{
				num2 = (uint)ipAddresses.Count;
				ptr3 = (_CLUSTER_IP_ENTRY*)global::_003CModule_003E.new_005B_005D((ulong)num2 * 16uL);
				int num3 = 0;
				IEnumerator<IPAddressInfo> enumerator = ipAddresses.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						IPAddressInfo current = enumerator.Current;
						_CLUSTER_IP_ENTRY* ptr4 = (_CLUSTER_IP_ENTRY*)((long)num3 * 16L + (nint)ptr3);
						num3++;
						*(long*)ptr4 = (nint)InteropHelp.StringToWstr(current.Address.ToString());
						*(uint*)((ulong)(nint)ptr4 + 8uL) = current.PrefixLength;
					}
				}
				finally
				{
					IEnumerator<IPAddressInfo> enumerator2 = enumerator;
					IDisposable disposable = enumerator;
					enumerator?.Dispose();
				}
			}
			delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int> delegate_002A = null;
			if (callback != null)
			{
				m_Callback = callback;
				delegate_002A = (delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int>)global::_003CModule_003E.__unep_0040_003FCreateClusterCallbackWrapper_0040CClusterCallbackHelper_0040ServerClusters_0040Internal_0040MS_0040_0040_0024_0024FSAHPEAXW4_CLUSTER_SETUP_PHASE_0040_0040W4_CLUSTER_SETUP_PHASE_TYPE_0040_0040W4_CLUSTER_SETUP_PHASE_SEVERITY_0040_0040KPEBGK_0040Z;
			}
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _CREATE_CLUSTER_CONFIG cREATE_CLUSTER_CONFIG);
			*(int*)(&cREATE_CLUSTER_CONFIG) = 2560;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 8)) = (nint)ptr;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 16)) = num;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 24)) = (nint)ptr2;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 32)) = num2;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 40)) = (nint)ptr3;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 48)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, AdminAccessPoint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 52)) = managementAccessPoint;
			System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_CONFIG, AdminAccessPointResType>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_CONFIG, 56)) = CNOResType;
			IntPtr clusterHandle = new IntPtr(global::_003CModule_003E.CreateCluster(&cREATE_CLUSTER_CONFIG, delegate_002A, null));
			SafeClusterHandle safeClusterHandle = new SafeClusterHandle(clusterHandle);
			if (safeClusterHandle.IsInvalid)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)global::_003CModule_003E.GetLastError(), new string[2]
				{
					Resources.CreateClusterFail_Text,
					clusterName
				});
			}
			return new Cluster(safeClusterHandle, ClusterAccessRights.GenericAll);
		}
		finally
		{
			m_Callback = null;
			InteropHelp.FreeWstr(ptr);
			if (ptr3 != null)
			{
				uint num4 = 0u;
				if (0 < num2)
				{
					do
					{
						InteropHelp.FreeWstr((ushort*)(*(ulong*)((long)num4 * 16L + (nint)ptr3)));
						num4 += 2;
					}
					while (num4 < num2);
				}
			}
			global::_003CModule_003E.delete_005B_005D(ptr3);
			if (ptr2 != null)
			{
				InteropHelp.FreePWSTRArray(ptr2, (int)num);
			}
		}
	}

	public static Cluster Open(string clusterName, ClusterAccessRights desiredAccess, ref ClusterAccessRights grantedAccess)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		grantedAccess = ClusterAccessRights.None;
		return new Cluster(NativeMethods.OpenCluster(clusterName, desiredAccess, ref grantedAccess), grantedAccess);
	}

	public static Cluster Open(string clusterName, ClusterAccessRights desiredAccess)
	{
		ClusterAccessRights clusterAccessRights = desiredAccess;
		ThreadWatchdog.PerformUIThreadCheck();
		clusterAccessRights = ClusterAccessRights.None;
		return new Cluster(NativeMethods.OpenCluster(clusterName, desiredAccess, ref clusterAccessRights), clusterAccessRights);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsClusterName(string clusterName)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		bool result = false;
		try
		{
			NativeMethods.OpenCluster(clusterName).Close();
			result = true;
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static void StartCluster(ICollection<string> nodeNames)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		new ActionMultiplexor<object, string, object>(StartClusterNode).AsyncExecute(null, nodeNames);
	}

	public static string ForceClusterStart(ICollection<string> nodeNames)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		bool flag = false;
		string result = null;
		bool flag2 = true;
		foreach (string nodeName in nodeNames)
		{
			try
			{
				string[] array = new string[1];
				if (flag2)
				{
					array[0] = "-fq";
				}
				else
				{
					array[0] = "-pq";
				}
				flag2 = StartClusterNode(array, nodeName) == null && flag2;
				result = nodeName;
				flag = true;
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Failed to force start service on node '{0}'", nodeName);
				continue;
			}
			break;
		}
		if (!flag)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.UnableToForceClusterStart_Text });
		}
		return result;
	}

	private void _007ECluster()
	{
		if (m_lifetimeHelper == null || Interlocked.Increment(ref m_disposed) != 1 || m_lifetimeHelper.IsDisposed)
		{
			return;
		}
		MarkAsClosed();
		m_lifetimeHelper.AquireDisposeLock();
		try
		{
			m_lifetimeHelper.MarkAsDisposed();
			try
			{
				NotificationManager notificationManager = m_notificationManager;
				if (notificationManager != null)
				{
					notificationManager.UnregisterMonitor();
					NotificationManager notificationManager2 = m_notificationManager;
					IDisposable disposable = notificationManager2;
					((IDisposable)notificationManager2)?.Dispose();
				}
			}
			catch (Exception exception)
			{
				DebugLog.LogException(exception, "Error deleting NotificationManager in the cluster");
			}
			finally
			{
				m_notificationManager = null;
			}
			try
			{
				ObjectManager objectManager = m_objectManager;
				IDisposable disposable2 = objectManager;
				((IDisposable)objectManager)?.Dispose();
			}
			catch (Exception exception2)
			{
				DebugLog.LogException(exception2, "Error deleting ObjectManager in the cluster");
			}
			finally
			{
				m_objectManager = null;
			}
			m_hCluster = null;
			ReaderWriterLockSlim hClusterLock = m_hClusterLock;
			IDisposable disposable3 = hClusterLock;
			((IDisposable)hClusterLock)?.Dispose();
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, Resources.Cluster_Close_Failed_Text, m_name);
		}
		finally
		{
			m_lifetimeHelper.ReleaseDisposeLock();
		}
	}

	public override void Refresh()
	{
		//Discarded unreachable code: IL_007d
		Exception ex = null;
		m_lifetimeHelper.CheckObjectState();
		try
		{
			m_quorumSettings = null;
			m_networks = null;
			m_resources = null;
			m_groups = null;
			RefreshNodes();
			m_resourceTypes = null;
			InitClusterSharedVolumeResourcesCache(null);
			LoadClusterInfo();
			m_objectManager.RefreshObjects();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_Refresh_Failed_Text,
				m_name
			});
		}
	}

	public unsafe ClusterGroup GetCoreClusterGroup()
	{
		//Discarded unreachable code: IL_0075
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (m_coreClusterGroup == null)
		{
			try
			{
				ClusterRegistryKey registryKey = GetRegistryKey(RegistryRights.QueryValues);
				Guid guid = new Guid((string)registryKey.GetValue(InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BK_0040MANOJGFL_0040_003F_0024AAC_003F_0024AAl_003F_0024AAu_003F_0024AAs_003F_0024AAt_003F_0024AAe_003F_0024AAr_003F_0024AAG_003F_0024AAr_003F_0024AAo_003F_0024AAu_003F_0024AAp_0040))));
				Guid groupId = guid;
				m_coreClusterGroup = GetGroup(GetClusterCoreGroupName(guid), groupId);
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetCoreClusterGroup_Failed_Text,
					m_name
				});
			}
		}
		return m_coreClusterGroup;
	}

	public unsafe ClusterResource GetCoreClusterNetworkName()
	{
		//Discarded unreachable code: IL_007f
		string text = null;
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (m_coreClusterNetName == null)
		{
			try
			{
				text = (string)GetRegistryKey(RegistryRights.QueryValues).GetValue(InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CI_0040DIIHBNEI_0040_003F_0024AAC_003F_0024AAl_003F_0024AAu_003F_0024AAs_003F_0024AAt_003F_0024AAe_003F_0024AAr_003F_0024AAN_003F_0024AAa_003F_0024AAm_003F_0024AAe_003F_0024AAR_003F_0024AAe_003F_0024AAs_003F_0024AAo_0040)), string.Empty);
				if (text.Length != 0)
				{
					ClusterResource resource = GetResource(text, Guid.Empty, null);
					m_coreClusterNetName = resource;
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetCoreNetworkName_Failed_Text,
					m_name
				});
			}
		}
		return m_coreClusterNetName;
	}

	public AdminAccessPoint LoadAdminAccessPoint()
	{
		Property property = null;
		if (!(CurrentVersion > ClusterVersion.Windows8))
		{
			AdminAccessPoint? adminAccessPoint = AdminAccessPoint.ActiveDirectoryAndDns;
			m_AdminAccessPoint = adminAccessPoint;
			return m_AdminAccessPoint.Value;
		}
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
		property = null;
		if (commonProperties.TryGetProperty("AdminAccessPoint", out property))
		{
			AdminAccessPoint? adminAccessPoint2 = (AdminAccessPoint)(uint)property.Value;
			m_AdminAccessPoint = adminAccessPoint2;
		}
		else
		{
			AdminAccessPoint? adminAccessPoint3 = AdminAccessPoint.ActiveDirectoryAndDns;
			m_AdminAccessPoint = adminAccessPoint3;
		}
		return m_AdminAccessPoint.Value;
	}

	public unsafe ClusterGroup GetAvailableStorageGroup()
	{
		//Discarded unreachable code: IL_0073
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (m_availableStorageGroup == null)
		{
			try
			{
				ClusterRegistryKey registryKey = GetRegistryKey(RegistryRights.QueryValues);
				Guid groupId = new Guid((string)registryKey.GetValue(InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CC_0040MEMJEIKA_0040_003F_0024AAA_003F_0024AAv_003F_0024AAa_003F_0024AAi_003F_0024AAl_003F_0024AAa_003F_0024AAb_003F_0024AAl_003F_0024AAe_003F_0024AAS_003F_0024AAt_003F_0024AAo_003F_0024AAr_003F_0024AAa_003F_0024AAg_0040))));
				m_availableStorageGroup = GetGroup(GetClusterCoreGroupName(groupId), groupId);
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetAvailableStorageGroup_Failed_Text,
					m_name
				});
			}
		}
		return m_availableStorageGroup;
	}

	public ClusterResourceCollection GetAvailableStorage()
	{
		//Discarded unreachable code: IL_0047, IL_0049
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ClusterGroup availableStorageGroup = GetAvailableStorageGroup();
			return ProcessDiskCollection(availableStorageGroup.GetResources());
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetAvailableStorage_Failed_Text,
				m_name
			});
		}
	}

	public void MoveAvailableStorageToSite(ClusterResource resource, [MarshalAs(UnmanagedType.U1)] bool sameSite)
	{
		IList<ClusterNode> list = null;
		ThreadWatchdog.PerformUIThreadCheck();
		if (resource == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "resource" });
		}
		ClusterGroup availableStorageGroup = GetAvailableStorageGroup();
		List<ClusterResource> list2 = new List<ClusterResource>();
		list2.Add(resource);
		ClusterNodeCollection nodes = GetNodes();
		IDictionary<ClusterResource, IList<ClusterNode>> diskConnectivity = GetDiskConnectivity(list2, nodes);
		ClusterNode ownerNode = availableStorageGroup.GetOwnerNode();
		list = null;
		if (!diskConnectivity.TryGetValue(resource, out list))
		{
			return;
		}
		if (sameSite)
		{
			foreach (ClusterNode item in list)
			{
				Guid id = ownerNode.Id;
				if (item.Id == id)
				{
					return;
				}
			}
			if (list.Count > 0)
			{
				ClusterNode node = list[0];
				try
				{
					availableStorageGroup.BringOnline(node);
					return;
				}
				catch (Exception caughtException)
				{
					ExceptionHelp.LogException(caughtException, "Available storage failed to move to site");
					Thread.Sleep(1000);
					return;
				}
			}
			return;
		}
		List<ClusterNode> list3 = new List<ClusterNode>();
		foreach (ClusterNode item2 in nodes)
		{
			bool flag = true;
			IEnumerator<ClusterNode> enumerator3 = list.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					ClusterNode current3 = enumerator3.Current;
					Guid id2 = item2.Id;
					if (current3.Id == id2)
					{
						flag = false;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<ClusterNode> enumerator4 = enumerator3;
				IDisposable disposable = enumerator3;
				enumerator3?.Dispose();
			}
			if (flag)
			{
				list3.Add(item2);
			}
		}
		List<ClusterNode>.Enumerator enumerator5 = list3.GetEnumerator();
		if (enumerator5.MoveNext())
		{
			do
			{
				Guid id3 = enumerator5.Current.Id;
				if (ownerNode.Id == id3)
				{
					return;
				}
			}
			while (enumerator5.MoveNext());
		}
		if (list3.Count > 0)
		{
			ClusterNode node2 = list3[0];
			try
			{
				availableStorageGroup.BringOnline(node2);
			}
			catch (Exception caughtException2)
			{
				ExceptionHelp.LogException(caughtException2, "Available storage failed to move to site");
				Thread.Sleep(1000);
			}
		}
	}

	public unsafe List<Tuple<ClusterResource, DiskReplicationEligible>> GetReplicationEligibleLogDisks(ClusterResource resource, [MarshalAs(UnmanagedType.U1)] bool includeOfflineDisks)
	{
		//Discarded unreachable code: IL_015f, IL_0161
		//IL_0108: Expected I, but got I8
		//IL_0112: Expected I, but got I8
		ClusterResourceType clusterResourceType = null;
		ControlExecutor controlExecutor = null;
		UnmanagedBuffer unmanagedBuffer = null;
		UnmanagedBuffer unmanagedBuffer2 = null;
		Win32Exception ex = null;
		List<Tuple<ClusterResource, DiskReplicationEligible>> list = null;
		ClusterResource clusterResource = null;
		Exception ex2 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ClusterNode ownerNode = resource.GetOwnerNode();
			string resourceTypeName = "Storage Replica";
			clusterResourceType = GetResourceType(resourceTypeName, Guid.Empty);
			controlExecutor = clusterResourceType.GetControlExecutor();
			Guid id = resource.Id;
			_GUID gUID = InteropHelp.ToGUID(&id);
			_GUID gUID2 = gUID;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _SR_RESOURCE_TYPE_QUERY_ELIGIBLE_LOGDISKS sR_RESOURCE_TYPE_QUERY_ELIGIBLE_LOGDISKS);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref sR_RESOURCE_TYPE_QUERY_ELIGIBLE_LOGDISKS, ref gUID, 16);
			System.Runtime.CompilerServices.Unsafe.As<_SR_RESOURCE_TYPE_QUERY_ELIGIBLE_LOGDISKS, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_QUERY_ELIGIBLE_LOGDISKS, 16)) = includeOfflineDisks;
			unmanagedBuffer = new UnmanagedBuffer(&sR_RESOURCE_TYPE_QUERY_ELIGIBLE_LOGDISKS, 20uL);
			unmanagedBuffer2 = new UnmanagedBuffer();
			unmanagedBuffer2.Allocate(256uL);
			try
			{
				controlExecutor.ExecuteInOutControl(ownerNode, 33562953u, unmanagedBuffer, unmanagedBuffer2);
			}
			catch (Win32Exception ex3)
			{
				if (ex3.NativeErrorCode == -2147024662)
				{
					uint num = (uint)ex3.Data["DATA_SIZE"];
					unmanagedBuffer2.Allocate(num);
					controlExecutor.ExecuteInOutControl(ownerNode, 33562953u, unmanagedBuffer, unmanagedBuffer2);
				}
			}
			_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT* pointer = (_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT*)unmanagedBuffer2.Pointer;
			list = new List<Tuple<ClusterResource, DiskReplicationEligible>>();
			for (int i = 0; i < *(ushort*)pointer; i++)
			{
				_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT* ptr = (_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT*)((long)i * 20L + (nint)pointer);
				Guid resourceId = InteropHelp.FromGUID((_GUID*)((ulong)(nint)ptr + 8uL));
				clusterResource = GetResource(resourceId);
				list.Add(new Tuple<ClusterResource, DiskReplicationEligible>(clusterResource, *(DiskReplicationEligible*)((ulong)(nint)ptr + 4uL)));
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetReplicationEligibleLogDisks_Failed_Text,
				resource.Name
			});
		}
	}

	public unsafe List<Tuple<ClusterResource, DiskReplicationEligible>> GetReplicationEligibleTargetDisks(ClusterResource resource, [MarshalAs(UnmanagedType.U1)] bool includeConnectivityCheck, [MarshalAs(UnmanagedType.U1)] bool includeOfflineDisks, Guid targetReplicationGroupId)
	{
		//Discarded unreachable code: IL_0185, IL_0187
		//IL_012e: Expected I, but got I8
		//IL_0138: Expected I, but got I8
		ClusterResourceType clusterResourceType = null;
		ControlExecutor controlExecutor = null;
		UnmanagedBuffer unmanagedBuffer = null;
		UnmanagedBuffer unmanagedBuffer2 = null;
		Win32Exception ex = null;
		List<Tuple<ClusterResource, DiskReplicationEligible>> list = null;
		ClusterResource clusterResource = null;
		Exception ex2 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ClusterNode ownerNode = resource.GetOwnerNode();
			string resourceTypeName = "Storage Replica";
			clusterResourceType = GetResourceType(resourceTypeName, Guid.Empty);
			controlExecutor = clusterResourceType.GetControlExecutor();
			Guid id = resource.Id;
			_GUID gUID = InteropHelp.ToGUID(&id);
			_GUID gUID2 = gUID;
			_GUID gUID3 = InteropHelp.ToGUID(&targetReplicationGroupId);
			_GUID gUID4 = gUID3;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _SR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS sR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref sR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS, ref gUID, 16);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS, 16), ref gUID3, 16);
			byte b = ((!includeConnectivityCheck) ? ((byte)1) : ((byte)0));
			System.Runtime.CompilerServices.Unsafe.As<_SR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS, 32)) = b;
			System.Runtime.CompilerServices.Unsafe.As<_SR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS, 33)) = includeOfflineDisks;
			unmanagedBuffer = new UnmanagedBuffer(&sR_RESOURCE_TYPE_ELIGIBLE_TARGET_DATADISKS, 36uL);
			unmanagedBuffer2 = new UnmanagedBuffer();
			unmanagedBuffer2.Allocate(256uL);
			try
			{
				controlExecutor.ExecuteInOutControl(ownerNode, 33562957u, unmanagedBuffer, unmanagedBuffer2);
			}
			catch (Win32Exception ex3)
			{
				if (ex3.NativeErrorCode == -2147024662)
				{
					uint num = (uint)ex3.Data["DATA_SIZE"];
					unmanagedBuffer2.Allocate(num);
					controlExecutor.ExecuteInOutControl(ownerNode, 33562957u, unmanagedBuffer, unmanagedBuffer2);
				}
			}
			_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT* pointer = (_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT*)unmanagedBuffer2.Pointer;
			list = new List<Tuple<ClusterResource, DiskReplicationEligible>>();
			for (int i = 0; i < *(ushort*)pointer; i++)
			{
				_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT* ptr = (_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT*)((long)i * 20L + (nint)pointer);
				Guid resourceId = InteropHelp.FromGUID((_GUID*)((ulong)(nint)ptr + 8uL));
				clusterResource = GetResource(resourceId);
				list.Add(new Tuple<ClusterResource, DiskReplicationEligible>(clusterResource, *(DiskReplicationEligible*)((ulong)(nint)ptr + 4uL)));
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetReplicationEligibleTargetDisks_Failed_Text,
				resource.Name
			});
		}
	}

	public unsafe List<Tuple<ClusterResource, DiskReplicationEligible>> GetReplicationEligibleSourceDisks(ClusterResource resource)
	{
		//Discarded unreachable code: IL_015f, IL_0161
		//IL_0108: Expected I, but got I8
		//IL_0112: Expected I, but got I8
		ClusterResourceType clusterResourceType = null;
		ControlExecutor controlExecutor = null;
		UnmanagedBuffer unmanagedBuffer = null;
		UnmanagedBuffer unmanagedBuffer2 = null;
		Win32Exception ex = null;
		List<Tuple<ClusterResource, DiskReplicationEligible>> list = null;
		ClusterResource clusterResource = null;
		Exception ex2 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ClusterNode ownerNode = resource.GetOwnerNode();
			string resourceTypeName = "Storage Replica";
			clusterResourceType = GetResourceType(resourceTypeName, Guid.Empty);
			controlExecutor = clusterResourceType.GetControlExecutor();
			Guid id = resource.Id;
			_GUID gUID = InteropHelp.ToGUID(&id);
			_GUID gUID2 = gUID;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _SR_RESOURCE_TYPE_ELIGIBLE_SOURCE_DATADISKS sR_RESOURCE_TYPE_ELIGIBLE_SOURCE_DATADISKS);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref sR_RESOURCE_TYPE_ELIGIBLE_SOURCE_DATADISKS, ref gUID, 16);
			System.Runtime.CompilerServices.Unsafe.As<_SR_RESOURCE_TYPE_ELIGIBLE_SOURCE_DATADISKS, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_ELIGIBLE_SOURCE_DATADISKS, 16)) = 0;
			unmanagedBuffer = new UnmanagedBuffer(&sR_RESOURCE_TYPE_ELIGIBLE_SOURCE_DATADISKS, 20uL);
			unmanagedBuffer2 = new UnmanagedBuffer();
			unmanagedBuffer2.Allocate(256uL);
			try
			{
				controlExecutor.ExecuteInOutControl(ownerNode, 33562961u, unmanagedBuffer, unmanagedBuffer2);
			}
			catch (Win32Exception ex3)
			{
				if (ex3.NativeErrorCode == -2147024662)
				{
					uint num = (uint)ex3.Data["DATA_SIZE"];
					unmanagedBuffer2.Allocate(num);
					controlExecutor.ExecuteInOutControl(ownerNode, 33562961u, unmanagedBuffer, unmanagedBuffer2);
				}
			}
			_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT* pointer = (_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT*)unmanagedBuffer2.Pointer;
			list = new List<Tuple<ClusterResource, DiskReplicationEligible>>();
			for (int i = 0; i < *(ushort*)pointer; i++)
			{
				_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT* ptr = (_SR_RESOURCE_TYPE_ELIGIBLE_DISKS_RESULT*)((long)i * 20L + (nint)pointer);
				Guid resourceId = InteropHelp.FromGUID((_GUID*)((ulong)(nint)ptr + 8uL));
				clusterResource = GetResource(resourceId);
				list.Add(new Tuple<ClusterResource, DiskReplicationEligible>(clusterResource, *(DiskReplicationEligible*)((ulong)(nint)ptr + 4uL)));
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetReplicationEligibleTargetDisks_Failed_Text,
				resource.Name
			});
		}
	}

	public unsafe List<ReplicatedDiskInfo> GetReplicatedDisks()
	{
		//Discarded unreachable code: IL_0127, IL_0129
		ClusterResourceType clusterResourceType = null;
		ControlExecutor controlExecutor = null;
		UnmanagedBuffer unmanagedBuffer = null;
		Win32Exception ex = null;
		ReplicatedDiskInfo replicatedDiskInfo = null;
		Exception ex2 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			List<ReplicatedDiskInfo> list = new List<ReplicatedDiskInfo>();
			string resourceTypeName = "Storage Replica";
			clusterResourceType = GetResourceType(resourceTypeName, Guid.Empty);
			controlExecutor = clusterResourceType.GetControlExecutor();
			unmanagedBuffer = new UnmanagedBuffer();
			unmanagedBuffer.Allocate(256uL);
			try
			{
				controlExecutor.ExecuteOutControl(33562965u, unmanagedBuffer);
			}
			catch (Win32Exception ex3)
			{
				if (ex3.NativeErrorCode == -2147024662)
				{
					uint num = (uint)ex3.Data["DATA_SIZE"];
					unmanagedBuffer.Allocate(num);
					controlExecutor.ExecuteOutControl(33562965u, unmanagedBuffer);
				}
			}
			_SR_RESOURCE_TYPE_REPLICATED_DISKS_RESULT* pointer = (_SR_RESOURCE_TYPE_REPLICATED_DISKS_RESULT*)unmanagedBuffer.Pointer;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _SR_RESOURCE_TYPE_REPLICATED_DISK sR_RESOURCE_TYPE_REPLICATED_DISK);
			for (int i = 0; i < *(ushort*)pointer; i++)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref sR_RESOURCE_TYPE_REPLICATED_DISK, (nint)pointer + (long)i * 556L + 4, 556);
				replicatedDiskInfo = new ReplicatedDiskInfo(*(ReplicatedDiskType*)(&sR_RESOURCE_TYPE_REPLICATED_DISK), *(Guid*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_REPLICATED_DISK, 4)), *(Guid*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_REPLICATED_DISK, 20)), new string((char*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sR_RESOURCE_TYPE_REPLICATED_DISK, 36))));
				list.Add(replicatedDiskInfo);
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.Cluster_GetReplicatedDisks_Failed_Text });
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GetReplicationValidateLogDisks(ClusterResource data, ClusterResource log, out string volumeId)
	{
		//Discarded unreachable code: IL_0224, IL_022e, IL_0257, IL_0259
		//IL_002d: Expected I, but got I8
		ClusterResourceType clusterResourceType = null;
		ControlExecutor controlExecutor = null;
		Win32Exception ex = null;
		Win32Exception ex2 = null;
		Dictionary<string, Property> dictionary = null;
		Property property = null;
		Exception ex3 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			_HCLUSPROPLIST* ptr = global::_003CModule_003E.CreatePropList(null, 0u);
			if (ptr == null)
			{
				string[] args = new string[0];
				throw ExceptionHelp.Build<Win32Exception>((int)global::_003CModule_003E.GetLastError(), args);
			}
			try
			{
				UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType(data.Id.ToString());
				try
				{
					uint num = global::_003CModule_003E.AddStringProperty(ptr, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BO_0040JLHNBCIO_0040_003F_0024AAD_003F_0024AAa_003F_0024AAt_003F_0024AAa_003F_0024AAR_003F_0024AAe_003F_0024AAs_003F_0024AAo_003F_0024AAu_003F_0024AAr_003F_0024AAc_003F_0024AAe_003F_0024AAI_003F_0024AAd_0040), (ushort*)unmanagedBuffer.Pointer);
					if (num != 0)
					{
						string[] args2 = new string[0];
						throw ExceptionHelp.Build<Win32Exception>((int)num, args2);
					}
				}
				finally
				{
					unmanagedBuffer.Free();
				}
				unmanagedBuffer = TypeConverter.ConvertToNativeType(log.Id.ToString());
				try
				{
					uint num = global::_003CModule_003E.AddStringProperty(ptr, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BM_0040HDCIKKPG_0040_003F_0024AAL_003F_0024AAo_003F_0024AAg_003F_0024AAR_003F_0024AAe_003F_0024AAs_003F_0024AAo_003F_0024AAu_003F_0024AAr_003F_0024AAc_003F_0024AAe_003F_0024AAI_003F_0024AAd_0040), (ushort*)unmanagedBuffer.Pointer);
					if (num != 0)
					{
						string[] args3 = new string[0];
						throw ExceptionHelp.Build<Win32Exception>((int)num, args3);
					}
				}
				finally
				{
					unmanagedBuffer.Free();
				}
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_LIST* pMem);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
				global::_003CModule_003E.GetPropertyListBuffer(ptr, &pMem, &num2);
				UnmanagedBuffer unmanagedBuffer2 = new UnmanagedBuffer(pMem, num2);
				UnmanagedBuffer unmanagedBuffer3 = new UnmanagedBuffer();
				unmanagedBuffer3.Allocate(1024uL);
				try
				{
					ClusterNode ownerNode = data.GetOwnerNode();
					string resourceTypeName = "Storage Replica";
					clusterResourceType = GetResourceType(resourceTypeName, Guid.Empty);
					controlExecutor = clusterResourceType.GetControlExecutor();
					bool result;
					bool result2;
					try
					{
						controlExecutor.ExecuteInOutControl(ownerNode, 33562973u, unmanagedBuffer2, unmanagedBuffer3);
					}
					catch (Win32Exception ex4)
					{
						if (ex4.NativeErrorCode == -2147024662)
						{
							try
							{
								uint num3 = (uint)ex4.Data["DATA_SIZE"];
								unmanagedBuffer3.Allocate(num3);
								controlExecutor.ExecuteInOutControl(ownerNode, 33562973u, unmanagedBuffer2, unmanagedBuffer3);
								goto end_IL_0135;
							}
							catch (Win32Exception ex5)
							{
								if (ex5.NativeErrorCode == -2147024846)
								{
									result = false;
									goto end_IL_017a;
								}
								goto end_IL_0135;
								end_IL_017a:;
							}
							goto IL_0216;
						}
						if (ex4.NativeErrorCode == -2147024846)
						{
							result2 = false;
							goto IL_0210;
						}
						end_IL_0135:;
					}
					dictionary = PropertyCollection.ConvertPropertyListToDictionary((CLUSPROP_LIST*)unmanagedBuffer3.Pointer, unmanagedBuffer3.Size, isReadOnly: true, null);
					property = null;
					if (!dictionary.TryGetValue("LogVolume", out property))
					{
						throw ExceptionHelp.Build<ApplicationException>(1168, new string[2]
						{
							Resources.Cluster_ValidateReplicationLogDisk_Failed_Text,
							data.Name
						});
					}
					volumeId = (string)property.Value;
					return true;
					IL_0210:
					return result2;
					IL_0216:
					return result;
				}
				finally
				{
					unmanagedBuffer2.Free();
				}
			}
			finally
			{
				global::_003CModule_003E.DestroyPropList(ptr);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_ValidateReplicationTargetDisk_Failed_Text,
				data.Name
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GetReplicationValidateTargetDisks(ClusterResource source, out List<string> volumesSource, ClusterResource destination, out List<string> volumesDestination)
	{
		//Discarded unreachable code: IL_0316, IL_0320, IL_0349, IL_034b
		//IL_0042: Expected I, but got I8
		ClusterResourceType clusterResourceType = null;
		ControlExecutor controlExecutor = null;
		Win32Exception ex = null;
		Win32Exception ex2 = null;
		Dictionary<string, Property> dictionary = null;
		Property property = null;
		List<string> list = null;
		StringEnumerator stringEnumerator = null;
		string text = null;
		Property property2 = null;
		List<string> list2 = null;
		StringEnumerator stringEnumerator2 = null;
		string text2 = null;
		Exception ex3 = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			_HCLUSPROPLIST* ptr = global::_003CModule_003E.CreatePropList(null, 0u);
			if (ptr == null)
			{
				string[] args = new string[0];
				throw ExceptionHelp.Build<Win32Exception>((int)global::_003CModule_003E.GetLastError(), args);
			}
			try
			{
				UnmanagedBuffer unmanagedBuffer = TypeConverter.ConvertToNativeType(source.Id.ToString());
				try
				{
					uint num = global::_003CModule_003E.AddStringProperty(ptr, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CC_0040LPPEEDCL_0040_003F_0024AAS_003F_0024AAo_003F_0024AAu_003F_0024AAr_003F_0024AAc_003F_0024AAe_003F_0024AAR_003F_0024AAe_003F_0024AAs_003F_0024AAo_003F_0024AAu_003F_0024AAr_003F_0024AAc_003F_0024AAe_003F_0024AAI_0040), (ushort*)unmanagedBuffer.Pointer);
					if (num != 0)
					{
						string[] args2 = new string[0];
						throw ExceptionHelp.Build<Win32Exception>((int)num, args2);
					}
				}
				finally
				{
					unmanagedBuffer.Free();
				}
				unmanagedBuffer = TypeConverter.ConvertToNativeType(destination.Id.ToString());
				try
				{
					uint num = global::_003CModule_003E.AddStringProperty(ptr, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CC_0040ICKPGBOI_0040_003F_0024AAT_003F_0024AAa_003F_0024AAr_003F_0024AAg_003F_0024AAe_003F_0024AAt_003F_0024AAR_003F_0024AAe_003F_0024AAs_003F_0024AAo_003F_0024AAu_003F_0024AAr_003F_0024AAc_003F_0024AAe_003F_0024AAI_0040), (ushort*)unmanagedBuffer.Pointer);
					if (num != 0)
					{
						string[] args3 = new string[0];
						throw ExceptionHelp.Build<Win32Exception>((int)num, args3);
					}
				}
				finally
				{
					unmanagedBuffer.Free();
				}
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_LIST* pMem);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
				global::_003CModule_003E.GetPropertyListBuffer(ptr, &pMem, &num2);
				UnmanagedBuffer unmanagedBuffer2 = new UnmanagedBuffer(pMem, num2);
				UnmanagedBuffer unmanagedBuffer3 = new UnmanagedBuffer();
				unmanagedBuffer3.Allocate(1024uL);
				try
				{
					ClusterNode ownerNode = source.GetOwnerNode();
					string resourceTypeName = "Storage Replica";
					clusterResourceType = GetResourceType(resourceTypeName, Guid.Empty);
					controlExecutor = clusterResourceType.GetControlExecutor();
					bool result;
					bool result2;
					try
					{
						controlExecutor.ExecuteInOutControl(ownerNode, 33562969u, unmanagedBuffer2, unmanagedBuffer3);
					}
					catch (Win32Exception ex4)
					{
						if (ex4.NativeErrorCode == -2147024662)
						{
							try
							{
								uint num3 = (uint)ex4.Data["DATA_SIZE"];
								unmanagedBuffer3.Allocate(num3);
								controlExecutor.ExecuteInOutControl(ownerNode, 33562969u, unmanagedBuffer2, unmanagedBuffer3);
								goto end_IL_014a;
							}
							catch (Win32Exception ex5)
							{
								if (ex5.NativeErrorCode == -2147024846)
								{
									result = false;
									goto end_IL_018f;
								}
								goto end_IL_014a;
								end_IL_018f:;
							}
							goto IL_0308;
						}
						if (ex4.NativeErrorCode == -2147024846)
						{
							result2 = false;
							goto IL_0302;
						}
						end_IL_014a:;
					}
					dictionary = PropertyCollection.ConvertPropertyListToDictionary((CLUSPROP_LIST*)unmanagedBuffer3.Pointer, unmanagedBuffer3.Size, isReadOnly: true, null);
					property = null;
					if (!dictionary.TryGetValue("SourceVolumes", out property))
					{
						throw ExceptionHelp.Build<ApplicationException>(1168, new string[2]
						{
							Resources.Cluster_ValidateReplicationTargetDisk_Failed_Text,
							source.Name
						});
					}
					list = new List<string>();
					stringEnumerator = ((StringCollection)property.Value).GetEnumerator();
					try
					{
						while (stringEnumerator.MoveNext())
						{
							text = stringEnumerator.Current;
							list.Add(text);
						}
					}
					finally
					{
						StringEnumerator stringEnumerator3 = stringEnumerator;
						if (stringEnumerator is IDisposable disposable)
						{
							disposable.Dispose();
						}
					}
					property2 = null;
					if (!dictionary.TryGetValue("TargetVolumes", out property2))
					{
						throw ExceptionHelp.Build<ApplicationException>(1168, new string[2]
						{
							Resources.Cluster_ValidateReplicationTargetDisk_Failed_Text,
							source.Name
						});
					}
					list2 = new List<string>();
					stringEnumerator2 = ((StringCollection)property2.Value).GetEnumerator();
					try
					{
						while (stringEnumerator2.MoveNext())
						{
							text2 = stringEnumerator2.Current;
							list2.Add(text2);
						}
					}
					finally
					{
						StringEnumerator stringEnumerator4 = stringEnumerator2;
						if (stringEnumerator2 is IDisposable disposable2)
						{
							disposable2.Dispose();
						}
					}
					volumesSource = list;
					volumesDestination = list2;
					return true;
					IL_0302:
					return result2;
					IL_0308:
					return result;
				}
				finally
				{
					unmanagedBuffer2.Free();
				}
			}
			finally
			{
				global::_003CModule_003E.DestroyPropList(ptr);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_ValidateReplicationTargetDisk_Failed_Text,
				source.Name
			});
		}
	}

	public void MoveStorageToAvailableStorage(ClusterResource resource, ClusterGroup srcGroup)
	{
		//Discarded unreachable code: IL_0095, IL_00d0
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			ClusterGroup clusterGroup = GetAvailableStorageGroup();
			try
			{
				resource.RemoveAllNonStorageDependencyLinks();
				if (resource.IsQuorumResource)
				{
					ClusterGroup clusterGroup2 = clusterGroup;
					IDisposable disposable = clusterGroup;
					((IDisposable)clusterGroup)?.Dispose();
					clusterGroup = GetCoreClusterGroup();
				}
				ICollection<ClusterResource> collection = resource.ChangeGroup2(clusterGroup);
				ICollection<ClusterResource> collection2 = collection;
				if (collection is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
				if (resource.PhysicalDisk_IsPassThroughDisk())
				{
					byte condition = ((!resource.IsQuorumResource) ? ((byte)1) : ((byte)0));
					Debug.Assert(condition != 0);
					resource.PhysicalDisk_ResetPassThroughDisk();
					resource.TakeOffline();
				}
				try
				{
					resource.BeginBringOnline();
				}
				catch (Exception caughtException)
				{
					if (ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
					{
						ExceptionHelp.LogException(caughtException, "Error when moving storage to available storage");
						return;
					}
					throw;
				}
			}
			finally
			{
				ClusterGroup clusterGroup3 = clusterGroup;
				IDisposable disposable3 = clusterGroup;
				((IDisposable)clusterGroup)?.Dispose();
				((IDisposable)srcGroup)?.Dispose();
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.Cluster_MoveToAvailableStorage_Failed_Text });
		}
	}

	public void MoveStorageToAvailableStorage(ClusterResource resource)
	{
		//Discarded unreachable code: IL_002c
		ThreadWatchdog.PerformUIThreadCheck();
		ClusterGroup clusterGroup = null;
		try
		{
			resource.Refresh();
			clusterGroup = resource.GetOwnerGroup();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[1] { Resources.Cluster_MoveToAvailableStorage_Failed_Text });
		}
		MoveStorageToAvailableStorage(resource, clusterGroup);
	}

	public ClusterResourceCollection GetClusterSharedVolumesStorage()
	{
		//Discarded unreachable code: IL_0060, IL_0062
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceCollection clusterResourceCollection = null;
		if (clusterResourceCollection == null || ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_clusterSharedVolumesStorageLock);
			try
			{
				if (clusterResourceCollection == null || ClusterItem.CachingDisabled)
				{
					clusterResourceCollection = new ClusterResourceCollection(BuildClusterSharedVolumesResourceAsyncEnum());
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.GetClusterSharedVolumesStorageFailedFormat_Text,
					m_name
				});
			}
			finally
			{
				Monitor.Exit(m_clusterSharedVolumesStorageLock);
			}
		}
		return clusterResourceCollection;
	}

	public void AddStorageToClusterSharedVolumes(ClusterResource diskResource)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (diskResource == null)
		{
			throw new ArgumentNullException("diskResource");
		}
		m_lifetimeHelper.CheckObjectState();
		NativeMethods.AddResourceToClusterSharedVolumes(this, diskResource);
	}

	public void RemoveStorageFromClusterSharedVolumes(ClusterResource diskResource)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (diskResource == null)
		{
			throw new ArgumentNullException("diskResource");
		}
		m_lifetimeHelper.CheckObjectState();
		NativeMethods.RemoveResourceFromClusterSharedVolumes(this, diskResource);
	}

	public unsafe ClusterNode AddNode(string nodeName, ClusterActionCallback callback)
	{
		//IL_0013: Expected I, but got I8
		//IL_0036: Expected I, but got I8
		//IL_005c: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		ClusterNode clusterNode = null;
		if (nodeName == null)
		{
			throw new ArgumentNullException("nodeName");
		}
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int> delegate_002A = null;
			if (callback != null)
			{
				m_Callback = callback;
				delegate_002A = (delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int>)global::_003CModule_003E.__unep_0040_003FCreateClusterCallbackWrapper_0040CClusterCallbackHelper_0040ServerClusters_0040Internal_0040MS_0040_0040_0024_0024FSAHPEAXW4_CLUSTER_SETUP_PHASE_0040_0040W4_CLUSTER_SETUP_PHASE_TYPE_0040_0040W4_CLUSTER_SETUP_PHASE_SEVERITY_0040_0040KPEBGK_0040Z;
			}
			SafeNodeHandle safeNodeHandle = new SafeNodeHandle(global::_003CModule_003E.AddClusterNode(Handle, ptr, delegate_002A, null));
			if (safeNodeHandle.IsInvalid)
			{
				int lastError = (int)global::_003CModule_003E.GetLastError();
				int num = -2147023174;
				if (lastError == -2147023174)
				{
					throw ExceptionHelp.Build<ApplicationException>(-2147023174, new string[3]
					{
						Resources.AddNodeFail_Text,
						nodeName,
						m_name
					});
				}
				int num2 = -2147024846;
				if (lastError != -2147024846 && lastError != 50)
				{
					ClusApiExceptionFactory.CreateAndThrow(this, lastError, Resources.AddNodeFail_Text, nodeName, m_name);
				}
				else
				{
					ClusApiExceptionFactory.CreateAndThrow(this, lastError, Resources.AddNodeFail_S2DNotSupported_Text, nodeName, m_name);
				}
			}
			clusterNode = ClusterNode.CreateObject(this, safeNodeHandle, nodeName);
			RefreshNodes();
			return clusterNode;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			m_Callback = null;
		}
	}

	public unsafe void AddStorageOnlyNode(string nodeName, ClusterActionCallback callback)
	{
		//IL_0013: Expected I, but got I8
		//IL_0032: Expected I, but got I8
		//IL_005a: Expected I, but got I8
		//IL_005a: Expected I, but got I8
		//IL_005a: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		if (nodeName == null)
		{
			throw new ArgumentNullException("nodeName");
		}
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int> delegate_002A = null;
			if (callback != null)
			{
				m_Callback = callback;
				delegate_002A = (delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int>)global::_003CModule_003E.__unep_0040_003FCreateClusterCallbackWrapper_0040CClusterCallbackHelper_0040ServerClusters_0040Internal_0040MS_0040_0040_0024_0024FSAHPEAXW4_CLUSTER_SETUP_PHASE_0040_0040W4_CLUSTER_SETUP_PHASE_TYPE_0040_0040W4_CLUSTER_SETUP_PHASE_SEVERITY_0040_0040KPEBGK_0040Z;
			}
			int num = (int)global::_003CModule_003E.AddClusterStorageNode(Handle, ptr, delegate_002A, null, null, null);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, num, Resources.AddNodeFail_Text, nodeName, m_name);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			m_Callback = null;
		}
	}

	public unsafe void RemoveStorageOnlyNode(string nodeName, TimeSpan timeout)
	{
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		if (nodeName == null)
		{
			throw new ArgumentNullException("nodeName");
		}
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			uint num = (uint)timeout.TotalMilliseconds;
			int num2 = (int)global::_003CModule_003E.RemoveClusterStorageNode(Handle, ptr, num, 0u);
			if (num2 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, num2, Resources.NodeEvictFail_Text, nodeName);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			m_Callback = null;
		}
	}

	public void BeginShutdown()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ControlExecutor controlExecutor = GetControlExecutor();
		try
		{
			controlExecutor.ExecuteControl(117440589u);
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null && firstException.NativeErrorCode != -2147023899)
			{
				throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
				{
					Resources.Cluster_ShutdownFailed_Text,
					m_name
				});
			}
		}
	}

	public unsafe void Destroy([MarshalAs(UnmanagedType.U1)] bool cleanupAD, ClusterActionCallback callback)
	{
		//IL_0015: Expected I, but got I8
		//IL_0039: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			uint num = 0u;
			delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int> delegate_002A = null;
			if (callback != null)
			{
				m_Callback = callback;
				delegate_002A = (delegate* unmanaged[Cdecl, Cdecl]<void*, _CLUSTER_SETUP_PHASE, _CLUSTER_SETUP_PHASE_TYPE, _CLUSTER_SETUP_PHASE_SEVERITY, uint, ushort*, uint, int>)global::_003CModule_003E.__unep_0040_003FCreateClusterCallbackWrapper_0040CClusterCallbackHelper_0040ServerClusters_0040Internal_0040MS_0040_0040_0024_0024FSAHPEAXW4_CLUSTER_SETUP_PHASE_0040_0040W4_CLUSTER_SETUP_PHASE_TYPE_0040_0040W4_CLUSTER_SETUP_PHASE_SEVERITY_0040_0040KPEBGK_0040Z;
			}
			uint num2 = global::_003CModule_003E.DestroyCluster(Handle, delegate_002A, null, cleanupAD ? 1 : 0);
			if (num2 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)num2, Resources.DestroyClusterFail_Text, m_name);
			}
		}
		finally
		{
			m_Callback = null;
		}
	}

	public ClusterGroup CreateOfflineGroup(string groupName, GroupType type)
	{
		//Discarded unreachable code: IL_0050
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterGroup clusterGroup = null;
		try
		{
			NativeMethods.CreateClusterGroup(this, groupName, type).Close();
			clusterGroup = ClusterGroup.CreateObject(this, groupName);
			m_groups = null;
			return clusterGroup;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.CreateGroup_Failed_Text,
				groupName
			});
		}
	}

	public ClusterGroup CreateGroup(string groupName, GroupType type)
	{
		//Discarded unreachable code: IL_005d
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterGroup clusterGroup = null;
		try
		{
			clusterGroup = CreateOfflineGroup(groupName, type);
			ClusterNode ownerNode = clusterGroup.GetOwnerNode();
			try
			{
				if (ownerNode.State == NodeState.Up)
				{
					clusterGroup.BeginBringOnline();
				}
			}
			finally
			{
				ClusterNode clusterNode = ownerNode;
				IDisposable disposable = ownerNode;
				((IDisposable)ownerNode)?.Dispose();
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.CreateGroup_Failed_Text,
				groupName
			});
		}
		return clusterGroup;
	}

	public unsafe ClusterResourceType CreateResourceType(string resourceTypeName, string displayName, string resourceTypeDll, long looksAlivePollInterval, long isAlivePollInterval)
	{
		//IL_0013: Expected I, but got I8
		//IL_0016: Expected I, but got I8
		//IL_0019: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		ushort* ptr2 = null;
		ushort* ptr3 = null;
		if (resourceTypeName == null)
		{
			throw new ArgumentNullException("resourceTypeName");
		}
		if (displayName == null)
		{
			throw new ArgumentNullException("displayName");
		}
		if (resourceTypeDll == null)
		{
			throw new ArgumentNullException("resourceTypeDll");
		}
		try
		{
			ptr = InteropHelp.StringToWstr(resourceTypeName);
			ptr2 = InteropHelp.StringToWstr(displayName);
			ptr3 = InteropHelp.StringToWstr(resourceTypeDll);
			uint num = global::_003CModule_003E.CreateClusterResourceType(Handle, ptr, ptr2, ptr3, (uint)looksAlivePollInterval, (uint)isAlivePollInterval);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)num, Resources.CreateResourceTypeFail_Text, displayName);
			}
			m_resourceTypes = null;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			InteropHelp.FreeWstr(ptr2);
			InteropHelp.FreeWstr(ptr3);
		}
		return GetResourceType(resourceTypeName, Guid.Empty);
	}

	public ClusterResourceType CreateResourceType(string resourceTypeName, string displayName, string resourceTypeDll)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		return CreateResourceType(resourceTypeName, displayName, resourceTypeDll, 5000L, 60000L);
	}

	public unsafe void CreateClusterNameAccount(string name, string domain, string userName, SecureString password, AdminAccessPointResType CNOResType, [MarshalAs(UnmanagedType.U1)] bool UpgradeVCOs)
	{
		//IL_005c: Expected I, but got I8
		//IL_005f: Expected I, but got I8
		//IL_0062: Expected I, but got I8
		//IL_00a3: Expected I8, but got I
		//IL_00aa: Expected I8, but got I
		//IL_00b7: Expected I8, but got I
		//IL_00be: Expected I8, but got I
		//IL_00f2: Expected I, but got I8
		//IL_00f2: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if ((!(domain != null) && !(userName != null) && password == null) || (!(domain == null) && !(userName == null) && password != null))
		{
			ushort* ptr = null;
			ushort* ptr2 = null;
			ushort* ptr3 = null;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				ptr = InteropHelp.StringToWstr(name);
				if (domain != null)
				{
					ptr2 = InteropHelp.StringToWstr(domain);
					ptr3 = InteropHelp.StringToWstr(userName);
					intPtr = Marshal.SecureStringToGlobalAllocUnicode(password);
				}
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _CREATE_CLUSTER_NAME_ACCOUNT cREATE_CLUSTER_NAME_ACCOUNT);
				*(int*)(&cREATE_CLUSTER_NAME_ACCOUNT) = 2560;
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 16)) = 0;
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 8)) = (nint)ptr;
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 24)) = (nint)ptr3;
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 32)) = (nint)intPtr.ToPointer();
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 40)) = (nint)ptr2;
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 48)) = 1;
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, AdminAccessPointResType>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 52)) = CNOResType;
				int num = (UpgradeVCOs ? 1 : 0);
				System.Runtime.CompilerServices.Unsafe.As<_CREATE_CLUSTER_NAME_ACCOUNT, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cREATE_CLUSTER_NAME_ACCOUNT, 56)) = (byte)num;
				uint num2 = global::_003CModule_003E.CreateClusterNameAccount(Handle, &cREATE_CLUSTER_NAME_ACCOUNT, null, null);
				if (num2 != 0)
				{
					ClusApiExceptionFactory.CreateAndThrow(this, (int)num2, Resources.CreateClusterNameAccountFail_Text, name);
				}
				return;
			}
			finally
			{
				InteropHelp.FreeWstr(ptr);
				InteropHelp.FreeWstr(ptr2);
				InteropHelp.FreeWstr(ptr3);
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
				}
			}
		}
		throw new ArgumentException(Resources.CreateClusterNameAccount_InvalidArguments_Text);
	}

	public unsafe void RemoveClusterNameAccount([MarshalAs(UnmanagedType.U1)] bool bDeleteComputerObjects)
	{
		uint num = global::_003CModule_003E.RemoveClusterNameAccount(Handle, bDeleteComputerObjects ? 1 : 0);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(this, (int)num, Resources.RemoveClusterNameAccountFail_Text);
		}
	}

	public unsafe static AdminAccessPointResType DetermineCNOResType(StringCollection nodeNames)
	{
		//IL_0003: Expected I, but got I8
		ushort** ptr = null;
		int count = nodeNames.Count;
		InteropHelp.ConvertStringCollectionToPWSTRArray(nodeNames, &ptr);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSTER_MGMT_POINT_RESTYPE result);
		uint num = global::_003CModule_003E.DetermineCNOResTypeFromNodelist((uint)count, ptr, &result);
		if (num != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)num, new string[1] { Resources.DetermineCNOResTypeFail_Text });
		}
		return (AdminAccessPointResType)result;
	}

	public QuorumSettings GetQuorumSettings()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		QuorumSettings quorumSettings = m_quorumSettings;
		if (quorumSettings == null || ClusterItem.CachingDisabled)
		{
			quorumSettings = (m_quorumSettings = InternalGetQuorumSettings());
		}
		return quorumSettings;
	}

	public ClusterResource GetQuorumResource()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (GetQuorumSettings() is IHasQuorumResource hasQuorumResource)
		{
			return hasQuorumResource.QuorumResource;
		}
		return null;
	}

	public QuorumManager GetQuorumManager()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return new QuorumManager(this);
	}

	public void Rename(string newName)
	{
		//Discarded unreachable code: IL_0061
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ClusterResource coreClusterNetworkName = GetCoreClusterNetworkName();
			if (coreClusterNetworkName != null)
			{
				OfflineManager offlineManager = OfflineManager.Create(coreClusterNetworkName);
				try
				{
					offlineManager.TakeOffline();
					RenameCluster(newName);
					return;
				}
				finally
				{
					offlineManager.BeginBringOnline();
				}
			}
			RenameCluster(newName);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_Rename_Failed_Text,
				m_name
			});
		}
	}

	public unsafe void SetNetworkPriorityOrder(ClusterNetwork[] networks)
	{
		//IL_004c: Expected I8, but got I
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ulong num = (ulong)networks.LongLength;
		_HNETWORK** ptr = (_HNETWORK**)global::_003CModule_003E.new_005B_005D((num > 2305843009213693951L) ? ulong.MaxValue : (num * 8));
		try
		{
			for (int i = 0; i < (nint)networks.LongLength; i++)
			{
				*(long*)((long)i * 8L + (nint)ptr) = (nint)networks[i].Handle;
			}
			uint num2 = global::_003CModule_003E.SetClusterNetworkPriorityOrder(Handle, (uint)networks.Length, ptr);
			if (num2 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(this, (int)num2, Resources.SetClusterNetworkPriorityOrderFail_Text, m_name);
			}
		}
		finally
		{
			global::_003CModule_003E.delete_005B_005D(ptr);
		}
	}

	public ClusterNodeCollection GetNodes()
	{
		//Discarded unreachable code: IL_007d, IL_007f
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterNodeCollection clusterNodeCollection = m_nodes;
		if (clusterNodeCollection == null || ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_nodesLock);
			try
			{
				clusterNodeCollection = m_nodes;
				if (clusterNodeCollection == null || ClusterItem.CachingDisabled)
				{
					clusterNodeCollection = (m_nodes = new ClusterNodeCollection(BuildNodeAsyncEnum()));
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetNodes_Failed_Text,
					m_name
				});
			}
			finally
			{
				Monitor.Exit(m_nodesLock);
			}
		}
		return clusterNodeCollection;
	}

	public IEnumerable<string> GetAllNodeFQDNNames()
	{
		List<string> list = new List<string>();
		foreach (ClusterNode currentNode in GetCurrentNodes())
		{
			list.Add(currentNode.GetFqdnName());
		}
		return list;
	}

	public IEnumerable<string> GetCurrentNodeNames(NodeState nodeState)
	{
		List<string> list = new List<string>();
		foreach (ClusterNode currentNode in GetCurrentNodes())
		{
			if (currentNode.State == nodeState)
			{
				list.Add(currentNode.Name);
			}
		}
		return list;
	}

	public IEnumerable<ClusterNode> GetCurrentNodes()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		List<ClusterNode> list = new List<ClusterNode>();
		foreach (ClusterNode node in GetNodes())
		{
			list.Add(node);
		}
		return list;
	}

	public AsyncEnumerationStatus GetNodesAsync(AsyncEnumerationCallback<ClusterNode> callback)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterNodeCollection nodes = m_nodes;
		AsyncEnumeration<ClusterNode> asyncEnumeration;
		if (nodes != null && !ClusterItem.CachingDisabled)
		{
			asyncEnumeration = new AsyncEnumeration<ClusterNode>(nodes);
		}
		else
		{
			asyncEnumeration = BuildNodeAsyncEnum();
			asyncEnumeration.EnumerationComplete += OnNodeAsyncEnumCompleted;
		}
		asyncEnumeration.SetCallback(callback);
		return asyncEnumeration.StartEnumeration(useDifferentThread: true);
	}

	public unsafe ClusterStorageOnlyNodeCollection GetStorageOnlyNodes()
	{
		//Discarded unreachable code: IL_00ed, IL_00ef
		//IL_004b: Expected I4, but got I8
		//IL_0069: Expected I, but got I8
		//IL_00b2: Expected I, but got I8
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterStorageOnlyNodeCollection clusterStorageOnlyNodeCollection = m_storageOnlyNodes;
		if (clusterStorageOnlyNodeCollection == null || ClusterItem.CachingDisabled)
		{
			clusterStorageOnlyNodeCollection = new ClusterStorageOnlyNodeCollection();
			ClusterFunctionalLevel functionalLevel = GetFunctionalLevel();
			if (ClusterFunctionalLevel.Redstone <= functionalLevel)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _ClusStorageEnclosureInfoArray clusStorageEnclosureInfoArray);
				// IL initblk instruction
				System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref clusStorageEnclosureInfoArray, 0, 16);
				Monitor.Enter(m_storageOnlyNodeLock);
				try
				{
					uint num = 0u;
					num = global::_003CModule_003E.ClusterGetClusterStorageEnclosureObjects(Handle, (CLUSTER_STORAGE_ENCLOSURE_ENUM_TYPE)2, null, &clusStorageEnclosureInfoArray, 0u);
					if (num != 0)
					{
						DebugLog.LogError("There was an error querying for cluster storage enclosures. The error is '{0}'.", num);
						ClusApiExceptionFactory.CreateAndThrow(this, (int)num);
					}
					else
					{
						for (uint num2 = 0u; num2 < (uint)(*(int*)(&clusStorageEnclosureInfoArray)); num2++)
						{
							clusterStorageOnlyNodeCollection.InternalAdd(new ClusterStorageOnlyNode(this, (_ClusStorageEnclosureInfo*)((long)num2 * 136L + System.Runtime.CompilerServices.Unsafe.As<_ClusStorageEnclosureInfoArray, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref clusStorageEnclosureInfoArray, 8)))));
						}
					}
					m_storageOnlyNodes = clusterStorageOnlyNodeCollection;
				}
				catch (Exception innerException)
				{
					throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
					{
						Resources.Cluster_GetNodes_Failed_Text,
						m_name
					});
				}
				finally
				{
					Monitor.Exit(m_storageOnlyNodeLock);
					global::_003CModule_003E.FreeClusStorageEnclosureInfoArray(&clusStorageEnclosureInfoArray);
				}
			}
		}
		return clusterStorageOnlyNodeCollection;
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

	public IEnumerable<Tuple<Guid, string>> GetAllGroupsGuidAndName()
	{
		//Discarded unreachable code: IL_0086, IL_00a9, IL_00cc, IL_00ce, IL_00d0, IL_00dc
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeClusterEnumHandle safeClusterEnumHandle = null;
		List<Tuple<Guid, string>> list = new List<Tuple<Guid, string>>();
		try
		{
			safeClusterEnumHandle = NativeMethods.ClusterOpenEnum(this, ClusterEnumType.Group, SafeClusterEnumHandleOptions.None);
			while (safeClusterEnumHandle.MoveNext())
			{
				ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeClusterEnumHandle.Current;
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
			safeClusterEnumHandle?.Close();
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

	public Dictionary<Guid, string> GetCurrentGroupNames()
	{
		Dictionary<Guid, string> groupsIdNamePairs = new Dictionary<Guid, string>();
		return GetGroupNames(groupsIdNamePairs, unique: false);
	}

	public Dictionary<Guid, string> GetUniqueGroupNames()
	{
		Dictionary<Guid, string> groupsIdNamePairs = new Dictionary<Guid, string>();
		return GetGroupNames(groupsIdNamePairs, unique: true);
	}

	public Dictionary<Guid, ClusterGroup> GetCoreGroups()
	{
		Dictionary<Guid, ClusterGroup> dictionary = new Dictionary<Guid, ClusterGroup>();
		Guid id = GetCoreClusterGroup().Id;
		dictionary.Add(id, GetCoreClusterGroup());
		Guid id2 = GetAvailableStorageGroup().Id;
		dictionary.Add(id2, GetAvailableStorageGroup());
		return dictionary;
	}

	public ClusterResourceCollection GetResources(string resourceType)
	{
		//Discarded unreachable code: IL_0040, IL_0042
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterResourceType.CreateObject(this, resourceType).GetResources();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				m_name
			});
		}
	}

	public ClusterResourceCollection GetResources()
	{
		//Discarded unreachable code: IL_007d, IL_007f
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceCollection clusterResourceCollection = m_resources;
		if (clusterResourceCollection == null || ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_resourcesLock);
			try
			{
				clusterResourceCollection = m_resources;
				if (clusterResourceCollection == null || ClusterItem.CachingDisabled)
				{
					clusterResourceCollection = (m_resources = new ClusterResourceCollection(BuildResourceAsyncEnum()));
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetResources_Failed_Text,
					m_name
				});
			}
			finally
			{
				Monitor.Exit(m_resourcesLock);
			}
		}
		return clusterResourceCollection;
	}

	public AsyncEnumerationStatus GetResourcesAsync(AsyncEnumerationCallback<ClusterResource> callback)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceCollection resources = m_resources;
		AsyncEnumeration<ClusterResource> asyncEnumeration;
		if (resources != null && !ClusterItem.CachingDisabled)
		{
			asyncEnumeration = new AsyncEnumeration<ClusterResource>(resources);
		}
		else
		{
			asyncEnumeration = BuildResourceAsyncEnum();
			asyncEnumeration.EnumerationComplete += OnResourceAsyncEnumCompleted;
		}
		asyncEnumeration.SetCallback(callback);
		return asyncEnumeration.StartEnumeration(useDifferentThread: true);
	}

	public Dictionary<Guid, string> GetCurrentResourceNames()
	{
		Dictionary<Guid, string> resourcesIdNamePairs = new Dictionary<Guid, string>();
		return GetResourceNames(resourcesIdNamePairs, unique: false);
	}

	public Dictionary<Guid, string> GetUniqueResourceNames()
	{
		Dictionary<Guid, string> resourcesIdNamePairs = new Dictionary<Guid, string>();
		return GetResourceNames(resourcesIdNamePairs, unique: true);
	}

	public ClusterResourceTypeCollection GetResourceTypes()
	{
		//Discarded unreachable code: IL_007d, IL_007f
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceTypeCollection clusterResourceTypeCollection = m_resourceTypes;
		if (clusterResourceTypeCollection == null || ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_resourceTypesLock);
			try
			{
				clusterResourceTypeCollection = m_resourceTypes;
				if (clusterResourceTypeCollection == null || ClusterItem.CachingDisabled)
				{
					clusterResourceTypeCollection = (m_resourceTypes = new ClusterResourceTypeCollection(BuildResourceTypeAsyncEnum()));
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetResourceTypes_Failed_Text,
					m_name
				});
			}
			finally
			{
				Monitor.Exit(m_resourceTypesLock);
			}
		}
		return clusterResourceTypeCollection;
	}

	public AsyncEnumerationStatus GetResourceTypesAsync(AsyncEnumerationCallback<ClusterResourceType> callback)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceTypeCollection resourceTypes = m_resourceTypes;
		AsyncEnumeration<ClusterResourceType> asyncEnumeration;
		if (resourceTypes != null && !ClusterItem.CachingDisabled)
		{
			asyncEnumeration = new AsyncEnumeration<ClusterResourceType>(resourceTypes);
		}
		else
		{
			asyncEnumeration = BuildResourceTypeAsyncEnum();
			asyncEnumeration.EnumerationComplete += OnResourceTypeAsyncEnumCompleted;
		}
		asyncEnumeration.SetCallback(callback);
		return asyncEnumeration.StartEnumeration(useDifferentThread: true);
	}

	public ClusterNetworkCollection GetNetworks()
	{
		//Discarded unreachable code: IL_007d, IL_007f
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterNetworkCollection clusterNetworkCollection = m_networks;
		if (clusterNetworkCollection == null || ClusterItem.CachingDisabled)
		{
			Monitor.Enter(m_networksLock);
			try
			{
				clusterNetworkCollection = m_networks;
				if (clusterNetworkCollection == null || ClusterItem.CachingDisabled)
				{
					clusterNetworkCollection = (m_networks = new ClusterNetworkCollection(BuildNetworkAsyncEnum()));
				}
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.Cluster_GetNetworks_Failed_Text,
					m_name
				});
			}
			finally
			{
				Monitor.Exit(m_networksLock);
			}
		}
		return clusterNetworkCollection;
	}

	public AsyncEnumerationStatus GetNetworksAsync(AsyncEnumerationCallback<ClusterNetwork> callback)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterNetworkCollection networks = m_networks;
		AsyncEnumeration<ClusterNetwork> asyncEnumeration;
		if (networks != null && !ClusterItem.CachingDisabled)
		{
			asyncEnumeration = new AsyncEnumeration<ClusterNetwork>(networks);
		}
		else
		{
			asyncEnumeration = BuildNetworkAsyncEnum();
			asyncEnumeration.EnumerationComplete += OnNetworkAsyncEnumCompleted;
		}
		asyncEnumeration.SetCallback(callback);
		return asyncEnumeration.StartEnumeration(useDifferentThread: true);
	}

	public ClusterResource GetClusterSharedVolumeResource(string sharedVolumeResourceName, Guid sharedVolumeResourceId)
	{
		//Discarded unreachable code: IL_0041, IL_0043
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterResource.CreateObject(this, sharedVolumeResourceName, sharedVolumeResourceId, null);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.ClusterGetSharedVolumeResourceFailedFormat_Text,
				m_name,
				sharedVolumeResourceName
			});
		}
	}

	public ClusterResource GetClusterSharedVolumeResource(string sharedVolumeResourceName)
	{
		return GetClusterSharedVolumeResource(sharedVolumeResourceName, Guid.Empty);
	}

	public string GetClusterSharedVolumeId(ClusterResource sharedVolume, string deviceId)
	{
		//Discarded unreachable code: IL_010b
		string text = null;
		ICollection<ClusterSharedVolumeInfo> collection = null;
		IEnumerator<ClusterSharedVolumeInfo> enumerator = null;
		ClusterSharedVolumeInfo clusterSharedVolumeInfo = null;
		Exception ex = null;
		if (sharedVolume == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolume" });
		}
		if (deviceId == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "deviceId" });
		}
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		text = string.Empty;
		try
		{
			collection = sharedVolume.PhysicalDisk_GetSharedVolumeInfo();
			enumerator = collection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					clusterSharedVolumeInfo = enumerator.Current;
					DebugLog.LogInfo("GetClusterSharedVolumeId(ClusterResource^, String^): deviceId = {0}, info->DeviceId = {1}", deviceId, clusterSharedVolumeInfo.DeviceId);
					if (string.Compare(deviceId, clusterSharedVolumeInfo.DeviceId, StringComparison.Ordinal) == 0)
					{
						text = BuildSharedVolumeId(sharedVolume, clusterSharedVolumeInfo.VolumeOffset);
						break;
					}
				}
			}
			finally
			{
				IEnumerator<ClusterSharedVolumeInfo> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			ExceptionHelp.LogException(ex2, "Could not get the shared volume id from resource '{0}' and deviceId '{1}'", sharedVolume.DisplayName, deviceId);
			throw ExceptionHelp.Build<ApplicationException>(ex2, new string[2]
			{
				Resources.Cluster_GetSharedVolumeId_From_Resource_Failed_Format_Text,
				sharedVolume.DisplayName
			});
		}
		return text;
	}

	public unsafe string GetClusterSharedVolumeId(string path)
	{
		//Discarded unreachable code: IL_0073, IL_009c
		if (path == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "path" });
		}
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ControlExecutor controlExecutor = GetControlExecutor();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BAA_0040G _0024ArrayType_0024_0024_0024BY0BAA_0040G);
		UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&_0024ArrayType_0024_0024_0024BY0BAA_0040G, 512uL);
		UnmanagedBuffer inputBuffer = UnmanagedBuffer.Create(path);
		try
		{
			controlExecutor.ExecuteInOutControl(117441169u, inputBuffer, outputBuffer);
		}
		catch (Exception ex)
		{
			try
			{
				ValidateSystemDrives();
			}
			catch (ClusterSystemDriveValidationException caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Could not get the shared volume id since the %SystemDrive% path is not the same across all of the cluster nodes.");
				throw;
			}
			ExceptionHelp.LogException(ex, "Could not get the shared volume id that contians path: '{0}'", path);
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Cluster_GetSharedVolumeId_Failed_Format_Text,
				path
			});
		}
		return new string((char*)(&_0024ArrayType_0024_0024_0024BY0BAA_0040G));
	}

	public void ValidateSystemDrives()
	{
		string text = null;
		foreach (ClusterNode node in GetNodes())
		{
			if (text == null)
			{
				text = node.GetSystemDrive();
			}
			else if (string.Compare(text, node.GetSystemDrive(), StringComparison.Ordinal) != 0)
			{
				throw ExceptionHelp.Build<ClusterSystemDriveValidationException>(new string[0]);
			}
		}
	}

	public long GetNodeCount()
	{
		//Discarded unreachable code: IL_0030, IL_0032
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return GetItemCount(ClusterEnumType.Node);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetNodeCount_Failed_Text,
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
			return GetItemCount(ClusterEnumType.Group, SafeClusterEnumHandleOptions.NoCoreGroups);
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

	public long GetResourceCount()
	{
		//Discarded unreachable code: IL_0030, IL_0032
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return GetItemCount(ClusterEnumType.Resource);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetResourceCount_Failed_Text,
				m_name
			});
		}
	}

	public long GetResourceTypeCount()
	{
		//Discarded unreachable code: IL_0030, IL_0032
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return GetItemCount(ClusterEnumType.ResourceType);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetResourceTypeCount_Failed_Text,
				m_name
			});
		}
	}

	public long GetNetworkCount()
	{
		//Discarded unreachable code: IL_0031, IL_0033
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return GetItemCount(ClusterEnumType.Network);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetNetworkCount_Failed_Text,
				m_name
			});
		}
	}

	public unsafe string GetClusterCoreGroupName(Guid groupId)
	{
		string empty = string.Empty;
		using SafeGroupHandle safeGroupHandle = NativeMethods.OpenClusterGroup(this, groupId.ToString());
		HKEY__* clusterGroupKey = global::_003CModule_003E.GetClusterGroupKey(safeGroupHandle.DangerousGetGroupHandle(), ClusterRegistryKey.RegistryRightsToRegSam(RegistryRights.QueryValues));
		uint lastError = global::_003CModule_003E.GetLastError();
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterGroupKey);
		if (safeRegistryHandle.IsInvalid)
		{
			throw ExceptionHelp.Build((int)lastError, Resources.ClusterGroup_GetRegistryKeyFailed_Text, groupId.ToString());
		}
		return (string)new ClusterRegistryKey(this, safeRegistryHandle).GetValue("Name");
	}

	public ClusterNode GetNode(string nodeName, Guid nodeId)
	{
		//Discarded unreachable code: IL_0040, IL_0042
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterNode.CreateObject(this, nodeName, nodeId);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Cluster_GetNode_Failed_Text,
				m_name,
				nodeName
			});
		}
	}

	public ClusterNode GetNode(string nodeName)
	{
		return GetNode(nodeName, Guid.Empty);
	}

	public ClusterGroup GetGroup(string groupName, Guid groupId)
	{
		//Discarded unreachable code: IL_0040, IL_0042
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterGroup.CreateObject(this, groupName, groupId);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Cluster_GetGroup_Failed_Text,
				m_name,
				groupName
			});
		}
	}

	public ClusterGroup GetGroup(string groupName)
	{
		return GetGroup(groupName, Guid.Empty);
	}

	public ClusterResourceType GetResourceType(string resourceTypeName, Guid resourceId)
	{
		//Discarded unreachable code: IL_0040, IL_0042
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterResourceType.CreateObject(this, resourceTypeName, resourceId);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Cluster_GetResourceType_Failed_Text,
				m_name,
				resourceTypeName
			});
		}
	}

	public ClusterResourceType GetResourceType(string resourceTypeName)
	{
		return GetResourceType(resourceTypeName, Guid.Empty);
	}

	public ClusterNetwork GetNetwork(string networkName, Guid resourceId)
	{
		//Discarded unreachable code: IL_0040, IL_0042
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return ClusterNetwork.CreateObject(this, networkName, resourceId);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Cluster_GetNetwork_Failed_Text,
				m_name,
				networkName
			});
		}
	}

	public ClusterNetwork GetNetwork(string networkName)
	{
		return GetNetwork(networkName, Guid.Empty);
	}

	public override void Close()
	{
		if (!m_closed)
		{
			m_closed = true;
			OnConnectionChanged(ClusterConnectionState.Closing);
			((IDisposable)this)?.Dispose();
		}
	}

	public override ControlExecutor GetControlExecutor()
	{
		return new ClusterControlExecutor(this);
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
				Resources.Cluster_GetCommonProperties_Failed_Text,
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
				Resources.Cluster_GetPrivateProperties_Failed_Text,
				m_name
			});
		}
	}

	public override string ToString()
	{
		return Name;
	}

	public unsafe ClusterRegistryKey GetRegistryKey(RegistryRights rights)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		HKEY__* clusterKey = global::_003CModule_003E.GetClusterKey(Handle, ClusterRegistryKey.RegistryRightsToRegSam(rights));
		uint lastError = global::_003CModule_003E.GetLastError();
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterKey);
		if (safeRegistryHandle.IsInvalid)
		{
			ClusApiExceptionFactory.CreateAndThrow(this, (int)lastError, Resources.Cluster_GetRegistryKeyFailed_Text, m_name);
		}
		return new ClusterRegistryKey(this, safeRegistryHandle);
	}

	public ClusterResourceCollection GetAllStorage([MarshalAs(UnmanagedType.U1)] bool includePoolResource)
	{
		//Discarded unreachable code: IL_00e1, IL_00e3
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceCollection clusterResourceCollection = new ClusterResourceCollection();
		try
		{
			IEnumerator<ClusterResourceType> enumerator = GetResourceTypes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResourceType current = enumerator.Current;
					if (!current.IsStorage)
					{
						if (!includePoolResource)
						{
							continue;
						}
						string text = "Storage Pool";
						if (!(current.Name == text))
						{
							continue;
						}
					}
					IEnumerator<ClusterResource> enumerator2 = GetResources(current.Name).GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ClusterResource current2 = enumerator2.Current;
							clusterResourceCollection.InternalAdd(current2);
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
			return clusterResourceCollection;
		}
		catch (Exception innerException)
		{
			if (clusterResourceCollection is IDisposable disposable3)
			{
				disposable3.Dispose();
			}
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetAllStorage_Failed_Text,
				m_name
			});
		}
	}

	public ClusterResourceCollection GetAllStorage()
	{
		return GetAllStorage(includePoolResource: false);
	}

	public ClusterResourceCollection GetAllPhysicalDisks()
	{
		//Discarded unreachable code: IL_0047, IL_0049
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			string resourceType = "Physical Disk";
			return ProcessDiskCollection(GetResources(resourceType));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetAllStorage_Failed_Text,
				m_name
			});
		}
	}

	public ClusterResourceCollection GetNetworkClassResources()
	{
		//Discarded unreachable code: IL_00c7, IL_00c9
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceCollection clusterResourceCollection = new ClusterResourceCollection();
		try
		{
			IEnumerator<ClusterResourceType> enumerator = GetResourceTypes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResourceType current = enumerator.Current;
					if (!current.IsNetwork)
					{
						continue;
					}
					IEnumerator<ClusterResource> enumerator2 = GetResources(current.Name).GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							ClusterResource current2 = enumerator2.Current;
							clusterResourceCollection.InternalAdd(current2);
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
			return clusterResourceCollection;
		}
		catch (Exception innerException)
		{
			if (clusterResourceCollection is IDisposable disposable3)
			{
				disposable3.Dispose();
			}
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetNetworkClassResources_Failed_Text,
				m_name
			});
		}
	}

	public void ValidateVcoName(string name)
	{
		GetCoreClusterNetworkName()?.ValidateVcoName(name);
	}

	public static ClusterSharedVolumeInfo GetClusterSharedVolumeInfo(ClusterResource sharedVolume, string deviceId)
	{
		//Discarded unreachable code: IL_00be
		if (sharedVolume == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolume" });
		}
		if (deviceId == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "deviceId" });
		}
		ThreadWatchdog.PerformUIThreadCheck();
		ClusterSharedVolumeInfo result = null;
		try
		{
			IEnumerator<ClusterSharedVolumeInfo> enumerator = sharedVolume.PhysicalDisk_GetSharedVolumeInfo().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterSharedVolumeInfo current = enumerator.Current;
					if (string.Compare(deviceId, current.DeviceId, StringComparison.Ordinal) == 0)
					{
						result = current;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<ClusterSharedVolumeInfo> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Could not get the shared volume info from resource '{0}' that has deviceId '{1}'", sharedVolume.DisplayName, deviceId);
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Cluster_GetSharedVolumeInfo_From_Resource_Failed_Format_Text,
				sharedVolume.DisplayName
			});
		}
		return result;
	}

	public ClusterResource GetResourceFromSharedVolumeId(string sharedVolumeId)
	{
		if (sharedVolumeId == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolumeId" });
		}
		string[] array = sharedVolumeId.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
		string resourceName = (((nint)array.LongLength >= 2) ? array[0] : sharedVolumeId);
		return GetResource(resourceName, Guid.Empty, null);
	}

	public void SuspendClusterSharedVolume(ClusterResource sharedVolume, string deviceId)
	{
		if (sharedVolume == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolume" });
		}
		if (deviceId == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "deviceId" });
		}
		ClusterSharedVolumeInfo clusterSharedVolumeInfo = GetClusterSharedVolumeInfo(sharedVolume, deviceId);
		string sharedVolumeId = BuildSharedVolumeId(sharedVolume, clusterSharedVolumeInfo.VolumeOffset);
		IList<ClusterGroup> groupsThatDependOnSharedVolumeById = GetGroupsThatDependOnSharedVolumeById(sharedVolumeId);
		InternalSuspendClusterSharedVolume(sharedVolume, groupsThatDependOnSharedVolumeById, clusterSharedVolumeInfo);
	}

	public void ResumeClusterSharedVolume(ClusterResource sharedVolume, string deviceId)
	{
		if (sharedVolume == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolume" });
		}
		if (deviceId == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "deviceId" });
		}
		ClusterSharedVolumeInfo clusterSharedVolumeInfo = GetClusterSharedVolumeInfo(sharedVolume, deviceId);
		InternalResumeClusterSharedVolume(sharedVolume, clusterSharedVolumeInfo);
	}

	public IList<ClusterGroup> GetGroupsThatDependOnSharedVolumeByPath(string path)
	{
		if (path == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "path" });
		}
		string clusterSharedVolumeId = GetClusterSharedVolumeId(path);
		return GetGroupsThatDependOnSharedVolumeById(clusterSharedVolumeId);
	}

	public IList<ClusterGroup> GetGroupsThatDependOnSharedVolumeById(string sharedVolumeId)
	{
		if (sharedVolumeId == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolumeId" });
		}
		List<ClusterGroup> list = new List<ClusterGroup>();
		string resourceType = "Virtual Machine Configuration";
		foreach (ClusterResource resource in GetResources(resourceType))
		{
			if (DoesResourceDependOnClusterSharedVolume(resource, sharedVolumeId))
			{
				list.Add(resource.GetOwnerGroup());
			}
		}
		return list;
	}

	public ClusterResourceTypeCollection GetStorageClassResourceTypes()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ClusterResourceTypeCollection clusterResourceTypeCollection = new ClusterResourceTypeCollection();
		foreach (ClusterResourceType resourceType in GetResourceTypes())
		{
			if (resourceType.IsStorage)
			{
				clusterResourceTypeCollection.InternalAdd(resourceType);
			}
		}
		return clusterResourceTypeCollection;
	}

	public static string BuildSharedVolumeId(ClusterResource resource, ulong volumeOffset)
	{
		Guid id = resource.Id;
		return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", id, volumeOffset);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool WaitForClusterToAttainQuorum(ICollection<string> nodeNames, int timeToWait)
	{
		bool flag = false;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		while (true)
		{
			foreach (string nodeName in nodeNames)
			{
				_HCLUSTER* ptr = OpenClusterInternal(nodeName);
				if (ptr == null)
				{
					continue;
				}
				try
				{
					IEnumerator<string> enumerator2 = nodeNames.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							string current = enumerator2.Current;
							_HNODE* ptr2 = OpenClusterNodeInternal(ptr, current);
							if (ptr2 == null)
							{
								continue;
							}
							try
							{
								if (global::_003CModule_003E.GetClusterNodeState(ptr2) == (CLUSTER_NODE_STATE)0)
								{
									flag = true;
									break;
								}
							}
							finally
							{
								global::_003CModule_003E.CloseClusterNode(ptr2);
							}
						}
					}
					finally
					{
						IEnumerator<string> enumerator3 = enumerator2;
						IDisposable disposable = enumerator2;
						enumerator2?.Dispose();
					}
					if (flag)
					{
						break;
					}
				}
				finally
				{
					global::_003CModule_003E.CloseCluster(ptr);
				}
			}
			if (flag || stopwatch.ElapsedMilliseconds > timeToWait)
			{
				break;
			}
			Thread.Sleep(1000);
		}
		return flag;
	}

	[HandleProcessCorruptedStateExceptions]
	protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			try
			{
				_007ECluster();
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

