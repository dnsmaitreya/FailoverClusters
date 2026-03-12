using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class SafeNetworkInterfaceHandle : SafeHandleBase
{
	public unsafe SafeNetworkInterfaceHandle(_HNETINTERFACE* hNativeHandle)
		: base((IntPtr)hNativeHandle)
	{
	}

	public unsafe _HNETINTERFACE* DangerousGetNetworkInterfaceHandle()
	{
		return (_HNETINTERFACE*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing netinterface handle.");
		IntPtr invalidClusterHandle = SafeHandleBase.InvalidClusterHandle;
		IntPtr intPtr = Interlocked.Exchange(ref handle, invalidClusterHandle);
		IntPtr invalidClusterHandle2 = SafeHandleBase.InvalidClusterHandle;
		if (intPtr != invalidClusterHandle2)
		{
			return (global::_003CModule_003E.CloseClusterNetInterface((_HNETINTERFACE*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
