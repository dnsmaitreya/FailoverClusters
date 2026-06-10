namespace FailoverClusters.Framework;

public enum ShareInfoType : uint
{
	DiskTree = 0u,
	PrintQ = 1u,
	Device = 2u,
	InterProcessCommunication = 3u,
	Mask = 255u,
	Temporary = 16777216u,
	Special = 2147483648u
}

