using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class SafeGroupHandle : SafeHandleBase
{
	public unsafe SafeGroupHandle(_HGROUP* hNativeHandle)
		: base((IntPtr)hNativeHandle)
	{
	}

	public unsafe _HGROUP* DangerousGetGroupHandle()
	{
		return (_HGROUP*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing group handle.");
		IntPtr invalidClusterHandle = SafeHandleBase.InvalidClusterHandle;
		IntPtr intPtr = Interlocked.Exchange(ref handle, invalidClusterHandle);
		IntPtr invalidClusterHandle2 = SafeHandleBase.InvalidClusterHandle;
		if (intPtr != invalidClusterHandle2)
		{
			return (global::_003CModule_003E.CloseClusterGroup((_HGROUP*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
