namespace FailoverClusters.Framework;

public enum VirtualDiskDetachedReason : ushort
{
	Unknown,
	None,
	ByPolicy,
	MajorityDisksUnhealthy,
	Incomplete
}

