using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterFieldNotFoundException : ClusterException
{
	public Guid Id { get; set; }

	public string Field { get; set; }

	public Type ObjectType { get; set; }

	public ClusterFieldNotFoundException()
		: this(Guid.Empty, string.Empty, typeof(object))
	{
	}

	public ClusterFieldNotFoundException(string message)
		: base(message)
	{
	}

	public ClusterFieldNotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterFieldNotFoundException(Guid id, string field, Type objectType)
		: base(ExceptionResources.FieldNotFound_Default.FormatCurrentCulture(field, id.ToString(), (objectType != null) ? objectType.Name : string.Empty))
	{
		Id = id;
		Field = field;
		ObjectType = objectType;
	}

	public ClusterFieldNotFoundException(Guid id, string field, Type objectType, Exception innerException)
		: base(ExceptionResources.FieldNotFound_Default.FormatCurrentCulture(field, id.ToString(), (objectType != null) ? objectType.Name : string.Empty), innerException)
	{
		Id = id;
		Field = field;
		ObjectType = objectType;
	}

	protected ClusterFieldNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

