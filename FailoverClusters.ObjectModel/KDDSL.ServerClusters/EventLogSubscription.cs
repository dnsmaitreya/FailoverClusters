using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class EventLogSubscription : IDisposable
{
	private EventHandler<EventLogEventLoggedEventArgs> _003Cbacking_store_003EEventLogged;

	private EventLogSession m_session;

	private SafeHandle m_handle;

	private unsafe EventLogSubscriptionCallbackWrapper* m_callbackWrapper = null;

	private bool m_disposed;

	public bool IsDisposed
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_disposed;
		}
	}

	public EventLogSession Session => m_session;

	[SpecialName]
	public event EventHandler<EventLogEventLoggedEventArgs> EventLogged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			_003Cbacking_store_003EEventLogged = (EventHandler<EventLogEventLoggedEventArgs>)Delegate.Combine(_003Cbacking_store_003EEventLogged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			_003Cbacking_store_003EEventLogged = (EventHandler<EventLogEventLoggedEventArgs>)Delegate.Remove(_003Cbacking_store_003EEventLogged, value);
		}
	}

	public unsafe void Subscribe(string nodeName, string channel, string query)
	{
		//IL_0081: Expected I, but got I8
		if (nodeName == null)
		{
			throw new ArgumentNullException("nodeName");
		}
		if (channel == null)
		{
			throw new ArgumentNullException("channel");
		}
		if (query == null)
		{
			throw new ArgumentNullException("query");
		}
		m_session = new EventLogSession(nodeName);
		if (m_callbackWrapper != null)
		{
			throw new InvalidOperationException("The subscription can be started only once");
		}
		EventLogSubscriptionCallbackDelegate callbackDelegate = ManagedCallback;
		EventLogSubscriptionCallbackWrapper* ptr = (EventLogSubscriptionCallbackWrapper*)global::_003CModule_003E.@new(16uL);
		EventLogSubscriptionCallbackWrapper* callbackWrapper;
		try
		{
			callbackWrapper = ((ptr == null) ? null : global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EEventLogSubscriptionCallbackWrapper_002E_007Bctor_007D(ptr, callbackDelegate));
		}
		catch
		{
			//try-fault
			global::_003CModule_003E.delete(ptr);
			throw;
		}
		m_callbackWrapper = callbackWrapper;
		m_handle = EventSubscribe(m_session.Handle, channel, query, callbackWrapper);
	}

	[SpecialName]
	protected void raise_EventLogged(object value0, EventLogEventLoggedEventArgs value1)
	{
		_003Cbacking_store_003EEventLogged?.Invoke(value0, value1);
	}

	private unsafe void _007EEventLogSubscription()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		m_disposed = true;
		EventLogSubscriptionCallbackWrapper* callbackWrapper = m_callbackWrapper;
		if (callbackWrapper != null)
		{
			EventLogSubscriptionCallbackWrapper* ptr = callbackWrapper;
			global::_003CModule_003E.MS_002EInternal_002EServerClusters_002EEventLogSubscriptionCallbackWrapper_002E_007Bdtor_007D(ptr);
			global::_003CModule_003E.delete(ptr);
		}
		SafeHandle handle = m_handle;
		if (handle != null)
		{
			handle.Close();
			m_handle = null;
		}
		EventLogSession session = m_session;
		if (session != null)
		{
			((IDisposable)session).Dispose();
			m_session = null;
		}
	}

	internal unsafe void ManagedCallback(void* eventHandle)
	{
		EventLogEvent eventLogEvent = new EventLogEvent(new EventSafeHandle(eventHandle, ownsHandle: false), m_session);
		OnEventLogged(new EventLogEventLoggedEventArgs(eventLogEvent));
	}

	protected void OnEventLogged(EventLogEventLoggedEventArgs e)
	{
		_003Cbacking_store_003EEventLogged?.Invoke(this, e);
	}

	private unsafe static EventSafeHandle EventSubscribe(EventSafeHandle m_session, string channel, string query, EventLogSubscriptionCallbackWrapper* callbackWrapper)
	{
		//Discarded unreachable code: IL_006d
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		//IL_0031: Expected I, but got I8
		//IL_0031: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(channel);
			ptr2 = InteropHelp.StringToWstr(query);
			void* ptr3 = global::_003CModule_003E.EvtSubscribe(m_session.Handle, null, ptr, ptr2, null, callbackWrapper, (delegate* unmanaged[Cdecl, Cdecl]<_EVT_SUBSCRIBE_NOTIFY_ACTION, void*, void*, uint>)global::_003CModule_003E.__unep_0040_003FCallback_0040EventLogSubscriptionCallbackWrapper_0040ServerClusters_0040Internal_0040MS_0040_0040_0024_0024FSAKW4_EVT_SUBSCRIBE_NOTIFY_ACTION_0040_0040PEAX1_0040Z, 1u);
			if (ptr3 == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[1] { Resources.EventSubscribeFailed_Text }, resultCode: (int)global::_003CModule_003E.GetLastError());
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

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EEventLogSubscription();
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
