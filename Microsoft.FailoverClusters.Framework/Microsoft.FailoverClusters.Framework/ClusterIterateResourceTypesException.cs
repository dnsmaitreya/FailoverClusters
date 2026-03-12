using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterIterateResourceTypesException : ClusterException
{
	public ClusterIterateResourceTypesException()
		: this((Exception)null)
	{
	}

	public ClusterIterateResourceTypesException(string message)
		: base(message)
	{
	}

	public ClusterIterateResourceTypesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateResourceTypesException(Exception innerException)
		: base(ExceptionResources.ClusterIterateResourceTypes_Default, innerException)
	{
	}

	protected ClusterIterateResourceTypesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
