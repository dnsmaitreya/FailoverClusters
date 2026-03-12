using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineNoCheckpointsToRevertException : ClusterDialogException
{
	public ClusterVirtualMachineNoCheckpointsToRevertException()
	{
	}

	public ClusterVirtualMachineNoCheckpointsToRevertException(string virtualMachineName)
		: this(ExceptionResources.ClusterVirtualMachineRevertCheckpointNotAvailableException_Text.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineNoCheckpointsToRevertException(string message, Exception ex)
		: base(message, ex)
	{
		base.Header = ExceptionResources.ClusterVirtualMachineRevertCheckpointNotAvailableException_Header;
		base.Caption = ExceptionResources.ClusterVirtualMachineRevertCheckpointNotAvailableException_Title;
		base.Icon = TaskDialogStandardIcon.Information;
	}

	protected ClusterVirtualMachineNoCheckpointsToRevertException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
