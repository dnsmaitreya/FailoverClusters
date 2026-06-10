using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class SafeNetworkHandle : SafeHandleBase
{
	public unsafe SafeNetworkHandle(_HNETWORK* hNativeHandle)
		: base((IntPtr)hNativeHandle)
	{
	}

	public unsafe _HNETWORK* DangerousGetNetworkHandle()
	{
		return (_HNETWORK*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing network handle.");
		IntPtr invalidClusterHandle = SafeHandleBase.InvalidClusterHandle;
		IntPtr intPtr = Interlocked.Exchange(ref handle, invalidClusterHandle);
		IntPtr invalidClusterHandle2 = SafeHandleBase.InvalidClusterHandle;
		if (intPtr != invalidClusterHandle2)
		{
			return (global::_003CModule_003E.CloseClusterNetwork((_HNETWORK*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
