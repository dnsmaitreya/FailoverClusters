using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterNodeGetProcessorInformationException : ClusterException
{
	public ClusterNodeGetProcessorInformationException()
		: this(string.Empty)
	{
	}

	public ClusterNodeGetProcessorInformationException(string nodeName)
		: base(ExceptionResources.NodeGetProcessorInformation_Default.FormatCurrentCulture(nodeName), null)
	{
	}

	public ClusterNodeGetProcessorInformationException(string message, string nodeName)
		: base(message.FormatCurrentCulture(nodeName), null)
	{
	}

	public ClusterNodeGetProcessorInformationException(string nodeName, Exception innerException)
		: base(ExceptionResources.NodeGetProcessorInformation_Default.FormatCurrentCulture(nodeName), innerException)
	{
	}

	public ClusterNodeGetProcessorInformationException(string message, string nodeName, Exception innerException)
		: base(message.FormatCurrentCulture(nodeName), innerException)
	{
	}

	protected ClusterNodeGetProcessorInformationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
