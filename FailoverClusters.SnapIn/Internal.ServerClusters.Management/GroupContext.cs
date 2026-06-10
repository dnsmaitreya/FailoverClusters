using System;
using FailoverClusters.UI.Controls;
using ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class GroupContext : ScopeNodeContextBase, IHasPropertyPages, IClusterSpecific
{
	private ClusterGroup group;

	private ClusterContext clusterContext;

	private CollectionNotificationManager<ClusterNode> nodeNotifications;

	private ResourceTypeManager resTypeManager;

	private string groupName;

	private Guid groupId = Guid.Empty;

	public override bool Deleted
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override ViewDescriptionCollection ViewDescriptions
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public Cluster Cluster => clusterContext.Cluster;

	public override Guid Id
	{
		get
		{
			if (group == null)
			{
				return groupId;
			}
			return group.Id;
		}
	}

	public ClusterGroup Group => group;

	public PropertyPageCollection PropertyPages
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	internal GroupContext(string groupName, Guid groupId, ClusterContext clusterContext)
		: base(ClusterAdministrator.GroupContextGuid, ExpandIconOptions.Show)
	{
		this.clusterContext = clusterContext;
		this.groupName = groupName;
		this.groupId = groupId;
		base.DisplayName = groupName;
		initialized = false;
	}

	internal GroupContext(ClusterGroup group, ClusterContext clusterContext)
		: base(ClusterAdministrator.GroupContextGuid, ExpandIconOptions.Show)
	{
		this.clusterContext = clusterContext;
		this.group = group;
		if (group != null)
		{
			groupId = group.Id;
			groupName = group.Name;
		}
		initialized = false;
		Initialize();
	}

	public override void Initialize()
	{
		if (!initialized)
		{
			initializing = true;
			if (group == null)
			{
				group = Cluster.GetGroup(groupName, groupId);
			}
			base.DisplayName = group.Name;
			base.ImageIndex = IconsHelp.GetHARoleIconIndex(group);
			nodeNotifications = new CollectionNotificationManager<ClusterNode>((GetCollection<ClusterNode>)group.Cluster.GetNodes, (NotificationSubscription<ClusterNode>)Utilities.NodeStateSubscription);
			group.Cluster.NodesChanged += nodeNotifications.OnCollectionChanged;
			resTypeManager = clusterContext.ResourceTypeManager;
			ResourceTypeManager resourceTypeManager = resTypeManager;
			resourceTypeManager.ResourceTypesChanged = (EventHandler)Delegate.Combine(resourceTypeManager.ResourceTypesChanged, new EventHandler(ResourceTypes_Changed));
			nodeNotifications.StartNotificationMonitoring();
			initializing = false;
			initialized = true;
			OnChildInitialized();
		}
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			if (group != null)
			{
				group.Cluster.NodesChanged -= nodeNotifications.OnCollectionChanged;
			}
			if (nodeNotifications != null)
			{
				nodeNotifications.Dispose();
			}
			GC.SuppressFinalize(this);
		}
	}

	private void ResourceTypes_Changed(object sender, EventArgs e)
	{
		throw new NotSupportedException();
	}

	protected override void UpdateStateBasedActions()
	{
		throw new NotSupportedException();
	}

	public override void Refresh()
	{
		throw new NotSupportedException();
	}
}

