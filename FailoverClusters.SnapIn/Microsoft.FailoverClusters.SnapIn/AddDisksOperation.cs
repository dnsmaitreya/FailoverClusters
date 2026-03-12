using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.SnapIn;

internal class AddDisksOperation
{
	private class CreateDiskResourcesArgs
	{
		public readonly string ReportFileName;

		public readonly ClusterableDisks ClusterableDisks;

		public CreateDiskResourcesArgs(string reportFileName, ClusterableDisks clusterableDisks)
		{
			ReportFileName = reportFileName;
			ClusterableDisks = clusterableDisks;
		}
	}

	private CluadminWaitDialog addDiskWaitDialog;

	private bool addDisksErrors;

	private ReportChannel report;

	private Cluster Cluster { get; set; }

	private Guid FilteringPoolId { get; set; }

	internal AddDisksOperation(Guid clusterId, string resourceName, Guid filteringPoolId)
	{
		ClusterResource legacyResource = LegacyFactory.GetLegacyResource(clusterId, resourceName);
		Cluster = legacyResource.Cluster;
		FilteringPoolId = filteringPoolId;
	}

	internal AddDisksOperation(Cluster cluster)
	{
		Cluster = cluster;
		FilteringPoolId = Guid.Empty;
	}

	public void Execute()
	{
		ClusterableDisks clusterableDisks = AddDiskDialog.AddDisksDialog(Cluster, ClusterAdministrator.NotifyUser, FilteringPoolId);
		if (clusterableDisks == null || clusterableDisks.AvailableDisks.Count == 0)
		{
			return;
		}
		string reportFileName = string.Format(CultureInfo.InvariantCulture, "{0}.htm", Path.GetTempFileName());
		addDiskWaitDialog = CluadminWaitDialog.Create(Resources.CreatingDiskResources_Text, string.Empty);
		addDisksErrors = false;
		addDiskWaitDialog.DisplayDelay = new TimeSpan(0L);
		using (addDiskWaitDialog)
		{
			clusterableDisks.ContinueCreateDisksOnError = true;
			clusterableDisks.CreateDiskResourcesProgress += AddSelectedDisksProgress;
			CreateDiskResourcesArgs data = new CreateDiskResourcesArgs(reportFileName, clusterableDisks);
			addDiskWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, CreateDiskResources, data);
			if (addDiskWaitDialog.IsCanceled)
			{
				return;
			}
		}
		addDiskWaitDialog = null;
		if (addDisksErrors)
		{
			ShowAddDiskReport(ClusterAdministrator.NotifyUser, reportFileName);
		}
	}

	private static void ShowAddDiskReport(INotifyUser notifyUser, string reportFileName)
	{
		if (notifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button1, Resources.AddStorage_Failed_Text) == DialogResult.Yes)
		{
			ReportViewer.LaunchReportViewer(reportFileName);
		}
	}

	private object CreateDiskResources(CluadminWaitDialog waitDialog, CreateDiskResourcesArgs args)
	{
		string tempFileName = Path.GetTempFileName();
		ReportBuilder val = ReportBuilder.CreateXmlReportBuilder(tempFileName);
		try
		{
			report = val.PrimaryChannel;
			try
			{
				report.ReportItem((CommonReportItem)22, Cluster.FqdnName);
				args.ClusterableDisks.CreateDiskResources();
			}
			catch (ApplicationException ex)
			{
				addDisksErrors = true;
				ExceptionHelp.LogException(ex, "Error adding disks");
				report.ReportFail(ExceptionHelp.GetExceptionReportMessage(ex, Resources.CannotAddStorage_Text));
			}
			finally
			{
				if (args.ClusterableDisks.Errors.Count > 0)
				{
					report.ReportFail(Resources.BulkDiskAddFailedReportHeader_Text);
					foreach (Exception error in args.ClusterableDisks.Errors)
					{
						report.ReportFail(ExceptionHelp.GetExceptionReportMessage(error));
					}
				}
				report.Close();
				report = null;
			}
		}
		catch (OperationCanceledException)
		{
		}
		finally
		{
			val.Close();
			XmlReportRenderer.TransformStandardHtmlReport(tempFileName, args.ReportFileName);
			try
			{
				File.Delete(tempFileName);
			}
			catch (IOException caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error deleting {0}", tempFileName);
			}
		}
		return null;
	}

	private void AddSelectedDisksProgress(object sender, OperationProgressEventArgs e)
	{
		addDiskWaitDialog.ThrowIfCanceled();
		addDiskWaitDialog.UpdateProgress(e.PercentDone, 100, e.Message);
		if (e.WarningLevel == OperationProgressWarningLevel.Error)
		{
			addDisksErrors = true;
		}
		ApplyProgressToReport(e, report);
	}

	private static void ApplyProgressToReport(OperationProgressEventArgs e, ReportChannel reportChannel)
	{
		switch (e.WarningLevel)
		{
		case OperationProgressWarningLevel.Error:
			reportChannel.ReportFail(e.Message);
			break;
		case OperationProgressWarningLevel.Information:
			reportChannel.ReportInfo(e.Message);
			break;
		case OperationProgressWarningLevel.Warning:
			reportChannel.ReportWarn(e.Message);
			break;
		}
	}
}
