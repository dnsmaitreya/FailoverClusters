using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Windows.Input;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;
using KDDSL.ServerClusters;

namespace FailoverClusters.Framework;

public class Checkpoint : IDataItem, INotifyCollectionChanged
{
	private VirtualMachineResource owner;

	private const string DeleteCheckpointConfirmationPrompt = "DeleteCheckpointConfirmationPrompt";

	private const string DeleteTreeCheckpointConfirmationPrompt = "DeleteTreeCheckpointConfirmationPrompt";

	private CommandCollection commandCollection;

	private ClusterCommand applyCheckpointCommand;

	private ClusterCommand deleteCheckpointCommand;

	private ClusterCommand renameCheckpointCommand;

	private ClusterCommand deleteTreeCheckpointCommand;

	public string Path { get; private set; }

	public string Name { get; set; }

	public ObservableCollection<Checkpoint> Children { get; private set; }

	public bool IsCurrentVirtualMachine { get; private set; }

	public Checkpoint Parent { get; set; }

	public VirtualMachineResource Owner
	{
		get
		{
			return owner;
		}
		private set
		{
			owner = value;
			foreach (Checkpoint child in Children)
			{
				child.owner = value;
			}
		}
	}

	public DateTime CreationTime { get; private set; }

	public string DisplayName
	{
		get
		{
			if (!IsCurrentVirtualMachine)
			{
				return Name;
			}
			return "Now";
		}
	}

	public IEnumerable<ICommand> Commands => GetCommands();

	public Guid Id { get; private set; }

	public event NotifyCollectionChangedEventHandler CollectionChanged;

	internal Checkpoint(ManagementObject virtualSystemSettingData, PVirtualMachineResource virtualMachineResource)
		: this(virtualSystemSettingData)
	{
		if (virtualMachineResource == null)
		{
			throw new ArgumentNullException("virtualMachineResource");
		}
		Owner = (VirtualMachineResource)virtualMachineResource.GetProxy();
	}

	internal Checkpoint(ManagementObject virtualSystemSettingData)
	{
		if (virtualSystemSettingData == null)
		{
			throw new ArgumentNullException("virtualSystemSettingData");
		}
		object obj = virtualSystemSettingData["VirtualSystemType"];
		object obj2 = virtualSystemSettingData["ElementName"];
		object obj3 = virtualSystemSettingData["CreationTime"];
		object obj4 = virtualSystemSettingData["ConfigurationID"];
		Path = virtualSystemSettingData.Path.ToString();
		IsCurrentVirtualMachine = obj?.ToString().Equals("Microsoft:Hyper-V:System:Realized", StringComparison.OrdinalIgnoreCase) ?? false;
		Name = ((obj2 != null) ? obj2.ToString() : string.Empty);
		CreationTime = ((obj3 != null) ? ManagementDateTimeConverter.ToDateTime(obj3.ToString()) : ManagementDateTimeConverter.ToDateTime(DateTime.Now.ToString(CultureInfo.CurrentCulture)));
		if (obj4 != null && Guid.TryParse(obj4.ToString(), out var result))
		{
			Id = result;
		}
		Children = new ObservableCollection<Checkpoint>();
	}

	private IEnumerable<ICommand> GetCommands()
	{
		return GenerateCheckpointCommands();
	}

	private CommandCollection GenerateCheckpointCommands()
	{
		if (commandCollection == null)
		{
			commandCollection = new CommandCollection(ClusterCommandCollectionId.Checkpoint);
			if (!IsCurrentVirtualMachine)
			{
				applyCheckpointCommand = new ClusterCommand(Owner, "ApplyCheckpoint", ClusterCommandId.VirtualMachineCheckpointApply, ClusterCommandCollectionId.Cluster, ApplyCheckpoint, (object obj) => true, this)
				{
					Text = CommandResources.VirtualMachineCheckpointApplyCommand_Text,
					Description = CommandResources.VirtualMachineCheckpointApplyCommand_Description
				};
				deleteCheckpointCommand = new ClusterCommand(Owner, "DeleteCheckpoint", ClusterCommandId.VirtualMachineCheckpointDelete, ClusterCommandCollectionId.Cluster, DeleteCheckpoint, (object obj) => true, this)
				{
					Text = CommandResources.VirtualMachineCheckpointDeleteCommand_Text,
					Description = CommandResources.VirtualMachineCheckpointDeleteCommand_Desciption
				};
				deleteTreeCheckpointCommand = new ClusterCommand(Owner, "DeleteTreeCheckpoint", ClusterCommandId.VirtualMachineCheckpointDeleteTree, ClusterCommandCollectionId.Cluster, DeleteCheckpointTree, (object obj) => true, this)
				{
					Text = CommandResources.VirtualMachineCheckpointDeleteTreeCommand_Text,
					Description = CommandResources.VirtualMachineCheckpointDeleteTreeCommand_Description
				};
				renameCheckpointCommand = new ClusterCommand(Owner, "RenameCheckpoint", ClusterCommandId.VirtualMachineCheckpointRename, ClusterCommandCollectionId.Cluster, RenameCheckpoint, (object obj) => true)
				{
					InputParameters = new InputParameterList<Checkpoint>(new Checkpoint[1] { this }),
					Text = CommandResources.VirtualMachineCheckpointRenameCommand_Text,
					Description = CommandResources.VirtualMachineCheckpointRenameCommand_Description
				};
				commandCollection.Add(applyCheckpointCommand);
				commandCollection.Add(deleteCheckpointCommand);
				commandCollection.Add(deleteTreeCheckpointCommand);
				commandCollection.Add(renameCheckpointCommand);
			}
		}
		return commandCollection;
	}

