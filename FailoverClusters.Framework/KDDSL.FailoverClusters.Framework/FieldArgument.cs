namespace KDDSL.FailoverClusters.Framework;

internal class FieldArgument : IClusterQueryArgument
{
	private readonly string name;

	public string Name => name;

	public string Field => name;

	public FieldArgument(string name)
	{
		this.name = name.ToLowerInvariant();
	}

	public override string ToString()
	{
		return name;
	}
}
