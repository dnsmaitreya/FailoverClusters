using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Common.Reactive.Linq;

namespace MS.Internal.FailoverClusters.Framework;

internal class PNetNameResource : PResource
{
	private readonly object lockObject = new object();

	private readonly List<IDisposable> observablesToCancel = new List<IDisposable>();

	private volatile ConcurrentDictionary<string, FileShare> shares;

	private const int PollingInterval = 5;

	private int sharesUpdating;

	private int shareVersion;

	private bool subscriptionActive;

	private DateTime refreshLockout = DateTime.MinValue;

	private Guid ownerNodeId = Guid.Empty;

	private UpdateShareOperation lastKnownUpdateOperation;

	public List<FileShare> Shares
	{
		get
		{
			if (shares != null)
			{
				lock (lockObject)
				{
					if (shares != null)
					{
						return new List<FileShare>(shares.Values);
					}
				}
			}
			return new List<FileShare>();
		}
	}

	protected PNetNameResource(PCluster cluster, Guid id, string name, PResourceType resourceType)
		: base(cluster, id, name, resourceType)
	{
	}

	public PNetNameResource(PCluster cluster, Guid id, string name)
		: this(cluster, id, name, new PResourceType(cluster, ResourceKind.NetworkName))
	{
	}

	public void UpdateShares(UpdateShareOperation updateOperation)
	{
		if (shares == null)
		{
			lock (lockObject)
			{
				if (shares == null)
				{
					shares = new ConcurrentDictionary<string, FileShare>();
				}
			}
		}
		if (lastKnownUpdateOperation == UpdateShareOperation.InitialQuery && updateOperation == UpdateShareOperation.PropertyChanged)
		{
			lastKnownUpdateOperation = updateOperation;
			return;
		}
		lastKnownUpdateOperation = updateOperation;
		if (((base.OwnerGroup == null || base.OwnerGroup.OwnerNode == null || !(base.OwnerGroup.OwnerNode.Id != ownerNodeId)) && ((subscriptionActive && updateOperation != 0) || DateTime.UtcNow < refreshLockout)) || Interlocked.Increment(ref sharesUpdating) > 1)
		{
			return;
		}
		Worker.Start(delegate
		{
			CancelObservables();
			subscriptionActive = true;
			refreshLockout = DateTime.UtcNow + TimeSpan.FromSeconds(5.0);
			if (base.OwnerGroup != null && base.OwnerGroup.OwnerNode != null)
			{
				ownerNodeId = base.OwnerGroup.OwnerNode.Id;
			}
			ConcurrentDictionary<string, FileShare> localShares = shares;
			do
			{
				Interlocked.Exchange(ref sharesUpdating, 1);
				LoadObject(5);
				base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
				shareVersion++;
				ClusterPropertyString clusterPropertyString = (ClusterPropertyString)base.Properties["Name"];
				if (clusterPropertyString == null)
				{
					break;
				}
				if (clusterPropertyString.PropertyKind == ClusterPropertyKind.Common)
				{
					clusterPropertyString = (ClusterPropertyString)base.Properties["Name_"];
					if (clusterPropertyString == null)
					{
						break;
					}
				}
				string netbiosName = clusterPropertyString.TypedValue;
				if (!string.IsNullOrWhiteSpace(netbiosName) && base.ResourceState == Microsoft.FailoverClusters.Framework.ResourceState.Online)
				{
					string nodeFqdn;
					if (base.OwnerGroup != null && base.OwnerGroup.OwnerNode != null && !string.IsNullOrWhiteSpace(base.OwnerGroup.OwnerNode.Fqdn))
					{
						nodeFqdn = base.OwnerGroup.OwnerNode.Fqdn;
					}
					else
					{
						nodeFqdn = netbiosName;
					}
					IObservable<IFileShareDataItem> source = ((base.OwnerGroup != null && (!base.OwnerGroup.GroupState.HasValue || (base.OwnerGroup.GroupState.Value != 0 && base.OwnerGroup.GroupState.Value != GroupState.PartialOnline))) ? (from s in Observable.Generate(GroupState.Unknown, (GroupState s) => true, (GroupState s) => (base.OwnerGroup == null || !base.OwnerGroup.GroupState.HasValue) ? GroupState.Unknown : base.OwnerGroup.GroupState.Value, (GroupState s) => s, (GroupState s) => TimeSpan.FromSeconds(5.0))
						where s == GroupState.Online || s == GroupState.PartialOnline
						select s).Take(1).SelectMany((GroupState x) => base.Cluster.FileServer.GetFileShareObservable(nodeFqdn, netbiosName)) : base.Cluster.FileServer.GetFileShareObservable(nodeFqdn, netbiosName));
					List<FileShareErrorItem> errorItems = new List<FileShareErrorItem>();
					IDisposable item2 = source.Where((IFileShareDataItem fs) => base.OwnerGroup == null || (base.OwnerGroup.GroupState.HasValue && (base.OwnerGroup.GroupState.Value == GroupState.Online || base.OwnerGroup.GroupState.Value == GroupState.PartialOnline))).Subscribe(delegate(IFileShareDataItem fs)
					{
						FileShare fileShare = fs as FileShare;
						FileShareErrorItem item3 = fs as FileShareErrorItem;
						if (fileShare != null)
						{
							fileShare.VcoFqdn = GetVcoFqdn();
							bool exists = false;
							localShares.AddOrUpdate(fileShare.Name, delegate
							{
								exists = false;
								fileShare.Version = shareVersion;
								return fileShare;
							}, delegate(string key, FileShare oldFileShare)
							{
								oldFileShare.Remark = fileShare.Remark;
								oldFileShare.CurrentUses = fileShare.CurrentUses;
								oldFileShare.MaxUses = fileShare.MaxUses;
								oldFileShare.Protocol = fileShare.Protocol;
								oldFileShare.ShareInfoType = fileShare.ShareInfoType;
								oldFileShare.Version = shareVersion;
								return oldFileShare;
							});
							if (!exists)
							{
								RouteEvent(new ClusterWrapperEventArgs(EventType.FileShareChanged, new ClusterFileShareEventArgs(fileShare, CollectionElementAction.Added)));
							}
						}
						else
						{
							errorItems.Add(item3);
						}
					}, ClusterLog.LogException, delegate
					{
						foreach (FileShare value2 in localShares.Values)
						{
							if (value2.Version != shareVersion)
							{
								localShares.TryRemove(value2.Name, out var _);
								RouteEvent(new ClusterWrapperEventArgs(EventType.FileShareChanged, new ClusterFileShareEventArgs(value2, CollectionElementAction.Removed)));
							}
						}
						if (!errorItems.Any((FileShareErrorItem item) => item.Protocol == FileShareProtocol.Smb))
						{
							SetupChangeNotification(nodeFqdn, netbiosName);
						}
						if (errorItems.Count > 0 && base.OwnerGroup != null)
						{
							ClusterFileShareErrorEventArgs forwardedPayload = new ClusterFileShareErrorEventArgs(errorItems);
							base.Cluster.Server.EnqueueNotification(new GroupNotification(new ClusterForwardedEventArgs(base.OwnerGroup.Id, base.ResourceType, forwardedPayload, 4)));
						}
					});
					lock (observablesToCancel)
					{
						observablesToCancel.Add(item2);
					}
				}
			}
			while (Interlocked.Decrement(ref sharesUpdating) > 0);
		});
	}

