using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FailoverClusters.UI.Controls;
using KDDSL.ServerClusters.Controls;

namespace KDDSL.ServerClusters.Management;

internal class NetworkGeneralPropertiesPage : PropertyPageControlBase
{
	private enum NetworkRoleTransitionResult
	{
		NowAvailableForClients,
		InvalidIPAddresses,
		Other
	}

	private NetworkRole networkRole;

	private string name;

	private string state;

	private List<string> subNetworks = new List<string>();

	private NetworkRole previousRole;

	private bool networkNameDirty;

	private bool networkRoleDirty;

	private NetworkContext context;

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

	internal NetworkGeneralPropertiesPage(IScopeNodeContext context)
		: this()
	{
		this.context = (NetworkContext)context;
	}

	public NetworkGeneralPropertiesPage()
		: base(Resources.General_Text)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		networkRole = context.Network.Role;
		previousRole = networkRole;
		name = context.Network.Name;
		state = FormatHelp.GetNetworkStateString(context.Network.State);
		subNetworks.Clear();
		foreach (NetworkInfo item in NetworkInfo.GetNetworkInfoFromNetwork(context.Network))
		{
			subNetworks.Add(item.Name);
		}
	}

	protected override void InitializePage()
	{
		WinFormsHelp.SetPictureBoxImage(networkIconPictureBox, Icons.Network);
		networkNameLabel.Text = name;
		nameTextBox.Text = name;
		stateValueLabel.Text = state;
		SetNetworkRoleControls();
		foreach (string subNetwork in subNetworks)
		{
			subnetsListView.Items.Add(new ListViewItem(subNetwork));
		}
		networkNameDirty = false;
		networkRoleDirty = false;
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
			CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.Network_RoleValidation_Text, Resources.Network_DetermineRoleValidation_Text);
			NetworkRoleTransitionResult result;
			using (cluadminWaitDialog)
			{
				result = cluadminWaitDialog.ShowDialog<object, NetworkRoleTransitionResult>(base.NotifyUser, DetermineNetworkRoleTransitionResult, null);
				if (cluadminWaitDialog.IsCanceled)
				{
					return false;
				}
			}
			DisplayNetworkRoleTransitionMessage(result);
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			if (networkNameDirty)
			{
				context.Network.Rename(name, "NetworkGeneralPropertiesPage.SaveProperties");
				networkNameDirty = false;
			}
			if (networkRoleDirty)
			{
				PropertyCollection commonProperties = context.Network.GetCommonProperties(PropertyCollectionSet.ReadWrite);
				commonProperties["Role"].Value = (uint)networkRole;
				commonProperties.SaveChanges();
				previousRole = networkRole;
				networkRoleDirty = false;
			}
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null && firstException.NativeErrorCode == -2147019830)
			{
				throw ExceptionHelp.Build<ApplicationException>(firstException, Array.Empty<string>());
			}
			ExceptionHelp.LogException(ex, "Error saving neteork general properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.NetworkSavedFailed_Text,
				context.DisplayName
			});
		}
	}

	protected override void CompleteSaveProperties()
	{
	}

	private void NetworkNameChanged(object sender, EventArgs e)
	{
		base.IsDirty = (networkNameDirty = true);
	}

	private void clusterUseRadioButton_CheckedChanged(object sender, EventArgs e)
	{
		MarkNetworkRoleAsDirty();
		UpdateNetworkRoleControls();
	}

	private void clientAccessCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		MarkNetworkRoleAsDirty();
	}

	private void dontUseRadioButton_CheckedChanged(object sender, EventArgs e)
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
		if (dontUseRadioButton.Checked)
		{
			clientAccessCheckBox.Enabled = false;
		}
		else
		{
			clientAccessCheckBox.Enabled = true;
		}
	}

	private NetworkRole DetermineNetworkRole()
	{
		if (dontUseRadioButton.Checked)
		{
			return NetworkRole.None;
		}
		if (clientAccessCheckBox.Checked)
		{
			return NetworkRole.InternalAndClient;
		}
		return NetworkRole.InternalUse;
	}

	private void SetNetworkRoleControls()
	{
		switch (networkRole)
		{
		case NetworkRole.None:
			dontUseRadioButton.Checked = true;
			clusterUseRadioButton.Checked = false;
			clientAccessCheckBox.Checked = true;
			break;
		case NetworkRole.InternalUse:
			dontUseRadioButton.Checked = false;
			clusterUseRadioButton.Checked = true;
			clientAccessCheckBox.Checked = false;
			break;
		case NetworkRole.InternalAndClient:
			dontUseRadioButton.Checked = false;
			clusterUseRadioButton.Checked = true;
			clientAccessCheckBox.Checked = true;
			break;
		default:
			DebugLog.LogWarning("Unsupported network role: " + networkRole);
			break;
		}
		UpdateNetworkRoleControls();
	}

	private NetworkRoleTransitionResult DetermineNetworkRoleTransitionResult(CluadminWaitDialog waitDialog, object data)
	{
		bool clientAccessForRole = GetClientAccessForRole(networkRole);
		bool clientAccessForRole2 = GetClientAccessForRole(previousRole);
		if (clientAccessForRole2 && !clientAccessForRole)
		{
			if (AreIPAddressOnNetwork())
			{
				return NetworkRoleTransitionResult.InvalidIPAddresses;
			}
			return NetworkRoleTransitionResult.Other;
		}
		if (clientAccessForRole && !clientAccessForRole2)
		{
			return NetworkRoleTransitionResult.NowAvailableForClients;
		}
		return NetworkRoleTransitionResult.Other;
	}

	private bool AreIPAddressOnNetwork()
	{
		bool result = false;
		foreach (ClusterResource networkClassResource in context.Network.Cluster.GetNetworkClassResources())
		{
			string networkFromResource = GetNetworkFromResource(networkClassResource);
			if (networkFromResource != null && string.Compare(context.Network.Name, networkFromResource, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private static string GetNetworkFromResource(ClusterResource resource)
	{
		string result = null;
		if (resource.IsResourceOfType(WellKnownResourceType.IPAddress))
		{
			result = (string)resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite)["Network"].Value;
		}
		else if (resource.IsResourceOfType(WellKnownResourceType.IPv6Address))
		{
			result = (string)resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite)["Network"].Value;
		}
		return result;
	}

	private bool GetClientAccessForRole(NetworkRole role)
	{
		if (role == NetworkRole.InternalAndClient || role == NetworkRole.ClientAccess)
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
			DebugLog.LogWarning("Unsupported network transition: " + result);
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
		((SnapinUserControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NetworkGeneralPropertiesPage));
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
		clusterUseRadioButton.CheckedChanged += clusterUseRadioButton_CheckedChanged;
		componentResourceManager.ApplyResources(dontUseRadioButton, "dontUseRadioButton");
		dontUseRadioButton.BackColor = SystemColors.Control;
		dontUseRadioButton.ForeColor = SystemColors.ControlText;
		dontUseRadioButton.Name = "dontUseRadioButton";
		dontUseRadioButton.TabStop = true;
		dontUseRadioButton.UseVisualStyleBackColor = false;
		dontUseRadioButton.CheckedChanged += dontUseRadioButton_CheckedChanged;
		componentResourceManager.ApplyResources(clientAccessCheckBox, "clientAccessCheckBox");
		clientAccessCheckBox.BackColor = SystemColors.Control;
		clientAccessCheckBox.ForeColor = SystemColors.ControlText;
		clientAccessCheckBox.Name = "clientAccessCheckBox";
		clientAccessCheckBox.UseVisualStyleBackColor = false;
		clientAccessCheckBox.CheckedChanged += clientAccessCheckBox_CheckedChanged;
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

