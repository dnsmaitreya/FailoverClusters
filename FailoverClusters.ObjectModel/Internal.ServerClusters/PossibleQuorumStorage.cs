using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class PossibleQuorumStorage
{
	private PossibleQuorumStorageStatus m_status;

	private ClusterResource m_resource;

	private ClusterDiskPartition m_bestPartition;

	private bool m_isAsymmetric;

	public bool IsAsymmetric
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_isAsymmetric;
		}
		[param: MarshalAs(UnmanagedType.U1)]
		internal set
		{
			m_isAsymmetric = value;
		}
	}

	public ClusterDiskPartition BestPartition => m_bestPartition;

	public ClusterResource Resource => m_resource;

	public PossibleQuorumStorageStatus Status => m_status;

	private PossibleQuorumStorageStatus DeterminePossibleQuorumStatus()
	{
		if (!m_resource.IsValidForQuorumResource)
		{
			return PossibleQuorumStorageStatus.TypeNotValidForQuorum;
		}
		string resourceType = "Physical Disk";
		if (m_resource.IsResourceOfType(resourceType) && m_resource.PhysicalDisk_IsMaintenanceModeOn())
		{
			return PossibleQuorumStorageStatus.MaintenanceMode;
		}
		if (m_resource.IsClusterSharedVolume)
		{
			return PossibleQuorumStorageStatus.ClusterSharedVolume;
		}
		if (ResourceState.Online != m_resource.State)
		{
			return PossibleQuorumStorageStatus.NotOnline;
		}
		if (m_resource.HasDependencies())
		{
			return PossibleQuorumStorageStatus.HasDependencies;
		}
		ClusterDisk clusterDisk = m_resource.Storage_GetDiskInfo(includeMountPoints: false);
		if (clusterDisk == null)
		{
			return PossibleQuorumStorageStatus.NoPartitionInformation;
		}
		ClusterDiskPartition clusterDiskPartition = (m_bestPartition = StorageQuorumSettings.PickQuorumPartition(clusterDisk));
		if (clusterDiskPartition == null)
		{
			return PossibleQuorumStorageStatus.NoNtfsPartition;
		}
		if (clusterDiskPartition.IsPartitionSizeValid && m_bestPartition.Size < 52428800)
		{
			return PossibleQuorumStorageStatus.NtfsPartitionTooSmall;
		}
		return PossibleQuorumStorageStatus.Valid;
	}

	internal PossibleQuorumStorage(ClusterResource resource)
	{
		m_resource = resource;
		m_bestPartition = null;
		m_status = DeterminePossibleQuorumStatus();
	}
}
