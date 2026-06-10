using System.Reflection;

namespace KDDSL.ServerClusters;

[DefaultMember("Item")]
public class ClusterGroupCollection : ClusterObjectCollectionBase<ClusterGroup>
{
	internal ClusterGroupCollection(AsyncEnumeration<ClusterGroup> asyncEnum)
	{
		LoadFromEnum(asyncEnum);
	}

	internal ClusterGroupCollection()
	{
	}
}
