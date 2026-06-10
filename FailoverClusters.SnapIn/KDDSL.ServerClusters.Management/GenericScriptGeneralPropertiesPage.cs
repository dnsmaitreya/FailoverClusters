using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class GenericScriptGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string scriptFilepath;

	private bool propertiesDirty;

	private IContainer components;

	private Label scriptLabel;

	private TextBox scriptTextBox;

	internal GenericScriptGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		scriptFilepath = (string)privateProperties["ScriptFilepath"].Value;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		scriptTextBox.Text = scriptFilepath;
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
			scriptFilepath = InputValidator.ValidateNonemptyString(scriptTextBox.Text, Resources.ScriptFilePath_Text);
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
				InputValidator.ValidateGenericScriptPath(base.Context.Resource.Cluster, scriptFilepath);
				PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				privateProperties["ScriptFilepath"].Value = scriptFilepath;
				SaveProperties(privateProperties);
				propertiesDirty = false;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving generic script properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.GenericScriptSavedFailed_Text,
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(GenericScriptGeneralPropertiesPage));
		scriptLabel = new Label();
		scriptTextBox = new TextBox();
		((Control)(object)this).SuspendLayout();
		scriptLabel.AutoEllipsis = true;
		scriptLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(scriptLabel, "scriptLabel");
		scriptLabel.Name = "scriptLabel";
		componentResourceManager.ApplyResources(scriptTextBox, "scriptTextBox");
		scriptTextBox.BackColor = SystemColors.Window;
		scriptTextBox.Name = "scriptTextBox";
		scriptTextBox.TextChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(scriptLabel);
		((Control)(object)this).Controls.Add(scriptTextBox);
		((Control)(object)this).Name = "GenericScriptGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(scriptTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(scriptLabel, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
