#define DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace MS.Internal.ServerClusters;

public class EventLogEvent
{
	private static Dictionary<string, string> UserNameCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private EventLogSession m_session;

	private string m_dummyItemReason;

	private string m_provider;

	private string m_source;

	private int m_eventid;

	private DateTime m_timeCreated;

	private string m_channel;

	private string m_computer;

	private int m_level;

	private string m_levelName;

	private string m_sid;

	private string m_user;

	private string m_opCode;

	private string m_task;

	private string m_keywords;

	private string m_messageText;

	private object m_tag;

	public string Message => m_messageText;

	public object Tag
	{
		get
		{
			return m_tag;
		}
		set
		{
			m_tag = value;
		}
	}

	public string User => m_user;

	public string Keywords => m_keywords;

	public string Task => m_task;

	public string OpCode => m_opCode;

	public string LevelName => m_levelName;

	public int Level => m_level;

	public string Source => m_source;

	public string Provider => m_provider;

	public string Computer => m_computer;

	public string Channel => m_channel;

	public DateTime TimeCreated => m_timeCreated;

	public int EventId => m_eventid;

	public string DummyItemReason => m_dummyItemReason;

	public bool IsDummyItem
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_dummyItemReason != null;
		}
	}

	internal unsafe EventLogEvent(EventSafeHandle eventHandle, EventLogSession session)
	{
		m_session = session;
		m_dummyItemReason = null;
		List<object> list = RenderEvent(eventHandle, (ushort**)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003FA0x0e3b6464_002E_003FeventProperties_0040_003F1_003F_003F_003F0EventLogEvent_0040ServerClusters_0040Internal_0040MS_0040_0040QE_0024AAM_0040PE_0024AAVEventSafeHandle_0040234_0040PE_0024AAVEventLogSession_0040234_0040_0040Z_00404PAPEAGA), 10u);
		m_source = session.GetPublisherShortName(m_provider = (string)list[0]);
		m_eventid = (int)list[1];
		ref DateTime timeCreated = ref m_timeCreated;
		timeCreated = (DateTime)list[2];
		m_channel = (string)list[3];
		m_computer = (string)list[4];
		m_levelName = FormatLevel(m_level = (int)list[5]);
		if ((m_sid = (string)list[6]) == null)
		{
			m_user = Resources.NotApplicable_Text;
		}
		else
		{
			m_user = SidToUserName(m_sid, m_computer);
		}
		EventLogPublisher publisher = session.GetPublisher(m_provider);
		int task = (int)list[7];
		m_task = publisher.FormatTask(eventHandle, task);
		if (list[8] == null)
		{
			m_opCode = string.Empty;
		}
		else
		{
			m_opCode = publisher.FormatOpCode(eventHandle, (int)list[8], task);
		}
		m_keywords = publisher.FormatKeywords(eventHandle, (ulong)list[9]);
		GetMessage(eventHandle);
		m_tag = null;
	}

	public EventLogEvent(string dummyItemReason, string message)
	{
		InitializeDummyEvent(dummyItemReason, message);
	}

	public EventLogEvent(Exception exception)
	{
		InitializeDummyEvent(Resources.Unavailable_Text, string.Format(CultureInfo.CurrentCulture, Resources.CannotReadEventLog_Text, ExceptionHelp.GetExceptionMessage(exception)));
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool Contains(string text, StringComparison comparison)
	{
		int num;
		if (m_levelName.IndexOf(text, comparison) == -1)
		{
			int eventid = m_eventid;
			if (eventid.ToString(CultureInfo.CurrentCulture).IndexOf(text, comparison) == -1)
			{
				DateTime timeCreated = m_timeCreated;
				if (timeCreated.ToString("G", CultureInfo.CurrentCulture).IndexOf(text, comparison) == -1 && m_computer.IndexOf(text, comparison) == -1 && m_channel.IndexOf(text, comparison) == -1 && m_messageText.IndexOf(text, comparison) == -1)
				{
					num = 0;
					goto IL_008d;
				}
			}
		}
		num = 1;
		goto IL_008d;
		IL_008d:
		return (byte)num != 0;
	}

	internal unsafe static string GetChannel(EventSafeHandle eventHandle)
	{
		ushort* ptr = (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1CK_0040CHBBOFD_0040_003F_0024AAE_003F_0024AAv_003F_0024AAe_003F_0024AAn_003F_0024AAt_003F_0024AA_003F1_003F_0024AAS_003F_0024AAy_003F_0024AAs_003F_0024AAt_003F_0024AAe_003F_0024AAm_003F_0024AA_003F1_003F_0024AAC_003F_0024AAh_0040);
		return (string)RenderEvent(eventHandle, &ptr, 1u)[0];
	}

	private unsafe void GetMessage(EventSafeHandle eventHandle)
	{
		//IL_002c: Expected I, but got I8
		try
		{
			string provider = m_provider;
			EventLogPublisher publisher = m_session.GetPublisher(provider);
			int eventid = m_eventid;
			m_messageText = publisher.FormatMessage(eventHandle, eventid, 0, null, (_EVT_FORMAT_MESSAGE_FLAGS)1);
		}
		catch (Exception caughtException)
		{
			if (!ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
			{
				throw;
			}
			string provider2 = m_provider;
			m_session.MarkInvalidPublisher(provider2);
			int eventid2 = m_eventid;
			ExceptionHelp.LogException(caughtException, "Error retrieving description for Crimson event {0}", eventid2);
			m_messageText = Resources.CannotGetEventDescription_Text;
		}
	}

	private void InitializeDummyEvent(string dummyItemReason, string message)
	{
		m_session = null;
		m_dummyItemReason = dummyItemReason;
		m_provider = string.Empty;
		m_source = string.Empty;
		m_eventid = int.MinValue;
		m_timeCreated = DateTime.MinValue;
		m_channel = string.Empty;
		m_computer = string.Empty;
		m_level = int.MinValue;
		m_levelName = string.Empty;
		m_sid = string.Empty;
		m_user = string.Empty;
		m_task = string.Empty;
		m_opCode = string.Empty;
		m_keywords = string.Empty;
		m_messageText = message;
		m_tag = null;
	}

	private static string FormatLevel(int level)
	{
		switch (level)
		{
		default:
		{
			int num = level;
			Debug.Fail("Unknown event level: " + num.ToString(CultureInfo.InvariantCulture));
			int num2 = level;
			return num2.ToString(CultureInfo.InvariantCulture);
		}
		case 5:
			return Resources.Verbose_Text;
		case 3:
			return Resources.Warning_Text;
		case 2:
			return Resources.Error_Text;
		case 1:
			return Resources.Critical_Text;
		case 0:
		case 4:
			return Resources.Informational_Text;
		}
	}

	private unsafe static List<object> RenderEvent(EventSafeHandle eventHandle, ushort** properties, uint propertiesCount)
	{
		//Discarded unreachable code: IL_0051
		//IL_003f: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			uint num = EventRender(EventCreateRenderContext(propertiesCount, properties, 0u), eventHandle, unmanagedBuffer);
			List<object> list = new List<object>((int)num);
			_EVT_VARIANT* ptr = (_EVT_VARIANT*)unmanagedBuffer.Pointer;
			for (uint num2 = 0u; num2 < num; num2++)
			{
				list.Add(ToObject(ptr));
				ptr = (_EVT_VARIANT*)((ulong)(nint)ptr + 16uL);
			}
			return list;
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private unsafe static object ToObject(_EVT_VARIANT* variant)
	{
		//IL_005b: Expected I, but got I8
		//IL_006a: Expected I, but got I8
		//IL_00af: Expected I, but got I8
		//IL_00b8: Expected I, but got I8
		//IL_00fe: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		uint num = *(uint*)((ulong)(nint)variant + 12uL);
		if (num <= 7)
		{
			switch (num)
			{
			case 6u:
				return (int)(*(ushort*)variant);
			case 5u:
				return (int)(*(short*)variant);
			case 4u:
				return (int)(*(byte*)variant);
			case 3u:
				return (int)(*(sbyte*)variant);
			case 2u:
			{
				IntPtr ptr = new IntPtr((void*)(*(ulong*)variant));
				return Marshal.PtrToStringAnsi(ptr);
			}
			case 1u:
				return InteropHelp.WstrToString((ushort*)(*(ulong*)variant));
			case 7u:
				return *(int*)variant;
			case 0u:
				return null;
			}
		}
		else
		{
			switch (num)
			{
			case 19u:
			{
				ushort* ptr2 = null;
				try
				{
					if (global::_003CModule_003E.ConvertSidToStringSidW((void*)(*(ulong*)variant), &ptr2) == 0)
					{
						DebugLog.LogError("Failure in ConvertSidToStringSid: {0}", global::_003CModule_003E.GetLastError());
						return string.Empty;
					}
					return InteropHelp.WstrToString(ptr2);
				}
				finally
				{
					if (ptr2 != null)
					{
						global::_003CModule_003E.LocalFree(ptr2);
					}
				}
			}
			case 18u:
				return DateTime.FromFileTime((long)SystemTimeToFileTime((_SYSTEMTIME*)(*(ulong*)variant)));
			case 17u:
				return DateTime.FromFileTime(*(long*)variant);
			case 10u:
			case 21u:
				return *(ulong*)variant;
			case 8u:
				return *(int*)variant;
			}
		}
		throw new ArgumentException("Unexpected variant type", "variant");
	}

	private static string SidToUserName(string sid, string machine)
	{
		//Discarded unreachable code: IL_0037
		Exception ex = null;
		string value = null;
		if (!UserNameCache.TryGetValue(sid, out value))
		{
			try
			{
				value = LookupAccountSidW(sid, machine);
			}
			catch (Exception caughtException)
			{
				if (!ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
				{
					throw;
				}
				ExceptionHelp.LogException(caughtException, "Cannot get account from SID");
				value = sid;
			}
			UserNameCache[sid] = value;
		}
		return value;
	}

	private unsafe static ulong SystemTimeToFileTime(_SYSTEMTIME* systemTime)
	{
		//IL_000a: Expected I4, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _FILETIME fILETIME);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref fILETIME, 0, 8);
		if (global::_003CModule_003E.SystemTimeToFileTime(systemTime, &fILETIME) == 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.SystemTimeToFileTimeFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _ULARGE_INTEGER uLARGE_INTEGER);
		*(int*)(&uLARGE_INTEGER) = *(int*)(&fILETIME);
		System.Runtime.CompilerServices.Unsafe.As<_ULARGE_INTEGER, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref uLARGE_INTEGER, 4)) = System.Runtime.CompilerServices.Unsafe.As<_FILETIME, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref fILETIME, 4));
		return *(ulong*)(&uLARGE_INTEGER);
	}

	private unsafe static string LookupAccountSidW(string sid, string computer)
	{
		//Discarded unreachable code: IL_01c9
		//IL_0009: Expected I, but got I8
		//IL_000d: Expected I, but got I8
		//IL_0010: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		void* ptr = null;
		ushort* ptr2 = null;
		ushort* ptr3 = null;
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		UnmanagedBuffer unmanagedBuffer2 = new UnmanagedBuffer();
		try
		{
			ptr2 = InteropHelp.StringToWstr(sid);
			ptr3 = InteropHelp.StringToWstr(computer);
			if (global::_003CModule_003E.ConvertStringSidToSidW(ptr2, &ptr) == 0)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.ConvertStringSidToSidFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
			}
			unmanagedBuffer.Allocate(256uL);
			unmanagedBuffer2.Allocate(256uL);
			uint num = (uint)unmanagedBuffer.Size;
			uint num2 = (uint)unmanagedBuffer2.Size;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _SID_NAME_USE sID_NAME_USE);
			if (global::_003CModule_003E.LookupAccountSidW(ptr3, ptr, (ushort*)unmanagedBuffer2.Pointer, &num2, (ushort*)unmanagedBuffer.Pointer, &num, &sID_NAME_USE) == 0)
			{
				uint lastError = global::_003CModule_003E.GetLastError();
				if (lastError != 122)
				{
					throw ExceptionHelp.Build<ApplicationException>((int)lastError, new string[1] { Resources.LookupAccountSidFailed_Text });
				}
				unmanagedBuffer.Allocate(num);
				unmanagedBuffer2.Allocate(num2);
				if (global::_003CModule_003E.LookupAccountSidW(ptr3, ptr, (ushort*)unmanagedBuffer2.Pointer, &num2, (ushort*)unmanagedBuffer.Pointer, &num, &sID_NAME_USE) == 0)
				{
					throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.LookupAccountSidFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
				}
			}
			string text = InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
			string text2 = InteropHelp.WstrToString((ushort*)unmanagedBuffer2.Pointer);
			IntPtr binaryForm = new IntPtr(ptr);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(binaryForm);
			if (!securityIdentifier.IsWellKnown(WellKnownSidType.NetworkServiceSid) && !securityIdentifier.IsWellKnown(WellKnownSidType.AnonymousSid) && !securityIdentifier.IsWellKnown(WellKnownSidType.LocalSystemSid) && !securityIdentifier.IsWellKnown(WellKnownSidType.LocalServiceSid) && !securityIdentifier.IsWellKnown(WellKnownSidType.LocalSid))
			{
				return string.Format(arg0: (text.Length != 0) ? text : computer, provider: CultureInfo.InvariantCulture, format: "{0}\\{1}", arg1: text2);
			}
			return text2;
		}
		finally
		{
			if (ptr != null)
			{
				global::_003CModule_003E.LocalFree(ptr);
			}
			if (ptr2 != null)
			{
				InteropHelp.FreeWstr(ptr2);
			}
			if (ptr3 != null)
			{
				InteropHelp.FreeWstr(ptr3);
			}
			unmanagedBuffer.Free();
			unmanagedBuffer2.Free();
		}
	}

	private unsafe static EventSafeHandle EventCreateRenderContext(uint valuePathsCount, ushort** valuePaths, uint flags)
	{
		void* ptr = global::_003CModule_003E.EvtCreateRenderContext(valuePathsCount, valuePaths, flags);
		if (ptr == null)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventRenderContextFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
		return new EventSafeHandle(ptr);
	}

	private unsafe static uint EventRender(EventSafeHandle context, EventSafeHandle eventHandle, UnmanagedBuffer buffer)
	{
		buffer.Allocate(4096uL);
		uint num = 0u;
		uint result = 0u;
		if (global::_003CModule_003E.EvtRender(context.Handle, eventHandle.Handle, 0u, (uint)buffer.Size, buffer.Pointer, &num, &result) == 0)
		{
			uint lastError = global::_003CModule_003E.GetLastError();
			if (lastError == 122)
			{
				buffer.Allocate(num);
				if (global::_003CModule_003E.EvtRender(context.Handle, eventHandle.Handle, 0u, (uint)buffer.Size, buffer.Pointer, &num, &result) != 0)
				{
					goto IL_0091;
				}
				lastError = global::_003CModule_003E.GetLastError();
			}
			if (lastError != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventRenderFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
			}
		}
		goto IL_0091;
		IL_0091:
		return result;
	}
}
