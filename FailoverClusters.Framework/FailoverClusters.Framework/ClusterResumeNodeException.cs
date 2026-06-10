using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
internal class ClusterResumeNodeException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterResumeNodeException()
		: base(CommonResources.ClusterPauseNodeException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterResumeNodeException(string name, Exception ex)
		: base(CommonResources.ClusterResumeNodeException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterResumeNodeException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterResumeNodeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

