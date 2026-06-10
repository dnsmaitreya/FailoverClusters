using System;

namespace KDDSL.ServerClusters;

[Flags]
public enum NetworkConfigurationOptions
{
	None = 0,
	IPv6 = 1,
	IPv6Tunneled = 2,
	RoleRequiresIPv4Static = 4,
	IPv4Static = 0x10,
	IPv4Dhcp = 0x20,
	PublicNetworks = 0x40,
	AllDynamic = 0x23,
	All = 0x33
}
