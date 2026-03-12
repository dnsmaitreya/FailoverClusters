using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.FailoverClusters.Framework;

public class ResourceStateStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is ResourceState)
		{
			return ((ResourceState)value).Translate();
		}
		if (value is VirtualMachineState)
		{
			return ((VirtualMachineState)value).Translate();
		}
		if (value is VirtualMachineResource)
		{
			return EnumLocalization.GetStringValues<VirtualMachineState>();
		}
		if (value is Resource)
		{
			return EnumLocalization.GetStringValues<ResourceState>();
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}
