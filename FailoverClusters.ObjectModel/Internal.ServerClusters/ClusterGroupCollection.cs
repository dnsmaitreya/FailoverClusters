using System.Reflection;

namespace MS.Internal.ServerClusters;

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
