using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineWarningToLogException : ClusterDialogException, IRedirectToCriticalEvents
{
	public ClusterObject CriticalEventsParameter { get; private set; }

	public ClusterVirtualMachineWarningToLogException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineWarningToLogException(string virtualMachineName)
		: this(virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineWarningToLogException(string virtualMachineName, Exception innerException)
		: this(virtualMachineName, null, innerException)
	{
	}

	public ClusterVirtualMachineWarningToLogException(string virtualMachineName, ClusterObject criticalEventsParameter, Exception innerException)
		: base(string.Empty, innerException)
	{
		base.Header = ((virtualMachineName == null) ? ExceptionResources.VirtualMachineWarningToLog_Header1 : ExceptionResources.VirtualMachineWarningToLog_Header.FormatCurrentCulture(virtualMachineName));
		CriticalEventsParameter = criticalEventsParameter;
	}

	protected ClusterVirtualMachineWarningToLogException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

