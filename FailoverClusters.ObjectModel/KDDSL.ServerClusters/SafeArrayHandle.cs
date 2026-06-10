using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace KDDSL.ServerClusters;

internal class SafeArrayHandle : SafeHandle
{
	public override bool IsInvalid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return handle == IntPtr.Zero;
		}
	}

	public unsafe SafeArrayHandle(void* pMem, [MarshalAs(UnmanagedType.U1)] bool owns)
		: base(new IntPtr(pMem), owns)
	{
	}

	public unsafe SafeArrayHandle(void* pMem)
		: base(new IntPtr(pMem), ownsHandle: true)
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		IntPtr intPtr = Interlocked.Exchange(ref handle, IntPtr.Zero);
		if (intPtr != IntPtr.Zero)
		{
			InteropHelp.FreeArray(intPtr.ToPointer());
		}
		return true;
	}
}
