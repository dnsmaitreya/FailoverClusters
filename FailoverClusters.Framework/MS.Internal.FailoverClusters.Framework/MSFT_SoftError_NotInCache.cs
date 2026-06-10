using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_SoftError_NotInCache : MSFT_SoftError
{
	public MSFT_SoftError_NotInCache()
	{
	}

	public MSFT_SoftError_NotInCache(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_SoftError_NotInCache> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_SoftError_NotInCache"))
		{
			yield return new MSFT_SoftError_NotInCache(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_SoftError_NotInCache> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_SoftError_NotInCache(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/microsoft/windows/storage", "WQL", "SELECT * FROM MSFT_SoftError_NotInCache").Subscribe(observer2);
	}
}

