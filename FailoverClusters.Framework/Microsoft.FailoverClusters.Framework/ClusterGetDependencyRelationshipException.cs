using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterGetDependencyRelationshipException : ClusterException
{
	public Guid ResourceId { get; set; }

	public string ResourceName { get; set; }

	public ClusterGetDependencyRelationshipException()
		: this(string.Empty)
	{
	}

	public ClusterGetDependencyRelationshipException(Guid resourceId)
		: base(ExceptionResources.LoadingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceId))
	{
		ResourceId = resourceId;
	}

	public ClusterGetDependencyRelationshipException(string resourceName)
		: base(ExceptionResources.LoadingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
	}

	public ClusterGetDependencyRelationshipException(Guid resourceId, Exception innerException)
		: base(ExceptionResources.LoadingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceId), innerException)
	{
		ResourceId = resourceId;
	}

	public ClusterGetDependencyRelationshipException(string resourceName, Exception innerException)
		: base(ExceptionResources.LoadingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
	}

	protected ClusterGetDependencyRelationshipException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

