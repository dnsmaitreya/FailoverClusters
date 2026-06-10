using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class ResourceDependenciesPropertyPage : SnapinPropertyPageControlBase
{
	private class ResourceInfo
	{
		private ResourceClass resourceClass;

		internal ResourceClass ResourceClass
		{
			get
			{
				return resourceClass;
			}
			set
			{
				resourceClass = value;
			}
		}

		internal string ResourceType { get; set; }
	}

	private readonly Dictionary<Resource, ResourceInfo> clusterResources = new Dictionary<Resource, ResourceInfo>();

	private FailoverClusters.Framework.RequiredDependencies requiredDependencies;

	private DependencyRelationship dependencyRelationship;

	private string dependecyExpression;

	private readonly FilterGrid2 _filterGrid2;

	private readonly ToolTip tooltip;

	private readonly Guid resourceId;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private Resource resource;

	private readonly AutoResetEvent dependencyExpressionChangedEvent = new AutoResetEvent(initialState: false);

	private IContainer components;

	private Label instructionsLabel;

	private Button deleteButton;

	private Button insertButton;

	private SplitContainer splitContainer;

	private Label expressionLabel;

	private Label topSplitterLabel;

	private Label bottomSplitterLabel;

	private Panel filterGridPanel;

	internal ResourceDependenciesPropertyPage()
	{
		InitializeComponent();
		tooltip = new ToolTip();
		_filterGrid2 = new FilterGrid2();
		_filterGrid2.Name = "filterGrid";
		_filterGrid2.Dock = DockStyle.Fill;
		_filterGrid2.ForeColor = SystemColors.ControlText;
		_filterGrid2.FilterExpressionChanged += FilterExpressionChanged;
		_filterGrid2.SelectionChanged += FilterSelectionChanged;
		filterGridPanel.Controls.Add(_filterGrid2);
		resource = null;
	}

	internal ResourceDependenciesPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid resourceId)
		: base(Resources.Dependencies_Text)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		if (resourceId == Guid.Empty)
		{
			throw new ArgumentException(ExceptionResources.InvalidArgument_Text, "resourceId");
		}
		this.cluster = cluster;
		this.resourceId = resourceId;
		InitializeComponent();
		tooltip = new ToolTip();
		_filterGrid2 = new FilterGrid2();
		_filterGrid2.Name = "filterGrid";
		_filterGrid2.Dock = DockStyle.Fill;
		_filterGrid2.ForeColor = SystemColors.ControlText;
		_filterGrid2.FilterExpressionChanged += FilterExpressionChanged;
		_filterGrid2.SelectionChanged += FilterSelectionChanged;
		filterGridPanel.Controls.Add(_filterGrid2);
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
			resource.DependencyRelationshipChanged += ResourceDependencyRelationshipChanged;
			resource.LoadAsync(delegate(ClusterLoadedEventArgs result)
			{
				if (result.Error != null)
				{
					throw result.Error;
				}
				resource.OwnerGroup.AllResourcesNonCsv.ExecuteQuery(AllResourcesQuery);
			}, ResourceLoadSelection.Basic | ResourceLoadSelection.PrivateProperties | ResourceLoadSelection.Dependencies | ResourceLoadSelection.DependenciesRelation | ResourceLoadSelection.RequiredDependencies | ResourceLoadSelection.Reload);
		}, OperationType.Async);
		return null;
	}

	private void ResourceDependencyRelationshipChanged(object sender, ClusterDependencyRelationshipEventArgs e)
	{
		dependencyExpressionChangedEvent.Set();
	}

	private void AllResourcesQuery(OperationResult<IClusterList<Resource>> groupResourcesResult)
	{
		if (groupResourcesResult.Error != null)
		{
			resource.DependencyRelationshipChanged -= ResourceDependencyRelationshipChanged;
			throw groupResourcesResult.Error;
		}
		IClusterList<Resource> resources = groupResourcesResult.Result;
		if (resource.Dependencies.Any())
		{
			dependencyExpressionChangedEvent.WaitOne();
		}
		resource.Cluster.LoadAsync(resources, null, delegate(OperationResult operationResult)
		{
			try
			{
				if (operationResult.Error != null)
				{
					throw operationResult.Error;
				}
				SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
				{
					requiredDependencies = resource.RequiredDependencies;
					dependencyRelationship = resource.DependencyRelationship;
					Dictionary<Guid, Guid> dependentResources = GetDependentResources();
					clusterResources.Clear();
					foreach (Resource item in resources)
					{
						if (!item.Name.Equals(resource.Name, StringComparison.OrdinalIgnoreCase) && !dependentResources.ContainsKey(item.Id))
						{
							ResourceInfo value = new ResourceInfo
							{
								ResourceClass = item.Class,
								ResourceType = item.ResourceType.Name
							};
							clusterResources.Add(item, value);
						}
					}
					_filterGrid2.SetResources(new List<Resource>(clusterResources.Keys));
					LoadControls();
				});
			}
			finally
			{
				resource.DependencyRelationshipChanged -= ResourceDependencyRelationshipChanged;
			}
		}, 4);
	}

	private Dictionary<Guid, Guid> GetDependentResources()
	{
		Dictionary<Guid, Guid> dictionary = new Dictionary<Guid, Guid>();
		GetDependentResources(dictionary, resource);
		return dictionary;
	}

	private void GetDependentResources(Dictionary<Guid, Guid> dependents, Resource parentResource)
	{
		if (parentResource.Dependents == null)
		{
			return;
		}
		foreach (Guid dependentId in parentResource.Dependents)
		{
			if (dependentId == parentResource.Id)
			{
				continue;
			}
			Resource.Get(cluster, dependentId, delegate(OperationResult<Resource> result)
			{
				if (result.Error != null)
				{
					ClusterLog.LogException((LogLevel)1, (Exception)result.Error, Extensions.FormatCurrentCulture("Dependent resource '{0}' not found in the cluster", (object)dependentId));
				}
				else
				{
					dependents[dependentId] = parentResource.Id;
					GetDependentResources(dependents, result.Result);
				}
			}, OperationType.Sync);
		}
	}

	protected override void InitializePage()
	{
		((Control)(object)this).Enabled = false;
	}

	private void LoadControls()
	{
		if (dependencyRelationship != null)
		{
			_filterGrid2.SetDependencyExpression(dependencyRelationship);
		}
		((Control)(object)this).Enabled = true;
		base.IsDirty = false;
	}

	protected override bool ValidateProperties()
	{
		string missingRequiredDependencies = GetMissingRequiredDependencies();
		if (missingRequiredDependencies.Length > 0 && base.NotifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, Resources.MissingDependencies_Text, new object[1] { missingRequiredDependencies }) != DialogResult.Yes)
		{
			return false;
		}
		dependecyExpression = _filterGrid2.GetDependencyExpression();
		if (dependecyExpression == null)
		{
			return false;
		}
		return true;
	}

	protected override bool SaveProperties()
	{
		bool success = false;
		SettingChanger dependencyUpdateWaiter = new SettingChanger(initialState: false);
		try
		{
			resource.RedirectAsyncOutput(delegate
			{
				DependencyRelationship.Parse(resource.OwnerGroup, dependecyExpression, ResultExecution.Sync, delegate(DependencyRelationship result)
				{
					if (result == null)
					{
						throw new ClusterSavePropertiesException(resource.DisplayName);
					}
					resource.DependencyRelationship = result;
				});
			}, delegate(OperationResult result)
			{
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				success = result.Error == null;
				dependencyUpdateWaiter.Set();
				if (!success)
				{
					ClusterLog.LogException((Exception)result.Error, "Error saving resource dependencies");
					ClusterDialogException.ShowTaskDialog(result.Error, base.HWND);
				}
			});
		}
		finally
		{
			if (dependencyUpdateWaiter != null)
			{
				((IDisposable)dependencyUpdateWaiter).Dispose();
			}
		}
		return success;
	}

	protected override void CompleteSaveProperties()
	{
	}

	private string GetMissingRequiredDependencies()
	{
		List<ResourceClass> list = new List<ResourceClass>(requiredDependencies.ResourceClassDependencies);
		List<string> list2 = new List<string>(requiredDependencies.ResourceTypeDependencies);
		foreach (ResourceInfo item in from dependency in _filterGrid2.GetDependencies()
			where dependency != null
			select clusterResources[dependency])
		{
			if (list.Count > 0)
			{
				list.Remove(item.ResourceClass);
			}
			if (list2.Count > 0)
			{
				list2.Remove(item.ResourceType);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ResourceClass item2 in list)
		{
			FormatHelp.AddToList(stringBuilder, item2.Translate());
		}
		foreach (string item3 in list2)
		{
			FormatHelp.AddToList(stringBuilder, item3);
		}
		return stringBuilder.ToString();
	}

	private void DeleteButtonClick(object sender, EventArgs e)
	{
		if (resource.ResourceType.ResourceKind != ResourceKind.DfsReplicatedFolder || base.NotifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, Resources.DeleteDFSRResourceConfirm_Text) == DialogResult.Yes)
		{
			_filterGrid2.DeleteSelectedClause();
		}
	}

	private void InsertButtonClick(object sender, EventArgs e)
	{
		int index = 0;
		if (_filterGrid2.CurrentRow != null)
		{
			index = _filterGrid2.CurrentRow.Index;
		}
		_filterGrid2.InsertRow(index);
		_filterGrid2.Focus();
	}

	private void FilterExpressionChanged(object sender, EventArgs e)
	{
		try
		{
			expressionLabel.Text = _filterGrid2.GetFriendlyDependencyExpression();
			tooltip.SetToolTip(expressionLabel, expressionLabel.Text);
			base.IsDirty = true;
			UpdateButtons();
		}
		catch (Exception ex)
		{
			ClusterLog.LogException(ex, "Error changing filter expressions");
			ClusterDialogException.ShowTaskDialogAsync(ex, base.HWND);
		}
	}

	private void FilterSelectionChanged(object sender, EventArgs e)
	{
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		deleteButton.Enabled = _filterGrid2.CanDeleteSelectedRows();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (components != null)
			{
				components.Dispose();
			}
			dependencyExpressionChangedEvent.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ResourceDependenciesPropertyPage));
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
		expressionLabel.Name = "expressionLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(splitContainer);
		((Control)(object)this).Name = "ResourceDependenciesPropertyPage";
		((Control)(object)this).Controls.SetChildIndex(splitContainer, 0);
		splitContainer.Panel1.ResumeLayout(performLayout: false);
		splitContainer.Panel2.ResumeLayout(performLayout: false);
		((ISupportInitialize)splitContainer).EndInit();
		splitContainer.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

