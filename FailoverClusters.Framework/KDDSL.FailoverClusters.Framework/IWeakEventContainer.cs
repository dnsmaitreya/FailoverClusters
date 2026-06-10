namespace KDDSL.FailoverClusters.Framework;

internal interface IWeakEventContainer
{
	bool? NeedCompactation { get; set; }

	void Compact();
}
