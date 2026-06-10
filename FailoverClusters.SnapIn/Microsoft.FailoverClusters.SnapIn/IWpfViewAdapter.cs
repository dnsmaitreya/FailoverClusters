using System;
using System.Collections.Generic;
using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;

namespace FailoverClusters.SnapIn;

public interface IWpfViewAdapter
{
	bool StatusBarStarted { get; set; }

	event EventHandler Show;

	event EventHandler Hide;

	ViewModelBase SetupViewModel(ViewModelData viewModelData);

	IEnumerable<IDataItem> ApplyFilterOnSelectedItems(IEnumerable<IDataItem> originalSetOfSelectedItems);

	void RecursivelyProcessCommands(IEnumerable<ICommand> commandsCollection, bool rootLevel);
}

