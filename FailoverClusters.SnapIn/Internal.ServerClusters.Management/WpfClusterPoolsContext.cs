using System;
using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class WpfClusterPoolsContext : ScopeNodeContextBase, IClusterSpecific
{
	private readonly ClusterContext clusterContext;

	private volatile ActionsPaneItemCollection actions;

	private bool needToRecreateActions = true;

	private FailoverClusters.Framework.Cluster frameworkCluster;

	private static readonly Guid contextId = new Guid("{9f5b51cb-e176-4736-8218-c4a3e3427a7f}");

	private Action<ViewModelDataSignal> signalViewModel;

	Cluster IClusterSpecific.Cluster => clusterContext.Cluster;

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			lock (base.ActionsLock)
			{
				if (needToRecreateActions)
				{
					ClearActions();
					CreateActions();
					needToRecreateActions = false;
				}
			}
			return actions;
		}
	}

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
				DisplayName = CommonResources.Storage_Text,
				ViewType = typeof(ClusterPoolsViewAdapter),
				ControlType = typeof(WpfViewHostControl<ClusterPoolsView, ClusterPoolsViewAdapter>),
				Tag = viewModelData
			});
			viewDescriptionCollection.DefaultIndex = 0;
			return viewDescriptionCollection;
		}
	}

	private FailoverClusters.Framework.Cluster FrameworkCluster
	{
		get
		{
			if (frameworkCluster == null)
			{
				frameworkCluster = clusterContext.FrameworkCluster;
			}
			return frameworkCluster;
		}
	}

	public Cluster Cluster => clusterContext.Cluster;

	internal WpfClusterPoolsContext(ClusterContext clusterContext)
		: base(contextId, ExpandIconOptions.Show)
	{
		base.DisplayName = Resources.Pools_Text;
		base.ImageIndex = Icons.StoragePoolIndex;
		this.clusterContext = clusterContext;
	}

	public override void ClearActions()
	{
		lock (base.ActionsLock)
		{
			if (actions != null)
			{
				actions = Utilities.DisposeActions(actions);
			}
		}
		base.ClearActions();
	}

	public override void Refresh()
	{
		signalViewModel.SafeCall(ViewModelDataSignal.TargetedRefresh);
		Cluster.Refresh();
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			ClearActions();
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	protected override void UpdateStateBasedActions()
	{
	}

	private void CreateActions()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005b: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00e8: Expected O, but got Unknown
		UICommand val = new UICommand("AddPool", (UICommandId)9, (Action<object>)delegate
		{
		}, (Predicate<object>)((object x) => true));
		((ClusterCommand)val).Text = StringExtensions.ReplaceAccelerator(CommandResources.AddPoolAction_Text);
		UICommand command = val;
		UICommand val2 = new UICommand("CreatePool", (UICommandId)10, (Action<object>)delegate
		{
		}, (Predicate<object>)((object x) => true));
		((ClusterCommand)val2).Text = StringExtensions.ReplaceAccelerator(CommandResources.CreatePoolAction_Text);
		ActionsPaneItem action = new AddPoolsActionPaneItem((ICommand)command, FrameworkCluster, clusterContext).Action;
		((ActionsPaneExtendedItem)action).Description = string.Empty;
		ActionsPaneItem action2 = new CreatePoolsActionPaneItem((ICommand)val2, FrameworkCluster).Action;
		((ActionsPaneExtendedItem)action2).Description = string.Empty;
		lock (base.ActionsLock)
		{
			actions = new ActionsPaneItemCollection { action, action2 };
		}
	}
}

