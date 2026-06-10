using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.SnapIn;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using FailoverClusters.UIFramework;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Controls;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.WinForms;

[DesignTimeVisible(true)]
internal class ResourceGeneralPropertyPage : ResourcePropertyPage
{
	private const int TextBoxBorderOffset = 3;

	private bool nameDirty;

	private Icon icon;

	private string name;

	private string state;

	private string status;

	private string resourceType;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private readonly Guid resourceId;

	private Resource resource;

	private bool renamable;

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

	internal ResourceGeneralPropertyPage()
	{
		InitializeComponent();
		resource = null;
	}

	public ResourceGeneralPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid resourceId)
		: this(cluster, resourceId, renamable: true)
	{
	}

	public ResourceGeneralPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid resourceId, bool renamable)
		: base(Resources.General_Text)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		if (resourceId == Guid.Empty)
		{
			throw new ArgumentException(ExceptionResources.InvalidArgument_Text, "resourceId");
		}
		this.cluster = cluster;
		this.resourceId = resourceId;
		this.renamable = renamable;
		InitializeComponent();
		nameDirty = false;
		resource = null;
	}

	protected void SetResourceName(string resourceName)
	{
		nameTextBox.Text = resourceName;
	}

	protected override object LoadProperties(object context)
	{
		Resource.Get(cluster, resourceId, delegate(OperationResult<Resource> cacheResult)
		{
			if (cacheResult.Error != null)
			{
				throw cacheResult.Error;
			}
			resource = cacheResult.Result;
			resource.LoadAsync(delegate(ClusterLoadedEventArgs result)
			{
				if (result.Error != null)
				{
					throw result.Error;
				}
				SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
				{
					int num = IconHelpers.GetResourceTypeIconIndex(resource);
					if (num == Icons.ResourceIndex)
					{
						num = IconHelpers.GetResourceIconIndex(resource, FailoverClusters.Framework.ResourceState.Online);
					}
					icon = Icons.GetIcon(num);
					name = resource.Name;
					state = resource.ResourceState.Translate();
					status = resource.Status;
					resourceType = resource.ResourceType.DisplayName;
					if (renamable)
					{
						IsResourceNameReadOnly = false;
						nameTextBox.TextChanged += ResourceNameChanged;
					}
					LoadControls();
				});
			}, ResourceLoadSelection.Basic);
		}, OperationType.Async);
		return null;
	}

	protected override void InitializePage()
	{
		int resourceIconIndex = IconHelpers.GetResourceIconIndex(null, FailoverClusters.Framework.ResourceState.Unknown);
		icon = Icons.GetIcon(resourceIconIndex);
		name = CommonResources.LoadingText;
		state = CommonResources.LoadingText;
		resourceType = CommonResources.LoadingText;
		LoadControls();
	}

	private void LoadControls()
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
		base.IsDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (nameDirty)
		{
			name = InputValidator.ValidateNonemptyString(nameTextBox.Text, Resources.Name_Text);
		}
		return true;
	}

	protected override bool SaveProperties()
	{
		bool success = true;
		SettingChanger renameWaiter = new SettingChanger(initialState: true);
		try
		{
			if (nameDirty)
			{
				nameDirty = false;
				success = false;
				renameWaiter.Reset();
				resource.RedirectAsyncOutput(delegate
				{
					resource.Name = name;
				}, delegate(OperationResult result)
				{
					//IL_008e: Unknown result type (might be due to invalid IL or missing references)
					success = result.Error == null;
					renameWaiter.Set();
					if (!success)
					{
						nameDirty = true;
						Exception ex = result.Error;
						if (result.Error is ClusterDefaultException)
						{
							ex = new ClusterObjectRenameException(resource.Name, result.Error);
						}
						ClusterLog.LogException(ex, "Error saving resource properties");
						ClusterDialogException.ShowTaskDialog(ex, base.HWND);
					}
				});
			}
		}
		finally
		{
			if (renameWaiter != null)
			{
				((IDisposable)renameWaiter).Dispose();
			}
		}
		return success;
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
			nameTextBox.TextChanged -= ResourceNameChanged;
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ResourceGeneralPropertyPage));
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
		nameTextBox.Name = "nameTextBox";
		nameTextBox.ReadOnly = true;
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
		((Control)(object)this).Name = "ResourceGeneralPropertyPage";
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

