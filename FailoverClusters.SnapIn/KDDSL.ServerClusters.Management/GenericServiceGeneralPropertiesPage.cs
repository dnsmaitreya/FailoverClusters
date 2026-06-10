using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class GenericServiceGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string serviceName;

	private string startupParameters;

	private bool useNetworkName;

	private bool propertiesDirty;

	private IContainer components;

	private Label serviceNameLabel;

	private TextBox serviceNameTextBox;

	private TextBox startupParametersTextBox;

	private Label startupParametersLabel;

	private CheckBox useNetworkNameCheckBox;

	internal GenericServiceGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		serviceName = (string)privateProperties["ServiceName"].Value;
		startupParameters = (string)privateProperties["StartupParameters"].Value;
		useNetworkName = (uint)privateProperties["UseNetworkName"].Value != 0;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		serviceNameTextBox.Text = serviceName;
		startupParametersTextBox.Text = startupParameters;
		useNetworkNameCheckBox.Checked = useNetworkName;
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
			serviceName = InputValidator.ValidateNonemptyString(serviceNameTextBox.Text, Resources.ServiceName_Text);
			useNetworkName = useNetworkNameCheckBox.Checked;
			startupParameters = startupParametersTextBox.Text;
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
				privateProperties["ServiceName"].Value = serviceName;
				privateProperties["UseNetworkName"].Value = (useNetworkName ? 1u : 0u);
				privateProperties["StartupParameters"].Value = startupParameters;
				SaveProperties(privateProperties);
				propertiesDirty = false;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving generic service properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.GenericServiceSavedFailed_Text,
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(GenericServiceGeneralPropertiesPage));
		serviceNameLabel = new Label();
		serviceNameTextBox = new TextBox();
		startupParametersTextBox = new TextBox();
		startupParametersLabel = new Label();
		useNetworkNameCheckBox = new CheckBox();
		((Control)(object)this).SuspendLayout();
		serviceNameLabel.AutoEllipsis = true;
		serviceNameLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(serviceNameLabel, "serviceNameLabel");
		serviceNameLabel.Name = "serviceNameLabel";
		serviceNameTextBox.BackColor = SystemColors.Window;
		componentResourceManager.ApplyResources(serviceNameTextBox, "serviceNameTextBox");
		serviceNameTextBox.Name = "serviceNameTextBox";
		serviceNameTextBox.TextChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(startupParametersTextBox, "startupParametersTextBox");
		startupParametersTextBox.BackColor = SystemColors.Window;
		startupParametersTextBox.Name = "startupParametersTextBox";
		startupParametersTextBox.TextChanged += PropertiesChanged;
		startupParametersLabel.AutoEllipsis = true;
		startupParametersLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(startupParametersLabel, "startupParametersLabel");
		startupParametersLabel.Name = "startupParametersLabel";
		componentResourceManager.ApplyResources(useNetworkNameCheckBox, "useNetworkNameCheckBox");
		useNetworkNameCheckBox.ForeColor = SystemColors.ControlText;
		useNetworkNameCheckBox.Name = "useNetworkNameCheckBox";
		useNetworkNameCheckBox.CheckedChanged += PropertiesChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(useNetworkNameCheckBox);
		((Control)(object)this).Controls.Add(startupParametersTextBox);
		((Control)(object)this).Controls.Add(startupParametersLabel);
		((Control)(object)this).Controls.Add(serviceNameTextBox);
		((Control)(object)this).Controls.Add(serviceNameLabel);
		((Control)(object)this).Name = "GenericServiceGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(serviceNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(serviceNameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(startupParametersLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(startupParametersTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(useNetworkNameCheckBox, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
