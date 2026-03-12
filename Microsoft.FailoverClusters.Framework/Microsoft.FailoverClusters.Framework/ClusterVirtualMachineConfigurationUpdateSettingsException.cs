using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineConfigurationUpdateSettingsException : ClusterException
{
	public string VirtualMachineName { get; private set; }

	public ClusterVirtualMachineConfigurationUpdateSettingsException()
	{
	}

	public ClusterVirtualMachineConfigurationUpdateSettingsException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineConfigurationUpdateSettingsException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineConfigurationUpdateSettings_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineConfigurationUpdateSettingsException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineConfigurationUpdateSettingsException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineConfigurationUpdateSettingsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		if (info != null)
		{
			VirtualMachineName = info.GetString("VirtualMachineName");
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info?.AddValue("VirtualMachineName", VirtualMachineName);
	}
}
