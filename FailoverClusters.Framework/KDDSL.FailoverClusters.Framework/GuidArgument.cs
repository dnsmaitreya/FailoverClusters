using FailoverClusters.UI.Common;

namespace KDDSL.FailoverClusters.Framework;

internal class GuidArgument : ValueArgument
{
	public GuidArgument(string name, object value)
		: base(value)
	{
		base.Name = name;
	}

	public override string ToString()
	{
		return "'{0}'".FormatInvariantCulture(Value.ToString());
	}
}

