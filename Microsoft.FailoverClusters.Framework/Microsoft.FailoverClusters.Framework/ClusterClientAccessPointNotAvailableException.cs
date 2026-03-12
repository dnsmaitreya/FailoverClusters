using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterClientAccessPointNotAvailableException : ClusterDialogException
{
	public ClusterClientAccessPointNotAvailableException()
		: this(string.Empty)
	{
	}

	public ClusterClientAccessPointNotAvailableException(string dnsName)
		: this(ExceptionResources.ClientAccessPointNotAvailable_Default.FormatCurrentCulture(dnsName), null, ExceptionResources.ClientAccessPointNotAvailable_Header)
	{
	}

	public ClusterClientAccessPointNotAvailableException(string message, Exception innerException)
		: this(message, innerException, string.Empty)
	{
	}

	public ClusterClientAccessPointNotAvailableException(string message, Exception innerException, string header)
		: base(message, innerException, CommonResources.Text_Error, TaskDialogStandardIcon.ShieldErrorBackground, header)
	{
	}

	protected ClusterClientAccessPointNotAvailableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
