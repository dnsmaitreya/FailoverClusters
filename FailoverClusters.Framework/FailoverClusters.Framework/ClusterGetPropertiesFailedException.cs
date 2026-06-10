using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterGetPropertiesFailedException : ClusterDialogException
{
	public ClusterGetPropertiesFailedException()
		: this((Exception)null)
	{
	}

	public ClusterGetPropertiesFailedException(string message)
		: this(message, null)
	{
	}

	public ClusterGetPropertiesFailedException(Exception innerException)
		: this(ExceptionResources.ClusterGetPropertiesFailed_Default, innerException)
	{
	}

	public ClusterGetPropertiesFailedException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.Header = ExceptionResources.ClusterGetPropertiesFailed_Header;
	}

	protected ClusterGetPropertiesFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

