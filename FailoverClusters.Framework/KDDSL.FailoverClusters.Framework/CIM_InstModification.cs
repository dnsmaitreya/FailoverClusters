using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_InstModification : CIM_InstIndication
{
	public CimInstance PreviousInstance
	{
		get
		{
			return (CimInstance)base.Instance.CimInstanceProperties["PreviousInstance"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["PreviousInstance"].Value = value;
		}
	}

	public CIM_InstModification()
	{
	}

	public CIM_InstModification(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<CIM_InstModification> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM CIM_InstModification"))
		{
			yield return new CIM_InstModification(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<CIM_InstModification> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new CIM_InstModification(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM CIM_InstModification").Subscribe(observer2);
	}
}

