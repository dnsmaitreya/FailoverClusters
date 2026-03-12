using System;

namespace MS.Internal.ServerClusters.Management;

internal class RefreshViewEventArgs : EventArgs
{
	private RefreshViewAction refreshViewAction;

	internal RefreshViewAction RefreshViewAction => refreshViewAction;

	internal RefreshViewEventArgs(RefreshViewAction refreshViewAction)
	{
		this.refreshViewAction = refreshViewAction;
	}
}
