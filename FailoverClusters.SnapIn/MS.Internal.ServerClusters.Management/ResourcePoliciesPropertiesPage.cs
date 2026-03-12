using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Controls;
using MS.Internal.ServerClusters.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class ResourcePoliciesPropertiesPage : ResourcePropertiesPage
{
	private IContainer components;

	private Label periodLabel;

	private Label pendingTimeoutLabel;

	private RadioButton restartRadioButton;

	private CheckBox failoverGroupCheckBox;

	private Label thresholdLabel;

	private RadioButton doNotRestartRadioButton;

	private SnapinGroupBox restartGroupBox;

	private Label label1;

	private SnapinGroupBox pendingTimeoutGroupBox;

	private NumericUpDown thresholdUpDown;

	private NumericUpDownNoCaret restartDelayUpDown;

	private ToolTip restartDelayToolTip;

	private TimePicker periodTimePicker;

	private TimePicker pendingTimeoutTimePicker;

	private TimePicker retryPeriodTimePicker;

	private CheckBox retryPeriodCheckBox;

	private LinkLabel helpLinkLabel;

	private ResourceRestartAction restartAction;

	private uint restartThreshold;

	private uint restartPeriod;

	private uint restartDelay;

	private uint pendingTimeout;

	private Label restartDelayLabel;

	private uint retryPeriodOnFailure;

	internal ResourcePoliciesPropertiesPage(ResourceContext context)
		: base(context, Resources.Policies_Text)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		PropertyCollection commonProperties = base.Context.Resource.GetCommonProperties(PropertyCollectionSet.ReadWrite);
		restartAction = (ResourceRestartAction)(uint)commonProperties["RestartAction"].Value;
		restartThreshold = (uint)commonProperties["RestartThreshold"].Value;
		restartPeriod = (uint)commonProperties["RestartPeriod"].Value;
		pendingTimeout = (uint)commonProperties["PendingTimeout"].Value;
		retryPeriodOnFailure = (uint)commonProperties["RetryPeriodOnFailure"].Value;
		restartDelay = (uint)commonProperties["RestartDelay"].Value;
	}

	protected override void InitializePage()
	{
		periodTimePicker.NotifyUser = base.NotifyUser;
		pendingTimeoutTimePicker.NotifyUser = base.NotifyUser;
		retryPeriodTimePicker.NotifyUser = base.NotifyUser;
		switch (restartAction)
		{
		case ResourceRestartAction.DoNotRestart:
			restartRadioButton.Checked = false;
			doNotRestartRadioButton.Checked = true;
			failoverGroupCheckBox.Checked = false;
			break;
		case ResourceRestartAction.RestartNotify:
			restartRadioButton.Checked = true;
			doNotRestartRadioButton.Checked = false;
			failoverGroupCheckBox.Checked = true;
			break;
		case ResourceRestartAction.RestartNoNotify:
			restartRadioButton.Checked = true;
			doNotRestartRadioButton.Checked = false;
			failoverGroupCheckBox.Checked = false;
			break;
		}
		EnableRestartChanged(restartRadioButton.Checked);
		thresholdUpDown.Value = restartThreshold;
		decimal num = decimal.Round((decimal)restartDelay / 1000m, 1);
		((NumericUpDown)(object)restartDelayUpDown).Value = ((num > ((NumericUpDown)(object)restartDelayUpDown).Maximum) ? ((NumericUpDown)(object)restartDelayUpDown).Maximum : num);
		restartDelayToolTip.SetToolTip((Control)(object)restartDelayUpDown, Resources.RestartDelayToolTip_Text);
		restartDelayToolTip.SetToolTip(restartDelayLabel, Resources.RestartDelayToolTip_Text);
		periodTimePicker.Value = restartPeriod;
		pendingTimeoutTimePicker.Value = pendingTimeout;
		retryPeriodTimePicker.DisplayUnits = (TimePickerUnits)0;
		if (retryPeriodOnFailure == uint.MaxValue)
		{
			retryPeriodCheckBox.Checked = false;
			retryPeriodTimePicker.Value = 0u;
		}
		else
		{
			retryPeriodCheckBox.Checked = true;
			retryPeriodTimePicker.Value = retryPeriodOnFailure;
		}
	}

	protected override bool ValidateProperties()
	{
		if (doNotRestartRadioButton.Checked)
		{
			restartAction = ResourceRestartAction.DoNotRestart;
			failoverGroupCheckBox.Checked = false;
		}
		else if (!failoverGroupCheckBox.Checked)
		{
			restartAction = ResourceRestartAction.RestartNoNotify;
		}
		else
		{
			restartAction = ResourceRestartAction.RestartNotify;
		}
		restartThreshold = (uint)thresholdUpDown.Value;
		restartDelay = (uint)(((NumericUpDown)(object)restartDelayUpDown).Value * 1000m);
		restartPeriod = periodTimePicker.Value;
		if (retryPeriodCheckBox.Checked)
		{
			if (retryPeriodTimePicker.Value < periodTimePicker.Value)
			{
				TimeSpan timeSpan = TimeSpan.FromMilliseconds(periodTimePicker.Value);
				base.NotifyUser.ShowError(Resources.RetryPeriodLessThanRestartPeriod_Text, new object[1] { timeSpan.TotalMinutes });
				return false;
			}
			retryPeriodOnFailure = retryPeriodTimePicker.Value;
		}
		else
		{
			retryPeriodOnFailure = uint.MaxValue;
		}
		pendingTimeout = pendingTimeoutTimePicker.Value;
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			PropertyCollection commonProperties = base.Context.Resource.GetCommonProperties(PropertyCollectionSet.ReadWrite);
			commonProperties["RestartAction"].Value = (uint)restartAction;
			commonProperties["RestartThreshold"].Value = restartThreshold;
			commonProperties["RestartPeriod"].Value = restartPeriod;
			commonProperties["RetryPeriodOnFailure"].Value = retryPeriodOnFailure;
			commonProperties["PendingTimeout"].Value = pendingTimeout;
			commonProperties["RestartDelay"].Value = restartDelay;
			SaveProperties(commonProperties);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving resource policies");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.ResourcePoliciesSavedFailed_Text,
				base.Context.DisplayName
			});
		}
	}

	private void PropertiesChanged(object sender, EventArgs e)
	{
		EnableRestartChanged(restartRadioButton.Checked);
		base.IsDirty = true;
	}

	private void EnableRestartChanged(bool enableRestart)
	{
		((Control)(object)periodTimePicker).Enabled = enableRestart;
		thresholdUpDown.Enabled = enableRestart;
		failoverGroupCheckBox.Enabled = enableRestart;
		retryPeriodCheckBox.Enabled = enableRestart;
		((Control)(object)retryPeriodTimePicker).Enabled = enableRestart && retryPeriodCheckBox.Checked;
		((Control)(object)restartDelayUpDown).Enabled = enableRestart;
	}

	private void OnRetryPeriodTimePickerEnabledChanged(object sender, EventArgs e)
	{
		if (((Control)(object)retryPeriodTimePicker).Enabled)
		{
			if (retryPeriodOnFailure == uint.MaxValue)
			{
				retryPeriodTimePicker.Value = restartPeriod;
			}
			else if (retryPeriodTimePicker.Value == 0)
			{
				retryPeriodTimePicker.Value = retryPeriodOnFailure;
			}
		}
		else if (!retryPeriodCheckBox.Checked)
		{
			retryPeriodTimePicker.Value = 0u;
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

	private void OnHelpLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		HelpProvider.ShowHelp(HelpTopics.GroupGeneralPropertyPageFwlink);
	}

	private void InitializeComponent()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ResourcePoliciesPropertiesPage));
		periodLabel = new Label();
		pendingTimeoutLabel = new Label();
		failoverGroupCheckBox = new CheckBox();
		thresholdLabel = new Label();
		doNotRestartRadioButton = new RadioButton();
		restartRadioButton = new RadioButton();
		restartGroupBox = new SnapinGroupBox();
		periodTimePicker = new TimePicker();
		thresholdUpDown = new NumericUpDown();
		restartDelayLabel = new Label();
		restartDelayUpDown = new NumericUpDownNoCaret();
		retryPeriodCheckBox = new CheckBox();
		retryPeriodTimePicker = new TimePicker();
		helpLinkLabel = new LinkLabel();
		restartDelayToolTip = new ToolTip(components);
		label1 = new Label();
		pendingTimeoutGroupBox = new SnapinGroupBox();
		pendingTimeoutTimePicker = new TimePicker();
		((Control)(object)restartGroupBox).SuspendLayout();
		((ISupportInitialize)thresholdUpDown).BeginInit();
		((ISupportInitialize)restartDelayUpDown).BeginInit();
		((Control)(object)pendingTimeoutGroupBox).SuspendLayout();
		((Control)(object)this).SuspendLayout();
		periodLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(periodLabel, "periodLabel");
		periodLabel.Name = "periodLabel";
		pendingTimeoutLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(pendingTimeoutLabel, "pendingTimeoutLabel");
		pendingTimeoutLabel.Name = "pendingTimeoutLabel";
		failoverGroupCheckBox.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(failoverGroupCheckBox, "failoverGroupCheckBox");
		failoverGroupCheckBox.Name = "failoverGroupCheckBox";
		failoverGroupCheckBox.CheckedChanged += PropertiesChanged;
		thresholdLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(thresholdLabel, "thresholdLabel");
		thresholdLabel.Name = "thresholdLabel";
		doNotRestartRadioButton.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(doNotRestartRadioButton, "doNotRestartRadioButton");
		doNotRestartRadioButton.Name = "doNotRestartRadioButton";
		doNotRestartRadioButton.CheckedChanged += PropertiesChanged;
		restartRadioButton.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(restartRadioButton, "restartRadioButton");
		restartRadioButton.Name = "restartRadioButton";
		restartRadioButton.CheckedChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(restartGroupBox, "restartGroupBox");
		((Control)(object)restartGroupBox).Controls.Add(doNotRestartRadioButton);
		((Control)(object)restartGroupBox).Controls.Add(restartRadioButton);
		((Control)(object)restartGroupBox).Controls.Add(periodLabel);
		((Control)(object)restartGroupBox).Controls.Add((Control)(object)periodTimePicker);
		((Control)(object)restartGroupBox).Controls.Add(thresholdLabel);
		((Control)(object)restartGroupBox).Controls.Add(thresholdUpDown);
		((Control)(object)restartGroupBox).Controls.Add(restartDelayLabel);
		((Control)(object)restartGroupBox).Controls.Add((Control)(object)restartDelayUpDown);
		((Control)(object)restartGroupBox).Controls.Add(failoverGroupCheckBox);
		((Control)(object)restartGroupBox).Controls.Add(retryPeriodCheckBox);
		((Control)(object)restartGroupBox).Controls.Add((Control)(object)retryPeriodTimePicker);
		((Control)(object)restartGroupBox).Controls.Add(helpLinkLabel);
		((GroupBox)(object)restartGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)restartGroupBox).ForeColor = SystemColors.ControlText;
		((Control)(object)restartGroupBox).Name = "restartGroupBox";
		((GroupBox)(object)restartGroupBox).TabStop = false;
		periodTimePicker.DisplayUnits = (TimePickerUnits)1;
		componentResourceManager.ApplyResources(periodTimePicker, "periodTimePicker");
		((Control)(object)periodTimePicker).Name = "periodTimePicker";
		periodTimePicker.NotifyUser = null;
		periodTimePicker.Value = 0u;
		periodTimePicker.ValueChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(thresholdUpDown, "thresholdUpDown");
		thresholdUpDown.Maximum = new decimal(new int[4] { -1, 0, 0, 0 });
		thresholdUpDown.Name = "thresholdUpDown";
		thresholdUpDown.ValueChanged += PropertiesChanged;
		restartDelayLabel.AutoEllipsis = true;
		restartDelayLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(restartDelayLabel, "restartDelayLabel");
		restartDelayLabel.Name = "restartDelayLabel";
		componentResourceManager.ApplyResources(restartDelayUpDown, "restartDelayUpDown");
		restartDelayUpDown.DecimalIncrement = new decimal(new int[4] { 1, 0, 0, 65536 });
		((NumericUpDown)(object)restartDelayUpDown).DecimalPlaces = 1;
		((NumericUpDown)(object)restartDelayUpDown).Increment = new decimal(new int[4] { 10, 0, 0, 65536 });
		((NumericUpDown)(object)restartDelayUpDown).Maximum = new decimal(new int[4] { 3600, 0, 0, 0 });
		((Control)(object)restartDelayUpDown).Name = "restartDelayUpDown";
		((NumericUpDown)(object)restartDelayUpDown).ValueChanged += PropertiesChanged;
		retryPeriodCheckBox.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(retryPeriodCheckBox, "retryPeriodCheckBox");
		retryPeriodCheckBox.Name = "retryPeriodCheckBox";
		retryPeriodCheckBox.CheckedChanged += PropertiesChanged;
		retryPeriodTimePicker.DisplayUnits = (TimePickerUnits)1;
		componentResourceManager.ApplyResources(retryPeriodTimePicker, "retryPeriodTimePicker");
		((Control)(object)retryPeriodTimePicker).Name = "retryPeriodTimePicker";
		retryPeriodTimePicker.NotifyUser = null;
		retryPeriodTimePicker.Value = 0u;
		retryPeriodTimePicker.ValueChanged += PropertiesChanged;
		((Control)(object)retryPeriodTimePicker).EnabledChanged += OnRetryPeriodTimePickerEnabledChanged;
		componentResourceManager.ApplyResources(helpLinkLabel, "helpLinkLabel");
		helpLinkLabel.Name = "helpLinkLabel";
		helpLinkLabel.TabStop = true;
		helpLinkLabel.UseCompatibleTextRendering = true;
		helpLinkLabel.LinkClicked += OnHelpLinkClicked;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.ForeColor = SystemColors.ControlText;
		label1.Name = "label1";
		componentResourceManager.ApplyResources(pendingTimeoutGroupBox, "pendingTimeoutGroupBox");
		((Control)(object)pendingTimeoutGroupBox).Controls.Add(label1);
		((Control)(object)pendingTimeoutGroupBox).Controls.Add(pendingTimeoutLabel);
		((Control)(object)pendingTimeoutGroupBox).Controls.Add((Control)(object)pendingTimeoutTimePicker);
		((GroupBox)(object)pendingTimeoutGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)pendingTimeoutGroupBox).ForeColor = SystemColors.ControlText;
		((Control)(object)pendingTimeoutGroupBox).Name = "pendingTimeoutGroupBox";
		((GroupBox)(object)pendingTimeoutGroupBox).TabStop = false;
		pendingTimeoutTimePicker.DisplayUnits = (TimePickerUnits)1;
		componentResourceManager.ApplyResources(pendingTimeoutTimePicker, "pendingTimeoutTimePicker");
		((Control)(object)pendingTimeoutTimePicker).Name = "pendingTimeoutTimePicker";
		pendingTimeoutTimePicker.NotifyUser = null;
		pendingTimeoutTimePicker.Value = 0u;
		pendingTimeoutTimePicker.ValueChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)restartGroupBox);
		((Control)(object)this).Controls.Add((Control)(object)pendingTimeoutGroupBox);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "ResourcePoliciesPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex((Control)(object)pendingTimeoutGroupBox, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)restartGroupBox, 0);
		((Control)(object)restartGroupBox).ResumeLayout(performLayout: false);
		((ISupportInitialize)thresholdUpDown).EndInit();
		((ISupportInitialize)restartDelayUpDown).EndInit();
		((Control)(object)pendingTimeoutGroupBox).ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
