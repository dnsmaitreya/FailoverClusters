namespace Microsoft.FailoverClusters.Framework;

public enum PartitionMbrType : ushort
{
	Unknown = 0,
	Fat12 = 1,
	Fat16 = 4,
	Extended = 5,
	Huge = 6,
	Ifs = 7,
	Fat32 = 12
}
