using System;
using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class ClusterPropertyUnknown : ClusterProperty
{
	private bool modified;

	public override object Value => TypedValue;

	public override bool IsModified => modified;

	public object TypedValue
	{
		get
		{
			if (modified)
			{
				return base.UpdatedValue;
			}
			return base.Value;
		}
		set
		{
			if (base.IsReadOnly)
			{
				string text = ((value != null) ? value.ToString() : string.Empty);
				throw new InvalidOperationException(CommonResources.ClusterProperty_ReadOnly_Set.FormatCurrentCulture(base.Name, text));
			}
			base.UpdatedValue = value;
			modified = true;
		}
	}

	public ClusterPropertyUnknown(string name, string realName, ClusterPropertyKind propertyKind, bool readOnly, object value)
		: base(name, realName, ClusterPropertyType.Unknown, propertyKind, readOnly, value)
	{
	}

	public override void Rollback()
	{
		base.Rollback();
		modified = false;
	}

	public override object Clone()
	{
		ClusterPropertyUnknown obj = (ClusterPropertyUnknown)base.Clone();
		obj.modified = modified;
		return obj;
	}

	public override void SetValue(object value)
	{
		TypedValue = value;
	}
}

