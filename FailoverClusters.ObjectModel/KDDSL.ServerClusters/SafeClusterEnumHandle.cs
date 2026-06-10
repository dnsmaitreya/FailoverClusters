using System;

namespace KDDSL.ServerClusters;

internal class SafeClusterEnumHandle : SafeFilteredEnumHandle
{
	public enum SafeClusEnumHandleType
	{
		HCLUSENUM,
		HCLUSENUMEX
	}

	internal ClusterEnumType enumType;

	internal new Cluster cluster;

	internal SafeClusterEnumHandleOptions options;

	internal SafeClusEnumHandleType handleType;

	public unsafe SafeClusterEnumHandle(void* enumHandle, Cluster cluster, ClusterEnumType enumType, SafeClusterEnumHandleOptions options, SafeClusEnumHandleType safeClusEnumHandleType)
		: base(cluster, (byte)((uint)(byte)options & (true ? 1u : 0u)) != 0, filterCoreResources: false)
	{
		try
		{
			this.cluster = cluster;
			this.enumType = enumType;
			this.options = options;
			handleType = safeClusEnumHandleType;
			if (enumHandle == null)
			{
				uint lastError = global::_003CModule_003E.GetLastError();
				ClusApiExceptionFactory.CreateAndThrow(base.Cluster, (int)lastError, Resources.ClusterOpenEnum_Fail_Text);
			}
			IntPtr intPtr = (IntPtr)enumHandle;
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

	public unsafe void* DangerousGetEnumHandle()
	{
		return handle.ToPointer();
	}

	protected override uint InternalReleaseHandle()
	{
		return (uint)NativeMethods.ClusterCloseEnum(this);
	}

	protected override uint FetchItemCount()
	{
		uint count = (uint)NativeMethods.ClusterGetEnumCount(this);
		return AdjustItemCount(count);
	}

	protected unsafe override uint FetchItem(uint index, _CLUSTER_ENUM_ITEM* enumItem)
	{
		int bytesNeeded = (int)((long)(uint)(*(int*)((ulong)(nint)enumItem + 8uL)) + (long)(uint)(*(int*)((ulong)(nint)enumItem + 24uL)) + 40);
		uint num = (uint)NativeMethods.ClusterEnum(this, (int)index, enumItem, ref bytesNeeded);
		if (num == 0)
		{
			ProcessItem(enumType, enumItem);
		}
		return num;
	}
}
