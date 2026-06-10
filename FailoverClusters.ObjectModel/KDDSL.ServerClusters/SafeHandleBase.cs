using System;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

internal abstract class SafeHandleBase : SafeHandle
{
	public override bool IsInvalid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			IntPtr invalidClusterHandle = InvalidClusterHandle;
			return handle == invalidClusterHandle;
		}
	}

	public static IntPtr InvalidClusterHandle => IntPtr.Zero;

	protected SafeHandleBase(IntPtr hNativeHandle)
		: base(InvalidClusterHandle, ownsHandle: true)
	{
		try
		{
			SetHandle(hNativeHandle);
			return;
		}
		catch
		{
			//try-fault
			base.Dispose(disposing: true);
			throw;
		}
	}

	protected SafeHandleBase()
		: base(InvalidClusterHandle, ownsHandle: true)
	{
	}
}
