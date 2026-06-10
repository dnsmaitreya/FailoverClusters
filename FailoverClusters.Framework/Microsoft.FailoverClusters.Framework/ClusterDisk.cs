using System;
using System.Collections.Generic;

namespace FailoverClusters.Framework;

public class ClusterDisk
{
	private ClusterDiskId diskId;

	public string DiskId
	{
		get
		{
			if (diskId == null)
			{
				return null;
			}
			return diskId.ToString();
		}
	}

	public ClusterDiskId ClusterDiskId => diskId;

	public uint? DiskNumber { get; set; }

	public List<ClusterDiskPartition> Partitions { get; private set; }

	public string Name { get; set; }

	public ulong? Size { get; set; }

	public StorageResource OwnerResource { get; set; }

	public PartitionStyle PartitionStyle { get; private set; }

	public ClusterDisk()
	{
		Size = 0uL;
		Partitions = new List<ClusterDiskPartition>();
		PartitionStyle = PartitionStyle.Raw;
	}

	public ClusterDisk(uint signature)
		: this()
	{
		diskId = ClusterDiskId.CreateDiskSignature(signature);
		PartitionStyle = PartitionStyle.MasterBootRecord;
	}

	public ClusterDisk(Guid diskId)
		: this()
	{
		this.diskId = ClusterDiskId.CreateDiskGuid(diskId);
		PartitionStyle = PartitionStyle.GuidPartitionTable;
	}

	public ClusterDisk Clone()
	{
		ClusterDisk clusterDisk = new ClusterDisk
		{
			diskId = diskId,
			Name = Name,
			DiskNumber = DiskNumber,
			Size = Size,
			OwnerResource = OwnerResource,
			PartitionStyle = PartitionStyle
		};
		foreach (ClusterDiskPartition partition in Partitions)
		{
			ClusterDiskPartition clusterDiskPartition = new ClusterDiskPartition(clusterDisk)
			{
				FileSystem = partition.FileSystem,
				FreeSpace = partition.FreeSpace,
				IsCompressed = partition.IsCompressed,
				IsDirty = partition.IsDirty,
				Name = partition.Name,
				Label = partition.Label,
				PartitionNumber = partition.PartitionNumber,
				Path = partition.Path,
				Size = partition.Size,
				VolumeGuid = partition.VolumeGuid,
				VolumeGuidPath = partition.VolumeGuidPath,
				IsMaintenanceModeOn = partition.IsMaintenanceModeOn,
				CsvFaultState = partition.CsvFaultState,
				Flags = partition.Flags,
				SerialNumber = partition.SerialNumber,
				DeviceNumber = partition.DeviceNumber,
				GptPartitionId = partition.GptPartitionId,
				PartitionName = partition.PartitionName,
				BitLockerFlags = partition.BitLockerFlags
			};
			clusterDiskPartition.SetMountPoints(partition.MountPoints);
			clusterDisk.Partitions.Add(clusterDiskPartition);
		}
		return clusterDisk;
	}
}

