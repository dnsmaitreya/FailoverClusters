using System;
using System.Collections.Generic;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class DfsReplicatedFolderResource : AverageResource
{
	private string displayName;

	public override string DisplayName => LoadAsync(displayName, 4);

	public override Icon2 Icon => ReturnInstance(ref resourceIcon, () => new Icon2(InvariantResources.DfsReplicatedFolderResource));

	internal DfsReplicatedFolderResource(Cluster cluster)
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
			ClusterPropertyString clusterPropertyString = (ClusterPropertyString)properties["Replicated Folder Name"];
			ClusterPropertyString clusterPropertyString2 = (ClusterPropertyString)properties["Replicated Folder Root Path"];
			if (clusterPropertyString != null && clusterPropertyString.Value != null && clusterPropertyString2 != null && clusterPropertyString2.Value != null)
			{
				displayName = CommonResources.ReplicatedFolderDisplayName_Text.FormatCurrentCulture(clusterPropertyString.TypedValue, clusterPropertyString2.TypedValue);
			}
			else
			{
				displayName = base.Name;
			}
		}
	}

	protected override void OnRefresh(bool targeted)
	{
		base.OnRefresh(targeted);
		displayName = null;
		UIHelper.ExecuteOnDispatcher(delegate
		{
			OnPropertyChanged("DisplayName");
		}, OperationType.Async);
	}
}
