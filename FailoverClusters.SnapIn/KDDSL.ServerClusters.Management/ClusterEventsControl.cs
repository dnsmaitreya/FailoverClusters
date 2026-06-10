using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class ClusterEventsControl : SnapinUserControl
{
	private SplitContainer splitContainer;

	private Label eventDetailsTitle;

	private TableLayoutPanel eventDetailsTablePanel;

	private Label keywordsValueLabel;

	private Label nodeValueLabel;

	private Label taskValueLabel;

	private Label logNameValueLabel;

	private Label loggedValueLabel;

	private Label sourceValueLabel;

	private Label opCodeValueLabel;

	private Label eventIdValueLabel;

	private Label userValueLabel;

	private Label levelValueLabel;

	private SnapinPanel eventDetailsPanel;

	private EventLogEntryListView eventLogListView;

	private RichTextBox richTextBoxDescription;

	private TableLayoutPanel tableLayoutPanelMain;

	private IContainer components;

	public int EventCount => ((BaseListView)eventLogListView).VirtualListSize;

	public event EventHandler SortStart;

	public event AsyncCompletedEventHandler SortCompleted;

	public event AsyncCompletedEventHandler QueryCompleted;

	public event EventHandler QueryStarted;

	public ClusterEventsControl()
	{
		InitializeComponent();
	}

	public void Initialize(INotifyUser notifyUser)
	{
		eventLogListView.Initialize(notifyUser, this);
		((BaseListView)eventLogListView).SortStart += delegate(object sender, EventArgs e)
		{
			OnSortStarted(e);
		};
		eventLogListView.SortCompleted += delegate(object sender, AsyncCompletedEventArgs e)
		{
			OnSortCompleted(e);
		};
		eventLogListView.QueryCompleted += delegate(object sender, AsyncCompletedEventArgs e)
		{
			OnQueryCompleted(e);
		};
		eventLogListView.QueryStarted += delegate(object sender, EventArgs e)
		{
			if (this.QueryStarted != null)
			{
				this.QueryStarted(this, e);
			}
		};
	}

	public void SetInstanceId(Guid id)
	{
		((BaseListView)eventLogListView).SetInstanceId(id);
	}

	protected override void OnCreateControl()
	{
		((UserControl)this).OnCreateControl();
		eventDetailsTablePanel.ColumnStyles[0].SizeType = SizeType.Percent;
		eventDetailsTablePanel.ColumnStyles[0].Width = 15f;
		eventDetailsTablePanel.ColumnStyles[1].SizeType = SizeType.Percent;
		eventDetailsTablePanel.ColumnStyles[1].Width = 35f;
		eventDetailsTablePanel.ColumnStyles[2].SizeType = SizeType.Percent;
		eventDetailsTablePanel.ColumnStyles[2].Width = 15f;
		eventDetailsTablePanel.ColumnStyles[3].SizeType = SizeType.Percent;
		eventDetailsTablePanel.ColumnStyles[3].Width = 35f;
	}

	private void EventLogListView_KeyUp(object sender, KeyEventArgs e)
	{
		((Control)(object)this).OnKeyUp(e);
	}

	public void EnableFindHotKey(bool enabled)
	{
		eventLogListView.EnableFindHotKey(enabled);
	}

	public void QueryAsync(EventLogFilter filter)
	{
		eventLogListView.QueryAsync(filter);
	}

	public void Cancel()
	{
		eventLogListView.Cancel();
	}

	public void ResetSort()
	{
		((BaseListView)eventLogListView).ResetSort();
	}

	public DialogResult ShowFindDialog()
	{
		return eventLogListView.ShowFindDialog();
	}

	public void ChooseColumns()
	{
		eventLogListView.ChooseColumns();
	}

	public void ShowMessage(string message)
	{
		((BaseListView)eventLogListView).ShowMessage(message);
	}

	public void HideMessage()
	{
		((BaseListView)eventLogListView).HideMessage();
	}

	protected virtual void OnSortStarted(EventArgs e)
	{
		this.SortStart?.Invoke(this, e);
	}

	protected virtual void OnSortCompleted(AsyncCompletedEventArgs e)
	{
		this.SortCompleted?.Invoke(this, e);
	}

	protected virtual void OnQueryCompleted(AsyncCompletedEventArgs e)
	{
		this.QueryCompleted?.Invoke(this, e);
	}

	public void Show(EventLogEvent evt)
	{
		if (evt.IsDummyItem)
		{
			logNameValueLabel.Text = evt.DummyItemReason;
			sourceValueLabel.Text = evt.DummyItemReason;
			loggedValueLabel.Text = evt.DummyItemReason;
			eventIdValueLabel.Text = evt.DummyItemReason;
			taskValueLabel.Text = evt.DummyItemReason;
			levelValueLabel.Text = evt.DummyItemReason;
			keywordsValueLabel.Text = evt.DummyItemReason;
			userValueLabel.Text = evt.DummyItemReason;
			nodeValueLabel.Text = evt.DummyItemReason;
			opCodeValueLabel.Text = evt.DummyItemReason;
			richTextBoxDescription.Text = evt.Message;
		}
		else
		{
			logNameValueLabel.Text = evt.Channel;
			sourceValueLabel.Text = evt.Source;
			loggedValueLabel.Text = evt.TimeCreated.ToString("G", CultureInfo.CurrentCulture);
			eventIdValueLabel.Text = evt.EventId.ToString(CultureInfo.CurrentCulture);
			taskValueLabel.Text = evt.Task;
			levelValueLabel.Text = evt.LevelName;
			keywordsValueLabel.Text = evt.Keywords;
			userValueLabel.Text = evt.User;
			nodeValueLabel.Text = evt.Computer;
			opCodeValueLabel.Text = evt.OpCode;
			richTextBoxDescription.Text = evt.Message;
		}
	}

	public void Clear()
	{
		Clear(string.Empty);
	}

	public void Clear(string message)
	{
		logNameValueLabel.Text = message;
		sourceValueLabel.Text = message;
		loggedValueLabel.Text = message;
		eventIdValueLabel.Text = message;
		taskValueLabel.Text = message;
		levelValueLabel.Text = message;
		keywordsValueLabel.Text = message;
		userValueLabel.Text = message;
		nodeValueLabel.Text = message;
		opCodeValueLabel.Text = message;
		richTextBoxDescription.Text = message;
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ClusterEventsControl));
		splitContainer = new SplitContainer();
		eventLogListView = new EventLogEntryListView();
		eventDetailsPanel = new SnapinPanel();
		tableLayoutPanelMain = new TableLayoutPanel();
		eventDetailsTitle = new Label();
		eventDetailsTablePanel = new TableLayoutPanel();
		keywordsValueLabel = new Label();
		nodeValueLabel = new Label();
		taskValueLabel = new Label();
		logNameValueLabel = new Label();
		loggedValueLabel = new Label();
		sourceValueLabel = new Label();
		opCodeValueLabel = new Label();
		eventIdValueLabel = new Label();
		userValueLabel = new Label();
		levelValueLabel = new Label();
		richTextBoxDescription = new RichTextBox();
		Label label = new Label();
		Label label2 = new Label();
		Label label3 = new Label();
		Label label4 = new Label();
		Label label5 = new Label();
		Label label6 = new Label();
		Label label7 = new Label();
		Label label8 = new Label();
		Label label9 = new Label();
		Label label10 = new Label();
		((ISupportInitialize)splitContainer).BeginInit();
		splitContainer.Panel1.SuspendLayout();
		splitContainer.Panel2.SuspendLayout();
		splitContainer.SuspendLayout();
		((Control)(object)eventDetailsPanel).SuspendLayout();
		tableLayoutPanelMain.SuspendLayout();
		eventDetailsTablePanel.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(label, "logNameLabel");
		label.ForeColor = SystemColors.ControlText;
		label.Name = "logNameLabel";
		componentResourceManager.ApplyResources(label2, "keywordsLabel");
		label2.ForeColor = SystemColors.ControlText;
		label2.Name = "keywordsLabel";
		componentResourceManager.ApplyResources(label3, "sourceLabel");
		label3.ForeColor = SystemColors.ControlText;
		label3.Name = "sourceLabel";
		componentResourceManager.ApplyResources(label4, "nodeLabel");
		label4.ForeColor = SystemColors.ControlText;
		label4.Name = "nodeLabel";
		componentResourceManager.ApplyResources(label5, "taskLabel");
		label5.ForeColor = SystemColors.ControlText;
		label5.Name = "taskLabel";
		componentResourceManager.ApplyResources(label6, "eventIdLabel");
		label6.ForeColor = SystemColors.ControlText;
		label6.Name = "eventIdLabel";
		componentResourceManager.ApplyResources(label7, "opCodeLabel");
		label7.ForeColor = SystemColors.ControlText;
		label7.Name = "opCodeLabel";
		componentResourceManager.ApplyResources(label8, "loggedLabel");
		label8.ForeColor = SystemColors.ControlText;
		label8.Name = "loggedLabel";
		componentResourceManager.ApplyResources(label9, "levelLabel");
		label9.ForeColor = SystemColors.ControlText;
		label9.Name = "levelLabel";
		componentResourceManager.ApplyResources(label10, "userLabel");
		label10.ForeColor = SystemColors.ControlText;
		label10.Name = "userLabel";
		splitContainer.BorderStyle = BorderStyle.FixedSingle;
		componentResourceManager.ApplyResources(splitContainer, "splitContainer");
		splitContainer.FixedPanel = FixedPanel.Panel2;
		splitContainer.Name = "splitContainer";
		splitContainer.Panel1.Controls.Add((Control)(object)eventLogListView);
		splitContainer.Panel2.Controls.Add((Control)(object)eventDetailsPanel);
		((ListView)(object)eventLogListView).BorderStyle = BorderStyle.None;
		componentResourceManager.ApplyResources(eventLogListView, "eventLogListView");
		((BaseListView)eventLogListView).EnableAutoResizeColumns = true;
		((BaseListView)eventLogListView).HideSelection = true;
		((Control)(object)eventLogListView).Name = "eventLogListView";
		((ListView)(object)eventLogListView).UseCompatibleStateImageBehavior = false;
		((Control)(object)eventLogListView).KeyUp += EventLogListView_KeyUp;
		componentResourceManager.ApplyResources(eventDetailsPanel, "eventDetailsPanel");
		((Control)(object)eventDetailsPanel).BackColor = SystemColors.Control;
		((Control)(object)eventDetailsPanel).Controls.Add(tableLayoutPanelMain);
		((Control)(object)eventDetailsPanel).Name = "eventDetailsPanel";
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		tableLayoutPanelMain.Controls.Add(eventDetailsTitle, 0, 0);
		tableLayoutPanelMain.Controls.Add(eventDetailsTablePanel, 0, 2);
		tableLayoutPanelMain.Controls.Add(richTextBoxDescription, 0, 1);
		tableLayoutPanelMain.Name = "tableLayoutPanelMain";
		eventDetailsTitle.AutoEllipsis = true;
		eventDetailsTitle.BackColor = SystemColors.ControlDark;
		componentResourceManager.ApplyResources(eventDetailsTitle, "eventDetailsTitle");
		eventDetailsTitle.Name = "eventDetailsTitle";
		componentResourceManager.ApplyResources(eventDetailsTablePanel, "eventDetailsTablePanel");
		eventDetailsTablePanel.BackColor = SystemColors.Control;
		eventDetailsTablePanel.Controls.Add(keywordsValueLabel, 3, 3);
		eventDetailsTablePanel.Controls.Add(nodeValueLabel, 3, 4);
		eventDetailsTablePanel.Controls.Add(label, 0, 0);
		eventDetailsTablePanel.Controls.Add(taskValueLabel, 3, 2);
		eventDetailsTablePanel.Controls.Add(label2, 2, 3);
		eventDetailsTablePanel.Controls.Add(logNameValueLabel, 1, 0);
		eventDetailsTablePanel.Controls.Add(label3, 0, 1);
		eventDetailsTablePanel.Controls.Add(loggedValueLabel, 3, 1);
		eventDetailsTablePanel.Controls.Add(label4, 2, 4);
		eventDetailsTablePanel.Controls.Add(label5, 2, 2);
		eventDetailsTablePanel.Controls.Add(sourceValueLabel, 1, 1);
		eventDetailsTablePanel.Controls.Add(opCodeValueLabel, 1, 5);
		eventDetailsTablePanel.Controls.Add(label6, 0, 2);
		eventDetailsTablePanel.Controls.Add(eventIdValueLabel, 1, 2);
		eventDetailsTablePanel.Controls.Add(label7, 0, 5);
		eventDetailsTablePanel.Controls.Add(label8, 2, 1);
		eventDetailsTablePanel.Controls.Add(label9, 0, 3);
		eventDetailsTablePanel.Controls.Add(userValueLabel, 1, 4);
		eventDetailsTablePanel.Controls.Add(levelValueLabel, 1, 3);
		eventDetailsTablePanel.Controls.Add(label10, 0, 4);
		eventDetailsTablePanel.Name = "eventDetailsTablePanel";
		keywordsValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(keywordsValueLabel, "keywordsValueLabel");
		keywordsValueLabel.ForeColor = SystemColors.ControlText;
		keywordsValueLabel.Name = "keywordsValueLabel";
		nodeValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(nodeValueLabel, "nodeValueLabel");
		nodeValueLabel.ForeColor = SystemColors.ControlText;
		nodeValueLabel.Name = "nodeValueLabel";
		taskValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(taskValueLabel, "taskValueLabel");
		taskValueLabel.ForeColor = SystemColors.ControlText;
		taskValueLabel.Name = "taskValueLabel";
		logNameValueLabel.AutoEllipsis = true;
		eventDetailsTablePanel.SetColumnSpan(logNameValueLabel, 3);
		componentResourceManager.ApplyResources(logNameValueLabel, "logNameValueLabel");
		logNameValueLabel.ForeColor = SystemColors.ControlText;
		logNameValueLabel.Name = "logNameValueLabel";
		loggedValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(loggedValueLabel, "loggedValueLabel");
		loggedValueLabel.ForeColor = SystemColors.ControlText;
		loggedValueLabel.Name = "loggedValueLabel";
		sourceValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(sourceValueLabel, "sourceValueLabel");
		sourceValueLabel.ForeColor = SystemColors.ControlText;
		sourceValueLabel.Name = "sourceValueLabel";
		opCodeValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(opCodeValueLabel, "opCodeValueLabel");
		opCodeValueLabel.ForeColor = SystemColors.ControlText;
		opCodeValueLabel.Name = "opCodeValueLabel";
		eventIdValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(eventIdValueLabel, "eventIdValueLabel");
		eventIdValueLabel.ForeColor = SystemColors.ControlText;
		eventIdValueLabel.Name = "eventIdValueLabel";
		userValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(userValueLabel, "userValueLabel");
		userValueLabel.ForeColor = SystemColors.ControlText;
		userValueLabel.Name = "userValueLabel";
		levelValueLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(levelValueLabel, "levelValueLabel");
		levelValueLabel.ForeColor = SystemColors.ControlText;
		levelValueLabel.Name = "levelValueLabel";
		componentResourceManager.ApplyResources(richTextBoxDescription, "richTextBoxDescription");
		richTextBoxDescription.Name = "richTextBoxDescription";
		richTextBoxDescription.ReadOnly = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add(splitContainer);
		((Control)this).Name = "ClusterEventsControl";
		splitContainer.Panel1.ResumeLayout(performLayout: false);
		splitContainer.Panel2.ResumeLayout(performLayout: false);
		((ISupportInitialize)splitContainer).EndInit();
		splitContainer.ResumeLayout(performLayout: false);
		((Control)(object)eventDetailsPanel).ResumeLayout(performLayout: false);
		tableLayoutPanelMain.ResumeLayout(performLayout: false);
		eventDetailsTablePanel.ResumeLayout(performLayout: false);
		eventDetailsTablePanel.PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
