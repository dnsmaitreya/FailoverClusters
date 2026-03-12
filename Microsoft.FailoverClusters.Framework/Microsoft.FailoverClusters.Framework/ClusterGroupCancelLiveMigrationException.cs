using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterGroupCancelLiveMigrationException : ClusterException
{
	public Guid GroupId { get; set; }

	public string GroupName { get; set; }

	public ClusterGroupCancelLiveMigrationException()
		: this(string.Empty)
	{
	}

	public ClusterGroupCancelLiveMigrationException(Guid groupId)
		: base(ExceptionResources.VirtualMachineGroup_LiveMigrationCancel_Default.FormatCurrentCulture(groupId))
	{
		GroupId = groupId;
	}

	public ClusterGroupCancelLiveMigrationException(string groupName)
		: base(ExceptionResources.VirtualMachineGroup_LiveMigrationCancel_Default.FormatCurrentCulture(groupName))
	{
		GroupName = groupName;
	}

	public ClusterGroupCancelLiveMigrationException(Guid groupId, Exception innerException)
		: base(ExceptionResources.VirtualMachineGroup_LiveMigrationCancel_Default.FormatCurrentCulture(groupId), innerException)
	{
		GroupId = groupId;
	}

	public ClusterGroupCancelLiveMigrationException(string groupName, Exception innerException)
		: base(ExceptionResources.VirtualMachineGroup_LiveMigrationCancel_Default.FormatCurrentCulture(groupName), innerException)
	{
		GroupName = groupName;
	}

	protected ClusterGroupCancelLiveMigrationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
