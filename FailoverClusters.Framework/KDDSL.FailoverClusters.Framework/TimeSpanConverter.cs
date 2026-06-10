using System;
using System.Management;

namespace KDDSL.FailoverClusters.Framework;

internal class TimeSpanConverter : WmiTypeConverter<TimeSpan>
{
	public override TimeSpan ConvertFromWmiType(object value)
	{
		return ManagementDateTimeConverter.ToTimeSpan((string)value);
	}

	public override object ConvertToWmiType(TimeSpan value)
	{
		return ManagementDateTimeConverter.ToDmtfTimeInterval(value);
	}
}
