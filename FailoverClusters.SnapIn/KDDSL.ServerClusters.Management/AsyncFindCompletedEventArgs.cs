using System;
using System.ComponentModel;

namespace KDDSL.ServerClusters.Management;

internal class AsyncFindCompletedEventArgs : AsyncCompletedEventArgs
{
	private int itemIndex;

	public int ItemIndex => itemIndex;

	public AsyncFindCompletedEventArgs(Exception error, bool cancelled, object userState, int itemIndex)
		: base(error, cancelled, userState)
	{
		this.itemIndex = itemIndex;
	}
}
