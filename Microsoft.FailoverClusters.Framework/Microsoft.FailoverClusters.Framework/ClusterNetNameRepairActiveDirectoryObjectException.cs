using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterNetNameRepairActiveDirectoryObjectException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterNetNameRepairActiveDirectoryObjectException()
		: base(ExceptionResources.ClusterNetNameRepairActiveDirectoryObject_Default)
	{
	}

	public ClusterNetNameRepairActiveDirectoryObjectException(string name)
		: this(name, null)
	{
	}

	public ClusterNetNameRepairActiveDirectoryObjectException(string name, Exception innerException)
		: this(name, null, innerException)
	{
	}

	public ClusterNetNameRepairActiveDirectoryObjectException(string name, string message, Exception innerException)
		: base(null, innerException)
	{
		Name = name;
		base.Content = message;
		base.Header = ExceptionResources.ClusterNetNameRepairActiveDirectoryObject_WithParameter.FormatCurrentCulture(name);
	}

	protected ClusterNetNameRepairActiveDirectoryObjectException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			Name = info.GetString("Name");
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info?.AddValue("Name", Name);
		base.GetObjectData(info, context);
	}
}
