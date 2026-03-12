using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineSettingsDialogException : ClusterDialogException
{
	public ClusterVirtualMachineSettingsDialogException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineSettingsDialogException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineSettingsDialog_Text.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineSettingsDialog_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineSettingsDialogException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineSettingsDialog_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineSettingsDialogException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineSettingsDialog_Text.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineSettingsDialog_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineSettingsDialogException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineSettingsDialog_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineSettingsDialogException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
