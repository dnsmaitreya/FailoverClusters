using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

internal class UnmanagedBuffer : IDisposable
{
	private ulong m_size;

	private SafeArrayHandle m_safeHandle;

	public unsafe void* Pointer => m_safeHandle.DangerousGetHandle().ToPointer();

	public ulong Size => m_size;

	public unsafe UnmanagedBuffer(void* pMem, ulong size)
	{
		m_size = size;
		base._002Ector();
		m_safeHandle = new SafeArrayHandle(pMem, owns: false);
	}

	public UnmanagedBuffer()
	{
		m_size = 0uL;
		base._002Ector();
		m_safeHandle = null;
	}

	private void _007EUnmanagedBuffer()
	{
		Free();
	}

	public unsafe virtual void Allocate(ulong cbSize)
	{
		Free();
		void* pMem = InteropHelp.AllocateArray(cbSize);
		m_safeHandle = new SafeArrayHandle(pMem);
		m_size = cbSize;
	}

	public virtual void Free()
	{
		SafeArrayHandle safeHandle = m_safeHandle;
		if (safeHandle != null)
		{
			((IDisposable)safeHandle).Dispose();
			m_safeHandle = null;
		}
		m_size = 0uL;
	}

	public unsafe static UnmanagedBuffer Create(string managedString)
	{
		//IL_001b: Expected I, but got I8
		//IL_003d: Expected I4, but got I8
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		ushort* ptr = InteropHelp.StringToWstr(managedString);
		try
		{
			ushort* ptr2 = ptr;
			while (System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ptr2) != 0)
			{
				ptr2 = (ushort*)((ulong)(nint)ptr2 + 2uL);
			}
			ulong num = (ulong)(((long)((nint)((byte*)ptr2 - (nuint)ptr) >> 1) + 1L) * 2);
			unmanagedBuffer.Allocate(num);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(unmanagedBuffer.Pointer, ptr, num);
			return unmanagedBuffer;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EUnmanagedBuffer();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
