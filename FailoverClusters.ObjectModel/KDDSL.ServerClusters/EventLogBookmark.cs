using System;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class EventLogBookmark : IDisposable
{
	private EventSafeHandle m_handle;

	private EventLogQuery m_query;

	private string m_channel;

	public EventLogEvent GetEvent()
	{
		if (m_handle.IsClosed)
		{
			return null;
		}
		EventSafeHandle channelHandle = m_query.GetChannelHandle(m_channel);
		EventSeek(channelHandle, 0L, m_handle, 0u, 4u);
		return new EventLogEvent(EventNext(channelHandle), m_query.Session);
	}

	private void _007EEventLogBookmark()
	{
		m_handle?.Close();
	}

	internal EventLogBookmark(EventSafeHandle eventHandle, EventLogQuery query)
	{
		EventSafeHandle eventSafeHandle = EventCreateBookmark();
		EventUpdateBookmark(eventSafeHandle, eventHandle);
		m_handle = eventSafeHandle;
		m_query = query;
		m_channel = EventLogEvent.GetChannel(eventHandle);
	}

	private unsafe static void EventSeek(EventSafeHandle resultSet, long pos, EventSafeHandle bookmark, uint timeOut, uint flags)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (global::_003CModule_003E.EvtSeek(resultSet.Handle, pos, bookmark.Handle, timeOut, flags) == 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventSeekFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
	}

	private unsafe static EventSafeHandle EventNext(EventSafeHandle resultSet)
	{
		//IL_0008: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		void* handle = null;
		uint num = 0u;
		if (global::_003CModule_003E.EvtNext(resultSet.Handle, 1u, &handle, uint.MaxValue, 0u, &num) == 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventNextFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
		return new EventSafeHandle(handle);
	}

	private unsafe static EventSafeHandle EventCreateBookmark()
	{
		//IL_000c: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		void* ptr = global::_003CModule_003E.EvtCreateBookmark(null);
		if (ptr == null)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventBookmarkFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
		return new EventSafeHandle(ptr);
	}

	private unsafe static void EventUpdateBookmark(EventSafeHandle bookMark, EventSafeHandle eventHandle)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (global::_003CModule_003E.EvtUpdateBookmark(bookMark.Handle, eventHandle.Handle) == 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventBookmarkFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EEventLogBookmark();
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
