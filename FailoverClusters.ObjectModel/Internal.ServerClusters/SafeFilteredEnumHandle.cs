using System;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

internal abstract class SafeFilteredEnumHandle : SafeEnumHandleBase
{
	private string coreClusterGroupGuidString;

	private string coreAvailableStorageGuidString;

	private string coreClusterGroupName;

	private string coreAvailableStorageName;

	private bool filterCoreGroups;

	private bool filterCoreResources;

	protected SafeFilteredEnumHandle(Cluster cluster, [MarshalAs(UnmanagedType.U1)] bool filterCoreGroups, [MarshalAs(UnmanagedType.U1)] bool filterCoreResources)
		: base(cluster)
	{
		try
		{
			this.filterCoreGroups = filterCoreGroups;
			this.filterCoreResources = filterCoreResources;
			coreClusterGroupGuidString = cluster.GetCoreClusterGroup().Id.ToString();
			coreAvailableStorageGuidString = cluster.GetAvailableStorageGroup().Id.ToString();
			coreClusterGroupName = cluster.GetCoreClusterGroup().Name;
			coreAvailableStorageName = cluster.GetAvailableStorageGroup().Name;
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	protected override void Construct()
	{
		base.Construct();
		base.NumElements = CoreItemCount() + base.NumElements;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe bool IsCoreGroupById(ushort* id)
	{
		string strA = InteropHelp.WstrToString(id);
		int num = ((string.Compare(strA, coreClusterGroupGuidString, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, coreAvailableStorageGuidString, StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0);
		return (byte)num != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe bool IsCoreGroupByName(ushort* name)
	{
		string strA = InteropHelp.WstrToString(name);
		int num = ((string.Compare(strA, coreClusterGroupName, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, coreAvailableStorageName, StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0);
		return (byte)num != 0;
	}

	protected unsafe void SkipItemIfCoreGroup(_CLUSTER_ENUM_ITEM* enumItem)
	{
		//IL_0018: Expected I, but got I8
		//IL_002b: Expected I, but got I8
		if (*(int*)enumItem == 0)
		{
			return;
		}
		if (*(int*)((ulong)(nint)enumItem + 8uL) != 0)
		{
			if (IsCoreGroupById((ushort*)(*(ulong*)((ulong)(nint)enumItem + 16uL))))
			{
				*(int*)enumItem = 0;
			}
		}
		else if (IsCoreGroupByName((ushort*)(*(ulong*)((ulong)(nint)enumItem + 32uL))))
		{
			*(int*)enumItem = 0;
		}
	}

	protected uint AdjustItemCount(uint count)
	{
		if (filterCoreGroups)
		{
			count = ((count >= 2) ? (count + 4294967294u) : 0u);
		}
		return count;
	}

	protected unsafe void ProcessItem(NodeEnumType enumType, _CLUSTER_ENUM_ITEM* enumItem)
	{
		if ((enumType & NodeEnumType.Groups) != 0 && filterCoreGroups)
		{
			SkipItemIfCoreGroup(enumItem);
		}
	}

	protected unsafe void ProcessItem(ClusterEnumType enumType, _CLUSTER_ENUM_ITEM* enumItem)
	{
		if ((enumType & ClusterEnumType.Group) != 0 && filterCoreGroups)
		{
			SkipItemIfCoreGroup(enumItem);
		}
	}

	private uint CoreItemCount()
	{
		return filterCoreGroups ? 2u : 0u;
	}
}
