using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class VirtualDiskInfo : DiskInfoBase
{
	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.DiskInfo);

	public override IEnumerable<ICommand> Commands => commands;

	public string Name { get; internal set; }

	public VirtualDiskNameFormat NameFormat { get; internal set; }

	public VirtualDiskUsage Usage { get; internal set; }

	public string UniqueIdFormatDescription { get; internal set; }

	public string OtherUsageDescription { get; internal set; }

	public IList<VirtualDiskOperationalStatus> OperationalStatus { get; internal set; }

	public string OtherOperationalStatusDescription { get; internal set; }

	public string ResiliencySettingName { get; internal set; }

	public VirtualDiskHealthStatus HealthStatus { get; internal set; }

	public ulong FootprintOnPool { get; internal set; }

	public VirtualDiskProvisioningType ProvisioningType { get; internal set; }

	public ushort NumberOfDataCopies { get; internal set; }

	public ushort PhysicalDiskRedundancy { get; internal set; }

	public VirtualDiskParityLayout ParityLayout { get; internal set; }

	public ushort NumberOfColumns { get; internal set; }

	public ulong Interleave { get; internal set; }

	public bool RequestNoSinglePointOfFailure { get; internal set; }

	public VirtualDiskAccess Access { get; internal set; }

	public bool IsSnapshot { get; internal set; }

	public bool IsManualAttach { get; internal set; }

	public bool IsDeduplicationEnabled { get; internal set; }

	public bool IsEnclosureAware { get; internal set; }

	public ushort NumberOfAvailableCopies { get; internal set; }

	public VirtualDiskDetachedReason DetachedReason { get; internal set; }

	public ulong WriteCacheSize { get; internal set; }

	public VirtualDiskInfo(ClusterObject owner)
		: base(owner)
	{
	}

	public static void GetInstance(Cluster cluster, string key, Action<OperationResult<VirtualDiskInfo>> operationResult, string serverName = null)
	{
		ObservableCollectionItem<IDiskInfo>.GetInstance(cluster, key, operationResult, serverName);
	}

	public void CopyFrom(VirtualDiskInfo source)
	{
		Exceptions.ThrowIfNull(source, "source");
		CopyFrom((IDiskInfo)source);
		if (Name != source.Name)
		{
			Name = source.Name;
			OnPropertyChanged("Name");
		}
		if (NameFormat != source.NameFormat)
		{
			NameFormat = source.NameFormat;
			OnPropertyChanged("NameFormat");
		}
		if (Usage != source.Usage)
		{
			Usage = source.Usage;
			OnPropertyChanged("Usage");
		}
		if (UniqueIdFormatDescription != source.UniqueIdFormatDescription)
		{
			UniqueIdFormatDescription = source.UniqueIdFormatDescription;
			OnPropertyChanged("UniqueIdFormatDescription");
		}
		if (OtherUsageDescription != source.OtherUsageDescription)
		{
			OtherUsageDescription = source.OtherUsageDescription;
			OnPropertyChanged("OtherUsageDescription");
		}
		if (OperationalStatus != source.OperationalStatus)
		{
			OperationalStatus = source.OperationalStatus;
			OnPropertyChanged("OperationalStatus");
		}
		if (OtherOperationalStatusDescription != source.OtherOperationalStatusDescription)
		{
			OtherOperationalStatusDescription = source.OtherOperationalStatusDescription;
			OnPropertyChanged("OtherOperationalStatusDescription");
		}
		if (ResiliencySettingName != source.ResiliencySettingName)
		{
			ResiliencySettingName = source.ResiliencySettingName;
			OnPropertyChanged("ResiliencySettingName");
		}
		if (HealthStatus != source.HealthStatus)
		{
			HealthStatus = source.HealthStatus;
			OnPropertyChanged("HealthStatus");
		}
		if (FootprintOnPool != source.FootprintOnPool)
		{
			FootprintOnPool = source.FootprintOnPool;
			OnPropertyChanged("FootprintOnPool");
		}
		if (ProvisioningType != source.ProvisioningType)
		{
			ProvisioningType = source.ProvisioningType;
			OnPropertyChanged("ProvisioningType");
		}
		if (NumberOfDataCopies != source.NumberOfDataCopies)
		{
			NumberOfDataCopies = source.NumberOfDataCopies;
			OnPropertyChanged("NumberOfDataCopies");
		}
		if (PhysicalDiskRedundancy != source.PhysicalDiskRedundancy)
		{
			PhysicalDiskRedundancy = source.PhysicalDiskRedundancy;
			OnPropertyChanged("PhysicalDiskRedundancy");
		}
		if (ParityLayout != source.ParityLayout)
		{
			ParityLayout = source.ParityLayout;
			OnPropertyChanged("ParityLayout");
		}
		if (NumberOfColumns != source.NumberOfColumns)
		{
			NumberOfColumns = source.NumberOfColumns;
			OnPropertyChanged("NumberOfColumns");
		}
		if (Interleave != source.Interleave)
		{
			Interleave = source.Interleave;
			OnPropertyChanged("Interleave");
		}
		if (RequestNoSinglePointOfFailure != source.RequestNoSinglePointOfFailure)
		{
			RequestNoSinglePointOfFailure = source.RequestNoSinglePointOfFailure;
			OnPropertyChanged("RequestNoSinglePointOfFailure");
		}
		if (Access != source.Access)
		{
			Access = source.Access;
			OnPropertyChanged("Access");
		}
		if (IsSnapshot != source.IsSnapshot)
		{
			IsSnapshot = source.IsSnapshot;
			OnPropertyChanged("IsSnapshot");
		}
		if (IsManualAttach != source.IsManualAttach)
		{
			IsManualAttach = source.IsManualAttach;
			OnPropertyChanged("IsManualAttach");
		}
		if (IsDeduplicationEnabled != source.IsDeduplicationEnabled)
		{
			IsDeduplicationEnabled = source.IsDeduplicationEnabled;
			OnPropertyChanged("IsDeduplicationEnabled");
		}
		if (IsEnclosureAware != source.IsEnclosureAware)
		{
			IsEnclosureAware = source.IsEnclosureAware;
			OnPropertyChanged("IsEnclosureAware");
		}
		if (NumberOfAvailableCopies != source.NumberOfAvailableCopies)
		{
			NumberOfAvailableCopies = source.NumberOfAvailableCopies;
			OnPropertyChanged("NumberOfAvailableCopies");
		}
		if (WriteCacheSize != source.WriteCacheSize)
		{
			WriteCacheSize = source.WriteCacheSize;
			OnPropertyChanged("WriteCacheSize");
		}
	}
}
