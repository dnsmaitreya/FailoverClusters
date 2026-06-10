using System.Reflection;

namespace MS.Internal.ServerClusters;

[DefaultMember("Item")]
public class ClusterNetworkCollection : ClusterObjectCollectionBase<ClusterNetwork>
{
	internal ClusterNetworkCollection(AsyncEnumeration<ClusterNetwork> asyncEnum)
	{
		LoadFromEnum(asyncEnum);
	}

	internal ClusterNetworkCollection()
	{
	}
}
