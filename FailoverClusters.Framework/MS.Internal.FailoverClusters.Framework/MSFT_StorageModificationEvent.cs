using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageModificationEvent : MSFT_StorageEvent
{
	public MSFT_StorageModificationEvent()
	{
	}

	public MSFT_StorageModificationEvent(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_StorageModificationEvent> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_StorageModificationEvent"))
		{
			yield return new MSFT_StorageModificationEvent(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_StorageModificationEvent> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_StorageModificationEvent(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_StorageModificationEvent").Subscribe(observer2);
	}
}

