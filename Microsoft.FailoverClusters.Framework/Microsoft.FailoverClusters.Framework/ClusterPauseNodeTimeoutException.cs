using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterPauseNodeTimeoutException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterPauseNodeTimeoutException()
		: this(string.Empty, null)
	{
	}

	public ClusterPauseNodeTimeoutException(string name, Exception ex)
		: base(CommonResources.ClusterPauseNodeTimeoutException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterPauseNodeTimeoutException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterPauseNodeTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
