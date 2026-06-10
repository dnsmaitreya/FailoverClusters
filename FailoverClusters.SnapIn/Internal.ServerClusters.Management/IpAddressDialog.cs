using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class IpAddressDialog : SnapinForm
{
	private enum IPAddressType
	{
		Static,
		Dynamic
	}

	private const ValidateIPAddressOptions ValidateIpAddressOptions = 113;

	private INotifyUser notifyUser;

	private IPAddressInfo ipAddressInfo;

	private IContainer components;

	private Button cancelButton;

	private Button okButton;

	private Label subnetLabel;

	private ComboBox subnetComboBox;

	private Label ipAddressLabel;

	private IPAddressControl ipAddressTextBox;

	private SnapinGroupBox addressGroupBox;

	private RadioButton staticRadioButton;

	private RadioButton dhcpRadioButton;

	public IPAddressInfo IPAddressInfo => ipAddressInfo;

	private NetworkInfo NetworkInfo => (NetworkInfo)subnetComboBox.SelectedItem;

	public IpAddressDialog(ICollection<NetworkInfo> networkInfos)
	{
		InitializeComponent();
		subnetComboBox.SuspendLayout();
		foreach (NetworkInfo networkInfo in networkInfos)
		{
			subnetComboBox.Items.Add(networkInfo);
		}
		subnetComboBox.ResumeLayout();
		CommonInit();
	}

	public IpAddressDialog(IPAddressInfo ipAddressInfo)
	{
		InitializeComponent();
		subnetComboBox.Enabled = false;
		subnetComboBox.Items.Add(ipAddressInfo.NetworkInfo);
		CommonInit();
		if (ipAddressInfo.UseDhcp)
		{
			dhcpRadioButton.Checked = true;
		}
		else
		{
			staticRadioButton.Checked = true;
		}
		((Control)(object)ipAddressTextBox).Text = ipAddressInfo.Address.ToString();
	}

	private void CommonInit()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		subnetComboBox.DisplayMember = "Name";
		subnetComboBox.SelectedIndex = 0;
		notifyUser = (INotifyUser)new MessageBoxNotifyUser((IWin32Window)this);
	}

	private void OkButtonClick(object sender, EventArgs e)
	{
		try
		{
			((Control)this).UseWaitCursor = true;
			if (staticRadioButton.Checked)
			{
				IPAddress ipAddress = InputValidator.ValidateIPAddress(((Control)(object)ipAddressTextBox).Text, NetworkInfo, (ValidateIPAddressOptions)113);
				ipAddressInfo = NetworkInfo.CreateStaticIPv4AddressInfo(ipAddress);
			}
			else
			{
				ipAddressInfo = NetworkInfo.CreateAutomaticIPAddress(NetworkConfigurationOptions.AllDynamic);
			}
			((Form)this).DialogResult = DialogResult.OK;
		}
		catch (ClusterInputValidationException ex)
		{
			notifyUser.ShowError((Exception)ex);
			return;
		}
		catch (Exception ex2)
		{
			notifyUser.ShowError(ex2, Resources.Error_Text);
			return;
		}
		finally
		{
			((Control)this).UseWaitCursor = false;
		}
		((Form)this).Close();
	}

	private void SetDhcpStatus(bool dhcpEnabled)
	{
		((Control)(object)ipAddressTextBox).Enabled = !dhcpEnabled;
		if (dhcpEnabled)
		{
			ipAddressInfo = NetworkInfo.CreateAutomaticIPAddress(NetworkConfigurationOptions.AllDynamic);
		}
		else
		{
			ipAddressTextBox.SetSubnet(NetworkInfo.Address, (int)NetworkInfo.PrefixLength);
		}
	}

	private void SubnetComboBoxSelectedIndexChanged(object sender, EventArgs e)
	{
		if (NetworkInfo.AddressType == AddressType.IPv6)
		{
			dhcpRadioButton.Enabled = false;
			staticRadioButton.Enabled = false;
			dhcpRadioButton.Checked = true;
			return;
		}
		staticRadioButton.Enabled = true;
		if (NetworkInfo.SupportsDhcp)
		{
			dhcpRadioButton.Enabled = true;
			dhcpRadioButton.Checked = true;
		}
		else
		{
			dhcpRadioButton.Enabled = false;
			staticRadioButton.Enabled = false;
			staticRadioButton.Checked = true;
			SetDhcpStatus(dhcpEnabled: false);
		}
		ipAddressTextBox.SetSubnet(NetworkInfo.Address, (int)NetworkInfo.PrefixLength);
		((Control)(object)ipAddressTextBox).Text = NetworkInfo.Address.ToString();
	}

	private void DhcpRadioButtonCheckedChanged(object sender, EventArgs e)
	{
		if (dhcpRadioButton.Checked)
		{
			staticRadioButton.Checked = false;
			SetDhcpStatus(dhcpEnabled: true);
		}
	}

	private void StaticRadioButtonCheckedChanged(object sender, EventArgs e)
	{
		if (staticRadioButton.Checked)
		{
			dhcpRadioButton.Checked = false;
			SetDhcpStatus(dhcpEnabled: false);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(IpAddressDialog));
		cancelButton = new Button();
		okButton = new Button();
		subnetLabel = new Label();
		subnetComboBox = new ComboBox();
		ipAddressLabel = new Label();
		ipAddressTextBox = new IPAddressControl();
		addressGroupBox = new SnapinGroupBox();
		staticRadioButton = new RadioButton();
		dhcpRadioButton = new RadioButton();
		((Control)(object)addressGroupBox).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(cancelButton, "cancelButton");
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Name = "cancelButton";
		componentResourceManager.ApplyResources(okButton, "okButton");
		okButton.Name = "okButton";
		okButton.Click += OkButtonClick;
		componentResourceManager.ApplyResources(subnetLabel, "subnetLabel");
		subnetLabel.Name = "subnetLabel";
		componentResourceManager.ApplyResources(subnetComboBox, "subnetComboBox");
		subnetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		subnetComboBox.FormattingEnabled = true;
		subnetComboBox.Name = "subnetComboBox";
		subnetComboBox.SelectedIndexChanged += SubnetComboBoxSelectedIndexChanged;
		componentResourceManager.ApplyResources(ipAddressLabel, "ipAddressLabel");
		ipAddressLabel.Name = "ipAddressLabel";
		((Control)(object)ipAddressTextBox).BackColor = SystemColors.Window;
		componentResourceManager.ApplyResources(ipAddressTextBox, "ipAddressTextBox");
		((Control)(object)ipAddressTextBox).MinimumSize = new Size(108, 20);
		((Control)(object)ipAddressTextBox).Name = "ipAddressTextBox";
		componentResourceManager.ApplyResources(addressGroupBox, "addressGroupBox");
		((Control)(object)addressGroupBox).Controls.Add(staticRadioButton);
		((Control)(object)addressGroupBox).Controls.Add(dhcpRadioButton);
		((Control)(object)addressGroupBox).Controls.Add((Control)(object)ipAddressTextBox);
		((Control)(object)addressGroupBox).Controls.Add(ipAddressLabel);
		((GroupBox)(object)addressGroupBox).FlatStyle = FlatStyle.System;
		((Control)(object)addressGroupBox).Name = "addressGroupBox";
		((GroupBox)(object)addressGroupBox).TabStop = false;
		componentResourceManager.ApplyResources(staticRadioButton, "staticRadioButton");
		staticRadioButton.Name = "staticRadioButton";
		staticRadioButton.TabStop = true;
		staticRadioButton.UseVisualStyleBackColor = true;
		staticRadioButton.CheckedChanged += StaticRadioButtonCheckedChanged;
		componentResourceManager.ApplyResources(dhcpRadioButton, "dhcpRadioButton");
		dhcpRadioButton.Name = "dhcpRadioButton";
		dhcpRadioButton.TabStop = true;
		dhcpRadioButton.UseVisualStyleBackColor = true;
		dhcpRadioButton.CheckedChanged += DhcpRadioButtonCheckedChanged;
		((Form)this).AcceptButton = okButton;
		componentResourceManager.ApplyResources(this, "$this");
		((ContainerControl)this).AutoScaleMode = AutoScaleMode.Font;
		((Form)this).CancelButton = cancelButton;
		((Control)this).Controls.Add((Control)(object)addressGroupBox);
		((Control)this).Controls.Add(subnetComboBox);
		((Control)this).Controls.Add(subnetLabel);
		((Control)this).Controls.Add(okButton);
		((Control)this).Controls.Add(cancelButton);
		((Control)this).Name = "IpAddressDialog";
		((Control)(object)addressGroupBox).ResumeLayout(performLayout: false);
		((Control)(object)addressGroupBox).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
