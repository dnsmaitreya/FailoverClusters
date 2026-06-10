using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using FailoverClusters.Configuration;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Wizards;
using FailoverClusters.UIFramework;
using ManagementConsole;
using Virtualization.Client.Common;
using KDDSL.ServerClusters.Configuration;
using KDDSL.ServerClusters.Validation;
using KDDSL.ServerClusters.Wizards;

namespace KDDSL.ServerClusters.Management;

internal static class SharedActions
{
	internal class FindValidationReportArg
	{
		private readonly ICollection<string> nodeNames;

		private readonly Cluster cluster;

		public ICollection<string> NodeNames
		{
			get
			{
				if (nodeNames != null)
				{
					return nodeNames;
				}
				return ClusterUtilities.GetNodeNamesFromCluster(cluster);
			}
		}

		public FindValidationReportArg(Cluster cluster)
		{
			this.cluster = cluster;
		}

		public FindValidationReportArg(ICollection<string> nodeNames)
		{
			this.nodeNames = nodeNames;
		}
	}

	public delegate string BuildDependencyReport(CluadminWaitDialog waitDialog);

	private class VirtualMachineCreationParams
	{
		public Cluster Cluster { get; private set; }

		public string Server { get; private set; }

		public VirtualMachineCreationParams(Cluster cluster, string server)
		{
			Cluster = cluster;
			Server = server;
		}
	}

	private static INewVirtualMachineWizard newVirtualMachineDialog;

	private static IWizard newVirtualHardDiskDialog;

	public static ActionBase CreateValidationAction(string actionDisplayName, SnapinActionEventHandler actionHandler)
	{
		if (string.IsNullOrEmpty(actionDisplayName))
		{
			throw new ArgumentNullException("actionDisplayName");
		}
		return ActionFactory.CreateAction(actionDisplayName, Resources.ValidateConfigurationAction_Description_Text, Icons.ValidateConfigurationIndex, actionHandler);
	}

