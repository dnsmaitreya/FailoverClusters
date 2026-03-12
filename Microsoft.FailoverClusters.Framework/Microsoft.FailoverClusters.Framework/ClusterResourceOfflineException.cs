using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterResourceOfflineException : ClusterException
{
	public Guid ResourceId { get; set; }

	public string ResourceName { get; set; }

	public ClusterResourceOfflineException()
		: this(Guid.Empty)
	{
	}

	public ClusterResourceOfflineException(Guid resourceId)
		: base(ExceptionResources.Resource_Offline.FormatCurrentCulture(resourceId))
	{
		ResourceId = resourceId;
	}

	public ClusterResourceOfflineException(string resourceName)
		: base(ExceptionResources.Resource_Offline.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
	}

	public ClusterResourceOfflineException(Guid resourceId, Exception innerException)
		: base(ExceptionResources.Resource_Offline.FormatCurrentCulture(resourceId), innerException)
	{
		ResourceId = resourceId;
	}

	public ClusterResourceOfflineException(string resourceName, Exception innerException)
		: base(ExceptionResources.Resource_Offline.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
	}

	protected ClusterResourceOfflineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
