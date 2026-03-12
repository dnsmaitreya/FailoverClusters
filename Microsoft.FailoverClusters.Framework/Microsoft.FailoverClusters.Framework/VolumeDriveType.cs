namespace Microsoft.FailoverClusters.Framework;

public enum VolumeDriveType : uint
{
	Unknown,
	InvalidRootPath,
	Removable,
	Fixed,
	Remote,
	CDRom,
	RamDisk
}
