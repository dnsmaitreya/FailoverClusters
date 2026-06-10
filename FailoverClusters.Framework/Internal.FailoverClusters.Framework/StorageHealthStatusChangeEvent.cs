using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageHealthStatusChangeEvent : MSFT_StorageEvent
{
	public ushort? CurrentHealthStatus => (ushort?)base.Instance.CimInstanceProperties["CurrentHealthStatus"].Value;

	public ushort? PreviousHealthStatus => (ushort?)base.Instance.CimInstanceProperties["PreviousHealthStatus"].Value;

	public string SourceUniqueId => (string)base.Instance.CimInstanceProperties["SourceUniqueId"].Value;

	public string StorageSubsystemUniqueId => (string)base.Instance.CimInstanceProperties["StorageSubsystemUniqueId"].Value;

	public MSFT_StorageHealthStatusChangeEvent()
	{
	}

	public MSFT_StorageHealthStatusChangeEvent(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_StorageHealthStatusChangeEvent> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_StorageHealthStatusChangeEvent"))
		{
			yield return new MSFT_StorageHealthStatusChangeEvent(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_StorageHealthStatusChangeEvent> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_StorageHealthStatusChangeEvent(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_StorageHealthStatusChangeEvent").Subscribe(observer2);
	}
}

