using System;
using System.Management;

namespace MS.Internal.FailoverClusters.Framework;

internal class DateTimeConverter : WmiTypeConverter<DateTime>
{
	public override DateTime ConvertFromWmiType(object value)
	{
		return ManagementDateTimeConverter.ToDateTime((string)value);
	}

	public override object ConvertToWmiType(DateTime value)
	{
		return ManagementDateTimeConverter.ToDmtfDateTime(value);
	}
}
