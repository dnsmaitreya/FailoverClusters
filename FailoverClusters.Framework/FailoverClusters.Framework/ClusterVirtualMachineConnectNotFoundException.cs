using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineConnectNotFoundException : ClusterDialogException
{
	public ClusterVirtualMachineConnectNotFoundException()
		: base(ExceptionResources.VirtualMachineCouldNotFindVMConnectTool_Default, null)
	{
		base.Caption = CommonResources.Text_ToolNotFound;
		base.Icon = TaskDialogStandardIcon.ShieldWarningBackground;
		base.Header = ExceptionResources.VirtualMachineCouldNotFindVMConnectTool_Header;
	}

	public ClusterVirtualMachineConnectNotFoundException(string message)
		: base(message, null)
	{
		base.Caption = CommonResources.Text_ToolNotFound;
		base.Icon = TaskDialogStandardIcon.ShieldWarningBackground;
		base.Header = ExceptionResources.VirtualMachineCouldNotFindVMConnectTool_Header;
	}

	public ClusterVirtualMachineConnectNotFoundException(Exception innerException)
		: base(ExceptionResources.VirtualMachineCouldNotFindVMConnectTool_Default, innerException)
	{
		base.Caption = CommonResources.Text_ToolNotFound;
		base.Icon = TaskDialogStandardIcon.ShieldWarningBackground;
		base.Header = ExceptionResources.VirtualMachineCouldNotFindVMConnectTool_Header;
	}

	public ClusterVirtualMachineConnectNotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.Caption = CommonResources.Text_ToolNotFound;
		base.Icon = TaskDialogStandardIcon.ShieldWarningBackground;
		base.Header = ExceptionResources.VirtualMachineCouldNotFindVMConnectTool_Header;
	}

	protected ClusterVirtualMachineConnectNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

