using System.Reflection;

namespace KDDSL.ServerClusters;

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
