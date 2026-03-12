using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterGroupMoveException : ClusterException
{
	public Guid GroupId { get; set; }

	public string GroupName { get; set; }

	public ClusterGroupMoveException()
		: this(ExceptionResources.Group_Move_Default)
	{
	}

	public ClusterGroupMoveException(Guid groupId)
		: base(ExceptionResources.Group_Move_Default)
	{
		GroupId = groupId;
	}

	public ClusterGroupMoveException(string groupName)
		: base(ExceptionResources.Group_Move_Default)
	{
		GroupName = groupName;
	}

	public ClusterGroupMoveException(Guid groupId, Exception innerException)
		: base(ExceptionResources.Group_Move_Default, innerException)
	{
		GroupId = groupId;
	}

	public ClusterGroupMoveException(string groupName, Exception innerException)
		: base(ExceptionResources.Group_Move_Default, innerException)
	{
		GroupName = groupName;
	}

	protected ClusterGroupMoveException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
