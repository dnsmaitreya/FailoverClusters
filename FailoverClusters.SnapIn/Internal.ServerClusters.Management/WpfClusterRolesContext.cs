using System;
using System.Collections.Generic;
using System.Linq;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class WpfClusterRolesContext : ScopeNodeContextBase, IClusterSpecific, IViewContext
{
	private readonly ClusterContext clusterContext;

	private Action<ViewModelDataSignal> signalViewModel;

	private volatile ActionsPaneItemCollection actions;

	private bool actionsAreDirty = true;

	public Cluster Cluster => clusterContext.Cluster;

	public override ViewDescriptionCollection ViewDescriptions
	{
		get
		{
			ViewDescriptionCollection viewDescriptionCollection = new ViewDescriptionCollection();
			ViewModelData viewModelData = new ViewModelData(clusterContext.Cluster.Id)
			{
				HelpTopic = HelpTopic
			};
			signalViewModel = viewModelData.ProcessMessage;
			viewDescriptionCollection.Add(new FormViewDescription
			{
				DisplayName = CommonResources.ServicesAndApplicationsTitle_Text,
				ViewType = typeof(ClusterRolesViewAdapter),
				ControlType = typeof(WpfViewHostControl<ClusterRolesView, ClusterRolesViewAdapter>),
				Tag = viewModelData
			});
			viewDescriptionCollection.DefaultIndex = 0;
			return viewDescriptionCollection;
		}
	}

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			if (actionsAreDirty)
			{
				lock (base.ActionsLock)
				{
					actions = Utilities.DisposeActions(actions);
					actions = CreateActions();
				}
				actionsAreDirty = false;
			}
			return actions;
		}
	}

	public ClusterContext ClusterContext => clusterContext;

	public string[] DisplayColumns => new string[5]
	{
		Resources.Name_Text,
		Resources.Status_Text,
		Resources.GroupType_Text,
		Resources.CurrentOwner_Text,
		Resources.AutoStartColumnHeader
	};

	public string EmptyMessage => Resources.List_NoGroups_Text;

	public bool IsEnumerating { get; private set; }

	internal WpfClusterRolesContext(ClusterContext clusterContext)
		: base(new Guid("{CDE92354-3FE6-43ae-AD99-723A6F5C457A}"), ExpandIconOptions.Show)
	{
		this.clusterContext = clusterContext;
		base.DisplayName = CommonResources.ServicesAndApplicationsTitle_Text;
		base.ImageIndex = Icons.GroupIndex;
		this.clusterContext.Cluster.GroupsChanged += OnGroupsChanged;
	}

	private void OnGroupsChanged(object sender, ClusterObjectEventArgs e)
	{
		if (e.EventType == ClusterObjectEventType.Added)
		{
			try
			{
				ClusterGroup group = clusterContext.Cluster.GetGroup(e.ClusterObject);
				GroupContext groupContext = CreateGroupContext(group);
				if (groupContext != null)
				{
					AddChildContext(groupContext, delayedAdd: true);
				}
				return;
			}
			catch (ApplicationException ex)
			{
				ClusterLog.LogException((Exception)ex, Extensions.FormatCurrentCulture("Cluster group '{0}' not found in the cluster", (object)e.ClusterObject));
				return;
			}
		}
		if (e.EventType == ClusterObjectEventType.Deleted && (e.ClusterObjectId == Guid.Empty || !RemoveChildContext(e.ClusterObjectId)))
		{
			RemoveChildContext(e.ClusterObject);
		}
	}

	private GroupContext CreateGroupContext(ClusterGroup group)
	{
		if (ClusterUtilities.IsHiddenGroup(group))
		{
			return null;
		}
		return ContextFactory.CreateContext(group, clusterContext);
	}

	public override void Refresh()
	{
		signalViewModel.SafeCall(ViewModelDataSignal.TargetedRefresh);
		Cluster.Refresh();
	}

	public override void Dispose()
	{
		clusterContext.Cluster.GroupsChanged -= OnGroupsChanged;
		GC.SuppressFinalize(this);
	}

	protected override void BeginChildEnumeration()
	{
		IsEnumerating = true;
		List<IContext> list = (from idName in Cluster.GetCurrentGroupNames()
			select new GroupContext(idName.Value, idName.Key, clusterContext)).Cast<IContext>().ToList();
		ClearChildContexts();
		list.Sort(ContextBase.Comparer);
		IsEnumerating = false;
		AddChildContext(list, delayedAdd: true);
	}

	protected override ICollection<IContext> GetUncachedChildren()
	{
		List<IContext> list = new List<IContext>();
		foreach (ClusterGroup group in clusterContext.Cluster.GetGroups())
		{
			IContext context = CreateGroupContext(group);
			if (context != null)
			{
				list.Add(context);
			}
		}
		return list;
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			actions = Utilities.DisposeActions(actions);
		}
		actionsAreDirty = true;
		base.ClearActions();
	}

	private ActionsPaneItemCollection CreateActions()
	{
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[4]
		{
			SharedActions.CreateNewServiceOrApplicationAction(OnMakeApplicationOrServiceHighlyAvailable),
			new ActionSeparator(),
			SharedActions.CreateNewVirtualMachineActions(Cluster, clusterContext.FrameworkCluster),
			new ActionSeparator()
		});
		actionsPaneItemCollection.Add(ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.Cluster_AddGroup), Resources.NewGroupActionDescription_Text, Icons.NewGroupIndex, OnNewGroup));
		return actionsPaneItemCollection;
	}

	private void OnNewGroup(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.GeneratingGroupName_Text);
		using (cluadminWaitDialog)
		{
			cluadminWaitDialog.ShowDialog(notifyUserFromSender, CreateEmptyGroup);
		}
	}

	private void CreateEmptyGroup(CluadminWaitDialog waitDialog)
	{
		string text = GroupHelp.GenerateGroupName(clusterContext.Cluster, Resources.DefaultNewGroupName_Text);
		waitDialog.ThrowIfCanceled();
		waitDialog.SetStatusText(Resources.CreatingGroup_Text, text);
		clusterContext.Cluster.CreateGroup(text, GroupType.Unknown);
	}

	private void OnMakeApplicationOrServiceHighlyAvailable(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformNewServiceOrApplicationAction(clusterContext.Cluster, notifyUserFromSender);
	}

	protected override void UpdateStateBasedActions()
	{
	}
}

