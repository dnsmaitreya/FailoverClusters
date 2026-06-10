using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class PropertyGridPropertyPage : ResourcePropertyPage
{
	private ClusterPropertyCollection privateProperties;

	private readonly IContainer components;

	private PropertyGrid2 _propertyGrid2;

	private Label instructionLabel;

	private readonly Guid resourceId;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private Resource resource;

	private bool propertyGridDirty;

	internal PropertyGridPropertyPage()
		: base(Resources.PropertyGrid_Text)
	{
		InitializeComponent();
	}

	internal PropertyGridPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid resourceId)
		: base(Resources.PropertyGrid_Text)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		if (resourceId == Guid.Empty)
		{
			throw new ArgumentException(ExceptionResources.InvalidArgument_Text, "resourceId");
		}
		this.cluster = cluster;
		this.resourceId = resourceId;
		InitializeComponent();
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
					privateProperties = resource.Properties;
					LoadControls();
					propertyGridDirty = false;
				});
			});
		}, OperationType.Async);
		return null;
	}

	protected override void InitializePage()
	{
	}

	private void LoadControls()
	{
		if (privateProperties != null)
		{
			_propertyGrid2.LoadProperties(privateProperties);
			_propertyGrid2.PropertiesChanged += PropertyGrid2PropertiesChanged;
		}
	}

	protected override bool ValidateProperties()
	{
		privateProperties = _propertyGrid2.GetUpdatedPropertyValues();
		return true;
	}

	public override void Cancel()
	{
		base.Cancel();
		if (propertyGridDirty && privateProperties != null)
		{
			privateProperties.Rollback();
		}
	}

	protected override bool SaveProperties()
	{
		bool? localIsDirty = null;
		base.IsDirty = false;
		SettingChanger propertiesUpdateWaiter = new SettingChanger(initialState: false);
		try
		{
			if (propertyGridDirty)
			{
				propertyGridDirty = false;
				privateProperties.Commit(delegate(OperationResult result)
				{
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_003b: Invalid comparison between Unknown and I4
					//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
					if (result.Error is ClusterResourcePropertyStoredException)
					{
						localIsDirty = false;
						propertiesUpdateWaiter.Set();
						if ((int)ClusterDialogException.ShowTaskDialog(result.Error, base.HWND) == 2)
						{
							resource.Recycle();
						}
					}
					else if (result.Error != null)
					{
						localIsDirty = (propertyGridDirty = true);
						propertiesUpdateWaiter.Set();
						Exception ex2 = result.Error;
						if (result.Error is ClusterControlCodeException)
						{
							ex2 = new ClusterSavePropertiesException(resource.Name, result.Error);
						}
						ClusterLog.LogException(ex2, "Error saving resource policies properties");
						ClusterDialogException.ShowTaskDialog(ex2, base.HWND);
					}
					else
					{
						propertiesUpdateWaiter.Set();
					}
				});
			}
		}
		catch (Exception ex)
		{
			ClusterLog.LogException(ex, "Error saving property");
			ClusterDialogException.ShowTaskDialogAsync(ex, base.HWND);
		}
		finally
		{
			if (propertiesUpdateWaiter != null)
			{
				((IDisposable)propertiesUpdateWaiter).Dispose();
			}
		}
		if (localIsDirty.HasValue)
		{
			base.IsDirty = localIsDirty.Value;
		}
		return !base.IsDirty;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			_propertyGrid2.PropertiesChanged -= PropertyGrid2PropertiesChanged;
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PropertyGridPropertyPage));
		DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
		instructionLabel = new Label();
		_propertyGrid2 = new PropertyGrid2();
		((ISupportInitialize)_propertyGrid2).BeginInit();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(instructionLabel, "instructionLabel");
		instructionLabel.BackColor = SystemColors.Control;
		instructionLabel.ForeColor = SystemColors.ControlText;
		instructionLabel.Name = "instructionLabel";
		_propertyGrid2.AllowUserToAddRows = false;
		_propertyGrid2.AllowUserToDeleteRows = false;
		_propertyGrid2.AllowUserToResizeRows = false;
		componentResourceManager.ApplyResources(_propertyGrid2, "_propertyGrid2");
		_propertyGrid2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle.BackColor = SystemColors.Window;
		dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
		dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
		_propertyGrid2.DefaultCellStyle = dataGridViewCellStyle;
		_propertyGrid2.EditMode = DataGridViewEditMode.EditOnEnter;
		_propertyGrid2.MultiSelect = false;
		_propertyGrid2.Name = "_propertyGrid2";
		_propertyGrid2.RowHeadersVisible = false;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(_propertyGrid2);
		((Control)(object)this).Controls.Add(instructionLabel);
		((Control)(object)this).ForeColor = SystemColors.ControlText;
		((Control)(object)this).Name = "PropertyGridPropertyPage";
		((Control)(object)this).Controls.SetChildIndex(instructionLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(_propertyGrid2, 0);
		((ISupportInitialize)_propertyGrid2).EndInit();
		((Control)(object)this).ResumeLayout(performLayout: false);
	}

	private void PropertyGrid2PropertiesChanged(object sender, EventArgs e)
	{
		base.IsDirty = (propertyGridDirty = true);
	}
}

