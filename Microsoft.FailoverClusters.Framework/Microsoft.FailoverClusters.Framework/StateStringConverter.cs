using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class StateStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is GroupState)
		{
			return ((GroupState)value).Translate();
		}
		if (value is ApplicationStatus)
		{
			return ((ApplicationStatus)value).Translate();
		}
		if (value is VirtualMachineState)
		{
			return ((VirtualMachineState)value).Translate();
		}
		if (value is VirtualMachineReplicationHealth)
		{
			return ((VirtualMachineReplicationHealth)value).Translate();
		}
		if (value is EnclosureHealthStatus)
		{
			return ((EnclosureHealthStatus)value).Translate();
		}
		if (value is PhysicalDiskHealthStatus)
		{
			return ((PhysicalDiskHealthStatus)value).Translate();
		}
		if (value is PhysicalDiskUsage)
		{
			return ((PhysicalDiskUsage)value).Translate();
		}
		if (value is MediaType)
		{
			return ((MediaType)value).Translate();
		}
		if (value is FanOperationalStatus)
		{
			return ((FanOperationalStatus)value).Translate();
		}
		if (value is PowerSupplyOperationalStatus)
		{
			return ((PowerSupplyOperationalStatus)value).Translate();
		}
		if (value is TemperatureSensorOperationalStatus)
		{
			return ((TemperatureSensorOperationalStatus)value).Translate();
		}
		if (value is VoltageSensorOperationalStatus)
		{
			return ((VoltageSensorOperationalStatus)value).Translate();
		}
		if (value is CurrentSensorOperationalStatus)
		{
			return ((CurrentSensorOperationalStatus)value).Translate();
		}
		if (value is IOControllerOperationalStatus)
		{
			return ((IOControllerOperationalStatus)value).Translate();
		}
		if (value is StorageNodeOperationalStatus)
		{
			return ((StorageNodeOperationalStatus)value).Translate();
		}
		if (value is VirtualMachineGroup)
		{
			return EnumLocalization.GetStringValues<VirtualMachineState>();
		}
		if (value is Group)
		{
			return EnumLocalization.GetStringValues<GroupState>();
		}
		if (value is NetworkState)
		{
			return ((NetworkState)value).Translate();
		}
		if (value is NetworkInterfaceState)
		{
			return ((NetworkInterfaceState)value).Translate();
		}
		if (value is NodeState)
		{
			return ((NodeState)value).Translate();
		}
		if (value is NodeStatus)
		{
			return ((NodeStatus)value).Translate();
		}
		if (value is PartitionStyle)
		{
			return ((PartitionStyle)value).Translate();
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}
