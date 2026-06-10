using System;
using System.Windows.Forms;
using FailoverClusters.Configuration;
using FailoverClusters.UI.Common;
using Virtualization.Client.Common;

namespace FailoverClusters.Framework;

public class VMReplicaBrokerSettingsCommand : WeakLazyBase<ClusterCommand>
{
	private ISettingsDialog settingDialog;

	public ClusterObject ClusterObj { get; private set; }

	public VMReplicaBrokerSettingsCommand(ClusterObject clusterObject)
	{
		ClusterObj = clusterObject;
	}

	protected override ClusterCommand GenerateInstance()
	{
		return new ClusterCommand(ClusterObj, "VMReplicaSettings", ClusterCommandId.VMReplicaBrokerResourceSettings, ClusterCommandCollectionId.VirtualMachineReplicaBrokerResource)
		{
			Text = EnumResources.VMReplicaBroker_Set_Settings,
			CanExecuteDelegate = (object x) => true,
			Visible = true,
			ExecuteDelegate = delegate
			{
				ReplicationSettings();
			}
		};
	}

	private void ReplicationSettings()
	{
		//IL_00e7: Expected O, but got Unknown
		if (!new WindowsFeature().IsVirtualMachineClientToolsInstalled())
		{
			ClusterLog.AdminEvents.WriteHyperVToolsNotInstalledEvent();
			return;
		}
		if (settingDialog != null)
		{
			((IForm)settingDialog).Activate();
			return;
		}
		string empty = string.Empty;
		if (ClusterObj is Resource)
		{
			empty = ((Resource)ClusterObj).OwnerGroup.OwnerNode.Name;
		}
		else
		{
			if (!(ClusterObj is Group))
			{
				ClusterLog.LogError("Invalid cluster object passed for ReplicationSettings.");
				return;
			}
			empty = ((Group)ClusterObj).OwnerNode.Name;
		}
		IServer server = HyperVObjectFactory.GetServer(empty);
		if (!UIHelper.AssertHyperVToolsSupport(server, (HyperVComponent)0))
		{
			return;
		}
		try
		{
			settingDialog = HyperVObjectFactory.GetRecoveryConfigurationDialog(server, true, (HyperVAssemblyVersion)0);
			((IForm)settingDialog).Closed += delegate
			{
				((IDisposable)settingDialog).Dispose();
				settingDialog = null;
			};
			IWin32Window defaultWindow = Global.DefaultWindow;
			((IForm)settingDialog).ShowInTaskbar = true;
			((IForm)settingDialog).Show(defaultWindow);
		}
		catch (VirtualizationException val)
		{
			ClusterDialogException.ShowTaskDialogAsync((Exception)val);
		}
	}
}

