using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public class StorageQuorumSettings : QuorumSettings, IHasQuorumResource
{
	private QuorumType m_quorumType;

	private ClusterResource m_resource;

	private static int AsymmetricDiskOnlineWaitTimeInMinutes = 3;

	public virtual ClusterResource QuorumResource => m_resource;

	public override QuorumType QuorumType => m_quorumType;

	private StorageQuorumSettings(ClusterResource resource, QuorumType quorumType)
		: base(resource.Cluster)
	{
		m_resource = resource;
		m_quorumType = quorumType;
	}

	internal override void Configure()
	{
		//Discarded unreachable code: IL_0110, IL_01fb
		bool flag = false;
		string format = ((m_quorumType != QuorumType.LegacyDisk) ? Resources.StorageWitnessConfiguringQuorumFormat_Text : Resources.LegacyDiskConfiguringQuorumFormat_Text);
		ReportOperationProcess(20, format, m_resource.Name);
		try
		{
			if (m_resource.GetOwnerGroup().GroupType == GroupType.AvailableStorage)
			{
				ClusterGroup coreClusterGroup = base.Cluster.GetCoreClusterGroup();
				ReportOperationProcess(40, Resources.MovingDiskToCoreGroupFormat_Text, m_resource.Name);
				try
				{
					m_resource.ChangeGroup(coreClusterGroup);
					flag = true;
				}
				catch (ApplicationException caughtException)
				{
					Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
					if (firstException == null || firstException.NativeErrorCode != -2147018933)
					{
						throw;
					}
					ReportOperationProcess(50, Resources.MovingCoreClusterGroup_Text);
					coreClusterGroup.BeginMove(null);
					TimeSpan timeout = TimeSpan.FromMinutes(3.0);
					if (!m_resource.TryWaitForDesiredState(ResourceState.Online, timeout))
					{
						ReportOperationProcess(55, Resources.ErrorMovingCoreClusterGroup_Text, OperationProgressWarningLevel.Error);
						throw ExceptionHelp.Build<ClusterResourceNotOnlineException>(new string[1] { m_resource.DisplayName });
					}
					ReportOperationProcess(55, Resources.DoneMovingCoreClusterGroup_Text);
				}
			}
			ReportOperationProcess(60, Resources.ChoosingBestVolume_Text);
			string deviceName = PickQuorumPartition(m_resource.Storage_GetDiskInfo(includeMountPoints: false)).BuildVolumeGuidPath("Cluster");
			string format2 = ((m_quorumType != QuorumType.LegacyDisk) ? Resources.StorageWitnessSettingAsQuorumFormat_Text : Resources.LegacyDiskSettingAsQuorumFormat_Text);
			ReportOperationProcess(80, format2, m_resource.Name);
			if (m_quorumType == QuorumType.LegacyDisk)
			{
				base.Cluster.SetLegacyQuorum(m_resource, deviceName);
			}
			else
			{
				base.Cluster.SetMajorityQuorum(m_resource, deviceName);
			}
		}
		catch (ApplicationException innerException)
		{
			if (flag)
			{
				ReportOperationProcess(90, Resources.RestoringPreviousGroupFormat_Text, m_resource.Name);
				base.Cluster.MoveStorageToAvailableStorage(m_resource);
			}
			string[] array = new string[2];
			string text = ((m_quorumType != QuorumType.LegacyDisk) ? Resources.StorageWitnessSettingAsQuorumFailedFormat_Text : Resources.LegacyDiskSettingAsQuorumFailedFormat_Text);
			array[0] = text;
			array[1] = m_resource.Name;
			throw ExceptionHelp.Build<ApplicationException>(innerException, array);
		}
	}

	internal override void Cleanup()
	{
		try
		{
			if (!m_resource.IsQuorumResource && m_resource.GetOwnerGroup().GroupType == GroupType.CoreCluster)
			{
				ReportOperationProcess(50, Resources.MovingStorageBackToAvailableStorageFormat_Text, m_resource.Name);
				base.Cluster.MoveStorageToAvailableStorage(m_resource);
			}
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Storage quorum cleanup failed");
		}
	}

	internal static ClusterDiskPartition PickQuorumPartition(ClusterDisk disk)
	{
		ulong num = 0uL;
		ClusterDiskPartition result = null;
		foreach (ClusterDiskPartition partition in disk.Partitions)
		{
			if (!partition.IsNtfs && !partition.IsReFs)
			{
				continue;
			}
			if (partition.IsPartitionSizeValid)
			{
				ulong size = partition.Size;
				if (num < size)
				{
					num = size;
					result = partition;
				}
			}
			else
			{
				num = 0uL;
				result = partition;
			}
		}
		return result;
	}

	internal static StorageQuorumSettings CreateLegacyDiskQuorumSettings(ClusterResource resource)
	{
		return new StorageQuorumSettings(resource, QuorumType.LegacyDisk);
	}

	internal static StorageQuorumSettings CreateStorageWitnessQuorumSettings(ClusterResource resource)
	{
		return new StorageQuorumSettings(resource, QuorumType.StorageWitness);
	}

	public override void VerifySettings()
	{
		if (m_resource.State != ResourceState.Online)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[2]
			{
				Resources.StorageResourceNotOnlineFormat_Text,
				m_resource.Name
			});
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public override bool AreQuorumSettingsEqual(QuorumSettings settings)
	{
		bool flag = false;
		if (QuorumType == settings.QuorumType && settings is StorageQuorumSettings storageQuorumSettings)
		{
			Guid id = QuorumResource.Id;
			flag = storageQuorumSettings.QuorumResource.Id == id || flag;
		}
		return flag;
	}
}
