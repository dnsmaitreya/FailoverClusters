using System;

namespace KDDSL.ServerClusters.Management;

internal interface ICloseable
{
	event EventHandler<ChildDeletedEventArgs> Closed;
}
