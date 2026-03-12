using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterNodeDrainFailedException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterNodeDrainFailedException()
		: this(string.Empty)
	{
	}

	public ClusterNodeDrainFailedException(string name)
		: this(name, null)
	{
	}

	public ClusterNodeDrainFailedException(string name, Exception ex)
		: base(ExceptionResources.ClusterNodeDrainFailed_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = ExceptionResources.ClusterNodeDrainFailed_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterNodeDrainFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
