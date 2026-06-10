using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
internal class ClusterHyperVNotSupportedException : ClusterDialogException
{
	public ClusterHyperVNotSupportedException()
		: this(ExceptionResources.ClusterHyperVNotSupportedException_Text, null)
	{
	}

	public ClusterHyperVNotSupportedException(string message)
		: this(message, null)
	{
	}

	public ClusterHyperVNotSupportedException(string message, Exception ex)
		: base(message, ex)
	{
		base.Header = ExceptionResources.ClusterHyperVNotSupportedException_Header;
		base.Caption = ExceptionResources.ClusterHyperVNotSupportedException_Title;
		base.Icon = TaskDialogStandardIcon.Information;
	}

	protected ClusterHyperVNotSupportedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

