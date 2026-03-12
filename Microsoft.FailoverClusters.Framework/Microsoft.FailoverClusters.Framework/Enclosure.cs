using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class Enclosure : ObservableCollectionItem<Enclosure>
{
	private readonly CommandCollection commands = new CommandCollection(ClusterCommandCollectionId.Enclosures);

	private Icon2 icon;

	public Icon2 Icon
	{
		get
		{
			Icon2 obj = icon ?? new Icon2(InvariantResources.StorageEnclosure);
			Icon2 result = obj;
			icon = obj;
			return result;
		}
	}

	public override string Key => ObjectId;

	public override string DisplayName => FriendlyName;

	public override IEnumerable<ICommand> Commands => commands;

	public string ObjectId { get; internal set; }

	public string Rack { get; internal set; }

	public string DeviceId { get; internal set; }

	public string FriendlyName { get; internal set; }

	public string Manufacturer { get; internal set; }

	public string Model { get; internal set; }

	public string SerialNumber { get; internal set; }

	public string FirmwareVersion { get; internal set; }

	public EnclosureHealthStatus HealthStatus { get; internal set; }

	public IList<PowerSupplyOperationalStatus> PowerSupplyOperationalStatus { get; internal set; }

	public IList<FanOperationalStatus> FanOperationalStatus { get; internal set; }

	public IList<TemperatureSensorOperationalStatus> TemperatureSensorOperationalStatus { get; internal set; }

	public IList<VoltageSensorOperationalStatus> VoltageSensorOperationalStatus { get; internal set; }

	public IList<CurrentSensorOperationalStatus> CurrentSensorOperationalStatus { get; internal set; }

	public IList<IOControllerOperationalStatus> IOControllerOperationalStatus { get; internal set; }

	public uint NumberOfSlots { get; internal set; }

	public IEnumerable<PhysicalDiskInfo> PhysicalDisks => ObservablePhysicalDiskCollection.GetAssociation(this, base.Cluster, this);

	public IEnumerable<StorageNode> PhysicallyConnectedStorageNodes => ObservablePhysicallyConnectedStorageNodeCollection.GetAssociation(this, base.Cluster, this);

	public IEnumerable<StorageNode> StorageNodes => ObservableStorageNodeCollection.GetAssociation(this, base.Cluster, this);

	public Enclosure(ClusterObject owner)
		: base(owner)
	{
	}

	public EnclosureIdentifyElementStatus IdentifyElement(bool enable, uint[] slotNumbers, out uint extendedStatus)
	{
		extendedStatus = 0u;
		return EnclosureIdentifyElementStatus.NotSupported;
	}

	public static void GetInstance(Cluster cluster, string key, Action<OperationResult<Enclosure>> operationResult, string serverName = null)
	{
		ObservableCollectionItem<Enclosure>.GetInstance(cluster, key, operationResult, serverName);
	}

	public override void CopyFrom(Enclosure source)
	{
		Exceptions.ThrowIfNull(source, "source");
		if (ObjectId != source.ObjectId)
		{
			ObjectId = source.ObjectId;
			OnPropertyChanged("ObjectId");
		}
		if (!object.Equals(CurrentSensorOperationalStatus, source.CurrentSensorOperationalStatus))
		{
			CurrentSensorOperationalStatus = source.CurrentSensorOperationalStatus;
			OnPropertyChanged("CurrentSensorOperationalStatus");
		}
		if (FanOperationalStatus != source.FanOperationalStatus)
		{
			FanOperationalStatus = source.FanOperationalStatus;
			OnPropertyChanged("FanOperationalStatus");
		}
		if (FirmwareVersion != source.FirmwareVersion)
		{
			FirmwareVersion = source.FirmwareVersion;
			OnPropertyChanged("FirmwareVersion");
		}
		if (FriendlyName != source.FriendlyName)
		{
			FriendlyName = source.FriendlyName;
			OnPropertyChanged("FriendlyName");
			OnPropertyChanged("DisplayName");
		}
		if (HealthStatus != source.HealthStatus)
		{
			HealthStatus = source.HealthStatus;
			OnPropertyChanged("HealthStatus");
		}
		if (IOControllerOperationalStatus != source.IOControllerOperationalStatus)
		{
			IOControllerOperationalStatus = source.IOControllerOperationalStatus;
			OnPropertyChanged("IOControllerOperationalStatus");
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
		if (PowerSupplyOperationalStatus != source.PowerSupplyOperationalStatus)
		{
			PowerSupplyOperationalStatus = source.PowerSupplyOperationalStatus;
			OnPropertyChanged("PowerSupplyOperationalStatus");
		}
		if (SerialNumber != source.SerialNumber)
		{
			SerialNumber = source.SerialNumber;
			OnPropertyChanged("SerialNumber");
		}
		if (NumberOfSlots != source.NumberOfSlots)
		{
			NumberOfSlots = source.NumberOfSlots;
			OnPropertyChanged("NumberOfSlots");
		}
		if (TemperatureSensorOperationalStatus != source.TemperatureSensorOperationalStatus)
		{
			TemperatureSensorOperationalStatus = source.TemperatureSensorOperationalStatus;
			OnPropertyChanged("TemperatureSensorOperationalStatus");
		}
		if (VoltageSensorOperationalStatus != source.VoltageSensorOperationalStatus)
		{
			VoltageSensorOperationalStatus = source.VoltageSensorOperationalStatus;
			OnPropertyChanged("VoltageSensorOperationalStatus");
		}
	}
}
