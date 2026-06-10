using System;
using System.Reflection;

namespace FailoverClusters.Framework;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class PropertyStoreAttribute : Attribute
{
	public string StoreFieldName { get; internal set; }

	public PropertyInfo SourceProperty { get; internal set; }

	public PropertyInfo TargetProperty { get; internal set; }

	public PropertyStoreAttribute(string storeFieldName)
	{
		StoreFieldName = storeFieldName;
	}
}

