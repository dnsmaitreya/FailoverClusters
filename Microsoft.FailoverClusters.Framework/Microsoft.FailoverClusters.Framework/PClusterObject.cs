using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.Framework;

internal abstract class PClusterObject<T> : PClusterObject, IPClusterObject<T> where T : ClusterObject
{
	protected PClusterObject()
	{
	}

	protected PClusterObject(PCluster cluster)
		: base(cluster)
	{
	}

	public override ClusterObject GetProxyObject()
	{
		return GetProxy();
	}

	public abstract T GetProxy();

	public abstract T GetProxy(ProxyCreateMode createMode);
}
