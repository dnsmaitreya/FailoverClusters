using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FailoverClusters.UI.Common;
using WindowsAPICodePack.Dialogs;
using KDDSL.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class CsvVolumeResource : StorageResource
{
	private WeakReferenceEx moveClusterSharedVolumeWeak;

	private ClusterSharedVolumeFaultState? redirectedAccess;

	public ClusterSharedVolumeFaultState RedirectedAccess => LoadAsync<ClusterSharedVolumeFaultState, ClusterSharedVolumeFaultState>(redirectedAccess, 256);

	public override ClusterList<Resource> Children => WeakReferenceEx.ReturnInstance(ref childrenWeak, () => new ClusterList<Resource>(base.Cluster));

	public void GetVolumeInformation(Action<CsvVolumeInformation> callback)
	{
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			PCsvVolumeResource pCsvVolumeResource = (PCsvVolumeResource)lockObject.Owner;
			callback.SafeCall(pCsvVolumeResource.GetVolumeInformation());
		}, OperationType.Async, LockAccess.Reader);
	}

	public static void SetRedirectedAccess(Dictionary<CsvVolumeResource, RedirectedAccessData> csvVolumeResourcesPair, bool redirectedAccess, bool askConfirmation)
	{
		Exceptions.ThrowIfNull(csvVolumeResourcesPair, "csvVolumeResourcesPair");
		int count = csvVolumeResourcesPair.Count;
		if (count <= 0)
		{
			return;
		}
		if (askConfirmation)
		{
			ConfirmationDialog confirmationDialog = new ConfirmationDialog
			{
				Icon = TaskDialogStandardIcon.Question
			};
			if (redirectedAccess)
			{
				confirmationDialog.Caption = DialogResources.TurnOnRedirectedAccessForCsvMessage_Title;
				confirmationDialog.Header = DialogResources.TurnOnRedirectedAccessForCsvMessage_Header;
				confirmationDialog.Content = DialogResources.TurnOnRedirectedAccessForCsvMessage_Content.FormatCurrentCulture(count);
			}
			else
			{
				confirmationDialog.Caption = DialogResources.TurnOffRedirectedAccessForCsvMessage_Title;
				confirmationDialog.Header = DialogResources.TurnOffRedirectedAccessForCsvMessage_Header;
				confirmationDialog.Content = DialogResources.TurnOffRedirectedAccessForCsvMessage_Content.FormatCurrentCulture(count);
			}
			if (confirmationDialog.ShowDialog() != TaskDialogResult.Yes)
			{
				return;
			}
		}
		Resource.EnqueueAndThrottleRequests(from p in csvVolumeResourcesPair
			select p.Key into r
			where r != null
			select r, delegate(Resource resource, Action<OperationResult> result)
		{
			CsvVolumeResource csvVolumeResource = resource as CsvVolumeResource;
			if (csvVolumeResource != null && csvVolumeResourcesPair.ContainsKey(csvVolumeResource))
			{
				csvVolumeResource.SetRedirectedAccess(result, csvVolumeResourcesPair[csvVolumeResource].Volume, redirectedAccess);
			}
		});
	}

	protected override void InitializeResourceCommands(CommandCollection commandsCollection)
	{
		ClusterCommandContainer item = WeakReferenceEx.ReturnInstance(ref moveClusterSharedVolumeWeak, delegate
		{
			ClusterCommandContainer obj = new ClusterCommandContainer(this, "MoveClusterSharedVolume", ClusterCommandId.GroupMoveTo, commandsCollection.Category)
			{
				Text = CommandResources.MoveResourceAction_Text,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					throw new NotImplementedException("Missing implementation, the command should implement a custom callback registration.");
				},
				CommandParameter = base.OwnerGroup
			};
			ClusterSharedVolumeMoveCommand clusterSharedVolumeMoveCommand = new ClusterSharedVolumeMoveCommand(base.OwnerGroup, this);
			ClusterCommand item2 = clusterSharedVolumeMoveCommand.AverageGroupMoveToBestCommand;
			ClusterCommand item3 = clusterSharedVolumeMoveCommand.AverageGroupMoveToSelectedCommand;
			obj.ChildrenInternal.Add(item2);
			obj.ChildrenInternal.Add(item3);
			return obj;
		});
		commandsCollection.Add(item);
		base.InitializeResourceCommands(commandsCollection);
	}

	protected override void InitializeMoreActionsCommands(ClusterCommandContainer commandContainer)
	{
		base.InitializeMoreActionsCommands(commandContainer);
		using (IEnumerator<ICommand> enumerator = commandContainer.ChildrenInternal.GetEnumerator())
		{
			while (enumerator.MoveNext() && (!(enumerator.Current is ClusterCommand clusterCommand) || !(clusterCommand.Text == CommandResources.MaintenanceModeOff_Text)))
			{
			}
		}
		ClusterCommand clusterCommand2 = commandContainer.ChildrenInternal.Cast<ClusterCommand>().FirstOrDefault((ClusterCommand command) => command.Id == ClusterCommandId.StorageRepair);
		if (clusterCommand2 != null)
		{
			commandContainer.ChildrenInternal.Remove(clusterCommand2);
		}
		ClusterCommand clusterCommand3 = commandContainer.ChildrenInternal.Cast<ClusterCommand>().FirstOrDefault((ClusterCommand command) => command.Id == ClusterCommandId.ResourceSimulateFailure);
		if (clusterCommand3 != null)
		{
			commandContainer.ChildrenInternal.Remove(clusterCommand3);
		}
	}

	protected override string GetRemoveFromGroupCommandText()
	{
		return CommandResources.RemoveFromClusterSharedVolumeCommand_Text;
	}

	internal CsvVolumeResource(Cluster cluster)
		: base(cluster)
	{
	}

	internal void SetRedirectedAccess(Guid deviceId, bool csvRedirectedAccessMode, bool askConfirmation = false)
	{
		SetRedirectedAccess(base.SetLastError, deviceId, csvRedirectedAccessMode, askConfirmation);
	}

	internal void SetRedirectedAccess(Action<OperationResult> turnOnRedirectedAccessOperation, Guid deviceId, bool csvRedirectedAccessMode, bool askConfirmation = false)
	{
		if (csvRedirectedAccessMode && askConfirmation && CreateConfirmationDialog(DialogResources.StorageCSVRedirectedAccessOn_Title, DialogResources.StorageCSVRedirectedAccessOn_Header.FormatCurrentCulture(base.Name), DialogResources.StorageCSVRedirectedAccessOn_Content.FormatCurrentCulture(base.Name)).ShowDialog() != TaskDialogResult.Yes)
		{
			return;
		}
		this.ExecuteMethod(delegate(ILockable lockObject)
		{
			if (lockObject.Owner is PCsvVolumeResource pCsvVolumeResource)
			{
				pCsvVolumeResource.SetCsvRedirectedAccess(deviceId, csvRedirectedAccessMode);
			}
		}, turnOnRedirectedAccessOperation, LockAccess.Reader);
	}

	internal override void TransferInternalData(PClusterObject privateObject, bool subscribeToEvents, bool ignorePossibleOwners = false)
	{
		base.TransferInternalData(privateObject, subscribeToEvents, ignorePossibleOwners);
		SetRedirectedAccess(notify: false);
	}

	internal override bool ProcessPrivateEvent(object sender, ClusterWrapperEventArgs e, Queue<Action> queueOnDispatcher)
	{
		EventType eventType = e.EventType;
		if (eventType == EventType.ResourceStoragePropertiesChanged)
		{
			bool result = base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
			SetRedirectedAccess(notify: true);
			return result;
		}
		return base.ProcessPrivateEvent(sender, e, queueOnDispatcher);
	}

	private void SetRedirectedAccess(bool notify)
	{
		redirectedAccess = ClusterSharedVolumeFaultState.NoFaults;
		ClusterDisk diskInfo = base.DiskInfo;
		if (diskInfo != null)
		{
			if (diskInfo.Partitions.Any((ClusterDiskPartition partition) => partition.CsvFaultState == ClusterSharedVolumeFaultState.NoAccess))
			{
				redirectedAccess = ClusterSharedVolumeFaultState.NoAccess;
			}
			else if (diskInfo.Partitions.Any((ClusterDiskPartition partition) => partition.CsvFaultState == ClusterSharedVolumeFaultState.NoDirectIO))
			{
				redirectedAccess = ClusterSharedVolumeFaultState.NoDirectIO;
			}
			else if (diskInfo.Partitions.Any((ClusterDiskPartition partition) => partition.CsvFaultState == ClusterSharedVolumeFaultState.Dismounted))
			{
				redirectedAccess = ClusterSharedVolumeFaultState.Dismounted;
			}
		}
		if (notify)
		{
			UIHelper.ExecuteOnDispatcher(delegate
			{
				OnPropertyChanged("RedirectedAccess");
			}, OperationType.Async);
		}
	}

	protected override void OnRefresh(bool targeted)
	{
		redirectedAccess = null;
		base.OnRefresh(targeted);
	}
}

