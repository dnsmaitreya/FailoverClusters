using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineHyperVNotFoundException : ClusterDialogException
{
	public ClusterVirtualMachineHyperVNotFoundException()
		: this(ExceptionResources.VirtualMachineCouldNotFindHyperVTool_Default, null)
	{
	}

	public ClusterVirtualMachineHyperVNotFoundException(string message)
		: this(message, null)
	{
	}

	public ClusterVirtualMachineHyperVNotFoundException(Exception innerException)
		: this(ExceptionResources.VirtualMachineCouldNotFindHyperVTool_Default, innerException)
	{
	}

	public ClusterVirtualMachineHyperVNotFoundException(string message, Exception innerException)
		: this(message, innerException, ExceptionResources.VirtualMachineCouldNotFindHyperVTool_Header)
	{
	}

	public ClusterVirtualMachineHyperVNotFoundException(string message, Exception innerException, string header)
		: base(message, innerException, CommonResources.Text_ToolNotFound, TaskDialogStandardIcon.ShieldWarningBackground, header)
	{
	}

	protected ClusterVirtualMachineHyperVNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
