using System;
using System.CodeDom.Compiler;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MiObserver : IObserver<CimSubscriptionResult>
{
	private Action<CimSubscriptionResult> next;

	private Action<Exception> error;

	private Action completed;

	public MiObserver(Action<CimSubscriptionResult> next, Action<Exception> error, Action completed)
	{
		this.next = next;
		this.error = error;
		this.completed = completed;
	}

	public void OnNext(CimSubscriptionResult result)
	{
		next(result);
	}

	public void OnError(Exception e)
	{
		error(e);
	}

	public void OnCompleted()
	{
		completed();
	}
}

