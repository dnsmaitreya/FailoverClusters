using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterResumeNodeTimeoutException : ClusterDialogException
{
	public string Name { get; set; }

	public ClusterResumeNodeTimeoutException()
		: this(string.Empty, null)
	{
	}

	public ClusterResumeNodeTimeoutException(string name, Exception ex)
		: base(CommonResources.ClusterResumeNodeTimeoutException_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = CommonResources.ClusterResumeNodeTimeoutException_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
		Name = name;
	}

	protected ClusterResumeNodeTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
