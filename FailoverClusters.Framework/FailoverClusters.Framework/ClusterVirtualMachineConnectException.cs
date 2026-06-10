using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineConnectException : ClusterDialogException
{
	public string VirtualMachineName { get; private set; }

	public ClusterVirtualMachineConnectException()
	{
	}

	public ClusterVirtualMachineConnectException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineConnectException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineConnect_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		VirtualMachineName = virtualMachineName;
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineConnect_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineConnectException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineConnectException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		VirtualMachineName = virtualMachineName;
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineConnect_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineConnectException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			VirtualMachineName = info.GetString("VirtualMachineName");
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info?.AddValue("VirtualMachineName", VirtualMachineName);
	}
}

