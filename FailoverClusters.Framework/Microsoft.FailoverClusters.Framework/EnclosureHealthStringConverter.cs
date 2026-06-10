using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FailoverClusters.Framework;

[Serializable]
public class EnclosureHealthStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value == DependencyProperty.UnsetValue)
		{
			return string.Empty;
		}
		return ((EnclosureHealthStatus)value).Translate(LocalizedEnum.EnclosureHealthStatus);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

