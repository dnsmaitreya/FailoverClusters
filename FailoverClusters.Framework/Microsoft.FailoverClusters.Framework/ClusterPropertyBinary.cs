using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ClusterPropertyBinary : ClusterProperty
{
	public override object Value => TypedValue;

	public byte[] TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (byte[])base.UpdatedValue;
			}
			return (byte[])base.Value;
		}
		set
		{
			if (base.IsReadOnly)
			{
				throw new InvalidOperationException(CommonResources.ClusterProperty_ReadOnly_Set.FormatCurrentCulture(base.Name, CommonResources.Text_Binary));
			}
			if (value == null)
			{
				value = new byte[0];
			}
			base.UpdatedValue = value;
		}
	}

	public ClusterPropertyBinary(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.Binary, propertyKind, readOnly)
	{
	}

	public ClusterPropertyBinary(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, byte[] value)
		: base(name, realName, ClusterPropertyType.Binary, propertyKind, readOnly, value)
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is byte[] typedValue))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(byte[]), value.GetType()));
		}
		TypedValue = typedValue;
	}
}

