using System.Reflection;

namespace MS.Internal.ServerClusters;

[DefaultMember("Item")]
public class ClusterNetworkInterfaceCollection : ClusterObjectCollectionBase<ClusterNetworkInterface>
{
	internal ClusterNetworkInterfaceCollection(AsyncEnumeration<ClusterNetworkInterface> asyncEnum)
	{
		LoadFromEnum(asyncEnum);
	}

	internal ClusterNetworkInterfaceCollection()
	{
	}
}
