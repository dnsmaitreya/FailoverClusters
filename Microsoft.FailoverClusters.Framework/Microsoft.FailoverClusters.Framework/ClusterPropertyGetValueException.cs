using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterPropertyGetValueException : ClusterException
{
	public Guid Id { get; set; }

	public string ObjectName { get; set; }

	public string ObjectType { get; set; }

	public string PropertyName { get; set; }

	public ClusterPropertyGetValueException()
		: this(Guid.Empty, string.Empty, "object")
	{
	}

	public ClusterPropertyGetValueException(string message)
		: base(message)
	{
	}

	public ClusterPropertyGetValueException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterPropertyGetValueException(Guid id, string propertyName, string objectType)
		: base(ExceptionResources.ClusterPropertyGetValue_Default.FormatCurrentCulture(propertyName, (objectType != null) ? objectType : string.Empty, id.ToString()))
	{
		Id = id;
		ObjectType = objectType;
		PropertyName = propertyName;
	}

	public ClusterPropertyGetValueException(Guid id, string propertyName, string objectType, int returnCode)
		: base(ExceptionResources.ClusterPropertyGetValue_Default.FormatCurrentCulture(propertyName, (objectType != null) ? objectType : string.Empty, id.ToString()), (returnCode != NativeMethods.ErrorCode.FileNotFound.ToInt()) ? ExceptionHelper.Build(returnCode) : null)
	{
		Id = id;
		ObjectType = objectType;
		PropertyName = propertyName;
	}

	public ClusterPropertyGetValueException(string objectName, string propertyName, string objectType)
		: this(objectName, propertyName, objectType, NativeMethods.ErrorCode.FileNotFound.ToInt())
	{
	}

	public ClusterPropertyGetValueException(string objectName, string propertyName, string objectType, int returnCode)
		: base(ExceptionResources.ClusterPropertyGetValue_Default.FormatCurrentCulture(propertyName, (objectType != null) ? objectType : string.Empty, objectName), (returnCode != NativeMethods.ErrorCode.FileNotFound.ToInt()) ? ExceptionHelper.Build(returnCode) : null)
	{
		ObjectName = objectName;
		ObjectType = objectType;
		PropertyName = propertyName;
	}

	protected ClusterPropertyGetValueException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
