using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public sealed class PoolPhysicalDiskInfo : IDataItem
{
	private readonly Guid id = Guid.NewGuid();

	private readonly string diskNumber;

	public string DriveName => PoolPhysicalDiskInfoInternal.DriveName;

	public ulong? TotalCapacity => PoolPhysicalDiskInfoInternal.TotalCapacity;

	public ulong ConsumedCapacity => PoolPhysicalDiskInfoInternal.ConsumedCapacity;

	public StoragePoolHealth DriveHealth => PoolPhysicalDiskInfoInternal.DriveHealth;

	public StoragePhysicalDriveState DriveState => PoolPhysicalDiskInfoInternal.DriveState;

	public StoragePhysicalDriveUsage Usage => PoolPhysicalDiskInfoInternal.Usage;

	public PhysicalDiskBusType BusType => PoolPhysicalDiskInfoInternal.BusType;

	public int? Slot => PoolPhysicalDiskInfoInternal.Slot;

	public string EnclosureName => PoolPhysicalDiskInfoInternal.EnclosureName;

	public StoragePoolResource OwningResource { get; private set; }

	public string DisplayName => DriveName;

	public string Name => DisplayName;

	public IEnumerable<ICommand> Commands => GetCommands();

	public Guid Id => id;

	private PoolPhysicalDiskInfoInternal PoolPhysicalDiskInfoInternal { get; set; }

	internal PoolPhysicalDiskInfo(PoolPhysicalDiskInfoInternal poolPhysicalDiskInfoInternal, StoragePoolResource owningResoruce)
	{
		Exceptions.ThrowIfNull(poolPhysicalDiskInfoInternal, "poolPhysicalDiskInfoInternal");
		Exceptions.ThrowIfNull(owningResoruce, "owningResoruce");
		OwningResource = owningResoruce;
		PoolPhysicalDiskInfoInternal = poolPhysicalDiskInfoInternal;
		diskNumber = ((OwningResource.Cluster.OsVersion == OSVersion.Windows2012R2) ? ParseDriveName(DriveName) : string.Empty);
	}

	private IEnumerable<ICommand> GetCommands()
	{
		return new CommandCollection(ClusterCommandCollectionId.PoolPhysicalDisk)
		{
			new ClusterCommand(OwningResource, "Refresh", ClusterCommandId.RefreshPoolPhysicalDisks)
			{
				Text = CommonResources.FileShare_RefreshShare,
				CanExecuteDelegate = (object x) => true,
				ExecuteDelegate = delegate
				{
					OwningResource.RefreshPhysicalDisksInfo();
				},
				CommandParameter = null
			}
		};
	}

	private static string ParseDriveName(string driveName)
	{
		Match match = new Regex("\\d+").Match(driveName);
		if (match.Length > 0)
		{
			return match.Value;
		}
		return string.Empty;
	}
}
