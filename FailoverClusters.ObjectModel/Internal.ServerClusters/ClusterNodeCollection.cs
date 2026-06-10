using System.Reflection;

namespace MS.Internal.ServerClusters;

[DefaultMember("Item")]
public class ClusterNodeCollection : ClusterObjectCollectionBase<ClusterNode>
{
	internal ClusterNodeCollection(AsyncEnumeration<ClusterNode> asyncEnum)
	{
		LoadFromEnum(asyncEnum);
	}

	internal ClusterNodeCollection()
	{
	}
}
