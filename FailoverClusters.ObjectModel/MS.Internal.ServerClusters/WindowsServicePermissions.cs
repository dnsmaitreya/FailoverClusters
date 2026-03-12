using System;

namespace MS.Internal.ServerClusters;

[Flags]
public enum WindowsServicePermissions
{
	Query = 1,
	ChangeState = 2,
	All = 0xF01FF
}
