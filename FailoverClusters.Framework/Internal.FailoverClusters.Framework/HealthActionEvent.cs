using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_HealthActionEvent : MSFT_StorageEvent
{
	public ushort? ChangeType => (ushort?)base.Instance.CimInstanceProperties["ChangeType"].Value;

	public string HealthActionId => (string)base.Instance.CimInstanceProperties["HealthActionId"].Value;

	public string HealthActionType => (string)base.Instance.CimInstanceProperties["HealthActionType"].Value;

	public ushort? PercentComplete => (ushort?)base.Instance.CimInstanceProperties["PercentComplete"].Value;

	public string Reason => (string)base.Instance.CimInstanceProperties["Reason"].Value;

	public string StorageSubsystemUniqueId => (string)base.Instance.CimInstanceProperties["StorageSubsystemUniqueId"].Value;

	public MSFT_HealthActionEvent()
	{
	}

	public MSFT_HealthActionEvent(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_HealthActionEvent> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_HealthActionEvent"))
		{
			yield return new MSFT_HealthActionEvent(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_HealthActionEvent> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_HealthActionEvent(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_HealthActionEvent").Subscribe(observer2);
	}
}

