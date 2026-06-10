using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class SafeChangeHandle : SafeHandleBase
{
	public unsafe SafeChangeHandle(_HCHANGE* hNativeHandle)
		: base((IntPtr)hNativeHandle)
	{
	}

	public unsafe _HCHANGE* DangerousGetChangeHandle()
	{
		return (_HCHANGE*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing cluster notify port handle.");
		IntPtr invalidClusterHandle = SafeHandleBase.InvalidClusterHandle;
		IntPtr intPtr = Interlocked.Exchange(ref handle, invalidClusterHandle);
		IntPtr invalidClusterHandle2 = SafeHandleBase.InvalidClusterHandle;
		if (intPtr != invalidClusterHandle2)
		{
			return (global::_003CModule_003E.CloseClusterNotifyPort((_HCHANGE*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
