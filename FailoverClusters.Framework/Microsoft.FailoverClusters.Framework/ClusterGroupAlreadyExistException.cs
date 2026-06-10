using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterGroupAlreadyExistException : ClusterDialogException
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public ClusterGroupAlreadyExistException()
		: this(string.Empty)
	{
	}

	public ClusterGroupAlreadyExistException(string name, Exception innerException)
		: base(ExceptionResources.GroupAlreadyExist_Default.FormatCurrentCulture(name), innerException)
	{
		base.Header = ExceptionResources.GroupAlreadyExist_Header.FormatCurrentCulture(name);
		Name = name;
	}

	public ClusterGroupAlreadyExistException(Guid id)
		: base(ExceptionResources.GroupAlreadyExist_Default.FormatCurrentCulture(id.ToString()))
	{
		base.Header = ExceptionResources.GroupAlreadyExist_Header.FormatCurrentCulture(id.ToString());
		Id = id;
	}

	public ClusterGroupAlreadyExistException(string name)
		: base(ExceptionResources.GroupAlreadyExist_Default.FormatCurrentCulture(name))
	{
		base.Header = ExceptionResources.GroupAlreadyExist_Header.FormatCurrentCulture(name);
		Name = name;
	}

	protected ClusterGroupAlreadyExistException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

