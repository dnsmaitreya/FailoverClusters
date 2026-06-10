using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class EventLogQuery : IDisposable
{
	private EventLogSession m_session;

	private Dictionary<string, EventSafeHandle> m_channelHandles;

	private EventSafeHandle m_handle;

	public EventLogSession Session => m_session;

	public unsafe ICollection<EventLogBookmark> GetResults(uint resultsCount)
	{
		//Discarded unreachable code: IL_007e
		//IL_0062: Expected I, but got I8
		uint num = 0u;
		if (resultsCount >= 536870911)
		{
			throw new ArgumentOutOfRangeException("resultsCount");
		}
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			unmanagedBuffer.Allocate((ulong)resultsCount * 8uL);
			if (!EventNext(m_handle, resultsCount, (void**)unmanagedBuffer.Pointer, &num))
			{
				return null;
			}
			List<EventLogBookmark> list = new List<EventLogBookmark>((int)resultsCount);
			for (uint num2 = 0u; num2 < num; num2++)
			{
				list.Add(new EventLogBookmark(new EventSafeHandle((void*)(*(ulong*)((nint)unmanagedBuffer.Pointer + (long)num2 * 8L))), this));
			}
			return list;
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private void _007EEventLogQuery()
	{
		Dictionary<string, EventSafeHandle>.ValueCollection.Enumerator enumerator = m_channelHandles.Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				enumerator.Current.Close();
			}
			while (enumerator.MoveNext());
		}
	}

	internal EventLogQuery(EventLogSession session, string channel, string text)
	{
		m_session = session;
		session.ResetInvalidPublishers();
		m_channelHandles = new Dictionary<string, EventSafeHandle>(StringComparer.OrdinalIgnoreCase);
		m_handle = EventQuery(session.Handle, channel, text, 513u);
	}

	internal EventSafeHandle GetChannelHandle(string channel)
	{
		EventSafeHandle value = null;
		if (!m_channelHandles.TryGetValue(channel, out value))
		{
			value = EventQuery(m_session.Handle, channel, "*", 1u);
			m_channelHandles[channel] = value;
		}
		return value;
	}

	private unsafe static EventSafeHandle EventQuery(EventSafeHandle session, string path, string query, uint flags)
	{
		//Discarded unreachable code: IL_0063
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(path);
			ptr2 = InteropHelp.StringToWstr(query);
			void* ptr3 = global::_003CModule_003E.EvtQuery(session.Handle, ptr, ptr2, flags);
			if (ptr3 == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventQueryFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
			}
			return new EventSafeHandle(ptr3);
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
			if (ptr2 != null)
			{
				InteropHelp.FreeWstr(ptr2);
			}
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe static bool EventNext(EventSafeHandle resultSet, uint eventsCount, void** events, uint* returned)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		bool flag = global::_003CModule_003E.EvtNext(resultSet.Handle, eventsCount, events, uint.MaxValue, 0u, returned) == 1;
		if (!flag)
		{
			uint lastError = global::_003CModule_003E.GetLastError();
			if (lastError != 259)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)lastError, new string[1] { Resources.EventNextFailed_Text });
			}
		}
		return flag;
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EEventLogQuery();
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
