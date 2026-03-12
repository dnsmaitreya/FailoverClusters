using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum StorageCaps : long
{
	None = 0L,
	VolumeInfo = 1L,
	Csv = 2L,
	Maintenance = 4L,
	VirtualDisk = 8L,
	PassThrough = 0x10L,
	Repair = 0x20L,
	PhysicalDiskResource = 0x3FL,
	CsvResource = 0xDL,
	[Filterable(false)]
	Fetching = 0x40000000L
}
