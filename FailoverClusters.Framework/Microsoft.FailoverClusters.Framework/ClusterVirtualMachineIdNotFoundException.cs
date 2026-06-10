using System;
using System.Runtime.Serialization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineIdNotFoundException : ClusterException
{
	public ClusterVirtualMachineIdNotFoundException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineIdNotFoundException(string resourceName)
		: base(ExceptionResources.VirtualMachineIdNotFound_Default.FormatCurrentCulture(resourceName), null)
	{
	}

	public ClusterVirtualMachineIdNotFoundException(string message, string resourceName)
		: base(message.FormatCurrentCulture(resourceName), null)
	{
	}

	public ClusterVirtualMachineIdNotFoundException(string resourceName, Exception innerException)
		: base(ExceptionResources.VirtualMachineIdNotFound_Default.FormatCurrentCulture(resourceName), innerException)
	{
	}

	public ClusterVirtualMachineIdNotFoundException(string message, string resourceName, Exception innerException)
		: base(message.FormatCurrentCulture(resourceName), innerException)
	{
	}

	protected ClusterVirtualMachineIdNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}

