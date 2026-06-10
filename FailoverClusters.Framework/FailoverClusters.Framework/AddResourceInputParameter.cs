using FailoverClusters.UI.Common;

namespace FailoverClusters.Framework;

public class AddResourceInputParameter
{
	private readonly string suggestedName;

	public string SuggestedName => suggestedName;

	public string Text { get; set; }

	public ResourceType ResourceType { get; private set; }

	public Group Group { get; internal set; }

	public AddResourceInputParameter(string suggestedName, string text, ResourceType resourceType)
	{
		this.suggestedName = suggestedName;
		ResourceType = resourceType;
		Text = text;
	}

	public virtual AddResourceInputParameter Clone()
	{
		return new AddResourceInputParameter(suggestedName, Text, ResourceType)
		{
			Group = Group
		};
	}

	public override string ToString()
	{
		return "{0} : {1} : {2}".FormatCurrentCulture(suggestedName, Text, ResourceType.ResourceKind.Translate());
	}
}

