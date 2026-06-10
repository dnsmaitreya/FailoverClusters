using System;

namespace KDDSL.ServerClusters;

internal class ObjectLifetimeEventArgs<T> : EventArgs
{
	private T clusterObject;

	private ulong id;

	private ObjectLifetime lifetime;

	public ObjectLifetime Lifetime => lifetime;

	public T ClusterObject => clusterObject;

	internal ulong Id => id;

	internal ObjectLifetimeEventArgs(T clusterObject, ulong id, ObjectLifetime lifetime)
	{
		this.clusterObject = clusterObject;
		this.id = id;
		this.lifetime = lifetime;
	}
}
