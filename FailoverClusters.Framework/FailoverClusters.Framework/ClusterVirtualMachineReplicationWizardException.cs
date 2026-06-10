using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineReplicationWizardException : ClusterDialogException
{
	public ClusterVirtualMachineReplicationWizardException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineReplicationWizardException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineReplicationWizard_Text.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineReplicationWizard_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineReplicationWizardException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineReplicationWizard_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineReplicationWizardException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineReplicationWizard_Text.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineReplicationWizard_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineReplicationWizardException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineReplicationWizard_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineReplicationWizardException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

