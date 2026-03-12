using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class SubStatusStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != DependencyProperty.UnsetValue)
		{
			if (value is GroupSubStatus && (GroupSubStatus)value != GroupSubStatus.Fetching)
			{
				return ((GroupSubStatus)value).Translate();
			}
			if (value is ResourceSubStatus && (ResourceSubStatus)value != ResourceSubStatus.Fetching)
			{
				return ((ResourceSubStatus)value).Translate();
			}
		}
		return CommonResources.LoadingText;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}
