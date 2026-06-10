using System;
using System.Globalization;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ClusterPropertyULong : ClusterProperty
{
	public override object Value => TypedValue;

	public ulong TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (ulong)base.UpdatedValue;
			}
			return (ulong)base.Value;
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

	public ClusterPropertyULong(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.UnsignedInt64, propertyKind, readOnly)
	{
	}

	public ClusterPropertyULong(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, ulong value)
		: base(name, realName, ClusterPropertyType.UnsignedInt64, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is ulong))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(ulong), value.GetType()));
		}
		TypedValue = (ulong)value;
	}
}

