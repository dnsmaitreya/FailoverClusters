using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Controls;
using KDDSL.ServerClusters;
using KDDSL.ServerClusters.Controls;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.WinForms;

[DesignTimeVisible(true)]
internal class NetworkGeneralPropertyPage : SnapinPropertyPageControlBase
{
	private enum NetworkRoleTransitionResult
	{
		NowAvailableForClients,
		InvalidIPAddresses,
		Other
	}

	private FailoverClusters.Framework.NetworkRole networkRole;

	private string name;

	private FailoverClusters.Framework.NetworkState state;

	private readonly List<string> subNetworks = new List<string>();

	private FailoverClusters.Framework.NetworkRole previousRole;

	private bool networkNameDirty;

	private bool networkRoleDirty;

	private readonly Guid networkId;

	private readonly FailoverClusters.Framework.Cluster cluster;

	private Network network;

	private IContainer components;

	private Label nameLabel;

	private TextBox nameTextBox;

	private Label stateLabel;

	private Label stateValueLabel;

	private Label subnetsLabel;

	private Label networkNameLabel;

	private PictureBox networkIconPictureBox;

	private BaseListView subnetsListView;

	private ColumnHeader subnet;

	private RadioButton clusterUseRadioButton;

	private RadioButton dontUseRadioButton;

	private CheckBox clientAccessCheckBox;

	private HorizontalLine horizontalLine;

