using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class StorageReplicaGroup : AverageGroup
{
	public override GroupType GroupType => GroupType.StorageReplica;

	public override CommandCollection ApplicationCommands => base.ApplicationCommands;

	protected override Icon2 GenerateIcon()
	{
		return new Icon2(InvariantResources.ClusterSharedVolumeGroup);
	}

	internal StorageReplicaGroup(Cluster cluster)
		: base(cluster)
	{
	}
}

