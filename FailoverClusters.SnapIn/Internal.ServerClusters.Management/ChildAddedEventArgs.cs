using System;
using System.Collections.Generic;

namespace MS.Internal.ServerClusters.Management;

internal class ChildAddedEventArgs : EventArgs
{
	private IContext childContext;

	private List<IContext> childContexts;

	private bool delayedAdd;

	private bool asyncEnumeration;

	public IContext ChildContext => childContext;

	public List<IContext> ChildContexts => childContexts;

	public bool DelayedAdd => delayedAdd;

	public ChildAddedEventArgs(IContext childContext, bool delayedAdd, bool asyncEnumeration)
	{
		if (childContext == null)
		{
			throw new ArgumentNullException("childContext");
		}
		this.childContext = childContext;
		this.delayedAdd = delayedAdd;
		this.asyncEnumeration = asyncEnumeration;
	}

	public ChildAddedEventArgs(List<IContext> childContexts, bool delayedAdd)
	{
		if (childContexts == null)
		{
			throw new ArgumentNullException("childContexts");
		}
		this.childContexts = childContexts;
		this.delayedAdd = delayedAdd;
		asyncEnumeration = false;
	}
}
