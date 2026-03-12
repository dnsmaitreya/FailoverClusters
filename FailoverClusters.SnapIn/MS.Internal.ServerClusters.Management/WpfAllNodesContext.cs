using System;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class WpfAllNodesContext : ScopeNodeContextBase, IClusterSpecific
{
	private readonly ClusterContext clusterContext;

	private volatile ActionsPaneItemCollection actions;

	protected bool actionsAreDirty = true;

	private Action<ViewModelDataSignal> signalViewModel;

	public Cluster Cluster => clusterContext.Cluster;

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
					actionsAreDirty = false;
				}
			}
			return actions;
		}
	}

	public PropertyPageCollection PropertyPages => new PropertyPageCollection();

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
				DisplayName = CommonResources.Nodes_Text,
				ViewType = typeof(ClusterNodesViewAdapter),
				ControlType = typeof(WpfViewHostControl<ClusterNodesView, ClusterNodesViewAdapter>),
				Tag = viewModelData
			});
			viewDescriptionCollection.DefaultIndex = 0;
			return viewDescriptionCollection;
		}
	}

	public ClusterContext ClusterContext => clusterContext;

	internal WpfAllNodesContext(ClusterContext clusterContext)
		: base(new Guid(ClusterAdministrator.NodesContextGuid.ToString()), ExpandIconOptions.Show)
	{
		this.clusterContext = clusterContext;
		base.EnabledStandardVerbs &= ~StandardVerbs.Properties;
		base.DisplayName = CommonResources.Nodes_Text;
		base.ImageIndex = Icons.NodesIndex;
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
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[1] { SharedActions.CreateAddNodesAction(OnAddNode) });
		return actionsPaneItemCollection;
	}

	private void OnAddNode(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SharedActions.PerformAddNodesAction(clusterContext.Cluster, notifyUserFromSender);
	}

	public override void Refresh()
	{
		signalViewModel.SafeCall(ViewModelDataSignal.TargetedRefresh);
		Cluster.Refresh();
	}

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	protected override void UpdateStateBasedActions()
	{
	}
}
