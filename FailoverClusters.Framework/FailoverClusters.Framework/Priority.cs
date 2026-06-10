namespace FailoverClusters.Framework;

[EnumSortable(EnumSortOrder.Value)]
public enum Priority
{
	[Filterable(false)]
	Unknown = -1,
	NoAutoStart = 0,
	Low = 1000,
	Medium = 2000,
	High = 3000,
	[Filterable(false)]
	Fetching = 1073741824
}

