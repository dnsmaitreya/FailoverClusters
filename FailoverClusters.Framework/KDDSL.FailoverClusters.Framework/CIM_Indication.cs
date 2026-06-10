using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_Indication : MiBase
{
	public string[] CorrelatedIndications
	{
		get
		{
			return (string[])base.Instance.CimInstanceProperties["CorrelatedIndications"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["CorrelatedIndications"].Value = value;
		}
	}

	public string IndicationFilterName
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["IndicationFilterName"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IndicationFilterName"].Value = value;
		}
	}

	public string IndicationIdentifier
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["IndicationIdentifier"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IndicationIdentifier"].Value = value;
		}
	}

	public DateTime? IndicationTime
	{
		get
		{
			return (DateTime?)base.Instance.CimInstanceProperties["IndicationTime"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["IndicationTime"].Value = value;
		}
	}

	public string OtherSeverity
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["OtherSeverity"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["OtherSeverity"].Value = value;
		}
	}

	public ushort? PerceivedSeverity
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["PerceivedSeverity"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["PerceivedSeverity"].Value = value;
		}
	}

	public string SequenceContext
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["SequenceContext"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SequenceContext"].Value = value;
		}
	}

	public long? SequenceNumber
	{
		get
		{
			return (long?)base.Instance.CimInstanceProperties["SequenceNumber"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["SequenceNumber"].Value = value;
		}
	}

	public CIM_Indication()
	{
	}

	public CIM_Indication(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static IEnumerable<CIM_Indication> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM CIM_Indication"))
		{
			yield return new CIM_Indication(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<CIM_Indication> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new CIM_Indication(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM CIM_Indication").Subscribe(observer2);
	}
}

