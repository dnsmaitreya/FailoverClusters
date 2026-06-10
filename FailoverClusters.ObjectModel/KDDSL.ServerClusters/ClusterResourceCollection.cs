using System.Reflection;

namespace KDDSL.ServerClusters;

[DefaultMember("Item")]
public class ClusterResourceCollection : ClusterObjectCollectionBase<ClusterResource>
{
	internal ClusterResourceCollection(AsyncEnumeration<ClusterResource> asyncEnum)
	{
		LoadFromEnum(asyncEnum);
	}

	public ClusterResourceCollection()
	{
	}
}
