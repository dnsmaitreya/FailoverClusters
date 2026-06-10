using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UI.Common.Services;
using FailoverClusters.UIFramework;
using FailoverClusters.WinForms;
using ManagementConsole;
using KDDSL.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal abstract class WpfViewAdapterBase<TViewModel> : FormView, IWpfViewAdapter where TViewModel : ViewModelBase, IViewModelRefreshable
{
	private readonly SelectedClusterObjectsToTextConverter selectedClusterObjectsToTextConverter;

	private static readonly string displayNamePropertyName = ViewModelBaseExtensions.GetMemberNameFromPropertyExpression<IDataItem, string>((Expression<Func<IDataItem, string>>)((IDataItem me) => me.DisplayName));

	private readonly IUiActionProducer uiActionProducer;

	private object frameworkObject;

	public bool StatusBarStarted { get; set; }

	protected ViewModelData ViewModelData { get; private set; }

	private object CurrentSelectedItem
	{
		get
		{
			lock (base.SelectionData)
			{
				return base.SelectionData.SelectionObject;
			}
		}
	}

	protected TViewModel ViewModel { get; private set; }

	private bool IsShutdown { get; set; }

	public event EventHandler Show;

	public event EventHandler Hide;

	protected WpfViewAdapterBase()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		selectedClusterObjectsToTextConverter = new SelectedClusterObjectsToTextConverter();
		uiActionProducer = ServiceContainer.Container.Resolve<IUiActionProducer>(Array.Empty<object>());
	}

	public ViewModelBase SetupViewModel(ViewModelData viewModelData)
	{
		Exceptions.ThrowIfNull((object)viewModelData, "viewModelData");
		ViewModelData = viewModelData;
		ViewModel = InitializeAndCreateViewModel(viewModelData);
		((ViewModelBase)ViewModel).PropertyChanged += OnViewModelPropertyChanged;
		viewModelData.Refresh += Refresh;
		viewModelData.TargetedRefresh += Refresh;
		return (ViewModelBase)(object)ViewModel;
	}

	protected abstract TViewModel InitializeAndCreateViewModel(ViewModelData viewModelData);

	protected override void OnInitialize(AsyncStatus status)
	{
		base.OnInitialize(status);
		ClusterAdministrator.SelectScopeNode += OnSelectScopeNode;
	}

	protected override void OnShow()
	{
		ClusterAdministrator.ActiveFormView = this;
		Global.DefaultWindow = base.Control;
		this.Show?.Invoke(this, EventArgs.Empty);
		ServiceContainer.Container.Resolve<ISetPropertyPageDelegateService>(Array.Empty<object>()).SetPropertyPageDelegate(ExecuteShowPropertiesPage);
		ISelectionContextService obj = ServiceContainer.Container.Resolve<ISelectionContextService>(Array.Empty<object>());
		obj.SelectionChanged += OnSelectionChanged;
		obj.SelectedItemsPropertyChanged += OnSelectedItemsPropertyChanged;
		object obj2 = ViewModel;
		IGridViewModel val = (IGridViewModel)((obj2 is IGridViewModel) ? obj2 : null);
		if (val != null)
		{
			val.ResumeEvents();
			ServiceContainer.Container.Resolve<ISelectionChangedService>(Array.Empty<object>()).SetSelectedItems(val.SelectedItems, val.SelectedItemsCommands);
		}
		base.OnShow();
	}

	protected override void OnHide()
	{
		base.OnHide();
		ISelectionContextService obj = ServiceContainer.Container.Resolve<ISelectionContextService>(Array.Empty<object>());
		obj.SelectedItemsPropertyChanged -= OnSelectedItemsPropertyChanged;
		obj.SelectionChanged -= OnSelectionChanged;
		object obj2 = ViewModel;
		IGridViewModel val = (IGridViewModel)((obj2 is IGridViewModel) ? obj2 : null);
		if (val != null)
		{
			val.SuspendEvents();
		}
		this.Hide?.Invoke(this, EventArgs.Empty);
	}

	protected override void OnShutdown(SyncStatus status)
	{
		IsShutdown = true;
		if (ViewModel != null)
		{
			((ViewModelBase)ViewModel).PropertyChanged -= OnViewModelPropertyChanged;
			if ((object)ViewModel is IDisposable disposable)
			{
				disposable.Dispose();
			}
			ViewModel = default(TViewModel);
		}
		ClusterAdministrator.SelectScopeNode -= OnSelectScopeNode;
		base.OnShutdown(status);
		ViewModelData.Refresh -= Refresh;
		ViewModelData.TargetedRefresh -= Refresh;
	}

	public virtual IEnumerable<IDataItem> ApplyFilterOnSelectedItems(IEnumerable<IDataItem> originalSetOfSelectedItems)
	{
		return originalSetOfSelectedItems;
	}

	private void Refresh(object sender, EventArgs e)
	{
		((IViewModelRefreshable)ViewModel).Refresh(false);
	}

	private void OnSelectionChanged(object sender, EventArgs args)
	{
		ISelectionContextService obj = ServiceContainer.Container.Resolve<ISelectionContextService>(Array.Empty<object>());
		IEnumerable<IDataItem> selectedItems = obj.SelectedItems;
		IEnumerable<ICommand> selectedItemsCommands = obj.SelectedItemsCommands;
		OnSelectedItemsChanged(selectedItems, selectedItemsCommands);
	}

	private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName == ViewModelBaseExtensions.GetMemberNameFromPropertyExpression<ClusterGridDetailsViewModelBase, IEnumerable<ICommand>>((Expression<Func<ClusterGridDetailsViewModelBase, IEnumerable<ICommand>>>)((ClusterGridDetailsViewModelBase me) => me.SelectedItemsCommands)))
		{
			ISelectionContextService obj = ServiceContainer.Container.Resolve<ISelectionContextService>(Array.Empty<object>());
			IEnumerable<IDataItem> selectedItems = obj.SelectedItems;
			IEnumerable<ICommand> selectedItemsCommands = obj.SelectedItemsCommands;
			OnSelectedItemsChanged(selectedItems, selectedItemsCommands);
		}
	}

	private void OnSelectedItemsPropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		if (!IsShutdown && args.PropertyName == displayNamePropertyName)
		{
			ISelectionContextService val = ServiceContainer.Container.Resolve<ISelectionContextService>(Array.Empty<object>());
			IEnumerable<IDataItem> selectedItems = ApplyFilterOnSelectedItems(val.SelectedItems);
			UpdateSelectedItemName(selectedItems);
		}
	}

	private void UpdateSelectedItemName(IEnumerable<IDataItem> selectedItems)
	{
		if (selectedItems == null)
		{
			selectedItems = Enumerable.Empty<IDataItem>();
		}
		string displayName = (string)selectedClusterObjectsToTextConverter.Convert(new object[1] { selectedItems }, typeof(string), (object)null, CultureInfo.CurrentCulture);
		lock (base.SelectionData)
		{
			base.SelectionData.BeginUpdates();
			base.SelectionData.DisplayName = displayName;
			base.SelectionData.EndUpdates();
		}
	}

	private void OnSelectedItemsChanged(IEnumerable<IDataItem> selectedItems, IEnumerable<ICommand> selectedItemsCommands)
	{
		if (!IsShutdown && ViewModelData != null)
		{
			uiActionProducer.Enqueue(new CommandsToActionsContainer(this, selectedItems.ToList(), selectedItemsCommands.ToList(), base.SelectionData, ViewModelData.HelpTopic));
		}
	}

	private void OnSelectScopeNode(object sender, SelectScopeNodeEventArgs e)
	{
		try
		{
			if (base.Control != null && !base.Control.IsDisposed && e.ScopeNode.Parent != null)
			{
				SelectScopeNode(e.ScopeNode);
			}
		}
		catch (Exception ex)
		{
			ClusterLog.LogException(ex, "Error selecting scope node");
		}
	}

	protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
	{
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Invalid comparison between Unknown and I4
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Invalid comparison between Unknown and I4
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Invalid comparison between Unknown and I4
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Invalid comparison between Unknown and I4
		object currentSelectedItem;
		if (frameworkObject != null)
		{
			currentSelectedItem = frameworkObject;
			frameworkObject = null;
		}
		else
		{
			currentSelectedItem = CurrentSelectedItem;
		}
		Group group = currentSelectedItem as Group;
		if (group != null)
		{
			FailoverClusters.WinForms.GroupContext.GetPropertyPages(propertyPageCollection, group.GroupType, group.Cluster, group.Id);
			return;
		}
		Network network = currentSelectedItem as Network;
		if (network != null)
		{
			FailoverClusters.WinForms.NetworkContext.GetNetworkPropertyPages(propertyPageCollection, network.Cluster, network.Id);
			return;
		}
		Resource resource = currentSelectedItem as Resource;
		if (resource != null)
		{
			PropertyPage propertyPage = LegacyFactory.CreateResourceGeneralPropertyPage(resource.Cluster.Id, resource.Name);
			if (propertyPage != null)
			{
				propertyPageCollection.Add(propertyPage);
			}
			FailoverClusters.WinForms.ResourceContext.GetPropertyPages(propertyPageCollection, resource.ResourceType, resource.Cluster, resource.Id);
			PropertyPage propertyPage2 = LegacyFactory.CreateResourceRegistryPropertyPage(resource.Cluster.Id, resource.Name);
			if (propertyPage2 != null)
			{
				propertyPageCollection.Add(propertyPage2);
			}
			if (currentSelectedItem.GetType() == typeof(StorageResource))
			{
				ClusterSnapinPropertyPage clusterSnapinPropertyPage = new ClusterSnapinPropertyPage();
				clusterSnapinPropertyPage.SetControl(new ShadowCopyPropertiesPage(resource.Cluster, resource.Id));
				propertyPageCollection.Add(clusterSnapinPropertyPage);
			}
			StorageResource storageResource = currentSelectedItem as StorageResource;
			if (storageResource != null && ((int)storageResource.ReplicationDiskType == 1 || (int)storageResource.ReplicationDiskType == 3))
			{
				ClusterSnapinPropertyPage clusterSnapinPropertyPage2 = new ClusterSnapinPropertyPage();
				clusterSnapinPropertyPage2.SetControl(new ReplicationPropertyPage(resource.Cluster, resource.Id));
				propertyPageCollection.Add(clusterSnapinPropertyPage2);
			}
			if (storageResource != null && ((int)storageResource.ReplicationDiskType == 2 || (int)storageResource.ReplicationDiskType == 4))
			{
				ClusterSnapinPropertyPage clusterSnapinPropertyPage3 = new ClusterSnapinPropertyPage();
				clusterSnapinPropertyPage3.SetControl(new ReplicationLogPropertyPage(resource.Cluster, resource.Id));
				propertyPageCollection.Add(clusterSnapinPropertyPage3);
			}
		}
		else
		{
			Cluster cluster = currentSelectedItem as Cluster;
			if (cluster != null)
			{
				FailoverClusters.WinForms.ClusterContext.GetPropertyPages(propertyPageCollection, cluster);
			}
		}
	}

	public void RecursivelyProcessCommands(IEnumerable<ICommand> commandsCollection, bool rootLevel)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (IsShutdown)
		{
			return;
		}
		foreach (ICommand item in commandsCollection)
		{
			if (item is UIProxyCommand)
			{
				VerifyCallbackRegistration(((UIProxyCommand)item).Command);
			}
			else
			{
				VerifyCallbackRegistration(item as ClusterCommand);
			}
			if (rootLevel)
			{
				if (item is UISeparator)
				{
					base.SelectionData.ActionsPaneItems.Add(new ActionSeparator());
				}
				else if (!(item is ClusterCommand clusterCommand) || clusterCommand.Visible)
				{
					base.SelectionData.ActionsPaneItems.Add(new MmcActionPaneItem(item).Action);
				}
			}
			UIProxyCommand val = (UIProxyCommand)((item is UIProxyCommand) ? item : null);
			if (val != null && val.Children != null && val.Children.Count > 0)
			{
				RecursivelyProcessCommands(val.Children, rootLevel: false);
			}
		}
	}

	protected virtual void VerifyCallbackRegistration(ClusterCommand clusterCommand)
	{
		if (clusterCommand != null)
		{
			switch (clusterCommand.Id)
			{
			case ClusterCommandId.GroupProperties:
			case ClusterCommandId.ResourceProperties:
			case ClusterCommandId.NetworkProperties:
			case ClusterCommandId.ClusterProperties:
				clusterCommand.RegisterExecuteDelegate(ExecuteShowPropertiesPage);
				break;
			case ClusterCommandId.GroupShowDependencyReport:
			case ClusterCommandId.ResourceShowDependencyReport:
				clusterCommand.RegisterExecuteDelegate(ExecuteShowDependencyReport);
				break;
			case ClusterCommandId.GroupAddStorage:
				clusterCommand.RegisterExecuteDelegate(ExecuteAddStorage);
				break;
			case ClusterCommandId.ShowCriticalEvent:
				clusterCommand.RegisterExecuteDelegate(ExecuteShowCriticalEvents);
				break;
			case ClusterCommandId.StorageChangeDriveLetter:
				clusterCommand.RegisterExecuteDelegate(ExecuteChangeDriveLetter);
				break;
			case ClusterCommandId.StorageRepair:
				clusterCommand.RegisterExecuteDelegate(ExecuteRepairStorage);
				break;
			case ClusterCommandId.StoragePoolAddDisks:
				clusterCommand.RegisterExecuteDelegate(ExecutePoolAddDisk);
				break;
			case ClusterCommandId.ConfigureRole:
				clusterCommand.RegisterExecuteDelegate(ExecuteConfigureRole);
				break;
			case ClusterCommandId.ValidateClusterConfiguration:
				clusterCommand.RegisterExecuteDelegate(ExecuteValidateClusterConfiguration);
				break;
			case ClusterCommandId.ViewValidationReport:
				clusterCommand.RegisterExecuteDelegate(ExecuteViewValidationReport);
				break;
			case ClusterCommandId.AddNode:
				clusterCommand.RegisterExecuteDelegate(ExecuteAddNode);
				break;
			case ClusterCommandId.ConfigureClusterQuorumSettings:
				clusterCommand.RegisterExecuteDelegate(ExecuteConfigureClusterQuorumSettings);
				break;
			case ClusterCommandId.CopyClusterRoles:
				clusterCommand.RegisterExecuteDelegate(ExecuteCopyClusterRoles);
				break;
			case ClusterCommandId.ClusterAwareUpdating:
				clusterCommand.RegisterExecuteDelegate(ExecuteClusterAwareUpdating);
				break;
			}
		}
	}

	private void ExecuteRepairStorage(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		Resource resource = value as Resource;
		if (resource == null)
		{
			throw new ArgumentException("ExecuteRepairStorage does not recognize [" + value.GetType()?.ToString() + "] as a parameter.");
		}
		LegacyFactory.RepairStorage(resource.Cluster.Id, resource.Name);
	}

	private static void ExecuteChangeDriveLetter(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is Tuple<Resource, uint> tuple))
		{
			throw new ArgumentException("ExecuteChangeDriveLetter does not recognize [" + value.GetType()?.ToString() + "] as a parameter.");
		}
		Resource item = tuple.Item1;
		LegacyFactory.ChangeDriveLetter(item.Cluster.Id, item.Name, tuple.Item2);
	}

	public void ExecuteShowPropertiesPage(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		frameworkObject = value;
		string text;
		if (frameworkObject is Cluster)
		{
			text = ((Cluster)frameworkObject).FullyQualifiedDomainName;
		}
		else if (frameworkObject is Resource)
		{
			Resource resource = (Resource)frameworkObject;
			text = LegacyFactory.GetResourceDisplayName(LegacyFactory.GetLegacyResource(resource.Cluster.Id, resource.Name));
		}
		else
		{
			if (!(frameworkObject is ClusterObject))
			{
				if (frameworkObject is FileShare)
				{
					((FileShare)frameworkObject).OpenProperties();
				}
				else
				{
					ClusterLog.LogVerbose((LogSubcategory)10, "ExecuteShowPropertiesPage does not recognize [" + frameworkObject.GetType()?.ToString() + "] as a parameter.");
				}
				return;
			}
			text = ((ClusterObject)frameworkObject).Name;
		}
		lock (base.SelectionData)
		{
			base.SelectionData.BeginUpdates();
			try
			{
				Group group = frameworkObject as Group;
				Network network = frameworkObject as Network;
				Resource resource2 = frameworkObject as Resource;
				Cluster cluster = frameworkObject as Cluster;
				base.SelectionData.Update(null, multiSelection: true, null, null);
				if (group != null)
				{
					base.SelectionData.Update(group, multiSelection: false, FailoverClusters.WinForms.GroupContext.GetNodeTypes(), FailoverClusters.WinForms.GroupContext.GetSharedData(group));
				}
				else if (network != null)
				{
					base.SelectionData.Update(network, multiSelection: false, FailoverClusters.WinForms.NetworkContext.GetNodeTypes(), FailoverClusters.WinForms.NetworkContext.GetSharedData(network));
				}
				else if (resource2 != null)
				{
					base.SelectionData.Update(resource2, multiSelection: false, FailoverClusters.WinForms.ResourceContext.GetNodeTypes(), FailoverClusters.WinForms.ResourceContext.GetSharedData(resource2));
				}
				else if (cluster != null)
				{
					base.SelectionData.Update(cluster, multiSelection: false, FailoverClusters.WinForms.ClusterContext.GetNodeTypes(), FailoverClusters.WinForms.ClusterContext.GetSharedData(cluster));
				}
				else
				{
					base.SelectionData.Update(frameworkObject, multiSelection: false, null, null);
				}
			}
			finally
			{
				base.SelectionData.EndUpdates();
			}
		}
		base.SelectionData.ShowPropertySheet(Extensions.FormatCurrentCulture(Resources.PropertiesPageTitleText, (object)text));
	}

	public void ExecuteShowDependencyReport(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		frameworkObject = value;
		ClusterObject clusterObject = frameworkObject as ClusterObject;
		if (clusterObject != null)
		{
			if (clusterObject is Group)
			{
				LegacyFactory.ShowGroupDependencyReport(clusterObject.Cluster.Id, clusterObject.Name);
			}
			else if (clusterObject is Resource)
			{
				LegacyFactory.ShowResourceDependencyReport(clusterObject.Cluster.Id, clusterObject.Name);
			}
			return;
		}
		throw new ArgumentException("ExecuteShowDependencyReport does not recognize [" + frameworkObject.GetType()?.ToString() + "] as a parameter.");
	}

	public void ExecutePoolAddDisk(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		StoragePoolResource storagePoolResource = value as StoragePoolResource;
		if (storagePoolResource != null)
		{
			LegacyFactory.PoolAddDisks(storagePoolResource.Cluster.Id, storagePoolResource.Name, storagePoolResource.Id);
			return;
		}
		throw new ArgumentException("ExecutePoolAddDisk does not recognize [" + value.GetType()?.ToString() + "] as a parameter.");
	}

	public void ExecuteAddStorage(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		frameworkObject = value;
		Group group = frameworkObject as Group;
		if (group != null)
		{
			LegacyFactory.AddStorageToGroup(group.Cluster.Id, group.Name);
			return;
		}
		throw new ArgumentException("ExecuteAddStorage does not recognize [" + frameworkObject.GetType()?.ToString() + "] as a parameter.");
	}

	public void ExecuteShowCriticalEvents(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		frameworkObject = value;
		ClusterObject clusterObject = frameworkObject as ClusterObject;
		if (clusterObject != null)
		{
			if (clusterObject is Group)
			{
				LegacyFactory.ShowGroupCriticalEventsDialog(clusterObject.Cluster.Id, clusterObject.Name);
			}
			else if (clusterObject is Resource)
			{
				StorageResource storageResource = clusterObject as StorageResource;
				if (storageResource != null)
				{
					storageResource.LoadAsync(delegate
					{
						//IL_0008: Unknown result type (might be due to invalid IL or missing references)
						Guid[] array = null;
						if ((int)storageResource.ReplicationDiskType != 0 && storageResource.ReplicationInfo != null)
						{
							array = storageResource.ReplicationInfo.ReplicationGroupIds.ToArray();
						}
						Global.DefaultDispatcher.BeginInvoke(new Action<Guid, string, Guid[]>(LegacyFactory.ShowResourceCriticalEventsDialog), clusterObject.Cluster.Id, clusterObject.Name, array);
					}, ResourceLoadSelection.StorageReplicationInfo);
				}
				else
				{
					LegacyFactory.ShowResourceCriticalEventsDialog(clusterObject.Cluster.Id, clusterObject.Name, null);
				}
			}
			else if (clusterObject is Network)
			{
				LegacyFactory.ShowNetworkCriticalEventsDialog(clusterObject.Cluster.Id, clusterObject.Name);
			}
			else if (clusterObject is NetworkInterface)
			{
				LegacyFactory.ShowNetworkInterfaceCriticalEventsDialog(clusterObject.Cluster.Id, clusterObject.Name);
			}
			else if (clusterObject is FailoverClusters.Framework.Node)
			{
				LegacyFactory.ShowNodeCriticalEventsDialog(clusterObject.Cluster.Id, clusterObject.Name);
			}
			return;
		}
		throw new ArgumentException("ExecuteShowCriticalEvents does not recognize [" + frameworkObject.GetType()?.ToString() + "] as a parameter.");
	}

	public void ExecuteConfigureRole(object value)
	{
		frameworkObject = value;
		LegacyFactory.ConfigureRole(((Cluster)value).Id);
	}

	public void ExecuteValidateClusterConfiguration(object value)
	{
		frameworkObject = value;
		LegacyFactory.ValidateClusterConfiguration(((Cluster)value).Id);
	}

	public void ExecuteViewValidationReport(object value)
	{
		frameworkObject = value;
		LegacyFactory.ViewValidationReport(((Cluster)value).Id);
	}

	public void ExecuteAddNode(object value)
	{
		frameworkObject = value;
		LegacyFactory.AddNode(((Cluster)value).Id);
	}

	public void ExecuteCloseConnection(object value)
	{
		frameworkObject = value;
		LegacyFactory.CloseConnection(((Cluster)value).Id);
	}

	public void ExecuteConfigureClusterQuorumSettings(object value)
	{
		frameworkObject = value;
		LegacyFactory.ConfigureClusterQuorumSettings(((Cluster)value).Id);
	}

	public void ExecuteCopyClusterRoles(object value)
	{
		frameworkObject = value;
		LegacyFactory.CopyClusterRoles(((Cluster)value).Id);
	}

	public void ExecuteShutdownCluster(object value)
	{
		frameworkObject = value;
		LegacyFactory.ConfigureClusterQuorumSettings(((Cluster)value).Id);
	}

	public void ExecuteDestroyCluster(object value)
	{
		frameworkObject = value;
		LegacyFactory.ConfigureClusterQuorumSettings(((Cluster)value).Id);
	}

	public void ExecuteClusterAwareUpdating(object value)
	{
		frameworkObject = value;
		LegacyFactory.ClusterAwareUpdating(((Cluster)value).Id);
	}
}

