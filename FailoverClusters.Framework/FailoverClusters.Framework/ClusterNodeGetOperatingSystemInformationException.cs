using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterNodeGetOperatingSystemInformationException : ClusterException
{
	public ClusterNodeGetOperatingSystemInformationException()
		: this(string.Empty)
	{
	}

	public ClusterNodeGetOperatingSystemInformationException(string nodeName)
		: base(ExceptionResources.NodeGetOperatingSystemInformation_Default.FormatCurrentCulture(nodeName), null)
	{
	}

	public ClusterNodeGetOperatingSystemInformationException(string message, string nodeName)
		: base(message.FormatCurrentCulture(nodeName), null)
	{
	}

	public ClusterNodeGetOperatingSystemInformationException(string nodeName, Exception innerException)
		: base(ExceptionResources.NodeGetOperatingSystemInformation_Default.FormatCurrentCulture(nodeName), innerException)
	{
	}

	public ClusterNodeGetOperatingSystemInformationException(string message, string nodeName, Exception innerException)
		: base(message.FormatCurrentCulture(nodeName), innerException)
	{
	}

	protected ClusterNodeGetOperatingSystemInformationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

