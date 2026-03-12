using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterNodeGetServerInformationException : ClusterException
{
	public ClusterNodeGetServerInformationException()
		: this(string.Empty)
	{
	}

	public ClusterNodeGetServerInformationException(string nodeName)
		: base(ExceptionResources.NodeGetServerInformation_Default.FormatCurrentCulture(nodeName), null)
	{
	}

	public ClusterNodeGetServerInformationException(string message, string nodeName)
		: base(message.FormatCurrentCulture(nodeName), null)
	{
	}

	public ClusterNodeGetServerInformationException(string nodeName, Exception innerException)
		: base(ExceptionResources.NodeGetServerInformation_Default.FormatCurrentCulture(nodeName), innerException)
	{
	}

	public ClusterNodeGetServerInformationException(string message, string nodeName, Exception innerException)
		: base(message.FormatCurrentCulture(nodeName), innerException)
	{
	}

	protected ClusterNodeGetServerInformationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
