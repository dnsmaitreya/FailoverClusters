using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class ResourceTypeGeneralPropertiesPage : PropertyPageControlBase
{
	private readonly List<ListViewItem> possibleOwners = new List<ListViewItem>();

	private readonly ClusterResourceType resourceType;

	private readonly List<string> adminExtensionsWarnings = new List<string>();

	private readonly List<string> extensionsToAdd = new List<string>();

	private readonly List<string> extensionsToRemove = new List<string>();

	private string name;

	private string displayName;

	private string dllName;

	private uint looksAlivePollInterval;

	private uint isAlivePollInterval;

	private bool isQuorumCapable;

	private bool isDeleteVcoOnResCleanupSupported;

	private bool deleteVcoOnResCleanup;

	private StringCollection adminExtensions;

	private bool nameDirty;

	private bool commonPropertiesDirty;

	private bool privatePropertiesDirty;

	private IContainer components;

	private Label displayNameLabel;

	private TextBox displayNameTextBox;

	private Label resourceTypeNameLabel;

	private PictureBox resourceTypeIconPictureBox;

	private Label possibleOwnerslabel;

	private SnapinGroupBox pollIntervalsGroupBox;

	private Label looksAliveLabel;

	private Label isAliveLabel;

	private BaseListView possibleOwnersList;

	private SnapinGroupBox extensionGroupBox;

	private Button RemoveButton;

	private Button AddButton;

	private NamedValueLabel quorumCapableValueLabel;

	private NamedValueLabel resourceDllValueLabel;

	private NamedValueLabel extensionsValueLabel;

	private FlowLayoutPanel flowLayoutPanel;

	private CheckBox deleteVcoCheckBox;

	private TimePicker looksAliveTimePicker;

	private TimePicker isAliveTimePicker;

	internal ResourceTypeGeneralPropertiesPage(ClusterResourceType resourceType)
		: base(Resources.General_Text)
	{
		this.resourceType = resourceType;
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		PropertyCollection commonProperties = resourceType.GetCommonProperties(PropertyCollectionSet.Both);
		PropertyCollection privateProperties = resourceType.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		name = resourceType.Name;
		displayName = (string)commonProperties["Name"].Value;
		dllName = (string)commonProperties["DllName"].Value;
		looksAlivePollInterval = (uint)commonProperties["LooksAlivePollInterval"].Value;
		isAlivePollInterval = (uint)commonProperties["IsAlivePollInterval"].Value;
		isQuorumCapable = resourceType.IsQuorumCapable;
		isDeleteVcoOnResCleanupSupported = privateProperties.Contains("DeleteVcoOnResCleanup");
		deleteVcoOnResCleanup = isDeleteVcoOnResCleanupSupported && (uint)privateProperties["DeleteVcoOnResCleanup"].Value != 0;
		adminExtensions = (StringCollection)commonProperties["AdminExtensions"].Value;
		possibleOwners.Clear();
		foreach (ListViewItem item in from owner in resourceType.GetPossibleOwnerNodes()
			select new ListViewItem
			{
				Tag = owner,
				Text = owner.Name,
				ImageIndex = Icons.NodeIndex
			})
		{
			possibleOwners.Add(item);
		}
	}

	protected override void InitializePage()
	{
		Icon icon = Icons.GetIcon(IconsHelp.GetResourceTypeIconIndex(name));
		WinFormsHelp.SetPictureBoxImage(resourceTypeIconPictureBox, icon);
		resourceTypeNameLabel.Text = name;
		displayNameTextBox.Text = displayName;
		looksAliveTimePicker.Value = looksAlivePollInterval;
		looksAliveTimePicker.NotifyUser = base.NotifyUser;
		isAliveTimePicker.Value = isAlivePollInterval;
		isAliveTimePicker.NotifyUser = base.NotifyUser;
		resourceDllValueLabel.DataValue = dllName;
		quorumCapableValueLabel.DataValue = (isQuorumCapable ? Resources.Yes_Text : Resources.No_Text);
		if (isDeleteVcoOnResCleanupSupported)
		{
			deleteVcoCheckBox.Checked = deleteVcoOnResCleanup;
		}
		else
		{
			deleteVcoCheckBox.Visible = false;
		}
		((ListView)(object)possibleOwnersList).BeginUpdate();
		((ListView)(object)possibleOwnersList).SmallImageList = IconsHelp.SmallImageList;
		((ListView)(object)possibleOwnersList).Columns.Add("0", ((Control)(object)possibleOwnersList).Width - 24);
		possibleOwnersList.Items.AddRange(possibleOwners.ToArray());
		((ListView)(object)possibleOwnersList).EndUpdate();
		UpdateExtensionCount();
		if (WellKnownResourceType.IsWellKnownResourceType(name))
		{
			displayNameTextBox.ReadOnly = true;
			((Control)(object)extensionGroupBox).Visible = false;
		}
		nameDirty = false;
		commonPropertiesDirty = false;
		privatePropertiesDirty = false;
		((Control)(object)possibleOwnersList).ForeColor = ((Control)(object)possibleOwnersList).ForeColor;
		quorumCapableValueLabel.UseBoldFontForName = false;
		resourceDllValueLabel.UseBoldFontForName = false;
	}

	private void UpdateExtensionCount()
	{
		extensionsValueLabel.DataValue = adminExtensions.Count.ToString(CultureInfo.CurrentCulture);
		RemoveButton.Enabled = adminExtensions.Count != 0;
	}

	protected override bool ValidateProperties()
	{
		if (nameDirty)
		{
			name = InputValidator.ValidateNonemptyString(displayNameTextBox.Text, Resources.Name_Text);
			InputValidator.ValidateResourceTypeName(name, resourceType.Cluster);
		}
		if (commonPropertiesDirty)
		{
			looksAlivePollInterval = looksAliveTimePicker.Value;
			isAlivePollInterval = isAliveTimePicker.Value;
		}
		if (privatePropertiesDirty)
		{
			deleteVcoOnResCleanup = deleteVcoCheckBox.Checked;
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			if (nameDirty)
			{
				PropertyCollection commonProperties = resourceType.GetCommonProperties(PropertyCollectionSet.ReadWrite);
				commonProperties["Name"].Value = name;
				commonProperties.SaveChanges();
				nameDirty = false;
			}
			if (commonPropertiesDirty)
			{
				PropertyCollection commonProperties2 = resourceType.GetCommonProperties(PropertyCollectionSet.ReadWrite);
				commonProperties2["LooksAlivePollInterval"].Value = looksAlivePollInterval;
				commonProperties2["IsAlivePollInterval"].Value = isAlivePollInterval;
				commonProperties2.SaveChanges();
				commonPropertiesDirty = false;
			}
			if (privatePropertiesDirty)
			{
				PropertyCollection privateProperties = resourceType.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				privateProperties["DeleteVcoOnResCleanup"].Value = (deleteVcoOnResCleanup ? 1u : 0u);
				privateProperties.SaveChanges();
				privatePropertiesDirty = false;
			}
			if (extensionsToAdd.Count <= 0 && extensionsToRemove.Count <= 0)
			{
				return;
			}
			try
			{
				foreach (string item in extensionsToAdd)
				{
					int num = RegisterAdminExtensions(resourceType.Cluster.ClusterHandle, item);
					if (num != 0)
					{
						throw ExceptionHelp.Build<ClusterInputValidationException>(new string[3]
						{
							Resources.CannotAddExtension_Text,
							item,
							MS.Internal.ServerClusters.Utilities.FormatError(num)
						});
					}
				}
				foreach (string item2 in extensionsToRemove)
				{
					int num2 = UnregisterAdminExtensions(resourceType.Cluster.ClusterHandle, item2);
					if (num2 != 0)
					{
						throw ExceptionHelp.Build<ClusterInputValidationException>(new string[3]
						{
							Resources.CannotRemoveExtension_Text,
							item2,
							MS.Internal.ServerClusters.Utilities.FormatError(num2)
						});
					}
				}
				adminExtensionsWarnings.Clear();
				string text = ToString(adminExtensions);
				adminExtensions = (StringCollection)resourceType.GetCommonProperties(PropertyCollectionSet.ReadWrite)["AdminExtensions"].Value;
				if (text.Equals(ToString(adminExtensions), StringComparison.OrdinalIgnoreCase))
				{
					adminExtensionsWarnings.Add(Resources.AdminExtensionsRegisterWarning_Text);
					return;
				}
				foreach (string item3 in extensionsToAdd)
				{
					string text2 = InstallExtensionDll(item3);
					if (text2 != null)
					{
						adminExtensionsWarnings.Add(text2);
					}
				}
			}
			finally
			{
				extensionsToAdd.Clear();
				extensionsToRemove.Clear();
			}
		}
		catch (ApplicationException ex)
		{
			ExceptionHelp.LogException(ex, "Error saving resource type properties");
			base.NotifyUser.ShowError((Exception)ex, Resources.ResourceTypeFailed_Text);
		}
		catch (ClusterBaseException ex2)
		{
			ExceptionHelp.LogException(ex2, "Error saving resource type properties");
			base.NotifyUser.ShowError((Exception)ex2, Resources.ResourceTypeFailed_Text);
		}
		catch (Win32Exception ex3)
		{
			ExceptionHelp.LogException(ex3, "Error saving resource type properties");
			base.NotifyUser.ShowError((Exception)ex3, Resources.ResourceTypeFailed_Text);
		}
		catch (Exception ex4)
		{
			ExceptionHelp.LogException(ex4, "Error saving resource type properties");
			throw ExceptionHelp.Build<ApplicationException>(ex4, new string[1] { Resources.ResourceTypeFailed_Text });
		}
	}

	protected override void CompleteSaveProperties()
	{
		UpdateExtensionCount();
		foreach (string adminExtensionsWarning in adminExtensionsWarnings)
		{
			base.NotifyUser.ShowWarning(adminExtensionsWarning);
		}
		adminExtensionsWarnings.Clear();
	}

	private static string ToString(StringCollection collection)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringEnumerator enumerator = collection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		return stringBuilder.ToString();
	}

	private string InstallExtensionDll(string extensionDll)
	{
		bool flag = false;
		Exception ex = null;
		try
		{
			ClusterNodeCollection nodes = resourceType.Cluster.GetNodes();
			foreach (Exception item in new ActionMultiplexor<string, ClusterNode, Exception>(InstallExtensionDllOnNode).Execute(extensionDll, nodes))
			{
				if (item != null)
				{
					ex = item;
				}
				else
				{
					flag = true;
				}
			}
		}
		catch (Exception ex2)
		{
			ExceptionHelp.LogException(ex2, "Error installing extension \"{0}\"", extensionDll);
			ex = ex2;
		}
		if (ex != null && !flag)
		{
			return string.Format(CultureInfo.CurrentCulture, Resources.AdminExtensionsInstallWarning_Text, extensionDll, ExceptionHelp.GetExceptionMessage(ex));
		}
		return null;
	}

	private static Exception InstallExtensionDllOnNode(string extensionDll, ClusterNode node)
	{
		Exception result = null;
		try
		{
			ClusterUtilities.InstallExtensionDll(extensionDll, node);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error installing extension \"{0}\" on node {1}", extensionDll, node.Name);
			result = ex;
		}
		return result;
	}

	private void NameChanged(object sender, EventArgs e)
	{
		base.IsDirty = (nameDirty = true);
	}

	private void PollIntervalChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
		commonPropertiesDirty = true;
	}

	private void PossibleOwnersListItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
	{
		if (e.IsSelected)
		{
			e.Item.Selected = false;
		}
	}

	private void DeleteVcoChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
		privatePropertiesDirty = true;
	}

	private void AddButtonClick(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = CreateOpenFileDialog(Resources.SelectExtensionDllToAdd_Text);
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			extensionsToAdd.Add(openFileDialog.FileName);
			extensionsToRemove.Remove(openFileDialog.FileName);
			base.IsDirty = true;
		}
	}

	private void RemoveButtonClick(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = CreateOpenFileDialog(Resources.SelectExtensionDllToRemove_Text);
		string environmentVariable = Environment.GetEnvironmentVariable("windir");
		if (environmentVariable != null)
		{
			string text = Path.Combine(Path.Combine(environmentVariable, "cluster"), "extensions");
			if (Directory.Exists(text))
			{
				openFileDialog.InitialDirectory = text;
			}
		}
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			extensionsToRemove.Add(openFileDialog.FileName);
			extensionsToAdd.Remove(openFileDialog.FileName);
			base.IsDirty = true;
		}
	}

	private OpenFileDialog CreateOpenFileDialog(string title)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.CheckFileExists = true;
		openFileDialog.Filter = Resources.DLLFilter_Text;
		openFileDialog.RestoreDirectory = true;
		openFileDialog.InitialDirectory = ".";
		openFileDialog.CheckFileExists = true;
		openFileDialog.Title = title;
		return openFileDialog;
	}

	[DllImport("failoverclusters.snapinsupport.dll", CharSet = CharSet.Auto)]
	private static extern int RegisterAdminExtensions(SafeHandle clusterHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string extensionDll);

	[DllImport("failoverclusters.snapinsupport.dll", CharSet = CharSet.Auto)]
	private static extern int UnregisterAdminExtensions(SafeHandle clusterHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string extensionDll);

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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ResourceTypeGeneralPropertiesPage));
		displayNameLabel = new Label();
		displayNameTextBox = new TextBox();
		resourceTypeNameLabel = new Label();
		resourceTypeIconPictureBox = new PictureBox();
		possibleOwnerslabel = new Label();
		pollIntervalsGroupBox = new SnapinGroupBox();
		isAliveTimePicker = new TimePicker();
		looksAliveTimePicker = new TimePicker();
		isAliveLabel = new Label();
		looksAliveLabel = new Label();
		possibleOwnersList = new BaseListView();
		extensionGroupBox = new SnapinGroupBox();
		extensionsValueLabel = new NamedValueLabel();
		RemoveButton = new Button();
		AddButton = new Button();
		quorumCapableValueLabel = new NamedValueLabel();
		resourceDllValueLabel = new NamedValueLabel();
		flowLayoutPanel = new FlowLayoutPanel();
		deleteVcoCheckBox = new CheckBox();
		((ISupportInitialize)resourceTypeIconPictureBox).BeginInit();
		((Control)(object)pollIntervalsGroupBox).SuspendLayout();
		((Control)(object)extensionGroupBox).SuspendLayout();
		flowLayoutPanel.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		displayNameLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(displayNameLabel, "displayNameLabel");
		displayNameLabel.Name = "displayNameLabel";
		displayNameTextBox.BackColor = SystemColors.Window;
		componentResourceManager.ApplyResources(displayNameTextBox, "displayNameTextBox");
		displayNameTextBox.Name = "displayNameTextBox";
		displayNameTextBox.TextChanged += NameChanged;
		componentResourceManager.ApplyResources(resourceTypeNameLabel, "resourceTypeNameLabel");
		resourceTypeNameLabel.ForeColor = SystemColors.ControlText;
		resourceTypeNameLabel.Name = "resourceTypeNameLabel";
		componentResourceManager.ApplyResources(resourceTypeIconPictureBox, "resourceTypeIconPictureBox");
		resourceTypeIconPictureBox.Name = "resourceTypeIconPictureBox";
		resourceTypeIconPictureBox.TabStop = false;
		possibleOwnerslabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(possibleOwnerslabel, "possibleOwnerslabel");
		possibleOwnerslabel.Name = "possibleOwnerslabel";
		((Control)(object)pollIntervalsGroupBox).Controls.Add((Control)(object)isAliveTimePicker);
		((Control)(object)pollIntervalsGroupBox).Controls.Add((Control)(object)looksAliveTimePicker);
		((Control)(object)pollIntervalsGroupBox).Controls.Add(isAliveLabel);
		((Control)(object)pollIntervalsGroupBox).Controls.Add(looksAliveLabel);
		((Control)(object)pollIntervalsGroupBox).ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(pollIntervalsGroupBox, "pollIntervalsGroupBox");
		((Control)(object)pollIntervalsGroupBox).Name = "pollIntervalsGroupBox";
		((GroupBox)(object)pollIntervalsGroupBox).TabStop = false;
		componentResourceManager.ApplyResources(isAliveTimePicker, "isAliveTimePicker");
		isAliveTimePicker.DisplayUnits = (TimePickerUnits)1;
		((Control)(object)isAliveTimePicker).Name = "isAliveTimePicker";
		isAliveTimePicker.NotifyUser = null;
		isAliveTimePicker.Value = 0u;
		isAliveTimePicker.ValueChanged += PollIntervalChanged;
		componentResourceManager.ApplyResources(looksAliveTimePicker, "looksAliveTimePicker");
		looksAliveTimePicker.DisplayUnits = (TimePickerUnits)1;
		((Control)(object)looksAliveTimePicker).Name = "looksAliveTimePicker";
		looksAliveTimePicker.NotifyUser = null;
		looksAliveTimePicker.Value = 0u;
		looksAliveTimePicker.ValueChanged += PollIntervalChanged;
		isAliveLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(isAliveLabel, "isAliveLabel");
		isAliveLabel.Name = "isAliveLabel";
		looksAliveLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(looksAliveLabel, "looksAliveLabel");
		looksAliveLabel.Name = "looksAliveLabel";
		componentResourceManager.ApplyResources(possibleOwnersList, "possibleOwnersList");
		possibleOwnersList.EnableAutoResizeColumns = true;
		((ListView)(object)possibleOwnersList).FullRowSelect = true;
		possibleOwnersList.HeaderStyle = ColumnHeaderStyle.None;
		((ListView)(object)possibleOwnersList).MultiSelect = false;
		((Control)(object)possibleOwnersList).Name = "possibleOwnersList";
		((ListView)(object)possibleOwnersList).UseCompatibleStateImageBehavior = false;
		((ListView)(object)possibleOwnersList).View = View.Details;
		((ListView)(object)possibleOwnersList).ItemSelectionChanged += PossibleOwnersListItemSelectionChanged;
		((Control)(object)extensionGroupBox).Controls.Add((Control)(object)extensionsValueLabel);
		((Control)(object)extensionGroupBox).Controls.Add(RemoveButton);
		((Control)(object)extensionGroupBox).Controls.Add(AddButton);
		((GroupBox)(object)extensionGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)extensionGroupBox).ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(extensionGroupBox, "extensionGroupBox");
		((Control)(object)extensionGroupBox).Name = "extensionGroupBox";
		((GroupBox)(object)extensionGroupBox).TabStop = false;
		componentResourceManager.ApplyResources(extensionsValueLabel, "extensionsValueLabel");
		extensionsValueLabel.EnableLink = false;
		((Control)(object)extensionsValueLabel).ForeColor = SystemColors.ControlText;
		((Control)(object)extensionsValueLabel).MinimumSize = new Size(10, 13);
		((Control)(object)extensionsValueLabel).Name = "extensionsValueLabel";
		extensionsValueLabel.UseBoldFontForName = false;
		componentResourceManager.ApplyResources(RemoveButton, "RemoveButton");
		RemoveButton.Name = "RemoveButton";
		RemoveButton.UseVisualStyleBackColor = true;
		RemoveButton.Click += RemoveButtonClick;
		componentResourceManager.ApplyResources(AddButton, "AddButton");
		AddButton.Name = "AddButton";
		AddButton.UseVisualStyleBackColor = true;
		AddButton.Click += AddButtonClick;
		componentResourceManager.ApplyResources(quorumCapableValueLabel, "quorumCapableValueLabel");
		quorumCapableValueLabel.EnableLink = false;
		((Control)(object)quorumCapableValueLabel).ForeColor = SystemColors.ControlText;
		((Control)(object)quorumCapableValueLabel).MinimumSize = new Size(10, 13);
		((Control)(object)quorumCapableValueLabel).Name = "quorumCapableValueLabel";
		quorumCapableValueLabel.UseBoldFontForName = false;
		componentResourceManager.ApplyResources(resourceDllValueLabel, "resourceDllValueLabel");
		resourceDllValueLabel.EnableLink = false;
		((Control)(object)resourceDllValueLabel).ForeColor = SystemColors.ControlText;
		((Control)(object)resourceDllValueLabel).MinimumSize = new Size(10, 13);
		((Control)(object)resourceDllValueLabel).Name = "resourceDllValueLabel";
		resourceDllValueLabel.UseBoldFontForName = false;
		flowLayoutPanel.Controls.Add((Control)(object)extensionGroupBox);
		flowLayoutPanel.Controls.Add(deleteVcoCheckBox);
		flowLayoutPanel.Controls.Add((Control)(object)quorumCapableValueLabel);
		flowLayoutPanel.Controls.Add((Control)(object)resourceDllValueLabel);
		componentResourceManager.ApplyResources(flowLayoutPanel, "flowLayoutPanel");
		flowLayoutPanel.Name = "flowLayoutPanel";
		componentResourceManager.ApplyResources(deleteVcoCheckBox, "deleteVcoCheckBox");
		deleteVcoCheckBox.ForeColor = SystemColors.ControlText;
		deleteVcoCheckBox.Name = "deleteVcoCheckBox";
		deleteVcoCheckBox.UseVisualStyleBackColor = true;
		deleteVcoCheckBox.CheckedChanged += DeleteVcoChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)pollIntervalsGroupBox);
		((Control)(object)this).Controls.Add((Control)(object)possibleOwnersList);
		((Control)(object)this).Controls.Add(flowLayoutPanel);
		((Control)(object)this).Controls.Add(possibleOwnerslabel);
		((Control)(object)this).Controls.Add(resourceTypeNameLabel);
		((Control)(object)this).Controls.Add(resourceTypeIconPictureBox);
		((Control)(object)this).Controls.Add(displayNameTextBox);
		((Control)(object)this).Controls.Add(displayNameLabel);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "ResourceTypeGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(displayNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(displayNameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(resourceTypeIconPictureBox, 0);
		((Control)(object)this).Controls.SetChildIndex(resourceTypeNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(possibleOwnerslabel, 0);
		((Control)(object)this).Controls.SetChildIndex(flowLayoutPanel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)possibleOwnersList, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)pollIntervalsGroupBox, 0);
		((ISupportInitialize)resourceTypeIconPictureBox).EndInit();
		((Control)(object)pollIntervalsGroupBox).ResumeLayout(performLayout: false);
		((Control)(object)extensionGroupBox).ResumeLayout(performLayout: false);
		flowLayoutPanel.ResumeLayout(performLayout: false);
		flowLayoutPanel.PerformLayout();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
