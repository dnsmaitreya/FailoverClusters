using System;

namespace MS.Internal.ServerClusters.Management;

internal class ResultsFetchedEventArgs : EventArgs
{
	private bool eof;

	public bool EOF => eof;

	public ResultsFetchedEventArgs(bool eof)
	{
		this.eof = eof;
	}
}
