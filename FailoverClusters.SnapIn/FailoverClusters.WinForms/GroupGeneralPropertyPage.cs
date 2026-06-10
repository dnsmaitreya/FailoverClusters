using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Controls;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.WinForms;

[DesignTimeVisible(true)]
internal class GroupGeneralPropertyPage : SnapinPropertyPageControlBase
{
	private const int QuorumPriority = 13000;

	private const int StoragePoolPriority = 11100;

	private const int CorePrioity = 11000;

	private readonly List<ListViewItem> preferredOwners = new List<ListViewItem>();

	private string name;

	private string state;

	private string owner;

	private FailoverClusters.Framework.GroupType groupType;

	private readonly Guid groupId;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private Group group;

	private bool nameDirty;

	private bool preferredOwnersDirty;

	private int persistentModeValue = -1;

	private Priority priorityValue;

	private IContainer components;

	private TextBox nameTextBox;

	private Label groupNameLabel;

	private PictureBox groupIconPictureBox;

	private Label nodeValueLabel;

	private Label stateValueLabel;

	private LinkLabel ownersInstructionsLabel;

	private HorizontalLine horizontalLine;

	private ComboBox priorityComboBox;

	private ToolTip priorityToolTip;

	private ToolTip persitentModeToolTip;

	private GroupBox groupBox1;

	private OrderedListView orderedListViewPreferredOwners;

	public GroupGeneralPropertyPage()
	{
		InitializeComponent();
	}

