using System;

namespace KDDSL.ServerClusters;

internal abstract class SafeEnumHandleBase : SafeEnumHandleBaseTemplate_003CMS_003A_003AInternal_003A_003AServerClusters_003A_003AClusterEnumItem_0020_005E_002C_CLUSTER_ENUM_ITEM_0020_002A_003E
{
	public virtual string CurrentName
	{
		get
		{
			ClusterEnumItem clusterEnumItem = currentItem;
			if (clusterEnumItem == null)
			{
				throw new InvalidOperationException();
			}
			return clusterEnumItem.Name;
		}
	}

	protected unsafe override ClusterEnumItem FetchItem(uint index)
	{
		//IL_0017: Expected I, but got I8
		//IL_0026: Expected I, but got I8
		//IL_00ac: Expected I, but got I8
		//IL_00db: Expected I, but got I8
		//IL_00f4: Expected I, but got I8
		ClusterEnumItem clusterEnumItem = null;
		_CLUSTER_ENUM_ITEM* ptr = (_CLUSTER_ENUM_ITEM*)global::_003CModule_003E.malloc(296uL);
		*(int*)ptr = 1;
		_CLUSTER_ENUM_ITEM* ptr2 = (_CLUSTER_ENUM_ITEM*)((ulong)(nint)ptr + 8uL);
		*(int*)ptr2 = 128;
		_CLUSTER_ENUM_ITEM* ptr3 = (_CLUSTER_ENUM_ITEM*)((ulong)(nint)ptr + 24uL);
		*(int*)ptr3 = 128;
		uint num = FetchItem(index, ptr);
		try
		{
			if (num == 234)
			{
				_CLUSTER_ENUM_ITEM* ptr4 = (_CLUSTER_ENUM_ITEM*)global::_003CModule_003E.malloc((ulong)((long)(uint)(*(int*)ptr2) + (long)(uint)(*(int*)ptr3) + 40));
				*(int*)ptr4 = 1;
				*(int*)((ulong)(nint)ptr4 + 8uL) = *(int*)ptr2;
				*(int*)((ulong)(nint)ptr4 + 24uL) = *(int*)ptr3;
				global::_003CModule_003E.free(ptr);
				ptr = ptr4;
				num = FetchItem(index, ptr4);
			}
			if (num != 0)
			{
				ClusApiExceptionFactory.CreateAndThrow(base.Cluster, (int)num, Resources.EnumFail_Text);
			}
			if (*(int*)ptr == 0)
			{
				return null;
			}
			clusterEnumItem = new ClusterEnumItem();
			if (*(int*)((ulong)(nint)ptr + 8uL) != 0)
			{
				string g = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)ptr + 16uL))).PadLeft(32, '0');
				Guid iD = new Guid(g);
				clusterEnumItem.ID = iD;
			}
			clusterEnumItem.Index = (int)index;
			clusterEnumItem.Name = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)ptr + 32uL)));
			return clusterEnumItem;
		}
		finally
		{
			if (*(int*)((ulong)(nint)ptr + 8uL) == 0)
			{
				global::_003CModule_003E.free((void*)(*(ulong*)((ulong)(nint)ptr + 32uL)));
			}
			global::_003CModule_003E.free(ptr);
		}
	}

	protected SafeEnumHandleBase(Cluster cluster)
		: base(cluster)
	{
	}
}
