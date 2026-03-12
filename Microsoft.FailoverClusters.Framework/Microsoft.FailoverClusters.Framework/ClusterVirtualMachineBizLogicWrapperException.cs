using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineBizLogicWrapperException : ClusterDialogException
{
	public ClusterVirtualMachineBizLogicWrapperException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineBizLogicWrapper_Default.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.FailoverClusterManager_Text;
		base.Buttons = TaskDialogStandardButtons.Close;
	}

	public ClusterVirtualMachineBizLogicWrapperException(string virtualMachineName, int errorCode)
		: base(ExceptionResources.VirtualMachineBizLogicWrapper_Default.FormatCurrentCulture(errorCode, virtualMachineName), null)
	{
		base.Caption = CommonResources.FailoverClusterManager_Text;
		base.Buttons = TaskDialogStandardButtons.Close;
	}

	public ClusterVirtualMachineBizLogicWrapperException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
		base.Caption = CommonResources.FailoverClusterManager_Text;
		base.Buttons = TaskDialogStandardButtons.Close;
	}

	public ClusterVirtualMachineBizLogicWrapperException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineBizLogicWrapper_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Caption = CommonResources.FailoverClusterManager_Text;
		base.Buttons = TaskDialogStandardButtons.Close;
	}

	public ClusterVirtualMachineBizLogicWrapperException(string message, string virtualMachineName, Exception innerException)
		: base(message, innerException)
	{
		base.Caption = CommonResources.FailoverClusterManager_Text;
		base.Buttons = TaskDialogStandardButtons.Close;
	}

	protected ClusterVirtualMachineBizLogicWrapperException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
