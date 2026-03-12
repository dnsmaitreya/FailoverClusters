namespace Microsoft.FailoverClusters.Framework;

public class RelatedResource
{
	public Resource Resource { get; private set; }

	public string DisplayName { get; private set; }

	public RelatedResource(Resource relatedResource, string displayName)
	{
		Resource = relatedResource;
		DisplayName = displayName;
	}
}
