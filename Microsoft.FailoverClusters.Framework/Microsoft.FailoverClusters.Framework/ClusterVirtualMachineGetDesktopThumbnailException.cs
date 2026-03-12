using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineGetDesktopThumbnailException : ClusterException
{
	public string VirtualMachineName { get; private set; }

	public ClusterVirtualMachineGetDesktopThumbnailException()
	{
	}

	public ClusterVirtualMachineGetDesktopThumbnailException(string virtualMachineName)
		: this(virtualMachineName, (Exception)null)
	{
	}

	public ClusterVirtualMachineGetDesktopThumbnailException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineGetDesktopThumbnail_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineGetDesktopThumbnailException(string message, string virtualMachineName)
		: this(message, virtualMachineName, null)
	{
	}

	public ClusterVirtualMachineGetDesktopThumbnailException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineGetDesktopThumbnailException(SerializationInfo info, StreamingContext context)
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
