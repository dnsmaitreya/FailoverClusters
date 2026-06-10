using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterGetDiskException : ClusterException
{
	public ClusterGetDiskException()
		: this((Exception)null)
	{
	}

	public ClusterGetDiskException(string resourceName)
		: this(resourceName, null)
	{
	}

	public ClusterGetDiskException(Exception innerException)
		: this(string.Empty, innerException)
	{
	}

	public ClusterGetDiskException(string resourceName, Exception innerException)
		: base(ExceptionResources.GetClusterableDisksFail_Default.FormatCurrentCulture(resourceName), innerException)
	{
	}

	protected ClusterGetDiskException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

