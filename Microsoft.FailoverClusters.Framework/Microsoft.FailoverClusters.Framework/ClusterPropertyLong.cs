using System;
using System.Globalization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterPropertyLong : ClusterProperty
{
	public override object Value => TypedValue;

	public long TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (long)base.UpdatedValue;
			}
			return (long)base.Value;
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

	public ClusterPropertyLong(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.Int64, propertyKind, readOnly)
	{
	}

	public ClusterPropertyLong(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, long value)
		: base(name, realName, ClusterPropertyType.Int64, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is long))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(long), value.GetType()));
		}
		TypedValue = (long)value;
	}
}
