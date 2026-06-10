using System;
using System.Globalization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ClusterPropertyDateTime : ClusterProperty
{
	public override object Value
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return base.UpdatedValue;
			}
			return TypedValue;
		}
	}

	public DateTime TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (DateTime)base.UpdatedValue;
			}
			return (DateTime)base.Value;
		}
		set
		{
			if (base.IsReadOnly)
			{
				throw new InvalidOperationException(CommonResources.ClusterProperty_ReadOnly_Set.FormatCurrentCulture(base.Name, value.ToString(CultureInfo.CurrentCulture)));
			}
			base.UpdatedValue = value;
		}
	}

	public ClusterPropertyDateTime(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.DateTime, propertyKind, readOnly)
	{
	}

	public ClusterPropertyDateTime(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, DateTime value)
		: base(name, realName, ClusterPropertyType.DateTime, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is DateTime))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(DateTime), value.GetType()));
		}
		TypedValue = (DateTime)value;
	}
}

