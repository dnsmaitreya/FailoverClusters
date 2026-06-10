using System;
using System.Runtime.CompilerServices;

namespace KDDSL.ServerClusters;

internal class SafeResourceEnumHandle : SafeEnumHandleBase
{
	public unsafe SafeResourceEnumHandle(Cluster cluster, SafeResourceHandle resource, ResourceEnumType enumType)
		: base(cluster)
	{
		try
		{
			_HRESENUM* ptr = global::_003CModule_003E.ClusterResourceOpenEnum(resource.DangerousGetResourceHandle(), (uint)enumType);
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

	protected unsafe _HRESENUM* DangerousGetEnumHandle()
	{
		return (_HRESENUM*)handle.ToPointer();
	}

	protected unsafe override uint InternalReleaseHandle()
	{
		return global::_003CModule_003E.ClusterResourceCloseEnum(DangerousGetEnumHandle());
	}

	protected unsafe override uint FetchItemCount()
	{
		return global::_003CModule_003E.ClusterResourceGetEnumCount(DangerousGetEnumHandle());
	}

	protected unsafe override uint FetchItem(uint index, _CLUSTER_ENUM_ITEM* enumItem)
	{
		//IL_005a: Expected I8, but got I
		uint num = (uint)(*(int*)((ulong)(nint)enumItem + 24uL)) >> 1;
		ushort* ptr = (ushort*)global::_003CModule_003E.malloc((ulong)num * 2uL);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
		uint num3 = global::_003CModule_003E.ClusterResourceEnum(DangerousGetEnumHandle(), index, &num2, ptr, &num);
		*(int*)((ulong)(nint)enumItem + 8uL) = 0;
		*(int*)((ulong)(nint)enumItem + 24uL) = (int)((long)(num + 1) * 2L);
		if (num3 != 0)
		{
			global::_003CModule_003E.free(ptr);
			return num3;
		}
		*(uint*)((ulong)(nint)enumItem + 4uL) = num2;
		*(long*)((ulong)(nint)enumItem + 16uL) = 0L;
		*(long*)((ulong)(nint)enumItem + 32uL) = (nint)ptr;
		return 0u;
	}
}
