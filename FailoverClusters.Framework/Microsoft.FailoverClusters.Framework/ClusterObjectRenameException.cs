using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterObjectRenameException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterObjectRenameException()
		: base(ExceptionResources.ClusterObjectRename_Default)
	{
	}

	public ClusterObjectRenameException(string name)
		: this(name, null)
	{
	}

	public ClusterObjectRenameException(string name, Exception innerException)
		: base(null, innerException)
	{
		Name = name;
		base.Header = ExceptionResources.ClusterObjectRename_WithParameter.FormatCurrentCulture(name);
	}

	protected ClusterObjectRenameException(SerializationInfo info, StreamingContext context)
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

