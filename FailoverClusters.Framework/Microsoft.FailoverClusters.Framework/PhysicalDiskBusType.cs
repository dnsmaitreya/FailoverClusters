namespace FailoverClusters.Framework;

public enum PhysicalDiskBusType
{
	Unknown = 0,
	Scsi = 1,
	Atapi = 2,
	Ata = 3,
	Bus1394 = 4,
	Ssa = 5,
	Fibre = 6,
	Usb = 7,
	Raid = 8,
	iScsi = 9,
	Sas = 10,
	Sata = 11,
	Sd = 12,
	Mmc = 13,
	Virtual = 14,
	FileBackedVirtual = 15,
	Spaces = 16,
	Nvme = 17,
	Scm = 18,
	Ufs = 19,
	Max = 20,
	MaxReserved = 127,
	[Filterable(false)]
	Fetching = 1073741824
}

