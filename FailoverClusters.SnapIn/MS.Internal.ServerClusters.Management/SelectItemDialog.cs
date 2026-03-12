using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class SelectItemDialog : SnapinForm
{
	private class EnumerationResult
	{
		private ICollection<ClusterListItem> resultItems;

		private Exception resultError;

		internal ICollection<ClusterListItem> Items => resultItems;

		internal Exception Error => resultError;

		public EnumerationResult(ICollection<ClusterListItem> items, Exception error)
		{
			resultItems = items;
			resultError = error;
		}
	}

	private ClusterList listView;

	private Button okButton;

	private Label introLabel;

	private IContainer components;

	private SelectItemStrategy strategy;

	private object lockObj;

	private EnumerationResult result;

	private bool hasCompleted;

	public ClusterListItem SelectedItem => listView.SelectedItem;

	public SelectItemDialog()
	{
		InitializeComponent();
	}

	public SelectItemDialog(SelectItemStrategy strategy)
	{
		InitializeComponent();
		lockObj = new object();
		result = null;
		hasCompleted = false;
		((Control)(object)this).Text = strategy.Title;
		introLabel.Text = strategy.Instructions;
		listView.SetColumns(strategy.ColumnNames);
		listView.EmptyText = strategy.FetchItemsMessage;
		((Control)this).VisibleChanged += OnVisibleChanged;
		UpdateOkButton();
		this.strategy = strategy;
		this.strategy.EnumerationCompleted += strategy_EnumerationCompleted;
		this.strategy.EnumerationResultsReady += strategy_EnumerationResultsReady;
		Background.QueueWorker((WaitCallback)StartItemEnum);
	}

	private void OnVisibleChanged(object sender, EventArgs e)
	{
		if (!((Control)this).Visible)
		{
			return;
		}
		lock (lockObj)
		{
			if (result != null)
			{
				OnEnumerationResultsReady(result.Items, result.Error);
				result = null;
			}
			if (hasCompleted)
			{
				OnResultsCompleted();
				hasCompleted = false;
			}
		}
	}

	private void StartItemEnum(object data)
	{
		strategy.StartItemEnumeration();
	}

	private void OnEnumerationResultsReady(ICollection<ClusterListItem> items, Exception error)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (items != null)
		{
			listView.AddRange(items);
		}
		else
		{
			((INotifyUser)new MessageBoxNotifyUser((IWin32Window)this)).ShowError(error, string.Empty);
		}
	}

	private void strategy_EnumerationResultsReady(object sender, EnumerationResultsEventArgs e)
	{
		if (((Control)this).Visible)
		{
			OnEnumerationResultsReady(e.Items, e.Error);
		}
		else
		{
			QueueResults(e.Items, e.Error);
		}
	}

	private void QueueResults(ICollection<ClusterListItem> items, Exception error)
	{
		lock (lockObj)
		{
			result = new EnumerationResult(items, error);
		}
	}

	private void QueueCompleted()
	{
		lock (lockObj)
		{
			hasCompleted = true;
		}
	}

	private void OnResultsCompleted()
	{
		listView.EmptyText = strategy.NoItemsMessage;
	}

	private void strategy_EnumerationCompleted(object sender, EventArgs e)
	{
		if (((Control)this).Visible)
		{
			OnResultsCompleted();
		}
		else
		{
			QueueCompleted();
		}
	}

	private void listView_SelectedItemChanged(object sender, EventArgs e)
	{
		UpdateOkButton();
	}

	private void UpdateOkButton()
	{
		if (listView.SelectedItem != null)
		{
			okButton.Enabled = true;
		}
		else
		{
			okButton.Enabled = false;
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SelectItemDialog));
		introLabel = new Label();
		listView = new ClusterList();
		okButton = new Button();
		Button button = new Button();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(button, "cancelButton");
		button.DialogResult = DialogResult.Cancel;
		button.Name = "cancelButton";
		button.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(introLabel, "introLabel");
		introLabel.AutoEllipsis = true;
		introLabel.Name = "introLabel";
		componentResourceManager.ApplyResources(listView, "listView");
		listView.CheckBoxes = false;
		listView.HeaderStyle = ColumnHeaderStyle.Clickable;
		listView.IsSortable = true;
		((Control)(object)listView).Name = "listView";
		listView.ShowGroups = false;
		listView.SingleCheckedItem = false;
		listView.View = View.Details;
		listView.VirtualMode = false;
		listView.SelectedItemChanged += listView_SelectedItemChanged;
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		okButton.UseVisualStyleBackColor = true;
		okButton.Click += okButton_Click;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = button;
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(button);
		((Control)this).Controls.Add((Control)(object)listView);
		((Control)this).Controls.Add(introLabel);
		((Control)this).Name = "SelectItemDialog";
		((Control)this).ResumeLayout(performLayout: false);
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		((Form)this).DialogResult = DialogResult.OK;
		((Form)this).Close();
	}
}
