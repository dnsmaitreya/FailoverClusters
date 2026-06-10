using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class NetworkNameNotReadyException : ClusterDialogException
{
	public NetworkNameNotReadyException()
		: this(string.Empty)
	{
	}

	public NetworkNameNotReadyException(string networkName)
		: base(ExceptionResources.NetworkNameNotReadyException_Default.FormatCurrentCulture(networkName))
	{
		base.Caption = ExceptionResources.NetworkNameNotReadyException_Caption;
		base.Icon = TaskDialogStandardIcon.ShieldWarningBackground;
		base.Header = ExceptionResources.NetworkNameNotReadyException_Header;
	}

	public NetworkNameNotReadyException(string networkName, Exception innerException)
		: base(ExceptionResources.NetworkNameNotReadyException_Default.FormatCurrentCulture(networkName), innerException)
	{
	}

	protected NetworkNameNotReadyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

