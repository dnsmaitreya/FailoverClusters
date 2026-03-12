using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class AddShareCommand : GroupCommandBase
{
	private readonly object bulkSyncObject = new object();

	private bool isLaunchingWizard;

	public AddShareCommand(Group group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		ClusterCommandContainer addShareContainer = new ClusterCommandContainer(base.ClusterGroup, "AddShare", ClusterCommandId.FileServerGroupAddSharedFolder, ClusterCommandCollectionId.FileServerGroup)
		{
			ExecuteIfNoChildren = true,
			Text = EnumResources.GroupState_FileServer_Set_AddSharedFolder,
			Visible = false
		};
		Group ownerGroup = base.ClusterGroup;
		ClusterList<Resource> addShareResources = (ClusterList<Resource>)(from r in new ClusterList<Resource>(base.ClusterGroup.Cluster)
			{
				Name = "Add Share Command"
			}
			where r.OwnerGroup == ownerGroup && ((int)r.ResourceType.ResourceKind == 7 || (int)r.ResourceType.ResourceKind == 26 || (int)r.ResourceType.ResourceKind == 25 || (int)r.Class == 1)
			orderby r.ResourceType
			select r);
		addShareResources.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!IsAddShareCandidate(addShareResources))
			{
				addShareContainer.Visible = false;
				addShareContainer.ChildrenInternal.Clear();
				base.ClusterGroup.RefreshCommands();
			}
			else
			{
				base.ClusterGroup.Cluster.LoadAsync(addShareResources, null, delegate
				{
					lock (bulkSyncObject)
					{
						bool needToRefresh = false;
						if (!addShareContainer.Visible)
						{
							addShareContainer.Visible = true;
							needToRefresh = true;
						}
						if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
						{
							UIHelper.ExecuteOnDispatcher(delegate
							{
								if (ProcessCollectionChanged(addShareContainer, addShareResources, e.Action))
								{
									needToRefresh = true;
								}
							}, OperationType.Sync);
						}
						if (needToRefresh)
						{
							base.ClusterGroup.RefreshCommands();
						}
					}
				}, 4);
			}
		};
		addShareContainer.CanExecuteDelegate = (object x) => IsAddShareEnabled(addShareResources) && !isLaunchingWizard;
		addShareContainer.ExecuteDelegate = delegate
		{
			LaunchAddShareWizard(addShareResources.ToList().FirstOrDefault((Resource resource) => resource is NetNameResource));
		};
		addShareResources.ForceLoadStart();
		return addShareContainer;
	}

	private bool ProcessCollectionChanged(ClusterCommandContainer addShareContainer, ClusterList<Resource> addShareResources, NotifyCollectionChangedAction action)
	{
		int count = addShareContainer.ChildrenInternal.Count;
		addShareResources.ForEach(delegate(Resource resource)
		{
			if (resource.ResourceType.ResourceKind == ResourceKind.NetworkName || resource.ResourceType.ResourceKind == ResourceKind.DistributedNetworkName)
			{
				ClusterCommand clusterCommand = addShareContainer.ChildrenInternal.OfType<ClusterCommand>().FirstOrDefault((ClusterCommand _) => (ClusterObject)_.CommandParameter == resource);
				if (clusterCommand != null)
				{
					if (action == NotifyCollectionChangedAction.Remove)
					{
						addShareContainer.ChildrenInternal.Remove(clusterCommand);
					}
				}
				else
				{
					Resource networkName = resource;
					ClusterCommand childrenCommand = new ClusterCommand(base.ClusterGroup, "AddShare", ClusterCommandId.FileServerGroupAddSharedFolder, addShareContainer.Category)
					{
						Text = EnumResources.GroupState_FileServer_Set_AddSharedFolderOn.FormatCurrentCulture(networkName.Name),
						CanExecuteDelegate = (object x) => networkName.ResourceState == ResourceState.Online && !isLaunchingWizard,
						ExecuteDelegate = delegate
						{
							LaunchAddShareWizard(networkName);
						},
						CommandParameter = networkName
					};
					addShareContainer.ChildrenInternal.Add(childrenCommand);
					PropertyChangedEventHandler value = delegate(object s, PropertyChangedEventArgs e)
					{
						Resource resource2 = s as Resource;
						if (resource2 != null && e.PropertyName == "DisplayName")
						{
							childrenCommand.Text = EnumResources.GroupState_FileServer_Set_AddSharedFolderOn.FormatCurrentCulture(resource2.Name);
							base.ClusterGroup.RefreshCommands();
						}
					};
					if (action == NotifyCollectionChangedAction.Add)
					{
						resource.PropertyChanged += value;
					}
					else
					{
						resource.PropertyChanged -= value;
					}
				}
			}
		});
		if (addShareContainer.ChildrenInternal.Count == 1)
		{
			addShareContainer.ChildrenInternal.Clear();
		}
		return count != addShareContainer.ChildrenInternal.Count;
	}

	protected override void DoPostGenerateInstance(ClusterCommand newValue)
	{
		Group ownerGroup = base.ClusterGroup;
		ClusterList<Resource> obj = (ClusterList<Resource>)(from r in new ClusterList<Resource>(base.ClusterGroup.Cluster)
			{
				Name = "Add Share command Subcribe Net Names"
			}
			where r.OwnerGroup == ownerGroup && (int)r.ResourceState == 2 && ((int)r.ResourceType.ResourceKind == 7 || (int)r.ResourceType.ResourceKind == 26 || (int)r.ResourceType.ResourceKind == 25 || (int)r.Class == 1)
			orderby r.ResourceType
			select r);
		obj.CollectionChanged += delegate
		{
			TryUpdateCanExecute();
			ClusterCommandContainer clusterCommandContainer = (ClusterCommandContainer)(ClusterCommand)this;
			foreach (ClusterCommand item in clusterCommandContainer.ChildrenInternal)
			{
				item.CanExecuteUpdate(clusterCommandContainer, EventArgs.Empty);
			}
		};
		obj.ForceLoadStart();
		base.DoPostGenerateInstance(newValue);
	}

	private static bool IsAddShareEnabled(ClusterList<Resource> resources)
	{
		bool storage = false;
		bool netName = false;
		bool distributedNetName = false;
		bool distributedFileServer = false;
		return resources.Any(delegate(Resource resource)
		{
			storage |= resource.Class == ResourceClass.Storage && resource.ResourceState == ResourceState.Online;
			netName |= resource.ResourceType.ResourceKind == ResourceKind.NetworkName && resource.ResourceState == ResourceState.Online;
			distributedNetName |= resource.ResourceType.ResourceKind == ResourceKind.DistributedNetworkName && resource.ResourceState == ResourceState.Online;
			distributedFileServer |= resource.ResourceType.ResourceKind == ResourceKind.ScaleOutFileServer && resource.ResourceState == ResourceState.Online;
			return (storage && netName) || (distributedNetName && distributedFileServer);
		});
	}

	private static bool IsAddShareCandidate(ClusterList<Resource> resources)
	{
		bool storage = false;
		bool netName = false;
		bool distributedNetName = false;
		bool distributedFileServer = false;
		return resources.Any(delegate(Resource resource)
		{
			storage |= resource.Class == ResourceClass.Storage;
			netName |= resource.ResourceType.ResourceKind == ResourceKind.NetworkName;
			distributedNetName |= resource.ResourceType.ResourceKind == ResourceKind.DistributedNetworkName;
			distributedFileServer |= resource.ResourceType.ResourceKind == ResourceKind.ScaleOutFileServer;
			return (storage && netName) || (distributedNetName && distributedFileServer);
		});
	}

	private void LaunchAddShareWizard(Resource networkName)
	{
		if (networkName == null)
		{
			throw new ArgumentNullException("networkName");
		}
		NetNameResource netName = networkName as NetNameResource;
		if (netName == null)
		{
			return;
		}
		UpdateAddShareCommandEnabledState(flag: true);
		Worker.Start(delegate
		{
			netName.LoadAsync(delegate
			{
				string dnsName = netName.DnsName;
				if (string.IsNullOrWhiteSpace(dnsName) || !DnsSupport.IsNetworkNameReady(dnsName))
				{
					ClusterDialogException.ShowTaskDialogAsync(new NetworkNameNotReadyException(dnsName ?? string.Empty));
					UpdateAddShareCommandEnabledState(flag: false);
				}
				else
				{
					UIHelper.ExecuteOnDispatcher(delegate
					{
						try
						{
							ServerManagerProxy.StartShareWizard(Utilities.CreateFqdn(dnsName, netName.Properties["DnsSuffix"].Value.ToString()));
						}
						catch (ClusterDefaultException ex)
						{
							ClusterDialogException.ShowTaskDialog(ex);
						}
						finally
						{
							UpdateAddShareCommandEnabledState(flag: false);
						}
					}, OperationType.Async);
				}
			}, ResourceLoadSelection.PrivateProperties);
		});
	}

	private void UpdateAddShareCommandEnabledState(bool flag)
	{
		isLaunchingWizard = flag;
		TryUpdateCanExecute();
	}
}
