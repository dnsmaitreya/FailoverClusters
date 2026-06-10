#define DEBUG
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using _003CCppImplementationDetails_003E;
using FailoverClusters.NativeHelp;
using FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public class ClusterResource : ClusterItem
{
	private volatile string m_name;

	private volatile string m_lastLoadedName;

	private volatile string m_displayName;

	private int m_loadingName;

	private Guid m_Id;

	private volatile string m_ownerNodeName;

	private volatile ClusterGroup m_ownerGroup;

	private volatile ClusterGroup m_previousOwnerGroup;

	private ResourceState m_state;

	private ResourceControlExecutor m_controlExecutor;

	private bool proxyResource;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private volatile string m_resourceTypeName;

	private volatile uint m_characteristics;

	private object m_nameLockObject;

	private object m_stateLockObject;

	private object m_classInfoLockObject;

	private unsafe _HRESOURCE* proxyHandle;

	private ClusterResourceClass? m_clusResClass;

	private volatile uint m_clusResSubClass;

	private static object m_creationLockObject = new object();

	private ResourceFlags? m_flags;

	private object m_flagsLock;

	private object m_displayNameLock;

	private bool m_closed;

	private object m_cachedSharedVolumeInfoLockObject;

	private ICollection<ClusterSharedVolumeInfo> m_cachedSharedVolumeInfo;

	private bool? m_cachedDiskIsMaintenanceMode;

	private PropertyCollection m_commonPropertyCollection;

	private object m_commonPropertyCollectionLockObject;

	private PropertyCollection m_privatePropertyCollection;

	private object m_privatePropertyCollectionLockObject;

	private volatile ClusterDisk m_disk;

	private bool diskIncludesMountPoints;

	private object m_diskLock;

	private TimeSpan StateChangeTimeout;

	private static uint PhysicalDiskPassThroughMode = 6u;

	private static uint PhysicalDiskNormalMode = 0u;

	private Cluster m_cluster;

	private SafeResourceHandle m_hResource;

	private ReaderWriterLockSlim m_hResourceLock;

	private bool m_isCsv;

	private Guid? m_poolId;

	private EventHandler _003Cbacking_store_003EStateChanged;

	private EventHandler _003Cbacking_store_003EPropertiesChanged;

	private EventHandler _003Cbacking_store_003EDeleted;

	public const uint LooksAlivePollIntervalMaximum = uint.MaxValue;

	public const uint LooksAlivePollIntervalMinimum = 10u;

	public const uint IsAlivePollIntervalMaximum = uint.MaxValue;

	public const uint IsAlivePollIntervalMinimum = 10u;

	public const uint PendingTimeoutMinimum = 10u;

	public const uint PendingTimeoutDefault = 180000u;

	public const uint FileShareMaxUsersMaximum = uint.MaxValue;

	public const uint FileShareMaxUsersMinimum = 1u;

	public const uint RetryPeriodOnFailureMaximum = uint.MaxValue;

	public const uint SeparateMonitorDefault = 0u;

	public const string VirtualMachine_OfflineAction = "OfflineAction";

	public const string VirtualMachine_PreviousOfflineAction = "PreviousOfflineAction";

	public const string VirtualMachineConfiguration_DependsOnSharedVolumes = "DependsOnSharedVolumes";

	public const uint Shi1005FlagsDfs = 1u;

	public const uint Shi1005FlagsDfsRoot = 2u;

	public const uint CscMask = 48u;

	public const uint CscCacheManualReint = 0u;

	public const uint CscCacheAutoReint = 16u;

	public const uint CscCacheVdo = 32u;

	public const uint CscCacheNone = 48u;

	public const uint Shi1005FlagsRestrictExclusiveOpens = 256u;

	public const uint Shi1005FlagsForceSharedDelete = 512u;

	public const uint Shi1005FlagsAllowNamespaceCaching = 1024u;

	public const uint Shi1005FlagsAccessBasedDirectoryEnum = 2048u;

	public bool IsSymmetric
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return GetSymmetry();
		}
	}

	public bool IsValidForQuorumResource
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsThisValidForQuorumResource();
		}
	}

	public bool IsQuorumResource
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsThisQuorumResource();
		}
	}

	public bool IsNetwork
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsNetworkResource();
		}
	}

	public bool IsClusterSharedVolume
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			if (proxyResource)
			{
				return m_isCsv;
			}
			return m_cluster.IsClusterSharedVolumeResource(this);
		}
	}

	public bool IsSpace
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsSpaceResource();
		}
	}

	public Guid PoolId => GetPoolId();

	public bool IsReplicationChain
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsReplicationChainResource();
		}
	}

	public bool IsReplication
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsReplicationResource();
		}
	}

	public bool IsStorage
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsStorageResource();
		}
	}

	public string ResourceTypeName
	{
		get
		{
			LoadResourceTypeName();
			return m_resourceTypeName;
		}
	}

	public Cluster Cluster => m_cluster;

	public bool IsInfrastructureResource
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((GetCharacteristics() >> 17) & 1u) != 0;
		}
	}

	public bool IsCoreResource
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)(GetFlags() & ResourceFlags.Core) != 0;
		}
	}

	public ResourceFlags Flags => GetFlags();

	public ResourceState State => GetState();

	public string OwnerNodeName => GetOwnerNodeName();

	public override bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_lifetimeHelper.IsDeleted;
		}
	}

	public override Guid Id => m_Id;

	public unsafe static string PhysicalDiskSignature => InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BM_0040NENPIHJA_0040_003F_0024AAD_003F_0024AAi_003F_0024AAs_003F_0024AAk_003F_0024AAS_003F_0024AAi_003F_0024AAg_003F_0024AAn_003F_0024AAa_003F_0024AAt_003F_0024AAu_003F_0024AAr_003F_0024AAe_0040));

	public string DisplayName
	{
		get
		{
			//Discarded unreachable code: IL_0055
			ThreadWatchdog.PerformUIThreadCheck();
			Monitor.Enter(m_displayNameLock);
			try
			{
				string text = m_displayName;
				if (ClusterItem.CachingDisabled && !IsDeleted)
				{
					text = null;
				}
				while (text == null)
				{
					text = GenerateDisplayName();
				}
				m_displayName = text;
				return text;
			}
			finally
			{
				Monitor.Exit(m_displayNameLock);
			}
		}
	}

	public string LastKnownName => m_lastLoadedName;

	public override string Name
	{
		get
		{
			Property property = null;
			Exception ex = null;
			string result = m_lastLoadedName;
			if (Interlocked.CompareExchange(ref m_loadingName, 1, 0) == 0)
			{
				try
				{
					if (!IsDeleted && (m_name == null || ClusterItem.CachingDisabled))
					{
						ThreadWatchdog.PerformUIThreadCheck();
						property = null;
						try
						{
							if (GetCommonProperties(PropertyCollectionSet.ReadOnly).TryGetProperty("Name", out property) && SetName((string)property.Value))
							{
								result = m_name;
								try
								{
									raise_PropertiesChanged(this, EventArgs.Empty);
								}
								catch (Exception exception)
								{
									DebugLog.LogException(exception, "There was an error from the call to raising event 'PropertiesChanged' in the ClusterGroup object");
								}
							}
						}
						catch (ClusterObjectDeletedException)
						{
							return m_lastLoadedName;
						}
					}
				}
				finally
				{
					Interlocked.Exchange(ref m_loadingName, 0);
				}
			}
			return result;
		}
	}

	public SafeHandle ResourceHandle
	{
		get
		{
			//Discarded unreachable code: IL_001d
			ThreadWatchdog.PerformUIThreadCheck();
			LockAccessToHandle(writeAccess: false);
			try
			{
				return m_hResource;
			}
			finally
			{
				UnlockAccessToHandle(writeAccess: false);
			}
		}
	}

	internal unsafe _HRESOURCE* Handle
	{
		get
		{
			//Discarded unreachable code: IL_0037
			//IL_0041: Expected I, but got I8
			if (proxyResource)
			{
				return proxyHandle;
			}
			m_lifetimeHelper.CheckObjectState();
			LockAccessToHandle(writeAccess: false);
			try
			{
				return m_hResource.DangerousGetResourceHandle();
			}
			finally
			{
				UnlockAccessToHandle(writeAccess: false);
			}
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

	private PropertyCollection GetDesiredCollection(PropertyCollection propertyCollection, PropertyCollectionSet propSet)
	{
		PropertyCollection propertyCollection2 = new PropertyCollection(propertyCollection);
		foreach (Property item in propertyCollection)
		{
			if (propSet == PropertyCollectionSet.Both || (item.IsReadOnly && propSet == PropertyCollectionSet.ReadOnly) || (!item.IsReadOnly && propSet == PropertyCollectionSet.ReadWrite))
			{
				propertyCollection2.Add(new Property(item));
			}
		}
		return propertyCollection2;
	}

	public unsafe ClusterResource(IntPtr clusterHandle, IntPtr handle, ClusterResourceClass resourceClass, string resourceTypeName, ResourceState resourceState, string ownerNode, [MarshalAs(UnmanagedType.U1)] bool isCsv)
	{
		try
		{
			m_lifetimeHelper = new ObjectLifetimeHelper();
			m_classInfoLockObject = new object();
			ClusterResourceClass? clusResClass = resourceClass;
			m_clusResClass = clusResClass;
			m_clusResSubClass = 0u;
			m_ownerNodeName = ownerNode;
			m_resourceTypeName = resourceTypeName;
			m_diskLock = new object();
			m_stateLockObject = new object();
			m_cachedSharedVolumeInfoLockObject = new object();
			proxyResource = true;
			proxyHandle = (_HRESOURCE*)handle.ToPointer();
			m_state = resourceState;
			m_controlExecutor = new ResourceControlExecutor(this, m_cluster = new Cluster(clusterHandle));
			m_isCsv = isCsv;
			m_closed = false;
			m_poolId = null;
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(A_0: true);
			throw;
		}
	}

	private ClusterResource(Cluster cluster, SafeResourceHandle hResource, Guid id, string resourceName, ClusterGroup group)
	{
		try
		{
			if (cluster == null)
			{
				throw new ArgumentNullException("cluster");
			}
			m_state = ResourceState.Unknown;
			m_ownerGroup = group;
			m_ownerNodeName = null;
			m_lastLoadedName = resourceName;
			m_name = resourceName;
			m_displayName = null;
			m_displayNameLock = new object();
			m_Id = id;
			m_hResourceLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			m_hResource = hResource;
			m_cluster = cluster;
			m_lifetimeHelper = new ObjectLifetimeHelper();
			TimeSpan stateChangeTimeout = new TimeSpan(0, 3, 0);
			StateChangeTimeout = stateChangeTimeout;
			m_characteristics = uint.MaxValue;
			m_nameLockObject = new object();
			m_stateLockObject = new object();
			m_classInfoLockObject = new object();
			m_cachedSharedVolumeInfoLockObject = new object();
			m_clusResClass = null;
			m_clusResSubClass = 0u;
			m_flagsLock = new object();
			m_flags = null;
			m_commonPropertyCollection = null;
			m_commonPropertyCollectionLockObject = new object();
			m_privatePropertyCollection = null;
			m_privatePropertyCollectionLockObject = new object();
			m_disk = null;
			diskIncludesMountPoints = false;
			m_diskLock = new object();
			m_cachedDiskIsMaintenanceMode = null;
			m_controlExecutor = new ResourceControlExecutor(this, m_cluster);
			m_cluster.ObjectMgr.RegisterInstance(this);
			if (Utilities.IsGuid(resourceName))
			{
				SetName(null);
				ClearDisplayName();
			}
			LoadResourceTypeName();
			if (!ClusterItem.CachingDisabled)
			{
				m_cluster.RegistryChanged += OnClusterRegistryChanged;
			}
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(A_0: true);
			throw;
		}
	}

	private static SafeResourceHandle OpenResource(Cluster cluster, string resourceName)
	{
		return NativeMethods.OpenClusterResource(cluster, resourceName);
	}

	private void LockAccessToHandle([MarshalAs(UnmanagedType.U1)] bool writeAccess)
	{
		if (writeAccess)
		{
			m_hResourceLock.EnterWriteLock();
			return;
		}
		m_hResourceLock.EnterReadLock();
		if (m_hResource == null)
		{
			m_hResourceLock.ExitReadLock();
			ClusApiExceptionFactory.ThrowObjectDeletedException();
		}
	}

	private void UnlockAccessToHandle([MarshalAs(UnmanagedType.U1)] bool writeAccess)
	{
		if (writeAccess)
		{
			m_hResourceLock.ExitWriteLock();
		}
		else
		{
			m_hResourceLock.ExitReadLock();
		}
	}

	private static Guid GetId(Cluster cluster, SafeResourceHandle hResource)
	{
		ResourceControlExecutor resourceControlExecutor = new ResourceControlExecutor(hResource, cluster);
		return resourceControlExecutor.GetId(resourceControlExecutor);
	}

	private ResourceState GetState()
	{
		if (proxyResource)
		{
			return m_state;
		}
		ThreadWatchdog.PerformUIThreadCheck();
		ResourceState resourceState = m_state;
		if (ClusterItem.CachingDisabled || IsDeleted)
		{
			resourceState = ResourceState.Unknown;
		}
		if (resourceState == ResourceState.Unknown)
		{
			while (!IsDeleted)
			{
				LoadState(loadOwnerGroup: false);
				resourceState = m_state;
				if (resourceState != ResourceState.Unknown)
				{
					break;
				}
			}
		}
		return resourceState;
	}

	private string GetOwnerNodeName()
	{
		if (proxyResource)
		{
			return m_ownerNodeName;
		}
		ThreadWatchdog.PerformUIThreadCheck();
		string text = m_ownerNodeName;
		if (ClusterItem.CachingDisabled && !IsDeleted)
		{
			text = null;
		}
		if (text == null)
		{
			do
			{
				LoadState(loadOwnerGroup: false);
				text = m_ownerNodeName;
			}
			while (text == null);
		}
		return text;
	}

	private void ResetState()
	{
		Monitor.Enter(m_stateLockObject);
		try
		{
			m_state = ResourceState.Unknown;
			if (m_ownerGroup != null)
			{
				m_ownerGroup.StateChanged -= OnOwnerGroupStateChanged;
				m_ownerGroup = null;
			}
			m_ownerNodeName = null;
		}
		finally
		{
			Monitor.Exit(m_stateLockObject);
		}
	}

	private unsafe void LoadState([MarshalAs(UnmanagedType.U1)] bool loadOwnerGroup)
	{
		//Discarded unreachable code: IL_016f, IL_0171
		string text = null;
		string text2 = null;
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		if (IsDeleted)
		{
			throw ExceptionHelp.Build<ClusterObjectDeletedException>(new string[1] { m_lastLoadedName });
		}
		bool? flag = null;
		if (m_previousOwnerGroup != null)
		{
			flag = m_previousOwnerGroup.IsDeleted;
		}
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_stateLockObject);
		LockAccessToHandle(writeAccess: false);
		try
		{
			if (m_hResource != null && (m_state == ResourceState.Unknown || m_ownerGroup == null || m_ownerNodeName == null || ClusterItem.CachingDisabled))
			{
				_HRESOURCE* hResource = m_hResource.DangerousGetResourceHandle();
				text = null;
				text2 = null;
				Cluster cluster = m_cluster;
				m_state = GetResourceState(cluster, hResource, ref text2, ref text);
				m_ownerNodeName = text2;
				if (m_previousOwnerGroup != null && flag.HasValue && !flag.Value && m_previousOwnerGroup.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					SetOwnerGroupInternal(m_previousOwnerGroup);
				}
				else if (loadOwnerGroup)
				{
					Cluster cluster2 = m_cluster;
					SetOwnerGroupInternal(cluster2.GetGroup(text));
					m_previousOwnerGroup = m_ownerGroup;
				}
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_LoadState_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
			Monitor.Exit(m_stateLockObject);
		}
	}

	private void ResetName()
	{
		SetName(null);
		ClearDisplayName();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool SetName(string name)
	{
		//Discarded unreachable code: IL_0079
		Monitor.Enter(m_nameLockObject);
		try
		{
			if (string.Compare(m_lastLoadedName, name, ignoreCase: true, CultureInfo.CurrentCulture) == 0)
			{
				m_name = name;
				return false;
			}
			if (!string.IsNullOrEmpty(name))
			{
				m_lastLoadedName = name;
			}
			else if (!string.IsNullOrEmpty(m_name))
			{
				m_lastLoadedName = m_name;
			}
			m_name = name;
			return true;
		}
		finally
		{
			Monitor.Exit(m_nameLockObject);
		}
	}

	private void LoadResourceTypeName()
	{
		//Discarded unreachable code: IL_0078
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		if (!(m_resourceTypeName == null))
		{
			return;
		}
		LockAccessToHandle(writeAccess: false);
		try
		{
			if (m_resourceTypeName == null)
			{
				try
				{
					Property property = GetCommonProperties(PropertyCollectionSet.ReadWrite)["Type"];
					m_resourceTypeName = (string)property.Value;
					return;
				}
				catch (Exception innerException)
				{
					throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
					{
						Resources.Resource_LoadResourceType_Fail_Text,
						m_lastLoadedName
					});
				}
			}
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	private unsafe ResourceFlags GetFlags()
	{
		//Discarded unreachable code: IL_00b2, IL_00b4, IL_00b6, IL_00c4
		ThreadWatchdog.PerformUIThreadCheck();
		Monitor.Enter(m_flagsLock);
		try
		{
			if (!m_flags.HasValue)
			{
				if (IsDeleted)
				{
					return ResourceFlags.None;
				}
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
				UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&num, 4uL);
				m_controlExecutor.ExecuteOutControl(m_controlExecutor.GetFlagsCode, outputBuffer);
				ResourceFlags? flags = (ResourceFlags)num;
				m_flags = flags;
				DebugLog.LogVerbose("Retrieved flags for resource '{0}'. Call stack: {1}", m_name, DebugLog.GetStackTrace());
			}
			return m_flags.Value;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetFlags_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_flagsLock);
		}
	}

	private void ResetFlags()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Monitor.Enter(m_flagsLock);
		try
		{
			m_flags = null;
		}
		finally
		{
			Monitor.Exit(m_flagsLock);
		}
	}

	private void SetOwnerGroupInternal(ClusterGroup newGroup)
	{
		if (m_ownerGroup != null)
		{
			m_ownerGroup.StateChanged -= OnOwnerGroupStateChanged;
		}
		m_ownerGroup = newGroup;
		if (m_ownerGroup != null)
		{
			m_ownerGroup.StateChanged += OnOwnerGroupStateChanged;
		}
	}

	private unsafe StringCollection GetCheckPoints(uint dwControlCode)
	{
		//Discarded unreachable code: IL_00a4, IL_00a6
		//IL_006f: Expected I, but got I8
		//IL_007f: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		StringCollection stringCollection = null;
		UnmanagedBuffer unmanagedBuffer = null;
		try
		{
			unmanagedBuffer = m_controlExecutor.ExecuteOutControl(dwControlCode);
			stringCollection = new StringCollection();
			if (unmanagedBuffer.Size != 0)
			{
				ushort* pointer = (ushort*)unmanagedBuffer.Pointer;
				ushort* ptr = pointer;
				while (*ptr != 0 && (ulong)((long)((nint)((byte*)ptr - (nuint)pointer) >> 1) * 2L) <= unmanagedBuffer.Size)
				{
					stringCollection.Add(InteropHelp.WstrToString(ptr));
					ushort* ptr2 = ptr;
					while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0)
					{
						ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
					}
					ptr = (ushort*)((long)((nint)((byte*)ptr2 - (nuint)ptr) >> 1) * 2L + (nint)ptr + 2);
				}
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Get_Checkpoint_Failed_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			unmanagedBuffer?.Free();
		}
		return stringCollection;
	}

	private void WaitForDesiredState(ResourceState state, TimeSpan timeout, [MarshalAs(UnmanagedType.U1)] bool allowFailed)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		TimeSpan timeSpan = default(TimeSpan);
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, 250);
		bool flag = false;
		flag = GetState() == ResourceState.Failed || flag;
		if (timeSpan < timeout)
		{
			do
			{
				ThreadWatchdog.PerformUIThreadCheck();
				ResetState();
				ResourceState state2 = GetState();
				if (state2 == state)
				{
					break;
				}
				if (flag)
				{
					if (state2 == ResourceState.Failed)
					{
						break;
					}
					flag = false;
				}
				else if (state2 == ResourceState.Failed)
				{
					break;
				}
				Thread.Sleep(timeSpan2);
				timeSpan += timeSpan2;
			}
			while (timeSpan < timeout);
		}
		ResourceState state3 = GetState();
		if ((!allowFailed || state3 != ResourceState.Failed) && state3 != state)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.StateChange_Timeout_Text,
				m_lastLoadedName
			});
		}
	}

	private void WaitForStateChange(ResourceState oldState, TimeSpan timeout, [MarshalAs(UnmanagedType.U1)] bool allowFailed)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		TimeSpan timeSpan = default(TimeSpan);
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, 250);
		if (timeSpan < timeout)
		{
			do
			{
				ThreadWatchdog.PerformUIThreadCheck();
				ResetState();
				ResourceState state = GetState();
				if (state != oldState || state == ResourceState.Failed)
				{
					break;
				}
				Thread.Sleep(timeSpan2);
				timeSpan += timeSpan2;
			}
			while (timeSpan < timeout);
		}
		ResourceState state2 = GetState();
		if ((!allowFailed || state2 != ResourceState.Failed) && state2 == oldState)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.StateChange_Timeout_Text,
				m_lastLoadedName
			});
		}
	}

	private unsafe ClusterDisk GetDisk([MarshalAs(UnmanagedType.U1)] bool includeMountPoints)
	{
		uint num = 0u;
		if (GetResourceClass(&num) == ClusterResourceClass.Storage)
		{
			ThreadWatchdog.PerformUIThreadCheck();
			m_lifetimeHelper.CheckObjectState();
			Monitor.Enter(m_diskLock);
			try
			{
				if (ClusterItem.CachingDisabled || (!diskIncludesMountPoints && includeMountPoints))
				{
					m_disk = null;
				}
				if (m_disk == null)
				{
					m_disk = LoadDiskFromCluster(includeMountPoints);
				}
				return m_disk;
			}
			finally
			{
				Monitor.Exit(m_diskLock);
			}
		}
		return null;
	}

	private void ResetDisk()
	{
		Monitor.Enter(m_diskLock);
		try
		{
			m_disk = null;
			m_poolId = null;
			diskIncludesMountPoints = false;
		}
		finally
		{
			Monitor.Exit(m_diskLock);
		}
		Monitor.Enter(m_cachedSharedVolumeInfoLockObject);
		try
		{
			m_cachedSharedVolumeInfo = null;
		}
		finally
		{
			Monitor.Exit(m_cachedSharedVolumeInfoLockObject);
		}
		m_cachedDiskIsMaintenanceMode = null;
	}

	private unsafe ClusterDisk LoadDiskFromCluster([MarshalAs(UnmanagedType.U1)] bool includeMountPoints)
	{
		//Discarded unreachable code: IL_02ee
		//IL_0020: Expected I, but got I8
		//IL_0072: Expected I, but got I8
		//IL_0072: Expected I, but got I8
		//IL_0097: Expected I, but got I8
		//IL_0097: Expected I, but got I8
		//IL_00ff: Expected I, but got I8
		//IL_0129: Expected I, but got I8
		//IL_0129: Expected I, but got I8
		//IL_01f7: Expected I, but got I8
		ClusterDisk clusterDisk = null;
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
			if (!proxyResource && GetUncachedState() == ResourceState.Initializing)
			{
				WaitForStateChange(ResourceState.Initializing, StateChangeTimeout, allowFailed: true);
			}
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			uint num = global::_003CModule_003E.CClusPropValueList_002EScGetResourceValueList(ptr2, Handle, 16777713u, null, null, 0uL);
			if (num == 1)
			{
				ResourceControlExecutor controlExecutor2 = m_controlExecutor;
				num = global::_003CModule_003E.CClusPropValueList_002EScGetResourceValueList(ptr2, Handle, 16777617u, null, null, 0uL);
			}
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.GetDiskInfoFail_Text, m_lastLoadedName);
			}
			string ownerNodeName = GetOwnerNodeName();
			Collection<ClusterDisk> collection = ClusterDisk.ParseClusterableDisks(m_cluster, ownerNodeName, ptr2);
			if (collection.Count != 1)
			{
				throw ExceptionHelp.Build<ApplicationException>(new string[2]
				{
					Resources.GetClusterableDisksFail_Text,
					m_lastLoadedName
				});
			}
			clusterDisk = collection[0];
			CClusPropValueList* ptr4 = (CClusPropValueList*)global::_003CModule_003E.@new(48uL);
			CClusPropValueList* ptr5;
			try
			{
				ptr5 = ((ptr4 == null) ? null : global::_003CModule_003E.CClusPropValueList_002E_007Bctor_007D(ptr4));
				CClusPropValueList* ptr6 = ptr5;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.delete(ptr4);
				throw;
			}
			CClusPropValueList* ptr7 = ptr5;
			try
			{
				bool flag = false;
				num = global::_003CModule_003E.CClusPropValueList_002EScGetResourceValueList(ptr5, Handle, 16777753u, null, null, 0uL);
				switch (num)
				{
				default:
				{
					Cluster cluster = m_cluster;
					ClusApiExceptionFactory.CreateAndThrow(cluster, (int)num, Resources.GetDiskDirtyFail_Text, m_lastLoadedName);
					break;
				}
				case 0u:
				{
					Collection<int> collection2 = ParseDirtyList(ptr5);
					IEnumerator<ClusterDiskPartition> enumerator = clusterDisk.Partitions.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ClusterDiskPartition current = enumerator.Current;
							if (flag)
							{
								current.SetDirty(DirtyState.Unknown);
								continue;
							}
							DirtyState dirty = (collection2.Contains((int)current.PartitionNumber) ? DirtyState.Dirty : DirtyState.NotDirty);
							current.SetDirty(dirty);
						}
					}
					finally
					{
						IEnumerator<ClusterDiskPartition> enumerator2 = enumerator;
						IDisposable disposable = enumerator;
						enumerator?.Dispose();
					}
					break;
				}
				case 1u:
				case 2u:
				case 87u:
				case 5023u:
				{
					bool flag2 = true;
					break;
				}
				}
			}
			finally
			{
				CClusPropValueList* ptr8 = ptr7;
				CClusPropValueList* ptr9 = ptr7;
				if (ptr7 != null)
				{
					void* ptr10 = global::_003CModule_003E.CClusPropValueList_002E__delDtor(ptr7, 1u);
				}
				else
				{
					void* ptr10 = null;
				}
			}
			if (includeMountPoints && GetState() == ResourceState.Online)
			{
				IEnumerator<ClusterDiskPartition> enumerator3 = collection[0].Partitions.GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						ClusterDiskPartition current2 = enumerator3.Current;
						if (!current2.IsPartitionNumberValid)
						{
							continue;
						}
						uint partitionNumber = current2.PartitionNumber;
						UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&partitionNumber, 4uL);
						UnmanagedBuffer unmanagedBuffer = null;
						try
						{
							ResourceControlExecutor controlExecutor3 = m_controlExecutor;
							ResourceControlExecutor resourceControlExecutor = controlExecutor3;
							unmanagedBuffer = controlExecutor3.ExecuteInOutControl(16777745u, inputBuffer);
							StringCollection mountPoints = (StringCollection)TypeConverter.ConvertToManagedType(unmanagedBuffer.Pointer, unmanagedBuffer.Size, DataType.MultiString);
							current2.SetMountPoints(mountPoints);
							diskIncludesMountPoints = true;
						}
						catch (Exception caughtException)
						{
							Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
							if (firstException != null)
							{
								int num2 = -2147024895;
								if (firstException.NativeErrorCode == -2147024895)
								{
									ExceptionHelp.LogException(caughtException, "Resource '{0}' does not support mount points.", m_lastLoadedName);
								}
								else
								{
									ExceptionHelp.LogException(caughtException, "Resource '{0}' is in an invalid state to retrieve mountpoints.", m_lastLoadedName);
								}
								continue;
							}
							throw;
						}
					}
				}
				finally
				{
					IEnumerator<ClusterDiskPartition> enumerator4 = enumerator3;
					IDisposable disposable2 = enumerator3;
					enumerator3?.Dispose();
				}
			}
		}
		finally
		{
			if (ptr3 != null)
			{
				global::_003CModule_003E.CClusPropValueList_002EDeleteValueList(ptr3);
				global::_003CModule_003E.delete(ptr3);
			}
		}
		return clusterDisk;
	}

	private unsafe void AddRemoveCheckpoint(string checkPointName, uint dwControlCode)
	{
		//IL_0027: Expected I, but got I8
		//IL_003c: Expected I, but got I8
		//IL_0044: Expected I8, but got I
		ThreadWatchdog.PerformUIThreadCheck();
		if (checkPointName == null)
		{
			throw new ArgumentNullException("checkPointName");
		}
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(checkPointName);
			ushort* ptr2 = ptr;
			while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0)
			{
				ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
			}
			ulong num = (ulong)((nint)((byte*)ptr2 - (nuint)ptr) >> 1);
			UnmanagedBuffer inputBuffer = new UnmanagedBuffer(ptr, (num + 1) * 2);
			m_controlExecutor.ExecuteInControl(dwControlCode, inputBuffer);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsStorageResource()
	{
		uint num = 0u;
		return GetResourceClass(&num) == ClusterResourceClass.Storage;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsNetworkResource()
	{
		uint num = 0u;
		return GetResourceClass(&num) == ClusterResourceClass.Network;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsReplicationResource()
	{
		uint num = 0u;
		GetResourceClass(&num);
		return (byte)((num >> 28) & 1u) != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsReplicationChainResource()
	{
		if (IsReplicationResource())
		{
			return true;
		}
		foreach (ClusterResource dependency in GetDependencies())
		{
			if (dependency.IsReplicationChainResource())
			{
				return true;
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsThisQuorumResource()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		ClusterResource quorumResource = m_cluster.GetQuorumResource();
		if (quorumResource != null)
		{
			Guid id = Id;
			if (quorumResource.Id == id)
			{
				return true;
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsThisValidForQuorumResource()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		int num = (((GetCharacteristics() & (true ? 1u : 0u)) != 0 && !IsQuorumBlocked()) ? 1 : 0);
		return (byte)num != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsFlagSet(uint value, uint flag)
	{
		return ((value & flag) != 0) ? true : false;
	}

	internal uint SetRetryAction(uint newValue)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
		string name = "RestartAction";
		Property property = commonProperties.GetProperty(name);
		uint result = (uint)property.Value;
		property.Value = newValue;
		commonProperties.SaveChanges();
		return result;
	}

	private Dictionary<ClusterResource, uint> SetRetryAction(ClusterResourceCollection resources, uint newValue)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Dictionary<ClusterResource, uint> dictionary = new Dictionary<ClusterResource, uint>();
		foreach (ClusterResource resource in resources)
		{
			dictionary.Add(resource, resource.SetRetryAction(newValue));
		}
		return dictionary;
	}

	private void RestoreRetryAction(Dictionary<ClusterResource, uint> previousActions)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Dictionary<ClusterResource, uint>.Enumerator enumerator = previousActions.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				ValueType valueType = enumerator.Current;
				((KeyValuePair<ClusterResource, uint>)valueType).Key.SetRetryAction(((KeyValuePair<ClusterResource, uint>)valueType).Value);
			}
			while (enumerator.MoveNext());
		}
	}

	private unsafe void PhysicalDisk_SetMaintenanceMode([MarshalAs(UnmanagedType.U1)] bool value)
	{
		//Discarded unreachable code: IL_0051
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUS_MAINTENANCE_MODE_INFO cLUS_MAINTENANCE_MODE_INFO);
		*(int*)(&cLUS_MAINTENANCE_MODE_INFO) = (value ? 1 : 0);
		UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&cLUS_MAINTENANCE_MODE_INFO, 4uL);
		try
		{
			m_controlExecutor.ExecuteInControl(20972006u, inputBuffer);
		}
		catch (Exception caughtException)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null)
			{
				int num = -2147023899;
				if (firstException.NativeErrorCode == -2147023899)
				{
					return;
				}
			}
			throw;
		}
		finally
		{
			ResetDisk();
		}
	}

	private unsafe void PhysicalDisk_SetSharedVolumeMaintenanceMode(string deviceId, [MarshalAs(UnmanagedType.U1)] bool value)
	{
		//Discarded unreachable code: IL_0092
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* pszSrc = InteropHelp.StringToWstr(deviceId);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUS_CSV_MAINTENANCE_MODE_INFO cLUS_CSV_MAINTENANCE_MODE_INFO);
		*(int*)(&cLUS_CSV_MAINTENANCE_MODE_INFO) = (value ? 1 : 0);
		int num = global::_003CModule_003E.StringCchCopyW((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUS_CSV_MAINTENANCE_MODE_INFO, 4)), 260uL, pszSrc);
		if (num < 0)
		{
			string[] args = new string[0];
			throw ExceptionHelp.Build<Win32Exception>(num, args);
		}
		UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&cLUS_CSV_MAINTENANCE_MODE_INFO, 524uL);
		try
		{
			m_controlExecutor.ExecuteInControl(20972182u, inputBuffer);
		}
		catch (Exception caughtException)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			ExceptionHelp.LogException(caughtException, "There was an error setting cluster shared volume '{0}' into MM mode '{1}'", deviceId, value);
			if (firstException != null && firstException.NativeErrorCode == -2147023899)
			{
				return;
			}
			throw;
		}
	}

	private unsafe void ChangeResourceGroup(ClusterGroup group)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		uint num = global::_003CModule_003E.ChangeClusterResourceGroup(Handle, group.Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.ResourceChangeGroupFail_Text, m_lastLoadedName);
		}
	}

	private string GenerateDisplayName()
	{
		//Discarded unreachable code: IL_0259
		PropertyCollection propertyCollection = null;
		string text = null;
		string text2 = null;
		PropertyCollection propertyCollection2 = null;
		string text3 = null;
		string text4 = null;
		PropertyCollection propertyCollection3 = null;
		string text5 = null;
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		string text6 = null;
		try
		{
			string resourceType = "DFS Replicated Folder";
			if (IsResourceOfType(resourceType))
			{
				propertyCollection = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				text = (string)propertyCollection["Replicated Folder Name"].Value;
				text2 = (string)propertyCollection["Replicated Folder Root Path"].Value;
				return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", text, text2);
			}
			string resourceType2 = "Network Name";
			if (IsResourceOfType(resourceType2))
			{
				propertyCollection2 = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				text3 = (string)propertyCollection2["DnsName"].Value;
				if (text3.Length != 0)
				{
					return string.Format(CultureInfo.CurrentCulture, Resources.NetworkName_NameDisplayName_Text, text3);
				}
				return Resources.NetworkName_UnconfiguredDisplayName_Text;
			}
			string resourceType3 = "IP Address";
			if (!IsResourceOfType(resourceType3))
			{
				string resourceType4 = "IPv6 Tunnel Address";
				if (!IsResourceOfType(resourceType4))
				{
					string resourceType5 = "IPv6 Address";
					if (!IsResourceOfType(resourceType5))
					{
						string resourceType6 = "File Share Witness";
						if (IsResourceOfType(resourceType6))
						{
							propertyCollection3 = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
							text5 = (string)propertyCollection3["SharePath"].Value;
							if (text5.Length != 0)
							{
								return string.Format(CultureInfo.CurrentCulture, Resources.FileShareWitnessDisplayNameFormat_Text, Name, text5);
							}
							return Resources.FileShareWitnessUnconfiguredDisplayName_Text;
						}
						return Name;
					}
				}
			}
			PropertyCollection privateProperties = GetPrivateProperties(PropertyCollectionSet.Both);
			string text7 = (string)privateProperties["Address"].Value;
			if (text7.Length != 0 && 0 != string.Compare(text7, "0.0.0.0", StringComparison.OrdinalIgnoreCase) && 0 != string.Compare(text7, "::", StringComparison.OrdinalIgnoreCase))
			{
				return string.Format(CultureInfo.CurrentCulture, Resources.IPAddress_AddressDisplayName_Text, text7);
			}
			text4 = string.Empty;
			string resourceType7 = "IPv6 Tunnel Address";
			if (!IsResourceOfType(resourceType7))
			{
				text4 = (string)privateProperties["Network"].Value;
			}
			if (text4.Length != 0)
			{
				return string.Format(CultureInfo.CurrentCulture, Resources.IPAddress_NetworkDisplayName_Text, text4);
			}
			return Resources.IPAddress_UnconfiguredDisplayName_Text;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetDisplayName_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	private void OnClusterRegistryChanged(object sender, ClusterRegistryEventArgs e)
	{
		if (IsDeleted)
		{
			return;
		}
		string strA = string.Format(CultureInfo.InvariantCulture, "Resources\\{0}\\Parameters", m_Id);
		string strA2 = string.Format(CultureInfo.InvariantCulture, "Resources\\{0}", m_Id);
		string strB = e.RegistryName.Replace("\\\\", "\\");
		if (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0)
		{
			string nameB = "IP Address";
			LoadResourceTypeName();
			if (ClusterResourceType.CompareResourceTypeName(m_resourceTypeName, nameB) == 0)
			{
				OnPropertiesChanged();
			}
		}
		if (string.Compare(strA2, strB, StringComparison.OrdinalIgnoreCase) == 0 && e.RegistryChangeType == ClusterRegistryChangeType.Value)
		{
			ResetFlags();
			OnPropertiesChanged();
		}
	}

	private void OnOwnerGroupStateChanged(object sender, EventArgs e)
	{
		OnStateChanged();
	}

	private unsafe Collection<int> ParseDirtyList(CClusPropValueList* dirtyList)
	{
		//IL_003f: Expected I, but got I8
		Collection<int> collection = new Collection<int>();
		uint num = global::_003CModule_003E.CClusPropValueList_002EScMoveToFirstValue(dirtyList);
		switch (num)
		{
		default:
			throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
			{
				Resources.GetDiskDirtyFail_Text,
				m_lastLoadedName
			});
		case 0u:
		{
			CClusPropValueList* ptr = (CClusPropValueList*)((ulong)(nint)dirtyList + 8uL);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
			do
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER, ptr, 8);
				if (*(int*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER)) == 65538)
				{
					collection.Add(*(int*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
				}
				num = global::_003CModule_003E.CClusPropValueList_002EScMoveToNextValue(dirtyList);
			}
			while (num != 259 && num == 0);
			break;
		}
		case 259u:
			break;
		}
		return collection;
	}

	private ICollection<ClusterResource> Storage_ChangeGroup(ClusterGroup group)
	{
		ClusterResource clusterResource = null;
		Dictionary<string, ClusterResource> dictionary = new Dictionary<string, ClusterResource>();
		ICollection<ClusterDiskPartition> collection = null;
		try
		{
			ResetDisk();
			collection = Storage_GetDiskInfo(includeMountPoints: true).Partitions;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Failed to get the disk info for storage resource '{0}'.", DisplayName);
			ChangeGroup(group);
			dictionary.Add(DisplayName, this);
			return dictionary.Values;
		}
		dictionary = BuildMountedVolumesSet(collection);
		clusterResource = null;
		if (!dictionary.TryGetValue(DisplayName, out clusterResource))
		{
			dictionary.Add(DisplayName, this);
		}
		Dictionary<string, ClusterResource>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				enumerator.Current.ChangeGroup(group);
			}
			while (enumerator.MoveNext());
		}
		return dictionary.Values;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool Storage_VolumeNameInList(ICollection<string> sourceDiskVolumeNames, string volumeName)
	{
		foreach (string sourceDiskVolumeName in sourceDiskVolumeNames)
		{
			if (string.Compare(sourceDiskVolumeName, 0, volumeName, 0, 1, StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return true;
			}
		}
		return false;
	}

	private void Storage_ProcessVolumeName(Dictionary<string, ClusterResource> resourcesToMove, ICollection<string> thisDiskVolumeNames, string volumeName, ClusterResource storageResource)
	{
		ClusterResource clusterResource = null;
		if (Storage_VolumeNameInList(thisDiskVolumeNames, volumeName))
		{
			clusterResource = null;
			if (!resourcesToMove.TryGetValue(storageResource.DisplayName, out clusterResource))
			{
				resourcesToMove.Add(storageResource.DisplayName, storageResource);
			}
		}
	}

	private unsafe ICollection<ClusterResource> GetAllStorageInGroup(ClusterGroup group)
	{
		List<ClusterResource> list = new List<ClusterResource>();
		foreach (ClusterResource resource in group.GetResources())
		{
			uint num = 0u;
			if (resource.GetResourceClass(&num) == ClusterResourceClass.Storage)
			{
				list.Add(resource);
			}
		}
		return list;
	}

	private unsafe Dictionary<uint, ClusterDiskPartition> BuildPartitionDictionary()
	{
		//IL_0033: Expected I, but got I8
		//IL_0062: Expected I, but got I8
		//IL_0062: Expected I, but got I8
		//IL_00ab: Expected I, but got I8
		//IL_00cc: Expected I, but got I8
		uint num = 0u;
		Debug.Assert(GetResourceClass(&num) == ClusterResourceClass.Storage);
		Dictionary<uint, ClusterDiskPartition> dictionary = new Dictionary<uint, ClusterDiskPartition>();
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
			System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
			*(long*)(&cLUSPROP_BUFFER_HELPER) = 0L;
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			uint num2 = global::_003CModule_003E.CClusPropValueList_002EScGetResourceValueList(ptr2, Handle, 16777713u, null, null, 0uL);
			if (num2 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2, Resources.GetDiskInfoFail_Text, m_lastLoadedName);
			}
			uint num3 = global::_003CModule_003E.CClusPropValueList_002EScMoveToFirstValue(ptr2);
			if (num3 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num3, Resources.GetDiskInfoFail_Text, m_lastLoadedName);
			}
			CClusPropValueList* ptr4 = (CClusPropValueList*)((ulong)(nint)ptr2 + 8uL);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER2);
			while (true)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER2, ptr4, 8);
				cLUSPROP_BUFFER_HELPER = cLUSPROP_BUFFER_HELPER2;
				if (*(int*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER2)) == 851969)
				{
					ClusterDiskPartition clusterDiskPartition = new ClusterDiskPartition((CLUS_PARTITION_INFO_EX*)(*(long*)(&cLUSPROP_BUFFER_HELPER2) + 8));
					dictionary.Add(clusterDiskPartition.PartitionNumber, clusterDiskPartition);
				}
				num3 = global::_003CModule_003E.CClusPropValueList_002EScMoveToNextValue(ptr2);
				switch (num3)
				{
				case 0u:
					continue;
				case 259u:
					goto end_IL_00ab;
				}
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num3, Resources.GetDiskInfoFail_Text, m_lastLoadedName);
				break;
				continue;
				end_IL_00ab:
				break;
			}
		}
		finally
		{
			if (ptr3 != null)
			{
				global::_003CModule_003E.CClusPropValueList_002EDeleteValueList(ptr3);
				global::_003CModule_003E.delete(ptr3);
			}
		}
		return dictionary;
	}

	public unsafe ICollection<ClusterResource> BuildMountedVolumesSet()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		uint num = 0u;
		if (!(GetResourceClass(&num) == ClusterResourceClass.Storage))
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.NotStorageClassResourceFormat_Text,
				DisplayName
			});
		}
		ICollection<ClusterDiskPartition> partitions = Storage_GetDiskInfo(includeMountPoints: true).Partitions;
		return BuildMountedVolumesSet(partitions).Values;
	}

	private Dictionary<string, ClusterResource> BuildMountedVolumesSet(ICollection<ClusterDiskPartition> partitions)
	{
		Dictionary<string, ClusterResource> dictionary = new Dictionary<string, ClusterResource>();
		ClusterGroup ownerGroup = GetOwnerGroup();
		ICollection<ClusterResource> allStorageInGroup = GetAllStorageInGroup(ownerGroup);
		List<string> list = new List<string>();
		foreach (ClusterDiskPartition partition in partitions)
		{
			if (partition.MountPoints.Count > 0)
			{
				IEnumerator<string> enumerator2 = partition.MountPoints.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.Current;
						list.Add(current2);
					}
				}
				finally
				{
					IEnumerator<string> enumerator3 = enumerator2;
					IDisposable disposable = enumerator2;
					enumerator2?.Dispose();
				}
			}
			else if (!string.IsNullOrEmpty(partition.DriveLetter))
			{
				list.Add(partition.DriveLetter);
			}
		}
		if (list.Count == 0)
		{
			return dictionary;
		}
		foreach (ClusterResource item in allStorageInGroup)
		{
			try
			{
				if (item.GetState() != ResourceState.Online)
				{
					continue;
				}
				item.ResetDisk();
				IEnumerator<ClusterDiskPartition> enumerator5 = item.Storage_GetDiskInfo(includeMountPoints: true).Partitions.GetEnumerator();
				try
				{
					while (enumerator5.MoveNext())
					{
						ClusterDiskPartition current4 = enumerator5.Current;
						if (current4.MountPoints.Count > 0)
						{
							IEnumerator<string> enumerator6 = current4.MountPoints.GetEnumerator();
							try
							{
								while (enumerator6.MoveNext())
								{
									string current5 = enumerator6.Current;
									Storage_ProcessVolumeName(dictionary, list, current5, item);
								}
							}
							finally
							{
								IEnumerator<string> enumerator7 = enumerator6;
								IDisposable disposable2 = enumerator6;
								enumerator6?.Dispose();
							}
						}
						else if (!string.IsNullOrEmpty(current4.DriveLetter))
						{
							Storage_ProcessVolumeName(dictionary, list, current4.DriveLetter, item);
						}
					}
				}
				finally
				{
					IEnumerator<ClusterDiskPartition> enumerator8 = enumerator5;
					IDisposable disposable3 = enumerator5;
					enumerator5?.Dispose();
				}
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Failed to get the disk info for storage resource '{0}' during building mounted volumes set for group '{1}'.", item.m_lastLoadedName, ownerGroup.Name);
				dictionary.Clear();
			}
		}
		return dictionary;
	}

	public void RemoveAllNonStorageDependencyLinks()
	{
		RemoveAllNonStorageDependencyLinks(new List<ClusterResource>());
	}

	private unsafe void RemoveAllNonStorageDependencyLinks(List<ClusterResource> visited)
	{
		visited.Add(this);
		RemoveAllNonStorageDependents();
		RemoveAllNonStorageDependencies();
		foreach (ClusterResource dependent in GetDependents())
		{
			uint num = 0u;
			if (dependent.GetResourceClass(&num) == ClusterResourceClass.Storage && !visited.Contains(dependent))
			{
				dependent.RemoveAllNonStorageDependencyLinks(visited);
			}
		}
		foreach (ClusterResource dependency in GetDependencies())
		{
			uint num2 = 0u;
			int num3 = ((dependency.GetResourceClass(&num2) == ClusterResourceClass.Storage) ? 1 : 0);
			if ((byte)num3 != 0 && !visited.Contains(dependency))
			{
				dependency.RemoveAllNonStorageDependencyLinks(visited);
			}
		}
	}

	private void OnLineShadowCopyResources(object state)
	{
		try
		{
			try
			{
				WaitForDesiredState(ResourceState.Online, StateChangeTimeout, allowFailed: false);
			}
			catch (Exception)
			{
			}
			IEnumerator<ClusterResource> enumerator = GetDependents().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					string resourceType = "Volume Shadow Copy Service Task";
					if (current.IsResourceOfType(resourceType))
					{
						current.BeginBringOnline(force: false, chooseBestNode: false);
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		catch (Exception caughtException)
		{
			ClusterLog.AdminEvents.WriteFailedOnlineShadowCopyResourceEvent(DisplayName);
			ExceptionHelp.LogException(caughtException, "Failed to Online Shadow Copy resource");
		}
	}

	private void DeleteShadowCopyResources()
	{
		try
		{
			IEnumerator<ClusterResource> enumerator = GetDependents().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					string resourceType = "Volume Shadow Copy Service Task";
					if (current.IsResourceOfType(resourceType))
					{
						current.Delete();
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		catch (Exception caughtException)
		{
			ClusterLog.AdminEvents.WriteFailedDeleteShadowCopyResourceEvent(DisplayName);
			ExceptionHelp.LogException(caughtException, "Failed to delete Shadow Copy resource");
		}
	}

	private void TakeDependentVirtualMachinesOffline()
	{
		try
		{
			List<ClusterGroup> groupsThatDependOnThisSharedVolume = GetGroupsThatDependOnThisSharedVolume();
			m_cluster.TakeGroupsThatDependOnClusterSharedVolumeOffline(this, groupsThatDependOnThisSharedVolume);
		}
		catch (Exception caughtException)
		{
			ClusterLog.AdminEvents.WriteCsvTakingDependentGroupsOfflineEvent(DisplayName);
			ExceptionHelp.LogException(caughtException, "There was an error taking the groups that depend upon cluster shared volume '{0}' offline.", DisplayName);
		}
	}

	private void SetOfflineAction(VirtualMachineOfflineActions offlineAction)
	{
		Property property = null;
		PropertyCollection privateProperties = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string name = "OfflineAction";
		VirtualMachineOfflineActions virtualMachineOfflineActions = (VirtualMachineOfflineActions)(uint)privateProperties.GetProperty(name).Value;
		if (!privateProperties.TryGetProperty("PreviousOfflineAction", out property))
		{
			privateProperties.Add(new Property("PreviousOfflineAction", ClusterPropertyType.UInt32));
			string name2 = "PreviousOfflineAction";
			property = privateProperties.GetProperty(name2);
		}
		property.Value = (uint)virtualMachineOfflineActions;
		string name3 = "OfflineAction";
		privateProperties.GetProperty(name3).Value = (uint)offlineAction;
		privateProperties.SaveChanges();
	}

	private List<ClusterGroup> GetGroupsThatDependOnThisSharedVolume()
	{
		List<ClusterGroup> list = new List<ClusterGroup>();
		string resourceType = "Virtual Machine Configuration";
		foreach (ClusterResource resource in m_cluster.GetResources(resourceType))
		{
			if (DoesResourceDependOnThisSharedVolume(resource))
			{
				list.Add(resource.GetOwnerGroup());
			}
		}
		return list;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool DoesResourceDependOnThisSharedVolume(ClusterResource resource)
	{
		Property property = null;
		if (resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite).TryGetProperty("DependsOnSharedVolumes", out property))
		{
			byte condition = ((property.PropertyType == ClusterPropertyType.StringCollection) ? ((byte)1) : ((byte)0));
			Debug.Assert(condition != 0);
			StringEnumerator enumerator = ((StringCollection)property.Value).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (string.Compare(enumerator.Current, Id.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		return false;
	}

	private Guid GetPoolId()
	{
		//Discarded unreachable code: IL_008d
		Monitor.Enter(m_diskLock);
		try
		{
			if (!m_poolId.HasValue)
			{
				Guid guid = Guid.Empty;
				string resourceType = "Physical Disk";
				if (IsResourceOfType(resourceType))
				{
					Property property = GetPrivateProperties(PropertyCollectionSet.ReadOnly)["PoolId"];
					if (property != null)
					{
						string text = property.Value.ToString();
						if (!string.IsNullOrWhiteSpace(text))
						{
							Guid guid2 = new Guid(text);
							guid = guid2;
						}
					}
				}
				Guid? poolId = guid;
				m_poolId = poolId;
			}
			return m_poolId.Value;
		}
		finally
		{
			Monitor.Exit(m_diskLock);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsSpaceResource()
	{
		return GetPoolId() != Guid.Empty;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool GetSymmetry()
	{
		uint num = 0u;
		if (!(GetResourceClass(&num) == ClusterResourceClass.Storage))
		{
			return false;
		}
		List<ClusterResource> list = new List<ClusterResource>();
		list.Add(this);
		ClusterNodeCollection nodes = m_cluster.GetNodes();
		IDictionary<ClusterResource, IList<ClusterNode>> diskConnectivity = m_cluster.GetDiskConnectivity(list, nodes);
		if (diskConnectivity.Count != 0)
		{
			return diskConnectivity[this].Count == nodes.Count;
		}
		return false;
	}

	private unsafe static _STORAGE_DEVICE_ID_DESCRIPTOR* GetDeviceIdDescriptorFromBlob(object uniqueIds)
	{
		//IL_001f: Expected I, but got I8
		//IL_0029: Expected I, but got I8
		//IL_003d: Expected I, but got I8
		//IL_0043: Expected I, but got I8
		//IL_0087: Expected I, but got I8
		//IL_0095: Expected I, but got I8
		//IL_0098: Expected I, but got I8
		//IL_009b: Expected I, but got I8
		byte[] array = (byte[])uniqueIds;
		fixed (byte* ptr = &array[0])
		{
			ulong num = (ulong)array.LongLength;
			switch (num)
			{
			case 0uL:
				return null;
			case 1uL:
			case 2uL:
			case 3uL:
			case 4uL:
			case 5uL:
			case 6uL:
			case 7uL:
			case 8uL:
			case 9uL:
			case 10uL:
			case 11uL:
				return null;
			default:
			{
				_STORAGE_DEVICE_ID_DESCRIPTOR* ptr2 = (_STORAGE_DEVICE_ID_DESCRIPTOR*)ptr;
				uint num2 = *(uint*)((ulong)(nint)ptr2 + 4uL);
				if (num < num2)
				{
					return null;
				}
				byte* ptr3 = (byte*)((ulong)(nint)ptr2 + 12uL);
				ulong num3 = num2 - 12;
				uint num4 = 0u;
				uint num5 = *(uint*)((ulong)(nint)ptr2 + 8uL);
				if (0 < num5)
				{
					do
					{
						if (num3 >= 16)
						{
							if (num3 >= (uint)(*(ushort*)((ulong)(nint)ptr3 + 8uL) + 16))
							{
								ushort num6 = *(ushort*)((ulong)(nint)ptr3 + 10uL);
								ulong num7 = num6;
								if (num3 >= num7)
								{
									num3 -= num7;
									ptr3 = (byte*)((ulong)num6 + (ulong)(nint)ptr3);
									num4++;
									continue;
								}
								return null;
							}
							return null;
						}
						return null;
					}
					while (num4 < num5);
				}
				return ptr2;
			}
			}
		}
	}

	private unsafe static uint GetSpaceIdsFromStorageId(_STORAGE_DEVICE_ID_DESCRIPTOR* pStorageDeviceDescriptor, _SP_ID* pId)
	{
		//IL_0006: Expected I, but got I8
		//IL_0030: Expected I, but got I8
		byte* ptr = (byte*)((ulong)(nint)pStorageDeviceDescriptor + 12uL);
		uint num = 1168u;
		uint num2 = 0u;
		if (0u < (uint)(*(int*)((ulong)(nint)pStorageDeviceDescriptor + 8uL)))
		{
			do
			{
				num = GetSpaceIdsFromStorageId((_STORAGE_IDENTIFIER*)ptr, pId);
				if (num != 1168)
				{
					break;
				}
				ptr = (byte*)((ulong)(*(ushort*)((ulong)(nint)ptr + 10uL)) + (ulong)(nint)ptr);
				num2++;
			}
			while (num2 < (uint)(*(int*)((ulong)(nint)pStorageDeviceDescriptor + 8uL)));
		}
		return num;
	}

	private unsafe static uint GetSpaceIdsFromStorageId(_STORAGE_IDENTIFIER* pIdentifier, _SP_ID* pId)
	{
		//IL_0023: Expected I, but got I8
		if (*(int*)((ulong)(nint)pIdentifier + 4uL) == 0 && *(int*)pIdentifier == 1 && *(int*)((ulong)(nint)pIdentifier + 12uL) == 0 && (uint)(*(ushort*)((ulong)(nint)pIdentifier + 8uL)) >= 48u)
		{
			_GUID* ptr = (_GUID*)((ulong)(nint)pIdentifier + 16uL);
			if ((byte)((global::_003CModule_003E.IsEqualGUID(ptr, (_GUID*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E.MS_002EInternal_002EServerClusters_002E_003FA0xa500668d_002ESpacesVendorSpecificPrefix)) != 0) ? 1u : 0u) != 0)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(pId, (long)(nint)ptr + 16L, 32);
				return 0u;
			}
		}
		return 1168u;
	}

	internal ResourceState GetUncachedState()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		ResetState();
		return GetState();
	}

	internal void ValidateVcoName(string name)
	{
		//Discarded unreachable code: IL_0046, IL_0048
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		UnmanagedBuffer unmanagedBuffer = null;
		try
		{
			unmanagedBuffer = UnmanagedBuffer.Create(name);
			m_controlExecutor.ExecuteInControl(16777605u, unmanagedBuffer);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ClusterInputValidationException>(innerException, new string[2]
			{
				Resources.Resource_InvalidVcoName_Text,
				name
			});
		}
		finally
		{
			((IDisposable)unmanagedBuffer)?.Dispose();
		}
	}

	internal static ClusterResource CreateObject(Cluster cluster, SafeResourceHandle hResource, string resourceName, Guid resourceId, ClusterGroup group)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Guid id;
		if (resourceId != Guid.Empty)
		{
			id = resourceId;
		}
		else
		{
			ResourceControlExecutor resourceControlExecutor = new ResourceControlExecutor(hResource, cluster);
			id = resourceControlExecutor.GetId(resourceControlExecutor);
		}
		Monitor.Enter(m_creationLockObject);
		ClusterResource clusterResource = null;
		try
		{
			clusterResource = cluster.ObjectMgr.GetResourceInstance(id);
			if (clusterResource == null)
			{
				clusterResource = new ClusterResource(cluster, hResource, id, resourceName, group);
			}
			else
			{
				((IDisposable)hResource)?.Dispose();
			}
		}
		finally
		{
			Monitor.Exit(m_creationLockObject);
		}
		return clusterResource;
	}

	internal static ClusterResource CreateObject(Cluster cluster, string resourceName, Guid resourceId, ClusterGroup group)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		SafeResourceHandle hResource;
		if (resourceId != Guid.Empty)
		{
			string resourceName2 = resourceId.ToString();
			hResource = NativeMethods.OpenClusterResource(cluster, resourceName2);
		}
		else
		{
			hResource = NativeMethods.OpenClusterResource(cluster, resourceName);
		}
		return CreateObject(cluster, hResource, resourceName, resourceId, group);
	}

	internal static ClusterResource CreateObject(Cluster cluster, SafeResourceHandle hResource, string resourceName, ClusterGroup group)
	{
		return CreateObject(cluster, hResource, resourceName, Guid.Empty, group);
	}

	internal void OnStateChanged()
	{
		Exception ex = null;
		Exception ex2 = null;
		if (IsDeleted)
		{
			return;
		}
		bool flag = true;
		m_lifetimeHelper.AquireDisposeLock();
		try
		{
			m_lifetimeHelper.CheckObjectState();
			ResetState();
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
		if (IsDeleted)
		{
			return;
		}
		m_lifetimeHelper.AquireDisposeLock();
		try
		{
			m_lifetimeHelper.CheckObjectState();
			SetName(null);
			ClearDisplayName();
			ResetDisk();
			Monitor.Enter(m_commonPropertyCollectionLockObject);
			try
			{
				m_commonPropertyCollection = null;
			}
			finally
			{
				Monitor.Exit(m_commonPropertyCollectionLockObject);
			}
			Monitor.Enter(m_privatePropertyCollectionLockObject);
			try
			{
				m_privatePropertyCollection = null;
			}
			finally
			{
				Monitor.Exit(m_privatePropertyCollectionLockObject);
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
			DebugLog.LogVerbose("Marking resource '{0}' as deleted.", m_lastLoadedName);
			m_cluster.ObjectMgr.UnregisterInstance(this);
			m_lifetimeHelper.MarkAsDeleted();
			Close();
			((IDisposable)this)?.Dispose();
		}
		catch (Exception e)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool rethrowException);
			flag = NotificationManager.ShouldRaiseNotificationEvent(e, ref rethrowException);
			if (rethrowException)
			{
				throw;
			}
			goto IL_0066;
		}
		goto IL_006a;
		IL_0066:
		if (!flag)
		{
			return;
		}
		goto IL_006a;
		IL_006a:
		try
		{
			raise_Deleted(this, EventArgs.Empty);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event Deleted");
		}
	}

	internal unsafe static ResourceState GetResourceState(Cluster cluster, _HRESOURCE* hResource, ref string nodeName, ref string groupName)
	{
		//Discarded unreachable code: IL_009a
		//IL_0003: Expected I, but got I8
		//IL_000a: Expected I, but got I8
		ushort* ptr = null;
		uint num = 64u;
		ushort* ptr2 = null;
		uint num2 = 64u;
		try
		{
			ptr = InteropHelp.AllocateWCharArray(num);
			ptr2 = InteropHelp.AllocateWCharArray(num2);
			CLUSTER_RESOURCE_STATE clusterResourceState = global::_003CModule_003E.GetClusterResourceState(hResource, ptr, &num, ptr2, &num2);
			uint lastError = global::_003CModule_003E.GetLastError();
			if (clusterResourceState == (CLUSTER_RESOURCE_STATE)(-1))
			{
				if (lastError == 234)
				{
					num++;
					InteropHelp.ReallocateWCharArray(&ptr, num);
					num2++;
					InteropHelp.ReallocateWCharArray(&ptr2, num2);
					clusterResourceState = global::_003CModule_003E.GetClusterResourceState(hResource, ptr, &num, ptr2, &num2);
					lastError = global::_003CModule_003E.GetLastError();
					if (clusterResourceState != (CLUSTER_RESOURCE_STATE)(-1))
					{
						goto IL_0078;
					}
				}
				ClusApiExceptionFactory.CreateAndThrow(cluster, (int)lastError);
			}
			goto IL_0078;
			IL_0078:
			nodeName = InteropHelp.WstrToString(ptr);
			groupName = InteropHelp.WstrToString(ptr2);
			return (ResourceState)clusterResourceState;
		}
		finally
		{
			InteropHelp.FreeArray(ptr);
			InteropHelp.FreeArray(ptr2);
		}
	}

	public override void Refresh()
	{
		ResetDisk();
		ResetState();
		SetName(null);
		ClearDisplayName();
	}

	public override void Close()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Monitor.Enter(m_stateLockObject);
		try
		{
			if (m_closed)
			{
				return;
			}
			m_closed = true;
			DebugLog.LogVerbose("Closing resource '{0}'.", m_lastLoadedName);
			LockAccessToHandle(writeAccess: true);
			try
			{
				SafeResourceHandle hResource = m_hResource;
				IDisposable disposable = hResource;
				((IDisposable)hResource)?.Dispose();
			}
			finally
			{
				m_hResource = null;
				UnlockAccessToHandle(writeAccess: true);
			}
			Monitor.Enter(m_cachedSharedVolumeInfoLockObject);
			try
			{
				if (m_cachedSharedVolumeInfo is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
				m_cachedSharedVolumeInfo = null;
			}
			finally
			{
				Monitor.Exit(m_cachedSharedVolumeInfoLockObject);
			}
			if (m_ownerGroup != null)
			{
				m_ownerGroup.StateChanged -= OnOwnerGroupStateChanged;
			}
			m_cluster.RegistryChanged -= OnClusterRegistryChanged;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "There was an error closing resource '{0}'.", DisplayName);
		}
		finally
		{
			Monitor.Exit(m_stateLockObject);
		}
	}

	internal void PhysicalDisk_ResetPassThroughDisk()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string resourceType = "Physical Disk";
		if (IsResourceOfType(resourceType))
		{
			PropertyCollection privateProperties = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
			string name = "DiskRunChkDsk";
			privateProperties.GetProperty(name).Value = 0u;
			try
			{
				privateProperties.SaveChanges();
				return;
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Resource '{0}' failed to set private property 'DiskRunChkDsk'.", m_lastLoadedName);
				return;
			}
		}
		throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.PhysicalDiskPassThroughResetInvalid_Text });
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

	public void ClearDisplayName()
	{
		Monitor.Enter(m_displayNameLock);
		try
		{
			m_displayName = null;
		}
		finally
		{
			Monitor.Exit(m_displayNameLock);
		}
	}

	public unsafe string GetNetworkName()
	{
		//Discarded unreachable code: IL_0094
		//IL_0008: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		try
		{
			m_lifetimeHelper.CheckObjectState();
			uint num = 128u;
			ptr = InteropHelp.AllocateWCharArray(129u);
			if (global::_003CModule_003E.GetClusterResourceNetworkName(Handle, ptr, &num) == 0)
			{
				uint lastError = global::_003CModule_003E.GetLastError();
				if (lastError != 234)
				{
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)lastError, Resources.ClusterResourceGetNetworkNameFail_Text);
				}
				num++;
				InteropHelp.ReallocateWCharArray(&ptr, num);
				if (global::_003CModule_003E.GetClusterResourceNetworkName(Handle, ptr, &num) == 0)
				{
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)global::_003CModule_003E.GetLastError(), Resources.ClusterResourceGetNetworkNameFail_Text);
				}
			}
			return InteropHelp.WstrToString(ptr);
		}
		finally
		{
			InteropHelp.FreeArray(ptr);
		}
	}

	public unsafe void BeginBringOnline([MarshalAs(UnmanagedType.U1)] bool force, [MarshalAs(UnmanagedType.U1)] bool chooseBestNode)
	{
		//Discarded unreachable code: IL_00c4
		//IL_0055: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ResourceState uncachedState = GetUncachedState();
			if (uncachedState != ResourceState.Online && uncachedState != ResourceState.OnlinePending)
			{
				Refresh();
				uint num = 0u;
				uint num2 = 0u;
				num2 = (force ? 1u : num2);
				if (chooseBestNode)
				{
					num2 |= 8u;
				}
				num = ((num2 == 0) ? global::_003CModule_003E.OnlineClusterResource(Handle) : global::_003CModule_003E.OnlineClusterResourceEx(Handle, num2, null, 0u));
				if (num != 0 && num != 997)
				{
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num);
				}
				string resourceType = "Physical Disk";
				if (IsResourceOfType(resourceType))
				{
					ThreadPool.QueueUserWorkItem(OnLineShadowCopyResources, this);
				}
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_BringOnline_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void BeginBringOnline()
	{
		BeginBringOnline(force: false, chooseBestNode: false);
	}

	public void BringOnline(ValueType timeout)
	{
		//Discarded unreachable code: IL_004e
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		BeginBringOnline(force: false, chooseBestNode: false);
		try
		{
			WaitForDesiredState(ResourceState.Online, (TimeSpan)timeout, allowFailed: false);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_Online_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void BringOnline()
	{
		BringOnline(StateChangeTimeout);
	}

	public unsafe void BeginTakeOffline([MarshalAs(UnmanagedType.U1)] bool force)
	{
		//Discarded unreachable code: IL_009e
		//IL_004b: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ResourceState uncachedState = GetUncachedState();
			if (uncachedState != ResourceState.Offline && uncachedState != ResourceState.OfflinePending)
			{
				if (IsClusterSharedVolume)
				{
					TakeDependentVirtualMachinesOffline();
				}
				Refresh();
				uint num = 0u;
				num = ((!force) ? global::_003CModule_003E.OfflineClusterResource(Handle) : global::_003CModule_003E.OfflineClusterResourceEx(Handle, 1u, null, 0u));
				if (num != 0 && num != 997 && num != 5038)
				{
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num);
				}
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_TakeOffline_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void BeginTakeOffline()
	{
		BeginTakeOffline(force: false);
	}

	public void TakeOffline(ValueType timeout)
	{
		//Discarded unreachable code: IL_004d
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		BeginTakeOffline(force: false);
		try
		{
			WaitForDesiredState(ResourceState.Offline, (TimeSpan)timeout, allowFailed: true);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_Offline_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void TakeOffline()
	{
		TakeOffline(StateChangeTimeout);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool TryWaitForDesiredState(ResourceState state, TimeSpan timeout)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		TimeSpan timeSpan = default(TimeSpan);
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, 250);
		bool result = false;
		if (timeSpan < timeout)
		{
			do
			{
				ThreadWatchdog.PerformUIThreadCheck();
				ResetState();
				if (GetState() != state)
				{
					Thread.Sleep(timeSpan2);
					timeSpan += timeSpan2;
					ThreadWatchdog.PerformUIThreadCheck();
					ResetState();
					if (GetState() == state)
					{
						result = true;
						break;
					}
					continue;
				}
				result = true;
				break;
			}
			while (timeSpan < timeout);
		}
		return result;
	}

	public unsafe void Delete()
	{
		//Discarded unreachable code: IL_021e
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			string resourceType = "Virtual Machine Configuration";
			if (IsResourceOfType(resourceType))
			{
				IEnumerator<ClusterResource> enumerator = GetDependents().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ClusterResource current = enumerator.Current;
						string resourceType2 = "Virtual Machine";
						if (current.IsResourceOfType(resourceType2))
						{
							current.Delete();
							return;
						}
					}
				}
				finally
				{
					IEnumerator<ClusterResource> enumerator2 = enumerator;
					IDisposable disposable = enumerator;
					enumerator?.Dispose();
				}
			}
			if (ResourceState.Failed != GetUncachedState() && (m_cluster.CurrentVersion <= ClusterVersion.Windows7 || (GetCharacteristics() & 0x400) == 0))
			{
				TakeOffline();
			}
			uint num = 0u;
			if (GetResourceClass(&num) == ClusterResourceClass.Storage)
			{
				DeleteShadowCopyResources();
			}
			IEnumerator<ClusterResource> enumerator3 = GetDependencies().GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					ClusterResource current2 = enumerator3.Current;
					string resourceType3 = "Virtual Machine Configuration";
					if (!current2.IsResourceOfType(resourceType3))
					{
						continue;
					}
					try
					{
						Cluster cluster = m_cluster;
						if (cluster.CurrentVersion <= ClusterVersion.Windows7 || (GetCharacteristics() & 0x400) == 0)
						{
							current2.TakeOffline();
						}
					}
					catch (Exception caughtException)
					{
						ExceptionHelp.LogException(caughtException, "There was an error taking the virtual machine cfg resource offline.");
					}
					try
					{
						try
						{
							IEnumerator<ClusterResource> enumerator4 = current2.GetDependencies().GetEnumerator();
							try
							{
								while (enumerator4.MoveNext())
								{
									ClusterResource current3 = enumerator4.Current;
									uint num2 = 0u;
									int num3 = ((current3.GetResourceClass(&num2) == ClusterResourceClass.Storage) ? 1 : 0);
									if ((byte)num3 != 0)
									{
										current3.BringOnline();
									}
								}
							}
							finally
							{
								IEnumerator<ClusterResource> enumerator5 = enumerator4;
								IDisposable disposable2 = enumerator4;
								enumerator4?.Dispose();
							}
						}
						catch (Exception caughtException2)
						{
							ExceptionHelp.LogException(caughtException2, "There was an error ensuing the storage on which the virtual machine cfg resource depends is online.");
						}
						RemoveDependency(current2);
						current2.Delete();
					}
					catch (Exception caughtException3)
					{
						ExceptionHelp.LogException(caughtException3, "There was an error deleting the virtual machine cfg resource.");
					}
					break;
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator6 = enumerator3;
				IDisposable disposable3 = enumerator3;
				enumerator3?.Dispose();
			}
			RemoveAllDependents();
			RemoveAllDependencies();
			uint num4 = global::_003CModule_003E.DeleteClusterResource(Handle);
			if (num4 != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num4);
			}
			m_ownerGroup?.RefreshResources();
			m_cluster.RefreshResources();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_Delete_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe void ChangeGroup(ClusterGroup group)
	{
		//Discarded unreachable code: IL_010b, IL_0137, IL_0139
		//IL_000c: Expected I8, but got I
		//IL_0091: Expected I, but got I8
		//IL_0091: Expected I, but got I8
		//IL_00a2: Expected I, but got I8
		//IL_00d3: Expected I, but got I8
		long num = (nint)stackalloc byte[(int)((long)global::_003CModule_003E.__CxxQueryExceptionSize() * 2L)];
		ThreadWatchdog.PerformUIThreadCheck();
		if (group == null)
		{
			throw new ArgumentNullException("group");
		}
		m_lifetimeHelper.CheckObjectState();
		using ClusterGroup clusterGroup = GetOwnerGroup();
		try
		{
			if (0 == string.Compare(group.Name, clusterGroup.Name, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			if (0 != string.Compare(group.OwnerNodeName, clusterGroup.OwnerNodeName, StringComparison.OrdinalIgnoreCase))
			{
				OfflineManager offlineManager = OfflineManager.Create(this);
				offlineManager.TakeOffline(OfflineOption.OfflineDependencies);
				try
				{
					ChangeResourceGroup(group);
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
							offlineManager.BringOnline();
							global::_003CModule_003E._CxxThrowException(null, null);
							goto end_IL_00a3;
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
						end_IL_00a3:;
					}
					finally
					{
						global::_003CModule_003E.__CxxUnregisterExceptionObject((void*)num, (int)num2);
					}
				}
				uint retryAction = SetRetryAction(0u);
				try
				{
					offlineManager.BringOnline();
					return;
				}
				finally
				{
					SetRetryAction(retryAction);
				}
			}
			ChangeResourceGroup(group);
		}
		catch (ApplicationException ex)
		{
			DebugLog.LogException(ex, "Resource::ChangeGroup() failed.");
			throw ex;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Resource_ChangeGroup_Fail_Text,
				m_lastLoadedName,
				group.Name
			});
		}
	}

	public unsafe ICollection<ClusterResource> ChangeGroup2(ClusterGroup group)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (group == null)
		{
			throw new ArgumentNullException("group");
		}
		m_lifetimeHelper.CheckObjectState();
		ClusterGroup ownerGroup = GetOwnerGroup();
		List<ClusterResource> list = new List<ClusterResource>();
		if (0 == string.Compare(group.Name, ownerGroup.Name, StringComparison.OrdinalIgnoreCase))
		{
			return list;
		}
		uint num = 0u;
		if (!(GetResourceClass(&num) == ClusterResourceClass.Storage))
		{
			list.Add(this);
			ChangeGroup(group);
			return list;
		}
		return Storage_ChangeGroup(group);
	}

	public unsafe void InitiateFailure()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		uint num = global::_003CModule_003E.FailClusterResource(Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.ResourceInitiateFailureFail_Text, m_lastLoadedName);
		}
	}

	public unsafe uint GetCharacteristics()
	{
		//Discarded unreachable code: IL_0081
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		if (m_characteristics == uint.MaxValue)
		{
			m_lifetimeHelper.CheckObjectState();
			LockAccessToHandle(writeAccess: false);
			try
			{
				if (m_characteristics == uint.MaxValue)
				{
					try
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out uint characteristics);
						UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&characteristics, 4uL);
						m_controlExecutor.ExecuteOutControl(m_controlExecutor.GetCharacteristicsCode, outputBuffer);
						m_characteristics = characteristics;
					}
					catch (Exception innerException)
					{
						throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
						{
							Resources.Resource_GetCharacteristics_Fail_Text,
							m_lastLoadedName
						});
					}
				}
			}
			finally
			{
				UnlockAccessToHandle(writeAccess: false);
			}
		}
		return m_characteristics;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool IsQuorumBlocked()
	{
		//Discarded unreachable code: IL_0073, IL_0079
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&num, 4uL);
			m_controlExecutor.ExecuteOutControl(16777905u, outputBuffer);
			return (byte)((num != 0) ? 1u : 0u) != 0;
		}
		catch (Win32Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error {0} occurred trying to determine if resource {1} is currently blocking quorum", ex.NativeErrorCode & 0xFFFF, m_lastLoadedName);
			if (ex.NativeErrorCode == -2147024895)
			{
				return false;
			}
			if (ex.NativeErrorCode == -2147019875)
			{
				return true;
			}
			throw;
		}
	}

	public ClusterResourceCollection GetDependents()
	{
		//Discarded unreachable code: IL_006a, IL_006c, IL_006e, IL_0078
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		LockAccessToHandle(writeAccess: false);
		try
		{
			SafeResourceEnumHandle enumHandle = new SafeResourceEnumHandle(m_cluster, m_hResource, ResourceEnumType.Dependants);
			return new ClusterResourceCollection(new AsyncEnumeration<ClusterResource>(m_cluster.GetResource, enumHandle));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetDependents_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	public ClusterResourceCollection GetDependencies()
	{
		//Discarded unreachable code: IL_006a, IL_006c, IL_006e, IL_0078
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		LockAccessToHandle(writeAccess: false);
		try
		{
			SafeResourceEnumHandle enumHandle = new SafeResourceEnumHandle(m_cluster, m_hResource, ResourceEnumType.Dependances);
			return new ClusterResourceCollection(new AsyncEnumeration<ClusterResource>(m_cluster.GetResource, enumHandle));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetDependencies_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	public ClusterResource GetCoreNetworkNameDependency()
	{
		ClusterResource clusterResource = null;
		foreach (ClusterResource dependency in GetDependencies())
		{
			string resourceType = "Network Name";
			if (dependency.IsResourceOfType(resourceType))
			{
				if (clusterResource != null)
				{
					throw ExceptionHelp.Build<ClusterMultipleCoreResourcesFoundException>(0, new string[1] { WellKnownResourceType.NetName });
				}
				clusterResource = dependency;
			}
		}
		return clusterResource;
	}

	public long GetDependentCount()
	{
		//Discarded unreachable code: IL_0052, IL_0054, IL_0056, IL_0060
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		LockAccessToHandle(writeAccess: false);
		try
		{
			return new SafeResourceEnumHandle(m_cluster, m_hResource, ResourceEnumType.Dependants).GetCount();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetDependentCount_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	public long GetDependenciesCount()
	{
		//Discarded unreachable code: IL_0052, IL_0054, IL_0056, IL_0060
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		LockAccessToHandle(writeAccess: false);
		try
		{
			return new SafeResourceEnumHandle(m_cluster, m_hResource, ResourceEnumType.Dependances).GetCount();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetDependencyCount_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool HasDependents()
	{
		return (byte)((GetDependentCount() != 0) ? 1u : 0u) != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool HasDependencies()
	{
		return (byte)((GetDependenciesCount() != 0) ? 1u : 0u) != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool CanBeDependent(ClusterResource resource)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (resource == null)
		{
			throw new ArgumentNullException("resource");
		}
		m_lifetimeHelper.CheckObjectState();
		return (global::_003CModule_003E.CanResourceBeDependent(Handle, resource.Handle) != 0) ? true : false;
	}

	public unsafe void AddDependency(ClusterResource resource)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (resource == null)
		{
			throw new ArgumentNullException("resource");
		}
		m_lifetimeHelper.CheckObjectState();
		uint num = global::_003CModule_003E.AddClusterResourceDependency(Handle, resource.Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.ResourceAddDependencyFail_Text, resource.Name);
		}
	}

	public unsafe void RemoveDependency(ClusterResource resource)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (resource == null)
		{
			throw new ArgumentNullException("resource");
		}
		m_lifetimeHelper.CheckObjectState();
		uint num = global::_003CModule_003E.RemoveClusterResourceDependency(Handle, resource.Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.ResourceRemoveDependencyFail_Text, resource.Name);
		}
	}

	public void RemoveAllDependencies()
	{
		ClusterResourceCollection clusterResourceCollection = GetDependencies();
		try
		{
			IEnumerator<ClusterResource> enumerator = clusterResourceCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					RemoveDependency(current);
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterResourceCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
	}

	public unsafe void RemoveAllNonStorageDependencies()
	{
		ClusterResourceCollection clusterResourceCollection = GetDependencies();
		try
		{
			IEnumerator<ClusterResource> enumerator = clusterResourceCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					uint num = 0u;
					int num2 = ((current.GetResourceClass(&num) == ClusterResourceClass.Storage) ? 1 : 0);
					if ((byte)num2 == 0)
					{
						RemoveDependency(current);
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterResourceCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
	}

	public void RemoveAllDependents()
	{
		ClusterResourceCollection clusterResourceCollection = GetDependents();
		try
		{
			IEnumerator<ClusterResource> enumerator = clusterResourceCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					string resourceType = "Volume Shadow Copy Service Task";
					if (!current.IsResourceOfType(resourceType))
					{
						current.RemoveDependency(this);
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterResourceCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
	}

	public unsafe void RemoveAllNonStorageDependents()
	{
		ClusterResourceCollection clusterResourceCollection = GetDependents();
		try
		{
			IEnumerator<ClusterResource> enumerator = clusterResourceCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					uint num = 0u;
					int num2 = ((current.GetResourceClass(&num) == ClusterResourceClass.Storage) ? 1 : 0);
					if ((byte)num2 == 0)
					{
						string resourceType = "Volume Shadow Copy Service Task";
						if (!current.IsResourceOfType(resourceType))
						{
							current.RemoveDependency(this);
						}
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterResourceCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
	}

	public void RemoveAllDependencyLinks()
	{
		RemoveAllDependents();
		RemoveAllDependencies();
	}

	public StringCollection GetRegistryCheckpoints()
	{
		//Discarded unreachable code: IL_0036, IL_0038
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			return GetCheckPoints(16777385u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetRegistryCheckpoints_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void AddRegistryCheckpoint(string checkpointName)
	{
		//Discarded unreachable code: IL_0036
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			AddRemoveCheckpoint(checkpointName, 20971682u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_AddRegistryCheckpoint_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void RemoveRegistryCheckpoint(string checkpointName)
	{
		//Discarded unreachable code: IL_0036
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			AddRemoveCheckpoint(checkpointName, 20971686u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_RemoveRegistryCheckpoint_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public StringCollection GetCryptoCheckpoints()
	{
		//Discarded unreachable code: IL_0036, IL_0038
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			return GetCheckPoints(16777397u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetCryptoCheckpoints_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void AddCryptoCheckpoint(string checkpointName)
	{
		//Discarded unreachable code: IL_0036
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			AddRemoveCheckpoint(checkpointName, 20971694u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_AddCryptoCheckpoint_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void RemoveCryptoCheckpoint(string checkpointName)
	{
		//Discarded unreachable code: IL_0036
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			AddRemoveCheckpoint(checkpointName, 20971698u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_RemoveCryptoCheckpoint_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public ClusterNode GetOwnerNode()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return m_cluster.GetNode(GetOwnerNodeName());
	}

	public ClusterNodeCollection GetPossibleOwnerNodes()
	{
		//Discarded unreachable code: IL_006a, IL_006c, IL_006e, IL_0078
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		LockAccessToHandle(writeAccess: false);
		try
		{
			SafeResourceEnumHandle enumHandle = new SafeResourceEnumHandle(m_cluster, m_hResource, ResourceEnumType.Nodes);
			return new ClusterNodeCollection(new AsyncEnumeration<ClusterNode>(m_cluster.GetNode, enumHandle));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetPossibleOwners_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	public unsafe void AddPossibleOwner(ClusterNode node)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		uint num = global::_003CModule_003E.AddClusterResourceNode(Handle, node.Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.AddClusterResourceNodeFail_Text, node.Name, m_lastLoadedName);
		}
	}

	public unsafe void RemovePossibleOwner(ClusterNode node)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		uint num = global::_003CModule_003E.RemoveClusterResourceNode(Handle, node.Handle);
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RemoveClusterResourceNodeFail_Text, node.Name, m_lastLoadedName);
		}
	}

	public ClusterGroup GetOwnerGroup()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		ClusterGroup clusterGroup = m_ownerGroup;
		if (ClusterItem.CachingDisabled && !IsDeleted)
		{
			clusterGroup = null;
		}
		if (clusterGroup == null)
		{
			do
			{
				LoadState(loadOwnerGroup: true);
				clusterGroup = m_ownerGroup;
			}
			while (clusterGroup == null);
		}
		return clusterGroup;
	}

	public unsafe void Rename(string newName)
	{
		//IL_0008: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		if (newName == null)
		{
			throw new ArgumentNullException("newName");
		}
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ptr = InteropHelp.StringToWstr(newName);
			uint num = global::_003CModule_003E.SetClusterResourceName(Handle, ptr);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RenameResourceFail_Text, m_lastLoadedName, newName);
			}
			SetName(null);
			ClearDisplayName();
			m_lastLoadedName = newName;
			m_name = newName;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	public unsafe RequiredDependencies GetRequiredDependencies()
	{
		//Discarded unreachable code: IL_00fd
		//IL_0028: Expected I, but got I8
		//IL_0052: Expected I, but got I8
		//IL_0052: Expected I, but got I8
		//IL_009f: Expected I, but got I8
		//IL_00c1: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
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
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			uint num = global::_003CModule_003E.CClusPropValueList_002EScGetResourceValueList(ptr2, Handle, 16777233u, null, null, 0uL);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.GetRequiredDependenciesFail_Text, m_lastLoadedName);
			}
			List<ClusterResourceClass> list = new List<ClusterResourceClass>();
			List<string> list2 = new List<string>();
			System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
			for (uint num2 = global::_003CModule_003E.CClusPropValueList_002EScMoveToFirstValue(ptr2); num2 != 259; num2 = global::_003CModule_003E.CClusPropValueList_002EScMoveToNextValue(ptr3))
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER, (long)(nint)ptr3 + 8L, 8);
				CLUSPROP_REQUIRED_DEPENDENCY* ptr4 = (CLUSPROP_REQUIRED_DEPENDENCY*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER));
				switch ((uint)(*(int*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER))))
				{
				case 262147u:
					list2.Add(InteropHelp.WstrToString((ushort*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8)));
					break;
				case 131074u:
					list.Add(*(ClusterResourceClass*)(*(long*)(&cLUSPROP_BUFFER_HELPER) + 8));
					break;
				}
			}
			return new RequiredDependencies(list, list2);
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

	public unsafe ClusterResourceClass GetResourceClass(uint* resourceSubClass)
	{
		//Discarded unreachable code: IL_0128, IL_012a, IL_012c, IL_013a
		//IL_0061: Expected I4, but got I8
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		Monitor.Enter(m_classInfoLockObject);
		try
		{
			if (!m_clusResClass.HasValue)
			{
				if (IsDeleted)
				{
					return ClusterResourceClass.Unknown;
				}
				string resourceType = "Virtual Machine";
				if (!IsResourceOfType(resourceType))
				{
					string resourceType2 = "Virtual Machine Configuration";
					if (!IsResourceOfType(resourceType2))
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUS_RESOURCE_CLASS_INFO cLUS_RESOURCE_CLASS_INFO);
						// IL initblk instruction
						System.Runtime.CompilerServices.Unsafe.InitBlock(ref cLUS_RESOURCE_CLASS_INFO, 0, 8);
						UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&cLUS_RESOURCE_CLASS_INFO, 8uL);
						ResourceControlExecutor controlExecutor = m_controlExecutor;
						ResourceControlExecutor resourceControlExecutor = controlExecutor;
						controlExecutor.ExecuteOutControl(16777229u, outputBuffer);
						ClusterResourceClass? clusResClass = *(ClusterResourceClass*)(&cLUS_RESOURCE_CLASS_INFO);
						m_clusResClass = clusResClass;
						m_clusResSubClass = System.Runtime.CompilerServices.Unsafe.As<CLUS_RESOURCE_CLASS_INFO, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUS_RESOURCE_CLASS_INFO, 4));
						Cluster cluster = m_cluster;
						int num = ((cluster.CurrentVersion == ClusterVersion.Windows8) ? 1 : 0);
						if ((byte)num != 0)
						{
							string resourceType3 = "Physical Disk";
							if (IsResourceOfType(resourceType3))
							{
								m_clusResSubClass |= 1073741824u;
							}
						}
						goto IL_00eb;
					}
				}
				return ClusterResourceClass.Unknown;
			}
			goto IL_00eb;
			IL_00eb:
			*resourceSubClass = m_clusResSubClass;
			return m_clusResClass.Value;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetClassInfo_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_classInfoLockObject);
		}
	}

	public unsafe ClusterResourceClass GetResourceClass()
	{
		uint num = 0u;
		return GetResourceClass(&num);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool IsStorageDisk()
	{
		uint num = 0u;
		GetResourceClass(&num);
		return (byte)((num >> 30) & 1u) != 0;
	}

	public ClusterDisk Storage_GetDiskInfo([MarshalAs(UnmanagedType.U1)] bool includeMountPoints)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		return GetDisk(includeMountPoints);
	}

	public ClusterDisk Storage_GetUncachedDiskInfo([MarshalAs(UnmanagedType.U1)] bool includeMountPoints)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ResetDisk();
		return GetDisk(includeMountPoints);
	}

	public unsafe void Storage_SetDriveLetter(uint partitionNumber, uint driveLetterMask)
	{
		//Discarded unreachable code: IL_005c
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUS_STORAGE_SET_DRIVELETTER cLUS_STORAGE_SET_DRIVELETTER);
			*(uint*)(&cLUS_STORAGE_SET_DRIVELETTER) = partitionNumber;
			System.Runtime.CompilerServices.Unsafe.As<_CLUS_STORAGE_SET_DRIVELETTER, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUS_STORAGE_SET_DRIVELETTER, 4)) = driveLetterMask;
			UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&cLUS_STORAGE_SET_DRIVELETTER, 8uL);
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			ResourceControlExecutor resourceControlExecutor = controlExecutor;
			controlExecutor.ExecuteInControl(20972010u, inputBuffer);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_SetDriveLetter_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool Storage_IsPathValid(string path)
	{
		//Discarded unreachable code: IL_0116
		//IL_0008: Expected I, but got I8
		//IL_003f: Expected I, but got I8
		//IL_0048: Expected I8, but got I
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		bool result = true;
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ptr = InteropHelp.StringToWstr(path);
			ushort* ptr2 = ptr;
			while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0)
			{
				ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
			}
			ulong num = (ulong)((nint)((byte*)ptr2 - (nuint)ptr) >> 1);
			UnmanagedBuffer inputBuffer = new UnmanagedBuffer(ptr, (num + 1) * 2);
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			m_controlExecutor.ExecuteInControl(GetOwnerNode(), 16777625u, inputBuffer);
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException == null)
			{
				goto IL_00f3;
			}
			int num2 = -2147024862;
			if (firstException.NativeErrorCode != -2147024862)
			{
				int num3 = -2147024893;
				if (firstException.NativeErrorCode != -2147024893)
				{
					int num4 = -2147024894;
					if (firstException.NativeErrorCode != -2147024894)
					{
						int num5 = -2147024773;
						if (firstException.NativeErrorCode != -2147024773)
						{
							int num6 = -2147024735;
							if (firstException.NativeErrorCode != -2147024735)
							{
								goto IL_00f3;
							}
						}
					}
				}
			}
			result = false;
			goto end_IL_007d;
			IL_00f3:
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Resource_IsPathValid_Fail_Text,
				m_lastLoadedName
			});
			end_IL_007d:;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return result;
	}

	public void IPAddress_ReleaseLease()
	{
		//Discarded unreachable code: IL_0047
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			ResourceControlExecutor resourceControlExecutor = controlExecutor;
			controlExecutor.ExecuteCommandControl(20971970u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_ReleaseLease_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void IPAddress_RenewLease()
	{
		//Discarded unreachable code: IL_0047
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			ResourceControlExecutor resourceControlExecutor = controlExecutor;
			controlExecutor.ExecuteCommandControl(20971966u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_RenewLease_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe void VM_StartMigration(ClusterNode node, NativeGroupHelp.VmMigrationType type, NativeGroupHelp.GroupMoveFlags flags)
	{
		//Discarded unreachable code: IL_00e5, IL_0188
		//IL_0063: Expected I, but got I8
		//IL_006c: Expected I8, but got I
		UnmanagedBuffer unmanagedBuffer = null;
		Exception ex = null;
		Exception ex2 = null;
		ResourceControlExecutor resourceControlExecutor = null;
		ThreadWatchdog.PerformUIThreadCheck();
		if (node == null && m_cluster.CurrentVersion <= ClusterVersion.Windows7)
		{
			throw new ArgumentNullException("node");
		}
		m_lifetimeHelper.CheckObjectState();
		if (m_cluster.CurrentVersion == ClusterVersion.Windows7)
		{
			ushort* ptr = InteropHelp.StringToWstr(node.Name);
			try
			{
				ushort* ptr2 = ptr;
				while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0)
				{
					ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
				}
				ulong num = (ulong)((nint)((byte*)ptr2 - (nuint)ptr) >> 1);
				unmanagedBuffer = new UnmanagedBuffer(ptr, (num + 1) * 2);
				ResourceControlExecutor controlExecutor = m_controlExecutor;
				resourceControlExecutor = controlExecutor;
				controlExecutor.ExecuteInControl(23068676u, unmanagedBuffer);
				return;
			}
			catch (Exception ex3)
			{
				Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex3);
				if (firstException != null)
				{
					int num2 = -2147023899;
					if (firstException.NativeErrorCode == -2147023899)
					{
						return;
					}
				}
				throw ExceptionHelp.Build<ApplicationException>(ex3, new string[2]
				{
					Resources.VM_StartMigration_Fail_Text,
					m_lastLoadedName
				});
			}
			finally
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
		try
		{
			PropertyListWrapper migrationTypePropertyList = NativeGroupHelp.GetMigrationTypePropertyList(type);
			try
			{
				if (type == NativeGroupHelp.VmMigrationType.Live)
				{
					GetOwnerGroup().BeginMoveEx(node, NativeGroupHelp.LiveMigrationFlags | flags, migrationTypePropertyList.PropertyList);
				}
				else
				{
					GetOwnerGroup().BeginMoveEx(node, flags, migrationTypePropertyList.PropertyList);
				}
			}
			finally
			{
				PropertyListWrapper propertyListWrapper = migrationTypePropertyList;
				IDisposable disposable = migrationTypePropertyList;
				((IDisposable)migrationTypePropertyList)?.Dispose();
			}
		}
		catch (Exception ex4)
		{
			Win32Exception firstException2 = ExceptionHelp.GetFirstException<Win32Exception>(ex4);
			if (firstException2 != null && firstException2.NativeErrorCode == -2147023899)
			{
				return;
			}
			throw ExceptionHelp.Build<ApplicationException>(ex4, new string[2]
			{
				Resources.VM_StartMigration_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void VM_StartMigration(ClusterNode node)
	{
		VM_StartMigration(node, NativeGroupHelp.VmMigrationType.Live, NativeGroupHelp.LiveMigrationFlags);
	}

	public void VM_CancelMigration()
	{
		//Discarded unreachable code: IL_0050, IL_0081
		m_lifetimeHelper.CheckObjectState();
		if (m_cluster.CurrentVersion == ClusterVersion.Windows7)
		{
			try
			{
				ResourceControlExecutor controlExecutor = m_controlExecutor;
				ResourceControlExecutor resourceControlExecutor = controlExecutor;
				controlExecutor.ExecuteCommandControl(23068680u);
				return;
			}
			catch (Exception innerException)
			{
				throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
				{
					Resources.VM_CancelMigration_Fail_Text,
					m_lastLoadedName
				});
			}
		}
		try
		{
			GetOwnerGroup().CancelOperation();
		}
		catch (Exception innerException2)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException2, new string[2]
			{
				Resources.VM_CancelMigration_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe void VM_SetNextOfflineAction(VirtualMachineOfflineActions virtualMachineOfflineAction)
	{
		//Discarded unreachable code: IL_007e, IL_00a4
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&virtualMachineOfflineAction, (uint)sizeof(VirtualMachineOfflineActions));
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			ResourceControlExecutor resourceControlExecutor = controlExecutor;
			controlExecutor.ExecuteInControl(23068692u, inputBuffer);
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null && firstException.NativeErrorCode == -2147024895)
			{
				try
				{
					SetOfflineAction(virtualMachineOfflineAction);
					return;
				}
				catch (Exception innerException)
				{
					throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
					{
						Resources.VM_StartMigration_Fail_Text,
						m_lastLoadedName
					});
				}
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.VM_StartMigration_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void VM_Configuration_UpdateConfiguration()
	{
		//Discarded unreachable code: IL_003e
		m_lifetimeHelper.CheckObjectState();
		try
		{
			m_controlExecutor.ExecuteCommandControl(23068676u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.VM_Configuration_UpdateConfiguration_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool PhysicalDisk_IsMaintenanceModeOn()
	{
		//Discarded unreachable code: IL_00df
		if (m_cachedDiskIsMaintenanceMode.HasValue && !ClusterItem.CachingDisabled)
		{
			return (bool)m_cachedDiskIsMaintenanceMode;
		}
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			bool? cachedDiskIsMaintenanceMode = false;
			m_cachedDiskIsMaintenanceMode = cachedDiskIsMaintenanceMode;
			uint num = 0u;
			if (GetResourceClass(&num) == ClusterResourceClass.Storage)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUS_MAINTENANCE_MODE_INFO cLUS_MAINTENANCE_MODE_INFO);
				UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&cLUS_MAINTENANCE_MODE_INFO, 4uL);
				m_controlExecutor.ExecuteOutControl(16777697u, outputBuffer);
				byte b = ((*(int*)(&cLUS_MAINTENANCE_MODE_INFO) == 1) ? ((byte)1) : ((byte)0));
				bool? cachedDiskIsMaintenanceMode2 = b != 0;
				m_cachedDiskIsMaintenanceMode = cachedDiskIsMaintenanceMode2;
			}
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null && firstException.NativeErrorCode == -2147024895)
			{
				ExceptionHelp.LogException(ex, "Resource '{0}' does not support maintenance mode.", m_lastLoadedName);
				return false;
			}
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Resource_IsMaintenanceModeOn_Fail_Text,
				m_lastLoadedName
			});
		}
		return m_cachedDiskIsMaintenanceMode.Value;
	}

	public void PhysicalDisk_EnableMaintenanceMode()
	{
		//Discarded unreachable code: IL_002f
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			PhysicalDisk_SetMaintenanceMode(value: true);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_EnableMaintenanceMode_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void PhysicalDisk_DisableMaintenanceMode()
	{
		//Discarded unreachable code: IL_002f
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			PhysicalDisk_SetMaintenanceMode(value: false);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_DisableMaintenanceModeOn_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe void PhysicalDisk_WaitForMaintenanceModeExit()
	{
		//IL_0010: Expected I4, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUS_MAINTENANCE_MODE_INFOEX cLUS_MAINTENANCE_MODE_INFOEX);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref cLUS_MAINTENANCE_MODE_INFOEX, 0, 16);
		UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&cLUS_MAINTENANCE_MODE_INFOEX, 16uL);
		m_controlExecutor.ExecuteOutControl(16777697u, outputBuffer);
		if (System.Runtime.CompilerServices.Unsafe.As<_CLUS_MAINTENANCE_MODE_INFOEX, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUS_MAINTENANCE_MODE_INFOEX, 8)) >= 128)
		{
			do
			{
				global::_003CModule_003E.Sleep(500u);
				m_controlExecutor.ExecuteOutControl(16777697u, outputBuffer);
			}
			while (System.Runtime.CompilerServices.Unsafe.As<_CLUS_MAINTENANCE_MODE_INFOEX, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUS_MAINTENANCE_MODE_INFOEX, 8)) >= 128);
		}
		OnPropertiesChanged();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool PhysicalDisk_HasMountPoints()
	{
		//Discarded unreachable code: IL_003d
		ICollection<ClusterResource> collection = null;
		try
		{
			collection = BuildMountedVolumesSet();
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Failed to build the mounted volume set for storage resource '{0}'.", DisplayName);
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.ResourceFailedGettingMountPointInfoFormat_Text,
				DisplayName
			});
		}
		return collection.Count > 1;
	}

	public void PhysicalDisk_EnableSharedVolumeMaintenanceMode(string deviceId)
	{
		//Discarded unreachable code: IL_0029
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			PhysicalDisk_SetSharedVolumeMaintenanceMode(deviceId, value: true);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_EnableSharedVolumeMaintenanceMode_Fail_Text,
				deviceId
			});
		}
	}

	public void PhysicalDisk_DisableSharedVolumeMaintenanceMode(string deviceId)
	{
		//Discarded unreachable code: IL_0029
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			PhysicalDisk_SetSharedVolumeMaintenanceMode(deviceId, value: false);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_DisableSharedVolumeMaintenanceMode_Fail_Text,
				deviceId
			});
		}
	}

	public unsafe void PhysicalDisk_SharedVolumeSetDirectIOMode(string deviceId)
	{
		//Discarded unreachable code: IL_0061
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			UnmanagedBuffer inputBuffer = UnmanagedBuffer.Create(deviceId);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0GE_0040G _0024ArrayType_0024_0024_0024BY0GE_0040G);
			UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&_0024ArrayType_0024_0024_0024BY0GE_0040G, 200uL);
			m_controlExecutor.ExecuteInOutControl(20972170u, inputBuffer, outputBuffer);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "There was an error setting cluster shared volume '{0}' into direct IO mode", deviceId);
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Resource_SharedVolumeSetDirectIOMode_Fail_Text,
				deviceId
			});
		}
	}

	public unsafe void PhysicalDisk_SharedVolumeSetRedirectedIOMode(string deviceId)
	{
		//Discarded unreachable code: IL_0061
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			UnmanagedBuffer inputBuffer = UnmanagedBuffer.Create(deviceId);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0GE_0040G _0024ArrayType_0024_0024_0024BY0GE_0040G);
			UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&_0024ArrayType_0024_0024_0024BY0GE_0040G, 200uL);
			m_controlExecutor.ExecuteInOutControl(20972174u, inputBuffer, outputBuffer);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "There was an error enabling redirected access for cluster shared volume '{0}'.", deviceId);
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.Resource_SharedVolumeSetRedirectedIOMode_Fail_Text,
				deviceId
			});
		}
	}

	public unsafe ICollection<ClusterSharedVolumeInfo> PhysicalDisk_GetSharedVolumeInfo()
	{
		//Discarded unreachable code: IL_01cd
		//IL_0070: Expected I, but got I8
		//IL_00ab: Expected I, but got I8
		//IL_00ab: Expected I, but got I8
		//IL_0126: Expected I, but got I8
		//IL_01b4: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		Monitor.Enter(m_cachedSharedVolumeInfoLockObject);
		try
		{
			if (m_cachedSharedVolumeInfo != null && !ClusterItem.CachingDisabled && !proxyResource)
			{
				return m_cachedSharedVolumeInfo;
			}
			m_lifetimeHelper.CheckObjectState();
			Collection<ClusterSharedVolumeInfo> collection = new Collection<ClusterSharedVolumeInfo>();
			if (!IsClusterSharedVolume)
			{
				return collection;
			}
			CClusPropValueList* ptr = (CClusPropValueList*)global::_003CModule_003E.@new(48uL);
			CClusPropValueList* ptr2;
			try
			{
				ptr2 = ((ptr == null) ? null : global::_003CModule_003E.CClusPropValueList_002E_007Bctor_007D(ptr));
				CClusPropValueList* ptr3 = ptr2;
			}
			catch
			{
				//try-fault
				global::_003CModule_003E.delete(ptr);
				throw;
			}
			CClusPropValueList* ptr4 = ptr2;
			Dictionary<uint, ClusterDiskPartition> dictionary = BuildPartitionDictionary();
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
				*(long*)(&cLUSPROP_BUFFER_HELPER) = 0L;
				ResourceControlExecutor controlExecutor = m_controlExecutor;
				uint num = global::_003CModule_003E.CClusPropValueList_002EScGetResourceValueList(ptr2, Handle, 16777765u, null, null, 0uL);
				if (num != 0)
				{
					Cluster cluster = m_cluster;
					ClusApiExceptionFactory.CreateAndThrow(cluster, (int)num, Resources.GetSharedVolumeInfoFailed_Text, m_lastLoadedName);
				}
				uint num2 = global::_003CModule_003E.CClusPropValueList_002EScMoveToFirstValue(ptr2);
				switch (num2)
				{
				case 259u:
					return collection;
				default:
				{
					Cluster cluster2 = m_cluster;
					ClusApiExceptionFactory.CreateAndThrow(cluster2, (int)num2, Resources.GetSharedVolumeInfoFailed_Text, m_lastLoadedName);
					break;
				}
				case 0u:
					break;
				}
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER2);
				while (true)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER2, (long)(nint)ptr4 + 8L, 8);
					cLUSPROP_BUFFER_HELPER = cLUSPROP_BUFFER_HELPER2;
					if (*(int*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER2)) == 65537)
					{
						_CLUS_CSV_VOLUME_INFO* ptr5 = (_CLUS_CSV_VOLUME_INFO*)(*(long*)(&cLUSPROP_BUFFER_HELPER2) + 8);
						ClusterDiskPartition partition = dictionary[*(uint*)((ulong)(nint)ptr5 + 8uL)];
						ClusterSharedVolumeInfo clusterSharedVolumeInfo = null;
						ClusterSharedVolumeInfo item;
						if (proxyResource)
						{
							item = new ClusterSharedVolumeInfo(ptr5, partition);
						}
						else
						{
							Cluster cluster3 = m_cluster;
							item = new ClusterSharedVolumeInfo(cluster3, ptr5, partition, this);
						}
						collection.Add(item);
					}
					num2 = global::_003CModule_003E.CClusPropValueList_002EScMoveToNextValue(ptr4);
					switch (num2)
					{
					case 0u:
						continue;
					case 259u:
						goto end_IL_0106;
					}
					Cluster cluster4 = m_cluster;
					ClusApiExceptionFactory.CreateAndThrow(cluster4, (int)num2, Resources.GetSharedVolumeInfoFailed_Text, m_lastLoadedName);
					break;
					continue;
					end_IL_0106:
					break;
				}
			}
			finally
			{
				CClusPropValueList* ptr6 = ptr4;
				CClusPropValueList* ptr7 = ptr4;
				if (ptr4 != null)
				{
					void* ptr8 = global::_003CModule_003E.CClusPropValueList_002E__delDtor(ptr4, 1u);
				}
				else
				{
					void* ptr8 = null;
				}
			}
			m_cachedSharedVolumeInfo = collection;
			return collection;
		}
		finally
		{
			Monitor.Exit(m_cachedSharedVolumeInfoLockObject);
		}
	}

	public unsafe ICollection<ClusterSharedVolumeStateInfo> PhysicalDisk_GetSharedVolumeStateInfo()
	{
		//IL_002b: Expected I, but got I8
		//IL_005b: Expected I, but got I8
		//IL_005b: Expected I, but got I8
		//IL_00b4: Expected I, but got I8
		//IL_00d8: Expected I, but got I8
		Collection<ClusterSharedVolumeStateInfo> collection = new Collection<ClusterSharedVolumeStateInfo>();
		if (!IsClusterSharedVolume)
		{
			return collection;
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
			System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER);
			*(long*)(&cLUSPROP_BUFFER_HELPER) = 0L;
			ResourceControlExecutor controlExecutor = m_controlExecutor;
			uint num = global::_003CModule_003E.CClusPropValueList_002EScGetResourceValueList(ptr2, Handle, 20972194u, null, null, 0uL);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.GetSharedVolumeInfoFailed_Text, m_lastLoadedName);
			}
			uint num2 = global::_003CModule_003E.CClusPropValueList_002EScMoveToFirstValue(ptr2);
			switch (num2)
			{
			case 259u:
				return collection;
			default:
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2, Resources.GetSharedVolumeInfoFailed_Text, m_lastLoadedName);
				break;
			case 0u:
				break;
			}
			CClusPropValueList* ptr4 = (CClusPropValueList*)((ulong)(nint)ptr2 + 8uL);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUSPROP_BUFFER_HELPER cLUSPROP_BUFFER_HELPER2);
			while (true)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref cLUSPROP_BUFFER_HELPER2, ptr4, 8);
				cLUSPROP_BUFFER_HELPER = cLUSPROP_BUFFER_HELPER2;
				if (*(int*)(*(ulong*)(&cLUSPROP_BUFFER_HELPER2)) == 65537)
				{
					long num3 = *(long*)(&cLUSPROP_BUFFER_HELPER2) + 8;
					ClusterSharedVolumeStateInfo clusterSharedVolumeStateInfo = null;
					ClusterSharedVolumeStateInfo item = new ClusterSharedVolumeStateInfo((_CLUSTER_SHARED_VOLUME_STATE_INFO_EX*)num3);
					collection.Add(item);
				}
				num2 = global::_003CModule_003E.CClusPropValueList_002EScMoveToNextValue(ptr2);
				switch (num2)
				{
				case 0u:
					continue;
				case 259u:
					goto end_IL_00b4;
				}
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2, Resources.GetSharedVolumeInfoFailed_Text, m_lastLoadedName);
				break;
				continue;
				end_IL_00b4:
				break;
			}
		}
		finally
		{
			if (ptr3 != null)
			{
				global::_003CModule_003E.CClusPropValueList_002EDeleteValueList(ptr3);
				global::_003CModule_003E.delete(ptr3);
			}
		}
		return collection;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool PhysicalDisk_IsPassThroughDisk()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string resourceType = "Physical Disk";
		if (IsResourceOfType(resourceType))
		{
			PropertyCollection privateProperties = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
			try
			{
				Property property = privateProperties["DiskRunChkDsk"];
				if (property.Value == null)
				{
					return false;
				}
				return (uint)property.Value == 6;
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Resource '{0}' returned an invalid DiskRunChkDsk private property.", m_lastLoadedName);
			}
		}
		return false;
	}

	public override ControlExecutor GetControlExecutor()
	{
		return new ResourceControlExecutor(this, m_cluster);
	}

	public override PropertyCollection GetCommonProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0083, IL_00a6, IL_00a8, IL_00aa, IL_00b8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_commonPropertyCollectionLockObject);
		try
		{
			if (m_commonPropertyCollection == null || ClusterItem.CachingDisabled)
			{
				m_commonPropertyCollection = new PropertyCollection(this, ClusterPropertyScope.Common, PropertyCollectionSet.Both);
			}
			return GetDesiredCollection(m_commonPropertyCollection, propSet);
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019890)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build(ex, Resources.Resource_GetCommonProperties_Fail_Text, m_lastLoadedName);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetCommonProperties_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_commonPropertyCollectionLockObject);
		}
	}

	public override PropertyCollection GetPrivateProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0083, IL_00a6, IL_00a8, IL_00aa, IL_00b8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_privatePropertyCollectionLockObject);
		try
		{
			if (m_privatePropertyCollection == null || ClusterItem.CachingDisabled)
			{
				m_privatePropertyCollection = new PropertyCollection(this, ClusterPropertyScope.Private, PropertyCollectionSet.Both);
			}
			return GetDesiredCollection(m_privatePropertyCollection, propSet);
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019890)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build(ex, Resources.Resource_GetPrivateProperties_Fail_Text, m_lastLoadedName);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetPrivateProperties_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_privatePropertyCollectionLockObject);
		}
	}

	public Property GetCommonProperty(string propertyName)
	{
		//Discarded unreachable code: IL_0085, IL_00ac, IL_00ae, IL_00b0, IL_00be
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_commonPropertyCollectionLockObject);
		try
		{
			if (m_commonPropertyCollection == null || ClusterItem.CachingDisabled)
			{
				m_commonPropertyCollection = new PropertyCollection(this, ClusterPropertyScope.Common, PropertyCollectionSet.Both);
			}
			return m_commonPropertyCollection.GetProperty(propertyName);
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019890)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build(ex, Resources.Resource_GetCommonProperties_Fail_Text, m_lastLoadedName);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Resource_GetCommonProperty_Fail_Text,
				propertyName,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_commonPropertyCollectionLockObject);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool TryGetCommonProperty(string propertyName, ref Property property)
	{
		//Discarded unreachable code: IL_006d, IL_006f, IL_0071, IL_007f
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_commonPropertyCollectionLockObject);
		try
		{
			if (m_commonPropertyCollection == null || ClusterItem.CachingDisabled)
			{
				m_commonPropertyCollection = new PropertyCollection(this, ClusterPropertyScope.Common, PropertyCollectionSet.Both);
			}
			return m_commonPropertyCollection.TryGetProperty(propertyName, out property);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Resource_GetCommonProperty_Fail_Text,
				propertyName,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_commonPropertyCollectionLockObject);
		}
	}

	public Property GetPrivateProperty(string propertyName)
	{
		//Discarded unreachable code: IL_0085, IL_00ac, IL_00ae, IL_00b0, IL_00be
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_privatePropertyCollectionLockObject);
		try
		{
			if (m_privatePropertyCollection == null || ClusterItem.CachingDisabled)
			{
				m_privatePropertyCollection = new PropertyCollection(this, ClusterPropertyScope.Private, PropertyCollectionSet.Both);
			}
			return m_privatePropertyCollection.GetProperty(propertyName);
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == -2147019884)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build(ex, Resources.Resource_GetPrivateProperties_Fail_Text, m_lastLoadedName);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Resource_GetPrivateProperty_Fail_Text,
				propertyName,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_privatePropertyCollectionLockObject);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool TryGetPrivateProperty(string propertyName, ref Property property)
	{
		//Discarded unreachable code: IL_006d, IL_006f, IL_0071, IL_007f
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		Monitor.Enter(m_privatePropertyCollectionLockObject);
		try
		{
			if (m_privatePropertyCollection == null || ClusterItem.CachingDisabled)
			{
				m_privatePropertyCollection = new PropertyCollection(this, ClusterPropertyScope.Private, PropertyCollectionSet.Both);
			}
			return m_privatePropertyCollection.TryGetProperty(propertyName, out property);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.Resource_GetPrivateProperty_Fail_Text,
				propertyName,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_privatePropertyCollectionLockObject);
		}
	}

	public unsafe ClusterResourceRelationship GetDependencyRelationship()
	{
		//Discarded unreachable code: IL_009d, IL_009f
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		uint num = 1024u;
		ClusterResourceRelationship clusterResourceRelationship = null;
		try
		{
			ptr = InteropHelp.AllocateWCharArray(num);
			uint clusterResourceDependencyExpression = global::_003CModule_003E.GetClusterResourceDependencyExpression(Handle, ptr, &num);
			if (clusterResourceDependencyExpression == 234)
			{
				num++;
				InteropHelp.ReallocateWCharArray(&ptr, num);
				clusterResourceDependencyExpression = global::_003CModule_003E.GetClusterResourceDependencyExpression(Handle, ptr, &num);
			}
			if (clusterResourceDependencyExpression != 0)
			{
				Cluster cluster = m_cluster;
				ClusApiExceptionFactory.CreateAndThrow(cluster, (int)clusterResourceDependencyExpression);
			}
			string expression = InteropHelp.WstrToString(ptr);
			return ClusterResourceRelationship.Parse(this, expression);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_GetDependencyRelationship_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			InteropHelp.FreeArray(ptr);
		}
	}

	public unsafe void SetDependencyRelationship(string dependencyExpression)
	{
		//Discarded unreachable code: IL_005b, IL_005d
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(dependencyExpression);
			uint num = global::_003CModule_003E.SetClusterResourceDependencyExpression(Handle, ptr);
			if (num != 0)
			{
				Cluster cluster = m_cluster;
				ClusApiExceptionFactory.CreateAndThrow(cluster, (int)num);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_SetDependencyRelationship_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	public unsafe void SetDependencyRelationship(ClusterResourceRelationship relationship)
	{
		//Discarded unreachable code: IL_0066, IL_0068
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		try
		{
			relationship.Normalize();
			ptr = InteropHelp.StringToWstr(relationship.ToString());
			uint num = global::_003CModule_003E.SetClusterResourceDependencyExpression(Handle, ptr);
			if (num != 0)
			{
				Cluster cluster = m_cluster;
				ClusApiExceptionFactory.CreateAndThrow(cluster, (int)num);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Resource_SetDependencyRelationship_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsResourceOfType(string resourceType)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		LoadResourceTypeName();
		string resourceTypeName = m_resourceTypeName;
		return 0 == ClusterResourceType.CompareResourceTypeName(resourceType, resourceTypeName);
	}

	public override string ToString()
	{
		return Name;
	}

	public unsafe ClusterRegistryKey GetRegistryKey(RegistryRights rights)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		HKEY__* clusterResourceKey = global::_003CModule_003E.GetClusterResourceKey(Handle, ClusterRegistryKey.RegistryRightsToRegSam(rights));
		uint lastError = global::_003CModule_003E.GetLastError();
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterResourceKey);
		if (safeRegistryHandle.IsInvalid)
		{
			if (lastError == 5006)
			{
				m_lifetimeHelper.MarkAsDeleted();
			}
			throw ExceptionHelp.Build((int)lastError, Resources.ClusterResource_GetRegistryKeyFailed_Text, m_lastLoadedName);
		}
		return new ClusterRegistryKey(m_cluster, safeRegistryHandle);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool WillOfflineLoseQuorum()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (GetState() != ResourceState.Online)
		{
			return false;
		}
		Guid id = Id;
		return m_cluster.WillVoterLoseQuorum(117440585u, id.ToString());
	}

	public unsafe void ResetPassword()
	{
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr((string)GetPrivateProperties(PropertyCollectionSet.ReadWrite)["DnsName"].Value);
			uint num = global::_003CModule_003E.ClRtlResetPassword(Handle, ptr);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.ResetPasswordFail_Text, m_lastLoadedName);
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool PhysicalDisk_SetDiskToPassThroughDisk()
	{
		//Discarded unreachable code: IL_009b
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		string resourceType = "Physical Disk";
		if (IsResourceOfType(resourceType))
		{
			PropertyCollection privateProperties = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
			string name = "DiskRunChkDsk";
			if ((uint)privateProperties.GetProperty(name).Value == 6)
			{
				return false;
			}
			string name2 = "DiskRunChkDsk";
			privateProperties.GetProperty(name2).Value = 6u;
			try
			{
				privateProperties.SaveChanges();
				return true;
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Resource '{0}' failed to set private property 'DiskRunChkDsk'.", m_lastLoadedName);
				return false;
			}
		}
		throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.PhysicalDiskPassThroughSetInvalid_Text });
	}

	public void NetworkName_RegisterDns()
	{
		//Discarded unreachable code: IL_0038
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			m_controlExecutor.ExecuteCommandControl(16777586u);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Register_Dns_Failed_Text,
				m_lastLoadedName
			});
		}
	}

	public static void AddSharedVolumeDependency(ClusterResource resource, string sharedVolumeId)
	{
		Property property = null;
		if (resource == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "resource" });
		}
		if (sharedVolumeId == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolumeId" });
		}
		if (!(resource.m_cluster.CurrentVersion > ClusterVersion.WindowsServer2008))
		{
			return;
		}
		property = null;
		PropertyCollection privateProperties = resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		if (!privateProperties.TryGetProperty("DependsOnSharedVolumes", out property))
		{
			DebugLog.LogWarning("The virtual machine configuration resource '{0}' does not have the DependsOnSharedVolumes private property. This prevents the UI from showing the shared volumes on which this resource depends.", resource.Name);
			return;
		}
		if (property.PropertyType != ClusterPropertyType.StringCollection)
		{
			ClusterLog.AdminEvents.WriteInvalidRegistryValueEvent("DependsOnSharedVolumes", "Multi-Sz");
			throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.InvalidRegistryValueType_Text });
		}
		StringCollection obj = (StringCollection)property.Value;
		StringCollection stringCollection = new StringCollection();
		StringEnumerator enumerator = obj.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				stringCollection.Add(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		if (!stringCollection.Contains(sharedVolumeId))
		{
			stringCollection.Add(sharedVolumeId);
			property.Value = stringCollection;
			privateProperties.SaveChanges();
		}
	}

	public static void ClearSharedVolumeDependencies(ClusterResource resource)
	{
		Property property = null;
		if (resource == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "resource" });
		}
		property = null;
		PropertyCollection privateProperties = resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		if (privateProperties.TryGetProperty("DependsOnSharedVolumes", out property))
		{
			byte condition = ((property.PropertyType == ClusterPropertyType.StringCollection) ? ((byte)1) : ((byte)0));
			Debug.Assert(condition != 0);
			property.Value = new StringCollection();
			privateProperties.SaveChanges();
		}
	}

	public static ICollection<ClusterResource> GetSharedVolumeDependencies(ClusterResource resource)
	{
		if (resource == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "resource" });
		}
		ICollection<ClusterResource> collection = new List<ClusterResource>();
		if (resource.m_cluster.CurrentVersion > ClusterVersion.WindowsServer2008)
		{
			foreach (string sharedVolumeDependencyId in GetSharedVolumeDependencyIds(resource))
			{
				try
				{
					Cluster cluster = resource.m_cluster;
					ClusterResource resourceFromSharedVolumeId = cluster.GetResourceFromSharedVolumeId(sharedVolumeDependencyId);
					collection.Add(resourceFromSharedVolumeId);
				}
				catch (Exception caughtException)
				{
					ExceptionHelp.LogException(caughtException, "GetSharedVolumeDependencies for resource {0} and sharedVolumeId {1} failed.", resource.m_lastLoadedName, sharedVolumeDependencyId);
				}
			}
		}
		return collection;
	}

	public static int GetSharedVolumeDependenciesCount(ClusterResource resource)
	{
		if (resource == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "resource" });
		}
		try
		{
			return GetSharedVolumeDependencyIds(resource).Count;
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "There was an error retrieving the shared volume dependencies from the registry for '{0}'", resource.m_lastLoadedName);
			return 0;
		}
	}

	public static ICollection<string> GetSharedVolumeDependencyIds(ClusterResource resource)
	{
		Property property = null;
		List<string> list = new List<string>();
		property = null;
		if (resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite).TryGetProperty("DependsOnSharedVolumes", out property))
		{
			if (property.PropertyType != ClusterPropertyType.StringCollection)
			{
				ClusterLog.AdminEvents.WriteInvalidRegistryValueEvent("DependsOnSharedVolumes", "Multi-Sz");
				throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.InvalidRegistryValueType_Text });
			}
			StringEnumerator enumerator = ((StringCollection)property.Value).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					list.Add(current);
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		return list;
	}

	public unsafe static Guid GetPoolIdFromUniqueIds(object uniqueIds)
	{
		_STORAGE_DEVICE_ID_DESCRIPTOR* deviceIdDescriptorFromBlob = GetDeviceIdDescriptorFromBlob(uniqueIds);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _SP_ID sP_ID);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlock(ref sP_ID, 0, 32);
		Guid result;
		if (GetSpaceIdsFromStorageId(deviceIdDescriptorFromBlob, &sP_ID) == 0)
		{
			Guid guid = new Guid(*(uint*)(&sP_ID), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 4)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 6)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 8)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 9)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 10)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 11)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 12)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 13)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 14)), System.Runtime.CompilerServices.Unsafe.As<_SP_ID, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sP_ID, 15)));
			result = guid;
		}
		else
		{
			result = Guid.Empty;
		}
		return result;
	}

	public void SuspendClusterSharedVolume()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Debug.Assert(IsClusterSharedVolume);
		TakeDependentVirtualMachinesOffline();
		PhysicalDisk_EnableMaintenanceMode();
	}

	public void ResumeClusterSharedVolume()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Debug.Assert(IsClusterSharedVolume);
		PhysicalDisk_DisableMaintenanceMode();
		if (GetState() != ResourceState.Online)
		{
			BringOnline();
			if (GetState() != ResourceState.Online)
			{
				throw ExceptionHelp.Build<ClusterSharedVolumeNotOnlineException>(new string[1] { DisplayName });
			}
		}
	}

	public void AddSharedVolumeDependencies(ICollection<ClusterResource> sharedVolumeResources)
	{
		Property property = null;
		if (sharedVolumeResources == null)
		{
			throw ExceptionHelp.Build<ArgumentNullException>(new string[1] { "sharedVolumeResources" });
		}
		ThreadWatchdog.PerformUIThreadCheck();
		string resourceType = "Virtual Machine Configuration";
		Debug.Assert(IsResourceOfType(resourceType));
		property = null;
		PropertyCollection privateProperties = GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		if (!privateProperties.TryGetProperty("DependsOnSharedVolumes", out property))
		{
			DebugLog.LogWarning("The virtual machine configuration resource '{0}' does not have the DependsOnSharedVolumes private property. This prevents the UI from showing the shared volumes on which this resource depends.", Name);
			return;
		}
		byte condition = ((property.PropertyType == ClusterPropertyType.StringCollection) ? ((byte)1) : ((byte)0));
		Debug.Assert(condition != 0);
		StringCollection obj = (StringCollection)property.Value;
		StringCollection stringCollection = new StringCollection();
		StringEnumerator enumerator = obj.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				stringCollection.Add(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		foreach (ClusterResource sharedVolumeResource in sharedVolumeResources)
		{
			string value = sharedVolumeResource.Id.ToString();
			if (!stringCollection.Contains(value))
			{
				stringCollection.Add(value);
			}
		}
		property.Value = stringCollection;
		privateProperties.SaveChanges();
	}

	public StringCollection GetRelatedResourceNames()
	{
		//Discarded unreachable code: IL_0045, IL_0047
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			return m_controlExecutor.ExecuteStringCollectionOutControl(16785325u, throwOnInvalidFunction: false);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Get_RelatedResources_Failed_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe ClusterNameOUValidationState ValidateOUPermissions()
	{
		//Discarded unreachable code: IL_00dc, IL_00de
		//IL_001b: Expected I4, but got I8
		//IL_0026: Expected I4, but got I8
		//IL_0096: Expected I, but got I8
		//IL_0099: Expected I, but got I8
		//IL_00b1: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT, 0, 24);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT2);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT2, 0, 24);
			System.Runtime.CompilerServices.Unsafe.As<_CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT, 4)) = 1;
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT, 8), ref global::_003CModule_003E.MS_002EInternal_002EServerClusters_002E_003FA0xa500668d_002EComputersGUID, 16);
			*(int*)(&cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT) = 1;
			System.Runtime.CompilerServices.Unsafe.As<_CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT2, 4)) = 16;
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT2, 8), ref global::_003CModule_003E.MS_002EInternal_002EServerClusters_002E_003FA0xa500668d_002EComputersGUID, 16);
			*(int*)(&cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT2) = 1;
			UnmanagedBuffer inputBuffer = new UnmanagedBuffer(&cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT, 24uL);
			UnmanagedBuffer inputBuffer2 = new UnmanagedBuffer(&cLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_INPUT2, 24uL);
			UnmanagedBuffer unmanagedBuffer = m_controlExecutor.ExecuteInOutControlWithoutThrowOnInvalidFunction(117448993u, inputBuffer);
			UnmanagedBuffer unmanagedBuffer2 = m_controlExecutor.ExecuteInOutControlWithoutThrowOnInvalidFunction(117448993u, inputBuffer2);
			_CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_OUTPUT* ptr = null;
			_CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_OUTPUT* ptr2 = null;
			ptr = (_CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_OUTPUT*)unmanagedBuffer.Pointer;
			ptr2 = (_CLUSCTL_RESOURCE_NETNAME_CHECK_OU_PERMISSIONS_OUTPUT*)unmanagedBuffer2.Pointer;
			string ouName = InteropHelp.WstrToString((ushort*)((ulong)(nint)ptr + 12uL));
			return new ClusterNameOUValidationState(*(int*)ptr, *(int*)ptr2, *(int*)((ulong)(nint)ptr + 4uL), ouName);
		}
		catch (Win32Exception innerException)
		{
			throw ExceptionHelp.Build(innerException, Resources.Resource_UnableToCheckOUPermissions_Text);
		}
	}

	public unsafe NetnameADCheckValidationState ValidateADState()
	{
		//Discarded unreachable code: IL_0053, IL_0055
		//IL_0023: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			UnmanagedBuffer unmanagedBuffer = m_controlExecutor.ExecuteOutControlWithoutThrowOnInvalidFunction(117448997u);
			_CLUSCTL_RESOURCE_NETNAME_CHECK_AD_STATE_OUTPUT* ptr = null;
			ptr = (_CLUSCTL_RESOURCE_NETNAME_CHECK_AD_STATE_OUTPUT*)unmanagedBuffer.Pointer;
			return new NetnameADCheckValidationState(ptr);
		}
		catch (Win32Exception innerException)
		{
			throw ExceptionHelp.Build(innerException, Resources.Resource_UnableToCheckADState_Text, m_lastLoadedName);
		}
	}
}

