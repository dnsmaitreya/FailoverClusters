using System.Threading;

namespace FailoverClusters.Framework;

public abstract class VirtualMachineMoreActionsCommandBase : GroupCommandBase
{
	protected VirtualMachineMoreActionsCommandBase(VirtualMachineGroup group)
		: base(group)
	{
		base.UpdateCanExecuteOnApplicationStatusChange = true;
	}

	protected void SendCanExecuteUpdateIfNeeded()
	{
		SendCanExecuteUpdateIfNeeded(Thread.CurrentThread.ManagedThreadId);
	}

	protected void SendCanExecuteUpdateIfNeeded(int sourceThreadId)
	{
		if (sourceThreadId == Thread.CurrentThread.ManagedThreadId)
		{
			return;
		}
		ClusterCommand commandInner = TryGetInstance();
		if (commandInner != null)
		{
			UIHelper.ExecuteOnDispatcher(delegate
			{
				commandInner.CanExecuteUpdate(base.ClusterGroup, new ClusterEventArgs(base.ClusterGroup.Id, null));
			}, OperationType.Async);
		}
	}
}

