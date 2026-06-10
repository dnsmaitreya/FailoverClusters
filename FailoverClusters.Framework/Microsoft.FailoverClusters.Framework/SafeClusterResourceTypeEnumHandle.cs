using System;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

internal class SafeClusterResourceTypeEnumHandle : SafeHandleZeroIsInvalid
{
	public SafeClusterResourceTypeEnumHandle()
		: base(ownsHandle: true)
	{
	}

	public SafeClusterResourceTypeEnumHandle(IntPtr resourceTypeOpenEnumHandle)
		: base(ownsHandle: true)
	{
		SetHandle(resourceTypeOpenEnumHandle);
	}

	protected override bool ReleaseHandle()
	{
		return NativeMethods.ErrorCode.None.IsEqual(NativeMethods.ClusterResourceTypeCloseEnum(handle));
	}
}

