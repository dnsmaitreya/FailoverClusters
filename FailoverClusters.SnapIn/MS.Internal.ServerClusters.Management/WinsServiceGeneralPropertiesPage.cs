using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class WinsServiceGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string databasePath;

	private string backupPath;

	private bool propertiesDirty;

	private IContainer components;

	private Label databasePathLabel;

	private TextBox databasePathTextBox;

	private TextBox backupPathTextBox;

	private Label backupPathLabel;

	internal WinsServiceGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		databasePath = (string)privateProperties["DatabasePath"].Value;
		backupPath = (string)privateProperties["BackupPath"].Value;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		databasePathTextBox.Text = databasePath;
		backupPathTextBox.Text = backupPath;
		propertiesDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (!base.ValidateProperties())
		{
			return false;
		}
		if (propertiesDirty)
		{
			databasePath = InputValidator.ValidateNonemptyString(databasePathTextBox.Text, Resources.DatabasePath_Text);
			backupPath = InputValidator.ValidateNonemptyString(backupPathTextBox.Text, Resources.BackupPath_Text);
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			base.SaveProperties(waitDialog);
			if (propertiesDirty)
			{
				PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				privateProperties["DatabasePath"].Value = databasePath;
				privateProperties["BackupPath"].Value = backupPath;
				SaveProperties(privateProperties);
				propertiesDirty = false;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving WINS Service properties");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.WinsSavedFailed_Text,
				base.Context.DisplayName
			});
		}
	}

	private void PropertiesChanged(object sender, EventArgs e)
	{
		propertiesDirty = true;
		base.IsDirty = true;
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(WinsServiceGeneralPropertiesPage));
		databasePathLabel = new Label();
		databasePathTextBox = new TextBox();
		backupPathTextBox = new TextBox();
		backupPathLabel = new Label();
		((Control)(object)this).SuspendLayout();
		databasePathLabel.AutoEllipsis = true;
		databasePathLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(databasePathLabel, "databasePathLabel");
		databasePathLabel.Name = "databasePathLabel";
		componentResourceManager.ApplyResources(databasePathTextBox, "databasePathTextBox");
		databasePathTextBox.BackColor = SystemColors.Window;
		databasePathTextBox.Name = "databasePathTextBox";
		databasePathTextBox.TextChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(backupPathTextBox, "backupPathTextBox");
		backupPathTextBox.BackColor = SystemColors.Window;
		backupPathTextBox.Name = "backupPathTextBox";
		backupPathTextBox.TextChanged += PropertiesChanged;
		backupPathLabel.AutoEllipsis = true;
		backupPathLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(backupPathLabel, "backupPathLabel");
		backupPathLabel.Name = "backupPathLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(databasePathLabel);
		((Control)(object)this).Controls.Add(databasePathTextBox);
		((Control)(object)this).Controls.Add(backupPathLabel);
		((Control)(object)this).Controls.Add(backupPathTextBox);
		((Control)(object)this).Name = "WinsServiceGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(backupPathTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(backupPathLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(databasePathTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(databasePathLabel, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
