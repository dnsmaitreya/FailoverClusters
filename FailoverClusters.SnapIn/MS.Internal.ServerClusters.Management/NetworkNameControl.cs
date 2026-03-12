using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Microsoft.FailoverClusters.UI.Controls;

namespace MS.Internal.ServerClusters.Management;

internal class NetworkNameControl : SnapinUserControl
{
	private INotifyUser notifyUser;

	private ClientAccessPoint cap;

	private IContainer components;

	private Button editButton;

	private BaseListView ipAddressesListView;

	private Button removeButton;

	private Button addButton;

	private ColumnHeader networkColumnHeader;

	private ColumnHeader ipAddressColumnHeader;

	private bool IsSelectedIPAddressEditable
	{
		get
		{
			IPAddressInfo iPAddressInfoFromListViewItem = GetIPAddressInfoFromListViewItem(((ListView)(object)ipAddressesListView).SelectedItems[0]);
			if (iPAddressInfoFromListViewItem.AddressType == AddressType.IPv6 || iPAddressInfoFromListViewItem.NetworkInfo == null)
			{
				return false;
			}
			return true;
		}
	}

	public event EventHandler IPAddressesListChanged;

	public NetworkNameControl()
	{
		InitializeComponent();
	}

	public void InitializeControl(ClientAccessPoint clientAccessPoint, INotifyUser notifyUserConsole)
	{
		if (clientAccessPoint == null)
		{
			throw new ArgumentNullException("clientAccessPoint");
		}
		if (notifyUserConsole == null)
		{
			throw new ArgumentNullException("notifyUserConsole");
		}
		((ListView)(object)ipAddressesListView).SmallImageList = IconsHelp.SmallImageList;
		editButton.Enabled = false;
		removeButton.Enabled = false;
		notifyUser = notifyUserConsole;
		cap = clientAccessPoint;
		UpdateIPAddressesListView();
		UpdateAddButtonEnabledStatus();
	}

	private void UpdateIPAddressesListView()
	{
		ipAddressesListView.Items.Clear();
		foreach (IPAddressInfo address in cap.Addresses)
		{
			AddIPAddress(address);
		}
	}

	private void UpdateAddButtonEnabledStatus()
	{
		addButton.Enabled = cap.UnconfiguredNetworks.Count > 0;
	}

	private static IPAddressInfo GetIPAddressInfoFromListViewItem(ListViewItem lvi)
	{
		return (IPAddressInfo)lvi.Tag;
	}

	private void AddButtonClick(object sender, EventArgs e)
	{
		IpAddressDialog ipAddressDialog = new IpAddressDialog(cap.UnconfiguredNetworks);
		if (notifyUser.ShowDialog((Form)(object)ipAddressDialog) == DialogResult.OK)
		{
			cap.Addresses.Add(ipAddressDialog.IPAddressInfo);
			AddIPAddress(ipAddressDialog.IPAddressInfo);
		}
	}

	protected virtual void OnIPAddressesListChanged()
	{
		if (this.IPAddressesListChanged != null)
		{
			this.IPAddressesListChanged(this, EventArgs.Empty);
		}
		UpdateAddButtonEnabledStatus();
	}

	private void AddIPAddress(IPAddressInfo ipAddressInfo)
	{
		string text = ((ipAddressInfo.NetworkInfo != null) ? ipAddressInfo.NetworkInfo.Name : Resources.NoNetwork_Text);
		ListViewItem listViewItem = new ListViewItem(text);
		listViewItem.ImageIndex = Icons.IPAddressIndex;
		string text2 = Resources.NotConfigured_Text;
		if (ipAddressInfo.Address != null && ipAddressInfo.Address != IPAddress.IPv6None && ipAddressInfo.Address != IPAddress.None)
		{
			text2 = ipAddressInfo.Address.ToString();
		}
		listViewItem.SubItems.Add(text2);
		listViewItem.Tag = ipAddressInfo;
		ipAddressesListView.Items.Add(listViewItem);
		OnIPAddressesListChanged();
	}

	private void RemoveIPAddress(IPAddressInfo ipAddressInfo)
	{
		cap.Addresses.Remove(ipAddressInfo);
		foreach (ListViewItem item in ipAddressesListView.Items)
		{
			if (GetIPAddressInfoFromListViewItem(item) == ipAddressInfo)
			{
				item.Remove();
				break;
			}
		}
		OnIPAddressesListChanged();
	}

