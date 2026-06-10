using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_SoftError_RelativePath : MSFT_SoftError
{
	public MSFT_SoftError_RelativePath()
	{
	}

	public MSFT_SoftError_RelativePath(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_SoftError_RelativePath> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/windows/storage", "WQL", "SELECT * FROM MSFT_SoftError_RelativePath"))
		{
			yield return new MSFT_SoftError_RelativePath(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_SoftError_RelativePath> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_SoftError_RelativePath(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/windows/storage", "WQL", "SELECT * FROM MSFT_SoftError_RelativePath").Subscribe(observer2);
	}
}

