using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MS.Internal.ServerClusters;

public class EventLogPublisher : IDisposable
{
	private EventSafeHandle m_handle;

	private Dictionary<int, string> m_tasksCache;

	private Dictionary<int, string> m_opcodesCache;

	private Dictionary<ulong, string> m_keywordsCache;

	public EventLogPublisher(EventLogSession session, string publisher)
	{
		//Discarded unreachable code: IL_0081
		if (session == null)
		{
			throw new ArgumentNullException("session");
		}
		if (publisher == null)
		{
			throw new ArgumentNullException("publisher");
		}
		m_tasksCache = new Dictionary<int, string>();
		m_opcodesCache = new Dictionary<int, string>();
		m_keywordsCache = new Dictionary<ulong, string>();
		try
		{
			m_handle = EventOpenPublisherMetadata(session.Handle, publisher);
		}
		catch (Exception caughtException)
		{
			if (ExceptionHelp.IsFirstExceptionFound<Win32Exception>(caughtException))
			{
				ExceptionHelp.LogException(caughtException, "Error opening the metadata for publisher {0}", publisher);
				m_handle = new EventSafeHandle();
				return;
			}
			throw;
		}
	}

	public unsafe string GetChannelName(string channel)
	{
		//Discarded unreachable code: IL_00ad
		//IL_0022: Expected I, but got I8
		//IL_0053: Expected I, but got I8
		//IL_0087: Expected I, but got I8
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			EventGetPublisherMetadataProperty(m_handle, (_EVT_PUBLISHER_METADATA_PROPERTY_ID)6, unmanagedBuffer);
			EventSafeHandle objectArray = new EventSafeHandle((void*)(*(ulong*)unmanagedBuffer.Pointer));
			uint num = EventGetObjectArraySize(objectArray);
			for (uint num2 = 0u; num2 < num; num2++)
			{
				UnmanagedBuffer unmanagedBuffer2 = new UnmanagedBuffer();
				UnmanagedBuffer unmanagedBuffer3 = new UnmanagedBuffer();
				try
				{
					EventGetObjectArrayProperty(objectArray, (_EVT_PUBLISHER_METADATA_PROPERTY_ID)7, (int)num2, unmanagedBuffer2);
					string value = InteropHelp.WstrToString((ushort*)(*(ulong*)unmanagedBuffer2.Pointer));
					if (channel.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						EventGetObjectArrayProperty(objectArray, (_EVT_PUBLISHER_METADATA_PROPERTY_ID)11, (int)num2, unmanagedBuffer3);
						uint pointer = *(uint*)unmanagedBuffer3.Pointer;
						if (pointer == uint.MaxValue)
						{
							break;
						}
						return FormatMessage(null, (int)pointer, 0, null, (_EVT_FORMAT_MESSAGE_FLAGS)8);
					}
				}
				finally
				{
					unmanagedBuffer2.Free();
					unmanagedBuffer3.Free();
				}
			}
			return channel;
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private void _007EEventLogPublisher()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_handle?.Close();
	}

	internal unsafe string FormatTask(EventSafeHandle eventHandle, int task)
	{
		//IL_002b: Expected I, but got I8
		string text = null;
		if (task == 0)
		{
			return Resources.NoTask_Text;
		}
		text = null;
		if (m_tasksCache.TryGetValue(task, out text))
		{
			return text;
		}
		text = TryFormatMessage(eventHandle, task, 0, null, (_EVT_FORMAT_MESSAGE_FLAGS)3);
		if (text == null)
		{
			text = string.Format(CultureInfo.InvariantCulture, "({0})", task);
		}
		m_tasksCache.Add(task, text);
		return text;
	}

