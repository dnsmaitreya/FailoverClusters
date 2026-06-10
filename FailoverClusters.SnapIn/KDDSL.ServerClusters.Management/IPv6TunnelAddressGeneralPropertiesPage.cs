using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KDDSL.ServerClusters.Management;

internal class IPv6TunnelAddressGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private string tunnelType;

	private string address;

	private IContainer components;

	private ClusterResource resource;

	private TextBox tunnelTypeTextBox;

	private Label tunnelTypeLabel;

	private TextBox addressTextBox;

	private Label addressLabel;

	internal IPv6TunnelAddressGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		resource = base.Context.Resource;
		InitializeComponent();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = resource.GetPrivateProperties(PropertyCollectionSet.ReadOnly);
		tunnelType = (string)privateProperties["TunnelType"].Value;
		address = (string)privateProperties["Address"].Value;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		tunnelTypeTextBox.Text = tunnelType;
		addressTextBox.Text = address;
	}

	protected override bool ValidateProperties()
	{
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(IPv6TunnelAddressGeneralPropertiesPage));
		tunnelTypeTextBox = new TextBox();
		tunnelTypeLabel = new Label();
		addressTextBox = new TextBox();
		addressLabel = new Label();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(tunnelTypeTextBox, "tunnelTypeTextBox");
		tunnelTypeTextBox.Name = "tunnelTypeTextBox";
		tunnelTypeTextBox.ReadOnly = true;
		tunnelTypeLabel.AutoEllipsis = true;
		tunnelTypeLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(tunnelTypeLabel, "tunnelTypeLabel");
		tunnelTypeLabel.Name = "tunnelTypeLabel";
		componentResourceManager.ApplyResources(addressTextBox, "addressTextBox");
		addressTextBox.Name = "addressTextBox";
		addressTextBox.ReadOnly = true;
		addressLabel.AutoEllipsis = true;
		addressLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(addressLabel, "addressLabel");
		addressLabel.Name = "addressLabel";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(addressTextBox);
		((Control)(object)this).Controls.Add(tunnelTypeTextBox);
		((Control)(object)this).Controls.Add(tunnelTypeLabel);
		((Control)(object)this).Controls.Add(addressLabel);
		((Control)(object)this).Name = "IPv6TunnelAddressGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(addressLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(tunnelTypeLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(tunnelTypeTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(addressTextBox, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}
}
