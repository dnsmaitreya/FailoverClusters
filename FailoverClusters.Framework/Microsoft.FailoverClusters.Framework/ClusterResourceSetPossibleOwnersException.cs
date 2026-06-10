using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterResourceSetPossibleOwnersException : ClusterDialogException
{
	public string ResourceName { get; set; }

	public ClusterResourceSetPossibleOwnersException(string resourceName)
		: base(ExceptionResources.Resource_SetPossibleOwnersNodes.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
		base.Header = ExceptionResources.SaveResourcePossibleOwners_Default.FormatCurrentCulture(resourceName);
	}

	public ClusterResourceSetPossibleOwnersException(string resourceName, Exception innerException)
		: base(ExceptionResources.Resource_SetPossibleOwnersNodes.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
		base.Header = ExceptionResources.SaveResourcePossibleOwners_Default.FormatCurrentCulture(resourceName);
	}

	protected ClusterResourceSetPossibleOwnersException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			ResourceName = info.GetString("ResourceName");
		}
	}

	public ClusterResourceSetPossibleOwnersException()
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info?.AddValue("ResourceName", ResourceName);
		base.GetObjectData(info, context);
	}
}

