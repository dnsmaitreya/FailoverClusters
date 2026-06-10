using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.WinForms;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class StorageResourceContext : StorageResourceContextBase
{
	private struct SetDriveLetterArgs
	{
		public readonly uint PartitionNumber;

		public readonly uint DriveLetterMask;

		public SetDriveLetterArgs(uint partitionNumber, uint driveLetterMask)
		{
			PartitionNumber = partitionNumber;
			DriveLetterMask = driveLetterMask;
		}
	}

	private enum MaintenanceModeResult
	{
		QuorumResource,
		Success
	}

	internal StorageResourceContext(ClusterResource resource)
		: base(resource)
	{
		base.Resource.Cluster.PropertiesChanged += ClusterPropertiesChanged;
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			base.Resource.Cluster.PropertiesChanged -= ClusterPropertiesChanged;
			GC.SuppressFinalize(this);
		}
	}

	private void ClusterPropertiesChanged(object sender, EventArgs e)
	{
		actionsAreDirty = true;
		OnActionsUpdated();
	}

	private ActionsPaneItem CreateChangeDriveLetterActionsPaneItem()
	{
		ICollection<ClusterDiskPartition> collection = null;
		if (base.Resource.State == ResourceState.Online)
		{
			try
			{
				collection = base.Resource.Storage_GetDiskInfo(includeMountPoints: false).Partitions;
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, string.Format(CultureInfo.CurrentCulture, "Failed to get partitions information for disk '{0}'", base.Resource.DisplayName));
			}
		}
		if (collection == null || collection.Count == 0)
		{
			return ActionFactory.CreateDisabledAction(StringExtensions.ReplaceAccelerator(CommandResources.ChangeDriveLetterAction_Text), Resources.ChangeDriveLetterActionDescription_Text, Icons.PhysicalDiskIndex);
		}
		if (collection.Count == 1)
		{
			using IEnumerator<ClusterDiskPartition> enumerator = collection.GetEnumerator();
			if (enumerator.MoveNext())
			{
				ClusterDiskPartition current = enumerator.Current;
				return ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ChangeDriveLetterAction_Text), Resources.ChangeDriveLetterActionDescription_Text, Icons.PhysicalDiskIndex, OnChangeDriveLetter, current);
			}
		}
		ActionGroup actionGroup = ActionFactory.CreateActionGroup(StringExtensions.ReplaceAccelerator(CommandResources.ChangeDriveLetterAction_Text), Resources.ChangeDriveLetterActionDescription_Text, Icons.PhysicalDiskIndex);
		foreach (ClusterDiskPartition item in collection)
		{
			actionGroup.Items.Add(ActionFactory.CreateAction(string.Format(CultureInfo.CurrentCulture, StringExtensions.ReplaceAccelerator(CommandResources.ChangeVolumeDriveLetterAction_Text), item.Name), Resources.ChangeDriveLetterActionDescription_Text, Icons.PhysicalDiskIndex, OnChangeDriveLetter, item));
		}
		ActionFactory.AssignAscendingMnemonics(actionGroup);
		return actionGroup;
	}

	private void OnChangeDriveLetter(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		try
		{
			ClusterGroup clusterGroup = null;
			uint availableDriveLettersMask;
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(StringExtensions.RemoveAccelerator(CommandResources.ChangeDriveLetterAction_Text), Resources.LookingForAvailableDriveLetters_Text))
			{
				try
				{
					availableDriveLettersMask = cluadminWaitDialog.ShowDialog<object, uint>(notifyUserFromSender, GetAvailableDriveLetters, null);
					clusterGroup = cluadminWaitDialog.ShowDialog(notifyUserFromSender, (CluadminWaitDialog _003Cp0_003E, object _003Cp1_003E) => base.Resource.GetOwnerGroup(), null);
				}
				catch (ClusterAvailableDriveLettersNodeDownException ex)
				{
					notifyUserFromSender.ShowError((Exception)ex);
					return;
				}
				if (cluadminWaitDialog.IsCanceled)
				{
					return;
				}
			}
			ClusterDiskPartition clusterDiskPartition = (ClusterDiskPartition)ActionData.GetActionTag(e.Action);
			DriveLetterDialog driveLetterDialog = new DriveLetterDialog(clusterGroup, clusterDiskPartition.DriveLetterMask, availableDriveLettersMask);
			if (notifyUserFromSender.ShowDialog((Form)(object)driveLetterDialog) != DialogResult.OK)
			{
				return;
			}
			using CluadminWaitDialog cluadminWaitDialog2 = CluadminWaitDialog.Create(StringExtensions.RemoveAccelerator(CommandResources.ChangeDriveLetterAction_Text), Resources.ChangingDriveLetter_Text);
			cluadminWaitDialog2.ShowDialog(notifyUserFromSender, SetDriveLetter, new SetDriveLetterArgs(clusterDiskPartition.PartitionNumber, driveLetterDialog.DriveLetterMask));
		}
		catch (Exception ex2)
		{
			notifyUserFromSender.ShowError(ex2, Resources.FailureChangingDriveLetter_Text);
			ExceptionHelp.LogException(ex2, "Error changing drive letter");
		}
	}

	private uint GetAvailableDriveLetters(CluadminWaitDialog waitDialog, object argument)
	{
		return ClusterUtilities.GetAvailableDriveLettersMask(base.Resource.Cluster);
	}

	private object SetDriveLetter(CluadminWaitDialog waitDialog, SetDriveLetterArgs args)
	{
		base.Resource.Storage_SetDriveLetter(args.PartitionNumber, args.DriveLetterMask);
		return null;
	}

	protected override PropertyPageCollection GetPropertyPages()
	{
		PropertyPageCollection propertyPages = base.GetPropertyPages();
		AddShadowCopyPropertyPage(propertyPages);
		return propertyPages;
	}

	protected virtual void AddShadowCopyPropertyPage(PropertyPageCollection pages)
	{
		try
		{
			ClusterSnapinPropertyPage clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
			clusterSnapinPropertyPage.SetControl(new ShadowCopyPropertiesPage((FailoverClusters.Framework.Cluster)(object)base.Cluster.FrameworkCluster, Id));
			pages.Add(clusterSnapinPropertyPage);
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, string.Format(CultureInfo.CurrentCulture, "Failed to create Shadow Copy Volume property page for resource '{0}'", base.Resource.DisplayName));
		}
	}

	protected override ActionsPaneItemCollection CreateResourceSpecificActions()
	{
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		ClusterGroup ownerGroup = base.Resource.GetOwnerGroup();
		GroupType groupType = ownerGroup.GroupType;
		actionsPaneItemCollection.AddRange(CreateOnlineOfflineActions().ToArray());
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[2]
		{
			new ActionSeparator(),
			CreateChangeDriveLetterActionsPaneItem()
		});
		base.GroupName = ownerGroup.Name;
		if (groupType != GroupType.CoreCluster)
		{
			actionsPaneItemCollection.Add(new ActionSeparator());
			string text = CommandResources.RemoveCommand_Text;
			if (groupType != GroupType.AvailableStorage)
			{
				text = string.Format(CultureInfo.CurrentCulture, CommandResources.RemoveFromGroupActionFormat_Text, base.GroupName);
			}
			actionsPaneItemCollection.Add(ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(text), Resources.RemoveStorageActionDescription_Text, Icons.RemoveIndex, OnRemoveStorage));
		}
		if (groupType == GroupType.AvailableStorage)
		{
			actionsPaneItemCollection.Add(new ActionSeparator());
			actionsPaneItemCollection.Add(ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.AddClusterSharedVolumesAction_Text), CommandResources.AddClusterSharedVolumesActionDescription_Text, Icons.NewStorageIndex, OnAddToClusterSharedVolumes));
		}
		return actionsPaneItemCollection;
	}

	protected override void PerformRemoveStorage(CluadminWaitDialog waitDialog)
	{
		GroupType groupType = base.Resource.GetOwnerGroup().GroupType;
		if (groupType != GroupType.AvailableStorage)
		{
			base.Resource.Cluster.MoveStorageToAvailableStorage(base.Resource);
		}
		else if (groupType == GroupType.ClusterSharedVolume)
		{
			base.Resource.Cluster.RemoveStorageFromClusterSharedVolumes(base.Resource);
		}
		else
		{
			base.Resource.Delete("snapin!StorageResourceContext.PerformRemoveStorage");
		}
	}

	protected override void OnRemoveStorage(object sender, SnapinActionEventArgs e)
	{
		string resName = Resources.Unavailable_Text;
		string confirmationMessage = Resources.RemoveStorageConfirm_Text;
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.RetrievingResourceName_Text);
		using (cluadminWaitDialog)
		{
			cluadminWaitDialog.DisplayDelay = TimeSpan.Zero;
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				resName = base.Resource.DisplayName;
				if (base.Resource.GetOwnerGroup().GroupType == GroupType.AvailableStorage)
				{
					confirmationMessage = Resources.Resource_Delete_Confirm_Text;
				}
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		PerformRemoveStorageAction(sender, e, string.Format(CultureInfo.CurrentCulture, confirmationMessage, resName));
	}

	private void OnAddToClusterSharedVolumes(object sender, SnapinActionEventArgs e)
	{
		AddStorage.AddStorageToCsv(new List<ClusterResource> { base.Resource });
	}

	protected override ActionsPaneItemCollection CreateResourceSpecificMoreActions()
	{
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		ActionBase actionBase = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.RepairPhysicalDiskAction_Text), Resources.RepairPhysicalDiskActionDescription_Text, Icons.BringResourceOnlineIndex, OnRepairPhysicalDisk);
		actionBase.Enabled = GetRepairPhysicalDiskActionEnabledState();
		actionsPaneItemCollection.Add(actionBase);
		actionsPaneItemCollection.Add(new ActionSeparator());
		actionsPaneItemCollection.AddRange(base.CreateResourceSpecificMoreActions().ToArray());
		if (base.Resource.IsResourceOfType(WellKnownResourceType.PhysicalDisk) && base.Resource.State == ResourceState.Online && !base.Resource.IsQuorumResource)
		{
			ActionBase item = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(base.Resource.PhysicalDisk_IsMaintenanceModeOn() ? CommandResources.MaintenanceModeOff_Text : CommandResources.MaintenanceModeOn_Text), Resources.MaintenanceDescription_Text, Icons.PhysicalDiskIndex, OnMaintenanceMode);
			actionsPaneItemCollection.Add(new ActionSeparator());
			actionsPaneItemCollection.Add(item);
		}
		return actionsPaneItemCollection;
	}

	internal bool GetRepairPhysicalDiskActionEnabledState()
	{
		if (base.Resource.State != ResourceState.Offline)
		{
			return base.Resource.State == ResourceState.Failed;
		}
		return true;
	}

	private void OnRepairPhysicalDisk(object sender, SnapinActionEventArgs e)
	{
		RepairPhysicalDisk(ActionData.GetNotifyUserFromSender(sender));
	}

	internal void RepairPhysicalDisk(INotifyUser notifyUser)
	{
		ClusterableDisks clusterableDisks = AddDiskDialog.RepairDiskDialog(base.Resource.Cluster, notifyUser, base.DisplayName);
		if (clusterableDisks == null || clusterableDisks.AvailableDisks.Count == 0)
		{
			return;
		}
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(string.Format(CultureInfo.CurrentCulture, Resources.RepairDiskWaitDialogTitleFormat_Text, base.DisplayName), Resources.RepairDiskWaitDialogInitialState_Text))
		{
			cluadminWaitDialog.DisplayDelay = new TimeSpan(0L);
			cluadminWaitDialog.ShowDialog(notifyUser, delegate
			{
				foreach (ClusterDisk availableDisk in clusterableDisks.AvailableDisks)
				{
					availableDisk.ConfigureDiskResource(base.Resource);
				}
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		notifyUser.ShowInformational(string.Format(CultureInfo.CurrentCulture, Resources.RepairDiskCompleteMessageFormat_Text, base.DisplayName));
	}

	private void OnMaintenanceMode(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		try
		{
			CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(e.Action.DisplayName);
			cluadminWaitDialog.CanCancel = true;
			MaintenanceModeResult maintenanceModeResult;
			using (cluadminWaitDialog)
			{
				maintenanceModeResult = cluadminWaitDialog.ShowDialog<object, MaintenanceModeResult>(notifyUserFromSender, PerformMaintenanceMode, null);
			}
			if (!cluadminWaitDialog.IsCanceled && maintenanceModeResult == MaintenanceModeResult.QuorumResource)
			{
				notifyUserFromSender.ShowError(Resources.MaintenanceMode_QuorumResource_Text, new object[1] { base.DisplayName });
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error setting maintenance mode");
			notifyUserFromSender.ShowError(ex, Resources.FailureSettingMaintenanceMode_Text);
		}
	}

	private MaintenanceModeResult PerformMaintenanceMode(CluadminWaitDialog waitDialog, object data)
	{
		if (base.Resource.IsQuorumResource)
		{
			return MaintenanceModeResult.QuorumResource;
		}
		if (base.Resource.PhysicalDisk_IsMaintenanceModeOn())
		{
			waitDialog.SetStatusText(Resources.DisablingMaintenanceMode_Text, base.DisplayName);
			base.Resource.PhysicalDisk_DisableMaintenanceMode();
			base.Resource.PhysicalDisk_WaitForMaintenanceModeExit();
		}
		else
		{
			waitDialog.SetStatusText(Resources.EnableMaintenanceMode_Text, base.DisplayName);
			base.Resource.PhysicalDisk_EnableMaintenanceMode();
		}
		return MaintenanceModeResult.Success;
	}
}

