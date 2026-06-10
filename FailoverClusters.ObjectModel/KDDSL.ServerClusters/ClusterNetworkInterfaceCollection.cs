using System.Reflection;

namespace KDDSL.ServerClusters;

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
