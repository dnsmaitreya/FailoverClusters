using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;

namespace KDDSL.ServerClusters.Management;

internal class ClusterEventsMonitor : ViewModelBase, IClusterEventsMonitor
{
	private class ClusterNodeEventsInfo
	{
		private string nodeName;

		private volatile int criticalEvents;

		private volatile int errorEvents;

		private volatile int warningEvents;

		public string NodeName => nodeName;

		public int CriticalEvents
		{
			get
			{
				return criticalEvents;
			}
			set
			{
				criticalEvents = value;
			}
		}

		public int ErrorsEvents
		{
			get
			{
				return errorEvents;
			}
			set
			{
				errorEvents = value;
			}
		}

		public int WarningEvents
		{
			get
			{
				return warningEvents;
			}
			set
			{
				warningEvents = value;
			}
		}

		public int TotalEvents => criticalEvents + errorEvents + warningEvents;

		public ClusterNodeEventsInfo(string nodeName)
		{
			if (nodeName == null || nodeName.Length == 0)
			{
				throw new ArgumentException("nodeName cannot be null or empty");
			}
			this.nodeName = nodeName;
		}

		public void ClearEvents()
		{
			criticalEvents = (errorEvents = (warningEvents = 0));
		}
	}

	private const int UpdateCriticalEventsTimerPeriod = 3600000;

	private const int LockTimeOutReader = 3000;

	private const int LockTimeOutWriter = 30000;

	private static EventLogFilter currentEventsFilter;

	private Dictionary<string, ClusterNodeEventsInfo> clusterEventsNodeInfo = new Dictionary<string, ClusterNodeEventsInfo>();

	private ReaderWriterLock clusterEventsNodeInfoLock = new ReaderWriterLock();

	private FailoverClusters.Framework.Cluster frameworkCluster;

	private string clusterDomain;

	private string clusterEventsSummary = Resources.NoRecentEvents_Text;

	private DateTime clusterEventsStartTime = DateTime.Now - new TimeSpan(24, 0, 0);

	private object pendingSubscriptionsLock = new object();

	private object clusterEventsSubscriptionsLock = new object();

	private Timer updateCriticalEventsTimer;

	private Dictionary<string, string> pendingSubscriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private List<EventLogSubscription> clusterEventsSubscriptions = new List<EventLogSubscription>();

	public static EventLogFilter CurrentEventsFilter
	{
		get
		{
			return currentEventsFilter;
		}
		set
		{
			currentEventsFilter = value;
		}
	}

	public string ClusterEventsSummary => clusterEventsSummary;

	public int ClusterEventsCount => GetClusterEventsCount(EventLevel.Critical | EventLevel.Error | EventLevel.Warning);

	public int ClusterCriticalEventsCount => GetClusterEventsCount(EventLevel.Critical);

	public int ClusterErrorsEventsCount => GetClusterEventsCount(EventLevel.Error);

	public int ClusterWarningEventsCount => GetClusterEventsCount(EventLevel.Warning);

	public ClusterEventsSummary Summary => new ClusterEventsSummary(ClusterCriticalEventsCount, ClusterErrorsEventsCount, ClusterWarningEventsCount, clusterEventsStartTime);

	public Guid ClusterCachedId { get; private set; }

	public event EventHandler<ClusterEventsSummaryEventArgs> ClusterEventsSummaryUpdated;

	public event EventHandler ClusterEventsChanged;

	public event EventHandler PendingEventSubscriptionsChanged;

	private ClusterEventsMonitor()
	{
		updateCriticalEventsTimer = new Timer(UpdateClusterEvents, null, 3600000, 3600000);
	}

	public ClusterEventsMonitor(FailoverClusters.Framework.Cluster frameworkCluster)
		: this()
	{
		this.frameworkCluster = frameworkCluster;
		clusterDomain = NetworkHelper.GetDnsSuffixFromFullyQualifiedDomainName(frameworkCluster.FullyQualifiedDomainName);
		ClusterCachedId = frameworkCluster.CacheId;
		clusterEventsStartTime = ClusterAdministrator.GetClusterEventsStartTime(frameworkCluster);
	}

