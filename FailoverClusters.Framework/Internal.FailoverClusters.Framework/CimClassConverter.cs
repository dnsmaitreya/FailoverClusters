using FailoverClusters.Framework;

namespace MS.Internal.FailoverClusters.Framework;

internal static class CimClassConverter
{
	public static StorageNode ToStorageNode(this MSFT_StorageNode miStorageNode, Cluster cluster)
	{
		if (miStorageNode == null)
		{
			return null;
		}
		return new StorageNode(cluster)
		{
			ObjectId = miStorageNode.ObjectId,
			Name = miStorageNode.Name,
			NameFormat = miStorageNode.NameFormat.ValueOrDefault(StorageNodeNameFormat.Unknown),
			OperationalStatus = miStorageNode.OperationalStatus.ValueOrDefault(StorageNodeOperationalStatus.Unknown),
			OtherIdentifyingInfo = miStorageNode.OtherIdentifyingInfo,
			OtherIdentifyingInfoDescription = miStorageNode.OtherIdentifyingInfoDescription
		};
	}

	public static Enclosure ToEnclosure(this MSFT_StorageEnclosure miEnclosure, Cluster cluster)
	{
		if (miEnclosure == null)
		{
			return null;
		}
		return new Enclosure(cluster)
		{
			ObjectId = miEnclosure.ObjectId,
			DeviceId = miEnclosure.DeviceId,
			FanOperationalStatus = miEnclosure.FanOperationalStatus.ToArrayOf<FanOperationalStatus>(),
			FirmwareVersion = miEnclosure.FirmwareVersion,
			FriendlyName = miEnclosure.FriendlyName,
			HealthStatus = miEnclosure.HealthStatus.ValueOrDefault(EnclosureHealthStatus.Unknown),
			IOControllerOperationalStatus = miEnclosure.IOControllerOperationalStatus.ToArrayOf<IOControllerOperationalStatus>(),
			Manufacturer = miEnclosure.Manufacturer,
			Model = miEnclosure.Model,
			PowerSupplyOperationalStatus = miEnclosure.PowerSupplyOperationalStatus.ToArrayOf<PowerSupplyOperationalStatus>(),
			SerialNumber = (miEnclosure.SerialNumber ?? string.Empty),
			NumberOfSlots = miEnclosure.NumberOfSlots.ValueOrDefault<uint>(0u),
			TemperatureSensorOperationalStatus = miEnclosure.TemperatureSensorOperationalStatus.ToArrayOf<TemperatureSensorOperationalStatus>(),
			VoltageSensorOperationalStatus = miEnclosure.VoltageSensorOperationalStatus.ToArrayOf<VoltageSensorOperationalStatus>()
		};
	}

