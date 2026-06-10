using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class DfsrGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	[Flags]
	private enum DfsrFolderFlags
	{
		None = 0,
		ReadOnly = 1
	}

	private string folderName;

	private string folderPath;

	private string groupName;

	private string stagingPath;

	private DfsrFolderFlags folderFlags;

	private IContainer components;

	private Label folderNameLabel;

	private Label folderNameValueLabel;

	private Label folderPathLabel;

	private Label folderPathValueLabel;

	private Label groupNameValueLabel;

	private Label groupNameLabel;

	private Label stagingPathValueLabel;

	private Label stagingPathLabel;

	private Label replicatedFolderTypeValueLabel;

	private Label replicatedFolderTypeLabel;

	internal DfsrGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: false)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.Both);
		folderName = ConvertAndNotNull(privateProperties["Replicated Folder Name"].Value);
		folderPath = ConvertAndNotNull(privateProperties["Replicated Folder Root Path"].Value);
		groupName = ConvertAndNotNull(privateProperties["Replication Group Name"].Value);
		stagingPath = ConvertAndNotNull(privateProperties["Staging path"].Value);
		folderFlags = ConvertAndNotNull((uint)privateProperties["Replicated Folder Flags"].Value);
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		folderNameValueLabel.Text = folderName;
		folderPathValueLabel.Text = folderPath;
		groupNameValueLabel.Text = groupName;
		stagingPathValueLabel.Text = stagingPath;
		replicatedFolderTypeValueLabel.Text = (((folderFlags & DfsrFolderFlags.ReadOnly) == DfsrFolderFlags.ReadOnly) ? Resources.DfsrFlagReadOnly : Resources.DfsrFlagReadWrite);
	}

	private string ConvertAndNotNull(object value)
	{
		if (value == null)
		{
			return string.Empty;
		}
		return (string)value;
	}

	private DfsrFolderFlags ConvertAndNotNull(uint value)
	{
		return (DfsrFolderFlags)value;
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DfsrGeneralPropertiesPage));
		folderNameLabel = new Label();
		folderNameValueLabel = new Label();
		folderPathLabel = new Label();
		folderPathValueLabel = new Label();
		groupNameValueLabel = new Label();
		groupNameLabel = new Label();
		stagingPathValueLabel = new Label();
		stagingPathLabel = new Label();
		replicatedFolderTypeValueLabel = new Label();
		replicatedFolderTypeLabel = new Label();
		((Control)(object)this).SuspendLayout();
		folderNameLabel.AutoEllipsis = true;
		folderNameLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(folderNameLabel, "folderNameLabel");
		folderNameLabel.Name = "folderNameLabel";
		folderNameValueLabel.AutoEllipsis = true;
		folderNameValueLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(folderNameValueLabel, "folderNameValueLabel");
		folderNameValueLabel.Name = "folderNameValueLabel";
		folderPathLabel.AutoEllipsis = true;
		folderPathLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(folderPathLabel, "folderPathLabel");
		folderPathLabel.Name = "folderPathLabel";
		folderPathValueLabel.AutoEllipsis = true;
		folderPathValueLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(folderPathValueLabel, "folderPathValueLabel");
		folderPathValueLabel.Name = "folderPathValueLabel";
		groupNameValueLabel.AutoEllipsis = true;
		groupNameValueLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(groupNameValueLabel, "groupNameValueLabel");
		groupNameValueLabel.Name = "groupNameValueLabel";
		groupNameLabel.AutoEllipsis = true;
		groupNameLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(groupNameLabel, "groupNameLabel");
		groupNameLabel.Name = "groupNameLabel";
		stagingPathValueLabel.AutoEllipsis = true;
		stagingPathValueLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(stagingPathValueLabel, "stagingPathValueLabel");
		stagingPathValueLabel.Name = "stagingPathValueLabel";
		stagingPathLabel.AutoEllipsis = true;
		stagingPathLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(stagingPathLabel, "stagingPathLabel");
		stagingPathLabel.Name = "stagingPathLabel";
		replicatedFolderTypeValueLabel.AutoEllipsis = true;
		replicatedFolderTypeValueLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(replicatedFolderTypeValueLabel, "replicatedFolderTypeValueLabel");
		replicatedFolderTypeValueLabel.Name = "replicatedFolderTypeValueLabel";
		replicatedFolderTypeLabel.AutoEllipsis = true;
		replicatedFolderTypeLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(replicatedFolderTypeLabel, "replicatedFolderTypeLabel");
		replicatedFolderTypeLabel.Name = "replicatedFolderTypeLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(replicatedFolderTypeValueLabel);
		((Control)(object)this).Controls.Add(replicatedFolderTypeLabel);
		((Control)(object)this).Controls.Add(stagingPathValueLabel);
		((Control)(object)this).Controls.Add(stagingPathLabel);
		((Control)(object)this).Controls.Add(groupNameValueLabel);
		((Control)(object)this).Controls.Add(groupNameLabel);
		((Control)(object)this).Controls.Add(folderPathValueLabel);
		((Control)(object)this).Controls.Add(folderPathLabel);
		((Control)(object)this).Controls.Add(folderNameValueLabel);
		((Control)(object)this).Controls.Add(folderNameLabel);
		((Control)(object)this).Name = "DfsrGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(folderNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(folderNameValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(folderPathLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(folderPathValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(groupNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(groupNameValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(stagingPathLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(stagingPathValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(replicatedFolderTypeLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(replicatedFolderTypeValueLabel, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