	private static void DeleteCheckpoint(object obj)
	{
		Checkpoint checkpoint = obj as Checkpoint;
		if (obj == null)
		{
			return;
		}
		if (UserSettings.GetBoolValue("DeleteCheckpointConfirmationPrompt"))
		{
			checkpoint.Owner.DeleteCheckpoint(checkpoint);
			return;
		}
		ConfirmationDialog confirmationDialog = new ConfirmationDialog
		{
			CustomIcon = InvariantResources.Warning,
			Caption = DialogResources.VirtualMachineDeleteCheckpoint_Title,
			Header = DialogResources.VirtualMachineDeleteCheckpoint_Header.FormatCurrentCulture(checkpoint.DisplayName),
			Footer = DialogResources.DoNotAskAgain_Footer
		};
		TaskDialogResult taskDialogResult = confirmationDialog.ShowDialog();
		if (taskDialogResult != TaskDialogResult.Yes)
		{
			_ = 4;
			return;
		}
		UserSettings.SetBoolValue("DeleteCheckpointConfirmationPrompt", confirmationDialog.IsFooterChecked);
		checkpoint.Owner.DeleteCheckpoint(checkpoint);
	}

	private static void DeleteCheckpointTree(object obj)
	{
		Checkpoint checkpoint = obj as Checkpoint;
		if (obj == null)
		{
			return;
		}
		if (UserSettings.GetBoolValue("DeleteTreeCheckpointConfirmationPrompt"))
		{
			checkpoint.Owner.DeleteCheckpointTree(checkpoint);
			return;
		}
		ConfirmationDialog confirmationDialog = new ConfirmationDialog
		{
			CustomIcon = InvariantResources.Warning,
			Caption = DialogResources.VirtualMachineDeleteCheckpointTree_Title,
			Header = DialogResources.VirtualMachineDeleteCheckpointTree_Header.FormatCurrentCulture(checkpoint.DisplayName),
			Footer = DialogResources.DoNotAskAgain_Footer
		};
		TaskDialogResult taskDialogResult = confirmationDialog.ShowDialog();
		if (taskDialogResult != TaskDialogResult.Yes)
		{
			_ = 4;
			return;
		}
		UserSettings.SetBoolValue("DeleteTreeCheckpointConfirmationPrompt", confirmationDialog.IsFooterChecked);
		checkpoint.Owner.DeleteCheckpointTree(checkpoint);
	}

	private static void ApplyCheckpoint(object obj)
	{
		if (!(obj is Checkpoint checkpoint))
		{
			return;
		}
		ConfirmationDialog confirmationDialog = new ConfirmationDialog();
		confirmationDialog.CustomIcon = InvariantResources.Warning;
		confirmationDialog.Caption = DialogResources.VirtualMachineApplyCheckpoint_Title;
		confirmationDialog.Header = DialogResources.VirtualMachineAplyCheckpoint_Header;
		confirmationDialog.Content = DialogResources.VirtualMachineApplyCheckpoint_Content;
		confirmationDialog.NoButtonText = DialogResources.VirtualMachineAplyCheckpoint_NoButton;
		confirmationDialog.YesButtonText = DialogResources.VirtualMachineAplyCheckpoint_YesButton;
		confirmationDialog.SetTaskDialogButtonStyle(new TaskDialogButtonsStyle(TaskDialogButtonsSettings.YesNoCancel, TaskDialogStandardButtons.Cancel));
		TaskDialogResult taskDialogResult = confirmationDialog.ShowDialog();
		if (taskDialogResult <= TaskDialogResult.No)
		{
			switch (taskDialogResult)
			{
			case TaskDialogResult.Yes:
				checkpoint.Owner.TakeCheckpoint();
				checkpoint.Owner.ApplyCheckpoint(checkpoint);
				break;
			case TaskDialogResult.No:
				checkpoint.Owner.ApplyCheckpoint(checkpoint);
				break;
			}
		}
		else if (taskDialogResult != TaskDialogResult.Cancel)
		{
			_ = 32;
		}
	}

	private static void RenameCheckpoint(object obj)
	{
		if (obj is RenameCheckpointParameters renameCheckpointParameters)
		{
			renameCheckpointParameters.Checkpoint.Owner.RenameCheckpoint(renameCheckpointParameters.Checkpoint, renameCheckpointParameters.NewCheckpointName);
		}
	}

	internal void OrderChilden()
	{
		Children = new ObservableCollection<Checkpoint>(Children.OrderByDescending((Checkpoint node) => node.CreationTime));
		foreach (Checkpoint child in Children)
		{
			child.OrderChilden();
		}
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, Children));
		}
	}
}

