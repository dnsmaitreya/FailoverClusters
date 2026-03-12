namespace MS.Internal.FailoverClusters.Framework;

internal class RelatedResourceInternal
{
	public PResource Resource { get; private set; }

	public string DisplayName { get; private set; }

	public RelatedResourceInternal(PResource relatedResource, string displayName)
	{
		Resource = relatedResource;
		DisplayName = displayName;
	}
}