	public static PhysicalDiskInfo ToPhysicalDiskInfo(this MSFT_PhysicalDisk miPhysicalDisk, Cluster cluster)
	{
		if (miPhysicalDisk == null)
		{
			return null;
		}
		return new PhysicalDiskInfo(cluster)
		{
			EnclosureName = string.Empty,
			ObjectId = miPhysicalDisk.ObjectId,
			AllocatedSize = miPhysicalDisk.AllocatedSize.ValueOrDefault<ulong>(0uL),
			BusType = miPhysicalDisk.BusType.ValueOrDefault(PhysicalDiskBusType.Unknown),
			CannotPoolReason = miPhysicalDisk.CannotPoolReason.ToArrayOf<CannotPoolReason>(),
			CanPool = miPhysicalDisk.CanPool.ValueOrDefault<bool>(defaultValue: false),
			Description = miPhysicalDisk.Description,
			DeviceId = miPhysicalDisk.DeviceId,
			EnclosureNumber = miPhysicalDisk.EnclosureNumber.ValueOrDefault<ushort>((ushort)0),
			FirmwareVersion = miPhysicalDisk.FirmwareVersion,
			FriendlyName = miPhysicalDisk.FriendlyName,
			HealthStatus = miPhysicalDisk.HealthStatus.ValueOrDefault(PhysicalDiskHealthStatus.Unknown),
			IsIndicationEnabled = miPhysicalDisk.IsIndicationEnabled.ValueOrDefault<bool>(defaultValue: false),
			IsPartial = miPhysicalDisk.IsPartial.ValueOrDefault<bool>(defaultValue: false),
			LogicalSectorSize = miPhysicalDisk.LogicalSectorSize.ValueOrDefault<ulong>(0uL),
			Manufacturer = miPhysicalDisk.Manufacturer,
			MediaType = miPhysicalDisk.MediaType.ValueOrDefault(MediaType.Unspecified),
			Model = miPhysicalDisk.Model,
			OperationalStatus = miPhysicalDisk.OperationalStatus.ToArrayOf<PhysicalDiskOperationalStatus>(),
			OtherCannotPoolReasonDescription = miPhysicalDisk.OtherCannotPoolReasonDescription,
			PartNumber = miPhysicalDisk.PartNumber,
			PhysicalLocation = miPhysicalDisk.PhysicalLocation,
			PhysicalSectorSize = miPhysicalDisk.PhysicalSectorSize.ValueOrDefault<ulong>(0uL),
			SerialNumber = miPhysicalDisk.SerialNumber.TrimOrDefault(),
			Size = miPhysicalDisk.Size.ValueOrDefault<ulong>(0uL),
			SlotNumber = miPhysicalDisk.SlotNumber.ValueOrDefault<ushort>((ushort)0),
			SoftwareVersion = miPhysicalDisk.SoftwareVersion,
			SpindleSpeed = miPhysicalDisk.SpindleSpeed.ValueOrDefault<uint>(0u),
			SupportedUsages = miPhysicalDisk.SupportedUsages.ToArrayOf<PhysicalDiskUsage>(),
			UniqueIdFormat = miPhysicalDisk.UniqueIdFormat.ValueOrDefault(UniqueIdFormat.VendorSpecific),
			Usage = miPhysicalDisk.Usage.ValueOrDefault(PhysicalDiskUsage.Unknown)
		};
	}

