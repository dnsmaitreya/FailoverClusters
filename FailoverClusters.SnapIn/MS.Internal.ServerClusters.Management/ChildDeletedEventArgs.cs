using System;

namespace MS.Internal.ServerClusters.Management;

internal class ChildDeletedEventArgs : EventArgs
{
	private string childName;

	private IContext childContext;

	public string ChildName => childName;

	public IContext ChildContext => childContext;

	public ChildDeletedEventArgs(string childName)
	{
		if (childName == null)
		{
			throw new ArgumentNullException("childName");
		}
		this.childName = childName;
	}

	public ChildDeletedEventArgs(IContext childContext)
	{
		if (childContext == null)
		{
			throw new ArgumentNullException("childContext");
		}
		this.childContext = childContext;
		childName = childContext.DisplayName;
	}
}
