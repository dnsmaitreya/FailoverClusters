using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FailoverClusters.UI.Controls;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class ErrorStartPageControl : StartPageContainerControl
{
	private Exception exception;

	private IContainer components;

	private Label messageLabel;

	private Label titleLabel;

	private PictureBox pictureBox;

	private Button restoreViewButton;

	private FlowLayoutPanel flowLayoutPanel;

	private Panel messagePanel;

	private TitleBarControl titleBarControl;

	public ErrorStartPageControl()
	{
		InitializeComponent();
		exception = null;
	}

	protected override void InitializeInternal(FormView view)
	{
		base.InitializeInternal(view);
		ErrorMessageViewData errorMessageViewData = (ErrorMessageViewData)Utilities.GetTagFromTag(view.ViewDescriptionTag);
		ActionBase item = ActionFactory.CreateAction(Resources.Action_ShowForm_Text, Resources.Action_ShowForm_Description_Text, Icons.ErrorIndex, OnRestoreView);
		view.ActionsPaneItems.Add(item);
		base.RefreshOnShow = false;
		SetError(errorMessageViewData.Exception);
	}

	private void SetError(Exception e)
	{
		WinFormsHelp.SetPictureBoxImage(pictureBox, Icons.Error);
		titleLabel.Text = Resources.UnexpectedError_Text;
		exception = e;
		string text = ((!ExceptionHelp.IsFirstExceptionFound<ClusterObjectDeletedException>(e)) ? GetStandardExceptionMessage(e) : GetDeletedObjectExceptionMessage());
		messageLabel.Text = text;
	}

	private void OnFlowLayoutPanelSizeChanged(object sender, EventArgs e)
	{
		messagePanel.MinimumSize = flowLayoutPanel.Size;
	}

	private string GetDeletedObjectExceptionMessage()
	{
		return Resources.ErrorMessageView_DeletedObject_Text;
	}

	private string GetStandardExceptionMessage(Exception e)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Resources.ErrorMessageView_Instructions_Text);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(ExceptionHelp.GetExceptionMessage(e));
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Environment.NewLine);
		if (DebugLog.ExtraExceptionData)
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Resources.DetailedError_Text);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(ExceptionHelp.GetExceptionDetails(e));
		}
		return stringBuilder.ToString();
	}

	private void OnRestoreView(object sender, SnapinActionEventArgs e)
	{
		RestoreView();
	}

	private void restoreViewButton_Click(object sender, EventArgs e)
	{
		RestoreView();
	}

	public override void OnRefreshView(UpdateReason reason)
	{
		RestoreView();
	}

	private void RestoreView()
	{
		base.CluAdminScopeNode.RestoreForm(base.View);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ErrorStartPageControl));
		titleBarControl = new TitleBarControl();
		pictureBox = new PictureBox();
		titleLabel = new Label();
		messageLabel = new Label();
		restoreViewButton = new Button();
		flowLayoutPanel = new FlowLayoutPanel();
		messagePanel = new Panel();
		((ISupportInitialize)pictureBox).BeginInit();
		flowLayoutPanel.SuspendLayout();
		messagePanel.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(titleBarControl, "titleBarControl");
		((Control)(object)titleBarControl).MinimumSize = new Size(20, 27);
		((Control)(object)titleBarControl).Name = "titleBarControl";
		componentResourceManager.ApplyResources(pictureBox, "pictureBox");
		pictureBox.Name = "pictureBox";
		pictureBox.TabStop = false;
		componentResourceManager.ApplyResources(titleLabel, "titleLabel");
		titleLabel.FlatStyle = FlatStyle.System;
		titleLabel.Name = "titleLabel";
		componentResourceManager.ApplyResources(messageLabel, "messageLabel");
		messageLabel.FlatStyle = FlatStyle.System;
		messageLabel.Name = "messageLabel";
		restoreViewButton.AutoEllipsis = true;
		componentResourceManager.ApplyResources(restoreViewButton, "restoreViewButton");
		restoreViewButton.Name = "restoreViewButton";
		restoreViewButton.UseVisualStyleBackColor = true;
		restoreViewButton.Click += restoreViewButton_Click;
		componentResourceManager.ApplyResources(flowLayoutPanel, "flowLayoutPanel");
		flowLayoutPanel.Controls.Add(titleLabel);
		flowLayoutPanel.Controls.Add(messageLabel);
		flowLayoutPanel.Controls.Add(restoreViewButton);
		flowLayoutPanel.Name = "flowLayoutPanel";
		flowLayoutPanel.SizeChanged += OnFlowLayoutPanelSizeChanged;
		messagePanel.Controls.Add(flowLayoutPanel);
		componentResourceManager.ApplyResources(messagePanel, "messagePanel");
		messagePanel.Name = "messagePanel";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(messagePanel);
		((Control)(object)this).Controls.Add(pictureBox);
		((Control)(object)this).Controls.Add((Control)(object)titleBarControl);
		((Control)(object)this).Name = "ErrorStartPageControl";
		((ISupportInitialize)pictureBox).EndInit();
		flowLayoutPanel.ResumeLayout(performLayout: false);
		flowLayoutPanel.PerformLayout();
		messagePanel.ResumeLayout(performLayout: false);
		messagePanel.PerformLayout();
		((Control)(object)this).ResumeLayout(performLayout: false);
	}
}

