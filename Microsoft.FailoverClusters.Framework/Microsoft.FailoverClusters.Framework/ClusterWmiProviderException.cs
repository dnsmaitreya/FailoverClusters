using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterWmiProviderException : ClusterDialogException
{
	public ClusterWmiProviderException()
		: this(string.Empty)
	{
	}

	public ClusterWmiProviderException(string message)
		: this(message, null)
	{
	}

	public ClusterWmiProviderException(StringBuilder nodeNames)
		: this(ExceptionResources.ClusterWmiProviderFailure_Default.FormatCurrentCulture(nodeNames?.ToString().Trim()), null, ExceptionResources.ClusterWmiProviderFailure_Header)
	{
	}

	public ClusterWmiProviderException(string message, Exception innerException)
		: this(message, innerException, string.Empty)
	{
	}

	public ClusterWmiProviderException(string message, Exception innerException, string header)
		: base(message, innerException, CommonResources.Text_Error, TaskDialogStandardIcon.ShieldErrorBackground, header)
	{
	}

	protected ClusterWmiProviderException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
