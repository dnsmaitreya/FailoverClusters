using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum ResourceLoadSelection
{
	None = 0,
	Basic = 1,
	CommonProperties = 2,
	PrivateProperties = 4,
	BasicExtended = 7,
	Dependencies = 8,
	Dependents = 0x10,
	DependenciesRelation = 0x20,
	RequiredDependencies = 0x40,
	PossibleOwners = 0x80,
	Storage = 0x100,
	StoragePoolInfo = 0x200,
	PoolPhysicalDisksInfo = 0x400,
	StorageReplicationInfo = 0x800,
	All = 0xFFF,
	Reload = 0x20000000
}
