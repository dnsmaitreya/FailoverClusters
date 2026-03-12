namespace Microsoft.FailoverClusters.Framework;

public enum VirtualDiskDetachedReason : ushort
{
	Unknown,
	None,
	ByPolicy,
	MajorityDisksUnhealthy,
	Incomplete
}
