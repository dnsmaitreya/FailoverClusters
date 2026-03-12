using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterNodeEvictionWillCauseQuorumLossException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterNodeEvictionWillCauseQuorumLossException()
		: base(CommonResources.ClusterNodeEvictionException_Default.FormatCurrentCulture(string.Empty, 0))
	{
	}

	public ClusterNodeEvictionWillCauseQuorumLossException(string name, Exception ex)
		: base(CommonResources.ClusterNodeEvictionWillCauseQuorumLossException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterNodeEvictionWillCauseQuorumLossException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	public ClusterNodeEvictionWillCauseQuorumLossException(string name)
		: this(name, null)
	{
	}

	protected ClusterNodeEvictionWillCauseQuorumLossException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
