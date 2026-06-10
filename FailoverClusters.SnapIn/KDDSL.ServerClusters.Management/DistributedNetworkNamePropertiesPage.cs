using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KDDSL.ServerClusters.Controls;

namespace KDDSL.ServerClusters.Management;

internal class DistributedNetworkNamePropertiesPage : NetworkNamePropertyPageBase
{
	private IContainer components;

	private Label nameLabel;

	private TextBox nameTextBox;

	private Label dnsStatusLabel;

	private Label kerberosStatusLabel;

	private Label dnsStatusValueLabel;

	private Label kerbStatusValueLabel;

	private Label fqdnLabel;

	private HorizontalLine horizontalLine1;

	internal DistributedNetworkNamePropertiesPage(ResourceContext context)
		: base(context, rename: false)
	{
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		base.LoadProperties();
		ValidateNetworkNameOptions val = (ValidateNetworkNameOptions)8759;
		base.capHelp = new ClientAccessPointHelp(new ClientAccessPoint(base.Context.Resource), (ValidateIPAddressSetOptions)0, (ValidateIPAddressOptions)0, val, NetworkConfigurationOptions.AllDynamic);
		PropertyCollection privateProperties = base.Context.Resource.GetPrivateProperties(PropertyCollectionSet.Both);
		base.networkName = base.capHelp.ClientAccessPoint.NetworkName;
		base.dnsStatus = FormatHelp.GetStatusString((uint)privateProperties["StatusDNS"].Value);
		base.kerberosStatus = FormatHelp.GetStatusString((uint)privateProperties["StatusKerberos"].Value);
		base.capHelp.CurrentNetworkName = base.networkName;
		base.domain = base.Context.Resource.Cluster.Domain;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		nameTextBox.Text = base.networkName;
		dnsStatusValueLabel.Text = base.dnsStatus;
		kerbStatusValueLabel.Text = base.kerberosStatus;
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DistributedNetworkNamePropertiesPage));
		nameLabel = new Label();
		nameTextBox = new TextBox();
		dnsStatusLabel = new Label();
		kerberosStatusLabel = new Label();
		dnsStatusValueLabel = new Label();
		kerbStatusValueLabel = new Label();
		fqdnLabel = new Label();
		horizontalLine1 = new HorizontalLine();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(nameLabel, "nameLabel");
		nameLabel.AutoEllipsis = true;
		nameLabel.ForeColor = SystemColors.ControlText;
		nameLabel.Name = "nameLabel";
		componentResourceManager.ApplyResources(nameTextBox, "nameTextBox");
		nameTextBox.BackColor = SystemColors.Window;
		nameTextBox.Name = "nameTextBox";
		nameTextBox.TextChanged += OnNameChanged;
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
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add((Control)(object)horizontalLine1);
		((Control)(object)this).Controls.Add(fqdnLabel);
		((Control)(object)this).Controls.Add(kerbStatusValueLabel);
		((Control)(object)this).Controls.Add(dnsStatusValueLabel);
		((Control)(object)this).Controls.Add(kerberosStatusLabel);
		((Control)(object)this).Controls.Add(dnsStatusLabel);
		((Control)(object)this).Controls.Add(nameTextBox);
		((Control)(object)this).Controls.Add(nameLabel);
		((Control)(object)this).Name = "DistributedNetworkNamePropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(nameLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(nameTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(dnsStatusLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(kerberosStatusLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(dnsStatusValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(kerbStatusValueLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(fqdnLabel, 0);
		((Control)(object)this).Controls.SetChildIndex((Control)(object)horizontalLine1, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
