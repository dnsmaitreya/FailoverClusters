using System;
using System.Globalization;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class NetworkNamePropertyPageBase : ResourceGeneralPropertiesPage
{
	protected string newResourceName { get; set; }

	protected string networkName { get; set; }

	protected string dnsStatus { get; set; }

	protected string kerberosStatus { get; set; }

	protected string domain { get; set; }

	protected ClientAccessPointHelp capHelp { get; set; }

	protected bool capDirty { get; set; }

	protected CluadminWaitDialog waitDialog { get; set; }

	internal NetworkNamePropertyPageBase(ResourceContext context, bool rename)
		: base(context, renamable: false)
	{
	}

	internal NetworkNamePropertyPageBase()
	{
	}

	protected virtual Label GetFqdnLabel()
	{
		return null;
	}

	protected virtual TextBox GetNameTextBox()
	{
		return null;
	}

	protected virtual string GetCurrentNetName()
	{
		return string.Empty;
	}

	protected void UpdateFullName()
	{
		if (GetNameTextBox().Text.Length == 0)
		{
			GetFqdnLabel().Visible = false;
			return;
		}
		GetFqdnLabel().Visible = true;
		GetFqdnLabel().Text = string.Format(CultureInfo.CurrentCulture, Resources.FqdnName_Text, NetworkHelp.BuildFqdn(GetNameTextBox().Text, domain));
	}

	protected override void SaveProperties(CluadminWaitDialog cluaAdminWaitDialog)
	{
		try
		{
			base.SaveProperties(cluaAdminWaitDialog);
			if (capDirty)
			{
				cluaAdminWaitDialog.StatusText = Resources.SaveClientAccessPoint_Title_Text;
				waitDialog = cluaAdminWaitDialog;
				capHelp.ValidateNetName(capHelp.ClientAccessPoint.Group.Cluster, capHelp.ClientAccessPoint.Group);
				EventHandler<OperationProgressEventArgs> value = CapSaveChangesProgress;
				capHelp.ClientAccessPoint.SaveChangesProgress += value;
				try
				{
					base.SaveRequiresRecycling = capHelp.ClientAccessPoint.SaveChanges() == PropertySaveStatus.ResourceRequiresRecycle;
				}
				finally
				{
					capHelp.ClientAccessPoint.SaveChangesProgress -= value;
				}
				capDirty = false;
				newResourceName = capHelp.ClientAccessPoint.NetworkName;
				networkName = capHelp.ClientAccessPoint.NetworkName;
				capHelp.CurrentNetworkName = networkName;
			}
			else
			{
				newResourceName = null;
			}
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error saving client access point properties");
			throw ExceptionHelp.Build<ApplicationException>(ex, new string[2]
			{
				Resources.CapPropertiesSavedFailed_Text,
				base.Context.DisplayName
			});
		}
	}

	protected void CapSaveChangesProgress(object sender, OperationProgressEventArgs e)
	{
		waitDialog.StatusText = e.Message;
	}

	protected override void CompleteSaveProperties()
	{
		if (base.SaveRequiresRecycling)
		{
			base.OfflineDependencies = true;
		}
		base.CompleteSaveProperties();
		if (newResourceName != null)
		{
			SetResourceName(newResourceName);
		}
	}

	protected override bool ValidateProperties()
	{
		if (!base.ValidateProperties())
		{
			return false;
		}
		if (capDirty)
		{
			string currentNetName = GetCurrentNetName();
			string text = capHelp.ClientAccessPoint.NetworkName;
			bool flag = false;
			if (string.Compare(currentNetName, capHelp.CurrentNetworkName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				capHelp.ClientAccessPoint.NetworkName = currentNetName;
				capHelp.NetNameValidated = false;
				flag = true;
			}
			else
			{
				capHelp.NetNameValidated = true;
			}
			capHelp.AddressesValidated = false;
			bool isCoreResource = false;
			ResourceState state = ResourceState.Unknown;
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ValidatingProperties_Text, string.Empty))
			{
				cluadminWaitDialog.ShowDialog(base.NotifyUser, delegate
				{
					state = base.Context.Resource.State;
					isCoreResource = base.Context.Resource.IsCoreResource;
				});
				if (cluadminWaitDialog.IsCanceled)
				{
					return false;
				}
			}
			DialogResult dialogResult = DialogResult.Yes;
			if (isCoreResource)
			{
				dialogResult = ((!flag) ? ClientAccessPointHelp.DisplayAdminClientInterruptionWarning(state, base.NotifyUser) : ClientAccessPointHelp.DisplayAdminClientNameChangeWarning(base.NotifyUser));
			}
			else if (flag)
			{
				dialogResult = ClientAccessPointHelp.DisplayClientNameChangeWarning(base.NotifyUser);
			}
			else
			{
				CluadminWaitDialog cluadminWaitDialog2 = CluadminWaitDialog.Create(Resources.SavingProperties_Text, Resources.SavingProperties_Text);
				using (cluadminWaitDialog2)
				{
					bool flag2 = cluadminWaitDialog2.ShowDialog<object, bool>(base.NotifyUser, delegate
					{
						state = base.Context.Resource.State;
						return capHelp.ClientAccessPoint.WillSaveOfflineNetName;
					}, null);
					if (!cluadminWaitDialog2.IsCanceled && flag2)
					{
						dialogResult = ClientAccessPointHelp.DisplayClientInterruptionWarning(state, base.NotifyUser);
					}
				}
			}
			if (dialogResult != DialogResult.Yes)
			{
				capHelp.ClientAccessPoint.NetworkName = text;
				return false;
			}
			capHelp.ValidateNetBiosName(base.NotifyUser);
		}
		return true;
	}
}
