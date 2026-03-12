using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Xml;

namespace MS.Internal.ServerClusters;

public class NetworkInfo
{
	private bool m_supportsDhcp;

	private IPAddressInfo m_address;

	private ClusterNetwork m_network;

	private NetworkRole m_role;

	public ClusterNetwork AssociatedNetwork => m_network;

	public string Name
	{
		get
		{
			uint prefixLength = m_address.PrefixLength;
			IPAddress address = m_address.Address;
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", address, prefixLength);
		}
	}

	public bool SupportsDhcp
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_supportsDhcp;
		}
	}

	public uint PrefixLength => m_address.PrefixLength;

	public IPAddress Address => m_address.Address;

	public bool IsTunneled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			if (m_address.AddressType == AddressType.IPv6)
			{
				return NetworkHelp.IsIPv6AddressTunneled(m_address.Address);
			}
			return false;
		}
	}

	public AddressType AddressType => m_address.AddressType;

	public bool IsPublic
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			NetworkRole role = m_role;
			if (role != NetworkRole.ClientAccess && role != NetworkRole.InternalAndClient)
			{
				return false;
			}
			return true;
		}
	}

	private NetworkInfo(NetworkRole role, IPAddress address, int prefixLength, [MarshalAs(UnmanagedType.U1)] bool supportsDhcp)
	{
		Construct(null, role, address, prefixLength, supportsDhcp);
	}

	private NetworkInfo(ClusterNetwork network, IPAddress address, int prefixLength, [MarshalAs(UnmanagedType.U1)] bool supportsDhcp)
	{
		Construct(network, network.Role, address, prefixLength, supportsDhcp);
	}

	private NetworkInfo(ClusterNetwork network, string address, int prefixLength, [MarshalAs(UnmanagedType.U1)] bool supportsDhcp)
	{
		IPAddress address2 = IPAddress.Parse(address);
		Construct(network, network.Role, address2, prefixLength, supportsDhcp);
	}

	private void Construct(ClusterNetwork network, NetworkRole role, IPAddress address, int prefixLength, [MarshalAs(UnmanagedType.U1)] bool supportsDhcp)
	{
		m_network = network;
		m_role = role;
		(m_address = new IPAddressInfo()).Address = address;
		m_address.PrefixLength = (uint)prefixLength;
		m_supportsDhcp = supportsDhcp;
	}

	private static NetworkRole GetRole(XmlReader xmlReader)
	{
		return (NetworkRole)int.Parse(xmlReader.ReadString(), CultureInfo.InvariantCulture);
	}

	private static Collection<IPAddress> GetPrefixes(XmlReader xmlReader)
	{
		Collection<IPAddress> collection = new Collection<IPAddress>();
		if (xmlReader.Read())
		{
			while (xmlReader.IsStartElement("string"))
			{
				IPAddress item = IPAddress.Parse(xmlReader.ReadString());
				collection.Add(item);
				if (!xmlReader.Read())
				{
					break;
				}
			}
		}
		return collection;
	}

	private static Collection<uint> GetPrefixLengths(XmlReader xmlReader)
	{
		Collection<uint> collection = new Collection<uint>();
		if (xmlReader.Read())
		{
			while (xmlReader.IsStartElement("int"))
			{
				uint item = uint.Parse(xmlReader.ReadString(), CultureInfo.InvariantCulture);
				collection.Add(item);
				if (!xmlReader.Read())
				{
					break;
				}
			}
		}
		return collection;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool GetDhcpEnabled(XmlReader xmlReader)
	{
		string strB = xmlReader.ReadString();
		bool flag = false;
		return 0 == string.Compare("true", strB, StringComparison.OrdinalIgnoreCase) || flag;
	}

	private static Collection<NetworkInfo> BuildNetworkCollection(NetworkRole role, Collection<IPAddress> prefixes, Collection<uint> prefixLengths, [MarshalAs(UnmanagedType.U1)] bool dhcpEnabled)
	{
		Collection<NetworkInfo> collection = new Collection<NetworkInfo>();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		int num = 0;
		if (0 < prefixes.Count)
		{
			do
			{
				IPAddress iPAddress = prefixes[num];
				uint prefixLength = prefixLengths[num];
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork && !flag)
				{
					NetworkInfo item = new NetworkInfo(role, iPAddress, (int)prefixLength, dhcpEnabled);
					collection.Add(item);
					flag = true;
					goto IL_009b;
				}
				if (iPAddress.AddressFamily == AddressFamily.InterNetworkV6 && !NetworkHelp.IsIPv6AddressLinkLocal(iPAddress))
				{
					if (NetworkHelp.IsIPv6AddressTunneled(iPAddress))
					{
						if (!flag3)
						{
							flag3 = true;
							goto IL_0082;
						}
					}
					else if (!flag2)
					{
						flag2 = true;
						goto IL_0082;
					}
				}
				goto IL_0097;
				IL_0082:
				NetworkInfo item2 = new NetworkInfo(role, iPAddress, (int)prefixLength, supportsDhcp: false);
				collection.Add(item2);
				goto IL_0097;
				IL_009b:
				if (flag2 && flag3)
				{
					break;
				}
				goto IL_00a1;
				IL_00a1:
				num++;
				continue;
				IL_0097:
				if (flag)
				{
					goto IL_009b;
				}
				goto IL_00a1;
			}
			while (num < prefixes.Count);
		}
		return collection;
	}

	private static void MergeNetworkInfoCollections(Collection<NetworkInfo> one, ICollection<NetworkInfo> two)
	{
		foreach (NetworkInfo item in two)
		{
			one.Add(item);
		}
	}

	private IPAddressInfo CreateIPAddressInfo(IPAddress ipAddress, [MarshalAs(UnmanagedType.U1)] bool useDhcp)
	{
		IPAddressInfo iPAddressInfo = new IPAddressInfo(this);
		iPAddressInfo.Address = ipAddress;
		iPAddressInfo.PrefixLength = m_address.PrefixLength;
		iPAddressInfo.UseDhcp = useDhcp;
		return iPAddressInfo;
	}

	internal static ICollection<NetworkInfo> GetXmlNetworkInfo(XmlReader xmlReader)
	{
		Collection<NetworkInfo> result = new Collection<NetworkInfo>();
		if (xmlReader.IsStartElement() && 0 == string.Compare("Network", xmlReader.Name, StringComparison.Ordinal))
		{
			Collection<IPAddress> prefixes = null;
			Collection<uint> prefixLengths = null;
			bool dhcpEnabled = false;
			NetworkRole role = NetworkRole.None;
			xmlReader.Read();
			while (XmlNodeType.EndElement != xmlReader.NodeType || (XmlNodeType.EndElement == xmlReader.NodeType && 0 != string.Compare("Network", xmlReader.Name, StringComparison.Ordinal)))
			{
				if (xmlReader.IsStartElement())
				{
					if (0 == string.Compare("Prefixes", xmlReader.Name, StringComparison.Ordinal))
					{
						prefixes = GetPrefixes(xmlReader);
					}
					else if (0 == string.Compare("PrefixLengths", xmlReader.Name, StringComparison.Ordinal))
					{
						prefixLengths = GetPrefixLengths(xmlReader);
					}
					else if (0 == string.Compare("NetworkRole", xmlReader.Name, StringComparison.Ordinal))
					{
						role = GetRole(xmlReader);
					}
					else if (0 == string.Compare("DhcpEnabled", xmlReader.Name, StringComparison.Ordinal))
					{
						dhcpEnabled = GetDhcpEnabled(xmlReader);
					}
					else
					{
						xmlReader.Skip();
					}
				}
				else
				{
					xmlReader.Read();
				}
			}
			result = BuildNetworkCollection(role, prefixes, prefixLengths, dhcpEnabled);
		}
		return result;
	}

	internal void ForcePublic()
	{
		m_role = NetworkRole.InternalAndClient;
	}

	public static ICollection<NetworkInfo> GetClusterNetworkInfo(Cluster cluster)
	{
		//Discarded unreachable code: IL_0062
		Collection<NetworkInfo> collection = new Collection<NetworkInfo>();
		try
		{
			IEnumerator<ClusterNetwork> enumerator = cluster.GetNetworks().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ICollection<NetworkInfo> networkInfoFromNetwork = GetNetworkInfoFromNetwork(enumerator.Current);
					MergeNetworkInfoCollections(collection, networkInfoFromNetwork);
				}
				return collection;
			}
			finally
			{
				IEnumerator<ClusterNetwork> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.NetworkInfo_GetNetworks_Fail_Text,
				cluster.Name
			});
		}
	}

	public static ICollection<NetworkInfo> GetPublicClusterNetworkInfo(Cluster cluster)
	{
		ICollection<NetworkInfo> clusterNetworkInfo = GetClusterNetworkInfo(cluster);
		Collection<NetworkInfo> collection = new Collection<NetworkInfo>();
		foreach (NetworkInfo item in clusterNetworkInfo)
		{
			if (item.IsPublic)
			{
				collection.Add(item);
			}
		}
		return collection;
	}

	public static ICollection<NetworkInfo> GetNetworkInfoFromNetwork(ClusterNetwork network)
	{
		Collection<NetworkInfo> collection = new Collection<NetworkInfo>();
		bool supportsDhcp = NetworkHelp.DoesNetworkSupportDhcp(network);
		PropertyCollection commonProperties = network.GetCommonProperties(PropertyCollectionSet.ReadOnly);
		string name = "IPv4Addresses";
		StringCollection stringCollection = (StringCollection)commonProperties.GetProperty(name).Value;
		string name2 = "IPv4PrefixLengths";
		StringCollection stringCollection2 = (StringCollection)commonProperties.GetProperty(name2).Value;
		if (stringCollection.Count != 0)
		{
			int prefixLength = int.Parse(stringCollection2[0], CultureInfo.InvariantCulture);
			NetworkInfo item = new NetworkInfo(network, stringCollection[0], prefixLength, supportsDhcp);
			collection.Add(item);
		}
		string name3 = "IPv6Addresses";
		StringCollection stringCollection3 = (StringCollection)commonProperties.GetProperty(name3).Value;
		string name4 = "IPv6PrefixLengths";
		StringCollection stringCollection4 = (StringCollection)commonProperties.GetProperty(name4).Value;
		bool flag = false;
		bool flag2 = false;
		if (stringCollection3.Count != 0)
		{
			int num = 0;
			if (0 < stringCollection3.Count)
			{
				do
				{
					int prefixLength2 = int.Parse(stringCollection4[num], CultureInfo.InvariantCulture);
					IPAddress iPAddress = IPAddress.Parse(stringCollection3[num]);
					if (!NetworkHelp.IsIPv6AddressLinkLocal(iPAddress))
					{
						bool flag3 = NetworkHelp.IsIPv6AddressTunneled(iPAddress);
						if (!flag && !flag3)
						{
							flag = true;
						}
						else if (!flag2)
						{
							flag2 = flag3 || flag2;
						}
						NetworkInfo item2 = new NetworkInfo(network, iPAddress, prefixLength2, supportsDhcp: false);
						collection.Add(item2);
					}
					num++;
				}
				while (num < stringCollection3.Count);
			}
		}
		return collection;
	}

	public IPAddressInfo CreateAutomaticIPAddress(NetworkConfigurationOptions options)
	{
		if (0 != (options & NetworkConfigurationOptions.IPv4Dhcp) && m_address.AddressType == AddressType.IPv4 && m_supportsDhcp)
		{
			return CreateDhcpIPv4AddressInfo();
		}
		if (m_address.AddressType == AddressType.IPv6)
		{
			return CreateIPv6AddressInfo(options);
		}
		if (m_address.AddressType == AddressType.IPv4 && (options & (NetworkConfigurationOptions.RoleRequiresIPv4Static | NetworkConfigurationOptions.IPv4Static)) == NetworkConfigurationOptions.IPv4Static && !m_supportsDhcp)
		{
			return CreateStaticIPv4AddressInfo(m_address.Address);
		}
		return null;
	}

	public IPAddressInfo CreateStaticIPv4AddressInfo(IPAddress ipAddress)
	{
		if (ipAddress == null)
		{
			throw new ArgumentNullException("ipAddress");
		}
		if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.IPAddressInfo_WrongAddressType_Text });
		}
		if (m_address.AddressType != AddressType.IPv4)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.NetworkInfo_WrongAddressType_Text });
		}
		return CreateIPAddressInfo(ipAddress, useDhcp: false);
	}

	public IPAddressInfo CreateDhcpIPv4AddressInfo()
	{
		if (m_address.AddressType != AddressType.IPv4)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.NetworkInfo_WrongAddressType_Text });
		}
		return CreateIPAddressInfo(m_address.Address, useDhcp: true);
	}

	public IPAddressInfo CreateIPv6AddressInfo(NetworkConfigurationOptions options)
	{
		IPAddressInfo result = null;
		if (m_address.AddressType != AddressType.IPv6)
		{
			throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.NetworkInfo_WrongAddressType_Text });
		}
		if (0 != (options & NetworkConfigurationOptions.IPv6) && !m_address.IsTunneled)
		{
			result = CreateIPAddressInfo(m_address.Address, useDhcp: false);
		}
		else if (0 != (options & NetworkConfigurationOptions.IPv6Tunneled) && m_address.IsTunneled)
		{
			result = CreateIPAddressInfo(m_address.Address, useDhcp: false);
		}
		return result;
	}
}
