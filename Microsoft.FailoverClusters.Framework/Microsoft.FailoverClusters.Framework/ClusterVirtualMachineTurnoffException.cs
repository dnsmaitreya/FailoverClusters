using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineTurnoffException : ClusterException
{
	public ClusterVirtualMachineTurnoffException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineTurnoffException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineTurnOff_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineTurnoffException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineTurnoffException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineTurnOff_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineTurnoffException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineTurnoffException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
