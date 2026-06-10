using System;
using System.Collections.Generic;
using System.Windows.Input;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class PhysicalDiskInfo : DiskInfoBase
{
	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.DiskInfo);

	public override IEnumerable<ICommand> Commands => commands;

	public PhysicalDiskUsage Usage { get; internal set; }

	public PhysicalDiskUsage[] SupportedUsages { get; internal set; }

	public string Description { get; internal set; }

	public string Manufacturer { get; internal set; }

	public string Model { get; internal set; }

	public string SerialNumber { get; internal set; }

	public string PartNumber { get; internal set; }

	public string FirmwareVersion { get; internal set; }

	public string SoftwareVersion { get; internal set; }

	public IList<PhysicalDiskOperationalStatus> OperationalStatus { get; internal set; }

	public PhysicalDiskBusType BusType { get; internal set; }

	public uint SpindleSpeed { get; internal set; }

	public bool IsIndicationEnabled { get; internal set; }

	public string PhysicalLocation { get; internal set; }

	public ushort EnclosureNumber { get; internal set; }

	public ushort SlotNumber { get; internal set; }

	public bool CanPool { get; internal set; }

	public IList<CannotPoolReason> CannotPoolReason { get; internal set; }

	public string OtherCannotPoolReasonDescription { get; internal set; }

	public bool IsPartial { get; internal set; }

	public MediaType MediaType { get; internal set; }

	public string EnclosureName { get; internal set; }

	public PhysicalDiskHealthStatus HealthStatus { get; internal set; }

	public PhysicalDiskInfo(ClusterObject owner)
		: base(owner)
	{
	}

	public static void GetInstance(Cluster cluster, string key, Action<OperationResult<PhysicalDiskInfo>> operationResult, string serverName = null)
	{
		ObservableCollectionItem<IDiskInfo>.GetInstance(cluster, key, operationResult, serverName);
	}

	public void CopyFrom(PhysicalDiskInfo source)
	{
		Exceptions.ThrowIfNull(source, "source");
		CopyFrom((IDiskInfo)source);
		if (Usage != source.Usage)
		{
			Usage = source.Usage;
			OnPropertyChanged("Usage");
		}
		if (SupportedUsages != source.SupportedUsages)
		{
			SupportedUsages = source.SupportedUsages;
			OnPropertyChanged("SupportedUsages");
		}
		if (Description != source.Description)
		{
			Description = source.Description;
			OnPropertyChanged("Description");
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
		if (PartNumber != source.PartNumber)
		{
			PartNumber = source.PartNumber;
			OnPropertyChanged("PartNumber");
		}
		if (FirmwareVersion != source.FirmwareVersion)
		{
			FirmwareVersion = source.FirmwareVersion;
			OnPropertyChanged("FirmwareVersion");
		}
		if (SoftwareVersion != source.SoftwareVersion)
		{
			SoftwareVersion = source.SoftwareVersion;
			OnPropertyChanged("SoftwareVersion");
		}
		if (OperationalStatus != source.OperationalStatus)
		{
			OperationalStatus = source.OperationalStatus;
			OnPropertyChanged("OperationalStatus");
		}
		if (BusType != source.BusType)
		{
			BusType = source.BusType;
			OnPropertyChanged("BysType");
		}
		if (SpindleSpeed != source.SpindleSpeed)
		{
			SpindleSpeed = source.SpindleSpeed;
			OnPropertyChanged("SpindleSpeed");
		}
		if (IsIndicationEnabled != source.IsIndicationEnabled)
		{
			IsIndicationEnabled = source.IsIndicationEnabled;
			OnPropertyChanged("IsIndicationEnabled");
		}
		if (PhysicalLocation != source.PhysicalLocation)
		{
			PhysicalLocation = source.PhysicalLocation;
			OnPropertyChanged("PhysicalLocation");
		}
		if (EnclosureNumber != source.EnclosureNumber)
		{
			EnclosureNumber = source.EnclosureNumber;
			OnPropertyChanged("EnclosureNumber");
		}
		if (SlotNumber != source.SlotNumber)
		{
			SlotNumber = source.SlotNumber;
			OnPropertyChanged("SlotNumber");
		}
		if (CanPool != source.CanPool)
		{
			CanPool = source.CanPool;
			OnPropertyChanged("CanPool");
		}
		if (CannotPoolReason != source.CannotPoolReason)
		{
			CannotPoolReason = source.CannotPoolReason;
			OnPropertyChanged("CannotPoolReason");
		}
		if (OtherCannotPoolReasonDescription != source.OtherCannotPoolReasonDescription)
		{
			OtherCannotPoolReasonDescription = source.OtherCannotPoolReasonDescription;
			OnPropertyChanged("OtherCannotPoolReasonDescription");
		}
		if (MediaType != source.MediaType)
		{
			MediaType = source.MediaType;
			OnPropertyChanged("MediaType");
		}
		if (EnclosureName != source.EnclosureName)
		{
			EnclosureName = source.EnclosureName;
			OnPropertyChanged("EnclosureName");
		}
		if (HealthStatus != source.HealthStatus)
		{
			HealthStatus = source.HealthStatus;
			OnPropertyChanged("FirmwareVersion");
		}
	}
}

