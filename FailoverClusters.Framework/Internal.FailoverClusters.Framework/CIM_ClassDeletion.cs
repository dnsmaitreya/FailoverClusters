using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_ClassDeletion : CIM_ClassIndication
{
	public CIM_ClassDeletion()
	{
	}

	public CIM_ClassDeletion(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<CIM_ClassDeletion> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM CIM_ClassDeletion"))
		{
			yield return new CIM_ClassDeletion(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<CIM_ClassDeletion> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new CIM_ClassDeletion(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM CIM_ClassDeletion").Subscribe(observer2);
	}
}

