using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterConnectionInformationException : ClusterException
{
	public ClusterConnectionInformationException()
		: this((Exception)null)
	{
	}

	public ClusterConnectionInformationException(string message)
		: base(message)
	{
	}

	public ClusterConnectionInformationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterConnectionInformationException(Exception innerException)
		: base(ExceptionResources.FailedToGetClusterConnectionInformation_Default, innerException)
	{
	}

	protected ClusterConnectionInformationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
