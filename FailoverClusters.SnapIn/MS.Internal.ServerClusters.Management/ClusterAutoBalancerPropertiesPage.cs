using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using FailoverClusters.Framework;

namespace MS.Internal.ServerClusters.Management;

internal class ClusterAutoBalancerPropertiesPage : PropertyPageControlBase
{
	private readonly ClusterContext context;

	private ClusterPropertyCollection properties;

	private AutoBalancerMode autoBalancerModeValue;

	private AutoBalancerLevel autoBalancerLevelValue;

	private bool isAutoBalancerDirty;

	private IContainer components;

	private CheckBox checkBoxAutoBalanceEnable;

	private GroupBox gbAutoBalancerModes;

	private RadioButton rbBalanceAlways;

	private RadioButton rbBalanceOnNodeUp;

	private GroupBox gbAutoBalancerLevels;

	private RadioButton rbLow;

	private RadioButton rbMedium;

	private RadioButton rbHigh;

	public ClusterAutoBalancerPropertiesPage(ClusterContext context)
		: base(Resources.AutoBalancer_Title)
	{
		InitializeComponent();
		this.context = context;
		checkBoxAutoBalanceEnable.Text = Resources.AutoBalancer_EnableText;
		gbAutoBalancerModes.Text = Resources.AutoBalancer_ModesLabel;
		rbBalanceOnNodeUp.Text = Resources.AutoBalancer_NodeUpText;
		rbBalanceAlways.Text = Resources.AutoBalancer_Always;
		gbAutoBalancerLevels.Text = Resources.AutoBalancer_LevelsLabel;
		rbLow.Text = Resources.AutoBalancer_Level_Low;
		rbMedium.Text = Resources.AutoBalancer_Level_Medium;
		rbHigh.Text = Resources.AutoBalancer_Level_High;
	}

	private void OnAutoBalanceEnabledOrDisabled(object sender, EventArgs e)
	{
		gbAutoBalancerModes.Enabled = checkBoxAutoBalanceEnable.Checked;
		gbAutoBalancerLevels.Enabled = checkBoxAutoBalanceEnable.Checked;
		bool flag2 = (base.IsDirty = true);
		isAutoBalancerDirty = flag2;
		if (checkBoxAutoBalanceEnable.Checked && !rbBalanceOnNodeUp.Checked && !rbBalanceAlways.Checked)
		{
			rbBalanceOnNodeUp.Checked = true;
		}
		if (checkBoxAutoBalanceEnable.Checked && !rbLow.Checked && !rbMedium.Checked && !rbHigh.Checked)
		{
			rbLow.Checked = true;
		}
	}

	private void OnAutoBalanceChanged(object sender, EventArgs e)
	{
		bool flag2 = (base.IsDirty = true);
		isAutoBalancerDirty = flag2;
	}

	protected override void LoadProperties()
	{
		properties = context.FrameworkCluster.Properties;
		ClusterProperty autoBalancerModeProperty = properties["AutoBalancerMode"];
		ClusterProperty autoBalancerLevelProperty = properties["AutoBalancerLevel"];
		if (autoBalancerModeProperty == null || autoBalancerLevelProperty == null)
		{
			CountdownEvent countDown = new CountdownEvent(1);
			context.FrameworkCluster.LoadPropertiesAsync(2, delegate(OperationResult result)
			{
				if (result.Error == null)
				{
					properties = context.FrameworkCluster.Properties;
					autoBalancerModeProperty = properties["AutoBalancerMode"];
					autoBalancerLevelProperty = properties["AutoBalancerLevel"];
				}
				countDown.Signal();
			});
			countDown.Wait();
		}
		if (autoBalancerModeProperty != null && autoBalancerModeProperty.PropertyType == FailoverClusters.Framework.ClusterPropertyType.UnsignedInt)
		{
			autoBalancerModeValue = (AutoBalancerMode)(uint)autoBalancerModeProperty.Value;
			if (autoBalancerLevelProperty != null && autoBalancerLevelProperty.PropertyType == FailoverClusters.Framework.ClusterPropertyType.UnsignedInt)
			{
				autoBalancerLevelValue = (AutoBalancerLevel)(uint)autoBalancerLevelProperty.Value;
				return;
			}
			throw new ClusterPropertyNotFoundException("AutoBalancerLevel");
		}
		throw new ClusterPropertyNotFoundException("AutoBalancerMode");
	}

