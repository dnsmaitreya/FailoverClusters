using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineStartReplicaException : ClusterDialogException
{
	public ClusterVirtualMachineStartReplicaException()
		: this("<Unknown>")
	{
	}

	public ClusterVirtualMachineStartReplicaException(string virtualMachineName)
		: this(virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineStartReplicaException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineReplicaStart_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineReplicaStart_Header;
	}

	public ClusterVirtualMachineStartReplicaException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
		base.Icon = TaskDialogStandardIcon.Error;
		base.Header = ExceptionResources.VirtualMachineReplicaStart_Header;
	}

	protected ClusterVirtualMachineStartReplicaException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

