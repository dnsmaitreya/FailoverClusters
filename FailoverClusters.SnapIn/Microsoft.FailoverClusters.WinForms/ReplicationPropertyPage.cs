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

internal class ReplicationPropertyPage : ResourcePropertyPage
{
	private readonly Guid resourceId;

	private readonly Microsoft.FailoverClusters.Framework.Cluster cluster;

	private Resource resource;

	private StorageResource storageResource;

	private IContainer components;

	private Label ConsistencyLabel;

	private HorizontalLine horizontalLine2;

	private Label ConsistencyNoteLabel;

	private Label LblConsistency;

	private FlowLayoutPanel flowLayoutPanel1;

	private Label LblConsistencyEnabled;

	internal ReplicationPropertyPage()
		: base(Resources.Replication_Text)
	{
		InitializeComponent();
	}

	internal ReplicationPropertyPage(Microsoft.FailoverClusters.Framework.Cluster cluster, Guid resourceId)
		: this()
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		Exceptions.ThrowIfEmpty(resourceId, "resourceId");
		LblConsistencyEnabled.Text = string.Empty;
		this.cluster = cluster;
		this.resourceId = resourceId;
	}

	protected override void InitializePage()
	{
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
						LblConsistencyEnabled.Text = (storageResource.ReplicationInfo.IsConsistencyEnabled ? Resources.ReplicationConsistencyEnabled_Text : Resources.ReplicationConsistencyDisabled_Text);
					});
				}
			}, ResourceLoadSelection.Storage | ResourceLoadSelection.StorageReplicationInfo);
		}, OperationType.Async);
		return null;
	}

	protected override bool SaveProperties()
	{
		return true;
	}

	protected override bool ValidateProperties()
	{
		return true;
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ReplicationPropertyPage));
		ConsistencyLabel = new Label();
		horizontalLine2 = new HorizontalLine();
		ConsistencyNoteLabel = new Label();
		LblConsistency = new Label();
		flowLayoutPanel1 = new FlowLayoutPanel();
		LblConsistencyEnabled = new Label();
		flowLayoutPanel1.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(ConsistencyLabel, "ConsistencyLabel");
		ConsistencyLabel.Name = "ConsistencyLabel";
		componentResourceManager.ApplyResources(horizontalLine2, "horizontalLine2");
		((Control)(object)horizontalLine2).Name = "horizontalLine2";
		componentResourceManager.ApplyResources(ConsistencyNoteLabel, "ConsistencyNoteLabel");
		ConsistencyNoteLabel.ForeColor = SystemColors.ControlText;
		ConsistencyNoteLabel.Name = "ConsistencyNoteLabel";
		componentResourceManager.ApplyResources(LblConsistency, "LblConsistency");
		LblConsistency.ForeColor = SystemColors.ControlText;
		LblConsistency.Name = "LblConsistency";
		flowLayoutPanel1.Controls.Add(LblConsistency);
		flowLayoutPanel1.Controls.Add(LblConsistencyEnabled);
		componentResourceManager.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
		flowLayoutPanel1.Name = "flowLayoutPanel1";
		componentResourceManager.ApplyResources(LblConsistencyEnabled, "LblConsistencyEnabled");
		LblConsistencyEnabled.ForeColor = SystemColors.ControlText;
		LblConsistencyEnabled.Name = "LblConsistencyEnabled";
		((Control)(object)this).AccessibleRole = AccessibleRole.PropertyPage;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(flowLayoutPanel1);
		((Control)(object)this).Controls.Add(ConsistencyNoteLabel);
		((Control)(object)this).Controls.Add(ConsistencyLabel);
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine2);
		((Control)(object)this).Name = "ReplicationPropertyPage";
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine2, 0);
		((Control)(object)this).Controls.SetChildIndex(ConsistencyLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(ConsistencyNoteLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(flowLayoutPanel1, 0);
		flowLayoutPanel1.ResumeLayout(performLayout: false);
		flowLayoutPanel1.PerformLayout();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
