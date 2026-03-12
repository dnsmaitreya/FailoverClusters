using System.Reflection;

namespace MS.Internal.ServerClusters;

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