	public static VirtualDiskInfo ToVirtualDiskInfo(this MSFT_VirtualDisk miVirtualDisk, Cluster cluster)
	{
		if (miVirtualDisk == null)
		{
			return null;
		}
		return new VirtualDiskInfo(cluster)
		{
			FriendlyName = miVirtualDisk.FriendlyName,
			Name = miVirtualDisk.Name,
			NameFormat = miVirtualDisk.NameFormat.ValueOrDefault(VirtualDiskNameFormat.Unknown),
			UniqueIdFormat = miVirtualDisk.UniqueIdFormat.ValueOrDefault(UniqueIdFormat.VendorSpecific),
			UniqueIdFormatDescription = miVirtualDisk.UniqueIdFormatDescription,
			Usage = miVirtualDisk.Usage.ValueOrDefault(VirtualDiskUsage.Unknown),
			OtherUsageDescription = miVirtualDisk.OtherUsageDescription,
			HealthStatus = miVirtualDisk.HealthStatus.ValueOrDefault(VirtualDiskHealthStatus.Unknown),
			OperationalStatus = miVirtualDisk.OperationalStatus.ToArrayOf<VirtualDiskOperationalStatus>(),
			OtherOperationalStatusDescription = miVirtualDisk.OtherOperationalStatusDescription,
			ResiliencySettingName = miVirtualDisk.ResiliencySettingName,
			Size = miVirtualDisk.Size.ValueOrDefault<ulong>(0uL),
			AllocatedSize = miVirtualDisk.AllocatedSize.ValueOrDefault<ulong>(0uL),
			LogicalSectorSize = miVirtualDisk.LogicalSectorSize.ValueOrDefault<ulong>(0uL),
			PhysicalSectorSize = miVirtualDisk.PhysicalSectorSize.ValueOrDefault<ulong>(0uL),
			FootprintOnPool = miVirtualDisk.FootprintOnPool.ValueOrDefault<ulong>(0uL),
			ProvisioningType = miVirtualDisk.ProvisioningType.ValueOrDefault(VirtualDiskProvisioningType.Unknown),
			NumberOfDataCopies = miVirtualDisk.NumberOfDataCopies.ValueOrDefault<ushort>((ushort)0),
			PhysicalDiskRedundancy = miVirtualDisk.PhysicalDiskRedundancy.ValueOrDefault<ushort>((ushort)0),
			ParityLayout = miVirtualDisk.ParityLayout.ValueOrDefault(VirtualDiskParityLayout.None),
			NumberOfColumns = miVirtualDisk.NumberOfColumns.ValueOrDefault<ushort>((ushort)0),
			Interleave = miVirtualDisk.Interleave.ValueOrDefault<ulong>(0uL),
			RequestNoSinglePointOfFailure = miVirtualDisk.RequestNoSinglePointOfFailure.ValueOrDefault<bool>(defaultValue: false),
			Access = miVirtualDisk.Access.ValueOrDefault(VirtualDiskAccess.Unknown),
			IsSnapshot = miVirtualDisk.IsSnapshot.ValueOrDefault<bool>(defaultValue: false),
			IsManualAttach = miVirtualDisk.IsManualAttach.ValueOrDefault<bool>(defaultValue: false),
			IsDeduplicationEnabled = miVirtualDisk.IsDeduplicationEnabled.ValueOrDefault<bool>(defaultValue: false),
			IsEnclosureAware = miVirtualDisk.IsEnclosureAware.ValueOrDefault<bool>(defaultValue: false),
			NumberOfAvailableCopies = miVirtualDisk.NumberOfAvailableCopies.ValueOrDefault<ushort>((ushort)0),
			DetachedReason = miVirtualDisk.DetachedReason.ValueOrDefault(VirtualDiskDetachedReason.Unknown),
			WriteCacheSize = miVirtualDisk.WriteCacheSize.ValueOrDefault<ulong>(0uL),
			ObjectId = miVirtualDisk.ObjectId
		};
	}

	public static MsftDiskInfo ToMsftDiskInfo(this MSFT_Disk miDisk, Cluster cluster)
	{
		if (miDisk == null)
		{
			return null;
		}
		return new MsftDiskInfo(cluster)
		{
			EnclosureName = "Test Enclosure",
			Rack = "Test Rack",
			ObjectId = miDisk.ObjectId,
			FriendlyName = miDisk.FriendlyName,
			PhysicalSectorSize = miDisk.PhysicalSectorSize.ValueOrDefault<uint>(0u),
			AllocatedSize = miDisk.AllocatedSize.ValueOrDefault<ulong>(0uL),
			UniqueIdFormat = miDisk.UniqueIdFormat.ValueOrDefault(UniqueIdFormat.VendorSpecific),
			LogicalSectorSize = miDisk.LogicalSectorSize.ValueOrDefault<uint>(0u),
			Size = miDisk.Size.ValueOrDefault<ulong>(0uL),
			BusType = miDisk.BusType.ValueOrDefault(PhysicalDiskBusType.Unknown),
			FirmwareVersion = miDisk.FirmwareVersion,
			HealthStatus = miDisk.HealthStatus.ValueOrDefault(DiskHealthStatus.Unknown),
			Manufacturer = miDisk.Manufacturer,
			Model = miDisk.Model,
			OperationalStatus = (DiskOperationalStatus)((miDisk.OperationalStatus != null) ? miDisk.OperationalStatus[0] : 0),
			SerialNumber = miDisk.SerialNumber.TrimOrDefault(),
			Path = miDisk.Path,
			Location = miDisk.Location,
			Number = miDisk.Number.ValueOrDefault<uint>(0u),
			LargestFreeExtent = miDisk.LargestFreeExtent.ValueOrDefault<ulong>(0uL),
			NumberOfPartitions = miDisk.NumberOfPartitions.ValueOrDefault<uint>(0u),
			ProvisioningType = miDisk.ProvisioningType.ValueOrDefault(DiskProvisioningType.Unknown),
			PartitionStyle = miDisk.PartitionStyle.ValueOrDefault(DiskPartitionStyle.Unknown),
			Signature = miDisk.Signature.ValueOrDefault<uint>(0u),
			Guid = miDisk.Guid,
			IsOffline = miDisk.IsOffline.ValueOrDefault<bool>(defaultValue: false),
			OfflineReason = miDisk.OfflineReason.ValueOrDefault(DiskOfflineReason.Unknown),
			IsSystem = miDisk.IsSystem.ValueOrDefault<bool>(defaultValue: false),
			IsClustered = miDisk.IsClustered.ValueOrDefault<bool>(defaultValue: false),
			IsBoot = miDisk.IsBoot.ValueOrDefault<bool>(defaultValue: false),
			BootFromDisk = miDisk.BootFromDisk.ValueOrDefault<bool>(defaultValue: false)
		};
	}

