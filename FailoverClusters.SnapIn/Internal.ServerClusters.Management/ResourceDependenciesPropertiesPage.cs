using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class ResourceDependenciesPropertiesPage : PropertyPageControlBase
{
	private class ResourceInfo
	{
		public ClusterResource Resource;

		public ClusterResourceClass ResourceClass;

		public string ResourceType;
	}

	private Dictionary<ClusterResource, ResourceInfo> clusterResources = new Dictionary<ClusterResource, ResourceInfo>();

	private RequiredDependencies requiredDependencies;

	private ClusterResourceRelationship dependencyRelationship;

	private string dependecyExpression;

	private ResourceContext context;

	private FilterGrid filterGrid;

	private ToolTip tooltip;

	private IContainer components;

	private Label instructionsLabel;

	private Button deleteButton;

	private Button insertButton;

	private SplitContainer splitContainer;

	private Label expressionLabel;

	private Label topSplitterLabel;

	private Label bottomSplitterLabel;

	private Panel filterGridPanel;

	internal ResourceDependenciesPropertiesPage(ResourceContext context)
		: base(Resources.Dependencies_Text)
	{
		this.context = context;
		InitializeComponent();
		tooltip = new ToolTip();
		filterGrid = new FilterGrid();
		filterGrid.Name = "filterGrid";
		filterGrid.Dock = DockStyle.Fill;
		filterGrid.ForeColor = SystemColors.ControlText;
		filterGrid.FilterExpressionChanged += FilterExpressionChanged;
		filterGrid.SelectionChanged += FilterSelectionChanged;
		filterGridPanel.Controls.Add(filterGrid);
	}

	protected override void LoadProperties()
	{
		requiredDependencies = context.Resource.GetRequiredDependencies();
		dependencyRelationship = context.Resource.GetDependencyRelationship();
		Dictionary<Guid, Guid> dependentResources = GetDependentResources();
		clusterResources.Clear();
		foreach (ClusterResource resource in context.Resource.GetOwnerGroup().GetResources())
		{
			if (!resource.Name.Equals(context.Resource.Name, StringComparison.OrdinalIgnoreCase) && !dependentResources.ContainsKey(resource.Id))
			{
				ResourceInfo resourceInfo = new ResourceInfo();
				resourceInfo.Resource = resource;
				resourceInfo.ResourceClass = resource.GetResourceClass();
				resourceInfo.ResourceType = resource.ResourceTypeName;
				clusterResources.Add(resource, resourceInfo);
			}
		}
		filterGrid.SetResources(new List<ClusterResource>(clusterResources.Keys));
	}

	private Dictionary<Guid, Guid> GetDependentResources()
	{
		Dictionary<Guid, Guid> dictionary = new Dictionary<Guid, Guid>();
		GetDependentResources(dictionary, context.Resource);
		return dictionary;
	}

	private void GetDependentResources(Dictionary<Guid, Guid> dependents, ClusterResource resource)
	{
		foreach (ClusterResource dependent in resource.GetDependents())
		{
			dependents[dependent.Id] = Guid.Empty;
			GetDependentResources(dependents, dependent);
		}
	}

	protected override void InitializePage()
	{
		filterGrid.SetDependencyExpression(dependencyRelationship);
	}

	protected override bool ValidateProperties()
	{
		string missingRequiredDependencies = GetMissingRequiredDependencies();
		if (missingRequiredDependencies.Length > 0 && base.NotifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, Resources.MissingDependencies_Text, new object[1] { missingRequiredDependencies }) != DialogResult.Yes)
		{
			return false;
		}
		dependecyExpression = filterGrid.GetDependencyExpression();
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			context.Resource.SetDependencyRelationship(ClusterResourceRelationship.Parse(context.Resource.GetOwnerGroup(), dependecyExpression));
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null && firstException.NativeErrorCode == -2147019877)
			{
				throw ExceptionHelp.Build<ApplicationException>(firstException, new string[2]
				{
					Resources.ResourceDependenciesSavedFailed_Text,
					context.DisplayName
				});
			}
			ExceptionHelp.LogException(ex, "Error saving resource dependencies");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.ResourceDependenciesSavedFailed_Text,
				context.DisplayName
			});
		}
	}

	protected override void CompleteSaveProperties()
	{
	}

	private string GetMissingRequiredDependencies()
	{
		List<ClusterResourceClass> list = new List<ClusterResourceClass>(requiredDependencies.ResourceClassDependencies);
		List<string> list2 = new List<string>(requiredDependencies.ResourceTypeDependencies);
		ClusterResource[] dependencies = filterGrid.GetDependencies();
		foreach (ClusterResource key in dependencies)
		{
			ResourceInfo resourceInfo = clusterResources[key];
			if (list.Count > 0)
			{
				list.Remove(resourceInfo.ResourceClass);
			}
			if (list2.Count > 0)
			{
				list2.Remove(resourceInfo.ResourceType);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ClusterResourceClass item in list)
		{
			FormatHelp.AddToList(stringBuilder, FormatHelp.GetResourceClassString(item));
		}
		foreach (string item2 in list2)
		{
			FormatHelp.AddToList(stringBuilder, item2);
		}
		return stringBuilder.ToString();
	}

	private void DeleteButtonClick(object sender, EventArgs e)
	{
		if (!(context.ResourceType == WellKnownResourceType.DfsReplicatedFolder) || base.NotifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, Resources.DeleteDFSRResourceConfirm_Text) == DialogResult.Yes)
		{
			filterGrid.DeleteSelectedClause();
		}
	}

	private void InsertButtonClick(object sender, EventArgs e)
	{
		int index = 0;
		if (filterGrid.CurrentRow != null)
		{
			index = filterGrid.CurrentRow.Index;
		}
		filterGrid.InsertRow(index);
		filterGrid.Focus();
	}

	private void FilterExpressionChanged(object sender, EventArgs e)
	{
		try
		{
			expressionLabel.Text = filterGrid.GetFriendlyDependencyExpression();
			tooltip.SetToolTip(expressionLabel, expressionLabel.Text);
			base.IsDirty = true;
			UpdateButtons();
		}
		catch (Exception caughtException)
		{
			if (ExceptionHelp.IsFirstExceptionFound<InvalidOperationException>(caughtException))
			{
				ExceptionHelp.LogException(caughtException, "Error setting page to be dirty");
				return;
			}
			throw;
		}
	}

	private void FilterSelectionChanged(object sender, EventArgs e)
	{
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		if (filterGrid.CanDeleteSelectedRows())
		{
			deleteButton.Enabled = true;
		}
		else
		{
			deleteButton.Enabled = false;
		}
	}

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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ResourceDependenciesPropertiesPage));
		instructionsLabel = new Label();
		deleteButton = new Button();
		insertButton = new Button();
		splitContainer = new SplitContainer();
		filterGridPanel = new Panel();
		topSplitterLabel = new Label();
		bottomSplitterLabel = new Label();
		expressionLabel = new Label();
		((ISupportInitialize)splitContainer).BeginInit();
		splitContainer.Panel1.SuspendLayout();
		splitContainer.Panel2.SuspendLayout();
		splitContainer.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(instructionsLabel, "instructionsLabel");
		instructionsLabel.ForeColor = SystemColors.ControlText;
		instructionsLabel.Name = "instructionsLabel";
		componentResourceManager.ApplyResources(deleteButton, "deleteButton");
		deleteButton.ForeColor = SystemColors.ControlText;
		deleteButton.Name = "deleteButton";
		deleteButton.UseVisualStyleBackColor = true;
		deleteButton.Click += DeleteButtonClick;
		componentResourceManager.ApplyResources(insertButton, "insertButton");
		insertButton.AutoEllipsis = true;
		insertButton.ForeColor = SystemColors.ControlText;
		insertButton.Name = "insertButton";
		insertButton.UseVisualStyleBackColor = true;
		insertButton.Click += InsertButtonClick;
		componentResourceManager.ApplyResources(splitContainer, "splitContainer");
		splitContainer.Name = "splitContainer";
		splitContainer.Panel1.Controls.Add(filterGridPanel);
		splitContainer.Panel1.Controls.Add(topSplitterLabel);
		splitContainer.Panel1.Controls.Add(insertButton);
		splitContainer.Panel1.Controls.Add(instructionsLabel);
		splitContainer.Panel1.Controls.Add(deleteButton);
		splitContainer.Panel2.Controls.Add(bottomSplitterLabel);
		splitContainer.Panel2.Controls.Add(expressionLabel);
		componentResourceManager.ApplyResources(filterGridPanel, "filterGridPanel");
		filterGridPanel.Name = "filterGridPanel";
		topSplitterLabel.BackColor = SystemColors.ControlDark;
		componentResourceManager.ApplyResources(topSplitterLabel, "topSplitterLabel");
		topSplitterLabel.ForeColor = SystemColors.ControlText;
		topSplitterLabel.Name = "topSplitterLabel";
		bottomSplitterLabel.BackColor = SystemColors.ControlDark;
		componentResourceManager.ApplyResources(bottomSplitterLabel, "bottomSplitterLabel");
		bottomSplitterLabel.ForeColor = SystemColors.ControlText;
		bottomSplitterLabel.Name = "bottomSplitterLabel";
		componentResourceManager.ApplyResources(expressionLabel, "expressionLabel");
		expressionLabel.AutoEllipsis = true;
		expressionLabel.ForeColor = SystemColors.ControlText;
		expressionLabel.Name = "expressionLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(splitContainer);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "ResourceDependenciesPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(splitContainer, 0);
		splitContainer.Panel1.ResumeLayout(performLayout: false);
		splitContainer.Panel2.ResumeLayout(performLayout: false);
		((ISupportInitialize)splitContainer).EndInit();
		splitContainer.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
