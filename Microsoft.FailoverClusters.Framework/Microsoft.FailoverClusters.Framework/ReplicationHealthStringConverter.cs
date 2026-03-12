using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.FailoverClusters.Framework;

public class ReplicationHealthStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value == DependencyProperty.UnsetValue)
		{
			return string.Empty;
		}
		return ((VirtualMachineReplicationHealth)value).Translate(LocalizedEnum.VirtualMachineReplicationHealth);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}
