using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
internal class ClusterGroupMovingException : ClusterDialogException
{
	public ClusterGroupMovingException()
		: base(ExceptionResources.ClusterGroupMoving_Default.FormatCurrentCulture(string.Empty, 0))
	{
		base.Header = ExceptionResources.ClusterGroupMoving_Header.FormatCurrentCulture(string.Empty);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
	}

	public ClusterGroupMovingException(string name, Exception ex)
		: base(ExceptionResources.ClusterGroupMoving_Default.FormatCurrentCulture(name), ex)
	{
		base.Header = ExceptionResources.ClusterGroupMoving_Header.FormatCurrentCulture(name);
		base.Caption = CommonResources.Text_Error;
		base.Icon = TaskDialogStandardIcon.Error;
	}

	protected ClusterGroupMovingException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
