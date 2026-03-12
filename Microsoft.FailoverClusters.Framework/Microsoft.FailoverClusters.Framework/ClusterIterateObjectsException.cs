using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterIterateObjectsException : ClusterException
{
	public ClusterIterateObjectsException()
		: this((Exception)null)
	{
	}

	public ClusterIterateObjectsException(string message)
		: base(message)
	{
	}

	public ClusterIterateObjectsException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateObjectsException(Exception innerException)
		: base(ExceptionResources.ClusterIterateNetworkInterfaces_Default, innerException)
	{
	}

	protected ClusterIterateObjectsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
