using System;

namespace MS.Internal.ServerClusters.Management;

internal class ChildEnumerationArgs : EventArgs
{
	private bool isCompleted;

	public bool IsCompleted => isCompleted;

	public ChildEnumerationArgs()
	{
		isCompleted = false;
	}

	public ChildEnumerationArgs(bool isCompleted)
	{
		this.isCompleted = isCompleted;
	}
}
