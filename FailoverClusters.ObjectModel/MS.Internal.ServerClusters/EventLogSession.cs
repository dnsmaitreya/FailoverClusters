using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public class EventLogSession : IDisposable
{
	private class PublisherName
	{
		public string LongName;

		public string ShortName;
	}

	private unsafe static delegate* unmanaged[Cdecl, Cdecl]<void*, ushort*, uint, uint, int, ushort*, uint*, int> s_evtIntGetClassicLogDisplayName = null;

	private string m_serverName;

	private EventSafeHandle m_handle;

	private Dictionary<string, EventLogPublisher> m_publishers;

	private Dictionary<string, PublisherName> m_publisherNames;

	private List<string> m_invalidatedPublishers;

	private object m_invalidatedPublishersLock;

	internal EventSafeHandle Handle => m_handle;

	public string ServerName => m_serverName;

	public EventLogSession(string serverName)
	{
		m_serverName = serverName;
		m_handle = OpenSession(serverName, 0);
		m_publishers = new Dictionary<string, EventLogPublisher>(StringComparer.OrdinalIgnoreCase);
		m_publisherNames = GetPublisherNames();
		m_invalidatedPublishers = new List<string>();
		m_invalidatedPublishersLock = new object();
	}

	public void MarkInvalidPublisher(string publisherId)
	{
		Monitor.Enter(m_invalidatedPublishersLock);
		try
		{
			if (!m_invalidatedPublishers.Contains(publisherId))
			{
				m_invalidatedPublishers.Add(publisherId);
			}
		}
		finally
		{
			Monitor.Exit(m_invalidatedPublishersLock);
		}
	}

	public void ResetInvalidPublishers()
	{
		Monitor.Enter(m_invalidatedPublishersLock);
		try
		{
			List<string>.Enumerator enumerator = m_invalidatedPublishers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				m_publishers.Remove(current);
			}
			m_invalidatedPublishers.Clear();
		}
		finally
		{
			Monitor.Exit(m_invalidatedPublishersLock);
		}
	}

	public List<string> GetChannels()
	{
		//Discarded unreachable code: IL_003e
		EventSafeHandle eventSafeHandle = null;
		try
		{
			eventSafeHandle = EventOpenChannelEnum(m_handle, 0);
			List<string> list = new List<string>();
			string text = null;
			while (true)
			{
				text = EventNextChannelPath(eventSafeHandle);
				if (!(text != null))
				{
					break;
				}
				list.Add(text);
			}
			return list;
		}
		finally
		{
			eventSafeHandle?.Close();
		}
	}

	public EventLogPublisher GetPublisher(string publisherId)
	{
		EventLogPublisher eventLogPublisher = null;
		ThreadWatchdog.PerformUIThreadCheck();
		eventLogPublisher = null;
		if (!m_publishers.TryGetValue(publisherId, out eventLogPublisher))
		{
			eventLogPublisher = new EventLogPublisher(this, publisherId);
			m_publishers.Add(publisherId, eventLogPublisher);
		}
		return eventLogPublisher;
	}

	public string GetClassicLogDisplayName(string logName)
	{
		return EventIntGetClassicLogDisplayName(m_handle, logName);
	}

	public string GetPublisherShortName(string publisher)
	{
		PublisherName value = null;
		string key = publisher.ToLower(CultureInfo.InvariantCulture);
		if (!m_publisherNames.TryGetValue(key, out value))
		{
			return publisher;
		}
		return value.ShortName;
	}

	public EventLogQuery CreateQuery(string channel, string text)
	{
		return new EventLogQuery(this, channel, text);
	}

	public unsafe int SaveEventsToFile(string fileName, string query)
	{
		//Discarded unreachable code: IL_0068
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		ushort* ptr = null;
		ushort* ptr2 = null;
		int result = 0;
		try
		{
			ptr = InteropHelp.StringToWstr(fileName);
			ptr2 = InteropHelp.StringToWstr(query);
			void* handle = m_handle.Handle;
			if (global::_003CModule_003E.EvtExportLog(handle, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_13BBDEGPLJ_0040_003F_0024AA_003F_0024CK_0040), ptr2, ptr, 8193u) == 0)
			{
				result = Marshal.GetLastWin32Error();
			}
			if (global::_003CModule_003E.EvtArchiveExportedLog(handle, ptr, (uint)CultureInfo.CurrentCulture.LCID, 0u) == 0)
			{
				result = Marshal.GetLastWin32Error();
			}
			return result;
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			InteropHelp.FreeWstr(ptr2);
		}
	}

	private void _007EEventLogSession()
	{
		m_invalidatedPublishers.Clear();
		Dictionary<string, EventLogPublisher>.ValueCollection.Enumerator enumerator = m_publishers.Values.GetEnumerator();
		if (enumerator.MoveNext())
		{
			do
			{
				((IDisposable)enumerator.Current)?.Dispose();
			}
			while (enumerator.MoveNext());
		}
		m_publishers.Clear();
		m_handle?.Close();
	}

	private Dictionary<string, PublisherName> GetPublisherNames()
	{
		//Discarded unreachable code: IL_00d8
		PublisherName publisherName = null;
		Dictionary<string, PublisherName> dictionary = new Dictionary<string, PublisherName>(StringComparer.OrdinalIgnoreCase);
		Dictionary<string, PublisherName> dictionary2 = new Dictionary<string, PublisherName>(StringComparer.OrdinalIgnoreCase);
		try
		{
			EventSafeHandle publisherEnum = OpenPublisherEnum(m_handle, 0);
			string text = null;
			while (true)
			{
				text = EventNextPublisherId(publisherEnum);
				if (!(text != null))
				{
					break;
				}
				PublisherName publisherName2 = new PublisherName();
				publisherName2.ShortName = text;
				string publisherNameDictionaryKey = GetPublisherNameDictionaryKey(publisherName2.LongName = ConvertToPublisherShortName(text));
				if (!dictionary.ContainsKey(publisherNameDictionaryKey))
				{
					string publisherNameDictionaryKey2 = GetPublisherNameDictionaryKey(publisherName2.ShortName);
					publisherName = null;
					if (dictionary2.TryGetValue(publisherNameDictionaryKey2, out publisherName))
					{
						SetPublisherAlternateShortName(publisherName);
						SetPublisherAlternateShortName(publisherName2);
						publisherNameDictionaryKey2 = GetPublisherNameDictionaryKey(publisherName2.ShortName);
					}
					dictionary.Add(publisherNameDictionaryKey, publisherName2);
					dictionary2.Add(publisherNameDictionaryKey2, publisherName2);
				}
			}
		}
		catch (Exception caughtException)
		{
			if (!ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
			{
				throw;
			}
			ExceptionHelp.LogException(caughtException, "Error mapping Crimson publishers to their short names");
		}
		return dictionary;
	}

	private static string GetPublisherNameDictionaryKey(string name)
	{
		return name.ToLower(CultureInfo.InvariantCulture);
	}

	private static string ConvertToPublisherShortName(string publisher)
	{
		int startIndex = 0;
		int num = 0;
		do
		{
			startIndex = publisher.IndexOf('-', startIndex);
			if (startIndex != -1 && startIndex != publisher.Length - 1)
			{
				startIndex++;
				num++;
				continue;
			}
			return publisher;
		}
		while (num < 2);
		return publisher.Substring(startIndex);
	}

	private static void SetPublisherAlternateShortName(PublisherName publisher)
	{
		if (publisher.ShortName != publisher.LongName)
		{
			string longName = publisher.LongName;
			publisher.ShortName = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", ConvertToPublisherShortName(longName), longName);
		}
	}

	private unsafe static EventSafeHandle OpenSession(string server, int timeout)
	{
		//Discarded unreachable code: IL_00cb
		//IL_0008: Expected I, but got I8
		//IL_008a: Expected I4, but got I8
		//IL_008e: Expected I8, but got I
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(server);
			DebugLog.LogVerbose(string.Format(CultureInfo.InvariantCulture, "Server {0}, pinging server before connect to Crimson", server));
			if (!NetworkHelper.CanPing(server))
			{
				DebugLog.LogVerbose(string.Format(CultureInfo.InvariantCulture, "Server {0} ping failed... aborting Crimson connection", server));
				global::_003CModule_003E.SetLastError(1722u);
				throw ExceptionHelp.Build<ApplicationException>(-2147023174, new string[2]
				{
					Resources.EventOpenSessionFailed_Text,
					server
				});
			}
			DebugLog.LogVerbose(string.Format(CultureInfo.InvariantCulture, "Server {0} ping succeed... trying Crimson connection", server));
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _EVT_RPC_LOGIN eVT_RPC_LOGIN);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref eVT_RPC_LOGIN, 0, 40);
			*(long*)(&eVT_RPC_LOGIN) = (nint)ptr;
			void* ptr2 = global::_003CModule_003E.EvtOpenSession((_EVT_LOGIN_CLASS)1, &eVT_RPC_LOGIN, (uint)timeout, 0u);
			if (ptr2 == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventOpenSessionFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
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

	private unsafe static EventSafeHandle OpenPublisherEnum(EventSafeHandle session, int flags)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		void* ptr = global::_003CModule_003E.EvtOpenPublisherEnum(session.Handle, (uint)flags);
		if (ptr == null)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventPublisherOpenFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
		return new EventSafeHandle(ptr);
	}

	private unsafe static string EventNextPublisherId(EventSafeHandle publisherEnum)
	{
		//Discarded unreachable code: IL_00a7
		ThreadWatchdog.PerformUIThreadCheck();
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			uint num = 1024u;
			unmanagedBuffer.Allocate(1024uL);
			uint num2 = 0u;
			if (global::_003CModule_003E.EvtNextPublisherId(publisherEnum.Handle, num, (ushort*)unmanagedBuffer.Pointer, &num) == 0)
			{
				num2 = global::_003CModule_003E.GetLastError();
				if (num2 == 122)
				{
					unmanagedBuffer.Allocate(num);
					num2 = 0u;
					if (global::_003CModule_003E.EvtNextPublisherId(publisherEnum.Handle, num, (ushort*)unmanagedBuffer.Pointer, &num) != 0)
					{
						goto IL_0091;
					}
					num2 = global::_003CModule_003E.GetLastError();
				}
				switch (num2)
				{
				case 259u:
					return null;
				default:
					throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventChannelEnumerationFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
				case 0u:
					break;
				}
			}
			goto IL_0091;
			IL_0091:
			return InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private unsafe static EventSafeHandle EventOpenChannelEnum(EventSafeHandle session, int flags)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		void* ptr = global::_003CModule_003E.EvtOpenChannelEnum(session.Handle, (uint)flags);
		if (ptr == null)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventOpenChannelEnumFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
		return new EventSafeHandle(ptr);
	}

	private unsafe static string EventNextChannelPath(EventSafeHandle channelEnum)
	{
		//Discarded unreachable code: IL_00a7
		ThreadWatchdog.PerformUIThreadCheck();
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			uint num = 1024u;
			unmanagedBuffer.Allocate(1024uL);
			uint num2 = 0u;
			if (global::_003CModule_003E.EvtNextChannelPath(channelEnum.Handle, num, (ushort*)unmanagedBuffer.Pointer, &num) == 0)
			{
				num2 = global::_003CModule_003E.GetLastError();
				if (num2 == 122)
				{
					unmanagedBuffer.Allocate(num);
					num2 = 0u;
					if (global::_003CModule_003E.EvtNextChannelPath(channelEnum.Handle, num, (ushort*)unmanagedBuffer.Pointer, &num) != 0)
					{
						goto IL_0091;
					}
					num2 = global::_003CModule_003E.GetLastError();
				}
				switch (num2)
				{
				case 259u:
					return null;
				default:
					throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventChannelEnumerationFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
				case 0u:
					break;
				}
			}
			goto IL_0091;
			IL_0091:
			return InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private unsafe static string EventIntGetClassicLogDisplayName(EventSafeHandle session, string logName)
	{
		//Discarded unreachable code: IL_0134
		//IL_0078: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		if (s_evtIntGetClassicLogDisplayName == (delegate* unmanaged[Cdecl, Cdecl]<void*, ushort*, uint, uint, int, ushort*, uint*, int>)null)
		{
			HINSTANCE__* ptr = global::_003CModule_003E.LoadLibraryW((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BI_0040IJHAFJEP_0040_003F_0024AAw_003F_0024AAe_003F_0024AAv_003F_0024AAt_003F_0024AAa_003F_0024AAp_003F_0024AAi_003F_0024AA_003F4_003F_0024AAd_003F_0024AAl_003F_0024AAl_0040));
			if (ptr == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[2]
				{
					Resources.EventGetClassicLogNameFailed_Text,
					logName
				}, resultCode: (int)global::_003CModule_003E.GetLastError());
			}
			s_evtIntGetClassicLogDisplayName = (delegate* unmanaged[Cdecl, Cdecl]<void*, ushort*, uint, uint, int, ushort*, uint*, int>)global::_003CModule_003E.GetProcAddress(ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BP_0040CKPHNGKJ_0040EvtIntGetClassicLogDisplayName_0040));
			if (s_evtIntGetClassicLogDisplayName == (delegate* unmanaged[Cdecl, Cdecl]<void*, ushort*, uint, uint, int, ushort*, uint*, int>)null)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventGetClassicLogNameFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
			}
		}
		ushort* ptr2 = null;
		UnmanagedBuffer unmanagedBuffer = null;
		try
		{
			ptr2 = InteropHelp.StringToWstr(logName);
			uint num = 1024u;
			UnmanagedBuffer unmanagedBuffer2 = new UnmanagedBuffer();
			unmanagedBuffer2.Allocate(num);
			uint num2 = 0u;
			if (s_evtIntGetClassicLogDisplayName(session.Handle, ptr2, 0u, 0u, (int)num, (ushort*)unmanagedBuffer2.Pointer, &num) == 0)
			{
				num2 = global::_003CModule_003E.GetLastError();
				if (num2 == 122)
				{
					unmanagedBuffer2.Allocate(num);
					num2 = 0u;
					if (s_evtIntGetClassicLogDisplayName(session.Handle, ptr2, 0u, 0u, (int)num, (ushort*)unmanagedBuffer2.Pointer, &num) != 0)
					{
						goto IL_0110;
					}
					num2 = global::_003CModule_003E.GetLastError();
				}
				if (num2 != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventGetClassicLogNameFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
				}
			}
			goto IL_0110;
			IL_0110:
			return InteropHelp.WstrToString((ushort*)unmanagedBuffer2.Pointer);
		}
		finally
		{
			if (ptr2 != null)
			{
				InteropHelp.FreeWstr(ptr2);
			}
			unmanagedBuffer?.Free();
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EEventLogSession();
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
