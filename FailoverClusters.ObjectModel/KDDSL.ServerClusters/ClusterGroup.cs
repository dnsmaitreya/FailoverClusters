using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using FailoverClusters.NativeHelp;
using Win32;

namespace KDDSL.ServerClusters;

public class ClusterGroup : ClusterItem
{
	private volatile string m_name;

	private volatile string m_lastLoadedName;

	private volatile string m_description;

	private int m_loadingName;

	private volatile string m_ownerNodeName;

	private GroupState m_state;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private Guid m_Id;

	private string m_typeRegKey;

	private GroupType m_groupType;

	private volatile bool m_deleting;

	private bool m_closed;

	private volatile ClusterResourceCollection m_resources;

	private object m_resourcesLock;

	private object m_nameLockObject;

	private object m_descriptionLockObject;

	private object m_stateLockObject;

	private object m_flagsLock;

	private object m_registryLock;

	private uint? m_flags;

	private bool? m_autoStart;

	private object m_autoStartLock;

	private Cluster m_cluster;

	private SafeGroupHandle m_hGroup;

	private ReaderWriterLockSlim m_hGroupLock;

	private TimeSpan StateChangeTimeout;

	private object m_deletingLockObject;

	private EventHandler _003Cbacking_store_003EStateChanged;

	private EventHandler _003Cbacking_store_003EPropertiesChanged;

	private EventHandler<DeletingEventArgs> _003Cbacking_store_003EDeleting;

	private EventHandler _003Cbacking_store_003EDeleted;

	private EventHandler _003Cbacking_store_003EGroupTypeChanged;

	public const uint MaximumFailoverThreshold = uint.MaxValue;

	public const uint MaximumFailoverPeriod = 1193u;

	public const uint MaximumFailbackWindowStart = 23u;

	public const uint MaximumFailbackWindowEnd = 23u;

	public const uint FailbackWindowNone = uint.MaxValue;

	public const uint AutoFailbackEnabled = 1u;

