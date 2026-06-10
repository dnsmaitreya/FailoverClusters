using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using FailoverClusters.WinForms;
using ManagementConsole;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal class UiActionProducer : IUiActionProducer
{
	private readonly SelectedClusterObjectsToTextConverter selectedClusterObjectsToTextConverter;

	private readonly object pendingActionsLock = new object();

	private const int sleepInterval = 100;

	private Stack<CommandsToActionsContainer> actionsStack = new Stack<CommandsToActionsContainer>();

	public UiActionProducer()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		selectedClusterObjectsToTextConverter = new SelectedClusterObjectsToTextConverter();
		Start();
	}

	public void Enqueue(CommandsToActionsContainer commandsToActionsContainer)
	{
		lock (pendingActionsLock)
		{
			actionsStack.Push(commandsToActionsContainer);
		}
	}

	private void ProcessPendingActions(CommandsToActionsContainer commandsToActionsContainer)
	{
		List<ICommand> selectedItemsCommands = commandsToActionsContainer.SelectedItemsCommands;
		List<IDataItem> list = (commandsToActionsContainer.ViewAdapter.ApplyFilterOnSelectedItems(commandsToActionsContainer.SelectedItems) ?? Enumerable.Empty<IDataItem>()).ToList();
		string text = (string)selectedClusterObjectsToTextConverter.Convert(new object[1] { list }, typeof(string), (object)null, CultureInfo.CurrentCulture);
		SelectionData selectionData = commandsToActionsContainer.SelectionData;
		ClusterAdministrator.SetStatusBarMessage(commandsToActionsContainer.ViewAdapter, text);
		lock (selectionData)
		{
			try
			{
				selectionData.BeginUpdates();
			}
			catch (Exception)
			{
				return;
			}
			try
			{
				selectionData.Clear();
				selectionData.HelpTopic = commandsToActionsContainer.HelpTopic;
				if (selectedItemsCommands.Count > 0)
				{
					selectionData.ActionsPaneItems.Clear();
					selectionData.DisplayName = text;
					commandsToActionsContainer.ViewAdapter.RecursivelyProcessCommands(selectedItemsCommands, rootLevel: true);
					if (list.Count == 1)
					{
						UpdateSelectionDataOnSingleItemSelection(selectionData, list[0]);
					}
					else
					{
						selectionData.Update(null, multiSelection: true, null, null);
					}
				}
			}
			finally
			{
				selectionData.EndUpdates();
			}
		}
	}

	private void UpdateSelection<T>(SelectionData selectionData, T clusterObject, Func<Guid[]> nodeTypes, Func<T, WritableSharedData> sharedData) where T : ClusterObject
	{
		if (!((ClusterObject)clusterObject == (ClusterObject)null))
		{
			selectionData.Update(clusterObject, multiSelection: false, nodeTypes(), sharedData(clusterObject));
		}
	}

	private void UpdateSelectionDataOnSingleItemSelection(SelectionData selectionData, IDataItem firstSelectedItem)
	{
		UpdateSelection(selectionData, firstSelectedItem as Resource, FailoverClusters.WinForms.ResourceContext.GetNodeTypes, FailoverClusters.WinForms.ResourceContext.GetSharedData);
		UpdateSelection(selectionData, firstSelectedItem as Group, FailoverClusters.WinForms.GroupContext.GetNodeTypes, FailoverClusters.WinForms.GroupContext.GetSharedData);
		UpdateSelection(selectionData, firstSelectedItem as Network, FailoverClusters.WinForms.NetworkContext.GetNodeTypes, FailoverClusters.WinForms.NetworkContext.GetSharedData);
		selectionData.Update(null, multiSelection: false, null, null);
	}

	private void Start()
	{
		Worker.Start(delegate
		{
			Thread.CurrentThread.Name = "UI Framework Action Producer";
			while (Thread.CurrentThread.ThreadState == ThreadState.Background && !Global.IsProcessShuttingDown && Thread.CurrentThread.ThreadState == ThreadState.Background)
			{
				try
				{
					CommandsToActionsContainer commandsToActionsContainer = null;
					lock (pendingActionsLock)
					{
						if (actionsStack.Count > 0)
						{
							commandsToActionsContainer = actionsStack.Pop();
							actionsStack.Clear();
						}
					}
					if (commandsToActionsContainer != null)
					{
						ProcessPendingActions(commandsToActionsContainer);
					}
					else
					{
						Thread.Sleep(100);
					}
				}
				catch (ThreadAbortException)
				{
				}
				catch (ClusterException)
				{
				}
			}
		});
	}
}

