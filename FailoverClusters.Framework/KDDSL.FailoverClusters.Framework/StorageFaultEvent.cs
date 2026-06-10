using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageFaultEvent : MSFT_StorageEvent
{
	public ushort? ChangeType => (ushort?)base.Instance.CimInstanceProperties["ChangeType"].Value;

	public string FaultId => (string)base.Instance.CimInstanceProperties["FaultId"].Value;

	public MSFT_StorageObject FaultingObject
	{
		get
		{
			return new MSFT_StorageObject(base.Session, (CimInstance)base.Instance.CimInstanceProperties["FaultingObject"].Value);
		}
		set
		{
			base.Instance.CimInstanceProperties["FaultingObject"].Value = value?.Instance;
		}
	}

	public string FaultingObjectDescription => (string)base.Instance.CimInstanceProperties["FaultingObjectDescription"].Value;

	public string FaultingObjectLocation => (string)base.Instance.CimInstanceProperties["FaultingObjectLocation"].Value;

	public string FaultType => (string)base.Instance.CimInstanceProperties["FaultType"].Value;

	public string Reason => (string)base.Instance.CimInstanceProperties["Reason"].Value;

	public string[] RecommendedActions => (string[])base.Instance.CimInstanceProperties["RecommendedActions"].Value;

	public string SourceUniqueId => (string)base.Instance.CimInstanceProperties["SourceUniqueId"].Value;

	public string StorageSubsystemUniqueId => (string)base.Instance.CimInstanceProperties["StorageSubsystemUniqueId"].Value;

	public MSFT_StorageFaultEvent()
	{
	}

	public MSFT_StorageFaultEvent(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_StorageFaultEvent> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/windows/storage", "WQL", "SELECT * FROM MSFT_StorageFaultEvent"))
		{
			yield return new MSFT_StorageFaultEvent(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_StorageFaultEvent> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_StorageFaultEvent(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/windows/storage", "WQL", "SELECT * FROM MSFT_StorageFaultEvent").Subscribe(observer2);
	}
}

