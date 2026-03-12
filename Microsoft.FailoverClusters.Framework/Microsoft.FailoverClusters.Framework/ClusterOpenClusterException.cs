using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterOpenClusterException : ClusterException
{
	public ClusterOpenClusterException()
		: this((Exception)null)
	{
	}

	public ClusterOpenClusterException(string message)
		: base(message)
	{
	}

	public ClusterOpenClusterException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterOpenClusterException(Exception innerException)
		: base(ExceptionResources.ClusterOpenCluster_Default, innerException)
	{
	}

	protected ClusterOpenClusterException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
