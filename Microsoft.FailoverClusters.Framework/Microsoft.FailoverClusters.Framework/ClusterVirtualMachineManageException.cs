using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineManageException : ClusterDialogException
{
	public ClusterVirtualMachineManageException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineManageException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineManage_Text.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineManage_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineManageException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineManage_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineManageException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineManage_Text.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineManage_Header.FormatCurrentCulture(virtualMachineName);
	}

	public ClusterVirtualMachineManageException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Caption = CommonResources.Text_ConnectionError;
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineManage_Header.FormatCurrentCulture(virtualMachineName);
	}

	protected ClusterVirtualMachineManageException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
