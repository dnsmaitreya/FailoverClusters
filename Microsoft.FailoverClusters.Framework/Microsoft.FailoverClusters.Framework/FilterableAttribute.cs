using System;

namespace Microsoft.FailoverClusters.Framework;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public sealed class FilterableAttribute : Attribute
{
	public bool Filterable { get; private set; }

	public FilterableAttribute(bool filterable)
	{
		Filterable = filterable;
	}
}
