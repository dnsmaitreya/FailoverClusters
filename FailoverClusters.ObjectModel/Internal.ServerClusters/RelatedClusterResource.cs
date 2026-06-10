namespace MS.Internal.ServerClusters;

public sealed class RelatedClusterResource
{
	private ClusterResource m_relatedResource;

	private string m_displayName;

	public string DisplayName => m_displayName;

	public ClusterResource RelatedResource => m_relatedResource;

	public RelatedClusterResource(ClusterResource relatedResource, string displayName)
	{
		m_relatedResource = relatedResource;
		m_displayName = displayName;
	}
}
