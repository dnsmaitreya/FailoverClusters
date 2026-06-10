using System;

namespace FailoverClusters.Framework;

public class ClusterStoragePropertiesChangedEventArgs : ClusterEventArgs
{
	public ClusterDisk Disk { get; set; }

	public bool Reload { get; set; }

	public ClusterStoragePropertiesChangedEventArgs(Guid id, ClusterDisk disk, bool reload, ClusterException exception)
		: base(id, exception)
	{
		Disk = disk;
		Reload = reload;
	}
}

