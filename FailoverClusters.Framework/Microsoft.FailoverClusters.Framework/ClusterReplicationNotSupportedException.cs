using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterReplicationNotSupportedException : ClusterDialogException
{
	public ClusterReplicationNotSupportedException()
		: this(ExceptionResources.ClusterReplicationSupportedException_Text, (Exception)null)
	{
	}

	public ClusterReplicationNotSupportedException(string message)
		: this(message, (Exception)null)
	{
	}

	public ClusterReplicationNotSupportedException(string header, string message)
		: this(message, header, null)
	{
	}

	public ClusterReplicationNotSupportedException(string message, Exception ex)
		: this(message, ExceptionResources.ClusterReplicationSupportedException_Header, ex)
	{
	}

	public ClusterReplicationNotSupportedException(string message, string header, Exception ex)
		: base(message, ex)
	{
		base.Header = header;
		base.Caption = ExceptionResources.ClusterReplicationNotSupportedException_Title;
		base.Icon = TaskDialogStandardIcon.Information;
	}

	protected ClusterReplicationNotSupportedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

