using System;
using System.Diagnostics;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class ClusterAddedEventArgs : ClusterEventArgs
{
	public string Name { get; internal set; }

	public int? ObjectType { get; internal set; }

	public string ObjectTypeName { get; internal set; }

	public Guid ParentId { get; internal set; }

	internal PCluster Cluster { get; set; }

	[DebuggerNonUserCode]
	public ClusterAddedEventArgs(Guid id, string name, int? objectType, ClusterException exception)
		: this(id, name, objectType, null, Guid.Empty, exception)
	{
	}

	[DebuggerNonUserCode]
	public ClusterAddedEventArgs(Guid id, string name, int? objectType, string objectTypeName, Guid parentId, ClusterException exception)
		: base(id, exception)
	{
		Name = name;
		ObjectType = objectType;
		ObjectTypeName = objectTypeName;
		ParentId = parentId;
	}
}

