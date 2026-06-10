using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterReplicationMbrNotSupportedException : ClusterDialogException
{
	public ClusterReplicationMbrNotSupportedException()
		: this(ExceptionResources.ClusterReplicationMbrNotSupportedException_Text, null)
	{
	}

	public ClusterReplicationMbrNotSupportedException(string message)
		: this(message, null)
	{
	}

	public ClusterReplicationMbrNotSupportedException(string message, Exception ex)
		: base(message, ex)
	{
		base.Header = ExceptionResources.ClusterReplicationMbrNotSupportedException_Header;
		base.Caption = ExceptionResources.ClusterReplicationMbrNotSupportedException_Title;
		base.Icon = TaskDialogStandardIcon.Information;
	}

	protected ClusterReplicationMbrNotSupportedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

