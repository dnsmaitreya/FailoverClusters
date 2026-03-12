namespace Microsoft.FailoverClusters.Framework;

public enum PartitionTransitionState : ushort
{
	ReservedForFutureUse = 0,
	Stable = 1,
	BeingExtended = 2,
	BeingShrunk = 3,
	BeingReconfigured = 4,
	BeingRestriped = 8
}
