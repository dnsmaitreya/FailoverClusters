using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class RegistryReplicationPropertiesPage : PropertyPageControlBase
{
	private StringCollection registryCheckpointStrings;

	private RegistryCheckpoints registryCheckpoints;

	private StringBuilder saveErrors;

	private readonly ResourceContext context;

	private IContainer components;

	private Label headerLabel;

	private Button addButton;

	private Button removeButton;

	private BaseListView registryKeysListView;

	private Button editButton;

	private ColumnHeader key;

	internal RegistryReplicationPropertiesPage(ResourceContext context)
		: base(Resources.RegistryReplication_Text)
	{
		this.context = context;
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		registryCheckpointStrings = context.Resource.GetRegistryCheckpoints();
	}

	protected override void InitializePage()
	{
		((ListView)(object)registryKeysListView).BeginUpdate();
		registryKeysListView.Items.AddRange((from string checkpoint in registryCheckpointStrings
			select new ListViewItem(checkpoint)).ToArray());
		((ListView)(object)registryKeysListView).EndUpdate();
		editButton.Enabled = false;
		removeButton.Enabled = false;
	}

	protected override bool ValidateProperties()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		registryCheckpoints = new RegistryCheckpoints();
		foreach (ListViewItem item in registryKeysListView.Items)
		{
			registryCheckpoints.AddRegistryKey(item.Text);
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			saveErrors = new StringBuilder();
			registryCheckpoints.ApplyCheckpointsProgress += OnApplyCheckpointsProgress;
			try
			{
				registryCheckpoints.ApplyCheckpoints(context.Resource);
			}
			finally
			{
				registryCheckpoints.ApplyCheckpointsProgress -= OnApplyCheckpointsProgress;
			}
			if (saveErrors.Length > 0)
			{
				throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { saveErrors.ToString() });
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving registry replication");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[1] { Resources.RegistryReplicationSavedFailed_Text });
		}
	}

	protected override void CompleteSaveProperties()
	{
	}

	private void OnApplyCheckpointsProgress(object sender, MessageProgressChangedEventArgs e)
	{
		string value = null;
		if (e.MessageLevel == OperationProgressWarningLevel.Warning)
		{
			value = string.Format(CultureInfo.CurrentCulture, Resources.WarningMessage_Text, e.Message);
		}
		else if (e.MessageLevel == OperationProgressWarningLevel.Error)
		{
			value = string.Format(CultureInfo.CurrentCulture, Resources.ErrorMessage_Text, e.Message);
		}
		if (saveErrors.Length > 0)
		{
			saveErrors.Append(Environment.NewLine).Append(Environment.NewLine);
		}
		saveErrors.Append(value);
	}

	private void AddButtonClick(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		RegistryKeyDialog dialog = new RegistryKeyDialog();
		if (base.NotifyUser.ShowDialog((Form)(object)dialog) == DialogResult.OK)
		{
			if (((IEnumerable)registryKeysListView.Items).Cast<ListViewItem>().Any((ListViewItem item) => string.Compare(item.Text, dialog.RegistryKey, StringComparison.OrdinalIgnoreCase) == 0))
			{
				base.NotifyUser.ShowError(Resources.DuplicateRegistryKeyFormatString_Text, new object[1] { dialog.RegistryKey });
			}
			else
			{
				registryKeysListView.Items.Add(new ListViewItem(dialog.RegistryKey));
				base.IsDirty = true;
			}
		}
	}

	private void EditButtonClick(object sender, EventArgs e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		ListViewItem listViewItem = ((ListView)(object)registryKeysListView).SelectedItems[0];
		RegistryKeyDialog val = new RegistryKeyDialog(listViewItem.Text);
		if (base.NotifyUser.ShowDialog((Form)(object)val) == DialogResult.OK)
		{
			listViewItem.Text = val.RegistryKey;
			base.IsDirty = true;
		}
	}

	private void RemoveButtonClick(object sender, EventArgs e)
	{
		((ListView)(object)registryKeysListView).SelectedItems[0].Remove();
		base.IsDirty = true;
	}

	private void SelectedRegistryKeyChanged(object sender, EventArgs e)
	{
		Button button = editButton;
		bool enabled = (removeButton.Enabled = ((ListView)(object)registryKeysListView).SelectedIndices.Count > 0);
		button.Enabled = enabled;
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(RegistryReplicationPropertiesPage));
		headerLabel = new Label();
		addButton = new Button();
		removeButton = new Button();
		registryKeysListView = new BaseListView();
		key = new ColumnHeader();
		editButton = new Button();
		((Control)(object)this).SuspendLayout();
		headerLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(headerLabel, "headerLabel");
		headerLabel.Name = "headerLabel";
		componentResourceManager.ApplyResources(addButton, "addButton");
		addButton.ForeColor = SystemColors.ControlText;
		addButton.Name = "addButton";
		addButton.UseVisualStyleBackColor = true;
		addButton.Click += AddButtonClick;
		componentResourceManager.ApplyResources(removeButton, "removeButton");
		removeButton.ForeColor = SystemColors.ControlText;
		removeButton.Name = "removeButton";
		removeButton.UseVisualStyleBackColor = true;
		removeButton.Click += RemoveButtonClick;
		componentResourceManager.ApplyResources(registryKeysListView, "registryKeysListView");
		((ListView)(object)registryKeysListView).Columns.AddRange(new ColumnHeader[1] { key });
		registryKeysListView.EnableAutoResizeColumns = true;
		((ListView)(object)registryKeysListView).FullRowSelect = true;
		((ListView)(object)registryKeysListView).MultiSelect = false;
		((Control)(object)registryKeysListView).Name = "registryKeysListView";
		((ListView)(object)registryKeysListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)registryKeysListView).View = View.Details;
		((ListView)(object)registryKeysListView).SelectedIndexChanged += SelectedRegistryKeyChanged;
		componentResourceManager.ApplyResources(key, "key");
		componentResourceManager.ApplyResources(editButton, "editButton");
		editButton.ForeColor = SystemColors.ControlText;
		editButton.Name = "editButton";
		editButton.UseVisualStyleBackColor = true;
		editButton.Click += EditButtonClick;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(editButton);
		((Control)(object)this).Controls.Add((Control)(object)registryKeysListView);
		((Control)(object)this).Controls.Add(removeButton);
		((Control)(object)this).Controls.Add(addButton);
		((Control)(object)this).Controls.Add(headerLabel);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "RegistryReplicationPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(headerLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(addButton, 0);
		((Control)(object)this).Controls.SetChildIndex(removeButton, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)registryKeysListView, 0);
		((Control)(object)this).Controls.SetChildIndex(editButton, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
