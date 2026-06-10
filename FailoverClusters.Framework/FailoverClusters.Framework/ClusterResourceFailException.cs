using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterResourceFailException : ClusterException
{
	public Guid ResourceId { get; set; }

	public string ResourceName { get; set; }

	public ClusterResourceFailException()
		: this(Guid.Empty)
	{
	}

	public ClusterResourceFailException(Guid resourceId)
		: base(ExceptionResources.Resource_Fail.FormatCurrentCulture(resourceId))
	{
		ResourceId = resourceId;
	}

	public ClusterResourceFailException(string resourceName)
		: base(ExceptionResources.Resource_Fail.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
	}

	public ClusterResourceFailException(Guid resourceId, Exception innerException)
		: base(ExceptionResources.Resource_Fail.FormatCurrentCulture(resourceId), innerException)
	{
		ResourceId = resourceId;
	}

	public ClusterResourceFailException(string resourceName, Exception innerException)
		: base(ExceptionResources.Resource_Fail.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
	}

	protected ClusterResourceFailException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

