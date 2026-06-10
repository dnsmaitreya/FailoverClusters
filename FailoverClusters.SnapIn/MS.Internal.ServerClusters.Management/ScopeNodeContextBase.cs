using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal abstract class ScopeNodeContextBase : ContextBase, IScopeNodeContext, IContext, IDisposable, IRefreshable
{
	private class ContextNameComparer : IEqualityComparer<IContext>
	{
		public bool Equals(IContext x, IContext y)
		{
			return x.DisplayName.Equals(y.DisplayName, StringComparison.OrdinalIgnoreCase);
		}

		public int GetHashCode(IContext obj)
		{
			return obj.DisplayName.GetHashCode();
		}
	}

	private readonly ExpandIconOptions expandIconState;

	private readonly List<IContext> contexts;

	private readonly object contextsLock = new object();

	private readonly object actionsLock = new object();

	protected volatile bool childrenEnumerated;

	protected volatile bool initialized = true;

	protected volatile bool initializing;

	protected ListViewItem listViewItem;

	public bool Initialized => initialized;

	public virtual bool Deleted => false;

	protected object ActionsLock => actionsLock;

	public virtual bool ClearActionsOnDeactivateScopeNode => true;

	public bool ChildrenPossible => expandIconState == ExpandIconOptions.Show;

	public int SelectedImageIndex => base.ImageIndex;

	protected bool AreChildrenEnumerated => childrenEnumerated;

	public abstract ViewDescriptionCollection ViewDescriptions { get; }

	public event EventHandler<ChildDeletedEventArgs> ChildDeleted;

	public event EventHandler<ChildAddedEventArgs> ChildAdded;

	public event EventHandler<ChildInsertedEventArgs> ChildInserted;

	public event EventHandler AsyncChildEnumerationComplete;

	public event EventHandler ChildInitialized;

	public event EventHandler ContextCleared;

	public virtual void Initialize()
	{
	}

	protected ScopeNodeContextBase(Guid nodeType, ExpandIconOptions expandIcon)
		: base(nodeType)
	{
		expandIconState = expandIcon;
		contexts = new List<IContext>();
		childrenEnumerated = false;
	}

	public abstract void Refresh();

	public List<IContext> GetChildContexts()
	{
		return GetChildContexts(AsyncEnumOptions.DoNotForce);
	}

	public List<IContext> GetChildContexts(AsyncEnumOptions options)
	{
		if (options == AsyncEnumOptions.Force)
		{
			childrenEnumerated = false;
		}
		if (!childrenEnumerated)
		{
			EnumerateChildrenAsync();
		}
		lock (contextsLock)
		{
			return new List<IContext>(contexts);
		}
	}

	public void EnumerateChildrenAsync()
	{
		if (childrenEnumerated)
		{
			EnumerateCollection();
			return;
		}
		childrenEnumerated = true;
		BeginChildEnumeration();
	}

	public void ExecuteUnderActionsLock(Action<ActionsPaneItemCollection> action)
	{
		lock (actionsLock)
		{
			action(ActionsPaneItems);
		}
	}

	protected virtual void OnChildInitialized()
	{
		if (this.ChildInitialized != null)
		{
			this.ChildInitialized(this, EventArgs.Empty);
		}
	}

	protected void ProcessAsyncEnumerationUpdate<T>(AsyncEnumerationUpdate<T> update, IContext childContext)
	{
		if (update.Error != null)
		{
			ClusterLog.AdminEvents.WriteDataOutOfSyncEvent(base.DisplayName, ExceptionHelp.GetExceptionMessage(update.Error));
			ExceptionHelp.LogException(update.Error, "Failed Refreshing Data for {0}", base.DisplayName);
			return;
		}
		if (childContext != null)
		{
			AddChildContext(childContext, delayedAdd: true, asyncEnumeration: true);
		}
		if (update.IsComplete)
		{
			OnAsyncChildEnumerationComplete();
		}
	}

	protected void RefreshChildren(SortOptions sort)
	{
		if (!childrenEnumerated)
		{
			return;
		}
		lock (contextsLock)
		{
			if (this is ClusterContext)
			{
				foreach (ScopeNodeContextBase context2 in contexts)
				{
					context2.Refresh();
				}
				return;
			}
			ICollection<IContext> uncachedChildren = GetUncachedChildren();
			Dictionary<IContext, IContext> dictionary = new Dictionary<IContext, IContext>(new ContextNameComparer());
			foreach (ScopeNodeContextBase item in new List<IContext>(contexts))
			{
				if (dictionary.ContainsKey(item))
				{
					RemoveChildContext(item);
					continue;
				}
				dictionary[item] = item;
				IContext context = ContextInList(item, uncachedChildren);
				if (context != null)
				{
					item.Refresh();
					context.Dispose();
				}
				else
				{
					RemoveChildContext(item);
				}
			}
			foreach (IContext item2 in uncachedChildren)
			{
				if (!item2.IsDisposed)
				{
					AddChildContext(item2, delayedAdd: true);
				}
			}
			if (sort == SortOptions.Sort)
			{
				contexts.Sort(CompareContext);
			}
		}
	}

	protected virtual ICollection<IContext> GetUncachedChildren()
	{
		return GetChildContexts(AsyncEnumOptions.DoNotForce);
	}

	private int CompareContext(IContext one, IContext two)
	{
		return string.Compare(one.DisplayName, two.DisplayName, StringComparison.CurrentCultureIgnoreCase);
	}

	private IContext ContextInList(IContext context, ICollection<IContext> contextList)
	{
		foreach (IContext context2 in contextList)
		{
			if (context.DisplayName.Equals(context2.DisplayName, StringComparison.OrdinalIgnoreCase))
			{
				return context2;
			}
		}
		return null;
	}

	private void OnChildDeleted(IContext childContext)
	{
		EventHandler<ChildDeletedEventArgs> childDeleted = this.ChildDeleted;
		if (childDeleted != null)
		{
			ChildDeletedEventArgs e = new ChildDeletedEventArgs(childContext);
			childDeleted(this, e);
		}
	}

	private void OnChildAdded(IContext childContext, bool delayedAdd, bool asyncEnumeration)
	{
		EventHandler<ChildAddedEventArgs> childAdded = this.ChildAdded;
		if (childAdded != null)
		{
			ChildAddedEventArgs e = new ChildAddedEventArgs(childContext, delayedAdd, asyncEnumeration);
			childAdded(this, e);
		}
	}

	private void OnChildAdded(List<IContext> childContexts, bool delayedAdd)
	{
		EventHandler<ChildAddedEventArgs> childAdded = this.ChildAdded;
		if (childAdded != null)
		{
			ChildAddedEventArgs e = new ChildAddedEventArgs(childContexts, delayedAdd);
			childAdded(this, e);
		}
	}

	protected virtual void OnContextCleared()
	{
		if (this.ContextCleared != null)
		{
			this.ContextCleared(this, EventArgs.Empty);
		}
	}

	protected void OnAsyncChildEnumerationComplete()
	{
		this.AsyncChildEnumerationComplete?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void BeginChildEnumeration()
	{
		EnumerateCollection();
	}

	private void EnumerateCollection()
	{
		foreach (IContext childContext in GetChildContexts())
		{
			OnChildAdded(childContext, delayedAdd: true, asyncEnumeration: true);
		}
		OnAsyncChildEnumerationComplete();
	}

	protected void ClearChildContexts()
	{
		lock (contextsLock)
		{
			foreach (IContext context in contexts)
			{
				context.Dispose();
			}
			contexts.Clear();
		}
		OnContextCleared();
	}

	protected void AddChildContext(List<IContext> childContexts, bool delayedAdd)
	{
		if (childContexts != null)
		{
			lock (contextsLock)
			{
				contexts.AddRange(childContexts);
			}
			OnChildAdded(childContexts, delayedAdd);
		}
		ChildEnumerationArgs childEnumerationArgs = new ChildEnumerationArgs(isCompleted: true);
		OnEndChildEnumeration(childEnumerationArgs);
		if (childEnumerationArgs.IsCompleted)
		{
			OnAsyncChildEnumerationComplete();
		}
	}

	protected virtual void OnEndChildEnumeration(ChildEnumerationArgs childArgs)
	{
	}

	protected void AddChildContext(IContext childContext, bool delayedAdd)
	{
		AddChildContext(childContext, delayedAdd, asyncEnumeration: false);
	}

	protected void AddChildContext(IContext childContext, bool delayedAdd, bool asyncEnumeration)
	{
		if (childContext != null)
		{
			lock (contextsLock)
			{
				contexts.Add(childContext);
			}
			OnChildAdded(childContext, delayedAdd, asyncEnumeration);
		}
	}

	protected bool RemoveChildContext(IContext childContext)
	{
		if (childContext != null)
		{
			OnChildDeleted(childContext);
			lock (contextsLock)
			{
				contexts.Remove(childContext);
			}
			childContext.Dispose();
			return true;
		}
		return false;
	}

	protected bool RemoveChildContext(Guid contextId)
	{
		IContext context = FindChildContext(contextId);
		if (context == null)
		{
			return false;
		}
		return RemoveChildContext(context);
	}

	protected bool RemoveChildContext(string displayName)
	{
		IContext context = FindChildContext(displayName);
		if (context == null)
		{
			return false;
		}
		return RemoveChildContext(context);
	}

	private void OnChildInserted(IContext childContext, int index)
	{
		EventHandler<ChildInsertedEventArgs> childInserted = this.ChildInserted;
		if (childInserted != null)
		{
			ChildInsertedEventArgs e = new ChildInsertedEventArgs(childContext, index);
			childInserted(this, e);
		}
	}

	internal IContext FindChildContext(string name)
	{
		lock (contextsLock)
		{
			foreach (IContext context in contexts)
			{
				if (string.Compare(name, context.DisplayName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return context;
				}
			}
		}
		return null;
	}

	protected IContext FindChildContext(Guid contextId)
	{
		lock (contextsLock)
		{
			foreach (IContext context in contexts)
			{
				if (context.Id == contextId)
				{
					return context;
				}
			}
		}
		return null;
	}

	protected void InsertChildContext(IContext childContext, int index)
	{
		if (childContext != null)
		{
			lock (contextsLock)
			{
				contexts.Insert(index, childContext);
			}
			OnChildInserted(childContext, index);
		}
	}

	public override void Dispose()
	{
		if (isDisposed)
		{
			return;
		}
		base.Dispose();
		lock (contextsLock)
		{
			foreach (IContext context in contexts)
			{
				context.Dispose();
			}
			contexts.Clear();
		}
		this.ChildDeleted = null;
		this.ChildAdded = null;
		this.ChildInserted = null;
		this.AsyncChildEnumerationComplete = null;
		this.ChildInitialized = null;
		listViewItem = null;
		GC.SuppressFinalize(this);
	}
}

