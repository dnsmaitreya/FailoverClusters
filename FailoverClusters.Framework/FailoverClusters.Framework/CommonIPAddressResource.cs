using System;
using System.Collections.Generic;
using System.Net;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public abstract class CommonIPAddressResource : AverageResource
{
	private string address;

	private string subnet;

	private string displayName;

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.IPAddressResource));

	public override bool? IsChild => LoadAsync(isChildResource, 16);

	public override string DisplayName => LoadAsync(displayName, 4);

	public override ClusterList<Resource> Children => WeakReferenceEx.ReturnInstance(ref childrenWeak, () => new ClusterList<Resource>(base.Cluster));

	public string Address
	{
		get
		{
			address = LoadAsync(address, 4);
			if (IPAddress.TryParse(address, out var iPAddress) && iPAddress.Equals(IPAddressKind))
			{
				return CommonResources.Text_IPAddressNotConfigured;
			}
			return address;
		}
	}

	public string Subnet
	{
		get
		{
			subnet = LoadAsync(subnet, 4);
			if (IPAddress.TryParse(subnet, out var iPAddress) && iPAddress.Equals(IPAddressKind))
			{
				return CommonResources.Text_IPAddressNotConfigured;
			}
			return subnet;
		}
	}

	protected IPAddress IPAddressKind { get; private set; }

	internal CommonIPAddressResource(Cluster cluster, IPAddress ipAddressKind)
		: base(cluster)
	{
		IPAddressKind = ipAddressKind;
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents, ignorePossibleOwners);
		ParseProperties(privateObject.Properties);
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		EventType eventType = e.EventType;
		if (eventType == EventType.PropertiesChanged)
		{
			ClusterPropertiesEventArgs clusterPropertiesEventArgs = e.EventArgument as ClusterPropertiesEventArgs;
			if (clusterPropertiesEventArgs.Error == null)
			{
				ParseProperties(clusterPropertiesEventArgs.Properties);
				if (clusterPropertiesEventArgs.Properties.PrivatePropertiesLoaded)
				{
					UIHelper.ExecuteOnDispatcher(delegate
					{
						OnPropertyChanged("Address");
						OnPropertyChanged("Subnet");
						OnPropertyChanged("DisplayName");
					}, OperationType.Async);
				}
			}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private void ParseProperties(ClusterPropertyCollection properties)
	{
		if (!properties.PrivatePropertiesLoaded)
		{
			return;
		}
		ClusterPropertyString clusterPropertyString = (ClusterPropertyString)properties["Address"];
		ClusterPropertyString clusterPropertyString2 = (ClusterPropertyString)properties["SubnetMask"];
		address = ((clusterPropertyString != null && clusterPropertyString.Value != null) ? clusterPropertyString.TypedValue : string.Empty);
		subnet = ((clusterPropertyString2 != null && clusterPropertyString2.Value != null) ? clusterPropertyString2.TypedValue : string.Empty);
		if (clusterPropertyString != null && clusterPropertyString.Value != null && !clusterPropertyString.TypedValue.Equals("0.0.0.0", StringComparison.OrdinalIgnoreCase) && !clusterPropertyString.TypedValue.Equals("::", StringComparison.OrdinalIgnoreCase))
		{
			displayName = CommonResources.IPAddressDisplayName_Text.FormatCurrentCulture(clusterPropertyString.TypedValue);
			return;
		}
		string text = string.Empty;
		if (!(this is IPv6TunnelAddressResource))
		{
			ClusterPropertyString clusterPropertyString3 = (ClusterPropertyString)properties["Network"];
			if (clusterPropertyString3 != null)
			{
				text = clusterPropertyString3.TypedValue;
			}
		}
		displayName = ((text.Length == 0) ? CommonResources.IPAddressNotConfigured_Text : ((this is IPAddressResource) ? CommonResources.IPAddressNetworkDisplayName_Text.FormatCurrentCulture(text) : CommonResources.IPAddressV6NetworkDisplayName_Text.FormatCurrentCulture(text)));
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		address = null;
		subnet = null;
		displayName = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("Address");
			OnPropertyChanged("Subnet");
			OnPropertyChanged("DisplayName");
		}, OperationType.Async);
	}
}

