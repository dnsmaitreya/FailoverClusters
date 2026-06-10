using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class DistributedNetworkNameResource : NetNameResource
{
	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.NetNameResource));

	public override ReadOnlyObservableCollection<object> IPAddresses
	{
		get
		{
			if (base.IpAddressesCollection == null)
			{
				LoadAsync(ResourceLoadSelection.Basic);
				base.IpAddressesCollection = new ObservableCollection<object>();
				base.IpAddressesCollectionReadOnly = new ReadOnlyObservableCollection<object>(base.IpAddressesCollection);
			}
			this.ExecuteMethod(delegate(ILockable lockObject)
			{
				UpdateIpAddresses(base.IpAddressesCollection, ((PDistributedNetworkNameResource)lockObject.Owner).ClusterNetworkInfos);
			}, LockAccess.Reader);
			return base.IpAddressesCollectionReadOnly;
		}
	}

	internal DistributedNetworkNameResource(Cluster cluster)
		: base(cluster)
	{
	}

	protected override void UpdateIpAddresses(ObservableCollection<object> ipAddresses, IEnumerable<Guid> dependencies)
	{
	}

	private void UpdateIpAddresses(ObservableCollection<object> ipAddresses, IEnumerable<NetworkInfo> networkInfos)
	{
		UIHelper.ExecuteOnDispatcher(ipAddresses.Clear, OperationType.Async);
		foreach (NetworkInfo networkInfo2 in networkInfos)
		{
			NetworkInfo networkInfo = networkInfo2;
			UIHelper.ExecuteOnDispatcher(delegate
			{
				ipAddresses.Add(networkInfo);
			}, OperationType.Async);
		}
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		EventType eventType = e.EventType;
		if (eventType == EventType.NetworkInterfacesChanged && base.IpAddressesCollection != null)
		{
			lock (mObjectLock)
			{
				ObservableCollection<object> ipAddressesCollection = base.IpAddressesCollection;
				if (ipAddressesCollection == null)
				{
					base.IpAddressesCollection = null;
					base.IpAddressesCollectionReadOnly = null;
				}
				else if (e.EventArgument is ClusterNetworkChangedEventArgs clusterNetworkChangedEventArgs && clusterNetworkChangedEventArgs.Error == null)
				{
					UpdateIpAddresses(ipAddressesCollection, clusterNetworkChangedEventArgs.NetworksInfo);
				}
			}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	protected override void CreateDeleteDialog(Action<ConfirmationDialog> confirmationDialogCreation, bool createDialog)
	{
		base.CreateDeleteDialog((Action<ConfirmationDialog>)delegate(ConfirmationDialog baseConfirmation)
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
					confirmation.Content = ((deleteVcoProperty.TypedValue == 0) ? DialogResources.DeleteResource_ContentDistributedNetNameNotRemoveVCO : DialogResources.DeleteResource_ContentDistributedNetNameRemoveVCO);
				});
				confirmationDialogCreation(confirmation);
			}, ResourceTypeLoadSelection.PrivateProperties | ResourceTypeLoadSelection.Reload);
		}, createDialog);
	}
}

