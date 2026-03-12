using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterSharedVolumeNotOnlineException : ClusterException
{
	public string ResourceName { get; set; }

	public ClusterSharedVolumeNotOnlineException()
		: this(string.Empty)
	{
	}

	public ClusterSharedVolumeNotOnlineException(string resourceName)
		: base(ExceptionResources.SharedVolumeNotOnline.FormatCurrentCulture(resourceName))
	{
		ResourceName = resourceName;
	}

	public ClusterSharedVolumeNotOnlineException(string resourceName, Exception innerException)
		: base(ExceptionResources.SharedVolumeNotOnline.FormatCurrentCulture(resourceName), innerException)
	{
		ResourceName = resourceName;
	}

	protected ClusterSharedVolumeNotOnlineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
