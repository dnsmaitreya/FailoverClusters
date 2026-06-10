using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class FindNextDialog : SnapinForm
{
	private INotifyUser notifyUser;

	private EventLogEntryListView eventLogListView;

	private Cursor savedCursor;

	private bool newSearch;

	private bool searching;

	private IContainer components;

	private Button cancelButton;

	private Button findButton;

	private TextBox findTextBox;

	private Label FindLabel;

	private SnapinGroupBox directionGroupBox;

	private RadioButton downButton;

	private RadioButton upButton;

	private CheckBox matchCaseCheckBox;

	public FindNextDialog(EventLogEntryListView eventLogListView)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		InitializeComponent();
		notifyUser = (INotifyUser)new MessageBoxNotifyUser((IWin32Window)this);
		this.eventLogListView = eventLogListView;
		this.eventLogListView.FindCompleted += EventLogListViewFindCompleted;
		findButton.Enabled = false;
	}

	private void FindTextBoxTextChanged(object sender, EventArgs e)
	{
		findButton.Enabled = findTextBox.Text.Trim().Length > 0;
		newSearch = true;
	}

	private void FindNextButtonClick(object sender, EventArgs e)
	{
		try
		{
			StringComparison comparison = (matchCaseCheckBox.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
			FindDirection findDirection = ((!downButton.Checked) ? FindDirection.Backward : FindDirection.Forward);
			int num = ((((ListView)(object)eventLogListView).SelectedIndices.Count != 0) ? ((ListView)(object)eventLogListView).SelectedIndices[0] : 0);
			if (newSearch)
			{
				newSearch = false;
			}
			else
			{
				num += ((findDirection == FindDirection.Forward) ? 1 : (-1));
			}
			if (0 <= num && num < ((BaseListView)eventLogListView).VirtualListSize)
			{
				findTextBox.Enabled = false;
				findButton.Enabled = false;
				((Control)(object)directionGroupBox).Enabled = false;
				savedCursor = ((Control)(object)this).Cursor;
				((Control)(object)this).Cursor = Cursors.WaitCursor;
				searching = true;
				eventLogListView.FindAsync(findTextBox.Text.Trim(), num, findDirection, comparison);
			}
			else
			{
				notifyUser.ShowError(Resources.EndOfEvents_Text);
			}
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.CannotExecuteFind_Text);
		}
	}

	private void EventLogListViewFindCompleted(object sender, AsyncFindCompletedEventArgs e)
	{
		try
		{
			if (!e.Cancelled)
			{
				if (e.Error != null)
				{
					notifyUser.ShowError(e.Error, Resources.CannotExecuteFind_Text);
				}
				else if (e.ItemIndex < 0)
				{
					notifyUser.ShowWarning(Resources.SearchStringNotFound_Text);
				}
			}
			findTextBox.Enabled = true;
			findButton.Enabled = true;
			((Control)(object)directionGroupBox).Enabled = true;
			((Control)(object)this).Cursor = savedCursor;
			searching = false;
			findTextBox.Focus();
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.CannotExecuteFind_Text);
		}
	}

	private void CancelButtonClick(object sender, EventArgs e)
	{
		Cancel();
	}

	private void ControlKeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
		{
			Cancel();
		}
	}

	private void Cancel()
	{
		if (searching)
		{
			eventLogListView.Cancel();
		}
		else
		{
			((Form)this).DialogResult = DialogResult.Cancel;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FindNextDialog));
		cancelButton = new Button();
		findButton = new Button();
		findTextBox = new TextBox();
		FindLabel = new Label();
		directionGroupBox = new SnapinGroupBox();
		downButton = new RadioButton();
		upButton = new RadioButton();
		matchCaseCheckBox = new CheckBox();
		((Control)(object)directionGroupBox).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		cancelButton.Click += CancelButtonClick;
		cancelButton.KeyUp += ControlKeyUp;
		componentResourceManager.ApplyResources(findButton, "findButton");
		findButton.Name = "findButton";
		findButton.Click += FindNextButtonClick;
		findButton.KeyUp += ControlKeyUp;
		componentResourceManager.ApplyResources(findTextBox, "findTextBox");
		findTextBox.Name = "findTextBox";
		findTextBox.TextChanged += FindTextBoxTextChanged;
		findTextBox.KeyUp += ControlKeyUp;
		componentResourceManager.ApplyResources(FindLabel, "FindLabel");
		FindLabel.ForeColor = SystemColors.ControlText;
		FindLabel.Name = "FindLabel";
		componentResourceManager.ApplyResources(directionGroupBox, "directionGroupBox");
		((Control)(object)directionGroupBox).Controls.Add(downButton);
		((Control)(object)directionGroupBox).Controls.Add(upButton);
		((GroupBox)(object)directionGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)directionGroupBox).Name = "directionGroupBox";
		((GroupBox)(object)directionGroupBox).TabStop = false;
		componentResourceManager.ApplyResources(downButton, "downButton");
		downButton.Checked = true;
		downButton.Name = "downButton";
		downButton.TabStop = true;
		downButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(upButton, "upButton");
		upButton.Name = "upButton";
		upButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(matchCaseCheckBox, "matchCaseCheckBox");
		matchCaseCheckBox.Name = "matchCaseCheckBox";
		matchCaseCheckBox.UseVisualStyleBackColor = true;
		((Form)this).AcceptButton = findButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add(matchCaseCheckBox);
		((Control)this).Controls.Add((Control)(object)directionGroupBox);
		((Control)this).Controls.Add(findTextBox);
		((Control)this).Controls.Add(FindLabel);
		((Control)this).Controls.Add(findButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Name = "FindNextDialog";
		((Control)this).KeyUp += ControlKeyUp;
		((Control)(object)directionGroupBox).ResumeLayout(performLayout: false);
		((Control)(object)directionGroupBox).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
