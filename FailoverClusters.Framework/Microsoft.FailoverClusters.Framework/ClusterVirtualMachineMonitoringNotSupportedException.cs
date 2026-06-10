using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineMonitoringNotSupportedException : ClusterException
{
	public ClusterVirtualMachineMonitoringNotSupportedException()
	{
	}

	public ClusterVirtualMachineMonitoringNotSupportedException(string virtualMachineName)
		: this(ExceptionResources.MonitoringNotSupported_Default, (Exception)null)
	{
	}

	public ClusterVirtualMachineMonitoringNotSupportedException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineMonitoringNotSupportedException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.MonitoringNotSupported_Default, innerException)
	{
	}

	public ClusterVirtualMachineMonitoringNotSupportedException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(BrandingResources.WINDOWS_SERVER_CURRENT_VERSION), innerException)
	{
	}

	protected ClusterVirtualMachineMonitoringNotSupportedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

