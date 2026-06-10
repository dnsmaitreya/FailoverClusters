using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterResourceOnlineException : ClusterException
{
	public Guid ResourceId { get; set; }

	public string ResourceName { get; set; }

	public ClusterResourceOnlineException()
		: this(Guid.Empty)
	{
	}

	public ClusterResourceOnlineException(Guid resourceId)
		: base(ExceptionResources.Resource_Online.FormatCurrentCulture(resourceId))
	{
		ResourceId = resourceId;
	}

	public ClusterResourceOnlineException(string resourceName)
		: base(ExceptionResources.Resource_Online.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
	}

	public ClusterResourceOnlineException(Guid resourceId, Exception innerException)
		: base(ExceptionResources.Resource_Online.FormatCurrentCulture(resourceId), innerException)
	{
		ResourceId = resourceId;
	}

	public ClusterResourceOnlineException(string resourceName, Exception innerException)
		: base(ExceptionResources.Resource_Online.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
	}

	protected ClusterResourceOnlineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

