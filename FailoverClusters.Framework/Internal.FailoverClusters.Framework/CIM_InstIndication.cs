using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_InstIndication : CIM_Indication
{
	public CimInstance SourceInstance
	{
		get
		{
			return (CimInstance)base.Instance.CimInstanceProperties["SourceInstance"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SourceInstance"].Value = value;
		}
	}

	public string SourceInstanceHost
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["SourceInstanceHost"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SourceInstanceHost"].Value = value;
		}
	}

	public string SourceInstanceModelPath
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["SourceInstanceModelPath"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SourceInstanceModelPath"].Value = value;
		}
	}

	public CIM_InstIndication()
	{
	}

	public CIM_InstIndication(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<CIM_InstIndication> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM CIM_InstIndication"))
		{
			yield return new CIM_InstIndication(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<CIM_InstIndication> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new CIM_InstIndication(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM CIM_InstIndication").Subscribe(observer2);
	}
}

