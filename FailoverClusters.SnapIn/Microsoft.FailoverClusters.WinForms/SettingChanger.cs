using System;
using System.Threading;

namespace Microsoft.FailoverClusters.WinForms;

internal class SettingChanger : IDisposable
{
	private readonly ManualResetEvent completed;

	public SettingChanger(bool initialState)
	{
		completed = new ManualResetEvent(initialState);
	}

	public void Reset()
	{
		completed.Reset();
	}

	public void Set()
	{
		completed.Set();
	}

	public void Dispose()
	{
		completed.WaitOne();
		completed.Dispose();
	}
}
