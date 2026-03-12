using System;
using System.Globalization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterPropertyUInt : ClusterProperty
{
	public override object Value => TypedValue;

	public uint TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (uint)base.UpdatedValue;
			}
			return (uint)base.Value;
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

	public ClusterPropertyUInt(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.UnsignedInt, propertyKind, readOnly)
	{
	}

	public ClusterPropertyUInt(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, uint value)
		: base(name, realName, ClusterPropertyType.UnsignedInt, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is uint))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(uint), value.GetType()));
		}
		TypedValue = (uint)value;
	}
}
