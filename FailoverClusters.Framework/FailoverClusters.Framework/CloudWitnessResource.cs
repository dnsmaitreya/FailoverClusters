using System;
using System.Collections.Generic;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class CloudWitnessResource : AverageResource
{
	private string displayName;

	private string accountName;

	public override string DisplayName => LoadAsync(displayName, 4);

	public string AccountName => LoadAsync(accountName, 4);

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.AzureCloudWitness));

	internal CloudWitnessResource(Cluster cluster)
		: base(cluster)
	{
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents, ignorePossibleOwners);
		displayName = CommonResources.CloudWitnessDisplayName_Text;
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
						OnPropertyChanged("DisplayName");
						OnPropertyChanged("AccountName");
					}, OperationType.Async);
				}
			}
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private void ParseProperties(ClusterPropertyCollection properties)
	{
		if (properties.PrivatePropertiesLoaded)
		{
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)properties["AccountName"];
			if (clusterPropertyString != null && clusterPropertyString.Value != null && clusterPropertyString.TypedValue.Length > 0)
			{
				accountName = clusterPropertyString.TypedValue;
			}
			else
			{
				accountName = string.Empty;
			}
		}
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		displayName = null;
		accountName = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("DisplayName");
			OnPropertyChanged("AccountName");
		}, OperationType.Async);
	}
}

