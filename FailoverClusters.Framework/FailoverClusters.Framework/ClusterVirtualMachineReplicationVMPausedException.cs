using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineReplicationVMPausedException : ClusterDialogException
{
	public ClusterVirtualMachineReplicationVMPausedException()
	{
	}

	public ClusterVirtualMachineReplicationVMPausedException(string message)
		: base(message)
	{
	}

	public ClusterVirtualMachineReplicationVMPausedException(string header, string message, string virtualMachineName)
		: base(message, null, CommonResources.FailoverClusterManager_Text, TaskDialogStandardIcon.ShieldWarningBackground, header)
	{
	}

	public ClusterVirtualMachineReplicationVMPausedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ClusterVirtualMachineReplicationVMPausedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

