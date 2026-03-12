using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterSavePropertiesException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterSavePropertiesException()
	{
	}

	public ClusterSavePropertiesException(string name)
		: base(ExceptionResources.SaveProperties_Default.FormatCurrentCulture(name), null)
	{
		base.Header = ExceptionResources.SaveProperties_Default.FormatCurrentCulture(name);
	}

	public ClusterSavePropertiesException(string name, Exception innerException)
		: base((innerException == null) ? ExceptionResources.SaveProperties_Default.FormatCurrentCulture(name) : innerException.Message, innerException)
	{
		base.Header = ExceptionResources.SaveProperties_Default.FormatCurrentCulture(name);
	}

	protected ClusterSavePropertiesException(SerializationInfo info, StreamingContext context)
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
