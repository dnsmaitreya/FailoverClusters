using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterGroupSetPreferredOwnersException : ClusterDialogException
{
	public string GroupName { get; set; }

	public ClusterGroupSetPreferredOwnersException()
	{
	}

	public ClusterGroupSetPreferredOwnersException(string groupName)
		: base(ExceptionResources.Group_SetPreferredOwnersNodes.FormatCurrentCulture(groupName))
	{
		GroupName = groupName;
		base.Header = ExceptionResources.SaveDependencies_Default.FormatCurrentCulture(groupName);
	}

	public ClusterGroupSetPreferredOwnersException(string groupName, Exception innerException)
		: base(ExceptionResources.Group_SetPreferredOwnersNodes.FormatCurrentCulture(groupName), innerException)
	{
		GroupName = groupName;
	}

	protected ClusterGroupSetPreferredOwnersException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			GroupName = info.GetString("GroupName");
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		Exceptions.ThrowIfNull(info, "info");
		base.GetObjectData(info, context);
		info.AddValue("GroupName", GroupName);
	}
}
