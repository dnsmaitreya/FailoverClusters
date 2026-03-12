using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterIterateNodesException : ClusterException
{
	public ClusterIterateNodesException()
		: this((Exception)null)
	{
	}

	public ClusterIterateNodesException(string message)
		: base(message)
	{
	}

	public ClusterIterateNodesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateNodesException(Exception innerException)
		: base(ExceptionResources.ClusterIterateNodes_Default, innerException)
	{
	}

	protected ClusterIterateNodesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
