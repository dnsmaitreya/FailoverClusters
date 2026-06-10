using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterShadowCopyException : ClusterDialogException
{
	public ClusterShadowCopyException()
		: this(string.Empty, string.Empty, null)
	{
	}

	public ClusterShadowCopyException(string taskName, string resourceName, Exception innerException)
		: base(ExceptionResources.ShadowCopyFailed_Default.FormatCurrentCulture(taskName, resourceName), innerException)
	{
		base.Header = ExceptionResources.ShadowCopyFailed_Header;
		base.Caption = ExceptionResources.ShadowCopyFailed_Caption;
	}

	public ClusterShadowCopyException(string taskName)
		: this(taskName, string.Empty, null)
	{
	}

	public ClusterShadowCopyException(string taskName, Exception exception)
		: this(taskName, string.Empty, exception)
	{
	}

	protected ClusterShadowCopyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

