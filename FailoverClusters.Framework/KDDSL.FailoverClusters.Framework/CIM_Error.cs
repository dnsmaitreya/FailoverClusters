using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class CIM_Error : MiBase
{
	public uint? CIMStatusCode
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["CIMStatusCode"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["CIMStatusCode"].Value = value;
		}
	}

	public string CIMStatusCodeDescription
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["CIMStatusCodeDescription"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["CIMStatusCodeDescription"].Value = value;
		}
	}

	public string ErrorSource
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["ErrorSource"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ErrorSource"].Value = value;
		}
	}

	public ushort? ErrorSourceFormat
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["ErrorSourceFormat"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ErrorSourceFormat"].Value = value;
		}
	}

	public ushort? ErrorType
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["ErrorType"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ErrorType"].Value = value;
		}
	}

	public string Message
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["Message"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["Message"].Value = value;
		}
	}

	public string[] MessageArguments
	{
		get
		{
			return (string[])base.Instance.CimInstanceProperties["MessageArguments"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["MessageArguments"].Value = value;
		}
	}

	public string MessageID
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["MessageID"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["MessageID"].Value = value;
		}
	}

	public string OtherErrorSourceFormat
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["OtherErrorSourceFormat"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["OtherErrorSourceFormat"].Value = value;
		}
	}

	public string OtherErrorType
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["OtherErrorType"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["OtherErrorType"].Value = value;
		}
	}

	public string OWningEntity
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["OWningEntity"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["OWningEntity"].Value = value;
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

	public ushort? ProbableCause
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["ProbableCause"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ProbableCause"].Value = value;
		}
	}

	public string ProbableCauseDescription
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["ProbableCauseDescription"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["ProbableCauseDescription"].Value = value;
		}
	}

	public string[] RecommendedActions
	{
		get
		{
			return (string[])base.Instance.CimInstanceProperties["RecommendedActions"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["RecommendedActions"].Value = value;
		}
	}

	public CIM_Error()
	{
	}

	public CIM_Error(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public static IEnumerable<CIM_Error> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM CIM_Error"))
		{
			yield return new CIM_Error(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<CIM_Error> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new CIM_Error(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM CIM_Error").Subscribe(observer2);
	}
}

