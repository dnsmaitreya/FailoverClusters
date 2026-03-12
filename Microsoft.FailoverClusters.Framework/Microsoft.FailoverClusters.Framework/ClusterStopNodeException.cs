using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterStopNodeException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterStopNodeException()
		: base(CommonResources.ClusterPauseNodeException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterStopNodeException(string name, Exception ex)
		: base(CommonResources.ClusterStopNodeException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterStopNodeException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterStopNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
