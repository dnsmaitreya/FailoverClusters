using System;

namespace Microsoft.FailoverClusters.Framework;

public class ClusterCommandContainer : ClusterCommand
{
	private readonly CommandCollection children = new CommandCollection(ClusterCommandCollectionId.Container);

	private readonly ReadOnlyCommandCollection clildrenReadOnly;

	private bool executeIfNoChildren;

	public bool ExecuteIfNoChildren
	{
		get
		{
			return executeIfNoChildren;
		}
		set
		{
			executeIfNoChildren = value;
		}
	}

	public ReadOnlyCommandCollection Children => clildrenReadOnly;

	internal CommandCollection ChildrenInternal => children;

	internal Action<string> SignalAction { get; set; }

	public ClusterCommandContainer(ClusterObject clusterObject, string name, ClusterCommandId id)
		: this(clusterObject, name, id, ClusterCommandCollectionId.Container, null)
	{
	}

	internal ClusterCommandContainer(ClusterObject clusterObject, string name, ClusterCommandId id, Action<string> signalAction)
		: this(clusterObject, name, id, ClusterCommandCollectionId.Container, signalAction)
	{
	}

	public ClusterCommandContainer(ClusterObject clusterObject, string name, ClusterCommandId id, ClusterCommandCollectionId collectionId)
		: this(clusterObject, name, id, collectionId, null)
	{
	}

	internal ClusterCommandContainer(ClusterObject clusterObject, string name, ClusterCommandId id, ClusterCommandCollectionId collectionId, Action<string> signalAction)
		: base(clusterObject, name, id, collectionId)
	{
		clildrenReadOnly = new ReadOnlyCommandCollection(children);
		SignalAction = signalAction;
	}

	internal void Signal(string signal)
	{
		SignalAction.SafeCall(signal);
	}
}
