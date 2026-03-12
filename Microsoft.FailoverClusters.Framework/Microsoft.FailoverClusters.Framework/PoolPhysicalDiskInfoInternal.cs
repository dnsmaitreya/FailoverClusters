using System;
using System.Globalization;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal class PoolPhysicalDiskInfoInternal
{
	internal string DriveName { get; private set; }

	internal ulong TotalCapacity { get; private set; }

	internal ulong ConsumedCapacity { get; private set; }

	internal StoragePoolHealth DriveHealth { get; private set; }

	internal StoragePhysicalDriveState DriveState { get; private set; }

	internal StoragePhysicalDriveUsage Usage { get; private set; }

	internal PhysicalDiskBusType BusType { get; private set; }

	internal int Slot { get; private set; }

	internal string EnclosureName { get; private set; }

	internal PoolPhysicalDiskInfoInternal(NativeMethods.CLUS_POOL_DRIVE_INFO poolDriveInfo)
	{
		DriveName = poolDriveInfo.DriveName;
		if (string.IsNullOrEmpty(DriveName))
		{
			DriveName = string.Empty;
		}
		TotalCapacity = poolDriveInfo.TotalCapacity;
		ConsumedCapacity = poolDriveInfo.ConsumeCapacity;
		DriveHealth = (StoragePoolHealth)Enum.Parse(typeof(StoragePoolHealth), poolDriveInfo.DriveHealth.ToString(CultureInfo.InvariantCulture));
		DriveState = (StoragePhysicalDriveState)Enum.Parse(typeof(StoragePhysicalDriveState), poolDriveInfo.DriveState.ToString(CultureInfo.InvariantCulture));
		Usage = (StoragePhysicalDriveUsage)Enum.Parse(typeof(StoragePhysicalDriveUsage), poolDriveInfo.Usage.ToString(CultureInfo.InvariantCulture));
		BusType = (PhysicalDiskBusType)Enum.Parse(typeof(PhysicalDiskBusType), poolDriveInfo.BusType.ToString(CultureInfo.InvariantCulture));
		Slot = poolDriveInfo.Slot;
		EnclosureName = poolDriveInfo.EnclosureName;
		if (string.IsNullOrEmpty(EnclosureName))
		{
			EnclosureName = string.Empty;
		}
	}
}
