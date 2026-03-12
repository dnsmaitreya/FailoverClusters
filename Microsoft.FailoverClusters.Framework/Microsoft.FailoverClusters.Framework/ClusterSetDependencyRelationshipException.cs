using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterSetDependencyRelationshipException : ClusterDialogException
{
	public Guid ResourceId { get; set; }

	public string ResourceName { get; set; }

	public ClusterSetDependencyRelationshipException()
	{
	}

	public ClusterSetDependencyRelationshipException(Guid resourceId)
		: base(ExceptionResources.SavingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceId))
	{
		ResourceId = resourceId;
		base.Header = ExceptionResources.SaveDependencies_Default.FormatCurrentCulture(resourceId.ToString());
	}

	public ClusterSetDependencyRelationshipException(string resourceName)
		: base(ExceptionResources.SavingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
		base.Header = ExceptionResources.SaveDependencies_Default.FormatCurrentCulture(resourceName);
	}

	public ClusterSetDependencyRelationshipException(Guid resourceId, Exception innerException)
		: base(ExceptionResources.SavingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceId), innerException)
	{
		ResourceId = resourceId;
		base.Header = ExceptionResources.SaveDependencies_Default.FormatCurrentCulture(resourceId.ToString());
	}

	public ClusterSetDependencyRelationshipException(string resourceName, Exception innerException)
		: base(ExceptionResources.SavingResourceDependencyRelationship_Default.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
		base.Header = ExceptionResources.SaveDependencies_Default.FormatCurrentCulture(resourceName);
	}

	protected ClusterSetDependencyRelationshipException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		ResourceId = (Guid)info.GetValue("ResourceId", typeof(Guid));
		ResourceName = info.GetString("ResourceName");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		if (info != null)
		{
			info.AddValue("ResourceId", ResourceId);
			info.AddValue("ResourceName", ResourceName);
		}
	}
}
