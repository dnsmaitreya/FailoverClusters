using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.FailoverClusters.UI.Controls;
using Microsoft.FailoverClusters.UIFramework;
using Microsoft.ManagementConsole;

namespace MS.Internal.ServerClusters.Management;

internal class ResourceContext : ContextBase, IDeleteable, IHasPropertyPages, IDisposable, IClusterSpecific
{
	private class SelectGroupForResource : SelectItemStrategy
	{
		private ClusterResource resource;

		private AsyncBatchEnumeration<ClusterListItem> asyncBatch;

		public SelectGroupForResource(ClusterResource resource)
			: base(Resources.SelectMoveToGroup_Title_Text, Resources.SelectMoveToGroup_Instructions_Text, Resources.SelectMoveToGroup_Fetching_Text, Resources.SelectMoveToGroup_None_Text, new string[5]
			{
				Resources.Name_Text,
				Resources.Status_Text,
				Resources.GroupType_Text,
				Resources.CurrentOwner_Text,
				Resources.Alerts_Text
			})
		{
			this.resource = resource;
			asyncBatch = new AsyncBatchEnumeration<ClusterListItem>(AsyncBatchReady);
		}

		public override void StartItemEnumeration()
		{
			resource.Cluster.GetGroupsAsync(AsyncEnumCallback);
		}

		private void AsyncBatchReady(ICollection<ClusterListItem> items)
		{
			OnEnumerationResultsReady(items);
		}

		private void AsyncEnumCallback(AsyncEnumerationUpdate<ClusterGroup> status)
		{
			try
			{
				if (status.Item != null && !ClusterUtilities.IsHiddenGroup(status.Item) && status.Item.Id != resource.GetOwnerGroup().Id)
				{
					ClusterListItem item = (ClusterListItem)(object)GroupListItem.Create(status.Item, (ClusterListItemOptions)1, (IDisposable)null, (ClusterListItemChildContext)1);
					asyncBatch.AddItem(item);
				}
				if (status.Error != null)
				{
					throw ExceptionHelp.Build<ApplicationException>(status.Error, new string[1] { Resources.SelectMoveToGroup_Error_Text });
				}
				if (status.IsComplete)
				{
					asyncBatch.FlushBatchedItems(null);
					OnEnumerationComplete();
				}
			}
			catch (Exception error)
			{
				OnEnumerationResultsReady(error);
			}
		}
	}

	private struct VerifyDeleteData
	{
		public bool IsLastCoreResource;

		public ClusterGroup OwnerGroup;

		public bool IsClientAccessPoint;

		public bool HasDependents;

		public bool VcoCleanup;
	}

	private class QuorumLossData
	{
		internal readonly ClusterGroup Group;

		internal readonly QuorumLossCheck Check;

		internal QuorumLossData(ClusterGroup group, QuorumLossCheck check)
		{
			Group = group;
			Check = check;
		}
	}

	private class VerifyResourceActionData
	{
		internal readonly VerifyActionCanBePerformedData verifyActionData;

		internal readonly QuorumLossData quorumLossData;

		internal VerifyResourceActionData(VerifyActionCanBePerformedData verifyActionData, QuorumLossData quorumLossData)
		{
			this.verifyActionData = verifyActionData;
			this.quorumLossData = quorumLossData;
		}
	}

	private volatile ActionsPaneItemCollection resourceActions;

	protected bool actionsAreDirty = true;

	private ClusterResource resource;

	private object stateActionLockObject = new object();

	private string resourceType;

	private string resourceName;

	private bool isCoreResource;

	private ActionBase bringResourceOnlineAction;

	private ActionBase repairNetworkNameAction;

	private TakeClusterNetnameOfflineActionPaneItem takeClusterNetnameOfflineActionPaneItem;

	private UICommand takeClusterNetnameOfflineCommand;

	private SimulateFailureActionPaneItem simulateFailureActionPaneItem;

	private UICommand simulateFailureCommand;

	public override Guid Id => resource.Id;

	public Cluster Cluster => resource.Cluster;

