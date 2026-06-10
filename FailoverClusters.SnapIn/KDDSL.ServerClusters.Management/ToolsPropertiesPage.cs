using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using KDDSL.ServerClusters.Wizards;

namespace KDDSL.ServerClusters.Management;

internal class ToolsPropertiesPage : PropertyPageControlBase
{
	private SnapInSettings snapinSettings;

	private CursorManager cursorManager;

	private BackgroundOperation<object, object> clearSettingsOp;

	private BackgroundOperation<object, object> clearMruOp;

	private InformationLabelControl infoLabel;

	private GroupBox grpBoxConnections;

	private GroupBox grpBoxPrefrences;

	private Button btnResetPrefrences;

	private Button btnConnections;

	private Label labelConnections;

	private Label labelPrefrences;

	private LinkLabel linkLabelGC;

	private IContainer components;

	internal ToolsPropertiesPage(SnapInSettings settings)
		: base(Resources.Tools_Text)
	{
		snapinSettings = settings;
		clearMruOp = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)delegate
		{
			snapinSettings.ClearSnapinSettings();
			return null;
		});
		clearMruOp.OperationCompleted += ClearMruCompleted;
		clearSettingsOp = new BackgroundOperation<object, object>((BackgroundOperationFunction<object, object>)delegate
		{
			UserSettings.RemoveUserSettings();
			return null;
		});
		clearSettingsOp.OperationCompleted += ClearSettingsCompleted;
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
	}

	protected override bool ValidateProperties()
	{
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
	}

	protected override void CompleteSaveProperties()
	{
	}

	protected override void InitializePage()
	{
		cursorManager = new CursorManager((Control)(object)this);
		if (DebugLog.PrivateComponentsEnabled)
		{
			((Control)(object)infoLabel).Location = new Point(7, 298);
			linkLabelGC = new LinkLabel();
			linkLabelGC.Name = "linkLabelGC";
			linkLabelGC.AutoSize = true;
			linkLabelGC.Location = new Point(341, 357);
			linkLabelGC.Size = new Size(39, 13);
			linkLabelGC.TabIndex = 4;
			linkLabelGC.Text = "Collect";
			linkLabelGC.Parent = (Control)(object)this;
			linkLabelGC.TabStop = true;
			linkLabelGC.LinkClicked += LinkLabelGC_LinkClicked;
			((Control)(object)this).Controls.Add(linkLabelGC);
			((Control)(object)this).Controls.SetChildIndex(linkLabelGC, 0);
		}
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
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ToolsPropertiesPage));
		infoLabel = new InformationLabelControl();
		grpBoxConnections = new GroupBox();
		labelConnections = new Label();
		btnConnections = new Button();
		grpBoxPrefrences = new GroupBox();
		labelPrefrences = new Label();
		btnResetPrefrences = new Button();
		grpBoxConnections.SuspendLayout();
		grpBoxPrefrences.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(infoLabel, "infoLabel");
		((Control)(object)infoLabel).Name = "infoLabel";
		grpBoxConnections.Controls.Add(labelConnections);
		grpBoxConnections.Controls.Add(btnConnections);
		componentResourceManager.ApplyResources(grpBoxConnections, "grpBoxConnections");
		grpBoxConnections.Name = "grpBoxConnections";
		grpBoxConnections.TabStop = false;
		labelConnections.AutoEllipsis = true;
		componentResourceManager.ApplyResources(labelConnections, "labelConnections");
		labelConnections.Name = "labelConnections";
		componentResourceManager.ApplyResources(btnConnections, "btnConnections");
		btnConnections.Name = "btnConnections";
		btnConnections.UseVisualStyleBackColor = true;
		btnConnections.Click += OnResetConnections;
		grpBoxPrefrences.Controls.Add(labelPrefrences);
		grpBoxPrefrences.Controls.Add(btnResetPrefrences);
		componentResourceManager.ApplyResources(grpBoxPrefrences, "grpBoxPrefrences");
		grpBoxPrefrences.Name = "grpBoxPrefrences";
		grpBoxPrefrences.TabStop = false;
		labelPrefrences.AutoEllipsis = true;
		componentResourceManager.ApplyResources(labelPrefrences, "labelPrefrences");
		labelPrefrences.Name = "labelPrefrences";
		componentResourceManager.ApplyResources(btnResetPrefrences, "btnResetPrefrences");
		btnResetPrefrences.Name = "btnResetPrefrences";
		btnResetPrefrences.UseVisualStyleBackColor = true;
		btnResetPrefrences.Click += OnResetPrefrences;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(grpBoxConnections);
		((Control)(object)this).Controls.Add(grpBoxPrefrences);
		((Control)(object)this).Controls.Add((Control)(object)infoLabel);
		((Control)(object)this).Name = "ToolsPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex((Control)(object)infoLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(grpBoxPrefrences, 0);
		((Control)(object)this).Controls.SetChildIndex(grpBoxConnections, 0);
		grpBoxConnections.ResumeLayout(performLayout: false);
		grpBoxPrefrences.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}

	private void OnResetConnections(object sender, EventArgs e)
	{
		cursorManager.BeginCursor(CursorType.DataLoad);
		clearMruOp.QueueOperation((object)null);
	}

	private void OnResetPrefrences(object sender, EventArgs e)
	{
		cursorManager.BeginCursor(CursorType.DataLoad);
		clearSettingsOp.QueueOperation((object)null);
	}

	private void ClearSettingsCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		cursorManager.EndCursor();
		if (!e.Cancelled)
		{
			if (e.Error != null)
			{
				infoLabel.SetErrorText(ExceptionHelp.GetExceptionMessage(e.Error));
			}
			else
			{
				infoLabel.SetInformationText(Resources.SnapinSettingsRemoved_Text);
			}
		}
	}

	private void ClearMruCompleted(object sender, BackgroundOperationCompletedEventArgs<object, object> e)
	{
		cursorManager.EndCursor();
		if (!e.Cancelled)
		{
			if (e.Error != null)
			{
				infoLabel.SetErrorText(ExceptionHelp.GetExceptionMessage(e.Error));
			}
			else
			{
				infoLabel.SetInformationText(Resources.SnapinMruCleared_Text);
			}
		}
	}

	private void LinkLabelGC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		try
		{
			Background.QueueWorker((WaitCallback)delegate
			{
				GC.Collect();
			});
		}
		catch (Exception)
		{
		}
	}
}
