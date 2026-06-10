using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class DhcpServiceGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string databasePath;

	private string logFilePath;

	private string backupPath;

	private bool propertiesDirty;

	private IContainer components;

	private Label databaseLabel;

	private TextBox databaseTextBox;

	private TextBox auditFileTextBox;

	private Label auditFileLabel;

	private TextBox backupPathTextBox;

	private Label backupPathLabel;

	internal DhcpServiceGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		databasePath = (string)privateProperties["DatabasePath"].Value;
		logFilePath = (string)privateProperties["LogFilePath"].Value;
		backupPath = (string)privateProperties["BackupPath"].Value;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		databaseTextBox.Text = databasePath;
		auditFileTextBox.Text = logFilePath;
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
			databasePath = InputValidator.ValidateNonemptyString(databaseTextBox.Text, Resources.DatabasePath_Text);
			logFilePath = InputValidator.ValidateNonemptyString(auditFileTextBox.Text, Resources.AuditFilePath_Text);
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
				privateProperties["LogFilePath"].Value = logFilePath;
				privateProperties["BackupPath"].Value = backupPath;
				SaveProperties(privateProperties);
				propertiesDirty = false;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving DHCP properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.DhcpServerSavedFailed_Text,
				base.Context.DisplayName
			});
		}
	}

	private void PropertiesChanged(object sender, EventArgs e)
	{
		base.IsDirty = (propertiesDirty = true);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DhcpServiceGeneralPropertiesPage));
		databaseLabel = new Label();
		databaseTextBox = new TextBox();
		auditFileTextBox = new TextBox();
		auditFileLabel = new Label();
		backupPathTextBox = new TextBox();
		backupPathLabel = new Label();
		((Control)(object)this).SuspendLayout();
		databaseLabel.AutoEllipsis = true;
		databaseLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(databaseLabel, "databaseLabel");
		databaseLabel.Name = "databaseLabel";
		componentResourceManager.ApplyResources(databaseTextBox, "databaseTextBox");
		databaseTextBox.BackColor = SystemColors.Window;
		databaseTextBox.Name = "databaseTextBox";
		databaseTextBox.TextChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(auditFileTextBox, "auditFileTextBox");
		auditFileTextBox.BackColor = SystemColors.Window;
		auditFileTextBox.Name = "auditFileTextBox";
		auditFileTextBox.TextChanged += PropertiesChanged;
		auditFileLabel.AutoEllipsis = true;
		auditFileLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(auditFileLabel, "auditFileLabel");
		auditFileLabel.Name = "auditFileLabel";
		componentResourceManager.ApplyResources(backupPathTextBox, "backupPathTextBox");
		backupPathTextBox.BackColor = SystemColors.Window;
		backupPathTextBox.Name = "backupPathTextBox";
		backupPathTextBox.TextChanged += PropertiesChanged;
		backupPathLabel.AutoEllipsis = true;
		backupPathLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(backupPathLabel, "backupPathLabel");
		backupPathLabel.Name = "backupPathLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(backupPathTextBox);
		((Control)(object)this).Controls.Add(backupPathLabel);
		((Control)(object)this).Controls.Add(auditFileTextBox);
		((Control)(object)this).Controls.Add(auditFileLabel);
		((Control)(object)this).Controls.Add(databaseTextBox);
		((Control)(object)this).Controls.Add(databaseLabel);
		((Control)(object)this).Name = "DhcpServiceGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(databaseLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(databaseTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(auditFileLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(auditFileTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(backupPathLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(backupPathTextBox, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
