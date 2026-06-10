using System;

namespace KDDSL.ServerClusters.Management;

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
