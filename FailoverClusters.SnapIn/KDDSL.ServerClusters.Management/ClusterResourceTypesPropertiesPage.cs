using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using ManagementConsole;

namespace KDDSL.ServerClusters.Management;

internal class ClusterResourceTypesPropertiesPage : PropertyPageControlBase
{
	private class ResourceTypeListViewItem : ListViewItem, IDisposable
	{
		public ClusterResourceType ResourceType;

		public readonly string ResourceDll;

		public ResourceTypeListViewItem(Control owner, ClusterResourceType resourceType)
		{
			base.Text = CommonResources.LoadingText;
			base.Name = resourceType.Name;
			base.ImageIndex = IconsHelp.GetResourceTypeIconIndex(resourceType.Name);
			ResourceType = resourceType;
			ResourceDll = null;
			base.SubItems.Add(resourceType.Name);
			Background.QueueWorker((WaitCallback)GetResourceTypeProc, (object)new object[2] { owner, resourceType });
		}

		public void Dispose()
		{
			if (ResourceType != null)
			{
				ResourceType.PropertiesChanged -= OnResourceTypePropertiesChanged;
				ResourceType = null;
			}
			GC.SuppressFinalize(this);
		}

		public ResourceTypeListViewItem(string resourceTypeName, string resourceTypeDisplayName, string resourceDll)
		{
			base.Text = resourceTypeDisplayName;
			base.Name = resourceTypeName;
			base.ImageIndex = IconsHelp.GetResourceTypeIconIndex(resourceTypeName);
			base.SubItems.Add(resourceTypeName);
			ResourceType = null;
			ResourceDll = resourceDll;
		}

		private void GetResourceTypeProc(object state)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			Control control = (Control)((object[])state)[0];
			ClusterResourceType clusterResourceType = (ClusterResourceType)((object[])state)[1];
			try
			{
				string displayName = clusterResourceType.DisplayName;
				UIThreadHandlerV val = (UIThreadHandlerV)delegate
				{
					base.Text = displayName;
				};
				UIHelper.ExecuteOnUIThread((ISynchronizeInvoke)control, (Delegate)(object)val, Array.Empty<object>());
				clusterResourceType.GetCommonProperties(PropertyCollectionSet.ReadOnly);
				ResourceType.PropertiesChanged += OnResourceTypePropertiesChanged;
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error loading resource type properties");
				ResourceType = null;
			}
		}

