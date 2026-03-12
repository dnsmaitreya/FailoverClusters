using System;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterPropertyString : ClusterProperty
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

	public ClusterPropertyString(string name)
		: base(name, name, ClusterPropertyType.String, ClusterPropertyKind.Private, readOnly: false)
	{
	}

	public ClusterPropertyString(string name, string value)
		: base(name, name, ClusterPropertyType.String, ClusterPropertyKind.Private, readOnly: false)
	{
		base.UpdatedValue = value;
	}

	internal ClusterPropertyString(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.String, propertyKind, readOnly)
	{
	}

	internal ClusterPropertyString(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, string value)
		: base(name, realName, ClusterPropertyType.String, propertyKind, readOnly, value)
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
