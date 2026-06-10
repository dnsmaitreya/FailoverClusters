using System;
using System.Collections.Generic;

namespace KDDSL.ServerClusters.Management;

internal class EnumerationResultsEventArgs : EventArgs
{
	private ICollection<ClusterListItem> items;

	private Exception error;

	public ICollection<ClusterListItem> Items => items;

	public Exception Error => error;

	private EnumerationResultsEventArgs()
	{
		items = null;
		error = null;
	}

	public EnumerationResultsEventArgs(ICollection<ClusterListItem> items)
		: this()
	{
		this.items = items;
	}

	public EnumerationResultsEventArgs(Exception error)
		: this()
	{
		this.error = error;
	}
}
