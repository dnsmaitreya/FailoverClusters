using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
internal class ClusterPauseNodeException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterPauseNodeException()
		: base(CommonResources.ClusterPauseNodeException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterPauseNodeException(string name, Exception ex)
		: base(CommonResources.ClusterPauseNodeException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterPauseNodeException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterPauseNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

