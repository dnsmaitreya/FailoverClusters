using System.Threading;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineDeleteSavedStateCommand : VirtualMachineMoreActionsCommandBase
{
	public VirtualMachineDeleteSavedStateCommand(VirtualMachineGroup group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		VirtualMachineGroup virtualMachineGroup = (VirtualMachineGroup)base.ClusterGroup;
		return new ClusterCommand(virtualMachineGroup, "DeleteSavedState", ClusterCommandId.VirtualMachineGroupDeleteSavedState, ClusterCommandCollectionId.VirtualMachineGroup)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_DeleteSavedState,
			CanExecuteDelegate = delegate
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				bool canExecute = false;
				virtualMachineGroup.ExecuteOnVmResource(delegate
				{
					canExecute = virtualMachineGroup.ApplicationVirtualMachineState == VirtualMachineState.Saved;
					SendCanExecuteUpdateIfNeeded(threadId);
				});
				return canExecute;
			},
			ExecuteDelegate = delegate
			{
				virtualMachineGroup.DeleteSavedState(askConfirmation: true);
			}
		};
	}
}

