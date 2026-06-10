using System;
using System.Collections.Generic;
using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal interface IConnectionAdapter
{
	ClusterAdapterType Adapter { get; }

	IConnectionAdapterCluster Cluster { get; }

	IConnectionAdapterGroup Group { get; }

	IConnectionAdapterResource Resource { get; }

	IConnectionAdapterNode Node { get; }

	IConnectionAdapterNetwork Network { get; }

	IConnectionAdapterNetworkInterface NetworkInterface { get; }

	IConnectionAdapterResourceType ResourceType { get; }

	IConnectionAdapterStorage Storage { get; }

	void Close();

	void Collect();

	void SaveProperties(PClusterObject clusterObject, ClusterPropertyCollection properties);

	IEnumerable<PClusterObject> Select<TInput>(IClusterList<TInput> query) where TInput : ClusterObject;

	IEnumerable<TResult> Select<TResult>(QueryInfo queryInfo) where TResult : PClusterObject;

	void EnqueueNotification(Notification notification);

	void SubscribeNotifications(Action notificationLostAction, Action<ClusterException> notificationConnectionUnrepairableAction);

	void PauseNotifications();

	void ResumeNotifications();

	void ResetNotifications();

	Notification DequeueNotification();

	Notification DequeueNotification(int millisecondsTimeout);

	void UnsubscribeNotifications();
}