	internal NetworkGeneralPropertyPage(FailoverClusters.Framework.Cluster cluster, Guid networkId)
		: base(Resources.General_Text)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		if (networkId == Guid.Empty)
		{
			throw new ArgumentException(ExceptionResources.InvalidArgument_Text, "networkId");
		}
		this.cluster = cluster;
		this.networkId = networkId;
		InitializeComponent();
	}

	public NetworkGeneralPropertyPage()
		: base(Resources.General_Text)
	{
		InitializeComponent();
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		nameTextBox.Enabled = false;
		clusterUseRadioButton.Enabled = false;
		dontUseRadioButton.Enabled = false;
		clientAccessCheckBox.Enabled = false;
	}

	protected override object LoadProperties(object context)
	{
		Network.Get(cluster, networkId, delegate(OperationResult<Network> cacheResult)
		{
			if (cacheResult.Error != null)
			{
				throw cacheResult.Error;
			}
			network = cacheResult.Result;
			network.LoadAsync(delegate(ClusterLoadedEventArgs result)
			{
				if (result.Error != null)
				{
					throw result.Error;
				}
				networkRole = network.Role;
				previousRole = networkRole;
				name = network.Name;
				state = network.State;
				subNetworks.Clear();
				LoadSubnets();
				SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
				{
					networkNameLabel.Text = name;
					nameTextBox.Text = name;
					stateValueLabel.Text = state.Translate();
					SetNetworkRoleControls();
					foreach (string subNetwork in subNetworks)
					{
						subnetsListView.Items.Add(new ListViewItem(subNetwork));
					}
					networkNameDirty = false;
					networkRoleDirty = false;
					base.IsDirty = false;
					nameTextBox.Enabled = true;
					clusterUseRadioButton.Enabled = true;
					dontUseRadioButton.Enabled = true;
					UpdateNetworkRoleControls();
				});
			}, NetworkLoadSelection.Basic | NetworkLoadSelection.CommonProperties);
		}, OperationType.Async);
		return null;
	}

	private void LoadSubnets()
	{
		IList<string> iPv4Addresses = network.IPv4Addresses;
		IList<string> iPv6Addresses = network.IPv6Addresses;
		IList<string> iPv4PrefixLengths = network.IPv4PrefixLengths;
		IList<string> iPv6PrefixLengths = network.IPv6PrefixLengths;
		if (iPv4Addresses != null && iPv4PrefixLengths != null && iPv6Addresses != null && iPv6PrefixLengths != null)
		{
			for (int i = 0; i < iPv4Addresses.Count; i++)
			{
				subNetworks.Add(iPv4Addresses[i] + "/" + iPv4PrefixLengths[i] + " ");
			}
			for (int j = 0; j < iPv6Addresses.Count; j++)
			{
				subNetworks.Add(iPv6Addresses[j] + "/" + iPv6PrefixLengths[j] + " ");
			}
		}
	}

	protected override void InitializePage()
	{
		WinFormsHelp.SetPictureBoxImage(networkIconPictureBox, Icons.Network);
	}

	protected override bool ValidateProperties()
	{
		if (networkNameDirty)
		{
			name = InputValidator.ValidateNonemptyString(nameTextBox.Text, Resources.Name_Text);
		}
		if (networkRoleDirty)
		{
			networkRole = DetermineNetworkRole();
			DisplayNetworkRoleTransitionMessage(DetermineNetworkRoleTransitionResult());
		}
		return true;
	}

	protected override bool SaveProperties()
	{
		bool success = true;
		SettingChanger renameWaiter = new SettingChanger(initialState: true);
		try
		{
			if (networkNameDirty)
			{
				networkNameDirty = false;
				success = false;
				renameWaiter.Reset();
				network.RedirectAsyncOutput(delegate
				{
					network.Name = name;
				}, delegate(OperationResult result)
				{
					//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
					success = result.Error == null;
					renameWaiter.Set();
					if (success)
					{
						SnapinPropertyPageControlBase.UpdateControl((Control)(object)this, delegate
						{
							networkNameLabel.Text = network.Name;
						});
					}
					else
					{
						networkNameDirty = true;
						Exception ex2 = result.Error;
						if (result.Error is ClusterDefaultException)
						{
							ex2 = new ClusterObjectRenameException(network.Name, result.Error);
						}
						ClusterLog.LogException(ex2, "Error renaming cluster network");
						ClusterDialogException.ShowTaskDialog(ex2, base.HWND);
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
		SettingChanger propertiesUpdateWaiter = new SettingChanger(initialState: true);
		try
		{
			ClusterPropertyCollection properties = network.Properties;
			if (networkRoleDirty)
			{
				((ClusterPropertyUInt)properties["Role"]).TypedValue = (uint)networkRole;
				previousRole = networkRole;
				networkRoleDirty = false;
			}
			success = false;
			propertiesUpdateWaiter.Reset();
			properties.Commit(delegate(OperationResult result)
			{
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				success = result.Error == null;
				propertiesUpdateWaiter.Set();
				if (!success)
				{
					Exception ex = result.Error;
					if (result.Error is ClusterControlCodeException)
					{
						ex = new ClusterSavePropertiesException(network.Name, result.Error);
					}
					ClusterLog.LogException(ex, "Error saving network general properties");
					ClusterDialogException.ShowTaskDialog(ex, base.HWND);
				}
			});
		}
		finally
		{
			if (propertiesUpdateWaiter != null)
			{
				((IDisposable)propertiesUpdateWaiter).Dispose();
			}
		}
		base.IsDirty = false;
		return true;
	}

	protected override void CompleteSaveProperties()
	{
	}

	private void NetworkNameChanged(object sender, EventArgs e)
	{
		base.IsDirty = (networkNameDirty = true);
	}

	private void ClusterUseRadioButtonCheckedChanged(object sender, EventArgs e)
	{
		MarkNetworkRoleAsDirty();
		UpdateNetworkRoleControls();
	}

	private void ClientAccessCheckBoxCheckedChanged(object sender, EventArgs e)
	{
		MarkNetworkRoleAsDirty();
	}

	private void DontUseRadioButtonCheckedChanged(object sender, EventArgs e)
	{
		MarkNetworkRoleAsDirty();
		UpdateNetworkRoleControls();
	}

	private void MarkNetworkRoleAsDirty()
	{
		base.IsDirty = (networkRoleDirty = true);
	}

	private void UpdateNetworkRoleControls()
	{
		clientAccessCheckBox.Enabled = !dontUseRadioButton.Checked;
	}

	private FailoverClusters.Framework.NetworkRole DetermineNetworkRole()
	{
		if (dontUseRadioButton.Checked)
		{
			return FailoverClusters.Framework.NetworkRole.None;
		}
		if (clientAccessCheckBox.Checked)
		{
			return FailoverClusters.Framework.NetworkRole.InternalAndClient;
		}
		return FailoverClusters.Framework.NetworkRole.InternalUse;
	}

	private void SetNetworkRoleControls()
	{
		switch (networkRole)
		{
		case FailoverClusters.Framework.NetworkRole.None:
			dontUseRadioButton.Checked = true;
			clusterUseRadioButton.Checked = false;
			clientAccessCheckBox.Checked = true;
			break;
		case FailoverClusters.Framework.NetworkRole.InternalUse:
			dontUseRadioButton.Checked = false;
			clusterUseRadioButton.Checked = true;
			clientAccessCheckBox.Checked = false;
			break;
		case FailoverClusters.Framework.NetworkRole.InternalAndClient:
			dontUseRadioButton.Checked = false;
			clusterUseRadioButton.Checked = true;
			clientAccessCheckBox.Checked = true;
			break;
		default:
			ClusterLog.LogWarning("Unsupported network role: " + networkRole);
			break;
		}
		UpdateNetworkRoleControls();
	}

	private NetworkRoleTransitionResult DetermineNetworkRoleTransitionResult()
	{
		bool clientAccessForRole = GetClientAccessForRole(networkRole);
		bool clientAccessForRole2 = GetClientAccessForRole(previousRole);
		if (clientAccessForRole2 && !clientAccessForRole)
		{
			return NetworkRoleTransitionResult.InvalidIPAddresses;
		}
		if (clientAccessForRole && !clientAccessForRole2)
		{
			return NetworkRoleTransitionResult.NowAvailableForClients;
		}
		return NetworkRoleTransitionResult.Other;
	}

	private bool GetClientAccessForRole(FailoverClusters.Framework.NetworkRole role)
	{
		if (role == FailoverClusters.Framework.NetworkRole.InternalAndClient || role == FailoverClusters.Framework.NetworkRole.ClientAccess)
		{
			return true;
		}
		return false;
	}

	private void DisplayNetworkRoleTransitionMessage(NetworkRoleTransitionResult result)
	{
		string text = null;
		switch (result)
		{
		case NetworkRoleTransitionResult.InvalidIPAddresses:
			text = Resources.Network_InvalidIPAddresses_Text;
			break;
		case NetworkRoleTransitionResult.NowAvailableForClients:
			text = Resources.Network_NowAvailableForClients_Text;
			break;
		default:
			ClusterLog.LogWarning("Unsupported network transition: " + result);
			break;
		case NetworkRoleTransitionResult.Other:
			break;
		}
		if (text != null)
		{
			base.NotifyUser.ShowInformational(text);
		}
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
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NetworkGeneralPropertyPage));
		nameLabel = new Label();
		nameTextBox = new TextBox();
		stateLabel = new Label();
		stateValueLabel = new Label();
		subnetsLabel = new Label();
		networkNameLabel = new Label();
		networkIconPictureBox = new PictureBox();
		subnetsListView = new BaseListView();
		subnet = new ColumnHeader();
		clusterUseRadioButton = new RadioButton();
		dontUseRadioButton = new RadioButton();
		clientAccessCheckBox = new CheckBox();
		horizontalLine = new HorizontalLine();
		((ISupportInitialize)networkIconPictureBox).BeginInit();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(nameLabel, "nameLabel");
		nameLabel.ForeColor = SystemColors.ControlText;
		nameLabel.Name = "nameLabel";
		componentResourceManager.ApplyResources(nameTextBox, "nameTextBox");
		nameTextBox.Name = "nameTextBox";
		nameTextBox.TextChanged += NetworkNameChanged;
		stateLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(stateLabel, "stateLabel");
		stateLabel.Name = "stateLabel";
		componentResourceManager.ApplyResources(stateValueLabel, "stateValueLabel");
		stateValueLabel.ForeColor = SystemColors.ControlText;
		stateValueLabel.Name = "stateValueLabel";
		subnetsLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(subnetsLabel, "subnetsLabel");
		subnetsLabel.Name = "subnetsLabel";
		componentResourceManager.ApplyResources(networkNameLabel, "networkNameLabel");
		networkNameLabel.ForeColor = SystemColors.ControlText;
		networkNameLabel.Name = "networkNameLabel";
		componentResourceManager.ApplyResources(networkIconPictureBox, "networkIconPictureBox");
		networkIconPictureBox.Name = "networkIconPictureBox";
		networkIconPictureBox.TabStop = false;
		componentResourceManager.ApplyResources(subnetsListView, "subnetsListView");
		((ListView)(object)subnetsListView).Columns.AddRange(new ColumnHeader[1] { subnet });
		subnetsListView.EnableAutoResizeColumns = true;
		subnetsListView.HeaderStyle = ColumnHeaderStyle.None;
		((Control)(object)subnetsListView).Name = "subnetsListView";
		((ListView)(object)subnetsListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)subnetsListView).View = View.Details;
		componentResourceManager.ApplyResources(subnet, "subnet");
		componentResourceManager.ApplyResources(clusterUseRadioButton, "clusterUseRadioButton");
		clusterUseRadioButton.BackColor = SystemColors.Control;
		clusterUseRadioButton.ForeColor = SystemColors.ControlText;
		clusterUseRadioButton.Name = "clusterUseRadioButton";
		clusterUseRadioButton.TabStop = true;
		clusterUseRadioButton.UseVisualStyleBackColor = false;
		clusterUseRadioButton.CheckedChanged += ClusterUseRadioButtonCheckedChanged;
		componentResourceManager.ApplyResources(dontUseRadioButton, "dontUseRadioButton");
		dontUseRadioButton.BackColor = SystemColors.Control;
		dontUseRadioButton.ForeColor = SystemColors.ControlText;
		dontUseRadioButton.Name = "dontUseRadioButton";
		dontUseRadioButton.TabStop = true;
		dontUseRadioButton.UseVisualStyleBackColor = false;
		dontUseRadioButton.CheckedChanged += DontUseRadioButtonCheckedChanged;
		componentResourceManager.ApplyResources(clientAccessCheckBox, "clientAccessCheckBox");
		clientAccessCheckBox.BackColor = SystemColors.Control;
		clientAccessCheckBox.ForeColor = SystemColors.ControlText;
		clientAccessCheckBox.Name = "clientAccessCheckBox";
		clientAccessCheckBox.UseVisualStyleBackColor = false;
		clientAccessCheckBox.CheckedChanged += ClientAccessCheckBoxCheckedChanged;
		componentResourceManager.ApplyResources(horizontalLine, "horizontalLine");
		((Control)(object)horizontalLine).Name = "horizontalLine";
		componentResourceManager.ApplyResources(this, "$this");
		((SnapinUserControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine);
		((Control)(object)this).Controls.Add(clientAccessCheckBox);
		((Control)(object)this).Controls.Add(dontUseRadioButton);
		((Control)(object)this).Controls.Add(clusterUseRadioButton);
		((Control)(object)this).Controls.Add((Control)(object)subnetsListView);
		((Control)(object)this).Controls.Add(networkNameLabel);
		((Control)(object)this).Controls.Add(networkIconPictureBox);
		((Control)(object)this).Controls.Add(subnetsLabel);
		((Control)(object)this).Controls.Add(stateValueLabel);
		((Control)(object)this).Controls.Add(stateLabel);
		((Control)(object)this).Controls.Add(nameLabel);
		((Control)(object)this).Controls.Add(nameTextBox);
		((Control)(object)this).ForeColor = SystemColors.Control;
		((Control)(object)this).Name = "NetworkGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(nameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(nameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(stateLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(stateValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(subnetsLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(networkIconPictureBox, 0);
		((Control)(object)this).Controls.SetChildIndex(networkNameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)subnetsListView, 0);
		((Control)(object)this).Controls.SetChildIndex(clusterUseRadioButton, 0);
		((Control)(object)this).Controls.SetChildIndex(dontUseRadioButton, 0);
		((Control)(object)this).Controls.SetChildIndex(clientAccessCheckBox, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine, 0);
		((ISupportInitialize)networkIconPictureBox).EndInit();
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}

