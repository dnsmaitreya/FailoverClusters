namespace Microsoft.FailoverClusters.Framework;

public enum ClusterPropertyType
{
	Unknown = 0,
	Binary = 1,
	UnsignedInt = 2,
	String = 3,
	ExpandString = 4,
	StringCollection = 5,
	UnsignedInt64 = 6,
	Int = 7,
	ExpandedString = 8,
	SecurityDescriptor = 9,
	Int64 = 10,
	UnsignedShort = 11,
	DateTime = 12,
	UserFormat = 32768
}