	internal GroupGeneralPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid groupId)
		: base(Resources.General_Text)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		if (groupId == Guid.Empty)
		{
			throw new ArgumentException(ExceptionResources.InvalidArgument_Text, "groupId");
		}
		this.cluster = cluster;
		this.groupId = groupId;
		InitializeComponent();
		foreach (Priority filterableValue in Priority.High.GetFilterableValues())
		{
			priorityComboBox.Items.Add(new PriorityWrapper(filterableValue));
		}
		group = null;
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		groupNameLabel.Text = CommonResources.LoadingText;
		nodeValueLabel.Text = CommonResources.LoadingText;
		stateValueLabel.Text = CommonResources.LoadingText;
		priorityToolTip.ToolTipTitle = CommonResources.PriorityText;
		persitentModeToolTip.ToolTipTitle = Resources.PersistentMode;
	}

	protected override object LoadProperties(object context)
	{
		Group.Get(cluster, groupId, delegate(OperationResult<Group> cacheResult)
		{
			if (cacheResult.Error != null)
			{
				throw cacheResult.Error;
			}
			group = cacheResult.Result;
			group.LoadAsync(delegate(ClusterLoadedEventArgs result)
			{
				if (result.Error != null)
				{
					throw result.Error;
				}
				group.Cluster.Nodes.ExecuteQuery(NodesQuery);
			});
		}, OperationType.Async);
		return null;
	}

	private void NodesQuery(OperationResult<IClusterList<Node>> nodeResult)
	{
		IClusterList<Node> nodes = nodeResult.Result;
		SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
		{
			name = group.Name;
			groupType = group.GroupType;
			state = group.ApplicationStatus.Translate();
			owner = group.OwnerNode.Name;
			ListDictionary listDictionary = new ListDictionary();
			preferredOwners.Clear();
			if (group.PreferredOwners != null)
			{
				foreach (Node preferredOwner in group.PreferredOwners)
				{
					listDictionary[preferredOwner.Name] = preferredOwner;
					preferredOwners.Add(CreatePreferredOwnersListItem(preferredOwner, isChecked: true));
				}
				if (nodes != null)
				{
					foreach (Node item in nodes.OrderBy((Node n) => n.Name, NativeMethods.StrCmpLogicalW))
					{
						if (!listDictionary.Contains(item.Name))
						{
							preferredOwners.Add(CreatePreferredOwnersListItem(item, isChecked: false));
						}
					}
				}
			}
			ClusterProperty clusterProperty = group.Properties["DefaultOwner"];
			persistentModeValue = (int)(uint)clusterProperty.Value;
			priorityValue = group.Priority;
			LoadControls();
		});
	}

	protected override void InitializePage()
	{
		WinFormsHelp.SetPictureBoxImage(groupIconPictureBox, InvariantResources.Group);
		name = CommonResources.LoadingText;
		state = CommonResources.LoadingText;
		owner = CommonResources.LoadingText;
		base.IsDirty = false;
	}

	private void LoadControls()
	{
		groupNameLabel.Text = name;
		nameTextBox.Text = name;
		stateValueLabel.Text = state;
		nodeValueLabel.Text = owner;
		orderedListViewPreferredOwners.Items.AddRange(preferredOwners.ToArray());
		WinFormsHelp.SetEncodedLinkLabelText(ownersInstructionsLabel, (groupType == FailoverClusters.Framework.GroupType.CoreCluster) ? Resources.CoreClusterGroupOwnersInstructions_Text : Resources.ServiceOrApplicationOwnersInstructions_Text);
		if (IsPriorityValueInEnumRange(priorityValue))
		{
			priorityComboBox.SelectedIndex = priorityComboBox.FindString(priorityValue.Translate());
		}
		else
		{
			string text;
			switch ((int)priorityValue)
			{
			case 13000:
				text = EnumResources.Priority_Quorum;
				priorityComboBox.Enabled = false;
				break;
			case 11100:
				text = EnumResources.Priority_Quorum;
				priorityComboBox.Enabled = false;
				break;
			case 11000:
				text = EnumResources.Priority_Core;
				priorityComboBox.Enabled = false;
				break;
			default:
				text = priorityValue.Translate();
				break;
			}
			int selectedIndex = priorityComboBox.Items.Add(new PriorityWrapper(text));
			priorityComboBox.SelectedIndex = selectedIndex;
		}
		if (group.GroupType == FailoverClusters.Framework.GroupType.CoreCluster)
		{
			priorityComboBox.Enabled = false;
		}
		priorityToolTip.SetToolTip(priorityComboBox, CommonResources.PriorityToolTip_Text);
		nameDirty = false;
		preferredOwnersDirty = false;
		base.IsDirty = false;
	}

	private static bool IsPriorityValueInEnumRange(Priority priority)
	{
		switch (priority)
		{
		case Priority.NoAutoStart:
		case Priority.Low:
		case Priority.Medium:
		case Priority.High:
			return true;
		default:
			return false;
		}
	}

	private static ListViewItem CreatePreferredOwnersListItem(Node node, bool isChecked)
	{
		return new ListViewItem
		{
			Tag = node,
			Text = node.Name,
			Checked = isChecked
		};
	}

	protected override bool ValidateProperties()
	{
		if (nameDirty)
		{
			name = InputValidator.ValidateNonemptyString(nameTextBox.Text, "Name_Text");
		}
		if (preferredOwnersDirty)
		{
			preferredOwners.Clear();
			foreach (ListViewItem checkedItem in orderedListViewPreferredOwners.CheckedItems)
			{
				preferredOwners.Add(checkedItem);
			}
		}
		return true;
	}

	protected override bool SaveProperties()
	{
		bool success = true;
		SettingChanger nameUpdateWaiter = new SettingChanger(initialState: true);
		try
		{
			SettingChanger preferredOwnersUpdateWaiter = new SettingChanger(initialState: true);
			try
			{
				SettingChanger propertiesUpdateWaiter = new SettingChanger(initialState: true);
				try
				{
					if (nameDirty)
					{
						nameDirty = false;
						success = false;
						nameUpdateWaiter.Reset();
						group.RedirectAsyncOutput(delegate
						{
							group.Name = name;
							nameDirty = false;
						}, delegate(OperationResult result)
						{
							//IL_007d: Unknown result type (might be due to invalid IL or missing references)
							success = result.Error == null;
							nameUpdateWaiter.Set();
							if (!success)
							{
								Exception ex2 = result.Error;
								if (result.Error is ClusterDefaultException)
								{
									ex2 = new ClusterObjectRenameException(group.Name, result.Error);
								}
								ClusterLog.LogException(ex2, "Error renaming group");
								ClusterDialogException.ShowTaskDialog(ex2, base.HWND);
								nameDirty = true;
							}
							else
							{
								SnapinPropertyPageControlBase.BeginUpdateControl((Control)(object)this, delegate
								{
									groupNameLabel.Text = name;
								});
							}
						});
					}
					if (preferredOwnersDirty)
					{
						ObservableCollection<Node> preferredOwnerNodes = new ObservableCollection<Node>();
						foreach (ListViewItem preferredOwner in preferredOwners)
						{
							preferredOwnerNodes.Add((Node)preferredOwner.Tag);
						}
						preferredOwnersDirty = false;
						success = false;
						preferredOwnersUpdateWaiter.Reset();
						group.RedirectAsyncOutput(delegate
						{
							group.PreferredOwners = new ReadOnlyObservableCollection<Node>(preferredOwnerNodes);
						}, delegate(OperationResult result)
						{
							//IL_0061: Unknown result type (might be due to invalid IL or missing references)
							success = result.Error == null;
							preferredOwnersUpdateWaiter.Set();
							if (!success)
							{
								ClusterLog.LogException((Exception)result.Error, "Error setting group preferred owners");
								ClusterDialogException.ShowTaskDialog(result.Error, base.HWND);
								preferredOwnersDirty = true;
							}
						});
					}
					if (priorityValue != ((PriorityWrapper)priorityComboBox.SelectedItem).Priority)
					{
						bool flag = false;
						ClusterPropertyCollection properties = group.Properties;
						if (priorityComboBox.Enabled && priorityComboBox.SelectedItem != null)
						{
							Priority priority = ((PriorityWrapper)priorityComboBox.SelectedItem).Priority;
							if (IsPriorityValueInEnumRange(priority) && priorityValue != priority)
							{
								((ClusterPropertyUInt)properties["Priority"]).TypedValue = (uint)priority;
								priorityValue = ((PriorityWrapper)priorityComboBox.SelectedItem).Priority;
								flag = true;
							}
						}
						if (flag)
						{
							success = false;
							propertiesUpdateWaiter.Reset();
							properties.Commit(delegate(OperationResult result)
							{
								//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
								success = result.Error == null;
								propertiesUpdateWaiter.Set();
								if (!success)
								{
									properties.Rollback();
									persistentModeValue = (int)((ClusterPropertyUInt)properties["DefaultOwner"]).TypedValue;
									priorityValue = (Priority)((ClusterPropertyUInt)properties["Priority"]).TypedValue;
									SnapinPropertyPageControlBase.BeginUpdateControl((Control)(object)this, delegate
									{
										priorityComboBox.SelectedIndex = priorityComboBox.FindString(priorityValue.Translate());
									});
									Exception ex = result.Error;
									if (result.Error is ClusterControlCodeException || result.Error is ClusterPropertyListBufferException)
									{
										ex = new ClusterSavePropertiesException(group.Name, result.Error);
									}
									ClusterLog.LogException(ex, "Error setting group properties");
									ClusterDialogException.ShowTaskDialog(ex, base.HWND);
								}
							});
						}
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
				if (preferredOwnersUpdateWaiter != null)
				{
					((IDisposable)preferredOwnersUpdateWaiter).Dispose();
				}
			}
		}
		finally
		{
			if (nameUpdateWaiter != null)
			{
				((IDisposable)nameUpdateWaiter).Dispose();
			}
		}
		return success;
	}

	protected override void CompleteSaveProperties()
	{
	}

	private void NameChanged(object sender, EventArgs e)
	{
		base.IsDirty = (nameDirty = true);
	}

	private void OrderedListViewPreferredOwnersItemChecked(object sender, ItemCheckedEventArgs e)
	{
		e.Item.Selected = e.Item.Checked;
		base.IsDirty = (preferredOwnersDirty = true);
	}

	private void OrderedListViewPreferredOwnersItemOrderChanged(object sender, EventArgs e)
	{
		if (((IEnumerable)orderedListViewPreferredOwners.Items).Cast<ListViewItem>().Any((ListViewItem itm) => itm.Checked))
		{
			preferredOwnersDirty = true;
			base.IsDirty = true;
		}
	}

	private void OwnersInstructionsLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		HelpProvider.ShowHelp(HelpTopics.GroupGeneralPropertyPageFwlink);
	}

	private void PersistentModeCheckBoxCheckedChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
	}

	private void PriorityComboBoxSelectedIndexChanged(object sender, EventArgs e)
	{
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(GroupGeneralPropertyPage));
		nameTextBox = new TextBox();
		groupNameLabel = new Label();
		groupIconPictureBox = new PictureBox();
		nodeValueLabel = new Label();
		stateValueLabel = new Label();
		ownersInstructionsLabel = new LinkLabel();
		horizontalLine = new HorizontalLine();
		priorityComboBox = new ComboBox();
		priorityToolTip = new ToolTip(components);
		persitentModeToolTip = new ToolTip(components);
		groupBox1 = new GroupBox();
		orderedListViewPreferredOwners = new OrderedListView();
		Label label = new Label();
		Label label2 = new Label();
		Label label3 = new Label();
		Label label4 = new Label();
		((ISupportInitialize)groupIconPictureBox).BeginInit();
		groupBox1.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(label, "nameLabel");
		label.ForeColor = SystemColors.ControlText;
		label.Name = "nameLabel";
		componentResourceManager.ApplyResources(label2, "stateLabel");
		label2.ForeColor = SystemColors.ControlText;
		label2.Name = "stateLabel";
		componentResourceManager.ApplyResources(label3, "nodeLabel");
		label3.ForeColor = SystemColors.ControlText;
		label3.Name = "nodeLabel";
		componentResourceManager.ApplyResources(label4, "ownersLabel");
		label4.ForeColor = SystemColors.ControlText;
		label4.Name = "ownersLabel";
		componentResourceManager.ApplyResources(nameTextBox, "nameTextBox");
		nameTextBox.BackColor = SystemColors.Window;
		nameTextBox.Name = "nameTextBox";
		nameTextBox.TextChanged += NameChanged;
		componentResourceManager.ApplyResources(groupNameLabel, "groupNameLabel");
		groupNameLabel.ForeColor = SystemColors.ControlText;
		groupNameLabel.Name = "groupNameLabel";
		componentResourceManager.ApplyResources(groupIconPictureBox, "groupIconPictureBox");
		groupIconPictureBox.Name = "groupIconPictureBox";
		groupIconPictureBox.TabStop = false;
		componentResourceManager.ApplyResources(nodeValueLabel, "nodeValueLabel");
		nodeValueLabel.ForeColor = SystemColors.ControlText;
		nodeValueLabel.Name = "nodeValueLabel";
		componentResourceManager.ApplyResources(stateValueLabel, "stateValueLabel");
		stateValueLabel.ForeColor = SystemColors.ControlText;
		stateValueLabel.Name = "stateValueLabel";
		componentResourceManager.ApplyResources(ownersInstructionsLabel, "ownersInstructionsLabel");
		ownersInstructionsLabel.ForeColor = SystemColors.ControlText;
		ownersInstructionsLabel.Name = "ownersInstructionsLabel";
		ownersInstructionsLabel.TabStop = true;
		ownersInstructionsLabel.LinkClicked += OwnersInstructionsLabelLinkClicked;
		componentResourceManager.ApplyResources(horizontalLine, "horizontalLine");
		((Control)(object)horizontalLine).Name = "horizontalLine";
		((Control)(object)horizontalLine).TabStop = false;
		priorityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		priorityComboBox.FormattingEnabled = true;
		componentResourceManager.ApplyResources(priorityComboBox, "priorityComboBox");
		priorityComboBox.Name = "priorityComboBox";
		priorityComboBox.SelectedIndexChanged += PriorityComboBoxSelectedIndexChanged;
		priorityToolTip.AutomaticDelay = 750;
		priorityToolTip.AutoPopDelay = 15000;
		priorityToolTip.InitialDelay = 750;
		priorityToolTip.ReshowDelay = 150;
		persitentModeToolTip.AutomaticDelay = 750;
		persitentModeToolTip.AutoPopDelay = 15000;
		persitentModeToolTip.InitialDelay = 750;
		persitentModeToolTip.ReshowDelay = 150;
		groupBox1.Controls.Add((Control)(object)orderedListViewPreferredOwners);
		groupBox1.Controls.Add(ownersInstructionsLabel);
		componentResourceManager.ApplyResources(groupBox1, "groupBox1");
		groupBox1.Name = "groupBox1";
		groupBox1.TabStop = false;
		((Control)(object)orderedListViewPreferredOwners).BackColor = SystemColors.Control;
		orderedListViewPreferredOwners.ImageList = null;
		componentResourceManager.ApplyResources(orderedListViewPreferredOwners, "orderedListViewPreferredOwners");
		((Control)(object)orderedListViewPreferredOwners).Name = "orderedListViewPreferredOwners";
		orderedListViewPreferredOwners.ItemOrderChanged += OrderedListViewPreferredOwnersItemOrderChanged;
		orderedListViewPreferredOwners.ItemChecked += OrderedListViewPreferredOwnersItemChecked;
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine);
		((Control)(object)this).Controls.Add(stateValueLabel);
		((Control)(object)this).Controls.Add(groupNameLabel);
		((Control)(object)this).Controls.Add(nodeValueLabel);
		((Control)(object)this).Controls.Add(groupIconPictureBox);
		((Control)(object)this).Controls.Add(priorityComboBox);
		((Control)(object)this).Controls.Add(label3);
		((Control)(object)this).Controls.Add(label2);
		((Control)(object)this).Controls.Add(nameTextBox);
		((Control)(object)this).Controls.Add(label);
		((Control)(object)this).Controls.Add(groupBox1);
		((Control)(object)this).Controls.Add(label4);
		((Control)(object)this).Name = "GroupGeneralPropertyPage";
		((Control)(object)this).Controls.SetChildIndex(label4, 0);
		((Control)(object)this).Controls.SetChildIndex(groupBox1, 0);
		((Control)(object)this).Controls.SetChildIndex(label, 0);
		((Control)(object)this).Controls.SetChildIndex(nameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(label2, 0);
		((Control)(object)this).Controls.SetChildIndex(label3, 0);
		((Control)(object)this).Controls.SetChildIndex(priorityComboBox, 0);
		((Control)(object)this).Controls.SetChildIndex(groupIconPictureBox, 0);
		((Control)(object)this).Controls.SetChildIndex(nodeValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(groupNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(stateValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine, 0);
		((ISupportInitialize)groupIconPictureBox).EndInit();
		groupBox1.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

