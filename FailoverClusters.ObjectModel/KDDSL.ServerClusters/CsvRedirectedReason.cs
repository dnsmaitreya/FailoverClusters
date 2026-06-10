namespace KDDSL.ServerClusters;

public enum CsvRedirectedReason : ulong
{
	UserRequest = 1uL,
	UnsafeFileSystemFilter = 2uL,
	UnsafeVolumeFilter = 4uL,
	FileSystemTiering = 8uL,
	FileSystemReFs = 32uL,
	Max = 9223372036854775808uL
}
