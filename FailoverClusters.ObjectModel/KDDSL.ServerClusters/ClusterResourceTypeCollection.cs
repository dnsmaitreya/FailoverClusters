using System.Reflection;

namespace KDDSL.ServerClusters;

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
