using System;
using System.Globalization;
using System.Windows.Data;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

[Serializable]
public class NodeStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return CommonResources.LoadingText;
		}
		if (!(value is Node))
		{
			throw new InvalidCastException("Node String Converter can only take parameters of type 'Node'");
		}
		return (value as Node).Name;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}

