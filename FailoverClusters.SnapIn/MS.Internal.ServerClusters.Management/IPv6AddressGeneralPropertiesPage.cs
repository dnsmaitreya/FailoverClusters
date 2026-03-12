using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class IPv6AddressGeneralPropertiesPage : ResourceGeneralPropertiesPage
{
	private Dictionary<NetworkInfo, string> networkInfo;

	private string network;

	private string address;

	private uint prefixLength;

	private NetworkInfo selectedNetworkInfo;

	private bool propertiesDirty;

	private IContainer components;

	private ClusterResource resource;

	private TextBox addressTextBox;

	private Label addressLabel;

	private ComboBox networkComboBox;

	private Label networkLabel;

	internal IPv6AddressGeneralPropertiesPage(ResourceContext context)
		: base(context, renamable: true)
	{
		resource = base.Context.Resource;
		InitializeComponent();
		networkInfo = new Dictionary<NetworkInfo, string>();
	}

	protected override void LoadProperties()
	{
		base.LoadProperties();
		PropertyCollection privateProperties = resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		foreach (NetworkInfo item in NetworkInfo.GetPublicClusterNetworkInfo(resource.Cluster))
		{
			networkInfo.Add(item, item.AssociatedNetwork.Name);
		}
		network = (string)privateProperties["Network"].Value;
		address = (string)privateProperties["Address"].Value;
		prefixLength = (uint)privateProperties["PrefixLength"].Value;
	}

	protected override void InitializePage()
	{
		base.InitializePage();
		string strB = network;
		bool flag = false;
		foreach (KeyValuePair<NetworkInfo, string> item in networkInfo)
		{
			if (item.Key.AddressType == AddressType.IPv6 && !item.Key.IsTunneled)
			{
				networkComboBox.Items.Add(item.Key);
				if (string.Compare(item.Value, strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					networkComboBox.SelectedItem = item.Key;
					flag = true;
				}
			}
		}
		if (!flag)
		{
			string noNetwork_Text = Resources.NoNetwork_Text;
			networkComboBox.Items.Add(noNetwork_Text);
			networkComboBox.SelectedItem = noNetwork_Text;
		}
		addressTextBox.Text = string.Format(CultureInfo.CurrentCulture, "{0}/{1}", address, prefixLength);
		propertiesDirty = false;
	}

	protected override bool ValidateProperties()
	{
		if (!base.ValidateProperties())
		{
			return false;
		}
		if (propertiesDirty)
		{
			if (!(networkComboBox.SelectedItem is NetworkInfo networkInfo))
			{
				throw ExceptionHelp.Build<ClusterInputValidationException>(new string[1] { Resources.NoNetworkSelected_Text });
			}
			address = networkInfo.Address.ToString();
			prefixLength = networkInfo.PrefixLength;
		}
		return true;
	}

	protected override void SaveProperties(CluadminWaitDialog waitDialog)
	{
		try
		{
			base.SaveProperties(waitDialog);
			network = selectedNetworkInfo.AssociatedNetwork.Name;
			if (propertiesDirty)
			{
				PropertyCollection privateProperties = resource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
				privateProperties["Network"].Value = network;
				privateProperties["Address"].Value = address;
				privateProperties["PrefixLength"].Value = prefixLength;
				SaveProperties(privateProperties);
				propertiesDirty = false;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving IPV6 addeess properites");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.IPV6AddressSavedFailed_Text,
				base.Context.DisplayName
			});
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
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(IPv6AddressGeneralPropertiesPage));
		addressTextBox = new TextBox();
		addressLabel = new Label();
		networkLabel = new Label();
		networkComboBox = new ComboBox();
		((Control)(object)this).SuspendLayout();
		componentResourceManager.ApplyResources(addressTextBox, "addressTextBox");
		addressTextBox.Name = "addressTextBox";
		addressTextBox.ReadOnly = true;
		addressLabel.AutoEllipsis = true;
		addressLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(addressLabel, "addressLabel");
		addressLabel.Name = "addressLabel";
		networkLabel.AutoEllipsis = true;
		networkLabel.ForeColor = SystemColors.ControlText;
		componentResourceManager.ApplyResources(networkLabel, "networkLabel");
		networkLabel.Name = "networkLabel";
		componentResourceManager.ApplyResources(networkComboBox, "networkComboBox");
		networkComboBox.DisplayMember = "Name";
		networkComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		networkComboBox.FormattingEnabled = true;
		networkComboBox.Name = "networkComboBox";
		networkComboBox.ValueMember = "Name";
		networkComboBox.SelectedIndexChanged += networkComboBox_SelectedIndexChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)(object)this).Controls.Add(addressLabel);
		((Control)(object)this).Controls.Add(networkLabel);
		((Control)(object)this).Controls.Add(networkComboBox);
		((Control)(object)this).Controls.Add(addressTextBox);
		((Control)(object)this).Name = "IPv6AddressGeneralPropertiesPage";
		((Control)(object)this).Controls.SetChildIndex(addressTextBox, 0);
		((Control)(object)this).Controls.SetChildIndex(networkComboBox, 0);
		((Control)(object)this).Controls.SetChildIndex(networkLabel, 0);
		((Control)(object)this).Controls.SetChildIndex(addressLabel, 0);
		((Control)(object)this).ResumeLayout(performLayout: false);
		((Control)(object)this).PerformLayout();
	}

	private void networkComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		base.IsDirty = (propertiesDirty = true);
		NetworkInfo networkInfo = networkComboBox.SelectedItem as NetworkInfo;
		if (networkInfo != null)
		{
			addressTextBox.Text = string.Format(CultureInfo.CurrentCulture, "{0}/{1}", networkInfo.Address, networkInfo.PrefixLength);
		}
		selectedNetworkInfo = networkInfo;
	}
}
