using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.FailoverClusters.UI.Common;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterPropertyMultipleStrings : ClusterProperty
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

	public ReadOnlyCollection<string> TypedValue
	{
		get
		{
			if (base.UpdatedValue != null)
			{
				return (ReadOnlyCollection<string>)base.UpdatedValue;
			}
			return (ReadOnlyCollection<string>)base.Value;
		}
		internal set
		{
			if (value == null)
			{
				value = new List<string>().AsReadOnly();
			}
			if (base.IsReadOnly)
			{
				throw new InvalidOperationException(CommonResources.ClusterProperty_ReadOnly_Set.FormatCurrentCulture(base.Name, value.ToString()));
			}
			base.UpdatedValue = value;
		}
	}

	public ClusterPropertyMultipleStrings(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly)
		: base(name, realName, ClusterPropertyType.StringCollection, propertyKind, readOnly, new List<string>().AsReadOnly())
	{
	}

	public ClusterPropertyMultipleStrings(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, IEnumerable<string> value)
		: base(name, realName, ClusterPropertyType.StringCollection, propertyKind, readOnly, new List<string>(value).AsReadOnly())
	{
	}

	public override void SetValue(object value)
	{
		Exceptions.ThrowIfNull(value, "value");
		if (!(value is ReadOnlyCollection<string> typedValue))
		{
			throw new ArgumentException(CommonResources.ClusterProperty_InvalidType_Set.FormatCurrentCulture(typeof(ReadOnlyCollection<string>), value.GetType()));
		}
		TypedValue = typedValue;
	}
}
