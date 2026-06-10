using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class LastReplicaTimeStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value == DependencyProperty.UnsetValue)
		{
			return string.Empty;
		}
		if ((DateTime)value == DateTime.MinValue)
		{
			return string.Empty;
		}
		if ((DateTime)value == DateTime.FromFileTime(0L))
		{
			return CommonResources.NoLastReplicaTimeString;
		}
		return ((DateTime)value).ToString(CultureInfo.InvariantCulture);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}

