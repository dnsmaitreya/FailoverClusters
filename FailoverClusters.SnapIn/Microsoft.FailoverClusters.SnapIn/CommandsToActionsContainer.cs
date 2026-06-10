using System.Collections.Generic;
using System.Windows.Input;
using FailoverClusters.Framework;
using ManagementConsole;

namespace FailoverClusters.SnapIn;

internal class CommandsToActionsContainer
{
	public IWpfViewAdapter ViewAdapter { get; private set; }

	public List<IDataItem> SelectedItems { get; private set; }

	public List<ICommand> SelectedItemsCommands { get; private set; }

	public SelectionData SelectionData { get; private set; }

	public string HelpTopic { get; private set; }

	public CommandsToActionsContainer(IWpfViewAdapter viewAdapter, List<IDataItem> selectedItems, List<ICommand> selectedItemsCommands, SelectionData selectionData, string helpTopic)
	{
		ViewAdapter = viewAdapter;
		SelectedItems = selectedItems;
		SelectedItemsCommands = selectedItemsCommands;
		SelectionData = selectionData;
		HelpTopic = helpTopic;
	}
}

