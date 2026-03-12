using System;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterPropertiesEventArgs : ClusterEventArgs
{
	private ClusterPropertyCollection properties;

	private string name;

	private int? type;

	private bool isVirtual;

	private VirtualPropertyPayloadStatus virtualPropertyPayloadStatus;

	public ClusterPropertyCollection Properties
	{
		get
		{
			return properties;
		}
		internal set
		{
			properties = value;
		}
	}

	internal PCluster Cluster { get; set; }

	public string Name
	{
		get
		{
			return name;
		}
		internal set
		{
			name = value;
		}
	}

	internal ClusterPropertyKind PropertyKind { get; set; }

	internal bool IsVirtual
	{
		get
		{
			return isVirtual;
		}
		set
		{
			isVirtual = value;
		}
	}

	internal VirtualPropertyPayloadStatus VirtualPropertyPayloadStatus
	{
		get
		{
			return virtualPropertyPayloadStatus;
		}
		set
		{
			virtualPropertyPayloadStatus = value;
		}
	}

	public int? ObjectType
	{
		get
		{
			return type;
		}
		internal set
		{
			type = value;
		}
	}

	public ClusterPropertiesEventArgs(Guid id, string name, int? type, ClusterException exception)
		: base(id, exception)
	{
		this.name = name;
		this.type = type;
	}
}
