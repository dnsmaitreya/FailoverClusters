using System;
using System.Collections.Generic;

namespace MS.Internal.ServerClusters;

internal class SafeResourceEnumExHandle : SafeEnumHandleBaseTemplate_003CMS_003A_003AInternal_003A_003AServerClusters_003A_003AClusterResourceExEnumItem_0020_005E_002C_CLUSTER_RESOURCE_ENUM_ITEM_0020_002A_003E
{
	public unsafe SafeResourceEnumExHandle(Cluster cluster, IEnumerable<string> rwCommonProperties, IEnumerable<string> roCommonProperties)
		: base(cluster)
	{
		//IL_000a: Expected I, but got I8
		//IL_000d: Expected I, but got I8
		//IL_0010: Expected I, but got I8
		try
		{
			ushort* ptr = null;
			ushort* ptr2 = null;
			_HRESENUMEX* ptr3 = null;
			try
			{
				int num = 0;
				int num2 = 0;
				ptr = InteropHelp.ConvertStringCollectionToPWSTR(rwCommonProperties, &num);
				ptr2 = InteropHelp.ConvertStringCollectionToPWSTR(roCommonProperties, &num2);
				ptr3 = global::_003CModule_003E.ClusterResourceOpenEnumEx(base.cluster.Handle, ptr, (uint)num, ptr2, (uint)num2, 0u);
				if (ptr3 == null)
				{
					uint lastError = global::_003CModule_003E.GetLastError();
					ClusApiExceptionFactory.CreateAndThrow(base.Cluster, (int)lastError, Resources.ClusterResourceOpenEnum_Fail_Text);
				}
			}
			finally
			{
				InteropHelp.FreeArray(ptr);
				InteropHelp.FreeArray(ptr2);
			}
			IntPtr intPtr = (IntPtr)ptr3;
			SetHandle(intPtr);
			Construct();
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	public unsafe SafeResourceEnumExHandle(Cluster cluster)
		: base(cluster)
	{
		//IL_001e: Expected I, but got I8
		//IL_001e: Expected I, but got I8
		try
		{
			_HRESENUMEX* ptr = global::_003CModule_003E.ClusterResourceOpenEnumEx(base.cluster.Handle, null, 0u, null, 0u, 0u);
			if (ptr == null)
			{
				uint lastError = global::_003CModule_003E.GetLastError();
				ClusApiExceptionFactory.CreateAndThrow(base.Cluster, (int)lastError, Resources.ClusterResourceOpenEnum_Fail_Text);
			}
			IntPtr intPtr = (IntPtr)ptr;
			SetHandle(intPtr);
			Construct();
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	protected unsafe _HRESENUMEX* DangerousGetEnumHandle()
	{
		return (_HRESENUMEX*)handle.ToPointer();
	}

	protected unsafe override uint InternalReleaseHandle()
	{
		return global::_003CModule_003E.ClusterResourceCloseEnumEx(DangerousGetEnumHandle());
	}

	protected unsafe override uint FetchItemCount()
	{
		return global::_003CModule_003E.ClusterResourceGetEnumCountEx(DangerousGetEnumHandle());
	}

	protected unsafe override ClusterResourceExEnumItem FetchItem(uint index)
	{
		//IL_0075: Expected I, but got I8
		//IL_007c: Expected I, but got I8
		//IL_0083: Expected I, but got I8
		//IL_008a: Expected I, but got I8
		//IL_0091: Expected I, but got I8
		//IL_0097: Expected I, but got I8
		//IL_0141: Expected I, but got I8
		//IL_0168: Expected I, but got I8
		//IL_0197: Expected I, but got I8
		//IL_01a8: Expected I, but got I8
		//IL_01b4: Expected I, but got I8
		//IL_01d6: Expected I, but got I8
		//IL_01ef: Expected I, but got I8
		//IL_0211: Expected I, but got I8
		ClusterResourceExEnumItem clusterResourceExEnumItem = null;
		_CLUSTER_RESOURCE_ENUM_ITEM* ptr = (_CLUSTER_RESOURCE_ENUM_ITEM*)global::_003CModule_003E.malloc(864uL);
		uint num;
		if (ptr != null)
		{
			*(int*)ptr = 1;
			*(int*)((ulong)(nint)ptr + 4uL) = 128;
			*(int*)((ulong)(nint)ptr + 16uL) = 128;
			*(int*)((ulong)(nint)ptr + 32uL) = 128;
			*(int*)((ulong)(nint)ptr + 48uL) = 128;
			*(int*)((ulong)(nint)ptr + 64uL) = 128;
			*(int*)((ulong)(nint)ptr + 80uL) = 128;
			num = FetchItem(index, ptr);
		}
		else
		{
			num = 8u;
		}
		try
		{
			if (num == 234)
			{
				_CLUSTER_RESOURCE_ENUM_ITEM* ptr2 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 80uL);
				_CLUSTER_RESOURCE_ENUM_ITEM* ptr3 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 64uL);
				_CLUSTER_RESOURCE_ENUM_ITEM* ptr4 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 48uL);
				_CLUSTER_RESOURCE_ENUM_ITEM* ptr5 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 32uL);
				_CLUSTER_RESOURCE_ENUM_ITEM* ptr6 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 16uL);
				_CLUSTER_RESOURCE_ENUM_ITEM* ptr7 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 4uL);
				_CLUSTER_RESOURCE_ENUM_ITEM* ptr8 = (_CLUSTER_RESOURCE_ENUM_ITEM*)global::_003CModule_003E.malloc((ulong)((uint)(*(int*)ptr6) + ((long)(uint)(*(int*)ptr3) + (long)(uint)(*(int*)ptr5) + (uint)(*(int*)ptr7) + (uint)(*(int*)ptr2) + (uint)(*(int*)ptr4)) + 96));
				if (ptr8 != null)
				{
					*(int*)ptr8 = 1;
					*(int*)((ulong)(nint)ptr8 + 4uL) = *(int*)ptr7;
					*(int*)((ulong)(nint)ptr8 + 16uL) = *(int*)ptr6;
					*(int*)((ulong)(nint)ptr8 + 32uL) = *(int*)ptr5;
					*(int*)((ulong)(nint)ptr8 + 48uL) = *(int*)ptr4;
					*(int*)((ulong)(nint)ptr8 + 64uL) = *(int*)ptr3;
					*(int*)((ulong)(nint)ptr8 + 80uL) = *(int*)ptr2;
				}
				global::_003CModule_003E.free(ptr);
				ptr = ptr8;
				num = ((ptr8 == null) ? 8u : FetchItem(index, ptr8));
			}
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(base.Cluster, (int)num, Resources.EnumFail_Text);
			}
			if (*(int*)ptr == 0)
			{
				return null;
			}
			clusterResourceExEnumItem = new ClusterResourceExEnumItem();
			string g = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)ptr + 8uL))).PadLeft(32, '0');
			Guid iD = new Guid(g);
			clusterResourceExEnumItem.ID = iD;
			string g2 = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)ptr + 56uL))).PadLeft(32, '0');
			Guid groupGuid = new Guid(g2);
			clusterResourceExEnumItem.groupGuid = groupGuid;
			clusterResourceExEnumItem.Index = (int)index;
			clusterResourceExEnumItem.Name = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)ptr + 24uL)));
			clusterResourceExEnumItem.groupName = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)ptr + 40uL)));
			_CLUSTER_RESOURCE_ENUM_ITEM* ptr9 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 72uL);
			if (*(long*)ptr9 != 0L)
			{
				clusterResourceExEnumItem.rwCommonProperties = PropertyCollection.ConvertPropertyListToDictionary((CLUSPROP_LIST*)(*(ulong*)ptr9), *(uint*)((ulong)(nint)ptr + 64uL), isReadOnly: false, clusterResourceExEnumItem.getName);
			}
			else
			{
				clusterResourceExEnumItem.rwCommonProperties = new Dictionary<string, Property>();
			}
			_CLUSTER_RESOURCE_ENUM_ITEM* ptr10 = (_CLUSTER_RESOURCE_ENUM_ITEM*)((ulong)(nint)ptr + 88uL);
			if (*(long*)ptr10 != 0L)
			{
				clusterResourceExEnumItem.roCommonProperties = PropertyCollection.ConvertPropertyListToDictionary((CLUSPROP_LIST*)(*(ulong*)ptr10), *(uint*)((ulong)(nint)ptr + 80uL), isReadOnly: true, clusterResourceExEnumItem.getName);
			}
			else
			{
				clusterResourceExEnumItem.roCommonProperties = new Dictionary<string, Property>();
			}
		}
		finally
		{
			global::_003CModule_003E.free(ptr);
		}
		return clusterResourceExEnumItem;
	}

	protected unsafe override uint FetchItem(uint index, _CLUSTER_RESOURCE_ENUM_ITEM* enumItem)
	{
		uint num = (uint)((uint)(*(int*)((ulong)(nint)enumItem + 4uL)) + ((long)(uint)(*(int*)((ulong)(nint)enumItem + 80uL)) + (long)(uint)(*(int*)((ulong)(nint)enumItem + 64uL)) + (uint)(*(int*)((ulong)(nint)enumItem + 48uL)) + (uint)(*(int*)((ulong)(nint)enumItem + 32uL)) + (uint)(*(int*)((ulong)(nint)enumItem + 16uL))) + 96);
		return global::_003CModule_003E.ClusterResourceEnumEx(DangerousGetEnumHandle(), index, enumItem, &num);
	}
}
