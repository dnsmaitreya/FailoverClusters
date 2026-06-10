using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_ClassIndication : CIM_Indication
{
	public CimInstance ClassDefinition
	{
		get
		{
			return (CimInstance)base.Instance.CimInstanceProperties["ClassDefinition"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ClassDefinition"].Value = value;
		}
	}

	public CIM_ClassIndication()
	{
	}

	public CIM_ClassIndication(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<CIM_ClassIndication> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM CIM_ClassIndication"))
		{
			yield return new CIM_ClassIndication(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<CIM_ClassIndication> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new CIM_ClassIndication(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM CIM_ClassIndication").Subscribe(observer2);
	}
}