	public static MsftPartitionInfo ToMsftDiskPartitionInfo(this MSFT_Partition miPartition, Cluster cluster)
	{
		if (miPartition == null)
		{
			return null;
		}
		return new MsftPartitionInfo(cluster)
		{
			AccessPaths = miPartition.AccessPaths,
			DiskId = miPartition.DiskId,
			DiskNumber = miPartition.DiskNumber.ValueOrDefault<uint>(0u),
			DriveLetter = miPartition.DriveLetter.ValueOrDefault<char>('\0'),
			IsActive = miPartition.IsActive.ValueOrDefault<bool>(defaultValue: false),
			IsBoot = miPartition.IsBoot.ValueOrDefault<bool>(defaultValue: false),
			IsHidden = miPartition.IsHidden.ValueOrDefault<bool>(defaultValue: false),
			IsOffline = miPartition.IsOffline.ValueOrDefault<bool>(defaultValue: false),
			IsReadOnly = miPartition.IsReadOnly.ValueOrDefault<bool>(defaultValue: false),
			IsShadowCopy = miPartition.IsShadowCopy.ValueOrDefault<bool>(defaultValue: false),
			IsSystem = miPartition.IsSystem.ValueOrDefault<bool>(defaultValue: false),
			MbrType = miPartition.MbrType.ValueOrDefault(PartitionMbrType.Unknown),
			NoDefaultDriveLetter = miPartition.NoDefaultDriveLetter.ValueOrDefault<bool>(defaultValue: false),
			Offset = miPartition.Offset.ValueOrDefault<ulong>(0uL),
			OperationalStatus = miPartition.OperationalStatus.ValueOrDefault(PartitionOperationalStatus.Unknown),
			PartitionNumber = miPartition.PartitionNumber.ValueOrDefault<uint>(0u),
			Size = miPartition.Size.ValueOrDefault<ulong>(0uL),
			TransitionState = miPartition.TransitionState.ValueOrDefault(PartitionTransitionState.ReservedForFutureUse)
		};
	}

	public static MsftVolumeInfo ToMsftVolumeInfo(this MSFT_Volume miVolume, Cluster cluster)
	{
		if (miVolume == null)
		{
			return null;
		}
		return new MsftVolumeInfo(cluster)
		{
			ObjectId = miVolume.ObjectId,
			DriveLetter = miVolume.DriveLetter.ValueOrDefault<char>('\0'),
			Path = miVolume.Path,
			HealthStatus = miVolume.HealthStatus.ValueOrDefault(VolumeHealthStatus.Unknown),
			FileSystem = miVolume.FileSystem,
			FileSystemLabel = miVolume.FileSystemLabel,
			Size = miVolume.Size.ValueOrDefault<ulong>(0uL),
			SizeRemaining = miVolume.SizeRemaining.ValueOrDefault<ulong>(0uL),
			DriveType = miVolume.DriveType.ValueOrDefault(VolumeDriveType.Unknown)
		};
	}
}

