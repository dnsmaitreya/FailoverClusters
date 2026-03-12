namespace Microsoft.FailoverClusters.Framework;

public enum UniqueIdFormat
{
	VendorSpecific = 0,
	VendorId = 1,
	Eui64 = 2,
	FcphName = 3,
	ScsiName = 8
}
