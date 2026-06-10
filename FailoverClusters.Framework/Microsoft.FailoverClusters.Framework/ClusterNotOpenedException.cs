using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterNotOpenedException : ClusterException
{
	public ClusterNotOpenedException()
		: base(ExceptionResources.ClusterNotOpened_Default)
	{
	}

	protected ClusterNotOpenedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