	private void UpdateIPAddress(ListViewItem selected, IPAddressInfo newIPAddressInfo)
	{
		cap.Addresses.Remove(GetIPAddressInfoFromListViewItem(selected));
		selected.SubItems[1].Text = newIPAddressInfo.Address.ToString();
		selected.Tag = newIPAddressInfo;
		cap.Addresses.Add(newIPAddressInfo);
		OnIPAddressesListChanged();
	}

	private void EditButtonClick(object sender, EventArgs e)
	{
		EditSelectedIPAddress();
	}

	private void EditSelectedIPAddress()
	{
		if (notifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, Resources.IPAddressEditConfirmation_Text) == DialogResult.Yes)
		{
			ListViewItem listViewItem = ((ListView)(object)ipAddressesListView).SelectedItems[0];
			IpAddressDialog ipAddressDialog = new IpAddressDialog(GetIPAddressInfoFromListViewItem(listViewItem));
			if (notifyUser.ShowDialog((Form)(object)ipAddressDialog) == DialogResult.OK)
			{
				UpdateIPAddress(listViewItem, ipAddressDialog.IPAddressInfo);
			}
		}
	}

	private void RemoveButtonClick(object sender, EventArgs e)
	{
		foreach (ListViewItem selectedItem in ((ListView)(object)ipAddressesListView).SelectedItems)
		{
			RemoveIPAddress(GetIPAddressInfoFromListViewItem(selectedItem));
		}
	}

	private void OnSelectedItemChanged(object sender, ListViewItemSelectionChangedEventArgs e)
	{
		removeButton.Enabled = ((ListView)(object)ipAddressesListView).SelectedItems.Count > 0;
		editButton.Enabled = removeButton.Enabled && IsSelectedIPAddressEditable;
	}

	private void OnLeave(object sender, EventArgs e)
	{
		Button button = removeButton;
		bool enabled = (editButton.Enabled = false);
		button.Enabled = enabled;
		UpdateAddButtonEnabledStatus();
		((ListView)(object)ipAddressesListView).SelectedIndices.Clear();
	}

	private void OnListviewDoubleClick(object sender, MouseEventArgs e)
	{
		if (IsSelectedIPAddressEditable)
		{
			EditSelectedIPAddress();
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NetworkNameControl));
		editButton = new Button();
		ipAddressesListView = new BaseListView();
		networkColumnHeader = new ColumnHeader();
		ipAddressColumnHeader = new ColumnHeader();
		removeButton = new Button();
		addButton = new Button();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(editButton, "editButton");
		editButton.ForeColor = SystemColors.ControlText;
		editButton.Name = "editButton";
		editButton.UseVisualStyleBackColor = true;
		editButton.Click += EditButtonClick;
		componentResourceManager.ApplyResources(ipAddressesListView, "ipAddressesListView");
		((ListView)(object)ipAddressesListView).Columns.AddRange(new ColumnHeader[2] { networkColumnHeader, ipAddressColumnHeader });
		ipAddressesListView.EnableAutoResizeColumns = true;
		((ListView)(object)ipAddressesListView).FullRowSelect = true;
		ipAddressesListView.HideSelection = false;
		((ListView)(object)ipAddressesListView).MultiSelect = false;
		((Control)(object)ipAddressesListView).Name = "ipAddressesListView";
		((ListView)(object)ipAddressesListView).UseCompatibleStateImageBehavior = false;
		((ListView)(object)ipAddressesListView).View = View.Details;
		((Control)(object)ipAddressesListView).MouseDoubleClick += OnListviewDoubleClick;
		((ListView)(object)ipAddressesListView).ItemSelectionChanged += OnSelectedItemChanged;
		networkColumnHeader.Name = "networkColumnHeader";
		componentResourceManager.ApplyResources(networkColumnHeader, "networkColumnHeader");
		componentResourceManager.ApplyResources(ipAddressColumnHeader, "ipAddressColumnHeader");
		componentResourceManager.ApplyResources(removeButton, "removeButton");
		removeButton.ForeColor = SystemColors.ControlText;
		removeButton.Name = "removeButton";
		removeButton.UseVisualStyleBackColor = true;
		removeButton.Click += RemoveButtonClick;
		componentResourceManager.ApplyResources(addButton, "addButton");
		addButton.ForeColor = SystemColors.ControlText;
		addButton.Name = "addButton";
		addButton.UseVisualStyleBackColor = true;
		addButton.Click += AddButtonClick;
		((Control)this).Controls.Add(editButton);
		((Control)this).Controls.Add((Control)(object)ipAddressesListView);
		((Control)this).Controls.Add(removeButton);
		((Control)this).Controls.Add(addButton);
		((Control)this).Name = "NetworkNameControl";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Leave += OnLeave;
		((Control)this).ResumeLayout(performLayout: false);
	}
}
