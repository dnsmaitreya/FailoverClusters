using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ExtendedStatus : MSFT_WmiError
{
	public CimInstance original_error
	{
		get
		{
			return (CimInstance)base.Instance.CimInstanceProperties["original_error"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["original_error"].Value = value;
		}
	}

	public MSFT_ExtendedStatus()
	{
	}

	public MSFT_ExtendedStatus(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_ExtendedStatus> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM MSFT_ExtendedStatus"))
		{
			yield return new MSFT_ExtendedStatus(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_ExtendedStatus> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_ExtendedStatus(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM MSFT_ExtendedStatus").Subscribe(observer2);
	}
}

