using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_ClassModification : CIM_ClassIndication
{
	public CimInstance PreviousClassDefinition
	{
		get
		{
			return (CimInstance)base.Instance.CimInstanceProperties["PreviousClassDefinition"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["PreviousClassDefinition"].Value = value;
		}
	}

	public CIM_ClassModification()
	{
	}

	public CIM_ClassModification(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<CIM_ClassModification> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM CIM_ClassModification"))
		{
			yield return new CIM_ClassModification(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<CIM_ClassModification> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new CIM_ClassModification(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM CIM_ClassModification").Subscribe(observer2);
	}
}
