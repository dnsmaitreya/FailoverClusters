using System.Reflection;

namespace MS.Internal.ServerClusters;

[DefaultMember("Item")]
public class ClusterResourceTypeCollection : ClusterObjectCollectionBase<ClusterResourceType>
{
	internal ClusterResourceTypeCollection(AsyncEnumeration<ClusterResourceType> asyncEnum)
	{
		LoadFromEnum(asyncEnum);
	}

	internal ClusterResourceTypeCollection()
	{
	}
}
