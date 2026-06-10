using System.Reflection;

namespace KDDSL.ServerClusters;

[DefaultMember("Item")]
public class ClusterStorageOnlyNodeCollection : ClusterObjectCollectionBase<ClusterStorageOnlyNode>
{
	internal ClusterStorageOnlyNodeCollection(AsyncEnumeration<ClusterStorageOnlyNode> asyncEnum)
	{
	}

	internal ClusterStorageOnlyNodeCollection()
	{
	}
}
