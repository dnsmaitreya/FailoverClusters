using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_WmiError : CIM_Error
{
	public ushort? error_Category
	{
		get
		{
			return (ushort?)base.Instance.CimInstanceProperties["error_Category"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["error_Category"].Value = value;
		}
	}

	public uint? error_Code
	{
		get
		{
			return (uint?)base.Instance.CimInstanceProperties["error_Code"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["error_Code"].Value = value;
		}
	}

	public string error_Type
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["error_Type"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["error_Type"].Value = value;
		}
	}

	public string error_WindowsErrorMessage
	{
		get
		{
			return (string)base.Instance.CimInstanceProperties["error_WindowsErrorMessage"].Value;
		}
		set
		{
			base.Instance.CimInstanceProperties["error_WindowsErrorMessage"].Value = value;
		}
	}

	public MSFT_WmiError()
	{
	}

	public MSFT_WmiError(CimSession session, CimInstance instance)
	{
		base.Session = session;
		base.Instance = instance;
	}

	public new static IEnumerable<MSFT_WmiError> Subscribe(CimSession session)
	{
		foreach (CimSubscriptionResult item in session.Subscribe("root/interop", "WQL", "SELECT * FROM MSFT_WmiError"))
		{
			yield return new MSFT_WmiError(session, item.Instance);
		}
	}

	public static IDisposable Subscribe(CimSession session, IObserver<MSFT_WmiError> observer)
	{
		MiObserver observer2 = new MiObserver(delegate(CimSubscriptionResult r)
		{
			observer.OnNext(new MSFT_WmiError(session, r.Instance));
		}, delegate(Exception e)
		{
			observer.OnError(e);
		}, delegate
		{
			observer.OnCompleted();
		});
		return session.SubscribeAsync("root/interop", "WQL", "SELECT * FROM MSFT_WmiError").Subscribe(observer2);
	}
}
