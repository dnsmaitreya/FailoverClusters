using System;
using System.Collections.Concurrent;

namespace Microsoft.FailoverClusters.Framework;

public abstract class ClusterCommandBase<T> where T : ClusterCommandBase<T>, new()
{
	private static ConcurrentDictionary<ulong, WeakReferenceEx<ClusterCommand>> commandReferences = new ConcurrentDictionary<ulong, WeakReferenceEx<ClusterCommand>>();

	private static object lockObject = new object();

	protected abstract ClusterCommand CreateInstance(Group group, ClusterCommandCollectionId id);

	public static ClusterCommand Create(Group group, ClusterCommandCollectionId id)
	{
		return commandReferences.AddOrUpdate((ulong)(((long)group.GetHashCode() << 32) | (uint)typeof(T).GetHashCode()), (ulong key) => new WeakReferenceEx<ClusterCommand>(new T().CreateInstance(group, id)), (ulong key, WeakReferenceEx<ClusterCommand> value) => ReturnInstance(value, () => new T().CreateInstance(group, id))).Target;
	}

	private static WeakReferenceEx<ClusterCommand> ReturnInstance(WeakReferenceEx<ClusterCommand> weakReference, Func<ClusterCommand> createInstanceCallback)
	{
		ClusterCommand clusterCommand = null;
		if (weakReference != null)
		{
			clusterCommand = weakReference.Target;
		}
		if (weakReference == null || clusterCommand == null)
		{
			lock (lockObject)
			{
				if (weakReference == null || clusterCommand == null)
				{
					clusterCommand = createInstanceCallback();
					weakReference = new WeakReferenceEx<ClusterCommand>(clusterCommand);
				}
			}
		}
		return weakReference;
	}

	public void Collect()
	{
	}
}
