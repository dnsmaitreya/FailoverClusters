using System;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IDisposableEx : IDisposable
{
	bool IsDisposed { get; }
}