	internal unsafe string FormatOpCode(EventSafeHandle eventHandle, int opCode, int task)
	{
		//IL_0027: Expected I, but got I8
		//IL_003d: Expected I, but got I8
		string value = null;
		if (m_opcodesCache.TryGetValue(opCode, out value))
		{
			return value;
		}
		int num = opCode << 16;
		value = TryFormatMessage(eventHandle, num | task, 0, null, (_EVT_FORMAT_MESSAGE_FLAGS)4);
		if (value == null)
		{
			value = TryFormatMessage(eventHandle, num, 0, null, (_EVT_FORMAT_MESSAGE_FLAGS)4);
		}
		if (value == null)
		{
			value = string.Format(CultureInfo.InvariantCulture, "({0})", opCode);
		}
		m_opcodesCache.Add(opCode, value);
		return value;
	}

	internal string FormatKeywords(EventSafeHandle eventHandle, ulong keywords)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ulong num = keywords & 0xFFFFFFFFFFFFFFuL;
		ulong num2 = 36028797018963968uL;
		if (num != 0L)
		{
			do
			{
				ulong num3 = num2 & num;
				if (num3 != 0L)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendFormat("{0} ", CultureInfo.CurrentCulture.TextInfo.ListSeparator);
					}
					stringBuilder.Append(GetFormattedKeyword(eventHandle, num3));
					num &= ~num3;
				}
				num2 >>= 1;
			}
			while (num != 0L);
		}
		return stringBuilder.ToString();
	}

	internal unsafe string FormatMessage(EventSafeHandle eventHandle, int messageId, int valueCount, _EVT_VARIANT* valueBuffer, _EVT_FORMAT_MESSAGE_FLAGS flags)
	{
		//Discarded unreachable code: IL_00d5
		//IL_0030: Expected I, but got I8
		//IL_007c: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer();
		try
		{
			uint num = 4096u;
			unmanagedBuffer.Allocate(8192uL);
			uint num2 = 0u;
			void* ptr = ((eventHandle == null) ? null : eventHandle.Handle);
			if (global::_003CModule_003E.EvtFormatMessage(m_handle.Handle, ptr, (uint)messageId, (uint)valueCount, valueBuffer, (uint)flags, num, (ushort*)unmanagedBuffer.Pointer, &num) == 0)
			{
				num2 = global::_003CModule_003E.GetLastError();
				if (num2 == 122)
				{
					unmanagedBuffer.Allocate((ulong)num * 2uL);
					num2 = 0u;
					void* ptr2 = ((eventHandle == null) ? null : eventHandle.Handle);
					if (global::_003CModule_003E.EvtFormatMessage(m_handle.Handle, ptr2, (uint)messageId, (uint)valueCount, valueBuffer, (uint)flags, num, (ushort*)unmanagedBuffer.Pointer, &num) != 0)
					{
						goto IL_00bf;
					}
					num2 = global::_003CModule_003E.GetLastError();
				}
				if (num2 != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[1] { Resources.FormatMessageFailed_Text });
				}
			}
			goto IL_00bf;
			IL_00bf:
			return InteropHelp.WstrToString((ushort*)unmanagedBuffer.Pointer);
		}
		finally
		{
			unmanagedBuffer.Free();
		}
	}

	private unsafe string GetFormattedKeyword(EventSafeHandle eventHandle, ulong keyword)
	{
		//IL_001f: Expected I4, but got I8
		string value = null;
		if (m_keywordsCache.TryGetValue(keyword, out value))
		{
			return value;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _EVT_VARIANT eVT_VARIANT);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref eVT_VARIANT, 0, 16);
		System.Runtime.CompilerServices.Unsafe.As<_EVT_VARIANT, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref eVT_VARIANT, 12)) = 10;
		*(ulong*)(&eVT_VARIANT) = keyword;
		value = TryFormatMessage(eventHandle, -1, 1, &eVT_VARIANT, (_EVT_FORMAT_MESSAGE_FLAGS)5);
		if (value == null)
		{
			value = string.Format(CultureInfo.InvariantCulture, "({0})", keyword);
		}
		m_keywordsCache.Add(keyword, value);
		return value;
	}

	private unsafe string TryFormatMessage(EventSafeHandle eventHandle, int messageId, int valueCount, _EVT_VARIANT* valueBuffer, _EVT_FORMAT_MESSAGE_FLAGS flags)
	{
		//Discarded unreachable code: IL_004c, IL_0050
		try
		{
			return FormatMessage(eventHandle, messageId, valueCount, valueBuffer, flags);
		}
		catch (Exception caughtException)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null)
			{
				if (firstException.NativeErrorCode != -2147009869 && firstException.NativeErrorCode != -2147009868)
				{
					ExceptionHelp.LogException(caughtException, "Failed to format message id {0}", messageId);
				}
				return null;
			}
			throw;
		}
	}

	private unsafe static EventSafeHandle EventOpenPublisherMetadata(EventSafeHandle session, string publisherId)
	{
		//Discarded unreachable code: IL_005f
		//IL_0016: Expected I, but got I8
		//IL_002d: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		if (session.IsInvalid)
		{
			return new EventSafeHandle();
		}
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(publisherId);
			void* ptr2 = global::_003CModule_003E.EvtOpenPublisherMetadata(session.Handle, ptr, null, 0u, 0u);
			if (ptr2 == null)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)global::_003CModule_003E.GetLastError(), new string[1] { Resources.EventPublisherOpenFailed_Text });
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

	private unsafe static void EventGetPublisherMetadataProperty(EventSafeHandle publisherMetadata, _EVT_PUBLISHER_METADATA_PROPERTY_ID propertyId, UnmanagedBuffer buffer)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		buffer.Allocate(16uL);
		uint num = 0u;
		if (global::_003CModule_003E.EvtGetPublisherMetadataProperty(publisherMetadata.Handle, propertyId, 0u, (uint)buffer.Size, (_EVT_VARIANT*)buffer.Pointer, &num) != 0)
		{
			return;
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		if (lastError == 122)
		{
			buffer.Allocate(num);
			if (global::_003CModule_003E.EvtGetPublisherMetadataProperty(publisherMetadata.Handle, propertyId, 0u, (uint)buffer.Size, (_EVT_VARIANT*)buffer.Pointer, &num) != 0)
			{
				return;
			}
			lastError = global::_003CModule_003E.GetLastError();
		}
		if (lastError != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)lastError, new string[1] { Resources.EventQueryPublisherFailed_Text });
		}
	}

	private unsafe static uint EventGetObjectArraySize(EventSafeHandle objectArray)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		uint result = 0u;
		if (global::_003CModule_003E.EvtGetObjectArraySize(objectArray.Handle, &result) == 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventQueryFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
		return result;
	}

	private unsafe static void EventGetObjectArrayProperty(EventSafeHandle objectArray, _EVT_PUBLISHER_METADATA_PROPERTY_ID propertyId, int arrayIndex, UnmanagedBuffer buffer)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		buffer.Allocate(16uL);
		uint num = 0u;
		if (global::_003CModule_003E.EvtGetObjectArrayProperty(objectArray.Handle, (uint)propertyId, (uint)arrayIndex, 0u, (uint)buffer.Size, (_EVT_VARIANT*)buffer.Pointer, &num) != 0)
		{
			return;
		}
		uint lastError = global::_003CModule_003E.GetLastError();
		if (lastError == 122)
		{
			buffer.Allocate(num);
			if (global::_003CModule_003E.EvtGetObjectArrayProperty(objectArray.Handle, (uint)propertyId, (uint)arrayIndex, 0u, (uint)buffer.Size, (_EVT_VARIANT*)buffer.Pointer, &num) != 0)
			{
				return;
			}
			lastError = global::_003CModule_003E.GetLastError();
		}
		if (lastError != 0)
		{
			throw ExceptionHelp.Build<ApplicationException>((int)lastError, new string[1] { Resources.EventQueryFailed_Text });
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EEventLogPublisher();
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
