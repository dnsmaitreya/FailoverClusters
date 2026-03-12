using System;
using System.CodeDom.Compiler;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public abstract class MiBase : IDisposable
{
	private bool disposed;

	public CimSession Session { get; protected set; }

	public CimInstance Instance { get; protected set; }

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				Instance.Dispose();
			}
			disposed = true;
		}
	}
}
