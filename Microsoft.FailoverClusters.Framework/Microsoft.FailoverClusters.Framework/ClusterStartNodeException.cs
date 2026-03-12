using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterStartNodeException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterStartNodeException()
		: base(CommonResources.ClusterPauseNodeException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterStartNodeException(string name, Exception ex)
		: base(CommonResources.ClusterStartNodeException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterStartNodeException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterStartNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
