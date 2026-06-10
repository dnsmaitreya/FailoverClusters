using System;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

internal class EventSafeHandle : SafeHandle
{
	private static IntPtr InvalidHandle => IntPtr.Zero;

	public override bool IsInvalid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			IntPtr invalidHandle = InvalidHandle;
			return handle == invalidHandle;
		}
	}

	public unsafe void* Handle => (void*)DangerousGetHandle();

	public unsafe EventSafeHandle(void* handle, [MarshalAs(UnmanagedType.U1)] bool ownsHandle)
		: base(new IntPtr(handle), ownsHandle)
	{
	}

	public unsafe EventSafeHandle(void* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public EventSafeHandle()
		: base(InvalidHandle, ownsHandle: true)
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe override bool ReleaseHandle()
	{
		return global::_003CModule_003E.EvtClose(Handle) == 1;
	}
}
