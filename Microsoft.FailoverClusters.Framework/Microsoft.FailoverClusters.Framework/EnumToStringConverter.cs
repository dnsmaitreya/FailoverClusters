using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.FailoverClusters.Framework;

[Serializable]
public class EnumToStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == DependencyProperty.UnsetValue || value == null)
		{
			return null;
		}
		return EnumLocalization.Translate((Enum)value);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}
