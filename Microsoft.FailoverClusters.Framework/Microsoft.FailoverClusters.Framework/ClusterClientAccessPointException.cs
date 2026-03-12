using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterClientAccessPointException : ClusterDialogException
{
	public ClusterClientAccessPointException()
		: this(string.Empty)
	{
	}

	public ClusterClientAccessPointException(string message)
		: this(message, null, ExceptionResources.ClusterAccessPointIssue_Header)
	{
	}

	public ClusterClientAccessPointException(string message, Exception innerException)
		: this(message, innerException, string.Empty)
	{
	}

	public ClusterClientAccessPointException(string message, Exception innerException, string header)
		: base(message, innerException, CommonResources.Text_Error, TaskDialogStandardIcon.ShieldErrorBackground, header)
	{
	}

	protected ClusterClientAccessPointException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
