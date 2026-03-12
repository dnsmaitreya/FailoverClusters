using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterEnumerateGroupsException : ClusterException
{
	public ClusterEnumerateGroupsException()
		: this((Exception)null)
	{
	}

	public ClusterEnumerateGroupsException(string message)
		: base(message)
	{
	}

	public ClusterEnumerateGroupsException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterEnumerateGroupsException(Exception innerException)
		: base(ExceptionResources.ClusterEnumerateGroups_Default, innerException)
	{
	}

	protected ClusterEnumerateGroupsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
