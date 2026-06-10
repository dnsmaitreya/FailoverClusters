using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterErrorException : ClusterException
{
	public ClusterErrorException()
		: this(string.Empty)
	{
	}

	public ClusterErrorException(string clusterName)
		: this(clusterName, null)
	{
	}

	public ClusterErrorException(string clusterName, Exception innerException)
		: base(ExceptionResources.FatalError_Default.FormatCurrentCulture(clusterName), innerException)
	{
	}

	protected ClusterErrorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

