using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterGroupOnlineException : ClusterException
{
	public Guid GroupId { get; set; }

	public string GroupName { get; set; }

	public ClusterGroupOnlineException()
		: this(Guid.Empty)
	{
	}

	public ClusterGroupOnlineException(Guid groupId)
		: base(ExceptionResources.Group_Online.FormatCurrentCulture(groupId))
	{
		GroupId = groupId;
	}

	public ClusterGroupOnlineException(string groupName)
		: base(ExceptionResources.Group_Online.FormatCurrentCulture(groupName))
	{
		GroupName = groupName;
	}

	public ClusterGroupOnlineException(Guid groupId, Exception innerException)
		: base(ExceptionResources.Group_Online.FormatCurrentCulture(groupId), innerException)
	{
		GroupId = groupId;
	}

	public ClusterGroupOnlineException(string groupName, Exception innerException)
		: base(ExceptionResources.Group_Online.FormatCurrentCulture(groupName), innerException)
	{
		GroupName = groupName;
	}

	protected ClusterGroupOnlineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

