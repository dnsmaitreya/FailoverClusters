using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class GenericApplicationGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string commandLine;

	private string parameters;

	private const string CommandlineSeparator = ".exe ";

	private string currentDirectory;

	private bool useNetworkName;

	private bool propertiesDirty;

	private bool commandLineDirty;

	private IContainer components;

	private TextBox commandLineTextBox;

	private Label commandLineLabel;

	private TextBox currentDirectoryTextBox;

	private Label currentDirectoryLabel;

	private CheckBox useNetworkNameCheckBox;

	private Label parametersLabel;

	private TextBox parametersTextBox;

	internal GenericApplicationGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string text = (string)privateProperties["CommandLine"].Value;
		int num = text.IndexOf(".exe ", StringComparison.OrdinalIgnoreCase);
		parameters = string.Empty;
		if (num != -1)
		{
			num += ".exe ".Length - 1;
			commandLine = text.Substring(0, num);
			if (num + 1 < text.Length)
			{
				parameters = text.Substring(num + 1).Trim();
			}
		}
		else
		{
			commandLine = text;
		}
		currentDirectory = (string)privateProperties["CurrentDirectory"].Value;
		useNetworkName = (uint)privateProperties["UseNetworkName"].Value != 0;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		commandLineTextBox.Text = commandLine;
		parametersTextBox.Text = parameters;
		currentDirectoryTextBox.Text = currentDirectory;
		useNetworkNameCheckBox.Checked = useNetworkName;
		propertiesDirty = false;
		commandLineDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (!base.ValidateProperties())
		{
			return false;
		}
		if (commandLineDirty)
		{
			commandLine = InputValidator.ValidateNonemptyString(commandLineTextBox.Text, Resources.CommandLine_Text);
			parameters = parametersTextBox.Text.Trim();
		}
		if (propertiesDirty)
		{
			currentDirectory = InputValidator.ValidateNonemptyString(currentDirectoryTextBox.Text, Resources.CurrentDirectory_Text);
			useNetworkName = useNetworkNameCheckBox.Checked;
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			base.SaveProperties(waitDialog);
			PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
			if (commandLineDirty)
			{
				InputValidator.ValidateGenericAppCommandLine(base.Context.Resource.Cluster, commandLine);
				privateProperties["CommandLine"].Value = (string.IsNullOrEmpty(parameters) ? commandLine : string.Format(CultureInfo.CurrentCulture, "{0} {1}", commandLine, parameters));
			}
			if (propertiesDirty)
			{
				InputValidator.ValidateGenericAppCurrentDirectory(base.Context.Resource.Cluster, currentDirectory);
				privateProperties["CurrentDirectory"].Value = currentDirectory;
				privateProperties["UseNetworkName"].Value = (useNetworkName ? 1u : 0u);
			}
			SaveProperties(privateProperties);
			propertiesDirty = false;
			commandLineDirty = false;
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving generic application properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.GenericApplicationSavedFailed_Text,
				base.Context.DisplayName
			});
		}
	}

	private void PropertiesChanged(object sender, EventArgs e)
	{
		base.IsDirty = (propertiesDirty = true);
	}

	private void CommandLineChanged(object sender, EventArgs e)
	{
		commandLineDirty = true;
		PropertiesChanged(sender, e);
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(GenericApplicationGeneralPropertiesPage));
		commandLineTextBox = new TextBox();
		commandLineLabel = new Label();
		currentDirectoryTextBox = new TextBox();
		currentDirectoryLabel = new Label();
		useNetworkNameCheckBox = new CheckBox();
		parametersLabel = new Label();
		parametersTextBox = new TextBox();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(commandLineTextBox, "commandLineTextBox");
		commandLineTextBox.BackColor = SystemColors.Window;
		commandLineTextBox.Name = "commandLineTextBox";
		commandLineTextBox.TextChanged += CommandLineChanged;
		commandLineLabel.AutoEllipsis = true;
		commandLineLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(commandLineLabel, "commandLineLabel");
		commandLineLabel.Name = "commandLineLabel";
		componentResourceManager.ApplyResources(currentDirectoryTextBox, "currentDirectoryTextBox");
		currentDirectoryTextBox.BackColor = SystemColors.Window;
		currentDirectoryTextBox.Name = "currentDirectoryTextBox";
		currentDirectoryTextBox.TextChanged += PropertiesChanged;
		currentDirectoryLabel.AutoEllipsis = true;
		currentDirectoryLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(currentDirectoryLabel, "currentDirectoryLabel");
		currentDirectoryLabel.Name = "currentDirectoryLabel";
		componentResourceManager.ApplyResources(useNetworkNameCheckBox, "useNetworkNameCheckBox");
		useNetworkNameCheckBox.ForeColor = SystemColors.ControlText;
		useNetworkNameCheckBox.Name = "useNetworkNameCheckBox";
		useNetworkNameCheckBox.CheckedChanged += PropertiesChanged;
		parametersLabel.AutoEllipsis = true;
		parametersLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(parametersLabel, "parametersLabel");
		parametersLabel.Name = "parametersLabel";
		componentResourceManager.ApplyResources(parametersTextBox, "parametersTextBox");
		parametersTextBox.BackColor = SystemColors.Window;
		parametersTextBox.Name = "parametersTextBox";
		parametersTextBox.TextChanged += CommandLineChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(parametersLabel);
		((Control)(object)this).Controls.Add(parametersTextBox);
		((Control)(object)this).Controls.Add(commandLineLabel);
		((Control)(object)this).Controls.Add(commandLineTextBox);
		((Control)(object)this).Controls.Add(currentDirectoryLabel);
		((Control)(object)this).Controls.Add(currentDirectoryTextBox);
		((Control)(object)this).Controls.Add(useNetworkNameCheckBox);
		((Control)(object)this).Name = "GenericApplicationGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(useNetworkNameCheckBox, 0);
		((Control)(object)this).Controls.SetChildIndex(currentDirectoryTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(currentDirectoryLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(commandLineTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(commandLineLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(parametersTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(parametersLabel, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
