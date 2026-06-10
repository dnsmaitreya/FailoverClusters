using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class WpfAllNetworksContext : ScopeNodeContextBase, IClusterSpecific, IHasPropertyPages
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

	public override List<WritableSharedDataItem> SharedData
	{
		get
		{
			List<WritableSharedDataItem> sharedData = base.SharedData;
			WritableSharedDataItem writableSharedDataItem = new WritableSharedDataItem("CLUSTER_NAME", requiresCallback: false);
			writableSharedDataItem.SetData(Encoding.Unicode.GetBytes(Cluster.ConnectedTo + "\0"));
			sharedData.Add(writableSharedDataItem);
			WritableSharedDataItem writableSharedDataItem2 = new WritableSharedDataItem("CLUSTER_LCID", requiresCallback: false);
			writableSharedDataItem2.SetData(ClusterHelp.Int32ToByteArray(CultureInfo.CurrentCulture.LCID));
			sharedData.Add(writableSharedDataItem2);
			return sharedData;
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
				DisplayName = CommonResources.Networks_Text,
				ViewType = typeof(ClusterNetworksViewAdapter),
				ControlType = typeof(WpfViewHostControl<ClusterNetworksView, ClusterNetworksViewAdapter>),
				Tag = viewModelData
			});
			viewDescriptionCollection.DefaultIndex = 0;
			return viewDescriptionCollection;
		}
	}

	public ClusterContext ClusterContext => clusterContext;

	internal WpfAllNetworksContext(ClusterContext clusterContext)
		: base(new Guid(ClusterAdministrator.NetworksContextGuid.ToString()), ExpandIconOptions.Show)
	{
		this.clusterContext = clusterContext;
		base.EnabledStandardVerbs &= ~StandardVerbs.Properties;
		base.DisplayName = CommonResources.Networks_Text;
		base.ImageIndex = Icons.NetworksIndex;
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
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[1] { ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ConfigureLiveMigrationNetworksAction_Text), CommandResources.ConfigureLiveMigrationNetworksAction_Description_Text, Icons.NetClusterIndex, OnConfigureLiveMigrationNetworks) });
		return actionsPaneItemCollection;
	}

	public override void Refresh()
	{
		signalViewModel.SafeCall(ViewModelDataSignal.TargetedRefresh);
	}

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	protected override void UpdateStateBasedActions()
	{
	}

	private void OnConfigureLiveMigrationNetworks(object sender, SnapinActionEventArgs e)
	{
		((CluAdminScopeNode)sender).ShowPropertySheet(CommandResources.LiveMigrationSettings_Text);
	}
}

