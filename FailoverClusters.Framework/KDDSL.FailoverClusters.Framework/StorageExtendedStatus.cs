using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageExtendedStatus : CIM_Error
{
	public MSFT_StorageExtendedStatus()
	{
	}

	public MSFT_StorageExtendedStatus(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_StorageExtendedStatus> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/windows/storage", "WQL", "SELECT * FROM MSFT_StorageExtendedStatus"))
		{
			yield return new MSFT_StorageExtendedStatus(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_StorageExtendedStatus> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_StorageExtendedStatus(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/windows/storage", "WQL", "SELECT * FROM MSFT_StorageExtendedStatus").Subscribe(observer2);
	}
}

