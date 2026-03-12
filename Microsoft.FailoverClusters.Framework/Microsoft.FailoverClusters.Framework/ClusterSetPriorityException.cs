using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterSetPriorityException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterSetPriorityException()
		: base(ExceptionResources.ClusterSetPriority_Default)
	{
	}

	public ClusterSetPriorityException(string name)
		: this(name, null)
	{
	}

	public ClusterSetPriorityException(string name, Exception innerException)
		: base(null, innerException)
	{
		Name = name;
		base.Header = ExceptionResources.ClusterSetPriority_WithParameter.FormatCurrentCulture(name);
	}

	protected ClusterSetPriorityException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			Name = info.GetString("Name");
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info?.AddValue("Name", Name);
		base.GetObjectData(info, context);
	}
}
