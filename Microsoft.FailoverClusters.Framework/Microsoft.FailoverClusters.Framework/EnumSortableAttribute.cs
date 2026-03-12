using System;

namespace Microsoft.FailoverClusters.Framework;

[AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
public sealed class EnumSortableAttribute : Attribute
{
	public EnumSortOrder SortOrder { get; private set; }

	public EnumSortableAttribute(EnumSortOrder sortOrder)
	{
		SortOrder = sortOrder;
	}
}
