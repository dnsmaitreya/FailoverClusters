using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterResourceAlreadyExistException : ClusterDialogException
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public ClusterResourceAlreadyExistException()
		: this(Guid.Empty)
	{
	}

	public ClusterResourceAlreadyExistException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterResourceAlreadyExistException(Guid id)
		: base(ExceptionResources.ResourceAlreadyExist_Default.FormatCurrentCulture(id.ToString()))
	{
		base.Header = ExceptionResources.ResourceAlreadyExist_Header.FormatCurrentCulture(id.ToString());
		Id = id;
	}

	public ClusterResourceAlreadyExistException(string name)
		: base(ExceptionResources.ResourceAlreadyExist_Default.FormatCurrentCulture(name))
	{
		base.Header = ExceptionResources.ResourceAlreadyExist_Header.FormatCurrentCulture(name);
		Name = name;
	}

	protected ClusterResourceAlreadyExistException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
