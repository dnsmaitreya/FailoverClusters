using System;

namespace MS.Internal.ServerClusters;

internal class SafeNodeEnumHandle : SafeFilteredEnumHandle
{
	public enum SafeNodeEnumHandleType
	{
		HNODEENUM,
		HNODEENUMEX,
		HCLUSENUM
	}

	internal NodeEnumType enumType;

	internal ClusterNode node;

	internal SafeNodeEnumHandleOptions options;

	internal SafeNodeEnumHandleType handleType;

	public unsafe SafeNodeEnumHandle(void* handle, ClusterNode node, NodeEnumType enumType, SafeNodeEnumHandleOptions options, SafeNodeEnumHandleType safeNodeEnumHandleType)
		: base(node.Cluster, (byte)((uint)(byte)options & (true ? 1u : 0u)) != 0, filterCoreResources: false)
	{
		try
		{
			this.node = node;
			this.enumType = enumType;
			this.options = options;
			handleType = safeNodeEnumHandleType;
			if (handle == null)
			{
				uint lastError = global::_003CModule_003E.GetLastError();
				ClusApiExceptionFactory.CreateAndThrow(base.Cluster, (int)lastError, Resources.ClusterNodeOpenEnum_Fail_Text);
			}
			IntPtr intPtr = (IntPtr)handle;
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
		return (uint)NativeMethods.ClusterNodeCloseEnum(this);
	}

	protected override uint FetchItemCount()
	{
		uint count = (uint)NativeMethods.ClusterNodeGetEnumCount(this);
		return AdjustItemCount(count);
	}

	protected unsafe override uint FetchItem(uint index, _CLUSTER_ENUM_ITEM* enumItem)
	{
		int bytesNeeded = (int)((long)(uint)(*(int*)((ulong)(nint)enumItem + 8uL)) + (long)(uint)(*(int*)((ulong)(nint)enumItem + 24uL)) + 40);
		uint num = (uint)NativeMethods.ClusterNodeEnum(this, (int)index, enumItem, ref bytesNeeded);
		if (num == 0)
		{
			ProcessItem(enumType, enumItem);
		}
		return num;
	}
}