	public static Cluster PerformValidationAction(INotifyUser notifyUser)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		Cluster result = null;
		ClusPrepWizardForm wiz = ClusPrepWizardForm.CreateHardwareValidationWizard(true);
		try
		{
			notifyUser.ShowDialog((Form)(object)wiz);
			if (!wiz.RunCreateCluster)
			{
				return null;
			}
			List<CandidateNode> clusterNodes = new List<CandidateNode>();
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.AddingClusterNodes_Text, Resources.AddingClusterNodes_Text))
			{
				cluadminWaitDialog.ShowDialog(notifyUser, delegate
				{
					clusterNodes.AddRange(wiz.ValidationEngine.Nodes.Select((Func<ValidationNode, CandidateNode>)((ValidationNode node) => new CandidateNode(node.Name))));
				});
			}
			CreateClusterWizard val = new CreateClusterWizard((ICollection<CandidateNode>)clusterNodes);
			try
			{
				notifyUser.ShowDialog((Form)(object)val);
				if (((ClusterWizardForm)val).WizardTaskCompleted)
				{
					result = val.Cluster;
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		finally
		{
			if (wiz != null)
			{
				((IDisposable)wiz).Dispose();
			}
		}
		return result;
	}

	public static ActionBase CreateViewValidationReportAction(SnapinActionEventHandler actionHandler)
	{
		return ActionFactory.CreateAction(Resources.ViewValidationReport_Text, Resources.ViewValidationReport_Description_Text, Icons.ViewValidationReportIndex, actionHandler);
	}

	public static void PerformViewValidationReportAction(ICollection<string> nodeNames, INotifyUser notifyUser, string displayName)
	{
		PerformViewValidationReportAction(new FindValidationReportArg(nodeNames), notifyUser, displayName);
	}

	public static void PerformViewValidationReportAction(Cluster cluster, INotifyUser notifyUser, string displayName)
	{
		PerformViewValidationReportAction(new FindValidationReportArg(cluster), notifyUser, displayName);
	}

	internal static void PerformViewValidationReportAction(FindValidationReportArg args, INotifyUser notifyUser, string displayName)
	{
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(displayName, Resources.FindingValidationReport_Text);
		string text;
		using (cluadminWaitDialog)
		{
			text = cluadminWaitDialog.ShowDialog(notifyUser, FindValidationReport, args);
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		if (text == null)
		{
			notifyUser.ShowError(Resources.NoValidationReport_Text);
		}
		else
		{
			ReportViewer.LaunchReportViewer(text);
		}
	}

	internal static string FindValidationReport(CluadminWaitDialog waitDialog, FindValidationReportArg data)
	{
		string text = ValidationUtilities.FindNodeSetSpecificReport(data.NodeNames, (ValidationMode)0);
		string text2 = null;
		if (text != null)
		{
			text2 = string.Format(CultureInfo.InvariantCulture, "{0}.htm", Path.GetTempFileName());
			XmlReportRenderer.TransformStandardHtmlReport(text, text2);
		}
		return text2;
	}

	public static ActionBase CreateAddNodesAction(SnapinActionEventHandler actionHandler)
	{
		return ActionFactory.CreateAction(CommandResources.AddNodeAction_Text, Resources.AddNodeAction_Description_Text, Icons.AddNodeIndex, actionHandler);
	}

	public static void PerformAddNodesAction(Cluster cluster, INotifyUser notifyUser)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		AddNodeWizard val = new AddNodeWizard(cluster);
		AddNodeWizard val2 = val;
		try
		{
			notifyUser.ShowDialog((Form)(object)val);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static ActionBase CreateNewServiceOrApplicationAction(SnapinActionEventHandler actionHandler)
	{
		return ActionFactory.CreateAction(CommandResources.MakeAppOrServiceHAAction_Text, Resources.MakeAppOrServiceHAAction_Description_Text, Icons.HARoleIndex, actionHandler);
	}

	public static ActionGroup CreateNewVirtualMachineActions(Cluster cluster, FailoverClusters.Framework.Cluster frameworkCluster)
	{
		ActionGroup actionGroup = new ActionGroup(CommandResources.VirtualMachineActions_Text, Resources.VirtualMachineActionsDescription_Text);
		ActionBase item = ActionFactory.CreateAction(CommandResources.NewVirtualMachineActionGroup_Text, Resources.NewVirtualMachineActionGroupDescription_Text, Icons.HARoleIndex, delegate(object s, SnapinActionEventArgs args)
		{
			PerformNewVirtualMachineWizardAction(s, args, isHardDisk: false);
		}, frameworkCluster, isSync: false);
		actionGroup.Items.Add(item);
		ActionBase item2 = ActionFactory.CreateAction(CommandResources.NewVirtualHardDiskActionGroup_Text, Resources.NewVirtualHardDiskActionGroupDescription_Text, Icons.HARoleIndex, delegate(object s, SnapinActionEventArgs args)
		{
			PerformNewVirtualMachineWizardAction(s, args, isHardDisk: true);
		}, frameworkCluster, isSync: false);
		actionGroup.Items.Add(item2);
		return actionGroup;
	}

	private static bool CheckVirtualMachineClientAvailable(INotifyUser notifyUser)
	{
		ClusterGenericException ex = ExceptionHelp.Build<ClusterGenericException>(Array.Empty<string>());
		ex.Caption = Resources.MessageBox_Information_Text;
		ex.Icon = TaskDialogIcon.Information;
		if (!ClusterAdministrator.IsVirtualMachineRoleInstalled)
		{
			ex.Header = Resources.CouldNotFindHyperVToolsHeader_Text;
			ex.Content = Resources.CouldNotFindHyperVToolsDescription_Text;
			notifyUser.ShowError((Exception)ex);
			return false;
		}
		return true;
	}

	public static void PerformNewServiceOrApplicationAction(Cluster cluster, INotifyUser notifyUser)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		ClusterResourceTypeCollection resourceTypes = null;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty))
		{
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				resourceTypes = cluster.GetResourceTypes();
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		HARoleWizard val = new HARoleWizard(cluster, resourceTypes);
		HARoleWizard val2 = val;
		try
		{
			notifyUser.ShowDialog((Form)(object)val);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static void PerformCopyClusterRoles(Cluster cluster, INotifyUser notifyUser)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		MigrateWizard val = new MigrateWizard(cluster);
		MigrateWizard val2 = val;
		try
		{
			notifyUser.ShowDialog((Form)(object)val);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static void PerformClusterAwareUpdating(Cluster cluster, INotifyUser notifyUser, string displayName)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		Exceptions.ThrowIfNull((object)notifyUser, "notifyUser");
		Exceptions.ThrowIfNullOrEmpty(displayName, "displayName");
		string clusterName = null;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(displayName, Resources.RetrievingClusterName_Text))
		{
			cluadminWaitDialog.ShowDialog(notifyUser, delegate
			{
				clusterName = cluster.Name;
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		LaunchClusterAwareUpdatingUI(clusterName);
	}

	public static void LaunchClusterAwareUpdatingUI(string clusterName)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Exceptions.ThrowIfNull((object)clusterName, "clusterName");
		if (!string.IsNullOrWhiteSpace(clusterName))
		{
			((ExecuteCommandLineCommand)new LaunchClusterAwareUpdatingCommand(clusterName)).Execute((object)null);
		}
	}

	public static void PerformValidateClusterConfiguration(Cluster targetCluster, INotifyUser notifyUser)
	{
		ClusPrepWizardForm wiz = ClusPrepWizardForm.CreateClusterValidationWizard();
		ClusPrepWizardForm val = wiz;
		try
		{
			BackgroundWaitDialogOperation<Cluster, object> backgroundOperation = delegate(CluadminWaitDialog waitDialog, Cluster data)
			{
				List<string> list = new List<string>();
				foreach (ClusterNode node in data.GetNodes())
				{
					string item;
					try
					{
						item = DnsSupport.CanonizeComputerName(node.Name);
					}
					catch (Exception caughtException)
					{
						Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
						if (firstException == null || (firstException.NativeErrorCode != -2147015893 && firstException.NativeErrorCode != -2147015395))
						{
							throw;
						}
						item = node.FqdnName;
					}
					list.Add(item);
				}
				wiz.AddAllNodesInCluster((IEnumerable<string>)list);
				return null;
			};
			using CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.ValidateConfiguringTitle_Text, Resources.ValidateConfiguringStateNodeNames_Text);
			cluadminWaitDialog.ShowDialog(notifyUser, backgroundOperation, targetCluster);
			if (!cluadminWaitDialog.IsCanceled)
			{
				notifyUser.ShowDialog((Form)(object)wiz);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static bool CheckVirtualMachineActionCanBePerformed(Cluster cluster, string nodeName, INotifyUser notifyUser)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		ServiceState serviceState = (ServiceState)0;
		try
		{
			Exception threadException = null;
			bool finishedRoleCollection = false;
			using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.NewVirtualMachineActionCollecting_Text, null))
			{
				cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					try
					{
						serviceState = ServiceData.GetVirtualMachineRoleState(nodeName);
						finishedRoleCollection = true;
					}
					catch (Exception ex3)
					{
						ExceptionHelp.LogException(ex3, Resources.NewVirtualMachineActionCollectingErrorTitle_Text);
						threadException = ex3;
					}
				});
				if (cluadminWaitDialog.IsCanceled)
				{
					return false;
				}
			}
			if (!finishedRoleCollection || threadException != null)
			{
				throw ExceptionHelp.Build<ApplicationException>(threadException, new string[1] { Resources.NewVirtualMachineActionCollectingError_Text });
			}
		}
		catch (Exception innerException)
		{
			ClusterGenericException ex = ExceptionHelp.Build<ClusterGenericException>(innerException, new string[1] { Resources.NewVirtualMachineActionCollectingError_Text });
			ex.Header = Resources.NewVirtualMachineActionCollectingErrorTitle_Text;
			throw ex;
		}
		bool flag = false;
		ClusterGenericException ex2 = ExceptionHelp.Build<ClusterGenericException>(Array.Empty<string>());
		ex2.Caption = Resources.MessageBox_Information_Text;
		ex2.Icon = TaskDialogIcon.Information;
		if ((int)serviceState == 1)
		{
			ex2.Header = string.Format(CultureInfo.CurrentCulture, Resources.NewVirtualMachineActionStatusUnsupportedHeader_Text, nodeName);
			ex2.Content = Resources.NewVirtualMachineActionStatusUnsupportedDescription_Text;
		}
		else if ((int)serviceState == 0)
		{
			ex2.Header = string.Format(CultureInfo.CurrentCulture, Resources.NewVirtualMachineActionStatusUnavailableHeader_Text, nodeName);
			ex2.Content = Resources.NewVirtualMachineActionStatusUnavailableDescription_Text;
		}
		else
		{
			flag = true;
		}
		if (!flag)
		{
			notifyUser.ShowError((Exception)ex2);
		}
		return flag;
	}

	private static Cluster GetClusterFromSender(object sender)
	{
		IScopeNodeContext context = ((CluAdminScopeNode)sender).Context;
		if (context is ClusterContext)
		{
			return ((ClusterContext)context).Cluster;
		}
		if (context is WpfClusterRolesContext)
		{
			return ((WpfClusterRolesContext)context).Cluster;
		}
		return null;
	}

	public static void NewVirtualHardDiskWizard(Cluster cluster, ClusterNode node, INotifyUser notifyUser)
	{
		if (newVirtualHardDiskDialog != null)
		{
			((IForm)newVirtualHardDiskDialog).Activate();
			return;
		}
		if (!IsHyperVToolsInstalled())
		{
			newVirtualHardDiskDialog = null;
			throw ExceptionHelp.Build<ClusterHyperVNotFoundException>(Array.Empty<string>());
		}
		try
		{
			IServer server = HyperVObjectFactory.GetServer(node.FqdnName);
			UIHelper.VerifyHyperVToolsSupport(server);
			newVirtualHardDiskDialog = HyperVObjectFactory.GetNewVirtualHardDiskWizard(server, (HyperVAssemblyVersion)0);
			((IForm)newVirtualHardDiskDialog).Closed += delegate
			{
				newVirtualHardDiskDialog = null;
			};
			IWin32Window win32Window = ((ClusterAdministrator.ActiveFormView != null) ? ClusterAdministrator.ActiveFormView.Control : null);
			newVirtualHardDiskDialog.StartModeless(win32Window);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error running NewVirtualHardDiskWizard");
			newVirtualHardDiskDialog = null;
			if (ex is ClusterHyperVNotSupportedException)
			{
				throw;
			}
			throw ExceptionHelp.Build<ClusterGenericException>(ex, new string[1] { Resources.VirtualMachineErrorOpenVMDiskWizard_Text });
		}
	}

	public static void NewVirtualMachineWizard(Cluster cluster, ClusterNode node, INotifyUser notifyUser)
	{
		if (newVirtualMachineDialog != null)
		{
			((IForm)newVirtualMachineDialog).Activate();
			return;
		}
		if (!IsHyperVToolsInstalled())
		{
			newVirtualHardDiskDialog = null;
			throw ExceptionHelp.Build<ClusterHyperVNotFoundException>(Array.Empty<string>());
		}
		try
		{
			string fqdnName = node.FqdnName;
			IServer server = HyperVObjectFactory.GetServer(fqdnName);
			UIHelper.VerifyHyperVToolsSupport(server);
			newVirtualMachineDialog = HyperVObjectFactory.GetNewVirtualMachineWizard(server, (HyperVAssemblyVersion)0);
			newVirtualMachineDialog.VirtualMachineCreated += VirtualMachineCreated;
			((IControl)newVirtualMachineDialog).Tag = new VirtualMachineCreationParams(cluster, fqdnName);
			((IForm)newVirtualMachineDialog).Closed += delegate
			{
				newVirtualMachineDialog = null;
			};
			IWin32Window win32Window = ((ClusterAdministrator.ActiveFormView != null) ? ClusterAdministrator.ActiveFormView.Control : null);
			((IWizard)newVirtualMachineDialog).StartModeless(win32Window);
		}
		catch (Exception ex)
		{
			ExceptionHelp.LogException(ex, "Error running NewVirtualMachineWizard");
			newVirtualHardDiskDialog = null;
			if (ex is ClusterHyperVNotSupportedException)
			{
				throw;
			}
			throw ExceptionHelp.Build<ClusterGenericException>(ex, new string[1] { Resources.VirtualMachineErrorOpenVMWizard_Text });
		}
	}

	public static void VirtualMachineCreated(string instanceId)
	{
		VirtualMachineCreationParams parameters = (VirtualMachineCreationParams)((IControl)newVirtualMachineDialog).Tag;
		BackgroundOperation<object, bool> obj = new BackgroundOperation<object, bool>((BackgroundOperationFunction<object, bool>)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			SynchronizeInvoke.Invoke((Delegate)(UIThreadHandlerV)delegate
			{
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Expected O, but got Unknown
				ViridianVirtualMachine virtualMachine = null;
				using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.GettingVirtualMachineConfiguration_Text, string.Empty))
				{
					cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
					{
						virtualMachine = ViridianVirtualMachine.GetVirtualMachine(parameters.Server, instanceId);
					});
					if (cluadminWaitDialog.IsCanceled)
					{
						return;
					}
				}
				HARoleWizard val = new HARoleWizard(parameters.Cluster, (HAStartupRoleWizard)1, new object[1] { virtualMachine });
				HARoleWizard val2 = val;
				try
				{
					ClusterAdministrator.NotifyUser.ShowDialog((Form)(object)val);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			});
			return true;
		});
		obj.QueueOperation((object)null);
		obj.OperationCompleted += delegate(object backgroundSender, BackgroundOperationCompletedEventArgs<object, bool> result)
		{
			if (!result.Success && result.Error != null)
			{
				ExceptionHelp.LogException(result.Error, "An error occurred creating a New Virtual Machine");
				ClusterAdministrator.NotifyUser.ShowError(result.Error, Resources.NewVirtualMachineWizardFailed_Text);
			}
		};
	}

	private static bool IsHyperVToolsInstalled()
	{
		bool installed = false;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingVirtualMachineToolsInstalled_Text, string.Empty))
		{
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				installed = new WindowsFeature().IsVirtualMachineClientToolsInstalled();
			});
			if (cluadminWaitDialog.IsCanceled)
			{
				installed = false;
			}
		}
		return installed;
	}

	public static void PerformNewVirtualMachineWizardAction(object sender, SnapinActionEventArgs e, bool isHardDisk)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		if (!CheckVirtualMachineClientAvailable(notifyUserFromSender))
		{
			return;
		}
		Cluster cluster = GetClusterFromSender(sender);
		FailoverClusters.Framework.Cluster cluster2 = (FailoverClusters.Framework.Cluster)((ActionData)e.Action.Tag).Tag;
		ClusterCommand obj = (isHardDisk ? new CreateVirtualHardDiskCommand(cluster2).GetInstance() : new CreateVirtualMachineCommand(cluster2).GetInstance());
		UIDialogProxyCommand val = new UIDialogProxyCommand(obj, (ICommand)obj, false);
		((UIProxyCommand)val).Execute();
		if (val.OutputObject == null)
		{
			return;
		}
		FailoverClusters.Framework.Node frameworkNode = val.OutputObject as FailoverClusters.Framework.Node;
		if (frameworkNode == null)
		{
			throw new InvalidOperationException("Dialog must return a framework node.");
		}
		ClusterNode node = null;
		using (CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.RetrievingItem_Text))
		{
			cluadminWaitDialog.ShowDialog(notifyUserFromSender, delegate
			{
				node = (from oldNode in cluster.GetNodes()
					where oldNode.Id == frameworkNode.Id
					select oldNode).FirstOrDefault();
			});
		}
		if (CheckVirtualMachineActionCanBePerformed(cluster, node.Name, notifyUserFromSender))
		{
			if (isHardDisk)
			{
				NewVirtualHardDiskWizard(cluster, node, notifyUserFromSender);
			}
			else
			{
				NewVirtualMachineWizard(cluster, node, notifyUserFromSender);
			}
		}
	}

	public static ActionBase CreateCloseConnectionAction(SnapinActionEventHandler actionHandler)
	{
		return ActionFactory.CreateAction(CommandResources.CloseClusterConnectionAction_Text, Resources.CloseClusterConnectionAction_Description_Text, Icons.CloseIndex, actionHandler);
	}

	public static ActionBase CreateResetRecentEventsAction(Cluster cluster)
	{
		return CreateResetRecentEventsAction(cluster, null);
	}

	public static ActionBase CreateResetRecentEventsAction(Cluster cluster, System.Action onCompleted)
	{
		return ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ResetRecentEvent_Text), CommandResources.ResetRecentEvent_Description_Text, Icons.ResetRecentEventsActionIndex, ResetRecentEvents, new Tuple<Cluster, System.Action>(cluster, onCompleted));
	}

	internal static void ResetRecentEvents(object sender, SnapinActionEventArgs e)
	{
		if (((ActionData)e.Action.Tag).Tag is Tuple<Cluster, System.Action> tuple)
		{
			Cluster item = tuple.Item1;
			if (item != null)
			{
				FindClusterContext(item)?.ResetRecentEvents();
				tuple.Item2.SafeCall();
			}
		}
	}

	public static ClusterContext FindClusterContext(Cluster cluster)
	{
		Exceptions.ThrowIfNull((object)cluster, "cluster");
		RootContext rootContext = (RootContext)ClusterAdministrator.Instance.RootNode.Context;
		string clusterFqdn = string.Empty;
		using (CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RetrievingItem_Text, string.Empty))
		{
			cluadminWaitDialog.ShowDialog(ClusterAdministrator.NotifyUser, delegate
			{
				clusterFqdn = cluster.FqdnName;
			});
		}
		return rootContext.FindChildContext(clusterFqdn) as ClusterContext;
	}

	public static ActionBase CreateStartServiceAction(SnapinActionEventHandler actionHandler)
	{
		return ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.StartServiceAction_Text), Resources.StartServiceActionDescription_Text, Icons.StartIndex, actionHandler, null, isSync: false);
	}

	public static ActionBase CreateStopServiceAction(SnapinActionEventHandler actionHandler)
	{
		return ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.StopServiceAction_Text), Resources.StopServiceActionDescription_Text, Icons.StopIndex, actionHandler, null, isSync: false);
	}

	public static ActionBase CreateShowDependencyReportAction(SnapinActionEventHandler actionHandler)
	{
		return ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ShowDependencyReport), Resources.ShowDependencyReportActionDescription_Text, Icons.DependencyReportIndex, actionHandler);
	}

	public static void PerformShowDependencyReport(INotifyUser notifyUser, SnapinActionEventArgs e, string itemName, BuildDependencyReport buildDependencyReport)
	{
		CluadminWaitDialog waitDialog = e.CreateWaitDialog(Resources.ShowingDependencyReport_Text, itemName);
		try
		{
			ReportViewer.LaunchReportViewer(waitDialog.ShowDialog(notifyUser, (CluadminWaitDialog _003Cp0_003E, BuildDependencyReport _003Cp1_003E) => buildDependencyReport(waitDialog), buildDependencyReport));
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.DependencyReportFailed_Text);
		}
		finally
		{
			if (waitDialog != null)
			{
				((IDisposable)waitDialog).Dispose();
			}
		}
	}

	public static void PerformConfigureClusterQuorumSettings(Cluster cluster, INotifyUser notifyUser)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		if (notifyUser == null)
		{
			notifyUser = ClusterAdministrator.NotifyUser;
		}
		QuorumConfigurationWizard val = new QuorumConfigurationWizard(cluster);
		QuorumConfigurationWizard val2 = val;
		try
		{
			notifyUser.ShowDialog((Form)(object)val);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}
}

