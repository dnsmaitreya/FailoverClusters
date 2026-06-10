using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MS.Internal.ServerClusters;

internal class SafeNetApiBuffer : SafeHandle
{
	public override bool IsInvalid
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return handle == IntPtr.Zero;
		}
	}

	public unsafe SafeNetApiBuffer(_LOCALGROUP_MEMBERS_INFO_1* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe SafeNetApiBuffer(_SERVER_TRANSPORT_INFO_0* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe SafeNetApiBuffer(_SHARE_INFO_502* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe SafeNetApiBuffer(_SHARE_INFO_1005* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe SafeNetApiBuffer(_USER_INFO_20* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe SafeNetApiBuffer(_DOMAIN_CONTROLLER_INFOW* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe SafeNetApiBuffer(_SERVER_INFO_101* handle)
		: base(new IntPtr(handle), ownsHandle: true)
	{
	}

	public unsafe _SERVER_INFO_101* DangerousNetServerInfoPointer()
	{
		return (_SERVER_INFO_101*)handle.ToPointer();
	}

	public unsafe _DOMAIN_CONTROLLER_INFOW* DangerousDcInfoPointer()
	{
		return (_DOMAIN_CONTROLLER_INFOW*)handle.ToPointer();
	}

	public unsafe _USER_INFO_20* DangerousUserInfoPointer()
	{
		return (_USER_INFO_20*)handle.ToPointer();
	}

	public unsafe _SHARE_INFO_1005* DangerousShareInfo1005Pointer()
	{
		return (_SHARE_INFO_1005*)handle.ToPointer();
	}

	public unsafe _SHARE_INFO_502* DangerousShareInfo502Pointer()
	{
		return (_SHARE_INFO_502*)handle.ToPointer();
	}

	public unsafe _LOCALGROUP_MEMBERS_INFO_1* DangerousLocalGroupMembersInfo1Pointer()
	{
		return (_LOCALGROUP_MEMBERS_INFO_1*)handle.ToPointer();
	}

	[return: MarshalAs(UnmanagedType.U1)]
	protected unsafe override bool ReleaseHandle()
	{
		DebugLog.LogVerbose("Closing netapi handle.");
		IntPtr intPtr = Interlocked.Exchange(ref handle, IntPtr.Zero);
		if (intPtr != IntPtr.Zero)
		{
			uint num = global::_003CModule_003E.NetApiBufferFree(intPtr.ToPointer());
			if (num != 0)
			{
				DebugLog.LogWarning("NetApiBufferFree returned {0}", num);
			}
		}
		return true;
	}
}
