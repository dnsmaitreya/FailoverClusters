using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterObjectNotFoundException : ClusterException, IExceptionLogLevel
{
	public Guid Id { get; private set; }

	public string Name { get; private set; }

	public Type ObjectType { get; private set; }

	public LogLevel Level => LogLevel.Verbose;

	public ClusterObjectNotFoundException()
		: this(string.Empty)
	{
	}

	public ClusterObjectNotFoundException(string name)
		: this(name, null)
	{
	}

	public ClusterObjectNotFoundException(string name, Exception innerException)
		: this(name, Guid.Empty, null, innerException)
	{
		if (name == null)
		{
			Exceptions.ThrowIfNull(name, "name");
		}
	}

	public ClusterObjectNotFoundException(string name, Guid id, ClusterIdentityType identity)
		: base(ExceptionResources.ClusterObjectNotFound_Default.FormatCurrentCulture(name ?? id.ToString(), identity.Translate(), null))
	{
	}

	public ClusterObjectNotFoundException(string name, Guid id, Type objectType)
		: base(ExceptionResources.ClusterObjectNotFound_Default.FormatCurrentCulture(name ?? id.ToString(), (objectType != null) ? objectType.Name : string.Empty, null))
	{
	}

	public ClusterObjectNotFoundException(string name, Guid id, Type objectType, Exception innerException)
		: base(ExceptionResources.ClusterObjectNotFound_Default.FormatCurrentCulture(name ?? id.ToString(), (objectType != null) ? objectType.Name : string.Empty, innerException))
	{
		Name = name;
		Id = id;
		ObjectType = objectType;
	}

	protected ClusterObjectNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

