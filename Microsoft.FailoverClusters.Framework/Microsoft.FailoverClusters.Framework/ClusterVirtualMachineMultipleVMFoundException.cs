using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineMultipleVMFoundException : ClusterDialogException
{
	public ClusterVirtualMachineMultipleVMFoundException(int count)
		: base(ExceptionResources.VirtualMachineMultipleVMFound_Default.FormatCurrentCulture(count), null)
	{
	}

	public ClusterVirtualMachineMultipleVMFoundException(int count, Exception innerException)
		: base(ExceptionResources.VirtualMachineMultipleVMFound_Default.FormatCurrentCulture(count), innerException)
	{
	}

	protected ClusterVirtualMachineMultipleVMFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
