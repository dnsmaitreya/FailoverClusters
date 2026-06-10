using System.Threading;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachinePauseCommand : VirtualMachineMoreActionsCommandBase
{
	public VirtualMachinePauseCommand(VirtualMachineGroup group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		VirtualMachineGroup virtualMachineGroup = (VirtualMachineGroup)base.ClusterGroup;
		return new ClusterCommand(virtualMachineGroup, "Pause", ClusterCommandId.VirtualMachineGroupPause, ClusterCommandCollectionId.VirtualMachineGroup)
		{
			Text = CommandResources.PauseAction_Text,
			CanExecuteDelegate = delegate
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				bool canExecute = false;
				virtualMachineGroup.ExecuteOnVmResource(delegate
				{
					canExecute = virtualMachineGroup.ApplicationVirtualMachineState == VirtualMachineState.Running;
					SendCanExecuteUpdateIfNeeded(threadId);
				});
				return canExecute;
			},
			ExecuteDelegate = delegate
			{
				virtualMachineGroup.Pause();
			}
		};
	}
}

