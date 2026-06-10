using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineSaveException : ClusterException
{
	public ClusterVirtualMachineSaveException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineSaveException(string virtualMachineName)
		: base(ExceptionResources.VirtualMachineSave_Default.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineSaveException(string message, string virtualMachineName)
		: base(message.FormatCurrentCulture(virtualMachineName), null)
	{
	}

	public ClusterVirtualMachineSaveException(string virtualMachineName, Exception innerException)
		: base(ExceptionResources.VirtualMachineSave_Default.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	public ClusterVirtualMachineSaveException(string message, string virtualMachineName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineName), innerException)
	{
	}

	protected ClusterVirtualMachineSaveException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

