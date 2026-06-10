using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ReplicationDiskTypeStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value == DependencyProperty.UnsetValue)
		{
			return string.Empty;
		}
		if (value is ReplicationDiskType)
		{
			if ("SKIPNONE".Equals(parameter) && (ReplicationDiskType)value == ReplicationDiskType.None)
			{
				return string.Empty;
			}
			return ((ReplicationDiskType)value).Translate(LocalizedEnum.ReplicationDiskType);
		}
		throw new NotSupportedException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}

