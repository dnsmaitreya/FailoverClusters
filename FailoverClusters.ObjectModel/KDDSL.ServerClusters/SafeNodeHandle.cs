using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class SafeNodeHandle : SafeHandleBase
{
	public unsafe SafeNodeHandle(_HNODE* hNativeHandle)
		: base((IntPtr)hNativeHandle)
	{
	}

	public unsafe _HNODE* DangerousGetNodeHandle()
	{
		return (_HNODE*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing node handle.");
		IntPtr invalidClusterHandle = SafeHandleBase.InvalidClusterHandle;
		IntPtr intPtr = Interlocked.Exchange(ref handle, invalidClusterHandle);
		IntPtr invalidClusterHandle2 = SafeHandleBase.InvalidClusterHandle;
		if (intPtr != invalidClusterHandle2)
		{
			return (global::_003CModule_003E.CloseClusterNode((_HNODE*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
