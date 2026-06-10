using System.Reflection;

namespace MS.Internal.ServerClusters;

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