	public override void Refresh(bool targeted)
	{
		shares = null;
		base.Refresh(targeted);
	}

	internal override void OnRemovedFromCache()
	{
		CancelObservables();
	}

	protected override void OnStateChanged(object sender, ClusterResourceStateEventArgs e)
	{
		if (e.State != Microsoft.FailoverClusters.Framework.ResourceState.Online && shares != null)
		{
			shares.Clear();
		}
	}

	protected override void OnPropertiesChanged(object sender, ClusterPropertiesEventArgs e)
	{
		if ((base.LoadedSelection & 1) != 1)
		{
			LoadObject(1);
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
			});
		}
		if ((base.LoadedSelection & 2) != 2)
		{
			LoadObject(2);
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				base.Cluster.RealtimeCollections.Change(this, "LoadSelection");
			});
		}
		e.Properties.Get("ResourceSpecificStatus", delegate(ClusterPropertyString resourceSpecificStatusProperty)
		{
			ClusterInformationEventArgs payload = new ClusterInformationEventArgs(base.OwnerGroup.Id, resourceSpecificStatusProperty.TypedValue);
			base.Cluster.Server.EnqueueNotification(new GroupNotification(payload));
			base.ExecuteOnReaderCallbacks.Add(delegate
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.Information, new ClusterInformationEventArgs(base.Id, resourceSpecificStatusProperty.TypedValue)));
			});
		});
		base.OnPropertiesChanged(sender, e);
	}

	public void CancelObservables()
	{
		lock (observablesToCancel)
		{
			observablesToCancel.ForEach(delegate(IDisposable o)
			{
				o.Dispose();
			});
			observablesToCancel.Clear();
		}
	}

	private void SetupChangeNotification(string nodeFqdn, string dnsName)
	{
		IDisposable item = base.Cluster.FileServer.GetFileShareSubscriptionObservable(nodeFqdn, dnsName).Subscribe(delegate(IFileShareDataItem fsdi)
		{
			FileShare fileShare = fsdi as FileShare;
			FileShareErrorItem errorItem = fsdi as FileShareErrorItem;
			if (fileShare != null)
			{
				switch (fileShare.EventType)
				{
				case ShareEventType.Modify:
				{
					ConcurrentDictionary<string, FileShare> concurrentDictionary2 = shares;
					if (concurrentDictionary2 != null && concurrentDictionary2.ContainsKey(fileShare.Name))
					{
						concurrentDictionary2[fileShare.Name] = fileShare;
						RouteEvent(new ClusterWrapperEventArgs(EventType.FileShareChanged, new ClusterFileShareEventArgs(fileShare, CollectionElementAction.Updated)));
					}
					break;
				}
				case ShareEventType.Create:
				{
					fileShare.VcoFqdn = GetVcoFqdn();
					ConcurrentDictionary<string, FileShare> concurrentDictionary3 = shares;
					if (concurrentDictionary3 != null)
					{
						if (!concurrentDictionary3.ContainsKey(fileShare.Name))
						{
							if (concurrentDictionary3.TryAdd(fileShare.Name, fileShare))
							{
								RouteEvent(new ClusterWrapperEventArgs(EventType.FileShareChanged, new ClusterFileShareEventArgs(fileShare, CollectionElementAction.Added)));
							}
						}
						else
						{
							ClusterLog.LogInfo("Attempted to add an already existing item - updating instead. Share Name = {0}", fileShare.Name);
							RouteEvent(new ClusterWrapperEventArgs(EventType.FileShareChanged, new ClusterFileShareEventArgs(fileShare, CollectionElementAction.Updated)));
						}
					}
					break;
				}
				case ShareEventType.Delete:
				{
					ConcurrentDictionary<string, FileShare> concurrentDictionary = shares;
					if (concurrentDictionary != null)
					{
						if (shares.ContainsKey(fileShare.Name))
						{
							if (concurrentDictionary.TryRemove(fileShare.Name, out var _))
							{
								RouteEvent(new ClusterWrapperEventArgs(EventType.FileShareChanged, new ClusterFileShareEventArgs(fileShare, CollectionElementAction.Removed)));
							}
						}
						else
						{
							ClusterLog.LogInfo("Attempted to delete an already deleted item - ignoring. Share Name = {0}", fileShare.Name);
						}
					}
					break;
				}
				}
			}
			else
			{
				RouteEvent(new ClusterWrapperEventArgs(EventType.FileShareError, new ClusterFileShareErrorEventArgs(errorItem)));
			}
		});
		lock (observablesToCancel)
		{
			observablesToCancel.Add(item);
		}
	}

	public void RepairActiveDirectoryObject()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.NetworkNameRepairActiveDirectoryObject(base.Id);
			});
		}, (ClusterException ex) => ex);
	}

	public void ResetCnoPassword()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.NetworkNameResetCnoPassword(this);
			});
		}, (ClusterException ex) => ex);
	}

	public void EnableADObject()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.NetworkNameEnableAdObject(this);
			});
		}, (ClusterException ex) => ex);
	}

	public void ReAclDNSRecords()
	{
		ProtectedScope(delegate
		{
			ReleaseExecuteAndReacquire(delegate
			{
				base.Cluster.Server.Resource.NetworkNameRepairReAclDNSRecords(this);
			});
		}, (ClusterException ex) => ex);
	}

	private string GetVcoFqdn()
	{
		LoadObject(4);
		string dnsSuffix = base.Properties.Get("DnsSuffix", (ClusterPropertyString childProperty) => childProperty.TypedValue);
		return Utilities.CreateFqdn(base.Properties.Get("DnsName", (ClusterPropertyString childProperty) => childProperty.TypedValue), dnsSuffix);
	}
}
