using System;
using System.Globalization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterPropertyUShort : ClusterProperty
{
	public override object Value => TypedValue;

	public ushort TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (ushort)base.UpdatedValue;
			}
			return (ushort)base.Value;
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

	public ClusterPropertyUShort(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.UnsignedShort, propertyKind, readOnly)
	{
	}

	public ClusterPropertyUShort(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, ushort value)
		: base(name, realName, ClusterPropertyType.UnsignedShort, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is ushort))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(ushort), value.GetType()));
		}
		TypedValue = (ushort)value;
	}
}
