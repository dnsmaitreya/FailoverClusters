using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterObjectLoadFailedException : ClusterException
{
	public Guid Id { get; private set; }

	public string Name { get; private set; }

	public ClusterObjectLoadFailedException()
		: this(string.Empty)
	{
	}

	public ClusterObjectLoadFailedException(string name)
		: this(name, null)
	{
	}

	public ClusterObjectLoadFailedException(string name, Exception innerException)
		: this(name, Guid.Empty, innerException)
	{
		if (name == null)
		{
			Exceptions.ThrowIfNull(name, "name");
		}
	}

	public ClusterObjectLoadFailedException(string name, Guid id)
		: this(ExceptionResources.ClusterObjectFailedToLoad_Default.FormatCurrentCulture(name ?? id.ToString()), null)
	{
	}

	public ClusterObjectLoadFailedException(string name, Guid id, Exception innerException)
		: base(ExceptionResources.ClusterObjectFailedToLoad_Default.FormatCurrentCulture(name ?? id.ToString()), innerException)
	{
		Name = name;
		Id = id;
	}

	protected ClusterObjectLoadFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