	public bool AutoStart
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			//Discarded unreachable code: IL_00bc, IL_00be, IL_00c5, IL_00d3
			Property property = null;
			ThreadWatchdog.PerformUIThreadCheck();
			Monitor.Enter(m_autoStartLock);
			try
			{
				bool value;
				try
				{
					byte b;
					if (!m_autoStart.HasValue || ClusterItem.CachingDisabled)
					{
						PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
						property = null;
						uint num = 1u;
						if (commonProperties.TryGetProperty("Priority", out property))
						{
							num = (uint)property.Value;
							if (num != 1)
							{
								b = 0;
								goto IL_0056;
							}
						}
						b = 1;
						goto IL_0056;
					}
					goto IL_0066;
					IL_0066:
					return m_autoStart.Value;
					IL_0056:
					bool? autoStart = b != 0;
					m_autoStart = autoStart;
					goto IL_0066;
				}
				catch (Exception caughtException)
				{
					if (!(m_cluster.CurrentVersion == ClusterVersion.WindowsServer2008))
					{
						throw;
					}
					ExceptionHelp.LogException(caughtException, "Unable to get the autostart property for the group {0}.", m_lastLoadedName);
					bool? autoStart2 = true;
					m_autoStart = autoStart2;
					value = m_autoStart.Value;
				}
				return value;
			}
			finally
			{
				Monitor.Exit(m_autoStartLock);
			}
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			//Discarded unreachable code: IL_007c
			ThreadWatchdog.PerformUIThreadCheck();
			Monitor.Enter(m_autoStartLock);
			try
			{
				PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
				commonProperties["Priority"].Value = (value ? 1u : 0u);
				commonProperties.SaveChanges();
				bool? autoStart = value;
				m_autoStart = autoStart;
			}
			catch (Exception caughtException)
			{
				if (m_cluster.CurrentVersion == ClusterVersion.WindowsServer2008)
				{
					ExceptionHelp.LogException(caughtException, "Unable to save the autostart property for the group {0}.", m_lastLoadedName);
					bool? autoStart2 = true;
					m_autoStart = autoStart2;
					return;
				}
				throw;
			}
			finally
			{
				Monitor.Exit(m_autoStartLock);
			}
		}
	}

	public bool IsCore
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)(GetFlags() & 1u) != 0;
		}
	}

	public GroupType GroupType
	{
		get
		{
			if (ClusterItem.CachingDisabled && !IsDeleted)
			{
				return GetGroupType();
			}
			return m_groupType;
		}
	}

	public Cluster Cluster => m_cluster;

	public bool ContainsQuorumResource
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return DoesContainQuorumResource();
		}
	}

	public GroupState State
	{
		get
		{
			GroupState groupState = m_state;
			if (ClusterItem.CachingDisabled || IsDeleted)
			{
				groupState = GroupState.Unknown;
			}
			if (groupState == GroupState.Unknown)
			{
				while (!IsDeleted)
				{
					ThreadWatchdog.PerformUIThreadCheck();
					LoadState();
					groupState = m_state;
					if (groupState != GroupState.Unknown)
					{
						break;
					}
				}
			}
			return groupState;
		}
	}

	public string OwnerNodeName
	{
		get
		{
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
					LoadState();
					text = m_ownerNodeName;
				}
				while (text == null);
			}
			return text;
		}
	}

	public override Guid Id => m_Id;

	public string Description => GetDescription();

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

	public bool IsDeleting
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return false;
		}
	}

	public override bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			//Discarded unreachable code: IL_003c
			m_lifetimeHelper.AquireDisposeLock();
			try
			{
				int num = ((m_lifetimeHelper.IsDeleted || m_lifetimeHelper.IsDisposed) ? 1 : 0);
				return (byte)num != 0;
			}
			finally
			{
				m_lifetimeHelper.ReleaseDisposeLock();
			}
		}
	}

	internal unsafe _HGROUP* Handle
	{
		get
		{
			//Discarded unreachable code: IL_0028
			//IL_0032: Expected I, but got I8
			m_lifetimeHelper.CheckObjectState();
			LockAccessToHandle(writeAccess: false);
			try
			{
				return m_hGroup.DangerousGetGroupHandle();
			}
			finally
			{
				UnlockAccessToHandle(writeAccess: false);
			}
		}
	}

	[SpecialName]
	public event EventHandler GroupTypeChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EGroupTypeChanged = (EventHandler)Delegate.Combine(_003Cbacking_store_003EGroupTypeChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EGroupTypeChanged = (EventHandler)Delegate.Remove(_003Cbacking_store_003EGroupTypeChanged, value);
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

	private ClusterGroup(Cluster cluster, SafeGroupHandle hGroup, Guid id, string groupName)
	{
		try
		{
			m_lastLoadedName = groupName;
			m_name = groupName;
			m_hGroupLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			m_hGroup = hGroup;
			m_cluster = cluster;
			m_nameLockObject = new object();
			m_descriptionLockObject = new object();
			m_deletingLockObject = new object();
			m_lifetimeHelper = new ObjectLifetimeHelper();
			TimeSpan stateChangeTimeout = new TimeSpan(0, 5, 0);
			StateChangeTimeout = stateChangeTimeout;
			m_resources = null;
			m_resourcesLock = new object();
			m_stateLockObject = new object();
			SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
			m_Id = id;
			m_closed = false;
			if (Utilities.IsGuid(groupName))
			{
				SetName(null);
			}
			m_typeRegKey = string.Format(CultureInfo.InvariantCulture, "Groups\\{0}", m_Id);
			m_groupType = GroupType.Unknown;
			if (!ClusterItem.CachingDisabled)
			{
				m_cluster.RegistryChanged += OnClusterRegistryChanged;
			}
			m_groupType = GetGroupType();
			m_flags = null;
			m_flagsLock = new object();
			m_autoStart = null;
			m_autoStartLock = new object();
			m_registryLock = new object();
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

	private static SafeGroupHandle OpenGroup(Cluster cluster, string groupName)
	{
		return NativeMethods.OpenClusterGroup(cluster, groupName);
	}

	private static Guid GetId(SafeGroupHandle hGroup, Cluster cluster)
	{
		GroupControlExecutor groupControlExecutor = new GroupControlExecutor(hGroup, cluster);
		return groupControlExecutor.GetId(groupControlExecutor);
	}

	private unsafe void LoadState()
	{
		//Discarded unreachable code: IL_0126, IL_0128
		//IL_002c: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		if (IsDeleted)
		{
			throw ExceptionHelp.Build<ClusterObjectDeletedException>(new string[1] { m_lastLoadedName });
		}
		ushort* ptr = null;
		uint num = 64u;
		if (m_state != GroupState.Unknown && !(m_ownerNodeName == null) && !ClusterItem.CachingDisabled)
		{
			return;
		}
		LockAccessToHandle(writeAccess: false);
		try
		{
			if (m_hGroup == null || (m_state != GroupState.Unknown && !(m_ownerNodeName == null) && !ClusterItem.CachingDisabled))
			{
				return;
			}
			ptr = InteropHelp.AllocateWCharArray(num);
			CLUSTER_GROUP_STATE clusterGroupState = global::_003CModule_003E.GetClusterGroupState(Handle, ptr, &num);
			uint lastError = global::_003CModule_003E.GetLastError();
			if (clusterGroupState == (CLUSTER_GROUP_STATE)(-1))
			{
				if (lastError == 234)
				{
					num++;
					InteropHelp.ReallocateWCharArray(&ptr, num);
					clusterGroupState = global::_003CModule_003E.GetClusterGroupState(Handle, ptr, &num);
					lastError = global::_003CModule_003E.GetLastError();
					if (clusterGroupState != (CLUSTER_GROUP_STATE)(-1))
					{
						goto IL_00f5;
					}
				}
				throw ExceptionHelp.Build((int)lastError, m_lastLoadedName);
			}
			goto IL_00f5;
			IL_00f5:
			SetState((GroupState)clusterGroupState, InteropHelp.WstrToString(ptr), GroupNotification.DoNotRaise);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_GetStateFail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
			InteropHelp.FreeArray(ptr);
		}
	}

	private void ResetState()
	{
		SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
	}

	private void SetState(GroupState groupState, string ownerNodeName, GroupNotification raiseNotification)
	{
		Monitor.Enter(m_stateLockObject);
		try
		{
			m_state = groupState;
			m_ownerNodeName = ownerNodeName;
		}
		finally
		{
			Monitor.Exit(m_stateLockObject);
		}
		if (raiseNotification == GroupNotification.Raise)
		{
			EventArgs empty = EventArgs.Empty;
			_003Cbacking_store_003EStateChanged?.Invoke(this, empty);
		}
	}

	private GroupState GetUncachedState()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
		return State;
	}

	private void ResetName()
	{
		SetName(null);
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

	private string GetDescription()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		string text = m_description;
		if (ClusterItem.CachingDisabled && !IsDeleted)
		{
			text = null;
		}
		if (text == null)
		{
			do
			{
				LoadDescription();
				text = m_description;
			}
			while (text == null);
		}
		return text;
	}

	private void ResetDescription()
	{
		m_description = null;
	}

	private void SetDescription(string description)
	{
		Monitor.Enter(m_descriptionLockObject);
		try
		{
			m_description = description;
		}
		finally
		{
			Monitor.Exit(m_descriptionLockObject);
		}
	}

	private void LoadDescription()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (!IsDeleted && (m_description == null || ClusterItem.CachingDisabled))
		{
			PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadWrite);
			string name = "Description";
			Property property = commonProperties.GetProperty(name);
			SetDescription((string)property.Value);
		}
	}

	private void WaitToLeaveNode(string sourceNodeName, TimeSpan timeout)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		TimeSpan timeSpan = default(TimeSpan);
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, 250);
		if (timeSpan < timeout)
		{
			do
			{
				if (0 == string.Compare(OwnerNodeName, sourceNodeName, StringComparison.OrdinalIgnoreCase) && (GetUncachedState() == GroupState.Pending || IsQueued()))
				{
					Thread.Sleep(timeSpan2);
					timeSpan += timeSpan2;
					SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
					if (0 != string.Compare(OwnerNodeName, sourceNodeName, StringComparison.OrdinalIgnoreCase) || (GetUncachedState() != GroupState.Pending && !IsQueued()))
					{
						return;
					}
					continue;
				}
				return;
			}
			while (timeSpan < timeout);
		}
		throw ExceptionHelp.Build<ApplicationException>(new string[2]
		{
			Resources.StateChange_Timeout_Text,
			m_lastLoadedName
		});
	}

	private void OnClusterRegistryChanged(object sender, ClusterRegistryEventArgs e)
	{
		//Discarded unreachable code: IL_00aa
		if (IsDeleted || 0 != string.Compare(m_typeRegKey, e.RegistryName, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		try
		{
			GroupType groupType = GetGroupType();
			if (m_groupType != groupType)
			{
				Monitor.Enter(m_registryLock);
				try
				{
					m_groupType = groupType;
				}
				finally
				{
					Monitor.Exit(m_registryLock);
				}
				OnGroupTypeChanged();
			}
			if (AutoStart)
			{
				return;
			}
			IEnumerator<ClusterResource> enumerator = GetResources().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.Refresh();
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
			if (ExceptionHelp.IsFirstExceptionFound<ObjectDisposedException>(caughtException) || ExceptionHelp.IsFirstExceptionFound<ClusterObjectDeletedException>(caughtException))
			{
				return;
			}
			throw;
		}
	}

	private void OnGroupTypeChanged()
	{
		Exception ex = null;
		try
		{
			raise_GroupTypeChanged(this, EventArgs.Empty);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event GroupTypeChanged");
		}
	}

	private long GetItemCount(GroupEnumType enumType)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		LockAccessToHandle(writeAccess: false);
		try
		{
			SafeGroupEnumHandle safeGroupEnumHandle = new SafeGroupEnumHandle(m_cluster, m_hGroup, enumType);
			try
			{
				return safeGroupEnumHandle.GetCount();
			}
			finally
			{
				SafeGroupEnumHandle safeGroupEnumHandle2 = safeGroupEnumHandle;
				IDisposable disposable = safeGroupEnumHandle;
				((IDisposable)safeGroupEnumHandle)?.Dispose();
			}
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool DoesContainQuorumResource()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		ClusterResource quorumResource = m_cluster.GetQuorumResource();
		if (quorumResource != null && 0 == string.Compare(quorumResource.GetOwnerGroup().Name, Name, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}

	private unsafe GroupType GetGroupType()
	{
		//Discarded unreachable code: IL_00ee, IL_00f0
		ThreadWatchdog.PerformUIThreadCheck();
		if (IsDeleted)
		{
			return GroupType.Unknown;
		}
		try
		{
			PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
			uint num;
			if (commonProperties.Contains("GroupType"))
			{
				num = (uint)commonProperties["GroupType"].Value;
			}
			else
			{
				ClusterRegistryKey registryKey = GetRegistryKey(RegistryRights.QueryValues);
				object obj = null;
				try
				{
					RegistryValueKind registryValueKind = RegistryValueKind.Unknown;
					if (!registryKey.TryGetValueKind("GroupType", &registryValueKind))
					{
						return GroupType.Unknown;
					}
					if (registryValueKind != RegistryValueKind.DWord)
					{
						return GroupType.Unknown;
					}
					obj = registryKey.GetValue("GroupType");
				}
				finally
				{
					ClusterRegistryKey clusterRegistryKey = registryKey;
					IDisposable disposable = registryKey;
					((IDisposable)registryKey)?.Dispose();
				}
				num = (uint)obj;
			}
			uint num2 = num;
			return (GroupType)Enum.Parse(typeof(GroupType), num2.ToString(CultureInfo.InvariantCulture));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_GetGroupType_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	private unsafe uint GetFlags()
	{
		//Discarded unreachable code: IL_00ac, IL_00ae, IL_00b0, IL_00be
		ThreadWatchdog.PerformUIThreadCheck();
		Monitor.Enter(m_flagsLock);
		try
		{
			if (!m_flags.HasValue)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
				UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer(&num, 4uL);
				Cluster cluster = m_cluster;
				new GroupControlExecutor(this, cluster).ExecuteOutControl(50331657u, unmanagedBuffer);
				uint? flags = *(uint*)unmanagedBuffer.Pointer;
				m_flags = flags;
				DebugLog.LogVerbose("Retrieved flags for group '{0}'. Call stack: {1}", m_lastLoadedName, DebugLog.GetStackTrace());
			}
			return m_flags.Value;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_GetFlags_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			Monitor.Exit(m_flagsLock);
		}
	}

	private GroupState GetCompositeGroupState()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		bool flag = true;
		bool flag2 = true;
		ClusterResourceCollection clusterResourceCollection = GetResources();
		int count;
		try
		{
			count = clusterResourceCollection.Count;
			IEnumerator<ClusterResource> enumerator = clusterResourceCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current.State)
					{
					case ResourceState.Failed:
						return GroupState.Failed;
					case ResourceState.Offline:
						flag = false;
						break;
					case ResourceState.Online:
						flag2 = false;
						break;
					case ResourceState.Initializing:
					case ResourceState.Pending:
					case ResourceState.OnlinePending:
					case ResourceState.OfflinePending:
						return GroupState.Pending;
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
		if (count == 0)
		{
			return State;
		}
		if (flag)
		{
			return GroupState.Online;
		}
		return flag2 ? GroupState.Offline : GroupState.PartialOnline;
	}

	private void LockAccessToHandle([MarshalAs(UnmanagedType.U1)] bool writeAccess)
	{
		if (writeAccess)
		{
			m_hGroupLock.EnterWriteLock();
			return;
		}
		m_hGroupLock.EnterReadLock();
		if (m_hGroup == null)
		{
			m_hGroupLock.ExitReadLock();
			ClusApiExceptionFactory.ThrowObjectDeletedException();
		}
	}

	private void UnlockAccessToHandle([MarshalAs(UnmanagedType.U1)] bool writeAccess)
	{
		if (writeAccess)
		{
			m_hGroupLock.ExitWriteLock();
		}
		else
		{
			m_hGroupLock.ExitReadLock();
		}
	}

	private ClusterResource GetGroupResource(string resourceName, Guid resourceId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return m_cluster.GetResource(resourceName, resourceId, this);
	}

	private ClusterResource GetGroupResource(string resourceName)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return m_cluster.GetResource(resourceName, this);
	}

	private AsyncEnumeration<ClusterResource> BuildResourceAsyncEnum()
	{
		//Discarded unreachable code: IL_0052, IL_0054, IL_0056, IL_0060
		LockAccessToHandle(writeAccess: false);
		try
		{
			Cluster cluster = m_cluster;
			SafeGroupEnumHandle enumHandle = new SafeGroupEnumHandle(cluster, m_hGroup, GroupEnumType.Resource);
			return new AsyncEnumeration<ClusterResource>(GetGroupResource, enumHandle);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_GetResources_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
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

	private ClusterResourceCollection FindResourcesOfType(ClusterResourceCollection resources, string resourceType)
	{
		ClusterResourceCollection clusterResourceCollection = new ClusterResourceCollection();
		foreach (ClusterResource resource in resources)
		{
			if (resource.IsResourceOfType(resourceType))
			{
				clusterResourceCollection.InternalAdd(resource);
			}
		}
		return clusterResourceCollection;
	}

	internal static ClusterGroup CreateObject(Cluster cluster, string groupName, Guid groupId)
	{
		//Discarded unreachable code: IL_00a1, IL_00af
		ThreadWatchdog.PerformUIThreadCheck();
		cluster.m_groupsCreateObjectLock.AcquireReaderLock(-1);
		try
		{
			ClusterGroup clusterGroup = null;
			ClusterGroup groupInstance = cluster.ObjectMgr.GetGroupInstance(groupName);
			if (groupInstance != null)
			{
				return groupInstance;
			}
			LockCookie lockCookie = cluster.m_groupsCreateObjectLock.UpgradeToWriterLock(-1);
			try
			{
				groupInstance = cluster.ObjectMgr.GetGroupInstance(groupName);
				if (groupInstance != null)
				{
					return groupInstance;
				}
				SafeGroupHandle safeGroupHandle = null;
				Guid guid = default(Guid);
				SafeGroupHandle hGroup;
				if (groupId != Guid.Empty)
				{
					hGroup = OpenGroup(cluster, groupId.ToString());
					guid = groupId;
				}
				else
				{
					hGroup = OpenGroup(cluster, groupName);
					guid = GetId(hGroup, cluster);
				}
				return new ClusterGroup(cluster, hGroup, guid, groupName);
			}
			finally
			{
				cluster.m_groupsCreateObjectLock.DowngradeFromWriterLock(ref lockCookie);
			}
		}
		finally
		{
			cluster.m_groupsCreateObjectLock.ReleaseReaderLock();
		}
	}

	internal static ClusterGroup CreateObject(Cluster cluster, string groupName)
	{
		return CreateObject(cluster, groupName, Guid.Empty);
	}

	internal override void Refresh()
	{
		//Discarded unreachable code: IL_004a
		Exception ex = null;
		try
		{
			m_resources = null;
			SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
			SetName(null);
			m_description = null;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_Refresh_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	internal void RefreshResources()
	{
		m_resources = null;
	}

	public override void Close()
	{
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			if (m_closed)
			{
				return;
			}
			m_closed = true;
			DebugLog.LogVerbose("Closing group '{0}'. Stack :{1}", m_lastLoadedName, DebugLog.GetStackTrace());
			m_cluster.RegistryChanged -= OnClusterRegistryChanged;
			LockAccessToHandle(writeAccess: true);
			try
			{
				SafeGroupHandle hGroup = m_hGroup;
				IDisposable disposable = hGroup;
				((IDisposable)hGroup)?.Dispose();
			}
			finally
			{
				m_hGroup = null;
				UnlockAccessToHandle(writeAccess: true);
			}
			if (m_resources != null)
			{
				ClusterResourceCollection resources = m_resources;
				if (resources is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
				m_resources = null;
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "There was an error closing group '{0}'.", m_lastLoadedName);
		}
	}

	internal void OnStateChanged()
	{
		Exception ex = null;
		if (!IsDeleted)
		{
			try
			{
				SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
				raise_StateChanged(this, EventArgs.Empty);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "There was an error reseting the state for group '{0}'.", m_lastLoadedName);
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
		try
		{
			m_resources = null;
			SetName(null);
			m_description = null;
			SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
			Monitor.Enter(m_autoStartLock);
			try
			{
				m_autoStart = null;
			}
			finally
			{
				Monitor.Exit(m_autoStartLock);
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
		bool flag = true;
		m_lifetimeHelper.AquireDisposeLock();
		try
		{
			DebugLog.LogVerbose("Marking group '{0}' as deleted.", m_lastLoadedName);
			m_cluster.ObjectMgr.UnregisterInstance(this);
			Close();
			m_lifetimeHelper.MarkAsDeleted();
			m_lifetimeHelper.MarkAsDisposed();
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
		}
		finally
		{
			m_lifetimeHelper.ReleaseDisposeLock();
		}
		if (flag)
		{
			try
			{
				raise_Deleted(this, EventArgs.Empty);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event Deleted");
			}
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

	[SpecialName]
	protected void raise_GroupTypeChanged(object value0, EventArgs value1)
	{
		_003Cbacking_store_003EGroupTypeChanged?.Invoke(value0, value1);
	}

	public unsafe ClusterResource CreateResource(string resourceName, string resourceId, string resourceTypeName, [MarshalAs(UnmanagedType.U1)] bool separateMonitor)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (resourceName == null)
		{
			throw new ArgumentNullException("resourceName");
		}
		if (resourceTypeName == null)
		{
			throw new ArgumentNullException("resourceTypeName");
		}
		ushort* ptr = InteropHelp.StringToWstr(resourceName);
		ushort* ptr2 = InteropHelp.StringToWstr((!(resourceId != null)) ? string.Empty : resourceId);
		ushort* ptr3 = InteropHelp.StringToWstr(resourceTypeName);
		uint num = 0u;
		ClusterResource clusterResource = null;
		try
		{
			num = (separateMonitor ? 1u : num);
			SafeResourceHandle safeResourceHandle = null;
			SafeResourceHandle safeResourceHandle2 = ((!string.IsNullOrEmpty(resourceId)) ? new SafeResourceHandle(global::_003CModule_003E.CreateClusterResourceWithId(Handle, ptr, ptr2, ptr3, num)) : new SafeResourceHandle(global::_003CModule_003E.CreateClusterResource(Handle, ptr, ptr3, num)));
			if (safeResourceHandle2.IsInvalid)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)global::_003CModule_003E.GetLastError(), Resources.CreateResourceFail_Text, resourceName, m_lastLoadedName);
			}
			m_cluster.RefreshResources();
			return ClusterResource.CreateObject(m_cluster, safeResourceHandle2, resourceName, this);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			InteropHelp.FreeWstr(ptr2);
			InteropHelp.FreeWstr(ptr3);
		}
	}

	public ClusterResource CreateResource(string resourceName, string resourceTypeName, [MarshalAs(UnmanagedType.U1)] bool separateMonitor)
	{
		return CreateResource(resourceName, null, resourceTypeName, separateMonitor);
	}

	public ClusterResource CreateResource(string resourceName, string resourceId, string resourceTypeName)
	{
		return CreateResource(resourceName, resourceId, resourceTypeName, separateMonitor: false);
	}

	public ClusterResource CreateResource(string resourceName, string resourceTypeName)
	{
		return CreateResource(resourceName, null, resourceTypeName, separateMonitor: false);
	}

	public unsafe int Rename(string newName)
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
			uint num = global::_003CModule_003E.SetClusterGroupName(Handle, ptr);
			switch (num)
			{
			case 183u:
				return -2147024713;
			default:
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.RenameGroupFail_Text, m_lastLoadedName, newName);
				break;
			case 0u:
				break;
			}
			m_lastLoadedName = newName;
			m_name = newName;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return 0;
	}

	public void Move(ClusterNode node)
	{
		//Discarded unreachable code: IL_0079
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		try
		{
			if (0 != string.Compare(node.Name, OwnerNodeName, StringComparison.OrdinalIgnoreCase))
			{
				string ownerNodeName = OwnerNodeName;
				BeginMove(node);
				WaitToLeaveNode(ownerNodeName, StateChangeTimeout);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_Move_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe void MoveEx(ClusterNode node, NativeGroupHelp.GroupMoveFlags flags, CClusPropList* propList)
	{
		//Discarded unreachable code: IL_006f
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			if (node == null || 0 != string.Compare(node.Name, OwnerNodeName, StringComparison.OrdinalIgnoreCase))
			{
				string ownerNodeName = OwnerNodeName;
				BeginMoveEx(node, flags, propList);
				WaitToLeaveNode(ownerNodeName, StateChangeTimeout);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_Move_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe void BeginMove(ClusterNode node)
	{
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		_HNODE* ptr = null;
		if (node != null)
		{
			ptr = node.Handle;
		}
		uint num = global::_003CModule_003E.MoveClusterGroup(Handle, ptr);
		if (num != 997 && num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.GroupMoveGroupFail_Text, m_lastLoadedName);
		}
	}

	public unsafe void BeginMoveEx(ClusterNode node, NativeGroupHelp.GroupMoveFlags flags)
	{
		//IL_000a: Expected I, but got I8
		BeginMoveEx(node, flags, null);
	}

	public unsafe void BeginMoveEx(ClusterNode node, NativeGroupHelp.GroupMoveFlags flags, CClusPropList* propList)
	{
		//IL_0013: Expected I, but got I8
		//IL_0016: Expected I, but got I8
		//IL_0036: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		_HNODE* ptr = null;
		byte* ptr2 = null;
		uint num = 0u;
		if (node != null)
		{
			ptr = node.Handle;
		}
		if (propList != null)
		{
			num = (uint)(*(long*)((ulong)(nint)propList + 32uL) + 4);
			ptr2 = (byte*)(*(ulong*)((ulong)(nint)propList + 8uL));
		}
		uint num2 = global::_003CModule_003E.MoveClusterGroupEx(Handle, ptr, (uint)flags, ptr2, num);
		if (num2 != 997 && num2 != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num2, Resources.GroupMoveGroupFail_Text, m_lastLoadedName);
		}
	}

	public unsafe void CancelOperation()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		uint num = global::_003CModule_003E.CancelClusterGroupOperation(Handle, 0u);
		if (num != 997 && num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.GroupMoveGroupFail_Text, m_lastLoadedName);
		}
	}

	public void BringOnline(ClusterNode node)
	{
		//Discarded unreachable code: IL_0052
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		BeginBringOnline(node, force: false, chooseBestNode: false);
		try
		{
			TimeSpan stateChangeTimeout = StateChangeTimeout;
			WaitForDesiredState(GroupState.Online, stateChangeTimeout, requireStateBeReached: true);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_BringOnline_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void BringOnline()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		BringOnline(null);
	}

	public unsafe void BeginBringOnline(ClusterNode node, [MarshalAs(UnmanagedType.U1)] bool force, [MarshalAs(UnmanagedType.U1)] bool chooseBestNode)
	{
		//Discarded unreachable code: IL_00c7
		//IL_004e: Expected I, but got I8
		//IL_007d: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			GroupState compositeGroupState = GetCompositeGroupState();
			if (ClusterItem.CachingDisabled || compositeGroupState != 0 || (node != null && 0 != string.Compare(node.Name, OwnerNodeName, StringComparison.OrdinalIgnoreCase)))
			{
				SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
				uint num = 0u;
				_HNODE* ptr = null;
				if (node != null)
				{
					ptr = node.Handle;
				}
				uint num2 = 0u;
				num2 = (force ? 1u : num2);
				if (chooseBestNode)
				{
					num2 |= 4u;
				}
				num = ((num2 == 0) ? global::_003CModule_003E.OnlineClusterGroup(Handle, ptr) : global::_003CModule_003E.OnlineClusterGroupEx(Handle, ptr, num2, null, 0u));
				if (num != 0 && num != 997)
				{
					ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num);
				}
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_BringOnline_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void BeginBringOnline(ClusterNode node)
	{
		BeginBringOnline(node, force: false, chooseBestNode: false);
	}

	public void BeginBringOnline([MarshalAs(UnmanagedType.U1)] bool force, [MarshalAs(UnmanagedType.U1)] bool chooseBestNode)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		BeginBringOnline(null, force, chooseBestNode);
	}

	public void BeginBringOnline()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		BeginBringOnline(null, force: false, chooseBestNode: false);
	}

	public void WaitForDesiredState(GroupState state, TimeSpan timeout, [MarshalAs(UnmanagedType.U1)] bool requireStateBeReached)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		TimeSpan timeSpan = default(TimeSpan);
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, 250);
		if (timeSpan < timeout)
		{
			do
			{
				if (GetCompositeGroupState() != state)
				{
					Thread.Sleep(timeSpan2);
					timeSpan += timeSpan2;
					GroupState uncachedState = GetUncachedState();
					if (uncachedState != state)
					{
						if (uncachedState != GroupState.Pending)
						{
							throw ExceptionHelp.Build<ApplicationException>(new string[2]
							{
								Resources.Group_NotInDesiredState_Text,
								m_lastLoadedName
							});
						}
						continue;
					}
					return;
				}
				return;
			}
			while (timeSpan < timeout);
		}
		if (requireStateBeReached)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.StateChange_Timeout_Text,
				m_lastLoadedName
			});
		}
	}

	public void WaitForDesiredState(GroupState state, TimeSpan timeout)
	{
		WaitForDesiredState(state, timeout, requireStateBeReached: true);
	}

	public void TakeOffline(ValueType timeout)
	{
		//Discarded unreachable code: IL_0050
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		BeginTakeOffline(force: false);
		try
		{
			TimeSpan timeout2 = (TimeSpan)timeout;
			WaitForDesiredState(GroupState.Offline, timeout2, requireStateBeReached: true);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_TakeOffline_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void TakeOffline()
	{
		TakeOffline(StateChangeTimeout);
	}

	public unsafe void BeginTakeOffline([MarshalAs(UnmanagedType.U1)] bool force)
	{
		//Discarded unreachable code: IL_0076
		//IL_002d: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
			uint num = 0u;
			num = ((!force) ? global::_003CModule_003E.OfflineClusterGroup(Handle) : global::_003CModule_003E.OfflineClusterGroupEx(Handle, 1u, null, 0u));
			if (num != 0 && num != 997)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num);
			}
		}
		catch (Win32Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_TakeOffline_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public void BeginTakeOffline()
	{
		BeginTakeOffline(force: false);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsQueued()
	{
		Property property = null;
		PropertyCollection commonProperties = GetCommonProperties(PropertyCollectionSet.ReadOnly);
		property = null;
		if (commonProperties.TryGetProperty("StatusInformation", out property))
		{
			return ((ApplicationStatusInformation)(object)(ApplicationStatusInformation)(ulong)property.Value).HasFlag(ApplicationStatusInformation.Queued);
		}
		return false;
	}

	public void WaitForFinishOnline(TimeSpan timeout)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		TimeSpan timeSpan = default(TimeSpan);
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 1);
		TimeSpan timeSpan3 = new TimeSpan(0, 0, 10);
		if (timeSpan < timeout)
		{
			do
			{
				GroupState compositeGroupState = GetCompositeGroupState();
				switch (compositeGroupState)
				{
				case GroupState.PartialOnline:
					if (State == GroupState.Online)
					{
						return;
					}
					goto default;
				default:
					if (compositeGroupState == GroupState.Failed)
					{
						string ownerNodeName = OwnerNodeName;
						timeSpan += timeSpan3;
						Thread.Sleep(timeSpan3);
						if (GetCompositeGroupState() == GroupState.Failed && 0 == string.Compare(ownerNodeName, OwnerNodeName, StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
					}
					else
					{
						timeSpan += timeSpan2;
						Thread.Sleep(timeSpan2);
					}
					break;
				case GroupState.Online:
					return;
				}
			}
			while (timeSpan < timeout);
		}
		throw ExceptionHelp.Build<ApplicationException>(new string[2]
		{
			Resources.StateChange_Timeout_Text,
			m_lastLoadedName
		});
	}

	public unsafe void Delete([MarshalAs(UnmanagedType.U1)] bool force, [MarshalAs(UnmanagedType.U1)] bool allowDetach)
	{
		//Discarded unreachable code: IL_0284
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			uint num;
			if (force)
			{
				ClusterResourceCollection resources = GetResources();
				if (GroupType == GroupType.VirtualMachine)
				{
					string resourceType = "Virtual Machine Configuration";
					IEnumerator<ClusterResource> enumerator = FindResourcesOfType(resources, resourceType).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ClusterResource current = enumerator.Current;
							try
							{
								if (!allowDetach)
								{
									current.TakeOffline();
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
									IEnumerator<ClusterResource> enumerator2 = current.GetDependencies().GetEnumerator();
									try
									{
										while (enumerator2.MoveNext())
										{
											ClusterResource current2 = enumerator2.Current;
											if (current2.IsStorage)
											{
												current2.BringOnline();
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
								catch (Exception caughtException2)
								{
									ExceptionHelp.LogException(caughtException2, "There was an error ensuing the storage on which the virtual machine cfg resource depends is online.");
								}
								current.Delete();
							}
							catch (Exception caughtException3)
							{
								ExceptionHelp.LogException(caughtException3, "There was an error deleting the virtual machine cfg resource.");
							}
						}
					}
					finally
					{
						IEnumerator<ClusterResource> enumerator4 = enumerator;
						IDisposable disposable2 = enumerator;
						enumerator?.Dispose();
					}
				}
				if (GroupType == GroupType.StandAloneDfs)
				{
					string resourceType2 = "Distributed File System";
					IEnumerator<ClusterResource> enumerator5 = FindResourcesOfType(resources, resourceType2).GetEnumerator();
					try
					{
						while (enumerator5.MoveNext())
						{
							ClusterResource current3 = enumerator5.Current;
							try
							{
								if (!allowDetach)
								{
									current3.TakeOffline();
								}
							}
							catch (Exception caughtException4)
							{
								ExceptionHelp.LogException(caughtException4, "There was an error taking the DFS namespace resource offline.");
							}
							try
							{
								current3.Delete();
							}
							catch (Exception caughtException5)
							{
								ExceptionHelp.LogException(caughtException5, "There was an error deleting the DFS namespace resource.");
							}
						}
					}
					finally
					{
						IEnumerator<ClusterResource> enumerator6 = enumerator5;
						IDisposable disposable3 = enumerator5;
						enumerator5?.Dispose();
					}
				}
				ClusterResourceCollection clusterResourceCollection = new ClusterResourceCollection();
				IEnumerator<ClusterResource> enumerator7 = resources.GetEnumerator();
				try
				{
					while (enumerator7.MoveNext())
					{
						ClusterResource current4 = enumerator7.Current;
						if (current4.IsStorage)
						{
							try
							{
								current4.TakeOffline();
							}
							catch (Exception caughtException6)
							{
								ExceptionHelp.LogException(caughtException6, "Could not take storage resource '{0}' offline.", current4.DisplayName);
							}
							current4.RemoveAllDependencyLinks();
							Cluster cluster = m_cluster;
							cluster.MoveStorageToAvailableStorage(current4, this);
							clusterResourceCollection.InternalAdd(current4);
						}
					}
				}
				finally
				{
					IEnumerator<ClusterResource> enumerator8 = enumerator7;
					IDisposable disposable4 = enumerator7;
					enumerator7?.Dispose();
				}
				IEnumerator<ClusterResource> enumerator9 = clusterResourceCollection.GetEnumerator();
				try
				{
					while (enumerator9.MoveNext())
					{
						ClusterResource current5 = enumerator9.Current;
						try
						{
							current5.BeginBringOnline();
						}
						catch (Exception caughtException7)
						{
							ExceptionHelp.LogException(caughtException7, "Failed to bring storage resource online after moving back to available storage.");
						}
					}
				}
				finally
				{
					IEnumerator<ClusterResource> enumerator10 = enumerator9;
					IDisposable disposable5 = enumerator9;
					enumerator9?.Dispose();
				}
				if (force)
				{
					num = global::_003CModule_003E.DestroyClusterGroup(Handle);
					goto IL_023d;
				}
			}
			num = global::_003CModule_003E.DeleteClusterGroup(Handle);
			goto IL_023d;
			IL_023d:
			uint num2 = num;
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num);
			}
			m_cluster.RefreshGroups();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_Delete_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool IsInDetachableState()
	{
		if (m_cluster.CurrentVersion <= ClusterVersion.Windows7)
		{
			return false;
		}
		foreach (ClusterResource resource in GetResources())
		{
			if (resource.State != ResourceState.Offline && resource.State != ResourceState.Failed && (resource.GetCharacteristics() & 0x400) == 0)
			{
				return false;
			}
		}
		return true;
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
					Resources.ClusterGroup_GetResources_Fail_Text,
					m_lastLoadedName
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

	public long GetResourceCount()
	{
		//Discarded unreachable code: IL_002b, IL_002d
		try
		{
			return GetItemCount(GroupEnumType.Resource);
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_GetResourceCount_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public ClusterNodeCollection GetPreferredOwnerNodes()
	{
		//Discarded unreachable code: IL_006d, IL_006f, IL_0071, IL_007b
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		LockAccessToHandle(writeAccess: false);
		try
		{
			Cluster cluster = m_cluster;
			SafeGroupEnumHandle enumHandle = new SafeGroupEnumHandle(cluster, m_hGroup, GroupEnumType.Node);
			return new ClusterNodeCollection(new AsyncEnumeration<ClusterNode>(m_cluster.GetNode, enumHandle, dontSort: true));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_GetPreferredOwners_Fail_Text,
				m_lastLoadedName
			});
		}
		finally
		{
			UnlockAccessToHandle(writeAccess: false);
		}
	}

	public unsafe void SetPreferredOwnerNodes(ICollection<ClusterNode> ownerNodes)
	{
		//IL_0017: Expected I, but got I8
		//IL_0078: Expected I8, but got I
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		uint num = 0u;
		uint num2 = 0u;
		_HNODE** ptr = null;
		if (ownerNodes == null)
		{
			throw new ArgumentNullException("ownerNodes");
		}
		try
		{
			ulong num3 = (ulong)ownerNodes.Count;
			ptr = (_HNODE**)global::_003CModule_003E.new_005B_005D((num3 > 2305843009213693951L) ? ulong.MaxValue : (num3 * 8));
			IEnumerator<ClusterNode> enumerator = ownerNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterNode current = enumerator.Current;
					*(long*)((long)num2 * 8L + (nint)ptr) = (nint)current.Handle;
					num2++;
				}
			}
			finally
			{
				IEnumerator<ClusterNode> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
			num = global::_003CModule_003E.SetClusterGroupNodeList(Handle, num2, ptr);
		}
		finally
		{
			if (ptr != null)
			{
				global::_003CModule_003E.delete_005B_005D(ptr);
			}
		}
		if (num != 0)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.SetPreferredOwnerNodesFail_Text, m_lastLoadedName);
		}
	}

	public ClusterNode GetOwnerNode()
	{
		//Discarded unreachable code: IL_006e
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		TimeSpan timeSpan = default(TimeSpan);
		TimeSpan timeSpan2 = new TimeSpan(0, 0, 5);
		TimeSpan timeSpan3 = new TimeSpan(0, 0, 1);
		while (timeSpan < timeSpan2)
		{
			try
			{
				return m_cluster.GetNode(OwnerNodeName);
			}
			catch (Exception)
			{
				if (GetCompositeGroupState() == GroupState.Failed)
				{
					timeSpan += timeSpan3;
					Thread.Sleep(timeSpan3);
					SetState(GroupState.Unknown, null, GroupNotification.DoNotRaise);
					continue;
				}
				throw;
			}
		}
		return null;
	}

	public override ControlExecutor GetControlExecutor()
	{
		Cluster cluster = m_cluster;
		return new GroupControlExecutor(this, cluster);
	}

	public override PropertyCollection GetCommonProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0031, IL_0054, IL_0056
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Common, propSet);
		}
		catch (Win32Exception innerException)
		{
			throw ExceptionHelp.Build(innerException, Resources.ClusterGroup_GetCommonProperties_Fail_Text, m_lastLoadedName);
		}
		catch (Exception innerException2)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException2, new string[2]
			{
				Resources.ClusterGroup_GetCommonProperties_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public override PropertyCollection GetPrivateProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_0031, IL_0054, IL_0056
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return new PropertyCollection(this, ClusterPropertyScope.Private, propSet);
		}
		catch (Win32Exception innerException)
		{
			throw ExceptionHelp.Build(innerException, Resources.ClusterGroup_GetPrivateProperties_Fail_Text, m_lastLoadedName);
		}
		catch (Exception innerException2)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException2, new string[2]
			{
				Resources.ClusterGroup_GetPrivateProperties_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public override string ToString()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		return Name;
	}

	public void SetGroupType(GroupType type)
	{
		//Discarded unreachable code: IL_0073
		ThreadWatchdog.PerformUIThreadCheck();
		if (IsDeleted)
		{
			return;
		}
		try
		{
			if (global::_003CModule_003E.IsGroupTypeValid((CLUSGROUP_TYPE)type) == 0)
			{
				throw ExceptionHelp.Build<InvalidOperationException>(new string[1] { Resources.GroupTypeInvalid_Text });
			}
			ClusterRegistryKey registryKey = GetRegistryKey(RegistryRights.SetValue);
			try
			{
				registryKey.SetValue("GroupType", (uint)type);
			}
			finally
			{
				ClusterRegistryKey clusterRegistryKey = registryKey;
				IDisposable disposable = registryKey;
				((IDisposable)registryKey)?.Dispose();
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_SetGroupType_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	public unsafe ClusterRegistryKey GetRegistryKey(RegistryRights rights)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		HKEY__* clusterGroupKey = global::_003CModule_003E.GetClusterGroupKey(Handle, ClusterRegistryKey.RegistryRightsToRegSam(rights));
		uint lastError = global::_003CModule_003E.GetLastError();
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterGroupKey);
		if (safeRegistryHandle.IsInvalid)
		{
			throw ExceptionHelp.Build((int)lastError, Resources.ClusterGroup_GetRegistryKeyFailed_Text, m_lastLoadedName);
		}
		return new ClusterRegistryKey(m_cluster, safeRegistryHandle);
	}

	public ClusterResource TryFindGroupNetworkName()
	{
		//Discarded unreachable code: IL_00d4, IL_00d6
		ThreadWatchdog.PerformUIThreadCheck();
		string name = Name;
		string value = name + " ";
		ClusterResource result = null;
		try
		{
			IEnumerator<ClusterResource> enumerator = GetResources().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					string resourceType = "Network Name";
					if (!current.IsResourceOfType(resourceType))
					{
						string resourceType2 = "Distributed Network Name";
						if (!current.IsResourceOfType(resourceType2))
						{
							continue;
						}
					}
					if (current.Name.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
					{
						result = current;
					}
					string strA = (string)current.GetPrivateProperties(PropertyCollectionSet.ReadWrite)["DnsName"].Value;
					if (0 == string.Compare(strA, name, StringComparison.CurrentCultureIgnoreCase))
					{
						result = current;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<ClusterResource> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
			return result;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_FindNetworkName_Fail_Text,
				m_lastLoadedName
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool HasNetworkName()
	{
		//Discarded unreachable code: IL_006e
		ThreadWatchdog.PerformUIThreadCheck();
		bool result = false;
		try
		{
			IEnumerator<ClusterResource> enumerator = GetResources().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ClusterResource current = enumerator.Current;
					string resourceType = "Network Name";
					if (current.IsResourceOfType(resourceType))
					{
						result = true;
						break;
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
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ClusterGroup_HasNetworkName_Fail_Text,
				m_lastLoadedName
			});
		}
		return result;
	}

	public void AddDiskToGroup(ClusterResource disk)
	{
		disk.ChangeGroup2(this);
	}

	public unsafe void SetFailoverCount(uint count)
	{
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer(&count, 4uL);
		try
		{
			GroupControlExecutor groupControlExecutor = new GroupControlExecutor(this, m_cluster);
			groupControlExecutor.ExecuteInControl(groupControlExecutor.SetFailoverCountCode, unmanagedBuffer);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}
}

