using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class SafeResourceHandle : SafeHandleBase
{
	public unsafe SafeResourceHandle(_HRESOURCE* hNativeHandle)
		: base((IntPtr)hNativeHandle)
	{
	}

	public unsafe _HRESOURCE* DangerousGetResourceHandle()
	{
		return (_HRESOURCE*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing resource handle.");
		IntPtr invalidClusterHandle = SafeHandleBase.InvalidClusterHandle;
		IntPtr intPtr = Interlocked.Exchange(ref handle, invalidClusterHandle);
		IntPtr invalidClusterHandle2 = SafeHandleBase.InvalidClusterHandle;
		if (intPtr != invalidClusterHandle2)
		{
			return (global::_003CModule_003E.CloseClusterResource((_HRESOURCE*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
