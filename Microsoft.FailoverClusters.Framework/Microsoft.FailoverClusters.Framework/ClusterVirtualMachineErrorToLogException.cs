using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineErrorToLogException : ClusterDialogException, IRedirectToCriticalEvents
{
	public ClusterObject CriticalEventsParameter { get; private set; }

	public ClusterVirtualMachineErrorToLogException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineErrorToLogException(string virtualMachineName)
		: this(virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineErrorToLogException(string virtualMachineName, Exception innerException)
		: this(virtualMachineName, null, innerException)
	{
	}

	public ClusterVirtualMachineErrorToLogException(string virtualMachineName, ClusterObject criticalEventsParameter, Exception innerException)
		: base(string.Empty, innerException)
	{
		base.Header = ((virtualMachineName == null) ? ExceptionResources.VirtualMachineErrorToLog_Header1 : ExceptionResources.VirtualMachineErrorToLog_Header.FormatCurrentCulture(virtualMachineName));
		CriticalEventsParameter = criticalEventsParameter;
	}

	protected ClusterVirtualMachineErrorToLogException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
