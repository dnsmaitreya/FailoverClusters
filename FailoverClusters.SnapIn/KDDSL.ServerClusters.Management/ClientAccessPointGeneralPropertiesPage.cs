using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KDDSL.ServerClusters.Controls;

namespace KDDSL.ServerClusters.Management;

internal class ClientAccessPointGeneralPropertiesPage : NetworkNamePropertyPageBase
{
	private string netBiosStatus;

	private bool oldPublishPtrVale;

	private IContainer components;

	private Label nameLabel;

	private TextBox nameTextBox;

	private ColumnHeader subnet;

	private ColumnHeader address;

	private Label ipAddressesLabel;

	private NetworkNameControl ipAddressListBox;

	private Label netBiosStatusLabel;

	private Label dnsStatusLabel;

	private Label kerberosStatusLabel;

	private Label netBiosStatusValueLabel;

	private Label dnsStatusValueLabel;

	private Label kerbStatusValueLabel;

	private Label fqdnLabel;

	private HorizontalLine horizontalLine1;

	private CheckBox publishPtrCheckbox;

	internal ClientAccessPointGeneralPropertiesPage(ResourceContext context)
		: base(context, rename: false)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		base.LoadProperties();
		ValidateIPAddressSetOptions val = (ValidateIPAddressSetOptions)17;
		ValidateIPAddressOptions val2 = (ValidateIPAddressOptions)32;
		ValidateNetworkNameOptions val3 = (ValidateNetworkNameOptions)8759;
		base.capHelp = new ClientAccessPointHelp(new ClientAccessPoint(base.Context.Resource), val, val2, val3, NetworkConfigurationOptions.AllDynamic);
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.Both);
		base.networkName = base.capHelp.ClientAccessPoint.NetworkName;
		netBiosStatus = FormatHelp.GetStatusString((uint)privateProperties["StatusNetBIOS"].Value);
		base.dnsStatus = FormatHelp.GetStatusString((uint)privateProperties["StatusDNS"].Value);
		base.kerberosStatus = FormatHelp.GetStatusString((uint)privateProperties["StatusKerberos"].Value);
		publishPtrCheckbox.Checked = (uint)privateProperties["PublishPTRRecords"].Value == 1;
		publishPtrCheckbox.CheckedChanged += PublishPtrCheckboxCheckedChanged;
		oldPublishPtrVale = publishPtrCheckbox.Checked;
		base.capHelp.CurrentNetworkName = base.networkName;
		base.domain = base.Context.Resource.Cluster.Domain;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		nameTextBox.Text = base.networkName;
		netBiosStatusValueLabel.Text = netBiosStatus;
		dnsStatusValueLabel.Text = base.dnsStatus;
		kerbStatusValueLabel.Text = base.kerberosStatus;
		ipAddressListBox.InitializeControl(base.capHelp.ClientAccessPoint, base.NotifyUser);
		base.capDirty = false;
	}

	protected override TextBox GetNameTextBox()
	{
		return nameTextBox;
	}

	protected override Label GetFqdnLabel()
	{
		return fqdnLabel;
	}

	private void PublishPtrCheckboxCheckedChanged(object sender, EventArgs e)
	{
		base.IsDirty = true;
	}

	private void NetworkListChanged(object sender, EventArgs e)
	{
		base.capDirty = true;
		base.IsDirty = true;
	}

	protected override string GetCurrentNetName()
	{
		return InputValidator.ValidateNonemptyString(nameTextBox.Text, Resources.Name_Text);
	}

	private void OnNameChanged(object sender, EventArgs e)
	{
		base.capDirty = true;
		base.IsDirty = true;
		UpdateFullName();
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
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ClientAccessPointGeneralPropertiesPage));
		nameLabel = new Label();
		nameTextBox = new TextBox();
		subnet = new ColumnHeader();
		address = new ColumnHeader();
		ipAddressesLabel = new Label();
		ipAddressListBox = new NetworkNameControl();
		netBiosStatusLabel = new Label();
		dnsStatusLabel = new Label();
		kerberosStatusLabel = new Label();
		netBiosStatusValueLabel = new Label();
		dnsStatusValueLabel = new Label();
		kerbStatusValueLabel = new Label();
		fqdnLabel = new Label();
		horizontalLine1 = new HorizontalLine();
		publishPtrCheckbox = new CheckBox();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(nameLabel, "nameLabel");
		nameLabel.AutoEllipsis = true;
		nameLabel.ForeColor = SystemColors.ControlText;
		nameLabel.Name = "nameLabel";
		componentResourceManager.ApplyResources(nameTextBox, "nameTextBox");
		nameTextBox.BackColor = SystemColors.Window;
		nameTextBox.Name = "nameTextBox";
		nameTextBox.TextChanged += OnNameChanged;
		componentResourceManager.ApplyResources(subnet, "subnet");
		componentResourceManager.ApplyResources(address, "address");
		componentResourceManager.ApplyResources(ipAddressesLabel, "ipAddressesLabel");
		ipAddressesLabel.AutoEllipsis = true;
		ipAddressesLabel.ForeColor = SystemColors.ControlText;
		ipAddressesLabel.Name = "ipAddressesLabel";
		componentResourceManager.ApplyResources(ipAddressListBox, "ipAddressListBox");
		((Control)(object)ipAddressListBox).Name = "ipAddressListBox";
		ipAddressListBox.IPAddressesListChanged += NetworkListChanged;
		netBiosStatusLabel.AutoEllipsis = true;
		netBiosStatusLabel.BackColor = SystemColors.Control;
		netBiosStatusLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(netBiosStatusLabel, "netBiosStatusLabel");
		netBiosStatusLabel.Name = "netBiosStatusLabel";
		dnsStatusLabel.AutoEllipsis = true;
		dnsStatusLabel.BackColor = SystemColors.Control;
		dnsStatusLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(dnsStatusLabel, "dnsStatusLabel");
		dnsStatusLabel.Name = "dnsStatusLabel";
		kerberosStatusLabel.AutoEllipsis = true;
		kerberosStatusLabel.BackColor = SystemColors.Control;
		kerberosStatusLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(kerberosStatusLabel, "kerberosStatusLabel");
		kerberosStatusLabel.Name = "kerberosStatusLabel";
		componentResourceManager.ApplyResources(netBiosStatusValueLabel, "netBiosStatusValueLabel");
		netBiosStatusValueLabel.AutoEllipsis = true;
		netBiosStatusValueLabel.BackColor = SystemColors.Control;
		netBiosStatusValueLabel.ForeColor = SystemColors.ControlText;
		netBiosStatusValueLabel.Name = "netBiosStatusValueLabel";
		componentResourceManager.ApplyResources(dnsStatusValueLabel, "dnsStatusValueLabel");
		dnsStatusValueLabel.AutoEllipsis = true;
		dnsStatusValueLabel.BackColor = SystemColors.Control;
		dnsStatusValueLabel.ForeColor = SystemColors.ControlText;
		dnsStatusValueLabel.Name = "dnsStatusValueLabel";
		componentResourceManager.ApplyResources(kerbStatusValueLabel, "kerbStatusValueLabel");
		kerbStatusValueLabel.AutoEllipsis = true;
		kerbStatusValueLabel.BackColor = SystemColors.Control;
		kerbStatusValueLabel.ForeColor = SystemColors.ControlText;
		kerbStatusValueLabel.Name = "kerbStatusValueLabel";
		fqdnLabel.AutoEllipsis = true;
		fqdnLabel.BackColor = SystemColors.Control;
		fqdnLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(fqdnLabel, "fqdnLabel");
		fqdnLabel.Name = "fqdnLabel";
		componentResourceManager.ApplyResources(horizontalLine1, "horizontalLine1");
		((Control)(object)horizontalLine1).Name = "horizontalLine1";
		componentResourceManager.ApplyResources(publishPtrCheckbox, "publishPtrCheckbox");
		publishPtrCheckbox.ForeColor = SystemColors.ControlText;
		publishPtrCheckbox.Name = "publishPtrCheckbox";
		publishPtrCheckbox.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(publishPtrCheckbox);
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine1);
		((Control)(object)this).Controls.Add(fqdnLabel);
		((Control)(object)this).Controls.Add(kerbStatusValueLabel);
		((Control)(object)this).Controls.Add(dnsStatusValueLabel);
		((Control)(object)this).Controls.Add(netBiosStatusValueLabel);
		((Control)(object)this).Controls.Add(kerberosStatusLabel);
		((Control)(object)this).Controls.Add(dnsStatusLabel);
		((Control)(object)this).Controls.Add(netBiosStatusLabel);
		((Control)(object)this).Controls.Add(ipAddressesLabel);
		((Control)(object)this).Controls.Add(nameTextBox);
		((Control)(object)this).Controls.Add(nameLabel);
		((Control)(object)this).Controls.Add((Control)(object)ipAddressListBox);
		((Control)(object)this).Name = "ClientAccessPointGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex((Control)(object)ipAddressListBox, 0);
		((Control)(object)this).Controls.SetChildIndex(nameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(nameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(ipAddressesLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(netBiosStatusLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(dnsStatusLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(kerberosStatusLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(netBiosStatusValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(dnsStatusValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(kerbStatusValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(fqdnLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine1, 0);
		((Control)(object)this).Controls.SetChildIndex(publishPtrCheckbox, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
