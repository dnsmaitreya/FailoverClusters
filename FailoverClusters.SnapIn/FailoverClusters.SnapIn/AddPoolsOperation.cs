using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal class AddPoolsOperation
{
	private class CreatePoolResourcesArgs
	{
		public readonly string ReportFileName;

		public readonly ClusterableStoragePoolsCollection ClusterableStoragePools;

		public readonly CancellationTokenSource CancellationToken;

		public CreatePoolResourcesArgs(string reportFileName, ClusterableStoragePoolsCollection clusterableStoragePools, CancellationTokenSource cancellationToken)
		{
			ReportFileName = reportFileName;
			ClusterableStoragePools = clusterableStoragePools;
			CancellationToken = cancellationToken;
		}
	}

	private CluadminWaitDialog addPoolWaitDialog;

	private bool addPoolsErrors;

	private ReportChannel report;

	private FailoverClusters.Framework.Cluster FrameworkCluster { get; set; }

	private KDDSL.ServerClusters.Cluster LegacyCluster { get; set; }

	internal AddPoolsOperation(FailoverClusters.Framework.Cluster frameworkCluster, KDDSL.ServerClusters.Cluster legacyCluster)
	{
		FrameworkCluster = frameworkCluster;
		LegacyCluster = legacyCluster;
	}

	public void Execute()
	{
		ClusterableStoragePoolsCollection clusterableStoragePoolsCollection = AddPoolDialog.AddPoolsDialog(FrameworkCluster, ClusterAdministrator.NotifyUser);
		if (clusterableStoragePoolsCollection == null || clusterableStoragePoolsCollection.Count == 0)
		{
			return;
		}
		string reportFileName = string.Format(CultureInfo.InvariantCulture, "{0}.htm", Path.GetTempFileName());
		addPoolWaitDialog = CluadminWaitDialog.Create(Resources.CreatingStoragePoolResources_Text, string.Empty);
		addPoolsErrors = false;
		addPoolWaitDialog.DisplayDelay = new TimeSpan(0L);
		using (addPoolWaitDialog)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CreatePoolResourcesArgs data = new CreatePoolResourcesArgs(reportFileName, clusterableStoragePoolsCollection, cancellationTokenSource);
			addPoolWaitDialog.CancelTime = TimeSpan.FromSeconds(5.0);
			addPoolWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, CreateStoragePoolResources, data);
			if (addPoolWaitDialog.IsCanceled)
			{
				cancellationTokenSource.Cancel();
				return;
			}
		}
		addPoolWaitDialog = null;
		if (addPoolsErrors)
		{
			ShowAddPoolsReport(ClusterAdministrator.NotifyUser, reportFileName);
		}
	}

	private static void ShowAddPoolsReport(INotifyUser notifyUser, string reportFileName)
	{
		if (notifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button1, Resources.AddStorage_Failed_Text) == DialogResult.Yes)
		{
			ReportViewer.LaunchReportViewer(reportFileName);
		}
	}

	private object CreateStoragePoolResources(CluadminWaitDialog waitDialog, CreatePoolResourcesArgs args)
	{
		string tempFileName = Path.GetTempFileName();
		ReportBuilder val = ReportBuilder.CreateXmlReportBuilder(tempFileName);
		int totalCount = args.ClusterableStoragePools.Count;
		int currentCount = 0;
		try
		{
			report = val.PrimaryChannel;
			try
			{
				report.ReportItem((CommonReportItem)22, LegacyCluster.FqdnName);
				AutoResetEvent autoResetEvent = new AutoResetEvent(initialState: false);
				try
				{
					FrameworkCluster.AddStoragePools(args.ClusterableStoragePools, delegate(ClusterableStoragePool p)
					{
						try
						{
							currentCount++;
							int percentDone = (int)((float)currentCount / (float)totalCount * 100f);
							if (p.Error != null)
							{
								AddSelectedPoolsProgress(percentDone, Extensions.FormatCurrentCulture(Resources.AddStoragePoolError_ReportText, new object[2]
								{
									p.DisplayName,
									p.Error.Message
								}), OperationProgressWarningLevel.Error);
							}
							else
							{
								AddSelectedPoolsProgress(percentDone, Extensions.FormatCurrentCulture(Resources.AddStoragePoolSuccess_ReportText, (object)p.DisplayName), OperationProgressWarningLevel.Information);
							}
						}
						catch (OperationCanceledException)
						{
							ClusterLog.LogInfo("Add Storage Pool operation has been canceled by the user");
						}
					}, delegate
					{
						autoResetEvent.Set();
					}, args.CancellationToken.Token);
					autoResetEvent.WaitOne();
				}
				finally
				{
					if (autoResetEvent != null)
					{
						((IDisposable)autoResetEvent).Dispose();
					}
				}
			}
			finally
			{
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

	private void AddSelectedPoolsProgress(int percentDone, string message, OperationProgressWarningLevel warningLevel)
	{
		addPoolWaitDialog.ThrowIfCanceled();
		addPoolWaitDialog.UpdateProgress(percentDone, 100, message);
		if (warningLevel == OperationProgressWarningLevel.Error)
		{
			addPoolsErrors = true;
		}
		ApplyProgressToReport(message, warningLevel, report);
	}

	private static void ApplyProgressToReport(string message, OperationProgressWarningLevel warningLevel, ReportChannel reportChannel)
	{
		switch (warningLevel)
		{
		case OperationProgressWarningLevel.Error:
			reportChannel.ReportFail(message);
			break;
		case OperationProgressWarningLevel.Information:
			reportChannel.ReportInfo(message);
			break;
		case OperationProgressWarningLevel.Warning:
			reportChannel.ReportWarn(message);
			break;
		}
	}
}

