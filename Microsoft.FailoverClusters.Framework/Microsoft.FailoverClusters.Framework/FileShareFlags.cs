using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum FileShareFlags
{
	None = 0,
	DistributedFileSystem = 1,
	DistributedFileSystemRoot = 2,
	RestrictExclusiveOpens = 4,
	ForceSharedDelete = 8,
	AllowNamespaceCaching = 0x10,
	AccessBasedDirectoryEnum = 0x20,
	ForceLevelOperationLock = 0x40,
	EnableHash = 0x80,
	CacheManualReintegration = 0x100,
	CacheAutomaticReintegration = 0x200,
	CachePrograms = 0x400,
	CacheNone = 0x800
}
