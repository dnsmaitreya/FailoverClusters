using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageEvent : MiBase
{
	public string Description => (string)base.Instance.CimInstanceProperties["Description"].Value;

	public DateTime? EventTime => (DateTime?)base.Instance.CimInstanceProperties["EventTime"].Value;

	public ushort? PerceivedSeverity => (ushort?)base.Instance.CimInstanceProperties["PerceivedSeverity"].Value;

	public string SourceClassName => (string)base.Instance.CimInstanceProperties["SourceClassName"].Value;

	public MSFT_StorageObject SourceInstance => new MSFT_StorageObject(base.Session, (CimInstance)base.Instance.CimInstanceProperties["SourceInstance"].Value);

	public string SourceNamespace => (string)base.Instance.CimInstanceProperties["SourceNamespace"].Value;

	public string SourceObjectId => (string)base.Instance.CimInstanceProperties["SourceObjectId"].Value;

	public string SourceServer => (string)base.Instance.CimInstanceProperties["SourceServer"].Value;

	public string StorageSubsystemObjectId => (string)base.Instance.CimInstanceProperties["StorageSubsystemObjectId"].Value;

	public MSFT_StorageEvent()
	{
	}

	public MSFT_StorageEvent(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static IEnumerable<MSFT_StorageEvent> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_StorageEvent"))
		{
			yield return new MSFT_StorageEvent(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_StorageEvent> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_StorageEvent(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_StorageEvent").Subscribe(observer2);
	}
}

