using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
internal class ClusterNodeDownWillCauseQuorumLossException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterNodeDownWillCauseQuorumLossException()
		: base(CommonResources.ClusterNodeDownException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterNodeDownWillCauseQuorumLossException(string name, Exception ex)
		: base(CommonResources.ClusterNodeDownWillCauseQuorumLossException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterNodeDownWillCauseQuorumLossException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	public ClusterNodeDownWillCauseQuorumLossException(string name)
		: this(name, null)
	{
	}

	protected ClusterNodeDownWillCauseQuorumLossException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

