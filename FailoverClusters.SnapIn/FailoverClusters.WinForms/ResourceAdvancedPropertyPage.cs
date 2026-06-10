using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using MS.Internal.ServerClusters;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.WinForms;

internal class ResourceAdvancedPropertyPage : ResourcePropertyPage
{
	private const string LooksAlivePollInterval = "LooksAlivePollInterval";

	private const string IsAlivePollInterval = "IsAlivePollInterval";

	private const int LooksAlivePollIntervalInitialValue = 5000;

	private const int IsAlivePollIntervalInitialValue = 60000;

	private List<ListViewItem> possibleOwners = new List<ListViewItem>();

	private uint looksAlivePollInterval;

	private uint isAlivePollInterval;

	private bool separateMonitor;

	private uint resourceTypeLooksAlivePollInterval;

	private uint resourceTypeIsAlivePollInterval;

	private readonly Guid resourceId;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private Resource resource;

	private bool possibleOwnersDirty;

	private bool propertiesDirty;

	private readonly Dictionary<string, Node> possibleOwnersCache = new Dictionary<string, Node>(StringComparer.OrdinalIgnoreCase);

	private IContainer components;

	private CheckBox separateMonitorCheckBox;

	private RadioButton looksAliveSpecifyValueRadioButton;

	private RadioButton isAliveUseValueRadioButton;

	private RadioButton isAliveSpecifyValueRadioButton;

	private BaseListView possibleOwnersList;

	private TimePicker looksAliveTimePicker;

	private TimePicker isAliveTimePicker;

	private RadioButton looksAliveUseValueRadioButton;

	public ResourceAdvancedPropertyPage()
	{
		InitializeComponent();
	}

