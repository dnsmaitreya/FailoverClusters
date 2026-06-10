using System;
using System.Collections.Generic;

namespace MS.Internal.ServerClusters;

internal class ClusterEnumItem : IComparer<ClusterEnumItem>, IComparable
{
	public int Index;

	public int Version;

	public Guid ID;

	public string Name;

	public string getName()
	{
		return Name;
	}

	public virtual int Compare(ClusterEnumItem x, ClusterEnumItem y)
	{
		if (x != null && !(x.Name == null))
		{
			if (y != null && !(y.Name == null))
			{
				return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
			}
			return 1;
		}
		if (y != null && !(y.Name == null))
		{
			return -1;
		}
		return 0;
	}

	public virtual int CompareTo(object y)
	{
		return Compare(this, (ClusterEnumItem)y);
	}
}