	public override ActionsPaneItemCollection ActionsPaneItems
	{
		get
		{
			if (resource == null || resource.IsDeleted || isDisposed)
			{
				return new ActionsPaneItemCollection();
			}
			if (actionsAreDirty)
			{
				resourceActions = Utilities.DisposeActions(resourceActions);
				resourceActions = CreateActions();
				actionsAreDirty = false;
			}
			return resourceActions;
		}
	}

	public ClusterResource Resource => resource;

	public string ResourceType => resourceType;

	public override List<WritableSharedDataItem> SharedData
	{
		get
		{
			if (resource.IsDeleted)
			{
				throw ExceptionHelp.Build<ClusterObjectDeletedException>(new string[1] { base.DisplayName });
			}
			List<WritableSharedDataItem> list = new List<WritableSharedDataItem>();
			WritableSharedDataItem writableSharedDataItem = new WritableSharedDataItem("CLUSTER_NAME", requiresCallback: false);
			writableSharedDataItem.SetData(Encoding.Unicode.GetBytes(Cluster.ConnectedTo + "\0"));
			list.Add(writableSharedDataItem);
			WritableSharedDataItem writableSharedDataItem2 = new WritableSharedDataItem("CLUSTER_RESOURCE_NAME", requiresCallback: false);
			writableSharedDataItem2.SetData(Encoding.Unicode.GetBytes(resource.Name + "\0"));
			list.Add(writableSharedDataItem2);
			WritableSharedDataItem writableSharedDataItem3 = new WritableSharedDataItem("CLUSTER_RESOURCE_TYPE_NAME", requiresCallback: false);
			writableSharedDataItem3.SetData(Encoding.Unicode.GetBytes(resource.ResourceTypeName + "\0"));
			list.Add(writableSharedDataItem3);
			WritableSharedDataItem writableSharedDataItem4 = new WritableSharedDataItem("CLUSTER_LCID", requiresCallback: false);
			writableSharedDataItem4.SetData(ClusterHelp.Int32ToByteArray(CultureInfo.CurrentCulture.LCID));
			list.Add(writableSharedDataItem4);
			return list;
		}
	}

	public PropertyPageCollection PropertyPages => GetPropertyPages();

	public override bool IsDeletable => !isCoreResource;

	internal ResourceContext(ClusterResource resource)
		: base(ClusterAdministrator.ResourceContextGuid)
	{
		this.resource = resource;
		resourceType = this.resource.ResourceTypeName;
		UpdateIsCoreResource();
		UpdateName();
		base.ImageIndex = IconsHelp.GetResourceIconIndex(this.resource);
		UpdateStandardVerbs();
		this.resource.StateChanged += OnStateChanged;
		this.resource.PropertiesChanged += OnPropertiesChanged;
		this.resource.Deleted += Resource_Deleted;
	}

	private void Resource_Deleted(object sender, EventArgs e)
	{
		Dispose();
	}

	private void UpdateName()
	{
		base.DisplayName = resource.DisplayName;
		resourceName = resource.Name;
	}

	private void UpdateIsCoreResource()
	{
		isCoreResource = Resource.IsCoreResource;
	}

	private void OnPropertiesChanged(object sender, EventArgs e)
	{
		UpdateName();
		UpdateIsCoreResource();
		actionsAreDirty = true;
		OnActionsUpdated();
	}

	private void OnStateChanged(object sender, EventArgs e)
	{
		UpdateStandardVerbs();
		UpdateStateBasedActions();
		actionsAreDirty = true;
		OnActionsUpdated();
		base.ImageIndex = IconsHelp.GetResourceIconIndex(resource);
	}

	public override void ClearActions()
	{
		resourceActions = Utilities.DisposeActions(resourceActions);
		takeClusterNetnameOfflineActionPaneItem?.Dispose();
		takeClusterNetnameOfflineActionPaneItem = null;
		takeClusterNetnameOfflineCommand = null;
		simulateFailureActionPaneItem?.Dispose();
		simulateFailureActionPaneItem = null;
		simulateFailureCommand = null;
		actionsAreDirty = true;
		base.ClearActions();
	}

