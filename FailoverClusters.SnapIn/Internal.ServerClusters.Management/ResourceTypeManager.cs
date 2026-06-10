using System;

namespace MS.Internal.ServerClusters.Management;

internal class ResourceTypeManager
{
	private CollectionNotificationManager<ClusterResourceType> resTypeNotifications;

	private Cluster cluster;

	private volatile object lockObject = new object();

	public EventHandler ResourceTypesChanged;

	public ResourceTypeManager(Cluster cluster)
	{
		this.cluster = cluster;
		resTypeNotifications = new CollectionNotificationManager<ClusterResourceType>((GetCollection<ClusterResourceType>)this.cluster.GetResourceTypes, (NotificationSubscription<ClusterResourceType>)ResourceTypeNotificationSubscription);
		this.cluster.ResourceTypesChanged += resTypeNotifications.OnCollectionChanged;
		resTypeNotifications.NotificationRaised += OnNotification;
		resTypeNotifications.StartNotificationMonitoring();
	}

	private void ResourceTypeNotificationSubscription(ClusterResourceType item, EventHandler notificationMethod, bool subscribe)
	{
		if (subscribe)
		{
			item.PropertiesChanged += notificationMethod;
		}
		else
		{
			item.PropertiesChanged -= notificationMethod;
		}
	}

	public void Refresh()
	{
	}

	private void OnNotification(object sender, EventArgs e)
	{
		Refresh();
		OnResourceTypesChanged();
	}

	private void OnResourceTypesChanged()
	{
		ResourceTypesChanged?.Invoke(this, EventArgs.Empty);
	}

	public virtual void Dispose()
	{
		cluster.ResourceTypesChanged -= resTypeNotifications.OnCollectionChanged;
		resTypeNotifications.Dispose();
	}
}