	private void OnNodesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		IList list = null;
		IList list2 = null;
		if (args.Action == NotifyCollectionChangedAction.Add)
		{
			list = args.NewItems;
		}
		else if (args.Action == NotifyCollectionChangedAction.Remove)
		{
			list2 = args.OldItems;
		}
		else if (args.Action == NotifyCollectionChangedAction.Replace)
		{
			list = args.NewItems;
			list2 = args.OldItems;
		}
		else if (args.Action == NotifyCollectionChangedAction.Reset)
		{
			list2 = args.OldItems;
		}
		if (list != null)
		{
			foreach (Node item in list.OfType<Node>())
			{
				string nodeName2 = NetworkHelp.BuildFqdn(item.Name, clusterDomain);
				Worker.Start(delegate
				{
					StartClusterEventSubscriptions(nodeName2);
				});
			}
		}
		if (list2 == null)
		{
			return;
		}
		foreach (Node item2 in list2.OfType<Node>())
		{
			string nodeName = NetworkHelp.BuildFqdn(item2.Name, clusterDomain);
			LockWriter((UIThreadHandlerV)delegate
			{
				clusterEventsNodeInfo.Remove(nodeName);
			});
		}
	}

	private void Subscription_EventLogged(object sender, EventLogEventLoggedEventArgs e)
	{
		try
		{
			ProcessEventLogEntry(e.Event);
			RefreshUI();
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception in event subscription callback");
		}
	}

	private int GetClusterEventsCount(EventLevel level)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		int events = 0;
		LockReader((UIThreadHandlerV)delegate
		{
			foreach (ClusterNodeEventsInfo value in clusterEventsNodeInfo.Values)
			{
				if ((level & EventLevel.Critical) == EventLevel.Critical)
				{
					events += value.CriticalEvents;
				}
				if ((level & EventLevel.Error) == EventLevel.Error)
				{
					events += value.ErrorsEvents;
				}
				if ((level & EventLevel.Warning) == EventLevel.Warning)
				{
					events += value.WarningEvents;
				}
			}
		});
		return events;
	}

	private bool LockReader(UIThreadHandlerV function)
	{
		try
		{
			clusterEventsNodeInfoLock.AcquireReaderLock(3000);
			function.Invoke();
			return true;
		}
		catch (ApplicationException ex)
		{
			if (ex.Message.Contains("0x800705B4"))
			{
				ExceptionHelp.LogException(ex, "Can't get the reader lock of events in the cluster");
				return false;
			}
			throw ex;
		}
		finally
		{
			if (clusterEventsNodeInfoLock.IsReaderLockHeld)
			{
				clusterEventsNodeInfoLock.ReleaseReaderLock();
			}
		}
	}

	private bool LockWriter(UIThreadHandlerV function)
	{
		LockCookie lockCookie = default(LockCookie);
		bool flag = false;
		try
		{
			if (clusterEventsNodeInfoLock.IsReaderLockHeld)
			{
				lockCookie = clusterEventsNodeInfoLock.UpgradeToWriterLock(30000);
				flag = true;
			}
			else
			{
				clusterEventsNodeInfoLock.AcquireWriterLock(30000);
			}
			function.Invoke();
			return true;
		}
		catch (ApplicationException ex)
		{
			if (ex.Message.Contains("0x800705B4"))
			{
				ExceptionHelp.LogException(ex, "Can't get the writer lock of events in the cluster");
				return false;
			}
			throw ex;
		}
		finally
		{
			if (flag)
			{
				if (clusterEventsNodeInfoLock.IsWriterLockHeld)
				{
					clusterEventsNodeInfoLock.DowngradeFromWriterLock(ref lockCookie);
				}
			}
			else if (clusterEventsNodeInfoLock.IsWriterLockHeld)
			{
				clusterEventsNodeInfoLock.ReleaseWriterLock();
			}
		}
	}

	private void StartClusterEventSubscriptions()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		LockWriter((UIThreadHandlerV)delegate
		{
			clusterEventsNodeInfo.Clear();
		});
		RefreshUI();
		CloseSessions();
		frameworkCluster.AllUpNodes.ExecuteQuery(delegate(OperationResult<IClusterList<Node>> r)
		{
			if (r.Error == null)
			{
				frameworkCluster.AllUpNodes.CollectionChanged += OnNodesCollectionChanged;
				r.Result.AsParallel().ForAll(delegate(Node node)
				{
					string nodeName = NetworkHelp.BuildFqdn(node.Name, clusterDomain);
					StartClusterEventSubscriptions(nodeName);
				});
			}
			else
			{
				ClusterLog.LogVerbose((LogSubcategory)10, "Fail to get all up nodes from Framework Cluster {0}", new object[1] { frameworkCluster.DisplayName });
			}
		});
	}

	public void SetDefaultQuery()
	{
		currentEventsFilter = GetClusterEventsFilter(null);
	}

	public string PendingEventSubscriptionError(string nodeName)
	{
		lock (pendingSubscriptionsLock)
		{
			string value = null;
			if (pendingSubscriptions.TryGetValue(nodeName, out value))
			{
				return value;
			}
			return null;
		}
	}

	public void Close()
	{
		if (frameworkCluster != null)
		{
			frameworkCluster.AllUpNodes.CollectionChanged -= OnNodesCollectionChanged;
		}
		updateCriticalEventsTimer.Dispose();
		CloseSessions();
	}

	private ClusterNodeEventsInfo GetInstanceFromFQDN(string nodeName)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		ClusterNodeEventsInfo nodeEventsInfo = null;
		LockReader((UIThreadHandlerV)delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			if (!clusterEventsNodeInfo.TryGetValue(nodeName, out nodeEventsInfo))
			{
				LockWriter((UIThreadHandlerV)delegate
				{
					nodeEventsInfo = new ClusterNodeEventsInfo(nodeName);
					clusterEventsNodeInfo[nodeName] = nodeEventsInfo;
				});
			}
		});
		return nodeEventsInfo;
	}

	private void ProcessEventLogEntry(EventLogEvent eventLogEntry)
	{
		if (eventLogEntry == null || string.IsNullOrEmpty(eventLogEntry.Computer))
		{
			return;
		}
		ClusterNodeEventsInfo instanceFromFQDN = GetInstanceFromFQDN(eventLogEntry.Computer);
		switch (eventLogEntry.Level)
		{
		case 1:
			instanceFromFQDN.CriticalEvents++;
			break;
		case 2:
			instanceFromFQDN.ErrorsEvents++;
			break;
		case 3:
			instanceFromFQDN.WarningEvents++;
			break;
		case 4:
			if (eventLogEntry.EventId == 104)
			{
				instanceFromFQDN.ClearEvents();
			}
			break;
		}
	}

	private void RefreshUI()
	{
		if (!Global.DefaultDispatcher.CheckAccess())
		{
			Global.DefaultDispatcher.BeginInvoke(new Action(RefreshUI), null);
			return;
		}
		ViewModelBaseExtensions.NotifyPropertyChanged<ClusterEventsMonitor, ClusterEventsSummary>(this, (Expression<Func<ClusterEventsMonitor, ClusterEventsSummary>>)((ClusterEventsMonitor me) => me.Summary));
		UpdateClusterEventsSummary();
		OnClusterEventsChanged();
	}

	private void StartClusterEventSubscriptions(string nodeName)
	{
		try
		{
			if (!NetworkHelper.CanPing(nodeName))
			{
				return;
			}
			EventLogSubscription subscription = new EventLogSubscription();
			subscription.EventLogged += Subscription_EventLogged;
			EventLogFilter clusterEventsFilter = GetClusterEventsFilter(nodeName, addLogClearToSystemChannel: true);
			subscription.Subscribe(nodeName, clusterEventsFilter.Channels[0], clusterEventsFilter.GetQuery());
			lock (clusterEventsSubscriptionsLock)
			{
				clusterEventsSubscriptions.Add(subscription);
			}
			int startIndex = 0;
			EventLogQuerySet querySet = new EventLogQuerySet();
			int endIndex;
			querySet.ResultsFetched += delegate
			{
				endIndex = querySet.GetResultsCount();
				foreach (EventLogEvent result in querySet.GetResults(startIndex, endIndex - 1))
				{
					if (subscription.IsDisposed)
					{
						querySet.Cancel();
						break;
					}
					ProcessEventLogEntry(result);
				}
				startIndex = endIndex;
				RefreshUI();
			};
			querySet.Query(GetClusterEventsFilter(nodeName));
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Exception starting subscription for critical events on {0}", nodeName);
			bool flag = false;
			lock (pendingSubscriptionsLock)
			{
				if (!pendingSubscriptions.ContainsKey(nodeName))
				{
					pendingSubscriptions.Add(nodeName, ExceptionHelp.GetExceptionMessage(ex));
					flag = true;
				}
			}
			if (flag)
			{
				OnPendingEventSubscriptionsChanged();
			}
		}
	}

	private void CloseSessions()
	{
		CloseSession(null);
	}

	private void CloseSession(string nodeName)
	{
		lock (clusterEventsSubscriptionsLock)
		{
			for (int num = clusterEventsSubscriptions.Count - 1; num >= 0; num--)
			{
				EventLogSubscription subscription = clusterEventsSubscriptions[num];
				if (nodeName == null || string.Compare(subscription.Session.ServerName, nodeName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					clusterEventsSubscriptions.RemoveAt(num);
					Background.QueueWorker((WaitCallback)delegate
					{
						subscription.Dispose();
					});
				}
			}
		}
		lock (pendingSubscriptionsLock)
		{
			if (nodeName == null)
			{
				pendingSubscriptions.Clear();
				OnPendingEventSubscriptionsChanged();
			}
			else if (pendingSubscriptions.ContainsKey(nodeName))
			{
				pendingSubscriptions.Remove(nodeName);
				OnPendingEventSubscriptionsChanged();
			}
		}
	}

	private void UpdateClusterEventsSummary()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		ClusterEventsSummaryValueConverter val = new ClusterEventsSummaryValueConverter();
		clusterEventsSummary = (string)val.Convert((object)Summary, typeof(string), (object)null, CultureInfo.CurrentCulture);
		OnClusterEventsSummaryUpdated();
	}

	private EventLogFilter GetClusterEventsFilter(string node)
	{
		return GetClusterEventsFilter(node, addLogClearToSystemChannel: false);
	}

	private EventLogFilter GetClusterEventsFilter(string node, bool addLogClearToSystemChannel)
	{
		EventLogFilter eventLogFilter = new EventLogFilter();
		if (addLogClearToSystemChannel)
		{
			eventLogFilter.AddLogClearToSystemChannel = true;
		}
		if (node != null)
		{
			eventLogFilter.Nodes.Add(node);
		}
		else
		{
			lock (clusterEventsSubscriptionsLock)
			{
				foreach (EventLogSubscription clusterEventsSubscription in clusterEventsSubscriptions)
				{
					eventLogFilter.Nodes.Add(clusterEventsSubscription.Session.ServerName);
				}
			}
		}
		eventLogFilter.Channels.Add(EventLog.SystemChannel);
		eventLogFilter.Channels.Add(EventLog.ClusterChannelOperational);
		eventLogFilter.Channels.Add(EventLog.ClusterAwareUpdatingChannelAdmin);
		eventLogFilter.Channels.Add(EventLog.ClusterAwareUpdatingManagementChannelAdmin);
		eventLogFilter.Level = EventLevel.Critical | EventLevel.Error | EventLevel.Warning;
		eventLogFilter.From = clusterEventsStartTime.ToLocalTime();
		eventLogFilter.Providers = EventLogFilter.ProvidersForSystemChannel;
		return eventLogFilter;
	}

	public void UpdateClusterEvents(DateTime clusterRecentEventStartTime)
	{
		clusterEventsStartTime = clusterRecentEventStartTime;
		UpdateClusterEvent(clusterRecentResetEventStartTimeFromCluster: false);
	}

	public void UpdateClusterEvents()
	{
		UpdateClusterEvents(null);
	}

	private void UpdateClusterEvents(object status)
	{
		UpdateClusterEvent(clusterRecentResetEventStartTimeFromCluster: true);
	}

	private void UpdateClusterEvent(bool clusterRecentResetEventStartTimeFromCluster)
	{
		try
		{
			if (clusterRecentResetEventStartTimeFromCluster)
			{
				clusterEventsStartTime = ClusterAdministrator.GetClusterEventsStartTime(frameworkCluster);
			}
			StartClusterEventSubscriptions();
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception attempting to start a pending event subscription");
		}
	}

	protected virtual void OnPendingEventSubscriptionsChanged()
	{
		if (this.PendingEventSubscriptionsChanged != null)
		{
			this.PendingEventSubscriptionsChanged(this, EventArgs.Empty);
		}
	}

	protected virtual void OnClusterEventsSummaryUpdated()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (this.ClusterEventsSummaryUpdated != null)
		{
			this.ClusterEventsSummaryUpdated(this, new ClusterEventsSummaryEventArgs(ClusterCriticalEventsCount, ClusterErrorsEventsCount, ClusterWarningEventsCount, ClusterEventsSummary));
		}
	}

	protected virtual void OnClusterEventsChanged()
	{
		if (this.ClusterEventsChanged != null)
		{
			this.ClusterEventsChanged(this, EventArgs.Empty);
		}
	}
}

