using System;
using FailoverClusters.ClusterSnapIn;
using FailoverClusters.Framework;
using FailoverClusters.UI.Common;
using FailoverClusters.UIFramework;
using FailoverClusters.WinForms;
using MS.Internal.ServerClusters.Management;

namespace FailoverClusters.SnapIn;

internal abstract class ClusterGridViewAdapterBase<TViewModel> : WpfViewAdapterBase<TViewModel> where TViewModel : ClusterViewModelBase
{
	private Cluster ConnectedCluster
	{
		get
		{
			if (base.ViewModel != null)
			{
				return ((ClusterViewModelBase)base.ViewModel).Cluster;
			}
			return null;
		}
	}

	protected override TViewModel InitializeAndCreateViewModel(ViewModelData viewModelData)
	{
		MS.Internal.ServerClusters.Management.ClusterContext clusterContextFromCache = ClusterConnectionFactory.GetClusterContextFromCache(base.ViewModelData.Id);
		if (clusterContextFromCache != null)
		{
			Cluster frameworkCluster = clusterContextFromCache.FrameworkCluster;
			if (frameworkCluster != null)
			{
				return CreateViewModel(frameworkCluster, viewModelData);
			}
		}
		return default(TViewModel);
	}

	protected abstract TViewModel CreateViewModel(Cluster cluster, ViewModelData viewModelData);

	public void ExecuteNewResourceWizard(AddResourceInputParameter parameter)
	{
		Exceptions.ThrowIfNull((object)parameter, "parameter");
		LegacyFactory.RunNewResourceWizard(ConnectedCluster.Id, parameter.ResourceType.Name, parameter.Group.Name);
	}

	public void ExecuteNewReplicationWizard(StorageResource storageResource, StorageResource replicatedResource, ReplicationWizardSetup replicationSetup)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!Global.DefaultDispatcher.CheckAccess())
		{
			Global.DefaultDispatcher.BeginInvoke(new Action<StorageResource, StorageResource, ReplicationWizardSetup>(ExecuteNewReplicationWizard), storageResource, replicatedResource, replicationSetup);
		}
		else
		{
			LegacyFactory.RunNewReplicationWizard(ConnectedCluster.Id, storageResource.Id, (replicatedResource != null) ? replicatedResource.Id : Guid.Empty, replicationSetup);
		}
	}

	protected override void VerifyCallbackRegistration(ClusterCommand clusterCommand)
	{
		if (clusterCommand == null)
		{
			return;
		}
		switch (clusterCommand.Id)
		{
		case ClusterCommandId.GroupAddResourceItem:
			clusterCommand.RegisterExecuteDelegate(delegate(object x)
			{
				ExecuteNewResourceWizard((AddResourceInputParameter)x);
			});
			break;
		case ClusterCommandId.ReplicationNew:
			clusterCommand.RegisterExecuteDelegate(delegate(object x)
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				StorageResource storageResource = (StorageResource)x;
				if (storageResource.Cluster.FunctionalLevel <= NativeMethods.NT9_MAJOR_VERSION)
				{
					ClusterDialogException.ShowTaskDialog(new ClusterReplicationNotSupportedException(ExceptionResources.ClusterReplicationSupportedException_NotSupportedCluster_Text));
				}
				else if ((int)storageResource.ReplicationDiskType != 0)
				{
					ClusterDialogException.ShowTaskDialog(new ClusterReplicationNotSupportedException(ExceptionResources.ClusterReplicationSupportedException_InReplication_Text));
				}
				else if (storageResource.OwnerGroup == storageResource.Cluster.AvailableStorage)
				{
					ClusterDialogException.ShowTaskDialog(new ClusterReplicationNotSupportedException());
				}
				else if (storageResource.ResourceState != ResourceState.Online)
				{
					ClusterDialogException.ShowTaskDialog(new ClusterReplicationNotSupportedException(ExceptionResources.ClusterReplicationSupportedException_NotOnline_Text));
				}
				else if (storageResource.DiskInfo != null && storageResource.DiskInfo.ClusterDiskId != null && !(storageResource.DiskInfo.ClusterDiskId is ClusterDiskGuidId))
				{
					ClusterDialogException.ShowTaskDialog(new ClusterReplicationMbrNotSupportedException());
				}
				else if (x as CsvVolumeResource != null)
				{
					ExecuteNewReplicationWizard((StorageResource)x, null, (ReplicationWizardSetup)0);
				}
				else
				{
					storageResource.LoadAsync(delegate
					{
						storageResource.OwnerGroup.AllResourcesNonCsv.ExecuteQuery(delegate(OperationResult<IClusterList<Resource>> opResult)
						{
							//IL_002f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0035: Invalid comparison between Unknown and I4
							if (opResult.Error == null)
							{
								StorageResource storageResource2 = null;
								foreach (Resource item in opResult.Result)
								{
									StorageResource storageResource3 = item as StorageResource;
									if (storageResource3 != null && (int)storageResource3.ReplicationDiskType == 1)
									{
										storageResource2 = storageResource3;
									}
								}
								ExecuteNewReplicationWizard(storageResource, storageResource2, (ReplicationWizardSetup)((!(storageResource2 == null)) ? 1 : 0));
							}
						});
					}, ResourceLoadSelection.Basic);
				}
			});
			break;
		case ClusterCommandId.ReplicationAdd:
			clusterCommand.RegisterExecuteDelegate(delegate(object x)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Invalid comparison between Unknown and I4
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				StorageResource storageResource4 = (StorageResource)x;
				if (storageResource4 != null)
				{
					if ((int)storageResource4.ReplicationDiskType == 1 && storageResource4.ReplicationInfo.ReplicationPrivateStorageResources != null && storageResource4.ReplicationInfo.ReplicationPrivateStorageResources.Count <= 2)
					{
						ClusterDialogException.ShowTaskDialog(new ClusterReplicationNotSupportedException(ExceptionResources.ClusterReplicationSupportedException_AddPartner_ClusterToCluster_Header, ExceptionResources.ClusterReplicationSupportedException_AddPartner_ClusterToCluster_Text));
					}
					else
					{
						ExecuteNewReplicationWizard(storageResource4, null, (ReplicationWizardSetup)2);
					}
				}
			});
			break;
		default:
			base.VerifyCallbackRegistration(clusterCommand);
			break;
		}
	}
}

