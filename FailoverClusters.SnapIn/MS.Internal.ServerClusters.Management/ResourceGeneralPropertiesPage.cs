using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using FailoverClusters.UIFramework;
using MS.Internal.ServerClusters.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class ResourceGeneralPropertiesPage : ResourcePropertiesPage
{
	private const int TextBoxBorderOffset = 3;

	private bool nameDirty;

	private Icon icon;

	private string name;

	private string state;

	private string status;

	private string resourceType;

	private IContainer components;

	private Label nameLabel;

	private TextBox nameTextBox;

	private Label resourceTypeLabel;

	private Label resourceTypeValueLabel;

	private PictureBox resourceIconPictureBox;

	private Label stateValueLabel;

	private Label stateLabel;

	private HorizontalLine horizontalLine;

	protected bool IsResourceNameReadOnly
	{
		get
		{
			return nameTextBox.ReadOnly;
		}
		set
		{
			if (value != nameTextBox.ReadOnly)
			{
				nameTextBox.ReadOnly = value;
				if (value)
				{
					nameTextBox.BorderStyle = BorderStyle.None;
					nameTextBox.BackColor = nameTextBox.Parent.BackColor;
					nameTextBox.Top += 3;
				}
				else
				{
					nameTextBox.BorderStyle = BorderStyle.Fixed3D;
					nameTextBox.BackColor = SystemColors.Window;
					nameTextBox.Top -= 3;
				}
			}
		}
	}

	protected string Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
		}
	}

	public ResourceGeneralPropertiesPage()
	{
		InitializeComponent();
	}

	internal ResourceGeneralPropertiesPage(ResourceContext context, bool renamable)
		: base(context, Resources.General_Text)
	{
		InitializeComponent();
		nameDirty = false;
		if (renamable)
		{
			nameTextBox.TextChanged += ResourceNameChanged;
		}
		else
		{
			IsResourceNameReadOnly = true;
		}
	}

	protected void SetResourceName(string resourceName)
	{
		nameTextBox.Text = resourceName;
	}

	protected override void LoadProperties()
	{
		ClusterResource resource = base.Context.Resource;
		int num = IconsHelp.GetResourceTypeIconIndex(resource);
		if (num == Icons.ResourceIndex)
		{
			num = IconsHelp.GetResourceIconIndex(resource, ResourceState.Online);
		}
		icon = Icons.GetIcon(num);
		name = resource.Name;
		state = FormatHelp.GetResourceStateShortString(resource);
		status = (string)resource.GetCommonProperties(PropertyCollectionSet.ReadWrite)["ResourceSpecificStatus"].Value;
		ClusterResourceType clusterResourceType = resource.Cluster.GetResourceType(resource.ResourceTypeName);
		resourceType = clusterResourceType.DisplayName;
	}

	protected override void InitializePage()
	{
		WinFormsHelp.SetPictureBoxImage(resourceIconPictureBox, icon);
		nameTextBox.Text = name;
		if (string.IsNullOrWhiteSpace(status))
		{
			stateValueLabel.Text = Extensions.FormatCurrentCulture(Resources.GeneralSummary_StatusOnlyField, (object)state);
		}
		else
		{
			stateValueLabel.Text = Extensions.FormatCurrentCulture(Resources.GeneralSummary_StatusAndSubStatus, new object[2] { state, status });
		}
		resourceTypeValueLabel.Text = resourceType;
		nameDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (nameDirty)
		{
			name = InputValidator.ValidateNonemptyString(nameTextBox.Text, Resources.Name_Text);
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			if (nameDirty)
			{
				base.Context.Resource.Rename(name, "ResourceGeneralPropertiesPage.SaveProperties");
				nameDirty = false;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving resource properties");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.ResourceSavedFailed_Text,
				base.Context.DisplayName
			});
		}
	}

	private void ResourceNameChanged(object sender, EventArgs e)
	{
		nameDirty = true;
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ResourceGeneralPropertiesPage));
		nameLabel = new Label();
		nameTextBox = new TextBox();
		resourceTypeLabel = new Label();
		resourceTypeValueLabel = new Label();
		resourceIconPictureBox = new PictureBox();
		stateValueLabel = new Label();
		stateLabel = new Label();
		horizontalLine = new HorizontalLine();
		((ISupportInitialize)resourceIconPictureBox).BeginInit();
		((Control)(object)this).SuspendLayout();
		nameLabel.AutoEllipsis = true;
		nameLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(nameLabel, "nameLabel");
		nameLabel.Name = "nameLabel";
		componentResourceManager.ApplyResources(nameTextBox, "nameTextBox");
		nameTextBox.BackColor = SystemColors.Window;
		nameTextBox.Name = "nameTextBox";
		resourceTypeLabel.AutoEllipsis = true;
		resourceTypeLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(resourceTypeLabel, "resourceTypeLabel");
		resourceTypeLabel.Name = "resourceTypeLabel";
		componentResourceManager.ApplyResources(resourceTypeValueLabel, "resourceTypeValueLabel");
		resourceTypeValueLabel.AutoEllipsis = true;
		resourceTypeValueLabel.ForeColor = SystemColors.ControlText;
		resourceTypeValueLabel.Name = "resourceTypeValueLabel";
		componentResourceManager.ApplyResources(resourceIconPictureBox, "resourceIconPictureBox");
		resourceIconPictureBox.Name = "resourceIconPictureBox";
		resourceIconPictureBox.TabStop = false;
		componentResourceManager.ApplyResources(stateValueLabel, "stateValueLabel");
		stateValueLabel.AutoEllipsis = true;
		stateValueLabel.ForeColor = SystemColors.ControlText;
		stateValueLabel.Name = "stateValueLabel";
		stateLabel.AutoEllipsis = true;
		stateLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(stateLabel, "stateLabel");
		stateLabel.Name = "stateLabel";
		componentResourceManager.ApplyResources(horizontalLine, "horizontalLine");
		((Control)(object)horizontalLine).Name = "horizontalLine";
		((Control)(object)horizontalLine).TabStop = false;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine);
		((Control)(object)this).Controls.Add(stateValueLabel);
		((Control)(object)this).Controls.Add(stateLabel);
		((Control)(object)this).Controls.Add(resourceIconPictureBox);
		((Control)(object)this).Controls.Add(resourceTypeValueLabel);
		((Control)(object)this).Controls.Add(resourceTypeLabel);
		((Control)(object)this).Controls.Add(nameTextBox);
		((Control)(object)this).Controls.Add(nameLabel);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "ResourceGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(nameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(nameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(resourceTypeLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(resourceTypeValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(resourceIconPictureBox, 0);
		((Control)(object)this).Controls.SetChildIndex(stateLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(stateValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine, 0);
		((ISupportInitialize)resourceIconPictureBox).EndInit();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

