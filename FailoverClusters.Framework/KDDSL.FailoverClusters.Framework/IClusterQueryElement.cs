namespace KDDSL.FailoverClusters.Framework;

internal interface IClusterQueryElement
{
	string Field { get; }

	string Name { get; }

	string Syntax();
}
