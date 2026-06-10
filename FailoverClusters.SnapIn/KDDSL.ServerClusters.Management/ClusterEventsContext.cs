using System;
using System.Collections.Generic;
using FailoverClusters.UI.Common;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class ClusterEventsContext : ScopeNodeContextBase
{
	private Cluster cluster;

	private string clusterDomain;

	private ICollection<string> nodeNames;

	public string ClusterDomain => clusterDomain;

	public Cluster Cluster => cluster;

	public ICollection<string> NodeNames
	{
		get
		{
			if (nodeNames == null)
			{
				return ClusterUtilities.GetNodeNamesFromCluster(cluster);
			}
			return nodeNames;
		}
	}

	internal EventHandler<ClusterObjectEventArgs> NodeNamesChanged
	{
		get
		{
			return this.NodeNamesChangedEvent;
		}
		set
		{
			this.NodeNamesChangedEvent = value;
		}
	}

	public override ViewDescriptionCollection ViewDescriptions => new ViewDescriptionCollection { Utilities.CreateFormViewDescription(CommonResources.Nodes_Text, typeof(ClusterEventsStartPageControl)) };

	public override ActionsPaneItemCollection ActionsPaneItems => new ActionsPaneItemCollection();

	public override string HelpTopic => HelpTopics.FailoverClustersOverviewFwlink;

	private event EventHandler<ClusterObjectEventArgs> NodeNamesChangedEvent;

	internal event EventHandler ContextRefreshed;

	private ClusterEventsContext()
		: base(new Guid("{255539BA-2C34-4700-B821-9BF65E2F49C4}"), ExpandIconOptions.Show)
	{
		base.DisplayName = CommonResources.ClusterEvents_Text;
		base.ImageIndex = Icons.ClusterEventsIndex;
	}

	internal ClusterEventsContext(ClusterDatabase clusterDatabase)
		: this()
	{
		cluster = null;
		clusterDomain = null;
		nodeNames = clusterDatabase.NodeNames;
	}

	internal ClusterEventsContext(Cluster cluster)
		: this()
	{
		this.cluster = cluster;
		clusterDomain = cluster.Domain;
		this.cluster.NodesChanged += Cluster_ClusterNodesChanged;
	}

	public override void Refresh()
	{
		OnContextRefreshed();
	}

	protected override void UpdateStateBasedActions()
	{
	}

	public override void ClearActions()
	{
		base.ClearActions();
	}

	private void Cluster_ClusterNodesChanged(object sender, ClusterObjectEventArgs e)
	{
		if (this.NodeNamesChangedEvent != null)
		{
			this.NodeNamesChangedEvent(this, e);
		}
	}

	private void OnContextRefreshed()
	{
		this.ContextRefreshed?.Invoke(this, EventArgs.Empty);
	}
}

