using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class IpAddressGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private enum AddressMode
	{
		Static,
		Dhcp
	}

	private Dictionary<NetworkInfo, string> networkInfo;

	private string network;

	private string subnetMask;

	private string address;

	private bool enableDhcp;

	private DateTime leaseObtained;

	private DateTime leaseExpires;

	private ResourceState state;

	private string resourceName;

	private bool saveAddress;

	private NetworkInfo selectedNetwork;

	private ValidateIPAddressOptions validateIpAddressOptions;

	private bool propertiesDirty;

	private bool isInitializingPage;

	private IContainer components;

	private Label networkLabel;

	private Label subnetLabel;

	private Label staticIpAddressLabel;

	private TextBox subnetTextBox;

	private IPAddressControl staticIpAddressTextBox;

	private Button renewButton;

	private ComboBox networkComboBox;

	private SnapinGroupBox addressGroupBox;

	private RadioButton staticRadioButton;

	private RadioButton dhcpRadioButton;

	private TextBox leaseExpiresTextBox;

	private TextBox leaseObtainedTextBox;

	private Label dhcpIpAddressLabel;

	private Button releaseButton;

	private Label leaseObtainedLabel;

	private Label leaseExpiresLabel;

	private Panel renewReleasePanel;

	private TextBox dhcpIpAddressTextBox;

	internal IpAddressGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		InitializeComponent();
		networkInfo = new Dictionary<NetworkInfo, string>();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		FetchPrivateProperties();
		foreach (NetworkInfo item in NetworkInfo.GetPublicClusterNetworkInfo(base.Context.Resource.Cluster))
		{
			networkInfo.Add(item, item.AssociatedNetwork.Name);
		}
		resourceName = base.Context.Resource.Name;
	}

	private void FetchPrivateProperties()
	{
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.Both);
		network = (string)privateProperties["Network"].Value;
		subnetMask = (string)privateProperties["SubnetMask"].Value;
		address = (string)privateProperties["Address"].Value;
		enableDhcp = (uint)privateProperties["EnableDhcp"].Value != 0;
		state = base.Context.Resource.State;
		if (enableDhcp && IsAddressValid())
		{
			leaseObtained = (DateTime)privateProperties["LeaseObtainedTime"].Value;
			leaseExpires = (DateTime)privateProperties["LeaseExpiresTime"].Value;
			if (state != ResourceState.Online)
			{
				subnetMask = IPAddress.Any.ToString();
			}
		}
		else
		{
			leaseObtained = DateTime.MinValue;
			leaseExpires = DateTime.MinValue;
		}
	}

	protected override void InitializePage()
	{
		isInitializingPage = true;
		try
		{
			base.InitializePage();
			string strB = network;
			NetworkInfo networkInfo = null;
			foreach (KeyValuePair<NetworkInfo, string> item in this.networkInfo)
			{
				if (item.Key.AddressType == AddressType.IPv4)
				{
					networkComboBox.Items.Add(item.Key);
					if (string.Compare(item.Value, strB, StringComparison.OrdinalIgnoreCase) == 0)
					{
						networkComboBox.SelectedItem = item.Key;
						networkInfo = item.Key;
					}
				}
			}
			if (networkInfo == null)
			{
				string noNetwork_Text = Resources.NoNetwork_Text;
				networkComboBox.Items.Add(noNetwork_Text);
				networkComboBox.SelectedItem = noNetwork_Text;
			}
			UpdateAddressInfo();
		}
		finally
		{
			isInitializingPage = false;
		}
		propertiesDirty = false;
	}

	private void UpdateAddressInfo()
	{
		if (enableDhcp)
		{
			SelectMode(AddressMode.Dhcp);
			SetDhcpAddress();
			SetLeaseInfo();
		}
		else
		{
			SelectMode(AddressMode.Static);
			ClearLeaseInfo();
			((Control)(object)staticIpAddressTextBox).Text = address;
		}
		subnetTextBox.Text = subnetMask;
		((Control)(object)staticIpAddressTextBox).Enabled = !enableDhcp;
		renewReleasePanel.Visible = enableDhcp;
		dhcpIpAddressTextBox.Enabled = false;
		leaseObtainedTextBox.Enabled = false;
		leaseExpiresTextBox.Enabled = false;
		if (IsAddressValid() && (state == ResourceState.Offline || state == ResourceState.Failed))
		{
			releaseButton.Enabled = true;
		}
		else
		{
			releaseButton.Enabled = false;
		}
		renewButton.Enabled = state == ResourceState.Online;
	}

	protected override bool ValidateProperties()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!base.ValidateProperties())
		{
			return false;
		}
		if (propertiesDirty)
		{
			selectedNetwork = networkComboBox.SelectedItem as NetworkInfo;
			if (selectedNetwork == null)
			{
				throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.NoNetworkSelected_Text });
			}
			saveAddress = staticRadioButton.Checked;
			if (saveAddress)
			{
				validateIpAddressOptions = (ValidateIPAddressOptions)112;
				if (!enableDhcp && string.Compare(address, ((Control)(object)staticIpAddressTextBox).Text, StringComparison.OrdinalIgnoreCase) != 0)
				{
					validateIpAddressOptions = (ValidateIPAddressOptions)(validateIpAddressOptions | 1);
				}
			}
			enableDhcp = dhcpRadioButton.Checked;
			if (enableDhcp)
			{
				address = dhcpIpAddressTextBox.Text;
			}
			else
			{
				address = ((Control)(object)staticIpAddressTextBox).Text;
			}
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			base.SaveProperties(waitDialog);
			if (propertiesDirty)
			{
				network = selectedNetwork.AssociatedNetwork.Name;
				subnetMask = NetworkHelp.PrefixLengthToSubnetMask(selectedNetwork.PrefixLength);
				PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				privateProperties["Network"].Value = network;
				privateProperties["EnableDhcp"].Value = (enableDhcp ? 1u : 0u);
				if (!enableDhcp)
				{
					privateProperties["SubnetMask"].Value = subnetMask;
				}
				if (saveAddress)
				{
					InputValidator.ValidateIPAddress(address, selectedNetwork, validateIpAddressOptions);
					privateProperties["Address"].Value = address;
				}
				SaveProperties(privateProperties);
				propertiesDirty = false;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving IPAddress properties");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.IPAddressSavedFailed_Text,
				base.Context.DisplayName
			});
		}
	}

	protected override void CompleteSaveProperties()
	{
		base.CompleteSaveProperties();
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.LoadingProperties_Text, "");
		using (cluadminWaitDialog)
		{
			cluadminWaitDialog.ShowDialog<object, object>(base.NotifyUser, delegate
			{
				FetchPrivateProperties();
				return null;
			}, null);
			if (!cluadminWaitDialog.IsCanceled)
			{
				if (!enableDhcp)
				{
					dhcpIpAddressTextBox.Text = IPAddress.Any.ToString();
					SetLeaseInfo();
				}
				else
				{
					SetDhcpAddress();
					SetLeaseInfo();
					staticIpAddressTextBox.IPAddress = IPAddress.Any;
				}
				renewButton.Enabled = state == ResourceState.Online;
				releaseButton.Enabled = state == ResourceState.Offline;
			}
		}
	}

	private void IpAddressChanged(object sender, EventArgs e)
	{
		base.IsDirty = (propertiesDirty = true);
	}

	private void RenewButtonClick(object sender, EventArgs e)
	{
		if (state == ResourceState.Online)
		{
			CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(string.Format(CultureInfo.CurrentCulture, Resources.RenewAddressTitle_Text, resourceName), Resources.RenewAddress_Text);
			cluadminWaitDialog.AutoCancelEnabled = false;
			try
			{
				cluadminWaitDialog.DisplayDelay = new TimeSpan(0L);
				cluadminWaitDialog.ShowDialog(base.NotifyUser, RenewAddress);
				UpdateUI();
				return;
			}
			catch (Exception ex)
			{
				base.NotifyUser.ShowError(ex, Resources.CannotRenewIpAddress_Text);
				return;
			}
		}
		base.NotifyUser.ShowError(Resources.IpAddressRenewOffline_Text);
	}

	private void UpdateUI()
	{
		SetLeaseInfo();
		UpdateAddressInfo();
	}

	private void ReleaseButtonClick(object sender, EventArgs e)
	{
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(string.Format(CultureInfo.CurrentCulture, Resources.ReleaseAddressTitle_Text, resourceName), Resources.ReleaseAddress_Text);
		cluadminWaitDialog.AutoCancelEnabled = false;
		try
		{
			cluadminWaitDialog.DisplayDelay = new TimeSpan(0L);
			cluadminWaitDialog.ShowDialog(base.NotifyUser, ReleaseAddress);
			UpdateUI();
		}
		catch (Exception ex)
		{
			base.NotifyUser.ShowError(ex, Resources.CannotReleaseIpAddress_Text);
		}
	}

	private void RenewAddress(CluadminWaitDialog waitDialog)
	{
		base.Context.Resource.IPAddress_RenewLease();
		LoadProperties();
	}

	private void ReleaseAddress(CluadminWaitDialog waitDialog)
	{
		base.Context.Resource.IPAddress_ReleaseLease();
		LoadProperties();
	}

	private void networkComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		base.IsDirty = (propertiesDirty = true);
		ConfigureForNetwork();
	}

	private void ConfigureForNetwork(NetworkInfo netInfo)
	{
		if (netInfo.SupportsDhcp)
		{
			dhcpRadioButton.Enabled = true;
		}
		else
		{
			dhcpRadioButton.Enabled = false;
			if (dhcpRadioButton.Checked)
			{
				SelectMode(AddressMode.Static);
			}
		}
		if (enableDhcp)
		{
			SetDhcpAddress();
			return;
		}
		staticIpAddressTextBox.SetSubnet(netInfo.Address, (int)netInfo.PrefixLength);
		SetStaticAddress(netInfo);
	}

	private void SelectMode(AddressMode mode)
	{
		switch (mode)
		{
		case AddressMode.Dhcp:
			dhcpRadioButton.Checked = true;
			((Control)(object)staticIpAddressTextBox).Enabled = false;
			renewReleasePanel.Visible = true;
			enableDhcp = true;
			break;
		case AddressMode.Static:
			staticRadioButton.Checked = true;
			((Control)(object)staticIpAddressTextBox).Enabled = true;
			renewReleasePanel.Visible = false;
			enableDhcp = false;
			break;
		default:
			DebugLog.LogWarning("invalid address mode");
			break;
		}
	}

	private void radioButton_CheckedChanged(object sender, EventArgs e)
	{
		if (!isInitializingPage)
		{
			base.IsDirty = (propertiesDirty = true);
			if (dhcpRadioButton.Checked)
			{
				SelectMode(AddressMode.Dhcp);
			}
			else
			{
				SelectMode(AddressMode.Static);
			}
			ConfigureForNetwork();
		}
	}

	private void ConfigureForNetwork()
	{
		if (networkComboBox.SelectedItem is NetworkInfo netInfo)
		{
			ConfigureForNetwork(netInfo);
		}
	}

	private string DateTimeToString(DateTime date)
	{
		return date.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
	}

	private void ClearLeaseInfo()
	{
		SetLeaseInfo(leaseObtainedTextBox, DateTime.MinValue);
		SetLeaseInfo(leaseExpiresTextBox, DateTime.MinValue);
	}

	private bool IsAddressValid()
	{
		return string.Compare(address, IPAddress.Any.ToString(), StringComparison.OrdinalIgnoreCase) != 0;
	}

	private void SetLeaseInfo()
	{
		SetLeaseInfo(leaseObtainedTextBox, leaseObtained);
		SetLeaseInfo(leaseExpiresTextBox, leaseExpires);
	}

	private void SetDhcpAddress()
	{
		if (IsAddressValid())
		{
			dhcpIpAddressTextBox.Text = address;
		}
		else
		{
			dhcpIpAddressTextBox.Text = IPAddress.Any.ToString();
		}
		subnetTextBox.Text = IPAddress.Any.ToString();
	}

	private void SetStaticAddress(NetworkInfo netInfo)
	{
		dhcpIpAddressTextBox.Text = IPAddress.Any.ToString();
		subnetTextBox.Text = NetworkHelp.PrefixLengthToSubnetMask(netInfo.PrefixLength);
		staticIpAddressTextBox.IPAddress = netInfo.Address;
	}

	private void SetLeaseInfo(TextBox textBox, DateTime dateTime)
	{
		if (dateTime != DateTime.MinValue)
		{
			textBox.Text = DateTimeToString(dateTime);
		}
		else
		{
			textBox.Text = Resources.NotConfigured_Text;
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(IpAddressGeneralPropertiesPage));
		networkLabel = new Label();
		subnetLabel = new Label();
		staticIpAddressLabel = new Label();
		renewButton = new Button();
		subnetTextBox = new TextBox();
		staticIpAddressTextBox = new IPAddressControl();
		networkComboBox = new ComboBox();
		addressGroupBox = new SnapinGroupBox();
		dhcpIpAddressTextBox = new TextBox();
		leaseObtainedLabel = new Label();
		leaseExpiresLabel = new Label();
		dhcpIpAddressLabel = new Label();
		leaseExpiresTextBox = new TextBox();
		leaseObtainedTextBox = new TextBox();
		staticRadioButton = new RadioButton();
		dhcpRadioButton = new RadioButton();
		releaseButton = new Button();
		renewReleasePanel = new Panel();
		((Control)(object)addressGroupBox).SuspendLayout();
		renewReleasePanel.SuspendLayout();
		((Control)(object)this).SuspendLayout();
		networkLabel.AutoEllipsis = true;
		networkLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(networkLabel, "networkLabel");
		networkLabel.Name = "networkLabel";
		subnetLabel.AutoEllipsis = true;
		subnetLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(subnetLabel, "subnetLabel");
		subnetLabel.Name = "subnetLabel";
		staticIpAddressLabel.AutoEllipsis = true;
		staticIpAddressLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(staticIpAddressLabel, "staticIpAddressLabel");
		staticIpAddressLabel.Name = "staticIpAddressLabel";
		componentResourceManager.ApplyResources(renewButton, "renewButton");
		renewButton.ForeColor = SystemColors.ControlText;
		renewButton.Name = "renewButton";
		renewButton.UseVisualStyleBackColor = true;
		renewButton.Click += RenewButtonClick;
		subnetTextBox.BackColor = SystemColors.Control;
		subnetTextBox.ForeColor = SystemColors.WindowText;
		componentResourceManager.ApplyResources(subnetTextBox, "subnetTextBox");
		subnetTextBox.MinimumSize = new Size(108, 20);
		subnetTextBox.Name = "subnetTextBox";
		subnetTextBox.ReadOnly = true;
		((Control)(object)staticIpAddressTextBox).BackColor = SystemColors.Window;
		componentResourceManager.ApplyResources(staticIpAddressTextBox, "staticIpAddressTextBox");
		((Control)(object)staticIpAddressTextBox).ForeColor = SystemColors.ControlText;
		((Control)(object)staticIpAddressTextBox).MinimumSize = new Size(108, 20);
		((Control)(object)staticIpAddressTextBox).Name = "staticIpAddressTextBox";
		((Control)(object)staticIpAddressTextBox).TextChanged += IpAddressChanged;
		componentResourceManager.ApplyResources(networkComboBox, "networkComboBox");
		networkComboBox.DisplayMember = "Name";
		networkComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		networkComboBox.FormattingEnabled = true;
		networkComboBox.Name = "networkComboBox";
		networkComboBox.ValueMember = "Name";
		networkComboBox.SelectedIndexChanged += networkComboBox_SelectedIndexChanged;
		componentResourceManager.ApplyResources(addressGroupBox, "addressGroupBox");
		((Control)(object)addressGroupBox).BackColor = SystemColors.Control;
		((Control)(object)addressGroupBox).Controls.Add(dhcpIpAddressTextBox);
		((Control)(object)addressGroupBox).Controls.Add(leaseObtainedLabel);
		((Control)(object)addressGroupBox).Controls.Add(leaseExpiresLabel);
		((Control)(object)addressGroupBox).Controls.Add(dhcpIpAddressLabel);
		((Control)(object)addressGroupBox).Controls.Add(leaseExpiresTextBox);
		((Control)(object)addressGroupBox).Controls.Add(leaseObtainedTextBox);
		((Control)(object)addressGroupBox).Controls.Add(staticRadioButton);
		((Control)(object)addressGroupBox).Controls.Add(dhcpRadioButton);
		((Control)(object)addressGroupBox).Controls.Add((Control)(object)staticIpAddressTextBox);
		((Control)(object)addressGroupBox).Controls.Add(staticIpAddressLabel);
		((GroupBox)(object)addressGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)addressGroupBox).ForeColor = SystemColors.ControlText;
		((Control)(object)addressGroupBox).Name = "addressGroupBox";
		((GroupBox)(object)addressGroupBox).TabStop = false;
		componentResourceManager.ApplyResources(dhcpIpAddressTextBox, "dhcpIpAddressTextBox");
		dhcpIpAddressTextBox.Name = "dhcpIpAddressTextBox";
		leaseObtainedLabel.AutoEllipsis = true;
		leaseObtainedLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(leaseObtainedLabel, "leaseObtainedLabel");
		leaseObtainedLabel.Name = "leaseObtainedLabel";
		leaseExpiresLabel.AutoEllipsis = true;
		leaseExpiresLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(leaseExpiresLabel, "leaseExpiresLabel");
		leaseExpiresLabel.Name = "leaseExpiresLabel";
		dhcpIpAddressLabel.AutoEllipsis = true;
		dhcpIpAddressLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(dhcpIpAddressLabel, "dhcpIpAddressLabel");
		dhcpIpAddressLabel.Name = "dhcpIpAddressLabel";
		componentResourceManager.ApplyResources(leaseExpiresTextBox, "leaseExpiresTextBox");
		leaseExpiresTextBox.Name = "leaseExpiresTextBox";
		componentResourceManager.ApplyResources(leaseObtainedTextBox, "leaseObtainedTextBox");
		leaseObtainedTextBox.Name = "leaseObtainedTextBox";
		componentResourceManager.ApplyResources(staticRadioButton, "staticRadioButton");
		staticRadioButton.ForeColor = SystemColors.ControlText;
		staticRadioButton.Name = "staticRadioButton";
		staticRadioButton.TabStop = true;
		staticRadioButton.UseVisualStyleBackColor = true;
		staticRadioButton.CheckedChanged += radioButton_CheckedChanged;
		componentResourceManager.ApplyResources(dhcpRadioButton, "dhcpRadioButton");
		dhcpRadioButton.ForeColor = SystemColors.ControlText;
		dhcpRadioButton.Name = "dhcpRadioButton";
		dhcpRadioButton.TabStop = true;
		dhcpRadioButton.UseVisualStyleBackColor = true;
		dhcpRadioButton.CheckedChanged += radioButton_CheckedChanged;
		componentResourceManager.ApplyResources(releaseButton, "releaseButton");
		releaseButton.ForeColor = SystemColors.ControlText;
		releaseButton.Name = "releaseButton";
		releaseButton.UseVisualStyleBackColor = true;
		releaseButton.Click += ReleaseButtonClick;
		renewReleasePanel.Controls.Add(releaseButton);
		renewReleasePanel.Controls.Add(renewButton);
		componentResourceManager.ApplyResources(renewReleasePanel, "renewReleasePanel");
		renewReleasePanel.Name = "renewReleasePanel";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(renewReleasePanel);
		((Control)(object)this).Controls.Add(networkLabel);
		((Control)(object)this).Controls.Add(subnetLabel);
		((Control)(object)this).Controls.Add((Control)(object)addressGroupBox);
		((Control)(object)this).Controls.Add(networkComboBox);
		((Control)(object)this).Controls.Add(subnetTextBox);
		((Control)(object)this).Name = "IpAddressGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(subnetTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(networkComboBox, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)addressGroupBox, 0);
		((Control)(object)this).Controls.SetChildIndex(subnetLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(networkLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(renewReleasePanel, 0);
		((Control)(object)addressGroupBox).ResumeLayout(performLayout: false);
		((Control)(object)addressGroupBox).PerformLayout();
		renewReleasePanel.ResumeLayout(performLayout: false);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
