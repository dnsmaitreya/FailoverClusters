using System;

namespace MS.Internal.ServerClusters.Management;

internal interface ICloseable
{
	event EventHandler<ChildDeletedEventArgs> Closed;
}
