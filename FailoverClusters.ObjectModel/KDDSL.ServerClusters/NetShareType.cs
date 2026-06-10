namespace KDDSL.ServerClusters;

public enum NetShareType
{
	DiskTree = 0,
	PrintQ = 1,
	Device = 2,
	Ipc = 3,
	TypeMask = 268435455,
	Special = int.MinValue,
	Temporary = 1073741824,
	AdminShare = -1073741824
}
