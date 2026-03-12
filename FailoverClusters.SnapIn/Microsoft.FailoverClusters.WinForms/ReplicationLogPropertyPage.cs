using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Controls;
using MS.Internal.ServerClusters.Management;

namespace Microsoft.FailoverClusters.WinForms;

internal class ReplicationLogPropertyPage : ResourcePropertyPage
{
	private readonly Guid resourceId;

	private readonly Microsoft.FailoverClusters.Framework.Cluster cluster;

	private Resource resource;

	private StorageResource storageResource;

	private string logSizePerVolumeText;

	private string maximumLogSizeOnDiskText;

	private const double Gb = 1073741824.0;

	private IContainer components;

	private Label LogheaderLabel;

	private HorizontalLine horizontalLine1;

	private Label LogSizePerVolumeLabel;

	private Label GBLabel;

	private Label TotalSpaceOnDiskLabel;

	private NumericUpDown LogSizeNumbericUpDown;

	internal ReplicationLogPropertyPage()
		: base(Resources.ReplicationLog_Text)
	{
		InitializeComponent();
	}

	internal ReplicationLogPropertyPage(Microsoft.FailoverClusters.Framework.Cluster cluster, Guid resourceId)
		: this()
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		Exceptions.ThrowIfEmpty(resourceId, "resourceId");
		this.cluster = cluster;
		this.resourceId = resourceId;
	}

	protected override void InitializePage()
	{
		logSizePerVolumeText = LogSizePerVolumeLabel.Text;
		LogSizePerVolumeLabel.Text = Extensions.FormatCurrentCulture(LogSizePerVolumeLabel.Text, (object)string.Empty);
		maximumLogSizeOnDiskText = TotalSpaceOnDiskLabel.Text;
		TotalSpaceOnDiskLabel.Text = Extensions.FormatCurrentCulture(TotalSpaceOnDiskLabel.Text, (object)string.Empty);
		LogSizeNumbericUpDown.Value = 0m;
		LogSizeNumbericUpDown.Enabled = false;
		((Control)(object)this).Enabled = false;
	}

	protected override object LoadProperties(object context)
	{
		Resource.Get(cluster, resourceId, delegate(OperationResult<Resource> cacheResult)
		{
			if (cacheResult.Error != null)
			{
				throw cacheResult.Error;
			}
			resource = cacheResult.Result;
			resource.LoadAsync(delegate(ClusterLoadedEventArgs result)
			{
				if (result.Error != null)
				{
					throw result.Error;
				}
				storageResource = resource as StorageResource;
				if (storageResource != null)
				{
					SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
					{
						LogSizePerVolumeLabel.Text = Extensions.FormatCurrentCulture(logSizePerVolumeText, (object)Math.Round((double)storageResource.ReplicationInfo.ContainerSize / 1073741824.0, 2));
						LogSizeNumbericUpDown.Increment = (decimal)Math.Round((double)storageResource.ReplicationInfo.ContainerSize / 1073741824.0, 2);
						LogSizeNumbericUpDown.DecimalPlaces = ((Math.Round((double)storageResource.ReplicationInfo.ContainerSize / 1073741824.0, 2) % 1.0 > 0.0) ? 2 : 0);
						LogSizeNumbericUpDown.Minimum = (decimal)Math.Round((double)storageResource.ReplicationInfo.MinLogSize / 1073741824.0, 2);
						LogSizeNumbericUpDown.Maximum = 9999m;
						LogSizeNumbericUpDown.Value = (decimal)Math.Round((double)storageResource.ReplicationInfo.LogSize / 1073741824.0, 2);
						CalculateSpaceOnDisk();
						LogSizeNumbericUpDown.Enabled = true;
						base.IsDirty = false;
						if (storageResource.ResourceState == Microsoft.FailoverClusters.Framework.ResourceState.Online)
						{
							((Control)(object)this).Enabled = true;
						}
					});
				}
			}, ResourceLoadSelection.Storage | ResourceLoadSelection.StorageReplicationInfo);
		}, OperationType.Async);
		return null;
	}

	protected override bool SaveProperties()
	{
		bool success = true;
		SettingChanger changeLogUpdateWaiter = new SettingChanger(initialState: true);
		try
		{
			success = false;
			changeLogUpdateWaiter.Reset();
			storageResource.RedirectAsyncOutput(delegate
			{
				long logSize = (int)Math.Round((double)LogSizeNumbericUpDown.Value * 1073741824.0 / (double)storageResource.ReplicationInfo.ContainerSize, MidpointRounding.AwayFromZero) * storageResource.ReplicationInfo.ContainerSize;
				storageResource.ReplicationInfo.LogSize = logSize;
			}, delegate(OperationResult result)
			{
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				success = result.Error == null;
				changeLogUpdateWaiter.Set();
				if (result.Error != null)
				{
					ClusterLog.LogException((Exception)result.Error, "Error changing the replication log size");
					ClusterDialogException.ShowTaskDialog(result.Error, base.HWND);
				}
			});
		}
		finally
		{
			if (changeLogUpdateWaiter != null)
			{
				((IDisposable)changeLogUpdateWaiter).Dispose();
			}
		}
		return success;
	}

	protected override bool ValidateProperties()
	{
		return true;
	}

	private void LogSizeNumbericUpDown_Leave(object sender, EventArgs e)
	{
		if (LogSizeNumbericUpDown.Enabled && storageResource != null)
		{
			double num = Math.Round((double)LogSizeNumbericUpDown.Value * 1073741824.0 / (double)storageResource.ReplicationInfo.ContainerSize, MidpointRounding.AwayFromZero) * (double)storageResource.ReplicationInfo.ContainerSize;
			LogSizeNumbericUpDown.Value = (decimal)Math.Round(num / 1073741824.0, 2);
			CalculateSpaceOnDisk();
		}
	}

	private void LogSizeNumbericUpDown_Validated(object sender, EventArgs e)
	{
		CalculateSpaceOnDisk();
	}

	private void LogSizeNumbericUpDown_ValueChanged(object sender, EventArgs e)
	{
		CalculateSpaceOnDisk();
		base.IsDirty = true;
	}

	private void CalculateSpaceOnDisk()
	{
		if (!(storageResource == null))
		{
			double num = Math.Round(Math.Round((double)LogSizeNumbericUpDown.Value * 1073741824.0 / (double)storageResource.ReplicationInfo.ContainerSize, MidpointRounding.AwayFromZero) * (double)storageResource.ReplicationInfo.ContainerSize * (double)storageResource.ReplicationInfo.MultiplicationFactor / 1073741824.0, 2);
			TotalSpaceOnDiskLabel.Text = Extensions.FormatCurrentCulture(maximumLogSizeOnDiskText, (object)num);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ReplicationLogPropertyPage));
		LogheaderLabel = new Label();
		horizontalLine1 = new HorizontalLine();
		LogSizePerVolumeLabel = new Label();
		GBLabel = new Label();
		TotalSpaceOnDiskLabel = new Label();
		LogSizeNumbericUpDown = new NumericUpDown();
		((ISupportInitialize)LogSizeNumbericUpDown).BeginInit();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(LogheaderLabel, "LogheaderLabel");
		LogheaderLabel.Name = "LogheaderLabel";
		componentResourceManager.ApplyResources(horizontalLine1, "horizontalLine1");
		((Control)(object)horizontalLine1).Name = "horizontalLine1";
		componentResourceManager.ApplyResources(LogSizePerVolumeLabel, "LogSizePerVolumeLabel");
		LogSizePerVolumeLabel.ForeColor = SystemColors.ControlText;
		LogSizePerVolumeLabel.Name = "LogSizePerVolumeLabel";
		componentResourceManager.ApplyResources(GBLabel, "GBLabel");
		GBLabel.ForeColor = SystemColors.ControlText;
		GBLabel.Name = "GBLabel";
		componentResourceManager.ApplyResources(TotalSpaceOnDiskLabel, "TotalSpaceOnDiskLabel");
		TotalSpaceOnDiskLabel.ForeColor = SystemColors.ControlText;
		TotalSpaceOnDiskLabel.Name = "TotalSpaceOnDiskLabel";
		componentResourceManager.ApplyResources(LogSizeNumbericUpDown, "LogSizeNumbericUpDown");
		LogSizeNumbericUpDown.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
		LogSizeNumbericUpDown.Name = "LogSizeNumbericUpDown";
		LogSizeNumbericUpDown.ReadOnly = true;
		LogSizeNumbericUpDown.Value = new decimal(new int[4] { 8888, 0, 0, 0 });
		LogSizeNumbericUpDown.ValueChanged += LogSizeNumbericUpDown_ValueChanged;
		LogSizeNumbericUpDown.Leave += LogSizeNumbericUpDown_Leave;
		LogSizeNumbericUpDown.Validated += LogSizeNumbericUpDown_Validated;
		((Control)(object)this).AccessibleRole = AccessibleRole.PropertyPage;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(LogSizeNumbericUpDown);
		((Control)(object)this).Controls.Add(TotalSpaceOnDiskLabel);
		((Control)(object)this).Controls.Add(GBLabel);
		((Control)(object)this).Controls.Add(LogSizePerVolumeLabel);
		((Control)(object)this).Controls.Add(LogheaderLabel);
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine1);
		((Control)(object)this).Name = "ReplicationLogPropertyPage";
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine1, 0);
		((Control)(object)this).Controls.SetChildIndex(LogheaderLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(LogSizePerVolumeLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(GBLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(TotalSpaceOnDiskLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(LogSizeNumbericUpDown, 0);
		((ISupportInitialize)LogSizeNumbericUpDown).EndInit();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
