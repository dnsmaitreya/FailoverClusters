using System;
using System.Globalization;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterPropertyInt : ClusterProperty
{
	public override object Value => TypedValue;

	public int TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (int)base.UpdatedValue;
			}
			return (int)base.Value;
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

	public ClusterPropertyInt(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.Int, propertyKind, readOnly)
	{
	}

	public ClusterPropertyInt(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, int value)
		: base(name, realName, ClusterPropertyType.Int, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is int))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(int), value.GetType()));
		}
		TypedValue = (int)value;
	}
}
