using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class IPAddressInfo
{
	private uint m_prefixLength;

	private IPAddress m_address;

	private NetworkInfo m_network;

	private bool m_dhcp;

	public NetworkInfo NetworkInfo => m_network;

	public bool UseDhcp
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_dhcp;
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			m_dhcp = value;
		}
	}

	public IPAddress Address
	{
		get
		{
			return m_address;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			m_address = value;
		}
	}

	public uint PrefixLength
	{
		get
		{
			return m_prefixLength;
		}
		set
		{
			m_prefixLength = value;
		}
	}

	public bool IsIPv6LinkLocal
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			if (NetworkHelp.GetAddressType(m_address) == AddressType.IPv6)
			{
				return NetworkHelp.IsIPv6AddressLinkLocal(m_address);
			}
			return false;
		}
	}

	public bool IsTunneled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			if (NetworkHelp.GetAddressType(m_address) == AddressType.IPv6)
			{
				return NetworkHelp.IsIPv6AddressTunneled(m_address);
			}
			return false;
		}
	}

	public AddressType AddressType => NetworkHelp.GetAddressType(m_address);

	private void Construct(NetworkInfo network)
	{
		m_prefixLength = 0u;
		m_address = null;
		m_network = network;
		m_dhcp = false;
	}

	private void LoadFromIPAddressResource(ReadOnlyNetworkInfoCollection networkInfos, ClusterResource ipAddressResource)
	{
		//Discarded unreachable code: IL_006a
		try
		{
			string resourceType = "IP Address";
			if (ipAddressResource.IsResourceOfType(resourceType))
			{
				LoadIPv4Address(networkInfos, ipAddressResource);
				return;
			}
			string resourceType2 = "IPv6 Address";
			if (ipAddressResource.IsResourceOfType(resourceType2))
			{
				LoadIPv6Address(networkInfos, ipAddressResource);
				return;
			}
			string resourceType3 = "IPv6 Tunnel Address";
			if (ipAddressResource.IsResourceOfType(resourceType3))
			{
				LoadIPv6TunnelAddress(networkInfos, ipAddressResource);
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.IPAddress_LoadIPAddress_Fail_Text,
				ipAddressResource.Name
			});
		}
	}

	private void AssignNetworkInfo(ReadOnlyNetworkInfoCollection networkInfos, string networkName, AddressType addressType, [MarshalAs(UnmanagedType.U1)] bool isIPv6TunnelAddress)
	{
		foreach (NetworkInfo networkInfo in networkInfos)
		{
			if (networkInfo.AddressType == addressType && networkInfo.IsTunneled == isIPv6TunnelAddress && 0 == string.Compare(networkInfo.AssociatedNetwork.Name, networkName, StringComparison.CurrentCultureIgnoreCase))
			{
				m_network = networkInfo;
				break;
			}
		}
	}

	private void AssignNetworkInfo(ReadOnlyNetworkInfoCollection networkInfos, string networkName)
	{
		AssignNetworkInfo(networkInfos, networkName, NetworkHelp.GetAddressType(m_address), isIPv6TunnelAddress: false);
	}

	private unsafe void LoadIPv4Address(ReadOnlyNetworkInfoCollection networkInfos, ClusterResource ipAddressResource)
	{
		PropertyCollection privateProperties = ipAddressResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string name = "Network";
		string networkName = (string)privateProperties.GetProperty(name).Value;
		string name2 = "Address";
		string address = (string)privateProperties.GetProperty(name2).Value;
		string name3 = "SubnetMask";
		string subnet = (string)privateProperties.GetProperty(name3).Value;
		string name4 = "EnableDhcp";
		bool flag = (byte)(((uint)privateProperties.GetProperty(name4).Value != 0) ? 1u : 0u) != 0;
		int prefixLength = NetworkHelp.SubnetMaskToPrefixLength(subnet);
		IPAddress iPAddress = null;
		if (flag)
		{
			ClusterNetwork clusterNetwork = FindNetwork(ipAddressResource.Cluster, networkName);
			if (clusterNetwork != null)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
				iPAddress = NetworkHelp.StringToIPAddress(NetworkHelp.GetNetworkIPv4Address(clusterNetwork, &num));
				prefixLength = (int)num;
			}
		}
		else
		{
			iPAddress = NetworkHelp.StringToIPAddress(address);
		}
		m_prefixLength = (uint)prefixLength;
		m_address = iPAddress;
		m_dhcp = flag;
		AssignNetworkInfo(networkInfos, networkName, NetworkHelp.GetAddressType(iPAddress), isIPv6TunnelAddress: false);
	}

	private ClusterNetwork FindNetwork(Cluster cluster, string networkName)
	{
		foreach (ClusterNetwork network in cluster.GetNetworks())
		{
			if (networkName.Equals(network.Name, StringComparison.CurrentCultureIgnoreCase))
			{
				return network;
			}
		}
		return null;
	}

	private void LoadIPv6Address(ReadOnlyNetworkInfoCollection networkInfos, ClusterResource ipAddressResource)
	{
		PropertyCollection privateProperties = ipAddressResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
		string name = "Network";
		string networkName = (string)privateProperties.GetProperty(name).Value;
		string name2 = "Address";
		string address = (string)privateProperties.GetProperty(name2).Value;
		string name3 = "PrefixLength";
		uint prefixLength = (uint)privateProperties.GetProperty(name3).Value;
		IPAddress iPAddress = NetworkHelp.StringToIPAddress(address);
		m_prefixLength = prefixLength;
		m_address = iPAddress;
		m_dhcp = false;
		AssignNetworkInfo(networkInfos, networkName, NetworkHelp.GetAddressType(iPAddress), isIPv6TunnelAddress: false);
	}

	private void LoadIPv6TunnelAddress(ReadOnlyNetworkInfoCollection networkInfos, ClusterResource ipAddressResource)
	{
		ClusterResource clusterResource = null;
		foreach (ClusterResource dependency in ipAddressResource.GetDependencies())
		{
			if (clusterResource == null)
			{
				string resourceType = "IP Address";
				if (dependency.IsResourceOfType(resourceType))
				{
					clusterResource = dependency;
					break;
				}
			}
		}
		if (clusterResource != null)
		{
			PropertyCollection privateProperties = clusterResource.GetPrivateProperties(PropertyCollectionSet.ReadWrite);
			string name = "Network";
			string networkName = (string)privateProperties.GetProperty(name).Value;
			AssignNetworkInfo(networkInfos, networkName, AddressType.IPv6, isIPv6TunnelAddress: true);
			m_dhcp = false;
			NetworkInfo network = m_network;
			if (network != null)
			{
				m_prefixLength = network.PrefixLength;
			}
			else
			{
				m_prefixLength = 0u;
			}
			PropertyCollection privateProperties2 = ipAddressResource.GetPrivateProperties(PropertyCollectionSet.ReadOnly);
			string name2 = "Address";
			string address = (string)privateProperties2.GetProperty(name2).Value;
			m_address = NetworkHelp.StringToIPAddress(address);
		}
	}

	public IPAddressInfo(IPAddress address, uint prefixLength)
	{
		Construct(null);
		m_prefixLength = prefixLength;
		m_address = address;
	}

	public IPAddressInfo(IPAddressInfo ipAddressInfo)
	{
		m_prefixLength = ipAddressInfo.m_prefixLength;
		m_address = ipAddressInfo.m_address;
		m_network = ipAddressInfo.m_network;
		m_dhcp = ipAddressInfo.m_dhcp;
	}

	internal IPAddressInfo(ReadOnlyNetworkInfoCollection networkInfos, ClusterResource ipAddressResource)
	{
		Construct(null);
		LoadFromIPAddressResource(networkInfos, ipAddressResource);
	}

	internal IPAddressInfo(NetworkInfo network)
	{
		Construct(network);
	}

	internal IPAddressInfo()
	{
		Construct(null);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool DoesMatch(IPAddressInfo ipAddressInfo)
	{
		if (m_prefixLength == ipAddressInfo.m_prefixLength)
		{
			IPAddress address = ipAddressInfo.m_address;
			if (m_address.Equals(address))
			{
				NetworkInfo network = m_network;
				if (network != null)
				{
					NetworkInfo network2 = ipAddressInfo.m_network;
					if (network2 != null)
					{
						NetworkInfo networkInfo = network2;
						if (network.AssociatedNetwork != networkInfo.AssociatedNetwork)
						{
							NetworkInfo network3 = ipAddressInfo.m_network;
							NetworkInfo network4 = m_network;
							if (0 != string.Compare(network4.AssociatedNetwork.Name, network3.AssociatedNetwork.Name, StringComparison.Ordinal))
							{
								goto IL_007c;
							}
						}
						return true;
					}
				}
				return false;
			}
		}
		goto IL_007c;
		IL_007c:
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool DoesNetworkMatch(IPAddressInfo ipAddressInfo)
	{
		if (m_prefixLength == ipAddressInfo.m_prefixLength)
		{
			NetworkInfo network = m_network;
			if (network != null)
			{
				NetworkInfo network2 = ipAddressInfo.m_network;
				if (network2 != null)
				{
					NetworkInfo networkInfo = network2;
					if (network.AssociatedNetwork != networkInfo.AssociatedNetwork)
					{
						NetworkInfo network3 = ipAddressInfo.m_network;
						NetworkInfo network4 = m_network;
						if (0 != string.Compare(network4.AssociatedNetwork.Name, network3.AssociatedNetwork.Name, StringComparison.OrdinalIgnoreCase))
						{
							goto IL_0065;
						}
					}
					return true;
				}
			}
			return false;
		}
		goto IL_0065;
		IL_0065:
		return false;
	}
}
