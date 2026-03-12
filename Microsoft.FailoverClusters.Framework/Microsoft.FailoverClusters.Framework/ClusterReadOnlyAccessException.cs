using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterReadOnlyAccessException : ClusterDialogException
{
	public ClusterReadOnlyAccessException()
		: this((Exception)null)
	{
	}

	public ClusterReadOnlyAccessException(string clusterName)
		: this(clusterName, null)
	{
	}

	public ClusterReadOnlyAccessException(Exception innerException)
		: this(string.Empty, innerException)
	{
	}

	public ClusterReadOnlyAccessException(string clusterName, Exception innerException)
		: base(ExceptionResources.ReadOnlyAccess_Default, innerException)
	{
		base.Header = ExceptionResources.ReadOnlyAccess_Header.FormatCurrentCulture(clusterName);
	}

	protected ClusterReadOnlyAccessException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
