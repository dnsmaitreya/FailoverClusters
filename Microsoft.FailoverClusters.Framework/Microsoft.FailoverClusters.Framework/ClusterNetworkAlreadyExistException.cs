using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterNetworkAlreadyExistException : ClusterDialogException
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public ClusterNetworkAlreadyExistException()
		: this(Guid.Empty)
	{
	}

	public ClusterNetworkAlreadyExistException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public ClusterNetworkAlreadyExistException(Guid id)
		: base(ExceptionResources.NetworkAlreadyExist_Default.FormatCurrentCulture(id.ToString()))
	{
		base.Header = ExceptionResources.NetworkAlreadyExist_Header.FormatCurrentCulture(id.ToString());
		Id = id;
	}

	public ClusterNetworkAlreadyExistException(string name)
		: base(ExceptionResources.NetworkAlreadyExist_Default.FormatCurrentCulture(name))
	{
		base.Header = ExceptionResources.NetworkAlreadyExist_Header.FormatCurrentCulture(name);
		Name = name;
	}

	protected ClusterNetworkAlreadyExistException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
