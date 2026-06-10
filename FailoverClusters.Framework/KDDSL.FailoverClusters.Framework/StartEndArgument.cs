namespace KDDSL.FailoverClusters.Framework;

internal class StartEndArgument : IClusterQueryArgument
{
	private readonly StartEndArgumentType conditionalType;

	public string Field => null;

	public string Name => null;

	public StartEndArgumentType ConditionalType => conditionalType;

	public StartEndArgument(StartEndArgumentType conditionalType)
	{
		this.conditionalType = conditionalType;
	}

	public override string ToString()
	{
		if (conditionalType != 0)
		{
			return ")";
		}
		return "(";
	}
}