	protected override void InitializePage()
	{
		if (autoBalancerModeValue == AutoBalancerMode.Disabled)
		{
			checkBoxAutoBalanceEnable.Checked = false;
			gbAutoBalancerModes.Enabled = false;
			gbAutoBalancerLevels.Enabled = false;
			rbBalanceOnNodeUp.Checked = false;
			rbBalanceAlways.Checked = false;
		}
		else
		{
			checkBoxAutoBalanceEnable.Checked = true;
			gbAutoBalancerModes.Enabled = true;
			gbAutoBalancerLevels.Enabled = true;
			rbBalanceOnNodeUp.Checked = autoBalancerModeValue == AutoBalancerMode.NodeUp;
			rbBalanceAlways.Checked = autoBalancerModeValue == AutoBalancerMode.Always;
		}
		rbLow.Checked = autoBalancerLevelValue == AutoBalancerLevel.Low;
		rbMedium.Checked = autoBalancerLevelValue == AutoBalancerLevel.Medium;
		rbHigh.Checked = autoBalancerLevelValue == AutoBalancerLevel.High;
		checkBoxAutoBalanceEnable.CheckedChanged += OnAutoBalanceEnabledOrDisabled;
		rbBalanceOnNodeUp.CheckedChanged += OnAutoBalanceChanged;
		rbBalanceAlways.CheckedChanged += OnAutoBalanceChanged;
		rbLow.CheckedChanged += OnAutoBalanceChanged;
		rbMedium.CheckedChanged += OnAutoBalanceChanged;
		rbHigh.CheckedChanged += OnAutoBalanceChanged;
		isAutoBalancerDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (checkBoxAutoBalanceEnable.Checked && ((!rbBalanceOnNodeUp.Checked && !rbBalanceAlways.Checked) || (!rbLow.Checked && !rbMedium.Checked && !rbHigh.Checked)))
		{
			return false;
		}
		AutoBalancerMode autoBalancerMode = (checkBoxAutoBalanceEnable.Checked ? (rbBalanceOnNodeUp.Checked ? AutoBalancerMode.NodeUp : AutoBalancerMode.Always) : AutoBalancerMode.Disabled);
		AutoBalancerLevel autoBalancerLevel = (rbLow.Checked ? AutoBalancerLevel.Low : (rbMedium.Checked ? AutoBalancerLevel.Medium : AutoBalancerLevel.High));
		isAutoBalancerDirty = autoBalancerModeValue != autoBalancerMode || autoBalancerLevelValue != autoBalancerLevel;
		autoBalancerModeValue = autoBalancerMode;
		autoBalancerLevelValue = autoBalancerLevel;
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			if (!isAutoBalancerDirty)
			{
				return;
			}
			properties["AutoBalancerMode"].SetValue((uint)autoBalancerModeValue);
			properties["AutoBalancerLevel"].SetValue((uint)autoBalancerLevelValue);
			properties.Commit(delegate(OperationResult commitResult)
			{
				if (commitResult.Error != null)
				{
					throw commitResult.Error.InnerException;
				}
			});
			isAutoBalancerDirty = false;
		}
		catch (ClusterException ex)
		{
			ExceptionHelp.LogException(ex, "Error saving cluster auto balancer properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.AutoBalancer_ErrorText,
				context.DisplayName
			});
		}
	}

	protected override void CompleteSaveProperties()
	{
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		checkBoxAutoBalanceEnable = new CheckBox();
		gbAutoBalancerModes = new GroupBox();
		rbBalanceAlways = new RadioButton();
		rbBalanceOnNodeUp = new RadioButton();
		gbAutoBalancerLevels = new GroupBox();
		rbLow = new RadioButton();
		rbMedium = new RadioButton();
		rbHigh = new RadioButton();
		gbAutoBalancerModes.SuspendLayout();
		gbAutoBalancerLevels.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		checkBoxAutoBalanceEnable.AutoSize = true;
		checkBoxAutoBalanceEnable.Location = new Point(30, 30);
		checkBoxAutoBalanceEnable.Name = "checkBoxAutoBalanceEnable";
		checkBoxAutoBalanceEnable.Size = new Size(99, 17);
		checkBoxAutoBalanceEnable.TabIndex = 4;
		checkBoxAutoBalanceEnable.Text = "Enable/Disable";
		checkBoxAutoBalanceEnable.UseVisualStyleBackColor = true;
		gbAutoBalancerModes.Controls.Add(rbBalanceAlways);
		gbAutoBalancerModes.Controls.Add(rbBalanceOnNodeUp);
		gbAutoBalancerModes.Location = new Point(30, 61);
		gbAutoBalancerModes.Name = "gbAutoBalancerModes";
		gbAutoBalancerModes.Size = new Size(318, 102);
		gbAutoBalancerModes.TabIndex = 5;
		gbAutoBalancerModes.TabStop = false;
		gbAutoBalancerModes.Text = "Mode";
		rbBalanceAlways.AutoSize = true;
		rbBalanceAlways.Location = new Point(20, 63);
		rbBalanceAlways.Name = "rbBalanceAlways";
		rbBalanceAlways.Size = new Size(31, 17);
		rbBalanceAlways.TabIndex = 1;
		rbBalanceAlways.TabStop = true;
		rbBalanceAlways.Text = "_";
		rbBalanceAlways.UseVisualStyleBackColor = true;
		rbBalanceOnNodeUp.AutoSize = true;
		rbBalanceOnNodeUp.Location = new Point(20, 28);
		rbBalanceOnNodeUp.Name = "rbBalanceOnNodeUp";
		rbBalanceOnNodeUp.Size = new Size(31, 17);
		rbBalanceOnNodeUp.TabIndex = 0;
		rbBalanceOnNodeUp.TabStop = true;
		rbBalanceOnNodeUp.Text = "_";
		rbBalanceOnNodeUp.UseVisualStyleBackColor = true;
		gbAutoBalancerLevels.Controls.Add(rbLow);
		gbAutoBalancerLevels.Controls.Add(rbMedium);
		gbAutoBalancerLevels.Controls.Add(rbHigh);
		gbAutoBalancerLevels.Location = new Point(30, 199);
		gbAutoBalancerLevels.Name = "gbAutoBalancerLevels";
		gbAutoBalancerLevels.Size = new Size(318, 135);
		gbAutoBalancerLevels.TabIndex = 5;
		gbAutoBalancerLevels.TabStop = false;
		gbAutoBalancerLevels.Text = "Levels";
		rbLow.AutoSize = true;
		rbLow.Location = new Point(20, 98);
		rbLow.Name = "rbLow";
		rbLow.Size = new Size(31, 17);
		rbLow.TabIndex = 1;
		rbLow.TabStop = true;
		rbLow.Text = "_";
		rbLow.UseVisualStyleBackColor = true;
		rbMedium.AutoSize = true;
		rbMedium.Location = new Point(20, 63);
		rbMedium.Name = "rbMedium";
		rbMedium.Size = new Size(31, 17);
		rbMedium.TabIndex = 1;
		rbMedium.TabStop = true;
		rbMedium.Text = "_";
		rbMedium.UseVisualStyleBackColor = true;
		rbHigh.AutoSize = true;
		rbHigh.Location = new Point(20, 28);
		rbHigh.Name = "rbHigh";
		rbHigh.Size = new Size(31, 17);
		rbHigh.TabIndex = 0;
		rbHigh.TabStop = true;
		rbHigh.Text = "_";
		rbHigh.UseVisualStyleBackColor = true;
		((ContainerControl)(object)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(gbAutoBalancerLevels);
		((Control)(object)this).Controls.Add(gbAutoBalancerModes);
		((Control)(object)this).Controls.Add(checkBoxAutoBalanceEnable);
		((Control)(object)this).Name = "ClusterAutoBalancerPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(checkBoxAutoBalanceEnable, 0);
		((Control)(object)this).Controls.SetChildIndex(gbAutoBalancerModes, 0);
		((Control)(object)this).Controls.SetChildIndex(gbAutoBalancerLevels, 0);
		gbAutoBalancerModes.ResumeLayout(performLayout: false);
		gbAutoBalancerModes.PerformLayout();
		gbAutoBalancerLevels.ResumeLayout(performLayout: false);
		gbAutoBalancerLevels.PerformLayout();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

