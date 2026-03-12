using System;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.SnapIn;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class WpfClusterEnclosuresContext : ScopeNodeContextBase, IClusterSpecific
{
	private readonly ClusterContext clusterContext;

	private volatile ActionsPaneItemCollection actions;

	private bool needToRecreateActions = true;

	private static readonly Guid contextId = new Guid("{1641550C-6A82-4B0D-B377-A7BCC3445609}");

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
				DisplayName = CommonResources.Enclosures_Text,
				ViewType = typeof(ClusterEnclosuresViewAdapter),
				ControlType = typeof(WpfViewHostControl<ClusterEnclosuresView, ClusterEnclosuresViewAdapter>),
				Tag = viewModelData
			});
			viewDescriptionCollection.DefaultIndex = 0;
			return viewDescriptionCollection;
		}
	}

	public Cluster Cluster => clusterContext.Cluster;

	internal WpfClusterEnclosuresContext(ClusterContext clusterContext)
		: base(contextId, ExpandIconOptions.Show)
	{
		base.DisplayName = Resources.Enclosures_Text;
		base.ImageIndex = Icons.StorageEnclosureIndex;
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
		lock (base.ActionsLock)
		{
			actions = new ActionsPaneItemCollection();
		}
	}
}
