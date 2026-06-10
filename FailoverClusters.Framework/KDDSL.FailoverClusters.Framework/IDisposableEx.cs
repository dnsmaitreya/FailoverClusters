using System;

namespace KDDSL.FailoverClusters.Framework;

internal interface IDisposableEx : IDisposable
{
	bool IsDisposed { get; }
}
