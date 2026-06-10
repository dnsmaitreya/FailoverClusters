using System;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class EventLogChannelInfo : IDisposable
{
	private EventSafeHandle m_handle;

	public EventLogChannelInfo(EventLogSession session, string channel)
	{
		m_handle = EventOpenChannelConfig(session.Handle, channel, 0u);
	}

	public unsafe EventLogChannelType GetChannelType()
	{
		//Discarded unreachable code: IL_0025
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			EventGetChannelConfigProperty(m_handle, (_EVT_CHANNEL_CONFIG_PROPERTY_ID)2, 0u, unmanagedBuffer);
			return *(EventLogChannelType*)unmanagedBuffer.Pointer;
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GetIsClassicChannel()
	{
		//Discarded unreachable code: IL_002e
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			EventGetChannelConfigProperty(m_handle, (_EVT_CHANNEL_CONFIG_PROPERTY_ID)4, 0u, unmanagedBuffer);
			return (*(int*)unmanagedBuffer.Pointer != 0) ? true : false;
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	public unsafe string GetChannelPublisher()
	{
		//Discarded unreachable code: IL_0047
		//IL_002d: Expected I, but got I8
		//IL_003d: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			EventGetChannelConfigProperty(m_handle, (_EVT_CHANNEL_CONFIG_PROPERTY_ID)3, 0u, unmanagedBuffer);
			_EVT_VARIANT* pointer = (_EVT_VARIANT*)unmanagedBuffer.Pointer;
			if (*(int*)((ulong)(nint)pointer + 12uL) == 2)
			{
				IntPtr ptr = new IntPtr((void*)(*(ulong*)pointer));
				return Marshal.PtrToStringAnsi(ptr);
			}
			return InteropHelp.WstrToString((ushort*)(*(ulong*)pointer));
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private void _007EEventLogChannelInfo()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_handle?.Close();
	}

	private unsafe static EventSafeHandle EventOpenChannelConfig(EventSafeHandle session, string channelPath, uint flags)
	{
		//Discarded unreachable code: IL_004e
		//IL_0008: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(channelPath);
			void* ptr2 = global::_003CModule_003E.EvtOpenChannelConfig(session.Handle, ptr, flags);
			if (ptr2 == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventChannelConfigFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
			}
			return new EventSafeHandle(ptr2);
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	private unsafe static void EventGetChannelConfigProperty(EventSafeHandle channelConfig, _EVT_CHANNEL_CONFIG_PROPERTY_ID propertyId, uint flags, UnmanagedBuffer buffer)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		buffer.Allocate(16uL);
		uint num = 0u;
		if (global::_003CModule_003E.EvtGetChannelConfigProperty(channelConfig.Handle, propertyId, flags, (uint)buffer.Size, (_EVT_VARIANT*)buffer.Pointer, &num) != 0)
		{
			return;
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		if (lastError == 122)
		{
			buffer.Allocate(num);
			if (global::_003CModule_003E.EvtGetChannelConfigProperty(channelConfig.Handle, propertyId, flags, (uint)buffer.Size, (_EVT_VARIANT*)buffer.Pointer, &num) != 0)
			{
				return;
			}
			lastError = global::_003CModule_003E.GetLastError();
		}
		if (lastError != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)lastError, new string[1] { Resources.EventChannelConfigFailed_Text });
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EEventLogChannelInfo();
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
