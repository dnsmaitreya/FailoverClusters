using System;
using System.Collections.Generic;
using FailoverClusters.UI.Common;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class FileShareWitnessResource : AverageResource
{
	private string displayName;

	private string sharePath;

	public override string DisplayName => LoadAsync(displayName, 4);

	public string SharePath => LoadAsync(sharePath, 4);

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.FileShareWitnessResource));

	internal FileShareWitnessResource(Cluster cluster)
		: base(cluster)
	{
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
						OnPropertyChanged("DisplayName");
						OnPropertyChanged("SharePath");
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
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)properties["SharePath"];
			if (clusterPropertyString != null && clusterPropertyString.Value != null && clusterPropertyString.TypedValue.Length > 0)
			{
				displayName = CommonResources.FileShareWitnessDisplayName_Text.FormatCurrentCulture(base.Name, clusterPropertyString.TypedValue);
				sharePath = clusterPropertyString.TypedValue;
			}
			else
			{
				displayName = CommonResources.FileShareWitnessUnconfiguredDisplayName_Text;
				sharePath = string.Empty;
			}
		}
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		displayName = null;
		sharePath = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("DisplayName");
			OnPropertyChanged("SharePath");
		}, OperationType.Async);
	}
}

