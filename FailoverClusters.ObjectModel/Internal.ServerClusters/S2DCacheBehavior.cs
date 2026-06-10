using System;

namespace MS.Internal.ServerClusters;

[Flags]
public enum S2DCacheBehavior : long
{
	Default = 0x58L,
	Dormant = 1L,
	SkipSpindlePartitionsRemovalOnDisable = 2L,
	SkipFlashPartitionsRemovalOnDisable = 4L,
	CacheModeHddRead = 8L,
	CacheModeHddWrite = 0x10L,
	CacheModeSsdread = 0x20L,
	CacheModeSsdWrite = 0x40L,
	UseScmForCapacity = 0x80L
}
