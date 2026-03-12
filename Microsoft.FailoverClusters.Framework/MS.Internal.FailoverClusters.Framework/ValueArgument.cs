using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

internal class ValueArgument : IClusterQueryArgument
{
	private object memberValue;

	public string Name { get; protected set; }

	public virtual object Value => memberValue;

	public ValueArgument(object value)
	{
		memberValue = value;
	}

	public override string ToString()
	{
		if (memberValue is string)
		{
			return "'{0}'".FormatInvariantCulture(memberValue.ToString());
		}
		return "{0}".FormatInvariantCulture(memberValue.ToString());
	}
}
