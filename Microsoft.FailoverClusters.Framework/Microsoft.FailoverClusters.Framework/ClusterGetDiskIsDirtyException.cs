using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterGetDiskIsDirtyException : ClusterException
{
	public ClusterGetDiskIsDirtyException()
		: this((Exception)null)
	{
	}

	public ClusterGetDiskIsDirtyException(string resourceName)
		: this(resourceName, null)
	{
	}

	public ClusterGetDiskIsDirtyException(Exception innerException)
		: this(string.Empty, innerException)
	{
	}

	public ClusterGetDiskIsDirtyException(string resourceName, Exception innerException)
		: base(ExceptionResources.ClusterDiskIsDirty_Default.FormatCurrentCulture(resourceName), innerException)
	{
	}

	protected ClusterGetDiskIsDirtyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
