using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterPropertiesNotAvailableException : ClusterException
{
	public ClusterPropertiesNotAvailableException()
		: this((Exception)null)
	{
	}

	public ClusterPropertiesNotAvailableException(string message)
		: base(message)
	{
	}

	public ClusterPropertiesNotAvailableException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterPropertiesNotAvailableException(Exception innerException)
		: base(ExceptionResources.ClusterPropertiesNotAvailable_Default, innerException)
	{
	}

	protected ClusterPropertiesNotAvailableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

