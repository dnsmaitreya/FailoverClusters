using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal static class AddStorage
{
	internal delegate void AddDiskDelegate(ClusterResource disk);

	internal static ICollection<ClusterResource> InvokeDialog(Cluster cluster, StorageType type, INotifyUser notifyUser, CluadminWaitDialog waitDialog)
	{
		return InvokeDialog(cluster, type, null, notifyUser, waitDialog);
	}

	internal static ICollection<ClusterResource> InvokeDialog(Cluster cluster, StorageType type, string title, INotifyUser notifyUser, CluadminWaitDialog waitDialog)
	{
		ClusterResourceCollection availableDisks = null;
		List<StorageListItem> disks = null;
		try
		{
			waitDialog.ShowDialog(notifyUser, delegate
			{
				availableDisks = cluster.GetAvailableStorage();
				disks = new List<StorageListItem>();
				foreach (ClusterResource item in availableDisks)
				{
					if (item.State == ResourceState.Online)
					{
						bool flag = false;
						ClusterDisk clusterDisk = item.Storage_GetDiskInfo(includeMountPoints: false);
						if (clusterDisk != null && clusterDisk.PartitionCount > 0)
						{
							flag = clusterDisk.Partitions.Any((ClusterDiskPartition partition) => partition.IsFormatted);
						}
						if (flag || (type & StorageType.NonFormatted) == StorageType.NonFormatted)
						{
							disks.Add(StorageListItem.Create(item, (IDisposable)null, (ClusterListItemChildContext)1));
						}
					}
				}
				disks = disks.OrderBy((StorageListItem n) => ((ResourceListItem)n).Resource.DisplayName, new ClusterHelp.CompareString()).ToList();
			});
			if (waitDialog.IsCanceled)
			{
				return null;
			}
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.CannotFindAvailableStorage_Text);
			return null;
		}
		if (availableDisks.Count == 0)
		{
			notifyUser.ShowError(Resources.NoAvailableDisks_Text);
			return null;
		}
		AddStorageDialog addStorageDialog = new AddStorageDialog(disks, title);
		try
		{
			if (notifyUser.ShowDialog((Form)(object)addStorageDialog) == DialogResult.OK)
			{
				return addStorageDialog.GetSelectedDisks();
			}
			return null;
		}
		finally
		{
			((IDisposable)addStorageDialog)?.Dispose();
		}
	}

	internal static void AddSelectedStorage(ICollection<ClusterResource> selectedStorage, AddDiskDelegate addDiskDelegate, INotifyUser notifyUser, CluadminWaitDialog waitDialog)
	{
		waitDialog.DisplayDelay = new TimeSpan(0L);
		waitDialog.ShowDialog(notifyUser, (BackgroundWaitDialogOperation<ICollection<ClusterResource>, object>)delegate(CluadminWaitDialog diag, ICollection<ClusterResource> selectedDisks)
		{
			foreach (ClusterResource selectedDisk in selectedDisks)
			{
				diag.SetStatusText(Resources.AddingDiskFormatString_Text, selectedDisk.Name);
				addDiskDelegate(selectedDisk);
			}
			return null;
		}, selectedStorage);
	}

	internal static void AddStorageToCsv(ICollection<ClusterResource> selectedStorage)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Invalid comparison between Unknown and I4
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (selectedStorage == null)
		{
			throw new ArgumentNullException("selectedStorage");
		}
		INotifyUser notifyUser = ClusterAdministrator.NotifyUser;
		ReportLevel val = (ReportLevel)1;
		ReportBuilder val2 = ReportBuilder.CreateXmlReportBuilder(Path.GetTempFileName());
		try
		{
			ReportChannel report = val2.PrimaryChannel;
			report.ReportItem((CommonReportItem)23, Resources.AddClusterSharedVolumesReportTitle_Text);
			report.InsertImage(Icons.SharedClusterVolume);
			try
			{
				CluadminWaitDialog waitDialog = CluadminWaitDialog.Create(Resources.AddClusterSharedVolumesReportTitle_Text, "");
				try
				{
					waitDialog.DisplayDelay = new TimeSpan(0L);
					waitDialog.ShowDialog(notifyUser, (BackgroundWaitDialogOperation<ICollection<ClusterResource>, object>)delegate(CluadminWaitDialog diag, ICollection<ClusterResource> selectedDisks)
					{
						foreach (ClusterResource selectedDisk in selectedDisks)
						{
							waitDialog.SetStatusText(Resources.AddingDiskFormatString_Text, selectedDisk.Name);
							if (selectedDisk.State == ResourceState.Online && selectedDisk.PhysicalDisk_HasMountPoints())
							{
								report.ReportWarn(Resources.AddClusterSharedVolumesHasMountPointsFormat_Text, new object[1] { selectedDisk.DisplayName });
							}
							AddDiskToClusterSharedVolumes(selectedDisk, report);
						}
						return null;
					}, selectedStorage);
				}
				finally
				{
					if (waitDialog != null)
					{
						((IDisposable)waitDialog).Dispose();
					}
				}
			}
			finally
			{
				val = report.ReportLevel;
				report.Close();
			}
		}
		finally
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.htm", Path.GetTempFileName());
			val2.Close();
			XmlReportRenderer.TransformStandardHtmlReport(val2.ReportFile, text);
			if ((int)val != 1)
			{
				ShowAddSharedVolumeReport(notifyUser, val, text);
			}
			try
			{
				File.Delete(val2.ReportFile);
			}
			catch (IOException caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error deleting {0}", val2.ReportFile);
			}
		}
	}

	private static void ShowAddSharedVolumeReport(INotifyUser notifyUser, ReportLevel reportLevel, string reportFileName)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		string text = string.Empty;
		if ((int)reportLevel == 4)
		{
			text = Resources.AddClusterSharedVolumesErrors_Text;
		}
		else if ((int)reportLevel == 2)
		{
			text = Resources.AddClusterSharedVolumesWarnings_Text;
		}
		if (notifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button1, text) == DialogResult.Yes)
		{
			ReportViewer.LaunchReportViewer(reportFileName);
		}
	}

	internal static void AddDiskToClusterSharedVolumes(ClusterResource disk, ReportChannel report)
	{
		try
		{
			disk.Cluster.AddStorageToClusterSharedVolumes(disk);
			report.ReportInfo(Resources.AddClusterSharedVolumeAddedFormat_Text, new object[1] { disk.DisplayName });
		}
		catch (ApplicationException ex)
		{
			ExceptionHelp.LogException(ex, "There was a failure adding the disk resource '{0}' to cluster shared volumes.", disk.DisplayName);
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null)
			{
				if (firstException.NativeErrorCode == -2147023899)
				{
					report.ReportWarn(Resources.AddClusterSharedVolumeAddFailedToComeOnlineFormat_Text, new object[1] { disk.DisplayName });
				}
				else
				{
					report.ReportFail(ExceptionHelp.GetExceptionReportMessage(ex, Resources.AddClusterSharedVolumeAddFailedFormat_Text, disk.DisplayName));
				}
			}
			else
			{
				report.ReportFail(ExceptionHelp.GetExceptionReportMessage(ex, Resources.AddClusterSharedVolumeAddFailedFormat_Text, disk.DisplayName));
			}
		}
	}
}