		private void OnResourceTypePropertiesChanged(object sender, EventArgs e)
		{
			string displayName = ResourceType.DisplayName;
			if (base.ListView != null && base.ListView.InvokeRequired)
			{
				base.ListView.Invoke((MethodInvoker)delegate
				{
					base.Text = displayName;
				});
			}
			else
			{
				base.Text = displayName;
			}
		}
	}

	private List<ResourceTypeListViewItem> defaultResourceTypes = new List<ResourceTypeListViewItem>();

	private List<ResourceTypeListViewItem> userResourceTypes = new List<ResourceTypeListViewItem>();

	private List<ResourceTypeListViewItem> resourceTypesToAdd = new List<ResourceTypeListViewItem>();

	private List<ClusterResourceType> resourceTypesToDelete = new List<ClusterResourceType>();

	private List<string> installWarnings = new List<string>();

	private ResourceTypeListViewItem selectedResourceType;

	private ClusterContext context;

	private IContainer components;

	private Label userResourceTypesLabel;

	private BaseListView userResourceTypesListView;

	private Button addButton;

	private Button removeButton;

	private Button propertiesButton;

	private Label addInstructionsLabel;

	private BaseListView defaultResourceTypesListView;

	private Label defaultResourceTypesLabel;

	private ColumnHeader displayNameColumn;

	private ColumnHeader nameColumn;

	private ColumnHeader userDisplayNameHeader;

	private ColumnHeader userNameColumn;

	internal ClusterResourceTypesPropertiesPage(ClusterContext context)
		: base(Resources.ResourceTypes_Text)
	{
		this.context = context;
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		ClearDefaultResourceTypesCollection();
		ClearUserResourceTypeCollection();
		foreach (ClusterResourceType resourceType in context.Cluster.GetResourceTypes())
		{
			if (WellKnownResourceType.IsWellKnownResourceType(resourceType.Name))
			{
				defaultResourceTypes.Add(new ResourceTypeListViewItem((Control)(object)this, resourceType));
			}
			else
			{
				userResourceTypes.Add(new ResourceTypeListViewItem((Control)(object)this, resourceType));
			}
		}
	}

	private void ClearUserResourceTypeCollection()
	{
		foreach (ResourceTypeListViewItem userResourceType in userResourceTypes)
		{
			userResourceType.Dispose();
		}
		userResourceTypes.Clear();
	}

	private void ClearDefaultResourceTypesCollection()
	{
		foreach (ResourceTypeListViewItem defaultResourceType in defaultResourceTypes)
		{
			defaultResourceType.Dispose();
		}
		defaultResourceTypes.Clear();
	}

	protected override void InitializePage()
	{
		((ListView)(object)defaultResourceTypesListView).SmallImageList = IconsHelp.SmallImageList;
		((ListView)(object)userResourceTypesListView).SmallImageList = IconsHelp.SmallImageList;
		LoadResourceLists();
		propertiesButton.Enabled = false;
		removeButton.Enabled = false;
	}

	private void LoadResourceLists()
	{
		((ListView)(object)defaultResourceTypesListView).BeginUpdate();
		defaultResourceTypesListView.Items.Clear();
		((ListView)(object)userResourceTypesListView).BeginUpdate();
		userResourceTypesListView.Items.Clear();
		SetSelectedResourceType(null);
		ClusterListItemCollection items = defaultResourceTypesListView.Items;
		ListViewItem[] array = defaultResourceTypes.ToArray();
		items.AddRange(array);
		ClusterListItemCollection items2 = userResourceTypesListView.Items;
		array = userResourceTypes.ToArray();
		items2.AddRange(array);
		((ListView)(object)defaultResourceTypesListView).EndUpdate();
		((ListView)(object)userResourceTypesListView).EndUpdate();
	}

	protected override bool ValidateProperties()
	{
		Dictionary<string, ClusterResourceType> userResourceTypesList = new Dictionary<string, ClusterResourceType>(StringComparer.OrdinalIgnoreCase);
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ValidatingProperties_Text, string.Empty))
		{
			cluadminWaitDialog.ShowDialog(base.NotifyUser, delegate
			{
				foreach (ClusterResourceType resourceType in context.Cluster.GetResourceTypes())
				{
					if (!WellKnownResourceType.IsWellKnownResourceType(resourceType.Name))
					{
						userResourceTypesList.Add(resourceType.Name, resourceType);
					}
				}
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return false;
			}
		}
		ClearResourceTypesToAddCollection();
		foreach (ResourceTypeListViewItem item in userResourceTypesListView.Items)
		{
			if (!userResourceTypesList.ContainsKey(item.Name))
			{
				resourceTypesToAdd.Add(item);
			}
		}
		resourceTypesToDelete.Clear();
		foreach (ClusterResourceType value in userResourceTypesList.Values)
		{
			if (userResourceTypesListView.Items.Find(value.Name).Length == 0 && base.NotifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, Resources.DoYouWantToDeleteResourceTypeFormatString_Text, new object[1] { value.Name }) == DialogResult.Yes)
			{
				resourceTypesToDelete.Add(value);
			}
		}
		SetSelectedResourceType(null);
		return true;
	}

	private void ClearResourceTypesToAddCollection()
	{
		foreach (ResourceTypeListViewItem item in resourceTypesToAdd)
		{
			item.Dispose();
		}
		resourceTypesToAdd.Clear();
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			installWarnings.Clear();
			foreach (ResourceTypeListViewItem item in resourceTypesToAdd)
			{
				if (InstallResourceDll(item.ResourceDll))
				{
					context.Cluster.CreateResourceType(item.Name, item.Text, Path.GetFileName(item.ResourceDll));
				}
				else
				{
					installWarnings.Add(string.Format(CultureInfo.CurrentCulture, Resources.CannotInstallResourceType_Text, item.Text));
				}
			}
			foreach (ClusterResourceType item2 in resourceTypesToDelete)
			{
				item2.Delete("ClusterResourceTypesPropertiesPage.SaveProperties");
			}
			LoadProperties();
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving resource type properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.ResourceTypeFailed_Text,
				context.DisplayName
			});
		}
	}

	protected override void CompleteSaveProperties()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string installWarning in installWarnings)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(Environment.NewLine).Append(Environment.NewLine);
			}
			stringBuilder.Append(installWarning);
		}
		if (stringBuilder.Length > 0)
		{
			base.NotifyUser.ShowWarning(stringBuilder.ToString());
		}
		LoadResourceLists();
	}

	private bool InstallResourceDll(string resourceDll)
	{
		bool result = false;
		try
		{
			result = ClusterUtilities.InstallResourceType(context.Cluster, resourceDll);
		}
		catch (Exception ex)
		{
			if (!ExceptionHelp.IsFirstExceptionFound<ApplicationException>(ex))
			{
				throw;
			}
			ExceptionHelp.LogException(ex, "Error installing resource DLL");
			lock (installWarnings)
			{
				installWarnings.Add(ExceptionHelp.GetExceptionMessage(ex));
			}
		}
		return result;
	}

	private void ResourceTypeListViewEnter(object sender, EventArgs e)
	{
		ListView listView = (ListView)sender;
		if (listView.SelectedItems.Count > 0)
		{
			SetSelectedResourceType((ResourceTypeListViewItem)listView.SelectedItems[0]);
			userResourceTypesListView.HideSelection = true;
			defaultResourceTypesListView.HideSelection = true;
			selectedResourceType.ListView.HideSelection = false;
		}
	}

	private void ResourceTypeListViewLeave(object sender, EventArgs e)
	{
		ListView listView = (ListView)sender;
		listView.HideSelection = selectedResourceType == null || selectedResourceType.ListView != listView;
	}

	private void SelectedResourceTypeChanged(object sender, ListViewItemSelectionChangedEventArgs e)
	{
		SetSelectedResourceType(e.IsSelected ? ((ResourceTypeListViewItem)e.Item) : null);
		userResourceTypesListView.HideSelection = true;
		defaultResourceTypesListView.HideSelection = true;
	}

	private void SetSelectedResourceType(ResourceTypeListViewItem item)
	{
		selectedResourceType = item;
		propertiesButton.Enabled = item != null && item.ResourceType != null;
		removeButton.Enabled = item != null && item.ListView == userResourceTypesListView;
	}

	private void AddButtonClick(object sender, EventArgs e)
	{
		AddResourceTypeDialog dialog = new AddResourceTypeDialog();
		while (((Form)(object)dialog).ShowDialog() == DialogResult.OK)
		{
			try
			{
				using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ValidatingProperties_Text, string.Empty))
				{
					cluadminWaitDialog.ShowDialog(base.NotifyUser, delegate
					{
						InputValidator.ValidateResourceTypeName(dialog.ResourceTypeName, context.Cluster);
						InputValidator.ValidateResourceTypeName(dialog.ResourceTypeDisplayName, context.Cluster);
					});
					if (cluadminWaitDialog.IsCanceled)
					{
						break;
					}
				}
				if (userResourceTypesListView.Items.Find(dialog.ResourceTypeName).Length != 0)
				{
					throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.ResourceTypeAlreadyAdded_Text });
				}
				ResourceTypeListViewItem resourceTypeListViewItem = new ResourceTypeListViewItem(dialog.ResourceTypeName, dialog.ResourceTypeDisplayName, dialog.ResourceDll);
				userResourceTypesListView.Items.Add((ListViewItem)resourceTypeListViewItem);
				resourceTypeListViewItem.EnsureVisible();
				base.IsDirty = true;
				break;
			}
			catch (ClusterInputValidationException ex)
			{
				base.NotifyUser.ShowError((Exception)ex);
			}
		}
	}

	private void RemoveButtonClick(object sender, EventArgs e)
	{
		if (selectedResourceType.ListView != defaultResourceTypesListView)
		{
			userResourceTypesListView.Items.RemoveAt(selectedResourceType.Index);
			selectedResourceType = null;
			removeButton.Enabled = false;
			propertiesButton.Enabled = false;
			base.IsDirty = true;
			((Control)(object)userResourceTypesListView).Focus();
		}
	}

	private void PropertiesButtonClick(object sender, EventArgs e)
	{
		ShowPropertySheetForSelectedItem();
	}

	private void ItemActivated(object sender, EventArgs e)
	{
		if (selectedResourceType.ResourceType != null)
		{
			ShowPropertySheetForSelectedItem();
		}
	}

	private void ShowPropertySheetForSelectedItem()
	{
		PropertyPageCollection propertyPageCollection = new PropertyPageCollection();
		ClusterPropertyPage clusterPropertyPage = new ClusterPropertyPage();
		clusterPropertyPage.SetControl(new ResourceTypeGeneralPropertiesPage(selectedResourceType.ResourceType));
		propertyPageCollection.Add(clusterPropertyPage);
		SnapinPropertySheet snapinPropertySheet = new SnapinPropertySheet(selectedResourceType.ResourceType.Name, propertyPageCollection);
		base.NotifyUser.ShowDialog((Form)(object)snapinPropertySheet);
		selectedResourceType.ListView.Focus();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			ClearUserResourceTypeCollection();
			ClearResourceTypesToAddCollection();
			ClearDefaultResourceTypesCollection();
			context = null;
		}
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ClusterResourceTypesPropertiesPage));
		userResourceTypesLabel = new Label();
		userResourceTypesListView = new BaseListView();
		userDisplayNameHeader = new ColumnHeader();
		userNameColumn = new ColumnHeader();
		addButton = new Button();
		removeButton = new Button();
		propertiesButton = new Button();
		addInstructionsLabel = new Label();
		defaultResourceTypesListView = new BaseListView();
		displayNameColumn = new ColumnHeader();
		nameColumn = new ColumnHeader();
		defaultResourceTypesLabel = new Label();
		((Control)(object)this).SuspendLayout();
		userResourceTypesLabel.AutoEllipsis = true;
		userResourceTypesLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(userResourceTypesLabel, "userResourceTypesLabel");
		userResourceTypesLabel.Name = "userResourceTypesLabel";
		((ListView)(object)userResourceTypesListView).Columns.AddRange(new ColumnHeader[2] { userDisplayNameHeader, userNameColumn });
		componentResourceManager.ApplyResources(userResourceTypesListView, "userResourceTypesListView");
		userResourceTypesListView.EnableAutoResizeColumns = true;
		((ListView)(object)userResourceTypesListView).FullRowSelect = true;
		userResourceTypesListView.HideSelection = true;
		((ListView)(object)userResourceTypesListView).MultiSelect = false;
		((Control)(object)userResourceTypesListView).Name = "userResourceTypesListView";
		((ListView)(object)userResourceTypesListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)userResourceTypesListView).View = System.Windows.Forms.View.Details;
		((ListView)(object)userResourceTypesListView).ItemActivate += ItemActivated;
		((ListView)(object)userResourceTypesListView).ItemSelectionChanged += SelectedResourceTypeChanged;
		((Control)(object)userResourceTypesListView).Enter += ResourceTypeListViewEnter;
		((Control)(object)userResourceTypesListView).Leave += ResourceTypeListViewLeave;
		componentResourceManager.ApplyResources(userDisplayNameHeader, "userDisplayNameHeader");
		componentResourceManager.ApplyResources(userNameColumn, "userNameColumn");
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
		componentResourceManager.ApplyResources(propertiesButton, "propertiesButton");
		propertiesButton.ForeColor = SystemColors.ControlText;
		propertiesButton.Name = "propertiesButton";
		propertiesButton.UseVisualStyleBackColor = true;
		propertiesButton.Click += PropertiesButtonClick;
		componentResourceManager.ApplyResources(addInstructionsLabel, "addInstructionsLabel");
		addInstructionsLabel.AutoEllipsis = true;
		addInstructionsLabel.ForeColor = SystemColors.ControlText;
		addInstructionsLabel.Name = "addInstructionsLabel";
		((ListView)(object)defaultResourceTypesListView).Columns.AddRange(new ColumnHeader[2] { displayNameColumn, nameColumn });
		componentResourceManager.ApplyResources(defaultResourceTypesListView, "defaultResourceTypesListView");
		defaultResourceTypesListView.EnableAutoResizeColumns = true;
		((ListView)(object)defaultResourceTypesListView).FullRowSelect = true;
		defaultResourceTypesListView.HideSelection = true;
		((ListView)(object)defaultResourceTypesListView).MultiSelect = false;
		((Control)(object)defaultResourceTypesListView).Name = "defaultResourceTypesListView";
		((ListView)(object)defaultResourceTypesListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)defaultResourceTypesListView).View = System.Windows.Forms.View.Details;
		((ListView)(object)defaultResourceTypesListView).ItemActivate += ItemActivated;
		((ListView)(object)defaultResourceTypesListView).ItemSelectionChanged += SelectedResourceTypeChanged;
		((Control)(object)defaultResourceTypesListView).Enter += ResourceTypeListViewEnter;
		((Control)(object)defaultResourceTypesListView).Leave += ResourceTypeListViewLeave;
		componentResourceManager.ApplyResources(displayNameColumn, "displayNameColumn");
		componentResourceManager.ApplyResources(nameColumn, "nameColumn");
		componentResourceManager.ApplyResources(defaultResourceTypesLabel, "defaultResourceTypesLabel");
		defaultResourceTypesLabel.ForeColor = SystemColors.ControlText;
		defaultResourceTypesLabel.Name = "defaultResourceTypesLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)defaultResourceTypesListView);
		((Control)(object)this).Controls.Add(defaultResourceTypesLabel);
		((Control)(object)this).Controls.Add(addInstructionsLabel);
		((Control)(object)this).Controls.Add(propertiesButton);
		((Control)(object)this).Controls.Add(removeButton);
		((Control)(object)this).Controls.Add(addButton);
		((Control)(object)this).Controls.Add((Control)(object)userResourceTypesListView);
		((Control)(object)this).Controls.Add(userResourceTypesLabel);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "ClusterResourceTypesPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(userResourceTypesLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)userResourceTypesListView, 0);
		((Control)(object)this).Controls.SetChildIndex(addButton, 0);
		((Control)(object)this).Controls.SetChildIndex(removeButton, 0);
		((Control)(object)this).Controls.SetChildIndex(propertiesButton, 0);
		((Control)(object)this).Controls.SetChildIndex(addInstructionsLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(defaultResourceTypesLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)defaultResourceTypesListView, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

