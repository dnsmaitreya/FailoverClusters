using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;

namespace KDDSL.ServerClusters;

public class ClusterResourceType : ClusterItem
{
	private bool m_deleted;

	private string m_name;

	private Guid m_Id;

	private Cluster m_cluster;

	private volatile uint m_characteristics;

	private volatile bool m_characteristicsValid;

	private ObjectLifetimeHelper m_lifetimeHelper;

	private object m_classInfoLockObject;

	private static object m_creationLockObject = new object();

	private ClusterResourceClass? m_clusResClass;

	private EventHandler _003Cbacking_store_003EPropertiesChanged;

	private EventHandler _003Cbacking_store_003EDeleted;

	public bool IsNetwork
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsNetworkClassResourceType();
		}
	}

	public bool IsStorage
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return IsStorageClassResourceType();
		}
	}

	public bool IsSingleClusterInstance
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((GetCharacteristics() >> 6) & 1u) != 0;
		}
	}

	public bool IsQuorumCapable
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)(GetCharacteristics() & 1u) != 0;
		}
	}

	public Cluster Cluster => m_cluster;

	public override bool IsDeleted
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			int num = ((m_deleted || m_lifetimeHelper.IsDeleted) ? 1 : 0);
			return (byte)num != 0;
		}
	}

	public override Guid Id => m_Id;

	public string DisplayName => GetDisplayName();

	public override string Name => m_name;

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

	private ClusterResourceType(Cluster cluster, Guid id, string resourceTypeName)
	{
		try
		{
			if (cluster == null)
			{
				throw new ArgumentNullException("cluster");
			}
			m_name = resourceTypeName;
			m_cluster = cluster;
			m_lifetimeHelper = new ObjectLifetimeHelper();
			m_Id = id;
			m_characteristics = 0u;
			m_characteristicsValid = false;
			m_deleted = false;
			m_classInfoLockObject = new object();
			m_clusResClass = null;
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

	private static Guid GetId(string resourceTypeName)
	{
		string text = resourceTypeName.ToLowerInvariant();
		int hashCode = text.GetHashCode();
		byte[] array = new byte[8];
		short b = (short)text[0];
		short c = 0;
		if (resourceTypeName.Length > 1)
		{
			c = (short)text[1];
			int num = Math.Min(8, text.Length - 2);
			int num2 = 0;
			if (0 < num)
			{
				do
				{
					byte b2 = (byte)text[num2 + 2];
					array[num2] = b2;
					num2++;
				}
				while (num2 < num);
			}
		}
		return new Guid(hashCode, b, c, array);
	}

	private string GetDisplayName()
	{
		//Discarded unreachable code: IL_0036
		ThreadWatchdog.PerformUIThreadCheck();
		string text = null;
		try
		{
			return (string)GetCommonProperties(PropertyCollectionSet.ReadWrite)["Name"].Value;
		}
		catch (Exception caughtException)
		{
			if (ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
			{
				return Resources.ResourceTypeUnavailable_Text;
			}
			throw;
		}
	}

	private unsafe ClusterResourceClass GetResourceTypeClass()
	{
		//Discarded unreachable code: IL_008b, IL_008d
		//IL_0033: Expected I4, but got I8
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
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLUS_RESOURCE_CLASS_INFO cLUS_RESOURCE_CLASS_INFO);
				// IL initblk instruction
				System.Runtime.CompilerServices.Unsafe.InitBlock(ref cLUS_RESOURCE_CLASS_INFO, 0, 8);
				UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&cLUS_RESOURCE_CLASS_INFO, 8uL);
				ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(m_name, m_cluster);
				resourceTypeControlExecutor.ExecuteOutControl(resourceTypeControlExecutor.GetTypeClassInfoCode, outputBuffer);
				ClusterResourceClass? clusResClass = *(ClusterResourceClass*)(&cLUS_RESOURCE_CLASS_INFO);
				m_clusResClass = clusResClass;
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ResourceType_GetClassInfo_Fail_Text,
				m_name
			});
		}
		finally
		{
			Monitor.Exit(m_classInfoLockObject);
		}
		return m_clusResClass.Value;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsStorageClassResourceType()
	{
		//Discarded unreachable code: IL_0025
		try
		{
			return GetResourceTypeClass() == ClusterResourceClass.Storage;
		}
		catch (Exception caughtException)
		{
			if (!ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
			{
				throw;
			}
			ExceptionHelp.LogException(caughtException, "Error calling GetResourceTypeClass().");
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool IsNetworkClassResourceType()
	{
		//Discarded unreachable code: IL_0025
		try
		{
			return GetResourceTypeClass() == ClusterResourceClass.Network;
		}
		catch (Exception caughtException)
		{
			if (!ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
			{
				throw;
			}
			ExceptionHelp.LogException(caughtException, "Error calling GetResourceTypeClass().");
		}
		return false;
	}

	internal static ClusterResourceType CreateObject(Cluster cluster, string resourceTypeName, Guid resourceTypeId)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		Guid id = ((!(resourceTypeId != Guid.Empty)) ? GetId(resourceTypeName) : resourceTypeId);
		Monitor.Enter(m_creationLockObject);
		ClusterResourceType clusterResourceType = null;
		try
		{
			clusterResourceType = cluster.ObjectMgr.GetResourceTypeInstance(id);
			if (clusterResourceType == null)
			{
				clusterResourceType = new ClusterResourceType(cluster, id, resourceTypeName);
			}
		}
		finally
		{
			Monitor.Exit(m_creationLockObject);
		}
		return clusterResourceType;
	}

	internal static ClusterResourceType CreateObject(Cluster cluster, string resourceTypeName)
	{
		return CreateObject(cluster, resourceTypeName, Guid.Empty);
	}

	internal void OnPropertiesChanged()
	{
		Exception ex = null;
		Exception ex2 = null;
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
			goto IL_0022;
		}
		goto IL_0026;
		IL_0022:
		if (!flag)
		{
			return;
		}
		goto IL_0026;
		IL_0026:
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
			ExceptionHelp.LogException(caughtException, "Exception thrown while raising the event Deleted");
		}
	}

	internal override void Refresh()
	{
	}

	public override void Close()
	{
		ThreadWatchdog.PerformUIThreadCheck();
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

	public unsafe void Delete()
	{
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(m_name);
			uint num = global::_003CModule_003E.DeleteClusterResourceType(m_cluster.Handle, ptr);
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)num, Resources.DeleteResourceTypeFail_Text, m_name);
			}
			m_deleted = true;
			m_cluster.RefreshResourceTypes();
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	public ClusterNodeCollection GetPossibleOwnerNodes()
	{
		//Discarded unreachable code: IL_0057, IL_0059
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			SafeResourceTypeEnumHandle enumHandle = NativeMethods.ClusterResourceTypeOpenEnum(this, ResourceTypeEnumType.Node, ResourceTypeEnumOptions.None);
			return new ClusterNodeCollection(new AsyncEnumeration<ClusterNode>(m_cluster.GetNode, enumHandle));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ResourceType_GetPossibleOwners_Fail_Text,
				Name
			});
		}
	}

	public ClusterResourceCollection GetResources()
	{
		//Discarded unreachable code: IL_0057, IL_0059
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		try
		{
			SafeResourceTypeEnumHandle enumHandle = NativeMethods.ClusterResourceTypeOpenEnum(this, ResourceTypeEnumType.Resources, ResourceTypeEnumOptions.None);
			return new ClusterResourceCollection(new AsyncEnumeration<ClusterResource>(m_cluster.GetResource, enumHandle));
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ResourceType_GetPossibleOwners_Fail_Text,
				Name
			});
		}
	}

	public IEnumerable<string> GetResourcesName()
	{
		//Discarded unreachable code: IL_007c, IL_009d, IL_00be, IL_00c0, IL_00c2, IL_00ce
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeResourceTypeEnumHandle safeResourceTypeEnumHandle = null;
		List<string> list = new List<string>();
		try
		{
			safeResourceTypeEnumHandle = NativeMethods.ClusterResourceTypeOpenEnum(this, ResourceTypeEnumType.Resources, ResourceTypeEnumOptions.None);
			while (safeResourceTypeEnumHandle.MoveNext())
			{
				ClusterEnumItem clusterEnumItem = (ClusterEnumItem)safeResourceTypeEnumHandle.Current;
				if (clusterEnumItem != null)
				{
					list.Add(clusterEnumItem.Name);
				}
			}
			list.Sort(StringComparer.InvariantCultureIgnoreCase);
			return list;
		}
		catch (ApplicationException innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				Name
			});
		}
		catch (Win32Exception innerException2)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException2, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				Name
			});
		}
		catch (ClusterBaseException innerException3)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException3, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				Name
			});
		}
		finally
		{
			safeResourceTypeEnumHandle?.Close();
		}
	}

	public IEnumerable<Guid> GetResourcesId()
	{
		//Discarded unreachable code: IL_00b7, IL_00d8, IL_00f9, IL_00fb, IL_00fd, IL_0109
		ThreadWatchdog.PerformUIThreadCheck();
		m_lifetimeHelper.CheckObjectState();
		SafeResourceEnumExHandle safeResourceEnumExHandle = null;
		List<Guid> list = new List<Guid>();
		try
		{
			List<string> list2 = new List<string>();
			string text = "Type";
			list2.Add(text);
			safeResourceEnumExHandle = new SafeResourceEnumExHandle(m_cluster, list2, null);
			while (safeResourceEnumExHandle.MoveNext())
			{
				ClusterResourceExEnumItem clusterResourceExEnumItem = (ClusterResourceExEnumItem)safeResourceEnumExHandle.Current;
				if (clusterResourceExEnumItem != null && (string)clusterResourceExEnumItem.rwCommonProperties[text].Value == m_name)
				{
					list.Add(clusterResourceExEnumItem.ID);
				}
			}
			return list;
		}
		catch (ApplicationException innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				Name
			});
		}
		catch (Win32Exception innerException2)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException2, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				Name
			});
		}
		catch (ClusterBaseException innerException3)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException3, new string[2]
			{
				Resources.Cluster_GetResources_Failed_Text,
				Name
			});
		}
		finally
		{
			safeResourceEnumExHandle?.Close();
		}
	}

	public override ControlExecutor GetControlExecutor()
	{
		return new ResourceTypeControlExecutor(m_name, m_cluster);
	}

	public unsafe uint GetCharacteristics()
	{
		//Discarded unreachable code: IL_00a4, IL_00a6, IL_00a8, IL_00b1
		//IL_000a: Expected I, but got I8
		Exception ex = null;
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* str = null;
		m_lifetimeHelper.CheckObjectState();
		try
		{
			if (m_characteristicsValid)
			{
				return m_characteristics;
			}
			str = InteropHelp.StringToWstr(Name);
			uint characteristics = 0u;
			UnmanagedBuffer outputBuffer = new UnmanagedBuffer(&characteristics, 4uL);
			ResourceTypeControlExecutor resourceTypeControlExecutor = new ResourceTypeControlExecutor(m_name, m_cluster);
			resourceTypeControlExecutor.ExecuteOutControl(resourceTypeControlExecutor.GetCharacteristicsCode, outputBuffer);
			m_characteristics = characteristics;
			m_characteristicsValid = true;
			return m_characteristics;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.ResourceType_GetCharacteristics_Fail_Text,
				Name
			});
		}
		finally
		{
			InteropHelp.FreeWstr(str);
		}
	}

	public override PropertyCollection GetCommonProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_003a, IL_003c
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
				Resources.ResourceType_GetCommonProperties_Fail_Text,
				Name
			});
		}
	}

	public override PropertyCollection GetPrivateProperties(PropertyCollectionSet propSet)
	{
		//Discarded unreachable code: IL_003a, IL_003c
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
				Resources.ResourceType_GetPrivateProperties_Fail_Text,
				Name
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
		ushort* ptr = InteropHelp.StringToWstr(Name);
		HKEY__* clusterResourceTypeKey;
		uint lastError;
		try
		{
			clusterResourceTypeKey = global::_003CModule_003E.GetClusterResourceTypeKey(m_cluster.Handle, ptr, ClusterRegistryKey.RegistryRightsToRegSam(rights));
			lastError = global::_003CModule_003E.GetLastError();
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		SafeRegistryHandle safeRegistryHandle = new SafeRegistryHandle(clusterResourceTypeKey);
		if (safeRegistryHandle.IsInvalid)
		{
			ClusApiExceptionFactory.CreateAndThrow(m_cluster, (int)lastError, Resources.ClusterResourceType_GetRegistryKeyFailed_Text, Name);
		}
		return new ClusterRegistryKey(m_cluster, safeRegistryHandle);
	}

	public static int CompareResourceTypeName(string nameA, string nameB)
	{
		return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
	}
}
