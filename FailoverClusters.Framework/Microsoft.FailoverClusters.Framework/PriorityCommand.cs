using System;
using System.Linq;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class PriorityCommand : GroupCommandBase
{
	public PriorityCommand(Group group)
		: base(group)
	{
	}

	protected override ClusterCommand GenerateInstance()
	{
		return new ClusterCommand(base.ClusterGroup, "Priority", ClusterCommandId.GroupPriority, ClusterCommandCollectionId.GroupPriority)
		{
			InputParameters = new InputParameterList<Priority>(base.ClusterGroup.Priority.GetFilterableValues().Cast<Priority>()),
			Text = "{0}",
			CanExecuteDelegate = delegate(object newPriority)
			{
				if (newPriority == null)
				{
					return false;
				}
				if (!(newPriority is Priority))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotPriorityCommand);
				}
				return base.ClusterGroup.Priority != (Priority)newPriority;
			},
			ExecuteDelegate = delegate(object newPriority)
			{
				Exceptions.ThrowIfNull(newPriority, "priority", ExceptionResources.ArgumentNull_PriorityCommand);
				if (!(newPriority is Priority))
				{
					throw new InvalidOperationException(ExceptionResources.InvalidOperation_IsNotPriorityCommand);
				}
				base.ClusterGroup.Priority = (Priority)newPriority;
			}
		};
	}

	protected override void DoPostGenerateInstance(ClusterCommand newValue)
	{
		base.ClusterGroup.PriorityChanged += base.UpdateCanExecute;
		newValue.Finalizing += delegate
		{
			base.ClusterGroup.PriorityChanged -= base.UpdateCanExecute;
			ReleaseReference();
		};
		base.DoPostGenerateInstance(newValue);
	}
}

