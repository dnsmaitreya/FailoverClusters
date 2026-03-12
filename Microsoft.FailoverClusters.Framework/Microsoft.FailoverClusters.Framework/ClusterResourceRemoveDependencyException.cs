using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterResourceRemoveDependencyException : ClusterException
{
	public Guid ResourceId { get; set; }

	public Guid ResourceDependOnId { get; set; }

	public ClusterResourceRemoveDependencyException()
		: this(Guid.Empty, Guid.Empty)
	{
	}

	public ClusterResourceRemoveDependencyException(string message)
		: base(message)
	{
	}

	public ClusterResourceRemoveDependencyException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterResourceRemoveDependencyException(Guid resourceId, Guid resourceDependOnId)
		: base(ExceptionResources.Resource_RemoveDependency.FormatCurrentCulture(resourceDependOnId, resourceId))
	{
		ResourceId = resourceId;
		ResourceDependOnId = resourceDependOnId;
	}

	public ClusterResourceRemoveDependencyException(Guid resourceId, Guid resourceDependOnId, Exception innerException)
		: base(ExceptionResources.Resource_RemoveDependency.FormatCurrentCulture(resourceDependOnId, resourceId), innerException)
	{
		ResourceId = resourceId;
		ResourceDependOnId = resourceDependOnId;
	}

	protected ClusterResourceRemoveDependencyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
