using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterIterateGroupsException : ClusterException
{
	public ClusterIterateGroupsException()
		: this((Exception)null)
	{
	}

	public ClusterIterateGroupsException(string message)
		: base(message)
	{
	}

	public ClusterIterateGroupsException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateGroupsException(Exception innerException)
		: base(ExceptionResources.ClusterIterateGroups_Default, innerException)
	{
	}

	protected ClusterIterateGroupsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
