using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageAlertEvent : MSFT_StorageEvent
{
	public ushort? AlertType => (ushort?)base.Instance.CimInstanceProperties["AlertType"].Value;

	public MSFT_StorageAlertEvent()
	{
	}

	public MSFT_StorageAlertEvent(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_StorageAlertEvent> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/windows/storage", "WQL", "SELECT * FROM MSFT_StorageAlertEvent"))
		{
			yield return new MSFT_StorageAlertEvent(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_StorageAlertEvent> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_StorageAlertEvent(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/windows/storage", "WQL", "SELECT * FROM MSFT_StorageAlertEvent").Subscribe(observer2);
	}
}

