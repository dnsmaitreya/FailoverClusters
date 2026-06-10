using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class SafeSecurityDescriptor : SafeHandle
{
	public override bool IsInvalid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return handle == IntPtr.Zero;
		}
	}

	public unsafe SafeSecurityDescriptor(void* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe void* DangerousPointer()
	{
		return handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		IntPtr intPtr = Interlocked.Exchange(ref handle, IntPtr.Zero);
		if (intPtr != IntPtr.Zero)
		{
			global::_003CModule_003E.LocalFree(intPtr.ToPointer());
		}
		return true;
	}
}
