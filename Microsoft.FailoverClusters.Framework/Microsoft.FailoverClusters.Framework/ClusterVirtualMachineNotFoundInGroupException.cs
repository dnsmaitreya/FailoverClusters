using System;
using System.Runtime.Serialization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class ClusterVirtualMachineNotFoundInGroupException : ClusterDialogException
{
	public ClusterVirtualMachineNotFoundInGroupException()
		: this(string.Empty)
	{
	}

	public ClusterVirtualMachineNotFoundInGroupException(string virtualMachineGroupName)
		: base(ExceptionResources.VirtualMachineNotFoundInGroup_Default, null)
	{
		base.Header = ExceptionResources.VirtualMachineNotFoundInGroup_Header.FormatCurrentCulture(virtualMachineGroupName);
	}

	public ClusterVirtualMachineNotFoundInGroupException(string message, string virtualMachineGroupName)
		: base(message.FormatCurrentCulture(virtualMachineGroupName), null)
	{
		base.Header = ExceptionResources.VirtualMachineNotFoundInGroup_Header.FormatCurrentCulture(virtualMachineGroupName);
	}

	public ClusterVirtualMachineNotFoundInGroupException(string virtualMachineGroupName, Exception innerException)
		: base(ExceptionResources.VirtualMachineNotFoundInGroup_Default, innerException)
	{
		base.Header = ExceptionResources.VirtualMachineNotFoundInGroup_Header.FormatCurrentCulture(virtualMachineGroupName);
	}

	public ClusterVirtualMachineNotFoundInGroupException(string message, string virtualMachineGroupName, Exception innerException)
		: base(message.FormatCurrentCulture(virtualMachineGroupName), innerException)
	{
		base.Header = ExceptionResources.VirtualMachineNotFoundInGroup_Header.FormatCurrentCulture(virtualMachineGroupName);
	}

	protected ClusterVirtualMachineNotFoundInGroupException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
	}
}
