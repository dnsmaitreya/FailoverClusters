using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal abstract class ContextBase : IContext, IDisposable
{
	private const int NoImageIndex = 0;

	private readonly Guid nodeType;

	private volatile StandardVerbs implementedStandardVerbs;

	private volatile StandardVerbs enabledStandardVerbs;

	private volatile string displayName = string.Empty;

	private volatile int imageIndex;

	private readonly object imageLockObject = new object();

	private readonly object displayNameLockObject = new object();

	private volatile ActionsPaneItem[] moveActions;

	protected bool moveActionsAreDirty = true;

	private volatile ActionsPaneItem[] moveVirtualMachineActions;

	protected bool moveVirtualMachineActionsAreDirty = true;

	protected bool isDisposed;

	public abstract ActionsPaneItemCollection ActionsPaneItems { get; }

	public bool IsDisposed => isDisposed;

	public virtual Guid Id => Guid.Empty;

	public StandardVerbs EnabledStandardVerbs
	{
		get
		{
			return enabledStandardVerbs;
		}
		protected set
		{
			enabledStandardVerbs = value;
		}
	}

	public string DisplayName
	{
		get
		{
			return displayName;
		}
		protected set
		{
			lock (displayNameLockObject)
			{
				if (!displayName.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					displayName = value;
					OnDisplayNameChanged();
				}
			}
		}
	}

	public virtual string HelpTopic => HelpTopics.FailoverClustersOverviewFwlink;

	public int ImageIndex
	{
		get
		{
			return imageIndex;
		}
		protected set
		{
			lock (imageLockObject)
			{
				if (value != imageIndex)
				{
					imageIndex = value;
					OnImageIndexChanged();
				}
			}
		}
	}

	public virtual List<WritableSharedDataItem> SharedData => new List<WritableSharedDataItem>();

	public Guid NodeType => nodeType;

	public virtual bool EnableRefresh => this is IRefreshable;

	public virtual bool IsDeletable => true;

	public virtual bool IsResetActionsNeeded => true;

	protected ActionsPaneItem[] MoveActions
	{
		get
		{
			if (moveActionsAreDirty)
			{
				moveActions = Utilities.DisposeActions(moveActions);
				moveActions = CreateMoveActions();
			}
			return moveActions;
		}
	}

	public event EventHandler ActionsUpdated;

	public event EventHandler DisplayNameChanged;

	public event EventHandler ImageIndexChanged;

	public event EventHandler<DeletingEventArgs> Deleting;

	protected ContextBase(Guid nodeType)
		: this()
	{
		this.nodeType = nodeType;
		DetermineImplementedStandardVerbs();
	}

	protected virtual ActionsPaneItem[] CreateMoveActions()
	{
		return null;
	}

	protected virtual ActionsPaneItem[] CreateVirtualMachineMoveActions()
	{
		return null;
	}

	public virtual void ClearActions()
	{
		moveVirtualMachineActions = Utilities.DisposeActions(moveVirtualMachineActions);
		moveVirtualMachineActionsAreDirty = true;
		moveActions = Utilities.DisposeActions(moveActions);
		moveActionsAreDirty = true;
	}

	public virtual void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			this.ActionsUpdated = null;
			this.DisplayNameChanged = null;
			this.ImageIndexChanged = null;
			this.Deleting = null;
			ClearActions();
			GC.SuppressFinalize(this);
		}
	}

	[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	internal static extern int StrCmpLogicalW(string x, string y);

	public static int Comparer(IContext x, IContext y)
	{
		if (x == null || x.DisplayName == null)
		{
			if (y != null && y.DisplayName != null)
			{
				return -1;
			}
			return 0;
		}
		if (y == null || y.DisplayName == null)
		{
			return 1;
		}
		return StrCmpLogicalW(x.DisplayName, y.DisplayName);
	}

	private void DetermineImplementedStandardVerbs()
	{
		implementedStandardVerbs = StandardVerbs.None;
		if (this is IRenameable)
		{
			implementedStandardVerbs |= StandardVerbs.Rename;
		}
		if (this is IDeleteable)
		{
			implementedStandardVerbs |= StandardVerbs.Delete;
		}
		if (this is IHasPropertyPages)
		{
			implementedStandardVerbs |= StandardVerbs.Properties;
		}
		if (EnableRefresh)
		{
			implementedStandardVerbs |= StandardVerbs.Refresh;
		}
		enabledStandardVerbs = implementedStandardVerbs;
	}

	protected abstract void UpdateStateBasedActions();

	public virtual void RefreshStateBasedActions()
	{
		Background.QueueWorker((WaitCallback)delegate
		{
			UpdateStateBasedActions();
		});
	}

	protected virtual void OnActionsUpdated()
	{
		this.ActionsUpdated?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnDisplayNameChanged()
	{
		this.DisplayNameChanged?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnImageIndexChanged()
	{
		this.ImageIndexChanged?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnDeleting(DeletingStage stage)
	{
		EventHandler<DeletingEventArgs> deleting = this.Deleting;
		if (deleting != null)
		{
			DeletingEventArgs e = new DeletingEventArgs(stage);
			deleting(this, e);
		}
	}

	protected virtual void ReportDeleteToSender(object sender)
	{
		if (sender is CluAdminScopeNode cluAdminScopeNode)
		{
			cluAdminScopeNode.Remove();
			return;
		}
		SnapInFormView obj = (SnapInFormView)sender;
		obj.RemoveResultNode(this);
		((CluAdminScopeNode)obj.ScopeNode).FindChild(DisplayName)?.Remove();
	}

	protected virtual CluAdminScopeNode GetScopeNodeFromSender(object sender)
	{
		CluAdminScopeNode cluAdminScopeNode = sender as CluAdminScopeNode;
		if (cluAdminScopeNode == null)
		{
			cluAdminScopeNode = (CluAdminScopeNode)((SnapInFormView)sender).ScopeNode;
		}
		return cluAdminScopeNode;
	}

	protected virtual void OnMove(object sender, SnapinActionEventArgs e)
	{
	}

	protected ContextBase()
	{
	}
}

