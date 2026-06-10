using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterGroupOfflineException : ClusterException
{
	public Guid GroupId { get; set; }

	public string GroupName { get; set; }

	public ClusterGroupOfflineException()
		: this(string.Empty)
	{
	}

	public ClusterGroupOfflineException(Guid groupId)
		: base(ExceptionResources.Group_Offline.FormatCurrentCulture(groupId))
	{
		GroupId = groupId;
	}

	public ClusterGroupOfflineException(string groupName)
		: base(ExceptionResources.Group_Offline.FormatCurrentCulture(groupName))
	{
		GroupName = groupName;
	}

	public ClusterGroupOfflineException(Guid groupId, Exception innerException)
		: base(ExceptionResources.Group_Offline.FormatCurrentCulture(groupId), innerException)
	{
		GroupId = groupId;
	}

	public ClusterGroupOfflineException(string groupName, Exception innerException)
		: base(ExceptionResources.Group_Offline.FormatCurrentCulture(groupName), innerException)
	{
		GroupName = groupName;
	}

	protected ClusterGroupOfflineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

