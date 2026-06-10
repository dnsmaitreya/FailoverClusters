using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Controls;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class GroupFailoverPropertyPage : SnapinPropertyPageControlBase
{
	private FailoverClusters.Framework.GroupFailbackType autoFailbackType;

	private uint failbackWindowStart;

	private uint failbackWindowEnd;

	private uint failoverThreshold;

	private uint failoverPeriod;

	private FailoverClusters.Framework.GroupType groupType;

	private uint defaultFailoverThreshold;

	private readonly ToolTip tooltipMaximumFailures = new ToolTip();

	private readonly ToolTip tooltipPeriodFailures = new ToolTip();

	private readonly Guid groupId;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private Group group;

	private bool failoverPolicyDirty;

	private bool failbackPolicyDirty;

	private IContainer components;

	private Label failoverThresholdLabel;

	private Label failoverPeriodLabel;

	private RadioButton failbackWindowButton;

	private RadioButton failbackImmediatelyButton;

	private RadioButton allowFailbackButton;

	private RadioButton preventFailbackButton;

	private Label hoursLabel2;

	private NumericUpDown failbackWindowEndUpDown;

	private Label andLabel;

	private NumericUpDown failbackWindowStartUpDown;

	private Label failoverLabel;

	private Label failbackLabel;

	private NumericUpDown failoverThresholdUpDown;

	private NumericUpDown failoverPeriodUpDown;

	private SnapinGroupBox allowFailbackGroupBox;

	private Label failoverInfoLabel;

	private Label failbackInstructionsLabel;

	private HorizontalLine horizontalLine1;

	private HorizontalLine horizontalLine2;

	internal GroupFailoverPropertyPage()
		: base(Resources.Failover_Text)
	{
		InitializeComponent();
		CommonConstruct();
	}

	internal GroupFailoverPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid groupId)
		: base(Resources.Failover_Text)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		if (groupId == Guid.Empty)
		{
			throw new ArgumentException(ExceptionResources.InvalidArgument_Text, "groupId");
		}
		this.cluster = cluster;
		this.groupId = groupId;
		group = null;
		InitializeComponent();
		CommonConstruct();
	}

	private void CommonConstruct()
	{
		tooltipMaximumFailures.SetToolTip(failoverThresholdUpDown, Resources.MaximumFailuresExplanation_Text);
		tooltipMaximumFailures.SetToolTip(failoverThresholdLabel, Resources.MaximumFailuresExplanation_Text);
		tooltipPeriodFailures.SetToolTip(failoverPeriodUpDown, Resources.PeriodFailuresExplanation_Text);
		tooltipPeriodFailures.SetToolTip(failoverPeriodLabel, Resources.PeriodFailuresExplanation_Text);
		failoverThresholdUpDown.KeyUp += FailoverThresholdUpDownKeyUp;
		failoverPeriodUpDown.KeyUp += FailoverPeriodUpDownKeyUp;
		defaultFailoverThreshold = 0u;
	}

	private void FailoverThresholdUpDownKeyUp(object sender, KeyEventArgs e)
	{
		_ = failoverThresholdUpDown.Value;
	}

	private void FailoverPeriodUpDownKeyUp(object sender, KeyEventArgs e)
	{
		_ = failoverPeriodUpDown.Value;
	}

	protected override object LoadProperties(object context)
	{
		Group.Get(cluster, groupId, delegate(OperationResult<Group> cacheResult)
		{
			if (cacheResult.Error != null)
			{
				throw cacheResult.Error;
			}
			group = cacheResult.Result;
			group.LoadAsync(delegate(ClusterLoadedEventArgs result)
			{
				if (result.Error != null)
				{
					throw result.Error;
				}
				group.Cluster.Nodes.ExecuteQuery(NodesQuery);
			});
		}, OperationType.Async);
		return null;
	}

	private void NodesQuery(OperationResult<IClusterList<Node>> nodeResult)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (nodeResult.Error != null)
		{
			ClusterDialogException.ShowTaskDialog(nodeResult.Error, base.HWND);
			return;
		}
		IClusterList<Node> nodes = nodeResult.Result;
		SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
		{
			groupType = group.GroupType;
			ClusterPropertyCollection properties = group.Properties;
			autoFailbackType = (FailoverClusters.Framework.GroupFailbackType)(uint)properties["AutoFailbackType"].Value;
			failbackWindowStart = (uint)properties["FailbackWindowStart"].Value;
			failbackWindowEnd = (uint)properties["FailbackWindowEnd"].Value;
			failoverThreshold = (uint)properties["FailoverThreshold"].Value;
			failoverPeriod = (uint)properties["FailoverPeriod"].Value;
			defaultFailoverThreshold = (uint)(nodes.Count - 1);
			LoadControls();
		});
	}

	protected override void InitializePage()
	{
		base.IsDirty = false;
	}

	private void LoadControls()
	{
		failoverThresholdUpDown.Minimum = 0m;
		failoverThresholdUpDown.Maximum = 4294967295m;
		failoverPeriodUpDown.Minimum = 0m;
		failoverPeriodUpDown.Maximum = 1193m;
		failbackWindowStartUpDown.Minimum = 0m;
		failbackWindowStartUpDown.Maximum = 23m;
		failbackWindowEndUpDown.Minimum = 0m;
		failbackWindowEndUpDown.Maximum = 23m;
		failoverThresholdUpDown.Value = ((failoverThreshold == uint.MaxValue) ? defaultFailoverThreshold : failoverThreshold);
		failoverPeriodUpDown.Value = failoverPeriod;
		preventFailbackButton.Checked = autoFailbackType == FailoverClusters.Framework.GroupFailbackType.PreventFailback;
		allowFailbackButton.Checked = autoFailbackType == FailoverClusters.Framework.GroupFailbackType.AllowFailback;
		((Control)(object)allowFailbackGroupBox).Enabled = allowFailbackButton.Checked;
		failbackImmediatelyButton.Checked = failbackWindowStart == uint.MaxValue || failbackWindowEnd == uint.MaxValue;
		failbackWindowButton.Checked = !failbackImmediatelyButton.Checked;
		failbackWindowStartUpDown.Enabled = failbackWindowButton.Checked;
		failbackWindowEndUpDown.Enabled = failbackWindowButton.Checked;
		failbackWindowStartUpDown.Value = ((failbackWindowStart != uint.MaxValue) ? failbackWindowStart : 0u);
		failbackWindowEndUpDown.Value = ((failbackWindowEnd != uint.MaxValue) ? failbackWindowEnd : 0u);
		if (groupType == FailoverClusters.Framework.GroupType.CoreCluster)
		{
			failoverInfoLabel.Text = Resources.CoreClusterGroupFailoverInfo_Text;
			failbackInstructionsLabel.Text = Resources.CoreClusterGroupFailbackInstructions_Text;
		}
		else
		{
			failoverInfoLabel.Text = Resources.ServiceOrApplicationFailoverInfo_Text;
			failbackInstructionsLabel.Text = Resources.ServiceOrApplicationFailbackInstructions_Text;
		}
		failoverPolicyDirty = false;
		base.IsDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (failoverPolicyDirty)
		{
			if ((uint)failoverThresholdUpDown.Value == defaultFailoverThreshold)
			{
				failoverThreshold = uint.MaxValue;
			}
			else
			{
				failoverThreshold = (uint)failoverThresholdUpDown.Value;
			}
			failoverPeriod = (uint)failoverPeriodUpDown.Value;
		}
		if (failbackPolicyDirty)
		{
			autoFailbackType = ((!preventFailbackButton.Checked) ? FailoverClusters.Framework.GroupFailbackType.AllowFailback : FailoverClusters.Framework.GroupFailbackType.PreventFailback);
			if (failbackImmediatelyButton.Checked)
			{
				failbackWindowStart = uint.MaxValue;
				failbackWindowEnd = uint.MaxValue;
				failbackWindowStartUpDown.Value = 0m;
				failbackWindowEndUpDown.Value = 0m;
			}
			else
			{
				failbackWindowStart = (uint)failbackWindowStartUpDown.Value;
				failbackWindowEnd = (uint)failbackWindowEndUpDown.Value;
			}
		}
		return true;
	}

	protected override bool SaveProperties()
	{
		bool success = true;
		SettingChanger propertiesUpdateWaiter = new SettingChanger(initialState: true);
		try
		{
			bool flag = false;
			ClusterPropertyCollection properties = group.Properties;
			if (failoverPolicyDirty)
			{
				((ClusterPropertyUInt)properties["FailoverThreshold"]).TypedValue = failoverThreshold;
				((ClusterPropertyUInt)properties["FailoverPeriod"]).TypedValue = failoverPeriod;
				flag = true;
			}
			if (failbackPolicyDirty)
			{
				((ClusterPropertyUInt)properties["AutoFailbackType"]).TypedValue = (uint)autoFailbackType;
				((ClusterPropertyUInt)properties["FailbackWindowStart"]).TypedValue = failbackWindowStart;
				((ClusterPropertyUInt)properties["FailbackWindowEnd"]).TypedValue = failbackWindowEnd;
				flag = true;
			}
			if (flag)
			{
				failoverPolicyDirty = false;
				failbackPolicyDirty = false;
				success = false;
				propertiesUpdateWaiter.Reset();
				properties.Commit(delegate(OperationResult result)
				{
					//IL_0083: Unknown result type (might be due to invalid IL or missing references)
					success = result.Error == null;
					propertiesUpdateWaiter.Set();
					if (!success)
					{
						failoverPolicyDirty = (failbackPolicyDirty = true);
						Exception ex = result.Error;
						if (result.Error is ClusterControlCodeException)
						{
							ex = new ClusterSavePropertiesException(group.Name, result.Error);
						}
						ClusterLog.LogException(ex, "Error saving group failover properites");
						ClusterDialogException.ShowTaskDialog(ex, base.HWND);
					}
				});
			}
		}
		finally
		{
			if (propertiesUpdateWaiter != null)
			{
				((IDisposable)propertiesUpdateWaiter).Dispose();
			}
		}
		return success;
	}

	protected override void CompleteSaveProperties()
	{
	}

	private void FailoverThresholdChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
		failoverPolicyDirty = true;
	}

	private void FailoverPeriodChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
		failoverPolicyDirty = true;
	}

	private void AllowFailbackButtonChanged(object sender, EventArgs e)
	{
		((Control)(object)allowFailbackGroupBox).Enabled = allowFailbackButton.Checked;
		if (allowFailbackButton.Checked)
		{
			failbackImmediatelyButton.Checked = failbackWindowStart == uint.MaxValue || failbackWindowEnd == uint.MaxValue;
			failbackWindowButton.Checked = !failbackImmediatelyButton.Checked;
			failbackWindowStartUpDown.Enabled = failbackWindowButton.Checked;
			failbackWindowEndUpDown.Enabled = failbackWindowButton.Checked;
		}
		base.IsDirty = true;
		failbackPolicyDirty = true;
	}

	private void FailbackWindowButtonChanged(object sender, EventArgs e)
	{
		failbackWindowStartUpDown.Enabled = failbackWindowButton.Checked;
		failbackWindowEndUpDown.Enabled = failbackWindowButton.Checked;
		base.IsDirty = true;
		failbackPolicyDirty = true;
	}

	private void FailbackWindowStartChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
		failbackPolicyDirty = true;
	}

	private void FailbackWindowEndChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
		failbackPolicyDirty = true;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			failoverThresholdUpDown.KeyUp -= FailoverThresholdUpDownKeyUp;
			failoverPeriodUpDown.KeyUp -= FailoverPeriodUpDownKeyUp;
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(GroupFailoverPropertyPage));
		failoverThresholdLabel = new Label();
		failoverPeriodLabel = new Label();
		hoursLabel2 = new Label();
		failbackWindowEndUpDown = new NumericUpDown();
		andLabel = new Label();
		failbackWindowStartUpDown = new NumericUpDown();
		failbackWindowButton = new RadioButton();
		failbackImmediatelyButton = new RadioButton();
		allowFailbackButton = new RadioButton();
		preventFailbackButton = new RadioButton();
		failoverLabel = new Label();
		failbackLabel = new Label();
		failoverThresholdUpDown = new NumericUpDown();
		failoverPeriodUpDown = new NumericUpDown();
		allowFailbackGroupBox = new SnapinGroupBox();
		failoverInfoLabel = new Label();
		failbackInstructionsLabel = new Label();
		horizontalLine1 = new HorizontalLine();
		horizontalLine2 = new HorizontalLine();
		((ISupportInitialize)failbackWindowEndUpDown).BeginInit();
		((ISupportInitialize)failbackWindowStartUpDown).BeginInit();
		((ISupportInitialize)failoverThresholdUpDown).BeginInit();
		((ISupportInitialize)failoverPeriodUpDown).BeginInit();
		((Control)(object)allowFailbackGroupBox).SuspendLayout();
		((Control)(object)this).SuspendLayout();
		failoverThresholdLabel.AutoEllipsis = true;
		failoverThresholdLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(failoverThresholdLabel, "failoverThresholdLabel");
		failoverThresholdLabel.Name = "failoverThresholdLabel";
		failoverPeriodLabel.AutoEllipsis = true;
		failoverPeriodLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(failoverPeriodLabel, "failoverPeriodLabel");
		failoverPeriodLabel.Name = "failoverPeriodLabel";
		hoursLabel2.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(hoursLabel2, "hoursLabel2");
		hoursLabel2.Name = "hoursLabel2";
		componentResourceManager.ApplyResources(failbackWindowEndUpDown, "failbackWindowEndUpDown");
		failbackWindowEndUpDown.Name = "failbackWindowEndUpDown";
		failbackWindowEndUpDown.ValueChanged += FailbackWindowEndChanged;
		andLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(andLabel, "andLabel");
		andLabel.Name = "andLabel";
		componentResourceManager.ApplyResources(failbackWindowStartUpDown, "failbackWindowStartUpDown");
		failbackWindowStartUpDown.Name = "failbackWindowStartUpDown";
		failbackWindowStartUpDown.ValueChanged += FailbackWindowStartChanged;
		failbackWindowButton.AutoEllipsis = true;
		failbackWindowButton.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(failbackWindowButton, "failbackWindowButton");
		failbackWindowButton.Name = "failbackWindowButton";
		failbackWindowButton.CheckedChanged += FailbackWindowButtonChanged;
		componentResourceManager.ApplyResources(failbackImmediatelyButton, "failbackImmediatelyButton");
		failbackImmediatelyButton.ForeColor = SystemColors.ControlText;
		failbackImmediatelyButton.Name = "failbackImmediatelyButton";
		componentResourceManager.ApplyResources(allowFailbackButton, "allowFailbackButton");
		allowFailbackButton.ForeColor = SystemColors.ControlText;
		allowFailbackButton.Name = "allowFailbackButton";
		allowFailbackButton.CheckedChanged += AllowFailbackButtonChanged;
		componentResourceManager.ApplyResources(preventFailbackButton, "preventFailbackButton");
		preventFailbackButton.ForeColor = SystemColors.ControlText;
		preventFailbackButton.Name = "preventFailbackButton";
		componentResourceManager.ApplyResources(failoverLabel, "failoverLabel");
		failoverLabel.ForeColor = SystemColors.ControlText;
		failoverLabel.Name = "failoverLabel";
		componentResourceManager.ApplyResources(failbackLabel, "failbackLabel");
		failbackLabel.ForeColor = SystemColors.ControlText;
		failbackLabel.Name = "failbackLabel";
		componentResourceManager.ApplyResources(failoverThresholdUpDown, "failoverThresholdUpDown");
		failoverThresholdUpDown.Name = "failoverThresholdUpDown";
		failoverThresholdUpDown.ValueChanged += FailoverThresholdChanged;
		componentResourceManager.ApplyResources(failoverPeriodUpDown, "failoverPeriodUpDown");
		failoverPeriodUpDown.Name = "failoverPeriodUpDown";
		failoverPeriodUpDown.ValueChanged += FailoverPeriodChanged;
		componentResourceManager.ApplyResources(allowFailbackGroupBox, "allowFailbackGroupBox");
		((Control)(object)allowFailbackGroupBox).Controls.Add(failbackImmediatelyButton);
		((Control)(object)allowFailbackGroupBox).Controls.Add(hoursLabel2);
		((Control)(object)allowFailbackGroupBox).Controls.Add(andLabel);
		((Control)(object)allowFailbackGroupBox).Controls.Add(failbackWindowButton);
		((Control)(object)allowFailbackGroupBox).Controls.Add(failbackWindowEndUpDown);
		((Control)(object)allowFailbackGroupBox).Controls.Add(failbackWindowStartUpDown);
		((GroupBox)(object)allowFailbackGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)allowFailbackGroupBox).Name = "allowFailbackGroupBox";
		((GroupBox)(object)allowFailbackGroupBox).TabStop = false;
		componentResourceManager.ApplyResources(failoverInfoLabel, "failoverInfoLabel");
		failoverInfoLabel.AutoEllipsis = true;
		failoverInfoLabel.ForeColor = SystemColors.ControlText;
		failoverInfoLabel.Name = "failoverInfoLabel";
		componentResourceManager.ApplyResources(failbackInstructionsLabel, "failbackInstructionsLabel");
		failbackInstructionsLabel.BackColor = SystemColors.Control;
		failbackInstructionsLabel.ForeColor = SystemColors.ControlText;
		failbackInstructionsLabel.Name = "failbackInstructionsLabel";
		componentResourceManager.ApplyResources(horizontalLine1, "horizontalLine1");
		((Control)(object)horizontalLine1).Name = "horizontalLine1";
		componentResourceManager.ApplyResources(horizontalLine2, "horizontalLine2");
		((Control)(object)horizontalLine2).Name = "horizontalLine2";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine2);
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine1);
		((Control)(object)this).Controls.Add(failbackInstructionsLabel);
		((Control)(object)this).Controls.Add(failoverInfoLabel);
		((Control)(object)this).Controls.Add(allowFailbackButton);
		((Control)(object)this).Controls.Add((Control)(object)allowFailbackGroupBox);
		((Control)(object)this).Controls.Add(failoverPeriodUpDown);
		((Control)(object)this).Controls.Add(failoverThresholdUpDown);
		((Control)(object)this).Controls.Add(failbackLabel);
		((Control)(object)this).Controls.Add(failoverLabel);
		((Control)(object)this).Controls.Add(failoverThresholdLabel);
		((Control)(object)this).Controls.Add(preventFailbackButton);
		((Control)(object)this).Controls.Add(failoverPeriodLabel);
		((Control)(object)this).Name = "GroupFailoverPropertyPage";
		((Control)(object)this).Controls.SetChildIndex(failoverPeriodLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(preventFailbackButton, 0);
		((Control)(object)this).Controls.SetChildIndex(failoverThresholdLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(failoverLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(failbackLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(failoverThresholdUpDown, 0);
		((Control)(object)this).Controls.SetChildIndex(failoverPeriodUpDown, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)allowFailbackGroupBox, 0);
		((Control)(object)this).Controls.SetChildIndex(allowFailbackButton, 0);
		((Control)(object)this).Controls.SetChildIndex(failoverInfoLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(failbackInstructionsLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine1, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine2, 0);
		((ISupportInitialize)failbackWindowEndUpDown).EndInit();
		((ISupportInitialize)failbackWindowStartUpDown).EndInit();
		((ISupportInitialize)failoverThresholdUpDown).EndInit();
		((ISupportInitialize)failoverPeriodUpDown).EndInit();
		((Control)(object)allowFailbackGroupBox).ResumeLayout(performLayout: false);
		((Control)(object)allowFailbackGroupBox).PerformLayout();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

