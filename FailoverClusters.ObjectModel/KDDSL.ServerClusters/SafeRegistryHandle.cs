using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class SafeRegistryHandle : SafeHandleBase
{
	public unsafe SafeRegistryHandle(HKEY__* hNativeHandle)
		: base((IntPtr)hNativeHandle)
	{
	}

	public unsafe HKEY__* DangerousGetRegistryHandle()
	{
		return (HKEY__*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing registry key handle.");
		IntPtr invalidClusterHandle = SafeHandleBase.InvalidClusterHandle;
		IntPtr intPtr = Interlocked.Exchange(ref handle, invalidClusterHandle);
		IntPtr invalidClusterHandle2 = SafeHandleBase.InvalidClusterHandle;
		if (intPtr != invalidClusterHandle2)
		{
			return (global::_003CModule_003E.ClusterRegCloseKey((HKEY__*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
