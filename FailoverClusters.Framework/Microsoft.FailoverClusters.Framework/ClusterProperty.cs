using System;
using System.Collections.Generic;
using System.Text;

namespace FailoverClusters.Framework;

public abstract class ClusterProperty : ICloneable
{
	private readonly ClusterPropertyType propertyType;

	private readonly ClusterPropertyKind propertyKind;

	private readonly bool isReadOnly;

	private string name;

	private string realName;

	private string displayName;

	private bool isDeleted;

	private object currentValue;

	protected object UpdatedValue { get; set; }

	public string DisplayName
	{
		get
		{
			if (displayName != null)
			{
				return displayName;
			}
			return Name;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		internal set
		{
			if (realName == null)
			{
				realName = name;
			}
			displayName = name;
			name = value;
		}
	}

	internal bool IsDeleted
	{
		get
		{
			return isDeleted;
		}
		set
		{
			isDeleted = value;
		}
	}

	internal string RealName => realName;

	public ClusterPropertyKind PropertyKind => propertyKind;

	public ClusterPropertyType PropertyType => propertyType;

	public bool IsReadOnly => isReadOnly;

	public virtual object Value => currentValue;

	public virtual bool IsModified => UpdatedValue != null;

	protected ClusterProperty(string name, string realName, ClusterPropertyType propertyType, ClusterPropertyKind propertyKind, bool readOnly)
		: this(name, realName, propertyType, propertyKind, readOnly, null)
	{
	}

	protected ClusterProperty(string name, string realName, ClusterPropertyType propertyType, ClusterPropertyKind propertyKind, bool readOnly, object currentValue)
	{
		this.name = name;
		this.realName = ((realName != name) ? realName : null);
		this.propertyType = propertyType;
		this.propertyKind = propertyKind;
		isReadOnly = readOnly;
		this.currentValue = currentValue;
	}

	public void OverrideCurrentValue(object value)
	{
		currentValue = value;
	}

	public abstract void SetValue(object value);

	public virtual void Rollback()
	{
		UpdatedValue = null;
		isDeleted = false;
	}

	public virtual object Clone()
	{
		ClusterProperty clusterProperty;
		switch (propertyType)
		{
		case ClusterPropertyType.Binary:
			clusterProperty = new ClusterPropertyBinary(name, realName, propertyKind, IsReadOnly, (byte[])currentValue);
			break;
		case ClusterPropertyType.UnsignedInt:
			clusterProperty = new ClusterPropertyUInt(name, realName, propertyKind, IsReadOnly, (uint)currentValue);
			break;
		case ClusterPropertyType.String:
			clusterProperty = new ClusterPropertyString(name, realName, propertyKind, IsReadOnly, (string)currentValue);
			break;
		case ClusterPropertyType.ExpandString:
			clusterProperty = new ClusterPropertyExpandString(name, realName, propertyKind, IsReadOnly, (string)currentValue);
			break;
		case ClusterPropertyType.ExpandedString:
			clusterProperty = new ClusterPropertyExpandedString(name, realName, propertyKind, IsReadOnly, (string)currentValue);
			break;
		case ClusterPropertyType.StringCollection:
			clusterProperty = new ClusterPropertyMultipleStrings(name, realName, propertyKind, IsReadOnly, (IEnumerable<string>)currentValue);
			break;
		case ClusterPropertyType.UnsignedInt64:
			clusterProperty = new ClusterPropertyULong(name, realName, propertyKind, IsReadOnly, (ulong)currentValue);
			break;
		case ClusterPropertyType.Int:
			clusterProperty = new ClusterPropertyInt(name, realName, propertyKind, IsReadOnly, (int)currentValue);
			break;
		case ClusterPropertyType.Int64:
			clusterProperty = new ClusterPropertyLong(name, realName, propertyKind, IsReadOnly, (long)currentValue);
			break;
		case ClusterPropertyType.UnsignedShort:
			clusterProperty = new ClusterPropertyUShort(name, realName, propertyKind, IsReadOnly, (ushort)currentValue);
			break;
		case ClusterPropertyType.DateTime:
			clusterProperty = new ClusterPropertyDateTime(name, realName, propertyKind, IsReadOnly, (DateTime)currentValue);
			break;
		case ClusterPropertyType.SecurityDescriptor:
			clusterProperty = new ClusterPropertyBinary(name, realName, propertyKind, IsReadOnly, (byte[])currentValue);
			break;
		case ClusterPropertyType.Unknown:
			clusterProperty = new ClusterPropertyUnknown(name, realName, propertyKind, IsReadOnly, currentValue);
			break;
		case ClusterPropertyType.UserFormat:
			return null;
		default:
			return null;
		}
		clusterProperty.UpdatedValue = UpdatedValue;
		return clusterProperty;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Name:");
		stringBuilder.Append(Name);
		stringBuilder.Append("    Type:");
		stringBuilder.Append(propertyType);
		stringBuilder.Append("    Kind:");
		stringBuilder.Append(propertyKind);
		stringBuilder.Append("    RW/RO:");
		stringBuilder.Append(IsReadOnly ? "ReadOnly" : "ReadWrite");
		stringBuilder.Append("    Value:");
		stringBuilder.Append((currentValue != null) ? currentValue.ToString() : "(null)");
		return stringBuilder.ToString();
	}
}

