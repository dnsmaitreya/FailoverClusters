using System;
using System.Collections.Generic;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class NotificationManager
{
	private IConnectionAdapter server;

	private readonly PCluster cluster;

	private bool subscribed;

	private readonly object subscribedLock = new object();

	public NotificationManager(PCluster cluster)
	{
		this.cluster = cluster;
	}

	public void Subscribe(IConnectionAdapter targetServer)
	{
		lock (subscribedLock)
		{
			if (subscribed)
			{
				return;
			}
			server = targetServer;
			server.SubscribeNotifications(delegate
			{
				cluster.Refresh(targeted: false);
			}, delegate(ClusterException ex)
			{
				cluster.HandleFatalError(ex);
			});
			subscribed = true;
		}
		Worker.Start(delegate
		{
			Notification notification = null;
			Thread.CurrentThread.Name = "Fx notification dispatcher - '{0}'".FormatCurrentCulture(cluster.Name);
			while (true)
			{
				try
				{
					lock (subscribedLock)
					{
						if (!subscribed)
						{
							break;
						}
						notification = server.DequeueNotification(500);
					}
					if (notification != null)
					{
						ProcessNotification(notification);
					}
				}
				catch (ClusterNotificationNotStartedException)
				{
					break;
				}
				catch (ThreadAbortException)
				{
					break;
				}
				catch (ClusterObjectLoadFailedException)
				{
				}
				catch (ClusterObjectNotFoundException)
				{
				}
				catch (ClusterObjectDeletingException)
				{
				}
				catch (ClusterResourceFailedException)
				{
				}
				catch (ClusterException ex8)
				{
					ClusterLog.AdminEvents.WriteFailedProcessNotification(ex8.ToString());
					ClusterLog.LogException(ex8, "There was an error processing a notification");
					if (cluster.DisconnectForRpcError(ex8))
					{
						break;
					}
					if (Global.ExtraExceptionData || Global.IsDebug)
					{
						ClusterDialogException.ShowTaskDialogAsync(new ClusterNotificationException("Exception processing a notification, Check the Log file for more information", ex8));
					}
				}
				catch (Exception ex9)
				{
					ClusterLog.AdminEvents.WriteFailedProcessNotification(ex9.ToString());
					ClusterLog.LogException(ex9, "There was an error processing a notification");
					if (!cluster.DisconnectForRpcError(new ClusterDefaultException(ex9)))
					{
						cluster.HandleFatalError(new ClusterNotificationException(ExceptionResources.ClusterNotificationProcessing, ex9));
					}
					break;
				}
			}
		});
	}

	public void Unsubscribe()
	{
		lock (subscribedLock)
		{
			subscribed = false;
			if (server != null)
			{
				server.UnsubscribeNotifications();
			}
		}
	}

	private void ProcessNotification<T>(Notification notification)
	{
		ClusterIdentityType identity = Cluster.IdentityFromType(typeof(T));
		ClusterLock clusterLock = cluster.CacheManager.Get(notification.Payload.Id, identity, LockAccess.UpgradableReader);
		LockAccess lockAccess = LockAccess.UpgradableReader;
		if (clusterLock == null)
		{
			return;
		}
		try
		{
			clusterLock.Writer();
			lockAccess = LockAccess.Writer;
			List<Action> list = clusterLock.Owner.ProcessNotification(notification);
			clusterLock.UnlockWriter();
			lockAccess = LockAccess.UpgradableReader;
			clusterLock.Reader();
			clusterLock.UnlockUpgradeableReader();
			lockAccess = LockAccess.Reader;
			list.ForEach(delegate(Action callback)
			{
				callback();
			});
			list.Clear();
		}
		finally
		{
			switch (lockAccess)
			{
			case LockAccess.Writer:
				clusterLock.UnlockWriter();
				clusterLock.UnlockUpgradeableReader();
				break;
			case LockAccess.UpgradableReader:
				clusterLock.UnlockUpgradeableReader();
				break;
			case LockAccess.Reader:
				clusterLock.UnlockReader();
				break;
			}
		}
	}

	private void ProcessNotification(Notification notification)
	{
		int defaultDispatcherPendingOperations = Global.DefaultDispatcherPendingOperations;
		if (defaultDispatcherPendingOperations > 100)
		{
			Thread.Sleep(defaultDispatcherPendingOperations / 10);
		}
		if (notification is ClusterNotification)
		{
			ClusterLock lockObject = cluster.LockObject;
			lockObject.Reader();
			try
			{
				lockObject.Owner.ProcessNotification(notification).ForEach(delegate(Action callback)
				{
					callback();
				});
				lockObject.Owner.ProcessNotification(notification).Clear();
				return;
			}
			finally
			{
				lockObject.UnlockReader();
			}
		}
		if (notification is GroupNotification)
		{
			if (notification.Payload is ClusterAddedEventArgs)
			{
				PGroup.ProcessNotificationSpecial(notification);
			}
			else
			{
				ProcessNotification<Group>(notification);
			}
		}
		else if (notification is ResourceNotification)
		{
			if (notification.Payload is ClusterAddedEventArgs)
			{
				PResource.ProcessNotificationSpecial(notification);
			}
			else
			{
				ProcessNotification<Resource>(notification);
			}
		}
		else if (notification is NodeNotification)
		{
			if (notification.Payload is ClusterAddedEventArgs)
			{
				PNode.ProcessNotificationSpecial(notification);
			}
			else
			{
				ProcessNotification<Node>(notification);
			}
		}
		else if (notification is ResourceTypeNotification)
		{
			if (notification.Payload is ClusterAddedEventArgs)
			{
				PResourceType.ProcessNotificationSpecial(notification);
			}
			else
			{
				ProcessNotification<ResourceType>(notification);
			}
		}
		else if (notification is NetworkNotification)
		{
			if (notification.Payload is ClusterAddedEventArgs)
			{
				PNetwork.ProcessNotificationSpecial(notification);
			}
			else
			{
				ProcessNotification<Network>(notification);
			}
		}
		else if (notification is NetworkInterfaceNotification)
		{
			if (notification.Payload is ClusterAddedEventArgs)
			{
				PNetworkInterface.ProcessNotificationSpecial(notification);
			}
			else
			{
				ProcessNotification<NetworkInterface>(notification);
			}
		}
		else
		{
			ClusterLog.LogWarning(string.Concat("Notification with type :", notification.GetType(), " was not handled"));
		}
	}
}

