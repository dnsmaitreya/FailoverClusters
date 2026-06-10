using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterInvalidPropertyDataException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterInvalidPropertyDataException()
	{
	}

	public ClusterInvalidPropertyDataException(string name)
		: base(ExceptionResources.SavePropertiesInvalidData_Default.FormatCurrentCulture(name), null)
	{
		base.Header = ExceptionResources.SavePropertiesInvalidData_Default.FormatCurrentCulture(name);
	}

	public ClusterInvalidPropertyDataException(string name, Exception innerException)
		: base((innerException == null) ? ExceptionResources.SavePropertiesInvalidData_Default.FormatCurrentCulture(name) : innerException.Message, innerException)
	{
		base.Header = ExceptionResources.SavePropertiesInvalidData_Default.FormatCurrentCulture(name);
	}

	protected ClusterInvalidPropertyDataException(SerializationInfo info, StreamingContext context)
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

