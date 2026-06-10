using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class DeleteCommand : ClusterCommandBase<DeleteCommand>
{
	protected override ClusterCommand CreateInstance(Group group, ClusterCommandCollectionId id)
	{
		return new ClusterCommand(group, "Delete", ClusterCommandId.GroupDelete, id)
		{
			Text = CommandResources.RemoveCommand_Text,
			CanExecuteDelegate = (object x) => true,
			ExecuteDelegate = delegate
			{
				group.Delete(askConfirmation: true);
			}
		};
	}
}

