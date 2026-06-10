using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class SafeServiceHandle : SafeHandle
{
	public override bool IsInvalid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return handle == IntPtr.Zero;
		}
	}

	public unsafe SafeServiceHandle(SC_HANDLE__* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe SC_HANDLE__* GetHandle()
	{
		return (SC_HANDLE__*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing service handle.");
		IntPtr intPtr = Interlocked.Exchange(ref handle, IntPtr.Zero);
		if (intPtr != IntPtr.Zero)
		{
			return (global::_003CModule_003E.CloseServiceHandle((SC_HANDLE__*)intPtr.ToPointer()) != 0) ? true : false;
		}
		return true;
	}
}
