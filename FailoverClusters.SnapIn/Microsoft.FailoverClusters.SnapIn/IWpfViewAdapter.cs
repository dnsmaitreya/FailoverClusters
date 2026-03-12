using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.FailoverClusters.ClusterSnapIn;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.SnapIn;

public interface IWpfViewAdapter
{
	bool StatusBarStarted { get; set; }

	event EventHandler Show;

	event EventHandler Hide;

	ViewModelBase SetupViewModel(ViewModelData viewModelData);

	IEnumerable<IDataItem> ApplyFilterOnSelectedItems(IEnumerable<IDataItem> originalSetOfSelectedItems);

	void RecursivelyProcessCommands(IEnumerable<ICommand> commandsCollection, bool rootLevel);
}
