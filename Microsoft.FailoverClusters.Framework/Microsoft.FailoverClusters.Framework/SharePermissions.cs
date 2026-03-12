using System;

namespace Microsoft.FailoverClusters.Framework;

[Flags]
public enum SharePermissions
{
	None = 0,
	Read = 1,
	Write = 2,
	Create = 4,
	Exec = 8,
	Delete = 0x10,
	Attribute = 0x20,
	Perm = 0x40,
	All = 0x7F
}
