using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class MsftDiskInfo : DiskInfoBase
{
	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.DiskInfo);

	private Icon2 icon;

	private bool? isClusterSharedVolume;

	private int isClusterSharedVolumeInfoLoading;

	private ObservableDiskPartitionCollection partitionCollection;

	private readonly List<ObservableVolumeCollection> volumes = new List<ObservableVolumeCollection>();

	private readonly List<string> csvVolumeNames = new List<string>();

	public Icon2 Icon
	{
		get
		{
			Icon2 obj = icon ?? new Icon2(InvariantResources.PhysicalDisk);
			Icon2 result = obj;
			icon = obj;
			return result;
		}
	}

	public override IEnumerable<ICommand> Commands => commands;

	public string Path { get; internal set; }

	public string Location { get; internal set; }

	public string Manufacturer { get; internal set; }

	public string Model { get; internal set; }

	public string SerialNumber { get; internal set; }

	public string FirmwareVersion { get; internal set; }

	public DiskOperationalStatus OperationalStatus { get; internal set; }

	public uint Number { get; internal set; }

	public ulong LargestFreeExtent { get; internal set; }

	public uint NumberOfPartitions { get; internal set; }

	public DiskProvisioningType ProvisioningType { get; internal set; }

	public PhysicalDiskBusType BusType { get; internal set; }

	public DiskPartitionStyle PartitionStyle { get; internal set; }

	public uint Signature { get; internal set; }

	public string Guid { get; internal set; }

	public bool IsOffline { get; internal set; }

	public DiskOfflineReason OfflineReason { get; internal set; }

	public bool IsSystem { get; internal set; }

	public bool IsClustered { get; internal set; }

	public bool IsBoot { get; internal set; }

	public bool BootFromDisk { get; internal set; }

	public DiskHealthStatus HealthStatus { get; internal set; }

	public string EnclosureName { get; internal set; }

	public string Rack { get; internal set; }

	public bool? IsClusterSharedVolume
	{
		get
		{
			if (!isClusterSharedVolume.HasValue && Interlocked.CompareExchange(ref isClusterSharedVolumeInfoLoading, 1, 0) == 0)
			{
				Partitions.ForceEnumeration();
			}
			return isClusterSharedVolume;
		}
		internal set
		{
			isClusterSharedVolume = value;
			OnPropertyChanged("IsClusterSharedVolume");
		}
	}

	public ObservableDiskPartitionCollection Partitions
	{
		get
		{
			partitionCollection = ObservableDiskPartitionCollection.GetAssociation(this, base.Cluster, this);
			partitionCollection.OnEnumerationComplete(PartitionsLoaded);
			return partitionCollection;
		}
	}

	public MsftDiskInfo(ClusterObject owner)
		: base(owner)
	{
	}

	public static void GetInstance(Cluster cluster, string key, Action<OperationResult<MsftDiskInfo>> operationResult, string serverName = null)
	{
		ObservableCollectionItem<IDiskInfo>.GetInstance(cluster, key, operationResult, serverName);
	}

	public void CopyFrom(MsftDiskInfo source)
	{
		Exceptions.ThrowIfNull(source, "source");
		CopyFrom((IDiskInfo)source);
		if (Path != source.Path)
		{
			Path = source.Path;
			OnPropertyChanged("Path");
		}
		if (Location != source.Location)
		{
			Location = source.Location;
			OnPropertyChanged("Location");
		}
		if (Manufacturer != source.Manufacturer)
		{
			Manufacturer = source.Manufacturer;
			OnPropertyChanged("Manufacturer");
		}
		if (Model != source.Model)
		{
			Model = source.Model;
			OnPropertyChanged("Model");
		}
		if (SerialNumber != source.SerialNumber)
		{
			SerialNumber = source.SerialNumber;
			OnPropertyChanged("SerialNumber");
		}
		if (FirmwareVersion != source.FirmwareVersion)
		{
			FirmwareVersion = source.FirmwareVersion;
			OnPropertyChanged("FirmwareVersion");
		}
		if (OperationalStatus != source.OperationalStatus)
		{
			OperationalStatus = source.OperationalStatus;
			OnPropertyChanged("OperationalStatus");
		}
		if (Number != source.Number)
		{
			Number = source.Number;
			OnPropertyChanged("Number");
		}
		if (LargestFreeExtent != source.LargestFreeExtent)
		{
			LargestFreeExtent = source.LargestFreeExtent;
			OnPropertyChanged("LargestFreeExtent");
		}
		if (ProvisioningType != source.ProvisioningType)
		{
			ProvisioningType = source.ProvisioningType;
			OnPropertyChanged("ProvisioningType");
		}
		if (NumberOfPartitions != source.NumberOfPartitions)
		{
			NumberOfPartitions = source.NumberOfPartitions;
			OnPropertyChanged("NumberOfPartitions");
		}
		if (BusType != source.BusType)
		{
			BusType = source.BusType;
			OnPropertyChanged("BusType");
		}
		if (PartitionStyle != source.PartitionStyle)
		{
			PartitionStyle = source.PartitionStyle;
			OnPropertyChanged("PartitionStyle");
		}
		if (Signature != source.Signature)
		{
			Signature = source.Signature;
			OnPropertyChanged("Signature");
		}
		if (Guid != source.Guid)
		{
			Guid = source.Guid;
			OnPropertyChanged("Guid");
		}
		if (IsOffline != source.IsOffline)
		{
			IsOffline = source.IsOffline;
			OnPropertyChanged("IsOffline");
		}
		if (OfflineReason != source.OfflineReason)
		{
			OfflineReason = source.OfflineReason;
			OnPropertyChanged("OfflineReason");
		}
		if (IsSystem != source.IsSystem)
		{
			IsSystem = source.IsSystem;
			OnPropertyChanged("IsSystem");
		}
		if (IsClustered != source.IsClustered)
		{
			IsClustered = source.IsClustered;
			OnPropertyChanged("IsClustered");
		}
		if (IsBoot != source.IsBoot)
		{
			IsBoot = source.IsBoot;
			OnPropertyChanged("IsBoot");
		}
		if (BootFromDisk != source.BootFromDisk)
		{
			BootFromDisk = source.BootFromDisk;
			OnPropertyChanged("BootFromDisk");
		}
		if (HealthStatus != source.HealthStatus)
		{
			HealthStatus = source.HealthStatus;
			OnPropertyChanged("BootHealthStatusFromDisk");
		}
		if (EnclosureName != source.EnclosureName)
		{
			EnclosureName = source.EnclosureName;
			OnPropertyChanged("EnclosureName");
		}
		if (Rack != source.Rack)
		{
			Rack = source.Rack;
			OnPropertyChanged("Rack");
		}
	}

	private void PartitionsLoaded(ObservableKeyCollection<MsftPartitionInfo> partitions)
	{
		List<MsftPartitionInfo> partitionsList = partitions.Where((MsftPartitionInfo partition) => partition.AccessPaths != null).ToList();
		if (partitionsList.Count == 0)
		{
			IsClusterSharedVolume = false;
			return;
		}
		int callbackNum = 0;
		foreach (MsftPartitionInfo item in partitionsList)
		{
			ObservableVolumeCollection association = ObservableVolumeCollection.GetAssociation(item, base.Cluster, item);
			volumes.Add(association);
			Action<ObservableKeyCollection<MsftVolumeInfo>> callback = delegate(ObservableKeyCollection<MsftVolumeInfo> collection)
			{
				try
				{
					if (collection.Any((MsftVolumeInfo volume) => volume.IsCsvFS))
					{
						IsClusterSharedVolume = true;
					}
				}
				finally
				{
					if (++callbackNum == partitionsList.Count)
					{
						if (!IsClusterSharedVolume.HasValue)
						{
							IsClusterSharedVolume = false;
						}
						volumes.Clear();
					}
				}
			};
			association.OnEnumerationComplete(callback);
			association.ForceEnumeration();
		}
	}
}
