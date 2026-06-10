using System;

namespace MS.Internal.ServerClusters;

internal class SafeResourceTypeEnumHandle : SafeEnumHandleBase
{
	public enum SafeResourceTypeEnumHandleType
	{
		HRESTYPEENUM,
		HCLUSENUM
	}

	internal ResourceTypeEnumType enumType;

	internal ClusterResourceType resourceType;

	internal ResourceTypeEnumOptions options;

	internal SafeResourceTypeEnumHandleType handleType;

	public unsafe SafeResourceTypeEnumHandle(void* enumHandle, ClusterResourceType resourceType, ResourceTypeEnumType enumType, ResourceTypeEnumOptions options, SafeResourceTypeEnumHandleType handleType)
		: base(resourceType.Cluster)
	{
		try
		{
			this.resourceType = resourceType;
			this.enumType = enumType;
			this.options = options;
			this.handleType = handleType;
			if (enumHandle == null)
			{
				uint lastError = global::_003CModule_003E.GetLastError();
				ClusApiExceptionFactory.CreateAndThrow(base.Cluster, (int)lastError, Resources.ClusterNodeOpenEnum_Fail_Text);
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
		return (uint)NativeMethods.ClusterResourceTypeCloseEnum(this);
	}

	protected override uint FetchItemCount()
	{
		return (uint)NativeMethods.ClusterResourceTypeGetEnumCount(this);
	}

	protected unsafe override uint FetchItem(uint index, _CLUSTER_ENUM_ITEM* enumItem)
	{
		int bytesNeeded = (int)((long)(uint)(*(int*)((ulong)(nint)enumItem + 8uL)) + (long)(uint)(*(int*)((ulong)(nint)enumItem + 24uL)) + 40);
		return (uint)NativeMethods.ClusterResourceTypeEnum(this, (int)index, enumItem, ref bytesNeeded);
	}
}
