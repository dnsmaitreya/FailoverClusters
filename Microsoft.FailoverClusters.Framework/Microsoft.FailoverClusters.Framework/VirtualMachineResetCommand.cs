using System.Threading;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualMachineResetCommand : VirtualMachineMoreActionsCommandBase
{
	public VirtualMachineResetCommand(VirtualMachineGroup group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		VirtualMachineGroup virtualMachineGroup = (VirtualMachineGroup)base.ClusterGroup;
		return new ClusterCommand(virtualMachineGroup, "Reset", ClusterCommandId.VirtualMachineGroupReset, ClusterCommandCollectionId.VirtualMachineGroup)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Reset,
			CanExecuteDelegate = delegate
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				bool canExecute = false;
				virtualMachineGroup.ExecuteOnVmResource(delegate
				{
					canExecute = virtualMachineGroup.ApplicationVirtualMachineState.HasValue && (virtualMachineGroup.ApplicationVirtualMachineState == VirtualMachineState.Running || virtualMachineGroup.ApplicationVirtualMachineState == VirtualMachineState.Paused);
					SendCanExecuteUpdateIfNeeded(threadId);
				});
				return canExecute;
			},
			ExecuteDelegate = delegate
			{
				virtualMachineGroup.Reset(askConfirmation: true);
			}
		};
	}
}
