using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ClusterPropertyExpandString : ClusterProperty
{
	public override object Value => TypedValue;

	public string TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (string)base.UpdatedValue;
			}
			return (string)base.Value;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (base.IsReadOnly)
			{
				throw new InvalidOperationException(CommonResources.ClusterProperty_ReadOnly_Set.FormatCurrentCulture(base.Name, value));
			}
			base.UpdatedValue = value;
		}
	}

	public ClusterPropertyExpandString(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.ExpandString, propertyKind, readOnly)
	{
	}

	public ClusterPropertyExpandString(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, string value)
		: base(name, realName, ClusterPropertyType.ExpandString, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is string typedValue))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(string), value.GetType()));
		}
		TypedValue = typedValue;
	}
}