	private ActionsPaneItemCollection CreateActions()
	{
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		actionsPaneItemCollection.AddRange(CreateResourceSpecificActions().ToArray());
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[2]
		{
			new ActionSeparator(),
			ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.ShowCriticalEvents), Resources.ShowResourceCriticalEventsActionDescription_Text, Icons.ClusterEventsIndex, OnShowCriticalEvents)
		});
		ActionsPaneItemCollection actionsPaneItemCollection2 = CreateResourceSpecificMoreActions();
		ActionsPaneItem dependencyReportAction = GetDependencyReportAction();
		if (dependencyReportAction != null)
		{
			actionsPaneItemCollection2.AddRange(new ActionsPaneItem[2]
			{
				new ActionSeparator(),
				dependencyReportAction
			});
		}
		if (actionsPaneItemCollection2.Count > 0)
		{
			ActionGroup actionGroup = new ActionGroup(StringExtensions.ReplaceAccelerator(CommandResources.MoreActions), Resources.MoreActionsActionsGroupDescription_Text, Icons.BlueArrowIndex);
			actionGroup.Items.AddRange(actionsPaneItemCollection2.ToArray());
			actionsPaneItemCollection.AddRange(new ActionsPaneItem[2]
			{
				new ActionSeparator(),
				actionGroup
			});
		}
		UpdateStateBasedActions();
		return actionsPaneItemCollection;
	}

	protected virtual ActionsPaneItem GetDependencyReportAction()
	{
		return SharedActions.CreateShowDependencyReportAction(OnShowDependencyReport);
	}

	protected ActionsPaneItemCollection CreateOnlineOfflineActions()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008e: Expected O, but got Unknown
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		bringResourceOnlineAction = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.BringResourceOnlineAction_Text), Resources.BringResourceOnlineActionDescription_Text, Icons.BringResourceOnlineIndex, OnBringOnline);
		UICommand val = new UICommand("TakeClusterNetnameOffline", (UICommandId)16, (Action<object>)delegate
		{
		}, (Predicate<object>)((object x) => resource.State == ResourceState.Online));
		((ClusterCommand)val).Text = StringExtensions.ReplaceAccelerator(CommandResources.TakeResourceOfflineAction_Text);
		((ClusterCommand)val).Description = ActionFactory.GenerateDisplayName(Resources.TakeResourceOfflineActionDescription_Text);
		takeClusterNetnameOfflineCommand = val;
		takeClusterNetnameOfflineActionPaneItem = new TakeClusterNetnameOfflineActionPaneItem((ICommand)takeClusterNetnameOfflineCommand, this);
		actionsPaneItemCollection.AddRange(new ActionsPaneItem[2] { bringResourceOnlineAction, takeClusterNetnameOfflineActionPaneItem.Action });
		return actionsPaneItemCollection;
	}

	protected virtual ActionsPaneItemCollection CreateResourceSpecificActions()
	{
		return CreateOnlineOfflineActions();
	}

	protected virtual ActionsPaneItemCollection CreateResourceSpecificMoreActions()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cf: Expected O, but got Unknown
		ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
		if (resource.IsResourceOfType(WellKnownResourceType.NetName) && resource.IsCoreResource)
		{
			repairNetworkNameAction = ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.RepairNetworkNameAction_Text), Resources.RepairNetworkNameActionDescription_Text, Icons.BringResourceOnlineIndex, OnRepairNetworkName);
			actionsPaneItemCollection.Add(repairNetworkNameAction);
		}
		if (actionsPaneItemCollection.Count > 0)
		{
			actionsPaneItemCollection.Add(new ActionSeparator());
		}
		UICommand val = new UICommand("SimulateFailure", (UICommandId)17, (Action<object>)delegate
		{
		}, (Predicate<object>)((object x) => resource.State != ResourceState.Failed && resource.State != ResourceState.Offline));
		((ClusterCommand)val).Text = StringExtensions.ReplaceAccelerator(CommandResources.SimulateResourceFailureAction_Text);
		((ClusterCommand)val).Description = ActionFactory.GenerateDisplayName(Resources.SimulateResourceFailureActionDescription_Text);
		simulateFailureCommand = val;
		simulateFailureActionPaneItem = new SimulateFailureActionPaneItem((ICommand)simulateFailureCommand, this);
		actionsPaneItemCollection.Add(simulateFailureActionPaneItem.Action);
		GroupType groupType = resource.GetOwnerGroup().GroupType;
		if (groupType != GroupType.CoreCluster && groupType != GroupType.ClusterSharedVolume)
		{
			actionsPaneItemCollection.AddRange(new ActionsPaneItem[2]
			{
				new ActionSeparator(),
				CreateMoveResourceActionsPaneItem(resource)
			});
		}
		return actionsPaneItemCollection;
	}

	protected override void UpdateStateBasedActions()
	{
		lock (stateActionLockObject)
		{
			ResourceState state = resource.State;
			((ClusterCommand)(object)takeClusterNetnameOfflineCommand)?.CanExecuteUpdate((object)this, new EventArgs());
			((ClusterCommand)(object)simulateFailureCommand)?.CanExecuteUpdate((object)this, new EventArgs());
			if (bringResourceOnlineAction != null)
			{
				if (state == ResourceState.Offline || state == ResourceState.Failed)
				{
					bringResourceOnlineAction.Enabled = true;
				}
				else
				{
					bringResourceOnlineAction.Enabled = false;
				}
			}
			if (repairNetworkNameAction != null)
			{
				repairNetworkNameAction.Enabled = state != ResourceState.Online;
			}
		}
	}

	private ActionsPaneItem CreateMoveResourceActionsPaneItem(ClusterResource clusterResource)
	{
		return ActionFactory.CreateAction(StringExtensions.ReplaceAccelerator(CommandResources.AssignToAnotherRole_Text), Resources.MoveResourceActionDescription_Text, Icons.MoveResourceIndex, OnMoveResource);
	}

	internal void OnSimulateFailure(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		VerifyActionCanBePerformedData verifyActionData = ActionVerification.BuildVerifyActionData(VerifyAction.QuorumLoss | VerifyAction.NetworkName, string.Format(CultureInfo.CurrentCulture, Resources.ClusterShutdownByResourceOffline_Text, base.DisplayName), QuorumLossCheck.Offline, ActionFactory.GenerateDisplayName(CommandResources.SimulateResourceFailureAction_Text), string.Format(CultureInfo.CurrentCulture, Resources.SimulateResourceFailureActionConfirmation_Text, base.DisplayName));
		if (VerifyActionCanBePerformed(notifyUserFromSender, e, null, verifyActionData, new ConfirmationMessage(Resources.SimulateResourceFailureActionConfirm_Text)))
		{
			CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.FailingResource_Text, base.DisplayName);
			using (cluadminWaitDialog)
			{
				cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformSimulateFailure);
			}
		}
	}

	private void PerformSimulateFailure(CluadminWaitDialog waitDialog)
	{
		resource.InitiateFailure();
	}

	private bool PerformCheckForQuorumLoss(CluadminWaitDialog waitDialog, QuorumLossData quorumLossData)
	{
		bool result = false;
		if (resource.IsQuorumResource && resource.State == ResourceState.Online)
		{
			bool flag = false;
			switch (quorumLossData.Check)
			{
			case QuorumLossCheck.GroupChange:
				if (!IsGroupOnSameNodeAsOwner(quorumLossData.Group))
				{
					flag = true;
				}
				break;
			case QuorumLossCheck.Offline:
				flag = true;
				break;
			case QuorumLossCheck.RemoveStorage:
			{
				ClusterGroup coreClusterGroup = resource.Cluster.GetCoreClusterGroup();
				if (!IsGroupOnSameNodeAsOwner(coreClusterGroup))
				{
					flag = true;
				}
				break;
			}
			default:
			{
				QuorumLossCheck check = quorumLossData.Check;
				DebugLog.LogWarning("Unknown check type: " + check);
				break;
			}
			}
			if (flag)
			{
				result = resource.WillOfflineLoseQuorum();
			}
		}
		return result;
	}

	private bool IsGroupOnSameNodeAsOwner(ClusterGroup one)
	{
		ClusterGroup ownerGroup = resource.GetOwnerGroup();
		return AreGroupsOnSameNode(ownerGroup, one);
	}

	private bool AreGroupsOnSameNode(ClusterGroup one, ClusterGroup two)
	{
		if (string.Compare(one.OwnerNodeName, two.OwnerNodeName, StringComparison.OrdinalIgnoreCase) != 0)
		{
			return false;
		}
		return true;
	}

	internal void OnTakeOffline(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		VerifyActionCanBePerformedData verifyActionData = ActionVerification.BuildVerifyActionData(VerifyAction.QuorumLoss | VerifyAction.NetworkName, string.Format(CultureInfo.CurrentCulture, Resources.ClusterShutdownByResourceOffline_Text, base.DisplayName), QuorumLossCheck.Offline, StringExtensions.RemoveAccelerator(CommandResources.TakeResourceOfflineAction_Text), string.Format(CultureInfo.CurrentCulture, Resources.TakeResourceOfflineActionConfirmationFormat_Text, base.DisplayName));
		if (VerifyActionCanBePerformed(notifyUserFromSender, e, null, verifyActionData, new ConfirmationMessage(Resources.TakeGroupOrResourceOfflineActionConfirm_Text)))
		{
			CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.TakingOfflineResource_Text, base.DisplayName);
			using (cluadminWaitDialog)
			{
				cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformTakeOffline);
			}
		}
	}

	private void PerformTakeOffline(CluadminWaitDialog waitDialog)
	{
		resource.BeginTakeOffline("snapin!ResourceContextActionPaneItem.PerformTakeOffline");
	}

	private void OnBringOnline(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		RepairNetworkName(notifyUserFromSender);
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.BringingOnlineResource_Text, base.DisplayName);
		using (cluadminWaitDialog)
		{
			try
			{
				cluadminWaitDialog.ShowDialog(notifyUserFromSender, delegate
				{
					Resource.BeginBringOnline("snapin!ResourceContextActionPaneItem.OnBringOnline");
				});
			}
			catch (Exception ex)
			{
				notifyUserFromSender.ShowError(ex);
			}
		}
	}

	private void RepairNetworkName(INotifyUser notifyUser)
	{
		try
		{
			using CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.RepairNetworkName_Text, Resources.RepairNetworkName_Text);
			cluadminWaitDialog.ShowDialog<object, IdentityReference>(notifyUser, SimpleNetworkNameRepair, null);
			if (!cluadminWaitDialog.IsCanceled)
			{
			}
		}
		catch (Exception ex)
		{
			notifyUser.ShowError(ex, Resources.RepairNetworkName_Failed_Text);
		}
	}

	private IdentityReference SimpleNetworkNameRepair(CluadminWaitDialog waitDialog, object data)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		if (resource.IsResourceOfType(WellKnownResourceType.NetName) && resource.State == ResourceState.Failed)
		{
			uint num = (uint)resource.GetPrivateProperties(PropertyCollectionSet.ReadOnly)["StatusKerberos"].Value;
			if ((num == 183 || num == 5) && (int)NetworkNameRepair.AttemptRepairAsSelf(resource) == 1)
			{
				return NetworkNameRepair.GetNetNameOwner(resource);
			}
		}
		return null;
	}

	private void OnShowCriticalEvents(object sender, SnapinActionEventArgs e)
	{
		LegacyFactory.ExecuteCriticalEventsDialog(Resource.Cluster, base.DisplayName, base.NodeType, delegate(EventLogFilter filter)
		{
			filter.ClusterResource = resourceName;
		}, null);
	}

	private void OnShowDependencyReport(object sender, SnapinActionEventArgs e)
	{
		SharedActions.PerformShowDependencyReport(ActionData.GetNotifyUserFromSender(sender), e, base.DisplayName, BuildDependencyReport);
	}

	private string BuildDependencyReport(CluadminWaitDialog waitDialog)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return new DependencyWriter().GenerateReport(resource);
	}

	private void OnMoveResource(object sender, SnapinActionEventArgs e)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		SelectItemDialog selectItemDialog = new SelectItemDialog(new SelectGroupForResource(resource));
		ClusterGroup clusterGroup = null;
		SelectItemDialog selectItemDialog2 = selectItemDialog;
		try
		{
			if (notifyUserFromSender.ShowDialog((Form)(object)selectItemDialog) != DialogResult.OK)
			{
				return;
			}
			clusterGroup = ((GroupListItem)selectItemDialog.SelectedItem).ClusterGroup;
		}
		finally
		{
			((IDisposable)selectItemDialog2)?.Dispose();
		}
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.RetrievingItem_Text);
		string text;
		using (cluadminWaitDialog)
		{
			text = cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformGetGroupName, clusterGroup);
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		VerifyActionCanBePerformedData verifyActionData = ActionVerification.BuildVerifyActionData(VerifyAction.QuorumLoss | VerifyAction.NetworkName, string.Format(CultureInfo.CurrentCulture, Resources.ClusterShutdownByResourceMove_Text, base.DisplayName), QuorumLossCheck.GroupChange, StringExtensions.ReplaceAccelerator(CommandResources.AssignToAnotherRole_Text), string.Format(CultureInfo.CurrentCulture, Resources.MoveResourceOfflineActionConfirmationFormat_Text, base.DisplayName, text));
		if (VerifyActionCanBePerformed(notifyUserFromSender, e, clusterGroup, verifyActionData, new ConfirmationMessage(string.Format(CultureInfo.CurrentCulture, Extensions.FormatCurrentCulture(Resources.MoveGroupOrResourceActionConfirm_Text, new object[2] { base.DisplayName, text })))))
		{
			CluadminWaitDialog cluadminWaitDialog3 = e.CreateWaitDialog(Resources.MovingResource_Text, base.DisplayName, text);
			using (cluadminWaitDialog3)
			{
				cluadminWaitDialog3.ShowDialog(notifyUserFromSender, PerformMoveResource, clusterGroup);
			}
		}
	}

	private string PerformGetGroupName(CluadminWaitDialog waitDialog, ClusterGroup group)
	{
		return group.Name;
	}

	private object PerformMoveResource(CluadminWaitDialog waitDialog, ClusterGroup group)
	{
		resource.ChangeGroup2(group);
		return null;
	}

	public override void Dispose()
	{
		if (!isDisposed)
		{
			base.Dispose();
			resource.StateChanged -= OnStateChanged;
			resource.PropertiesChanged -= OnPropertiesChanged;
			resource.Deleted -= Resource_Deleted;
			bringResourceOnlineAction = null;
			repairNetworkNameAction = null;
			GC.SuppressFinalize(this);
		}
	}

	protected virtual PropertyPageCollection GetPropertyPages()
	{
		return new PropertyPageCollection();
	}

	public virtual void Delete(object sender, Status status)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		CluadminWaitDialog cluadminWaitDialog = CluadminWaitDialog.Create(Resources.Delete_Resource_Text, Resources.VerifyResourceDelete_Text, base.DisplayName);
		VerifyDeleteData verifyDeleteData;
		using (cluadminWaitDialog)
		{
			verifyDeleteData = cluadminWaitDialog.ShowDialog<object, VerifyDeleteData>(notifyUserFromSender, VerifyDelete, null);
			if (cluadminWaitDialog.IsCanceled)
			{
				return;
			}
		}
		if (!ConfirmDeletion(notifyUserFromSender, verifyDeleteData))
		{
			return;
		}
		cluadminWaitDialog = CluadminWaitDialog.Create(Resources.Delete_Resource_Text, Resources.DeletingResource_Text, base.DisplayName);
		cluadminWaitDialog.DisplayDelay = new TimeSpan(0, 0, 1);
		using (cluadminWaitDialog)
		{
			cluadminWaitDialog.ShowDialog(notifyUserFromSender, PerformDelete, verifyDeleteData);
			if (!cluadminWaitDialog.IsCanceled)
			{
				ReportDeleteToSender(sender);
			}
		}
	}

	private bool ConfirmDeletion(INotifyUser notifyUser, VerifyDeleteData deleteData)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (deleteData.IsLastCoreResource)
		{
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}  ", Resources.Resource_Delete_CoreResource_Text);
		}
		if (deleteData.IsClientAccessPoint)
		{
			if (deleteData.VcoCleanup)
			{
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}  ", Resources.Resource_Delete_VcoCleanupOn_Text);
			}
			else
			{
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}  ", Resources.Resource_Delete_VcoCleanupOff_Text);
			}
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}  ", Resources.Resource_Delete_ClientAccessPoint_Text);
		}
		if (deleteData.HasDependents)
		{
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}  ", Resources.Resource_Delete_HasDependents_Text);
		}
		stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.Resource_Delete_Confirm_Text, base.DisplayName);
		return notifyUser.ShowYesNoQuestion(MessageBoxDefaultButton.Button2, stringBuilder.ToString()) == DialogResult.Yes;
	}

	private VerifyDeleteData VerifyDelete(CluadminWaitDialog waitDialog, object data)
	{
		VerifyDeleteData result = default(VerifyDeleteData);
		result.OwnerGroup = resource.GetOwnerGroup();
		result.IsClientAccessPoint = resource.IsResourceOfType(WellKnownResourceType.NetName);
		result.HasDependents = resource.GetDependentCount() != 0;
		result.VcoCleanup = Utilities.IsVcoCleanupOn(resource.Cluster);
		result.IsLastCoreResource = false;
		if (IsGroupCoreResource(resource, result.OwnerGroup))
		{
			result.IsLastCoreResource = true;
			foreach (ClusterResource resource in result.OwnerGroup.GetResources())
			{
				if (resource.Id != this.resource.Id && IsGroupCoreResource(resource, result.OwnerGroup))
				{
					result.IsLastCoreResource = false;
					break;
				}
			}
		}
		return result;
	}

	private object PerformDelete(CluadminWaitDialog waitDialog, VerifyDeleteData deleteData)
	{
		if (deleteData.IsClientAccessPoint)
		{
			ClusterResourceCollection dependencies = resource.GetDependencies();
			Collection<ClusterResource> collection = new Collection<ClusterResource>();
			foreach (ClusterResource item in dependencies)
			{
				if (!item.IsNetwork)
				{
					continue;
				}
				if (item.IsResourceOfType(WellKnownResourceType.IPv6TunnelAddress))
				{
					if (1 == item.GetDependentCount())
					{
						item.Delete("snapin!ResourceContextActionPaneItem.PerformDelete");
					}
				}
				else
				{
					collection.Add(item);
				}
			}
			foreach (ClusterResource item2 in collection)
			{
				if (1 == item2.GetDependentCount())
				{
					item2.Delete("snapin!ResourceContextActionPaneItem.PerformDelete");
				}
			}
		}
		try
		{
			resource.Delete("snapin!ResourceContextActionPaneItem.PerformDelete");
		}
		catch (Exception ex)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(ex);
			if (firstException != null && firstException.NativeErrorCode == -2147019859)
			{
				ClusterGenericException ex2 = ExceptionHelp.Build<ClusterGenericException>(ex, new string[2]
				{
					Resources.Resource_Delete_NodesDown_Text,
					base.DisplayName
				});
				ex2.Header = string.Format(CultureInfo.CurrentCulture, Resources.CannotDelete_Text, base.DisplayName);
				throw ex2;
			}
			throw;
		}
		if (deleteData.IsLastCoreResource)
		{
			try
			{
				deleteData.OwnerGroup.SetGroupType(GroupType.Unknown);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Error changing type");
			}
		}
		return null;
	}

	private bool IsGroupCoreResource(ClusterResource clusterResource, ClusterGroup group)
	{
		switch (group.GroupType)
		{
		case GroupType.StandAloneDfs:
			if (!IsDfsRoot(clusterResource))
			{
				return false;
			}
			break;
		case GroupType.DhcpServer:
			if (!clusterResource.IsResourceOfType(WellKnownResourceType.DhcpService))
			{
				return false;
			}
			break;
		case GroupType.Dtc:
			if (!clusterResource.IsResourceOfType(WellKnownResourceType.DistributedTransactionCoordinator))
			{
				return false;
			}
			break;
		case GroupType.Msmq:
			if (!clusterResource.IsResourceOfType(WellKnownResourceType.MicrosoftMessageQueue))
			{
				return false;
			}
			break;
		case GroupType.Wins:
			if (!clusterResource.IsResourceOfType(WellKnownResourceType.WinsService))
			{
				return false;
			}
			break;
		case GroupType.VirtualMachine:
			if (!clusterResource.IsResourceOfType(WellKnownResourceType.VirtualMachine))
			{
				return false;
			}
			break;
		case (GroupType)126u:
			if (!clusterResource.IsResourceOfType(WellKnownResourceType.HcsVirtualMachine))
			{
				return false;
			}
			break;
		default:
			return false;
		}
		foreach (ClusterResource dependency in clusterResource.GetDependencies())
		{
			if (dependency.IsResourceOfType(WellKnownResourceType.NetName))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsDfsRoot(ClusterResource clusterResource)
	{
		return clusterResource.IsResourceOfType(WellKnownResourceType.DfsNamespace);
	}

	public virtual void Refresh()
	{
		resource.Refresh();
		UpdateName();
		base.ImageIndex = IconsHelp.GetResourceIconIndex(resource);
		actionsAreDirty = true;
		UpdateStandardVerbs();
		UpdateStateBasedActions();
	}

	private bool PerformOwnerGroupHasNetworkNameResource(CluadminWaitDialog waitDialog)
	{
		return resource.GetOwnerGroup().HasNetworkName();
	}

	protected bool VerifyActionCanBePerformed(INotifyUser notifyUser, SnapinActionEventArgs e, ClusterGroup destGroup, VerifyActionCanBePerformedData verifyActionData, ConfirmationMessage confirmText)
	{
		bool canceled = false;
		CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.Resource_VerifyActionCanBePerformed_Text);
		VerifyAction verified;
		using (cluadminWaitDialog)
		{
			QuorumLossData quorumLossData = new QuorumLossData(destGroup, verifyActionData.QuorumData.QuorumCheck);
			VerifyResourceActionData data = new VerifyResourceActionData(verifyActionData, quorumLossData);
			verified = cluadminWaitDialog.ShowDialog(notifyUser, PerformVerifyActionCanBePerformed, data);
			canceled = cluadminWaitDialog.IsCanceled;
		}
		return ActionVerification.ProcessVerifyActionResult(notifyUser, verifyActionData, canceled, verified, confirmText);
	}

	private VerifyAction PerformVerifyActionCanBePerformed(CluadminWaitDialog waitDialog, VerifyResourceActionData verifyActionData)
	{
		VerifyAction verifyAction = VerifyAction.None;
		if ((verifyActionData.verifyActionData.Verifications & VerifyAction.QuorumLoss) == VerifyAction.QuorumLoss && PerformCheckForQuorumLoss(waitDialog, verifyActionData.quorumLossData))
		{
			verifyAction |= VerifyAction.QuorumLoss;
		}
		else if ((verifyActionData.verifyActionData.Verifications & VerifyAction.NetworkName) == VerifyAction.NetworkName && PerformOwnerGroupHasNetworkNameResource(waitDialog))
		{
			verifyAction |= VerifyAction.NetworkName;
		}
		return verifyAction;
	}

	protected virtual void UpdateStandardVerbs()
	{
		if (IsDeletable)
		{
			base.EnabledStandardVerbs |= StandardVerbs.Delete;
		}
		else
		{
			base.EnabledStandardVerbs &= ~StandardVerbs.Delete;
		}
	}

	private void OnRepairNetworkName(object sender, SnapinActionEventArgs e)
	{
		INotifyUser notifyUserFromSender = ActionData.GetNotifyUserFromSender(sender);
		using CluadminWaitDialog cluadminWaitDialog = e.CreateWaitDialog(Resources.RepairingNetName_Text, string.Empty);
		cluadminWaitDialog.ShowDialog(notifyUserFromSender, delegate
		{
			resource.ResetPassword();
			resource.BeginBringOnline("snapin!ResourceContextActionPaneItem.OnRepairNetworkName");
		});
		_ = cluadminWaitDialog.IsCanceled;
	}
}
