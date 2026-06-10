using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace KDDSL.ServerClusters;

public sealed class NetworkHelp
{
	private static int NetBiosNameLength => 15;

	private NetworkHelp()
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe static bool IsIPv6AddressIstap(IPAddress ipAddress)
	{
		in6_addr iN6_ADDR = GetIN6_ADDR(ipAddress);
		return (global::_003CModule_003E.IN6_IS_ADDR_ISATAP(&iN6_ADDR) != 0) ? true : false;
	}

	private unsafe static in6_addr GetIN6_ADDR(IPAddress ipAddress)
	{
		//IL_0012: Expected I4, but got I8
		//IL_002c: Expected I, but got I8
		byte[] addressBytes = ipAddress.GetAddressBytes();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out in6_addr in6_addr);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref in6_addr, 0, 16);
		int num = 0;
		int num2 = addressBytes.Length;
		if (0 < num2)
		{
			in6_addr* ptr = &in6_addr;
			do
			{
				*(byte*)ptr = addressBytes[num];
				num++;
				ptr = (in6_addr*)((ulong)(nint)ptr + 1uL);
			}
			while (num < num2);
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out in6_addr result);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(ref result, ref in6_addr, 16);
		return result;
	}

	private unsafe static _SERVER_TRANSPORT_INFO_0* NetServerTransportEnum(ushort* pszServerName, uint* pStatus)
	{
		//IL_0003: Expected I, but got I8
		_SERVER_TRANSPORT_INFO_0* result = null;
		uint num = 0u;
		*pStatus = global::_003CModule_003E.NetServerTransportEnum(pszServerName, 0u, (byte**)(&result), uint.MaxValue, &num, &num, &num);
		return result;
	}

	internal static AddressType GetAddressType(IPAddress ipAddress)
	{
		if (ipAddress == null)
		{
			return AddressType.Unknown;
		}
		return ipAddress.AddressFamily switch
		{
			AddressFamily.InterNetworkV6 => AddressType.IPv6, 
			AddressFamily.InterNetwork => AddressType.IPv4, 
			_ => throw new InvalidOperationException(), 
		};
	}

	internal unsafe static string GetNetworkIPv4Address(ClusterNetwork network, [Out] uint* prefixLength)
	{
		//Discarded unreachable code: IL_009a, IL_009c
		string text = null;
		PropertyCollection propertyCollection = null;
		StringCollection stringCollection = null;
		StringCollection stringCollection2 = null;
		string text2 = null;
		Exception ex = null;
		text = string.Empty;
		try
		{
			propertyCollection = network.GetCommonProperties(PropertyCollectionSet.ReadOnly);
			stringCollection = (StringCollection)propertyCollection["IPv4Addresses"].Value;
			if (stringCollection.Count != 0)
			{
				text = stringCollection[0];
				stringCollection2 = (StringCollection)propertyCollection["IPv4PrefixLengths"].Value;
				text2 = stringCollection2[0];
				*prefixLength = uint.Parse(text2, CultureInfo.InvariantCulture);
			}
			return text;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.NetworkHelp_GetNetworkIPv4Address_Fail_Text,
				network.Name
			});
		}
	}

	internal static int AddressFromPrefixLength(uint prefixLength)
	{
		int num = 0;
		uint num2 = 32u;
		do
		{
			num <<= 1;
			if (prefixLength != 0)
			{
				num |= 1;
				prefixLength += uint.MaxValue;
			}
			num2 += uint.MaxValue;
		}
		while (num2 != 0);
		return IPAddress.HostToNetworkOrder(num);
	}

	internal static int AddressFromIPAddress(IPAddress ipAddress)
	{
		return (int)ipAddress.Address;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool IsIPv6AddressTunneled(IPAddress ipAddress)
	{
		in6_addr iN6_ADDR = GetIN6_ADDR(ipAddress);
		return (global::_003CModule_003E.IN6_IS_ADDR_ISATAP(&iN6_ADDR) != 0) ? true : false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool IsIPv6AddressLinkLocal(IPAddress ipAddress)
	{
		in6_addr iN6_ADDR = GetIN6_ADDR(ipAddress);
		return (global::_003CModule_003E.IN6_IS_ADDR_LINKLOCAL(&iN6_ADDR) != 0) ? true : false;
	}

	public static IPAddress StringToIPAddress(string address)
	{
		if (address.Equals("::", StringComparison.Ordinal))
		{
			return IPAddress.IPv6None;
		}
		return IPAddress.Parse(address);
	}

	public static int SubnetMaskToPrefixLength(string subnet)
	{
		IPAddress iPAddress = IPAddress.Parse(subnet);
		int num = 0;
		byte[] addressBytes = iPAddress.GetAddressBytes();
		int num2 = 0;
		int num3 = addressBytes.Length;
		if (0 < num3)
		{
			do
			{
				byte b = addressBytes[num2];
				if (b != 0)
				{
					do
					{
						if ((byte)(b & 1u) != 0)
						{
							num++;
						}
						b = (byte)((uint)b >> 1);
					}
					while (b != 0);
				}
				num2++;
			}
			while (num2 < num3);
		}
		return num;
	}

	public static string PrefixLengthToSubnetMask(uint prefixLength)
	{
		int num = AddressFromPrefixLength(prefixLength);
		return string.Format(args: new object[4]
		{
			num & 0xFF,
			(num >> 8) & 0xFF,
			(num >> 16) & 0xFF,
			(num >> 24) & 0xFF
		}, provider: CultureInfo.InvariantCulture, format: "{0}.{1}.{2}.{3}");
	}

	public static IPAddress GetDefaultAddressForNetwork(IPAddress networkAddress)
	{
		byte[] addressBytes = networkAddress.GetAddressBytes();
		addressBytes[3] = (byte)(addressBytes[3] | 1);
		return new IPAddress(addressBytes);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsAddressInNetwork(IPAddress ipAddress, IPAddress networkAddress, uint prefixLength)
	{
		int num = (int)ipAddress.Address;
		int num2 = (int)networkAddress.Address;
		return (AddressFromPrefixLength(prefixLength) & num) == num2;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool DoesNetworkSupportDhcp(ClusterNetwork network)
	{
		bool result = false;
		ClusterNetworkInterfaceCollection clusterNetworkInterfaceCollection = network.GetNetworkInterfaces();
		try
		{
			IEnumerator<ClusterNetworkInterface> enumerator = clusterNetworkInterfaceCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if ((uint)enumerator.Current.GetCommonProperties(PropertyCollectionSet.ReadOnly)["DhcpEnabled"].Value != 0)
					{
						result = true;
						break;
					}
				}
			}
			finally
			{
				IEnumerator<ClusterNetworkInterface> enumerator2 = enumerator;
				IDisposable disposable = enumerator;
				enumerator?.Dispose();
			}
		}
		finally
		{
			IDisposable disposable2 = clusterNetworkInterfaceCollection as IDisposable;
			if (disposable2 != null)
			{
				disposable2.Dispose();
			}
		}
		return result;
	}

	public static IDictionary<string, string> GetClusterNetworkNames(Cluster cluster, ClusterGroup group, ClusterNetworkNameSet set)
	{
		//Discarded unreachable code: IL_0164, IL_0166
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		if ((set == ClusterNetworkNameSet.ExcludeGroupName || set == ClusterNetworkNameSet.ExcludeGroupNetNames) && group == null)
		{
			throw new ArgumentNullException("group");
		}
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			Dictionary<Guid, string> currentGroupNames = cluster.GetCurrentGroupNames();
			try
			{
				Dictionary<Guid, string>.ValueCollection.Enumerator enumerator = currentGroupNames.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (set != ClusterNetworkNameSet.ExcludeGroupName || 0 != string.Compare(group.Name, current, StringComparison.OrdinalIgnoreCase))
					{
						dictionary[current] = current;
					}
				}
			}
			finally
			{
				Dictionary<Guid, string> dictionary2 = currentGroupNames;
				if (currentGroupNames is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			string resourceType = "Network Name";
			ClusterResourceCollection resources = cluster.GetResources(resourceType);
			try
			{
				IEnumerator<ClusterResource> enumerator2 = resources.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ClusterResource current2 = enumerator2.Current;
						if (set != ClusterNetworkNameSet.ExcludeGroupNetNames || 0 != string.Compare(group.Name, current2.GetOwnerGroup().Name, StringComparison.OrdinalIgnoreCase))
						{
							string text = (string)current2.GetPrivateProperties(PropertyCollectionSet.ReadWrite)["DnsName"].Value;
							dictionary[text] = text;
							dictionary[current2.Name] = current2.Name;
						}
					}
				}
				finally
				{
					IEnumerator<ClusterResource> enumerator3 = enumerator2;
					IDisposable disposable2 = enumerator2;
					enumerator2?.Dispose();
				}
			}
			finally
			{
				ClusterResourceCollection clusterResourceCollection = resources;
				if (resources is IDisposable disposable3)
				{
					disposable3.Dispose();
				}
			}
			return dictionary;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.NetworkHelp_GetNetworkNames_Fail_Text,
				cluster.Name
			});
		}
	}

	public static IDictionary<string, string> GetGroupNetworkNames(ClusterGroup group)
	{
		//Discarded unreachable code: IL_00d5, IL_00d7
		if (group == null)
		{
			throw new ArgumentNullException("group");
		}
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			ClusterResourceCollection resources = group.GetResources();
			try
			{
				IEnumerator<ClusterResource> enumerator = resources.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ClusterResource current = enumerator.Current;
						string resourceType = "Network Name";
						if (current.IsResourceOfType(resourceType))
						{
							string text = (string)current.GetPrivateProperties(PropertyCollectionSet.ReadWrite)["Name"].Value;
							dictionary[text] = text;
						}
						dictionary[current.Name] = current.Name;
					}
				}
				finally
				{
					IEnumerator<ClusterResource> enumerator2 = enumerator;
					IDisposable disposable = enumerator;
					enumerator?.Dispose();
				}
			}
			finally
			{
				ClusterResourceCollection clusterResourceCollection = resources;
				if (resources is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
			}
			return dictionary;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.NetworkHelp_GetGroupNetworkNames_Fail_Text,
				group.Name
			});
		}
	}

	public static IDictionary<string, string> GetNetBiosNetworkNames(IDictionary<string, string> clusterNetworkNames)
	{
		if (clusterNetworkNames == null)
		{
			throw new ArgumentNullException("clusterNetworkNames");
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (string key in clusterNetworkNames.Keys)
		{
			string netBiosName = GetNetBiosName(key);
			dictionary[netBiosName] = netBiosName;
		}
		return dictionary;
	}

	public static string GetNetBiosName(string networkName)
	{
		if (networkName == null)
		{
			throw new ArgumentNullException("networkName");
		}
		return (!IsNetBiosName(networkName)) ? networkName.Substring(0, 15) : networkName;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsNetBiosName(string networkName)
	{
		if (networkName == null)
		{
			throw new ArgumentNullException("networkName");
		}
		return networkName.Length <= 15;
	}

	[HandleProcessCorruptedStateExceptions]
	public unsafe static uint TryNetServerTransportEnum(string server)
	{
		//Discarded unreachable code: IL_0055
		//IL_0003: Expected I, but got I8
		ushort* ptr = null;
		try
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			ptr = InteropHelp.StringToWstr(server);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			SafeNetApiBuffer safeNetApiBuffer = new SafeNetApiBuffer(NetServerTransportEnum(ptr, &num));
			SafeNetApiBuffer safeNetApiBuffer2 = safeNetApiBuffer;
			IDisposable disposable = safeNetApiBuffer;
			((IDisposable)safeNetApiBuffer)?.Dispose();
			if (num == 234)
			{
				return 0u;
			}
			return num;
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	public static string BuildFqdn(string networkName, string domainName)
	{
		if (string.IsNullOrEmpty(networkName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "networkName");
		}
		if (string.IsNullOrEmpty(domainName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "domainName");
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", networkName, domainName);
	}
}
