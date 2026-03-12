using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterPropertyNotFoundException : ClusterException
{
	public Guid Id { get; set; }

	public string ObjectName { get; set; }

	public Type ObjectType { get; set; }

	public string PropertyName { get; set; }

	public ClusterPropertyNotFoundException()
		: this(Guid.Empty, string.Empty, typeof(object))
	{
	}

	public ClusterPropertyNotFoundException(string message)
		: base(message)
	{
	}

	public ClusterPropertyNotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterPropertyNotFoundException(Guid id, string propertyName, Type objectType)
		: base(ExceptionResources.ClusterPropertyNotFound_Default.FormatCurrentCulture(propertyName, (objectType != null) ? objectType.Name : string.Empty, id.ToString()))
	{
		Id = id;
		ObjectType = objectType;
		PropertyName = propertyName;
	}

	public ClusterPropertyNotFoundException(Guid id, string propertyName, Type objectType, int returnCode)
		: base(ExceptionResources.ClusterPropertyNotFound_Default.FormatCurrentCulture(propertyName, (objectType != null) ? objectType.Name : string.Empty, id.ToString()), (returnCode != NativeMethods.ErrorCode.FileNotFound.ToInt()) ? ExceptionHelper.Build(returnCode) : null)
	{
		Id = id;
		ObjectType = objectType;
		PropertyName = propertyName;
	}

	public ClusterPropertyNotFoundException(string objectName, string propertyName, Type objectType)
		: this(objectName, propertyName, objectType, NativeMethods.ErrorCode.FileNotFound.ToInt())
	{
	}

	public ClusterPropertyNotFoundException(string objectName, string propertyName, Type objectType, int returnCode)
		: base(ExceptionResources.ClusterPropertyNotFound_Default.FormatCurrentCulture(propertyName, (objectType != null) ? objectType.Name : string.Empty, objectName), (returnCode != NativeMethods.ErrorCode.FileNotFound.ToInt()) ? ExceptionHelper.Build(returnCode) : null)
	{
		ObjectName = objectName;
		ObjectType = objectType;
		PropertyName = propertyName;
	}

	protected ClusterPropertyNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
