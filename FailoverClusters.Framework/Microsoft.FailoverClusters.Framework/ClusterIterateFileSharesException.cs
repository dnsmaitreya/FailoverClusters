using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterIterateFileSharesException : ClusterException
{
	public ClusterIterateFileSharesException()
		: this((Exception)null)
	{
	}

	public ClusterIterateFileSharesException(string message)
		: base(message)
	{
	}

	public ClusterIterateFileSharesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterIterateFileSharesException(Exception innerException)
		: base(ExceptionResources.ClusterIterateFileShares_Default, innerException)
	{
	}

	protected ClusterIterateFileSharesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

