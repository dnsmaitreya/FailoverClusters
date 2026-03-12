using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterClientAccessPointNotOnlineException : ClusterDialogException
{
	public ClusterClientAccessPointNotOnlineException()
		: this(string.Empty)
	{
	}

	public ClusterClientAccessPointNotOnlineException(string message)
		: this(message, null)
	{
	}

	public ClusterClientAccessPointNotOnlineException(Resource clientAccessPoint)
		: this(ExceptionResources.ClientAccessPointNotOnline_Default.FormatCurrentCulture((clientAccessPoint != null) ? clientAccessPoint.Name : string.Empty), null, ExceptionResources.ClientAccessPointNotOnline_Header)
	{
	}

	public ClusterClientAccessPointNotOnlineException(string message, Exception innerException)
		: this(message, innerException, string.Empty)
	{
	}

	public ClusterClientAccessPointNotOnlineException(string message, Exception innerException, string header)
		: base(message, innerException, CommonResources.Text_Error, TaskDialogStandardIcon.ShieldErrorBackground, header)
	{
	}

	protected ClusterClientAccessPointNotOnlineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
