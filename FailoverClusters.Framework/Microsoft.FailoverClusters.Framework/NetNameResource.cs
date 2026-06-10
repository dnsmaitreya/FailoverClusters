using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class NetNameResource : AverageResource, ISharesContainer
{
	private string displayName;

	private string dnsName;

	public override string DisplayName => LoadAsync(displayName, 4);

	public string DnsName => LoadAsync(dnsName, 4);

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.ClientAccessPoint));

	public ReadOnlyObservableCollection<FileShare> Shares => ReadOnlyShares;

	public virtual ReadOnlyObservableCollection<object> IPAddresses
	{
		get
		{
			if (IpAddressesCollection == null)
			{
				IpAddressesCollection = new ObservableCollection<object>();
				IpAddressesCollectionReadOnly = new ReadOnlyObservableCollection<object>(IpAddressesCollection);
			}
			this.ExecuteMethod(delegate
			{
				UpdateIpAddresses(IpAddressesCollection, base.Dependencies);
			}, LockAccess.Reader);
			return IpAddressesCollectionReadOnly;
		}
	}

	private WeakLazy<ReadOnlyObservableCollection<FileShare>> ReadOnlyShares { get; set; }

	private WeakLazy<ObservableCollection<FileShare>> SharesCollection { get; set; }

	protected ObservableCollection<object> IpAddressesCollection { get; set; }

	protected ReadOnlyObservableCollection<object> IpAddressesCollectionReadOnly { get; set; }

	internal void UpdateShares()
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PNetNameResource)lockObject.Owner).UpdateShares(UpdateShareOperation.Refresh);
		}, null, LockAccess.Reader);
	}

	public void Repair()
	{
		Repair(base.SetLastError);
	}

	public void Repair(Action<OperationResult> resourceOpRepairActiveDirectoryObject)
	{
		ExecuteSafe(7, resourceOpRepairActiveDirectoryObject, delegate(ILockable resourceObject)
		{
			PNetNameResource pNetNameResource = (PNetNameResource)resourceObject.Owner;
			ClusterLog.LogInfo("Repairing the active directory object '{0}'", DisplayName);
			if (pNetNameResource.Flags.HasValue && pNetNameResource.Flags.Value.HasFlag(ResourceFlags.Core))
			{
				ClusterLog.LogInfo("Re-Acling DNS records");
				pNetNameResource.ReAclDNSRecords();
				ClusterLog.LogInfo("Resetting CNO password");
				pNetNameResource.ResetCnoPassword();
				ClusterLog.LogInfo("Enable Computer object");
				pNetNameResource.EnableADObject();
			}
			else
			{
				pNetNameResource.RepairActiveDirectoryObject();
			}
			pNetNameResource.Online();
		}, delegate(Exception exception)
		{
			ClusterException ex = exception as ClusterException;
			if (ex == null)
			{
				ex = new ClusterNetNameRepairActiveDirectoryObjectException(base.Name, exception);
			}
			ClusterLog.LogException(exception, "Error repairing the active directory object for '{0}'.", DisplayName);
			return ex;
		});
	}

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		ClusterCommand item = new ClusterCommand(this, "Repair", ClusterCommandId.NetworkNameRepairActiveDirectoryObject, ClusterCommandCollectionId.ResourceGeneral)
		{
			Text = CommandResources.RepairNetworkNameAction_Text,
			CanExecuteDelegate = (object x) => base.ResourceState != ResourceState.Online && base.ResourceState != ResourceState.Pending && base.ResourceState != ResourceState.Fetching,
			ExecuteDelegate = delegate
			{
				Repair();
			},
			CommandParameter = this
		};
		commandContainer.ChildrenInternal.Add(item);
		base.InitializeMoreActionsCommands(commandContainer);
	}

	internal NetNameResource(Cluster cluster)
		: base(cluster)
	{
		Init();
	}

	private void Init()
	{
		WeakLazy<ObservableCollection<FileShare>> sharesCollection = new WeakLazy<ObservableCollection<FileShare>>((Func<ObservableCollection<FileShare>>)delegate
		{
			ObservableCollection<FileShare> initialShares = null;
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				PNetNameResource pNetNameResource = (PNetNameResource)lockObject.Owner;
				initialShares = new ObservableCollection<FileShare>(pNetNameResource.Shares);
				pNetNameResource.UpdateShares(UpdateShareOperation.InitialQuery);
			}, LockAccess.Reader);
			if (initialShares == null)
			{
				initialShares = new ObservableCollection<FileShare>();
			}
			return initialShares;
		});
		SharesCollection = sharesCollection;
		ReadOnlyShares = new WeakLazy<ReadOnlyObservableCollection<FileShare>>(() => new ReadOnlyObservableCollection<FileShare>(sharesCollection));
	}

	internal void Restart()
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			((PNetNameResource)lockObject.Owner).UpdateShares(UpdateShareOperation.InitialQuery);
		}, LockAccess.Reader);
	}

	protected virtual void UpdateIpAddresses(ObservableCollection<object> ipAddresses, IEnumerable<Guid> dependencies)
	{
		if (dependencies == null)
		{
			return;
		}
		UIHelper.ExecuteOnDispatcher(ipAddresses.Clear, OperationType.Async);
		foreach (Guid dependency in dependencies)
		{
			Resource.Get(base.Cluster, dependency, delegate(OperationResult<Resource> result)
			{
				if (result.Result != null && (result.Result.ResourceType.ResourceKind == ResourceKind.IPAddress || result.Result.ResourceType.ResourceKind == ResourceKind.IPv6Address || result.Result.ResourceType.ResourceKind == ResourceKind.IPv6TunnelAddress))
				{
					Resource r = result.Result;
					UIHelper.ExecuteOnDispatcher(delegate
					{
						ipAddresses.Add(r);
					}, OperationType.Async);
				}
			}, OperationType.Async);
		}
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents, ignorePossibleOwners);
		ParseProperties(privateObject.Properties);
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		switch (e.EventType)
		{
		case EventType.FileShareChanged:
		{
			ObservableCollection<FileShare> strongShares = SharesCollection.TryGetInstance();
			if (strongShares == null)
			{
				return true;
			}
			ClusterFileShareEventArgs args = e.EventArgument as ClusterFileShareEventArgs;
			if (args == null || args.Error != null)
			{
				break;
			}
			UIHelper.ExecuteOnDispatcher((Action)delegate
			{
				FileShare fileShare = args.Share;
				fileShare.Cluster = base.Cluster;
				switch (args.Action)
				{
				case CollectionElementAction.Added:
					strongShares.Add(fileShare);
					break;
				case CollectionElementAction.Removed:
				{
					int num2 = strongShares.FindIndex((FileShare fs) => fs.Name == fileShare.Name);
					if (num2 > -1)
					{
						strongShares.RemoveAt(num2);
					}
					break;
				}
				case CollectionElementAction.Updated:
				{
					int num = strongShares.FindIndex((FileShare fs) => fs.Name == fileShare.Name);
					if (num > -1)
					{
						strongShares[num].CopyFrom(fileShare);
					}
					break;
				}
				}
				OnPropertyChanged("Shares");
			}, OperationType.Async, queueOnDispatcher);
			break;
		}
		case EventType.ResourceDependenciesChanged:
			if (IpAddressesCollection == null)
			{
				break;
			}
			lock (mObjectLock)
			{
				if (e.EventArgument is ClusterDependenciesEventArgs clusterDependenciesEventArgs && clusterDependenciesEventArgs.Error == null)
				{
					UpdateIpAddresses(IpAddressesCollection, clusterDependenciesEventArgs.Dependencies);
				}
			}
			break;
		case EventType.PropertiesChanged:
		{
			ClusterPropertiesEventArgs clusterPropertiesEventArgs = e.EventArgument as ClusterPropertiesEventArgs;
			if (clusterPropertiesEventArgs.Error == null && ParseProperties(clusterPropertiesEventArgs.Properties))
			{
				UIHelper.ExecuteOnDispatcher(delegate
				{
					OnPropertyChanged("DisplayName");
					OnPropertyChanged("DnsName");
				}, OperationType.Async);
			}
			break;
		}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private bool ParseProperties(ClusterPropertyCollection properties)
	{
		if (properties.PrivatePropertiesLoaded)
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)properties["DnsName"];
			if (clusterPropertyString != null && clusterPropertyString.Value != null && clusterPropertyString.TypedValue.Length > 0)
			{
				string text = CommonResources.NetNameDisplayName_Text.FormatCurrentCulture(clusterPropertyString.TypedValue);
				if (displayName != text)
				{
					dnsName = clusterPropertyString.TypedValue;
					displayName = text;
					return true;
				}
			}
			else
			{
				string text2 = (string.IsNullOrWhiteSpace(base.Name) ? CommonResources.NetNameDisplayNameNotConfigured_Text : CommonResources.NetNameDisplayName_Text.FormatCurrentCulture(base.Name));
				dnsName = string.Empty;
				if (displayName != text2)
				{
					displayName = text2;
					return true;
				}
			}
		}
		return false;
	}

	protected override void CreateDeleteDialog(Action<ConfirmationDialog> confirmationDialogCreation, bool createDialog)
	{
		base.CreateDeleteDialog(delegate(ConfirmationDialog baseConfirmation)
		{
			base.ResourceType.LoadAsync(delegate
			{
				ConfirmationDialog confirmation;
				if (baseConfirmation != null)
				{
					confirmation = baseConfirmation;
				}
				else
				{
					confirmation = new ConfirmationDialog
					{
						CustomIcon = Icon.NativeIcon,
						Caption = DialogResources.DeleteResource_Title.FormatCurrentCulture(base.ResourceType.ResourceKind.Translate()),
						Header = DialogResources.DeleteResource_Header.FormatCurrentCulture(DisplayName)
					};
				}
				base.ResourceType.Properties.Get("DeleteVcoOnResCleanup", delegate(ClusterPropertyUInt deleteVcoProperty)
				{
					confirmation.Content = confirmation.Content.AppendLine((deleteVcoProperty.TypedValue == 0) ? DialogResources.DeleteResource_ContentNetNameNotRemoveVCO : DialogResources.DeleteResource_ContentNetNameRemoveVCO);
				});
				confirmationDialogCreation(confirmation);
			}, ResourceTypeLoadSelection.PrivateProperties | ResourceTypeLoadSelection.Reload);
		}, createDialog);
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		SharesCollection.ReleaseReference();
		ReadOnlyShares.ReleaseReference();
		IpAddressesCollection = null;
		IpAddressesCollectionReadOnly = null;
		displayName = null;
		dnsName = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("Shares");
			OnPropertyChanged("IPAddresses");
			OnPropertyChanged("DisplayName");
			OnPropertyChanged("DnsName");
		}, OperationType.Async);
	}
}