	public ResourceAdvancedPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid resourceId)
		: base(Resources.AdvancedPolicies_Text)
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
		if (((Control)(object)this).IsDisposed)
		{
			return null;
		}
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
				resource.Cluster.ResourceTypes.ExecuteQuery(ResourceTypesQuery);
			}, ResourceLoadSelection.CommonProperties | ResourceLoadSelection.PossibleOwners);
		}, OperationType.Async);
		return null;
	}

	private void ResourceTypesQuery(OperationResult<IClusterList<ResourceType>> resourceTypeResult)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (resourceTypeResult.Error != null)
		{
			ClusterDialogException.ShowTaskDialog(resourceTypeResult.Error, base.HWND);
			return;
		}
		foreach (ResourceType resType in resourceTypeResult.Result)
		{
			if (!(resType.Name == resource.ResourceType.Name))
			{
				continue;
			}
			resType.LoadAsync(delegate(ClusterLoadedEventArgs typeLoadResult)
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				if (typeLoadResult.Error != null)
				{
					ClusterDialogException.ShowTaskDialog(typeLoadResult.Error, base.HWND);
				}
				else
				{
					UpdatePageControl(resType);
				}
			});
			return;
		}
		UpdatePageControl(null);
	}

	private void UpdatePageControl(ResourceType resourceType)
	{
		if (((Control)(object)this).IsDisposed)
		{
			return;
		}
		SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
		{
			List<ListViewItem> list = new List<ListViewItem>();
			possibleOwnersCache.Clear();
			foreach (Node possibleOwner in resource.PossibleOwners)
			{
				possibleOwnersCache[possibleOwner.Name] = possibleOwner;
				list.Add(CreatePossibleOwnersListItem(possibleOwner, isChecked: true));
			}
			if (resourceType != null)
			{
				list.AddRange(from owner in resourceType.PossibleOwners
					where !possibleOwnersCache.ContainsKey(owner.Name)
					select CreatePossibleOwnersListItem(owner, isChecked: false));
				resourceTypeIsAlivePollInterval = (uint)resourceType.Properties["IsAlivePollInterval"].Value;
				resourceTypeLooksAlivePollInterval = (uint)resourceType.Properties["LooksAlivePollInterval"].Value;
			}
			possibleOwners = list.OrderBy((ListViewItem n) => ((Node)n.Tag).Name, NativeMethods.StrCmpLogicalW);
			ClusterPropertyCollection properties = resource.Properties;
			looksAlivePollInterval = (uint)properties["LooksAlivePollInterval"].Value;
			isAlivePollInterval = (uint)properties["IsAlivePollInterval"].Value;
			separateMonitor = (uint)properties["SeparateMonitor"].Value != 0;
			LoadControls();
		});
	}

	protected override void InitializePage()
	{
		looksAliveTimePicker.NotifyUser = base.NotifyUser;
		isAliveTimePicker.NotifyUser = base.NotifyUser;
		((ListView)(object)possibleOwnersList).SmallImageList = IconsHelp.SmallImageList;
		((ListView)(object)possibleOwnersList).Columns.Add(CommonResources.Zero_Text);
		((Control)(object)this).Enabled = false;
	}

	private void LoadControls()
	{
		if (((Control)(object)this).IsDisposed)
		{
			return;
		}
		((ListView)(object)possibleOwnersList).BeginUpdate();
		possibleOwnersList.Items.AddRange(possibleOwners.ToArray());
		((ListView)(object)possibleOwnersList).AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
		((ListView)(object)possibleOwnersList).EndUpdate();
		if (looksAlivePollInterval == uint.MaxValue)
		{
			looksAliveUseValueRadioButton.Checked = true;
			((Control)(object)looksAliveTimePicker).Enabled = false;
			if (resourceTypeLooksAlivePollInterval < 10 || resourceTypeLooksAlivePollInterval >= uint.MaxValue)
			{
				looksAliveTimePicker.Value = 5000u;
			}
			else
			{
				looksAliveTimePicker.Value = resourceTypeLooksAlivePollInterval;
			}
		}
		else
		{
			looksAliveSpecifyValueRadioButton.Checked = true;
			((Control)(object)looksAliveTimePicker).Enabled = true;
			looksAliveTimePicker.Value = looksAlivePollInterval;
		}
		if (isAlivePollInterval == uint.MaxValue)
		{
			isAliveUseValueRadioButton.Checked = true;
			((Control)(object)isAliveTimePicker).Enabled = false;
			if (resourceTypeIsAlivePollInterval < 10 || resourceTypeIsAlivePollInterval >= uint.MaxValue)
			{
				isAliveTimePicker.Value = 60000u;
			}
			else
			{
				isAliveTimePicker.Value = resourceTypeIsAlivePollInterval;
			}
		}
		else
		{
			isAliveSpecifyValueRadioButton.Checked = true;
			((Control)(object)isAliveTimePicker).Enabled = true;
			isAliveTimePicker.Value = isAlivePollInterval;
		}
		separateMonitorCheckBox.Checked = separateMonitor;
		possibleOwnersDirty = false;
		propertiesDirty = false;
		((Control)(object)this).Enabled = true;
		base.IsDirty = false;
	}

	private static ListViewItem CreatePossibleOwnersListItem(Node node, bool isChecked)
	{
		return new ListViewItem
		{
			Tag = node,
			Text = node.Name,
			ImageIndex = Icons.NodeIndex,
			Checked = isChecked
		};
	}

	protected override bool ValidateProperties()
	{
		if (propertiesDirty)
		{
			if (looksAliveUseValueRadioButton.Checked)
			{
				looksAlivePollInterval = uint.MaxValue;
				looksAliveTimePicker.Value = 5000u;
			}
			else
			{
				looksAlivePollInterval = looksAliveTimePicker.Value;
			}
			if (isAliveUseValueRadioButton.Checked)
			{
				isAlivePollInterval = uint.MaxValue;
				isAliveTimePicker.Value = 60000u;
			}
			else
			{
				isAlivePollInterval = isAliveTimePicker.Value;
			}
			separateMonitor = separateMonitorCheckBox.Checked;
		}
		return true;
	}

	private bool PossibleOwnerItemCheked(int index)
	{
		UIThreadHandler<bool, int> val = PossibleOwnerItemCheked;
		bool result = false;
		if (UIHelper.ExecuteOnUIThread<bool, int>(ref result, (ISynchronizeInvoke)this, (Delegate)(object)val, index))
		{
			return result;
		}
		return possibleOwners[index].Checked;
	}

	protected override bool SaveProperties()
	{
		bool success = true;
		SettingChanger possibleOwnersUpdateWaiter = new SettingChanger(initialState: true);
		try
		{
			SettingChanger propertiesUpdateWaiter = new SettingChanger(initialState: true);
			try
			{
				if (possibleOwnersDirty)
				{
					bool flag = false;
					ObservableCollection<Node> possibleOwnerNodes = new ObservableCollection<Node>();
					for (int i = 0; i < possibleOwners.Count; i++)
					{
						Node node = (Node)possibleOwners[i].Tag;
						if (PossibleOwnerItemCheked(i))
						{
							flag = true;
							possibleOwnerNodes.Add((Node)possibleOwners[i].Tag);
							if (!possibleOwnersCache.ContainsKey(node.Name))
							{
								possibleOwnersCache[node.Name] = node;
							}
						}
						else if (possibleOwnersCache.ContainsKey(node.Name))
						{
							flag = true;
							possibleOwnersCache.Remove(node.Name);
						}
					}
					if (flag)
					{
						possibleOwnersDirty = false;
						success = false;
						possibleOwnersUpdateWaiter.Reset();
						resource.RedirectAsyncOutput(delegate
						{
							resource.PossibleOwners = new ReadOnlyObservableCollection<Node>(possibleOwnerNodes);
						}, delegate(OperationResult result)
						{
							//IL_004f: Unknown result type (might be due to invalid IL or missing references)
							success = result.Error == null;
							possibleOwnersUpdateWaiter.Set();
							if (result.Error != null)
							{
								possibleOwnersDirty = true;
								ClusterLog.LogException((Exception)result.Error, "Error saving resource possible owner nodes");
								ClusterDialogException.ShowTaskDialog(result.Error, base.HWND);
							}
						});
					}
				}
				if (propertiesDirty)
				{
					ClusterPropertyCollection properties = resource.Properties;
					((ClusterPropertyUInt)properties["LooksAlivePollInterval"]).TypedValue = looksAlivePollInterval;
					((ClusterPropertyUInt)properties["IsAlivePollInterval"]).TypedValue = isAlivePollInterval;
					((ClusterPropertyUInt)properties["SeparateMonitor"]).TypedValue = (separateMonitor ? 1u : 0u);
					propertiesDirty = false;
					success = false;
					propertiesUpdateWaiter.Reset();
					properties.Commit(delegate(OperationResult result)
					{
						//IL_003a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0040: Invalid comparison between Unknown and I4
						//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
						if (result.Error is ClusterResourcePropertyStoredException)
						{
							success = true;
							propertiesUpdateWaiter.Set();
							if ((int)ClusterDialogException.ShowTaskDialog(result.Error, base.HWND) == 2)
							{
								resource.Recycle();
							}
						}
						else
						{
							success = result.Error == null;
							propertiesUpdateWaiter.Set();
							if (!success)
							{
								propertiesDirty = true;
								Exception ex = result.Error;
								if (result.Error is ClusterControlCodeException)
								{
									ex = new ClusterSavePropertiesException(resource.Name, result.Error);
								}
								ClusterLog.LogException(ex, "Error saving resource advanced properties");
								ClusterDialogException.ShowTaskDialog(ex, base.HWND);
							}
						}
					});
				}
			}
			finally
			{
				if (propertiesUpdateWaiter != null)
				{
					((IDisposable)propertiesUpdateWaiter).Dispose();
				}
			}
		}
		finally
		{
			if (possibleOwnersUpdateWaiter != null)
			{
				((IDisposable)possibleOwnersUpdateWaiter).Dispose();
			}
		}
		return success;
	}

	private void PossibleOwnersListItemChecked(object sender, ItemCheckedEventArgs e)
	{
		possibleOwnersDirty = true;
		base.IsDirty = true;
	}

	private void LooksAliveChanged(object sender, EventArgs e)
	{
		((Control)(object)looksAliveTimePicker).Enabled = looksAliveSpecifyValueRadioButton.Checked;
		propertiesDirty = true;
		base.IsDirty = true;
	}

	private void IsAliveChanged(object sender, EventArgs e)
	{
		((Control)(object)isAliveTimePicker).Enabled = isAliveSpecifyValueRadioButton.Checked;
		propertiesDirty = true;
		base.IsDirty = true;
	}

	private void SeparateMonitorChanged(object sender, EventArgs e)
	{
		propertiesDirty = true;
		base.IsDirty = true;
	}

	private static void PreferredOwnersLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		HelpProvider.ShowHelp(HelpTopics.GroupGeneralPropertyPageFwlink);
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
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ResourceAdvancedPropertyPage));
		looksAliveTimePicker = new TimePicker();
		looksAliveUseValueRadioButton = new RadioButton();
		looksAliveSpecifyValueRadioButton = new RadioButton();
		isAliveTimePicker = new TimePicker();
		isAliveUseValueRadioButton = new RadioButton();
		isAliveSpecifyValueRadioButton = new RadioButton();
		separateMonitorCheckBox = new CheckBox();
		possibleOwnersList = new BaseListView();
		SnapinGroupBox val = new SnapinGroupBox();
		SnapinGroupBox val2 = new SnapinGroupBox();
		Label label = new Label();
		Label label2 = new Label();
		LinkLabel linkLabel = new LinkLabel();
		((Control)(object)val).SuspendLayout();
		((Control)(object)val2).SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(val, "looksAliveGroupBox");
		((Control)(object)val).Controls.Add((Control)(object)looksAliveTimePicker);
		((Control)(object)val).Controls.Add(looksAliveUseValueRadioButton);
		((Control)(object)val).Controls.Add(looksAliveSpecifyValueRadioButton);
		((GroupBox)(object)val).FlatStyle = FlatStyle.System;
		((Control)(object)val).ForeColor = SystemColors.ControlText;
		((Control)(object)val).Name = "looksAliveGroupBox";
		((GroupBox)(object)val).TabStop = false;
		componentResourceManager.ApplyResources(looksAliveTimePicker, "looksAliveTimePicker");
		looksAliveTimePicker.DisplayUnits = (TimePickerUnits)1;
		((Control)(object)looksAliveTimePicker).Name = "looksAliveTimePicker";
		looksAliveTimePicker.Value = 0u;
		looksAliveTimePicker.ValueChanged += LooksAliveChanged;
		componentResourceManager.ApplyResources(looksAliveUseValueRadioButton, "looksAliveUseValueRadioButton");
		looksAliveUseValueRadioButton.ForeColor = SystemColors.ControlText;
		looksAliveUseValueRadioButton.Name = "looksAliveUseValueRadioButton";
		componentResourceManager.ApplyResources(looksAliveSpecifyValueRadioButton, "looksAliveSpecifyValueRadioButton");
		looksAliveSpecifyValueRadioButton.ForeColor = SystemColors.ControlText;
		looksAliveSpecifyValueRadioButton.Name = "looksAliveSpecifyValueRadioButton";
		looksAliveSpecifyValueRadioButton.CheckedChanged += LooksAliveChanged;
		componentResourceManager.ApplyResources(val2, "isAliveGroupBox");
		((Control)(object)val2).Controls.Add((Control)(object)isAliveTimePicker);
		((Control)(object)val2).Controls.Add(isAliveUseValueRadioButton);
		((Control)(object)val2).Controls.Add(isAliveSpecifyValueRadioButton);
		((GroupBox)(object)val2).FlatStyle = FlatStyle.System;
		((Control)(object)val2).ForeColor = SystemColors.ControlText;
		((Control)(object)val2).Name = "isAliveGroupBox";
		((GroupBox)(object)val2).TabStop = false;
		componentResourceManager.ApplyResources(isAliveTimePicker, "isAliveTimePicker");
		isAliveTimePicker.DisplayUnits = (TimePickerUnits)1;
		((Control)(object)isAliveTimePicker).Name = "isAliveTimePicker";
		isAliveTimePicker.Value = 0u;
		isAliveTimePicker.ValueChanged += IsAliveChanged;
		componentResourceManager.ApplyResources(isAliveUseValueRadioButton, "isAliveUseValueRadioButton");
		isAliveUseValueRadioButton.ForeColor = SystemColors.ControlText;
		isAliveUseValueRadioButton.Name = "isAliveUseValueRadioButton";
		componentResourceManager.ApplyResources(isAliveSpecifyValueRadioButton, "isAliveSpecifyValueRadioButton");
		isAliveSpecifyValueRadioButton.ForeColor = SystemColors.ControlText;
		isAliveSpecifyValueRadioButton.Name = "isAliveSpecifyValueRadioButton";
		isAliveSpecifyValueRadioButton.CheckedChanged += IsAliveChanged;
		componentResourceManager.ApplyResources(label, "possibleOnwersInstructionsLabel");
		label.ForeColor = SystemColors.ControlText;
		label.Name = "possibleOnwersInstructionsLabel";
		componentResourceManager.ApplyResources(label2, "separateMonitorInstructionsLabel");
		label2.ForeColor = SystemColors.ControlText;
		label2.Name = "separateMonitorInstructionsLabel";
		componentResourceManager.ApplyResources(linkLabel, "possibleOwnersLinkLabel");
		linkLabel.Name = "possibleOwnersLinkLabel";
		linkLabel.TabStop = true;
		linkLabel.LinkClicked += PreferredOwnersLinkLabelLinkClicked;
		componentResourceManager.ApplyResources(separateMonitorCheckBox, "separateMonitorCheckBox");
		separateMonitorCheckBox.ForeColor = SystemColors.ControlText;
		separateMonitorCheckBox.Name = "separateMonitorCheckBox";
		separateMonitorCheckBox.CheckedChanged += SeparateMonitorChanged;
		componentResourceManager.ApplyResources(possibleOwnersList, "possibleOwnersList");
		((ListView)(object)possibleOwnersList).CheckBoxes = true;
		possibleOwnersList.EnableAutoResizeColumns = false;
		((ListView)(object)possibleOwnersList).FullRowSelect = true;
		possibleOwnersList.HeaderStyle = ColumnHeaderStyle.None;
		((ListView)(object)possibleOwnersList).MultiSelect = true;
		((Control)(object)possibleOwnersList).Name = "possibleOwnersList";
		((ListView)(object)possibleOwnersList).UseCompatibleStateImageBehavior = false;
		((ListView)(object)possibleOwnersList).View = View.Details;
		((ListView)(object)possibleOwnersList).ItemChecked += PossibleOwnersListItemChecked;
		((Control)(object)this).AccessibleRole = AccessibleRole.PropertyPage;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add(linkLabel);
		((Control)(object)this).Controls.Add((Control)(object)possibleOwnersList);
		((Control)(object)this).Controls.Add(label2);
		((Control)(object)this).Controls.Add(label);
		((Control)(object)this).Controls.Add((Control)(object)val2);
		((Control)(object)this).Controls.Add((Control)(object)val);
		((Control)(object)this).Controls.Add(separateMonitorCheckBox);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "ResourceAdvancedPropertyPage";
		((Control)(object)this).Controls.SetChildIndex(separateMonitorCheckBox, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)val, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)val2, 0);
		((Control)(object)this).Controls.SetChildIndex(label, 0);
		((Control)(object)this).Controls.SetChildIndex(label2, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)possibleOwnersList, 0);
		((Control)(object)this).Controls.SetChildIndex(linkLabel, 0);
		((Control)(object)val).ResumeLayout(performLayout: false);
		((Control)(object)val2).ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

