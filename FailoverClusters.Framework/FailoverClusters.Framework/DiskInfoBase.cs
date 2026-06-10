namespace FailoverClusters.Framework;

public abstract class DiskInfoBase : DiskInfo<IDiskInfo>, IDiskInfo, IKeyQueryable
{
	protected DiskInfoBase(ClusterObject owner)
		: base(owner)
	{
	}
}

