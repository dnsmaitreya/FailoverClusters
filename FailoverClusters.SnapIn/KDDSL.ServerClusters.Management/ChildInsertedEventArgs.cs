using System;

namespace KDDSL.ServerClusters.Management;

internal class ChildInsertedEventArgs : EventArgs
{
	private IContext childContext;

	private int index;

	public IContext ChildContext => childContext;

	public int Index => index;

	public ChildInsertedEventArgs(IContext childContext, int index)
	{
		if (childContext == null)
		{
			throw new ArgumentNullException("childContext");
		}
		this.childContext = childContext;
		this.index = index;
	}
}
