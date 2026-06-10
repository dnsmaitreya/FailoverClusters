using System.Threading;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class VirtualMachineResumeCommand : VirtualMachineMoreActionsCommandBase
{
	public VirtualMachineResumeCommand(VirtualMachineGroup group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		VirtualMachineGroup virtualMachineGroup = (VirtualMachineGroup)base.ClusterGroup;
		return new ClusterCommand(virtualMachineGroup, "ResumeSavedState", ClusterCommandId.VirtualMachineGroupResume, ClusterCommandCollectionId.VirtualMachineGroup)
		{
			Text = EnumResources.GroupState_VirtualMachine_Set_Resume,
			CanExecuteDelegate = delegate
			{
				int threadId = Thread.CurrentThread.ManagedThreadId;
				bool canExecute = true;
				virtualMachineGroup.ExecuteOnVmResource(delegate
				{
					canExecute = virtualMachineGroup.ApplicationVirtualMachineState == VirtualMachineState.Paused;
					SendCanExecuteUpdateIfNeeded(threadId);
				});
				return canExecute;
			},
			ExecuteDelegate = delegate
			{
				virtualMachineGroup.Resume();
			}
		};
	}
}

