#define DEBUG
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public sealed class DnsSupport
{
	private sealed class DnsEntry
	{
		private string m_hostName;

		private Collection<IPAddress> m_addresses;

		public ICollection<IPAddress> Addresses => m_addresses;

		public string HostName => m_hostName;

		public DnsEntry()
		{
			m_hostName = string.Empty;
			m_addresses = new Collection<IPAddress>();
		}

		public unsafe void AddDnsRecord(_DnsRecordW* pDnsRecord)
		{
			//IL_0020: Expected I, but got I8
			//IL_008b: Expected I, but got I8
			//IL_0099: Expected I, but got I8
			//IL_00b6: Expected I, but got I8
			_DnsRecordW* ptr = pDnsRecord;
			if (pDnsRecord == null)
			{
				return;
			}
			do
			{
				if (m_hostName.Length == 0)
				{
					m_hostName = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)ptr + 8uL)));
				}
				ushort num = *(ushort*)((ulong)(nint)ptr + 16uL);
				if (num == 1)
				{
					uint num2 = *(uint*)((ulong)(nint)ptr + 20uL) & 3u;
					if (num2 == 0 || num2 == 1)
					{
						IPAddress item = new IPAddress((uint)(*(int*)((ulong)(nint)ptr + 32uL)));
						m_addresses.Add(item);
						goto IL_00b3;
					}
				}
				if (num == 28)
				{
					uint num3 = *(uint*)((ulong)(nint)ptr + 20uL) & 3u;
					if (num3 == 0 || num3 == 1)
					{
						long num4 = (long)(nint)ptr + 32L;
						byte[] array = new byte[16];
						int num5 = 0;
						DNS_AAAA_DATA* ptr2 = (DNS_AAAA_DATA*)num4;
						do
						{
							array[num5] = *(byte*)ptr2;
							num5++;
							ptr2 = (DNS_AAAA_DATA*)((ulong)(nint)ptr2 + 1uL);
						}
						while (num5 < 16);
						IPAddress item2 = new IPAddress(array);
						m_addresses.Add(item2);
					}
				}
				goto IL_00b3;
				IL_00b3:
				ptr = (_DnsRecordW*)(*(ulong*)ptr);
			}
			while (ptr != null);
		}

		public void FetchHostName(string computerName)
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(computerName);
			m_hostName = hostEntry.HostName;
		}
	}

	public static int MaxDnsComputerNameLength = 63;

	public static int MaxNetBiosComputerNameLength = 15;

	private DnsSupport()
	{
	}

	private unsafe static _DOMAIN_CONTROLLER_INFOW* GetDcName(string domainName, [MarshalAs(UnmanagedType.U1)] bool forceDiscovery)
	{
		//IL_0004: Expected I, but got I8
		//IL_0007: Expected I, but got I8
		//IL_0030: Expected I, but got I8
		//IL_0030: Expected I, but got I8
		//IL_0030: Expected I, but got I8
		//IL_0049: Expected I, but got I8
		//IL_0049: Expected I, but got I8
		//IL_0049: Expected I, but got I8
		_DOMAIN_CONTROLLER_INFOW* result = null;
		ushort* ptr = null;
		uint num = 1073745936u;
		num = (forceDiscovery ? 1073745937u : num);
		try
		{
			ptr = InteropHelp.StringToWstr(domainName);
			if (global::_003CModule_003E.DsGetDcNameW(null, ptr, null, null, num, &result) != 0)
			{
				num &= 0xFFFFEFFFu;
				uint num2 = global::_003CModule_003E.DsGetDcNameW(null, ptr, null, null, num, &result);
				if (num2 != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
					{
						Resources.Dns_ErrorResolvingName_Text,
						domainName
					});
				}
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		return result;
	}

	private static DnsEntry GetDnsEntry(string computerName)
	{
		IPAddress iPAddress = null;
		DnsEntry dnsEntry = new DnsEntry();
		iPAddress = null;
		if (IPAddress.TryParse(computerName, out iPAddress))
		{
			dnsEntry.FetchHostName(computerName);
			computerName = dnsEntry.HostName;
		}
		PopulateDnsEntry(computerName, 1, dnsEntry);
		PopulateDnsEntry(computerName, 28, dnsEntry);
		return dnsEntry;
	}

	private unsafe static void PopulateDnsEntry(string computerName, ushort wType, DnsEntry dnsEntry)
	{
		//IL_0020: Expected I, but got I8
		//IL_0023: Expected I, but got I8
		//IL_0038: Expected I, but got I8
		//IL_0038: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		if (string.IsNullOrEmpty(computerName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "computerName");
		}
		ushort* ptr = null;
		_DnsRecordW* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(computerName);
			int num = global::_003CModule_003E.DnsQuery_W(ptr, wType, 8u, null, &ptr2, null);
			if (0 == num)
			{
				dnsEntry.AddDnsRecord(ptr2);
			}
			else if (9501 != num && 9003 != num)
			{
				if (1460 == num)
				{
					throw ExceptionHelp.Build<ApplicationException>(new string[1] { Resources.DnsTimeout_Text });
				}
				throw ExceptionHelp.Build<ApplicationException>(num, new string[2]
				{
					Resources.Dns_ErrorResolvingName_Text,
					computerName
				});
			}
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
			if (ptr2 != null)
			{
				global::_003CModule_003E.DnsFree(ptr2, (DNS_FREE_TYPE)1);
			}
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool IsIpAddress(string computerName)
	{
		IPAddress address = null;
		return IPAddress.TryParse(computerName, out address);
	}

	private unsafe static string GetBuiltinAccountName(string nodeName, WELL_KNOWN_SID_TYPE wellKnownSid)
	{
		//Discarded unreachable code: IL_0077
		//IL_0003: Expected I, but got I8
		ushort* ptr = null;
		ushort* ptr2 = InteropHelp.StringToWstr(nodeName);
		try
		{
			uint num = global::_003CModule_003E.ClRtlGetWellKnownUserName(wellKnownSid, &ptr, ptr2);
			if (num != 0)
			{
				DebugLog.LogError("Getting built-in account name for {0} from node {1} failed with error {2}", (uint)wellKnownSid, nodeName, num);
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[1] { Resources.Dns_ErrorValidatingUsersGroup_Text });
			}
			return InteropHelp.WstrToString(ptr);
		}
		finally
		{
			if (ptr != null)
			{
				global::_003CModule_003E.LocalFree(ptr);
			}
			if (ptr2 != null)
			{
				InteropHelp.FreeWstr(ptr2);
			}
		}
	}

	public unsafe static string GetDomainControllerName(string domainName, [MarshalAs(UnmanagedType.U1)] bool forceDiscovery)
	{
		//IL_0032: Expected I, but got I8
		if (domainName == null)
		{
			throw new ArgumentNullException("domainName");
		}
		ThreadWatchdog.PerformUIThreadCheck();
		SafeNetApiBuffer safeNetApiBuffer = new SafeNetApiBuffer(GetDcName(domainName, forceDiscovery));
		string text = InteropHelp.WstrToString((ushort*)(*(ulong*)safeNetApiBuffer.DangerousDcInfoPointer()));
		((IDisposable)safeNetApiBuffer)?.Dispose();
		if (text.Length > 2 && text.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
		{
			return text.Substring(2);
		}
		return text;
	}

	public static string CanonizeComputerName(string computerName)
	{
		//Discarded unreachable code: IL_005e, IL_0060
		try
		{
			computerName = PromoteLocalComputerName(computerName);
			DnsEntry dnsEntry = GetDnsEntry(computerName);
			if (string.IsNullOrEmpty(dnsEntry.HostName))
			{
				throw ExceptionHelp.Build<ApplicationException>(9501, new string[2]
				{
					Resources.CanonizeComputerName_Failed_Text,
					computerName
				});
			}
			return dnsEntry.HostName;
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[2]
			{
				Resources.CanonizeComputerName_Failed_Text,
				computerName
			});
		}
	}

	public static string GetNodeDomain(string nodeName)
	{
		if (nodeName == null)
		{
			throw new ArgumentNullException("nodeName");
		}
		string text = WmiHelper.GetNodeFullyQualifiedDomainName(nodeName);
		if (string.CompareOrdinal(text, nodeName) == 0)
		{
			text = CanonizeComputerName(nodeName);
		}
		int num = text.IndexOf(".", StringComparison.OrdinalIgnoreCase);
		if (0 < num && num < text.Length - 1)
		{
			return text.Substring(num + 1);
		}
		return string.Empty;
	}

	public static string GetClusterDomain(ICollection<string> nodeNames)
	{
		//Discarded unreachable code: IL_0074
		if (nodeNames == null)
		{
			throw new ArgumentNullException("nodeNames");
		}
		if (nodeNames.Count == 0)
		{
			throw new ArgumentOutOfRangeException("nodeNames");
		}
		int num = 0;
		foreach (string nodeName in nodeNames)
		{
			try
			{
				return GetNodeDomain(nodeName);
			}
			catch (Exception caughtException)
			{
				ExceptionHelp.LogException(caughtException, "Exception querying domain from node '{0}'", nodeName);
				if (num == nodeNames.Count - 1)
				{
					throw;
				}
			}
			num++;
		}
		throw new InvalidOperationException();
	}

	public static string PromoteLocalComputerName(string computerName)
	{
		if (0 == string.Compare(".", computerName, StringComparison.OrdinalIgnoreCase) || 0 == string.Compare("localhost", computerName, StringComparison.OrdinalIgnoreCase))
		{
			computerName = Environment.MachineName;
		}
		return computerName;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool IsValidDnsName(string dnsName)
	{
		//IL_0020: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		if (string.IsNullOrEmpty(dnsName))
		{
			throw new ArgumentException(Resources.Argument_NullOrEmpty_Text, "dnsName");
		}
		ushort* ptr = null;
		int num;
		try
		{
			ptr = InteropHelp.StringToWstr(dnsName);
			num = global::_003CModule_003E.DnsValidateName_W(ptr, (_DNS_NAME_FORMAT)3);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
		if (9556 == num)
		{
			return true;
		}
		return 0 == num;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsNetworkNameReady(string serverName)
	{
		if (!NetworkHelper.CanPing(serverName))
		{
			return false;
		}
		uint num = NetworkHelp.TryNetServerTransportEnum(serverName);
		switch (num)
		{
		default:
			throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
			{
				Resources.NetworkNameNotReadyFailureFormat_Text,
				serverName
			});
		case 5u:
		case 53u:
			return false;
		case 0u:
			return true;
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsComputerNameFullyQualified(string computerName)
	{
		if (computerName == null)
		{
			throw new ArgumentNullException("computerName");
		}
		return computerName.Contains(".");
	}

	public static void ParseFullyQualifiedComputerName(string computerName, out string shortName, out string domainName)
	{
		if (computerName == null)
		{
			throw new ArgumentNullException("computerName");
		}
		if (!IsComputerNameFullyQualified(computerName))
		{
			shortName = computerName;
			domainName = "";
			return;
		}
		int num = computerName.IndexOf('.');
		byte condition = ((num != -1) ? ((byte)1) : ((byte)0));
		Debug.Assert(condition != 0);
		shortName = computerName.Substring(0, num);
		domainName = computerName.Substring(num + 1);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool IsDcReadOnly(string dcName)
	{
		//Discarded unreachable code: IL_0047
		//IL_0003: Expected I, but got I8
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(dcName);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out int num);
			uint num2 = global::_003CModule_003E.ClRtlIsDomainControllerReadOnly(ptr, &num);
			if (num2 != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
				{
					Resources.Dns_ErrorResolvingName_Text,
					dcName
				});
			}
			if (num != 0)
			{
				return true;
			}
			return false;
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	public unsafe static string IsCliUsrMemberOfUsersGroup(string nodeName, ref bool foundCliusr)
	{
		//Discarded unreachable code: IL_00e4
		//IL_0006: Expected I, but got I8
		//IL_0021: Expected I, but got I8
		//IL_0038: Expected I, but got I8
		//IL_00a8: Expected I, but got I8
		foundCliusr = false;
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			string builtinAccountName = GetBuiltinAccountName(nodeName, (WELL_KNOWN_SID_TYPE)27);
			uint num = 0u;
			uint num2 = 0u;
			byte* handle = null;
			uint num3 = global::_003CModule_003E.NetLocalGroupGetMembers(ptr, InteropHelp.StringToWstr(builtinAccountName), 1u, &handle, uint.MaxValue, &num, &num2, null);
			if (num3 != 0)
			{
				DebugLog.LogError("Getting members of built-in users group from node {0} failed with error {1}", nodeName, num3);
				throw ExceptionHelp.Build<ApplicationException>((int)num3, new string[1] { Resources.Dns_ErrorValidatingUsersGroup_Text });
			}
			SafeNetApiBuffer safeNetApiBuffer = new SafeNetApiBuffer((_LOCALGROUP_MEMBERS_INFO_1*)handle);
			for (uint num4 = 0u; num4 < num; num4++)
			{
				long num5 = (long)num4 * 24L;
				if (global::_003CModule_003E._wcsicmp((ushort*)(*(ulong*)(num5 + (nint)safeNetApiBuffer.DangerousLocalGroupMembersInfo1Pointer() + 16)), (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1O_0040LCICMCNL_0040_003F_0024AAC_003F_0024AAL_003F_0024AAI_003F_0024AAU_003F_0024AAS_003F_0024AAR_0040)) == 0 && *(int*)(num5 + (nint)safeNetApiBuffer.DangerousLocalGroupMembersInfo1Pointer() + 8) == 1)
				{
					foundCliusr = true;
					break;
				}
			}
			SafeNetApiBuffer safeNetApiBuffer2 = safeNetApiBuffer;
			IDisposable disposable = safeNetApiBuffer;
			((IDisposable)safeNetApiBuffer)?.Dispose();
			return builtinAccountName;
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	public unsafe static string IsAuthenticatedUsersMemberOfUsersGroup(string nodeName, ref bool foundAuthenticatedUsers)
	{
		//Discarded unreachable code: IL_00f0
		//IL_0006: Expected I, but got I8
		//IL_002b: Expected I, but got I8
		//IL_0042: Expected I, but got I8
		//IL_00b4: Expected I, but got I8
		foundAuthenticatedUsers = false;
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			string builtinAccountName = GetBuiltinAccountName(nodeName, (WELL_KNOWN_SID_TYPE)27);
			string builtinAccountName2 = GetBuiltinAccountName(nodeName, (WELL_KNOWN_SID_TYPE)17);
			uint num = 0u;
			uint num2 = 0u;
			byte* handle = null;
			uint num3 = global::_003CModule_003E.NetLocalGroupGetMembers(ptr, InteropHelp.StringToWstr(builtinAccountName), 1u, &handle, uint.MaxValue, &num, &num2, null);
			if (num3 != 0)
			{
				DebugLog.LogError("Getting members of built-in users group from node {0} failed with error {1}", nodeName, num3);
				throw ExceptionHelp.Build<ApplicationException>((int)num3, new string[1] { Resources.Dns_ErrorValidatingUsersGroup_Text });
			}
			SafeNetApiBuffer safeNetApiBuffer = new SafeNetApiBuffer((_LOCALGROUP_MEMBERS_INFO_1*)handle);
			for (uint num4 = 0u; num4 < num; num4++)
			{
				long num5 = (long)num4 * 24L;
				if (global::_003CModule_003E._wcsicmp((ushort*)(*(ulong*)(num5 + (nint)safeNetApiBuffer.DangerousLocalGroupMembersInfo1Pointer() + 16)), InteropHelp.StringToWstr(builtinAccountName2)) == 0 && *(int*)(num5 + (nint)safeNetApiBuffer.DangerousLocalGroupMembersInfo1Pointer() + 8) == 5)
				{
					foundAuthenticatedUsers = true;
					break;
				}
			}
			SafeNetApiBuffer safeNetApiBuffer2 = safeNetApiBuffer;
			IDisposable disposable = safeNetApiBuffer;
			((IDisposable)safeNetApiBuffer)?.Dispose();
			return builtinAccountName;
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	public unsafe static string IsMemberOfAdministratorsGroup(string nodeName, ref bool foundMember, string memberName)
	{
		//Discarded unreachable code: IL_00f7
		//IL_0006: Expected I, but got I8
		//IL_0009: Expected I, but got I8
		//IL_002b: Expected I, but got I8
		//IL_0042: Expected I, but got I8
		//IL_00b2: Expected I, but got I8
		foundMember = false;
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			ptr2 = InteropHelp.StringToWstr(memberName);
			string builtinAccountName = GetBuiltinAccountName(nodeName, (WELL_KNOWN_SID_TYPE)26);
			uint num = 0u;
			uint num2 = 0u;
			byte* handle = null;
			uint num3 = global::_003CModule_003E.NetLocalGroupGetMembers(ptr, InteropHelp.StringToWstr(builtinAccountName), 1u, &handle, uint.MaxValue, &num, &num2, null);
			if (num3 != 0)
			{
				DebugLog.LogError("Getting members of built-in administrators group from node {0} failed with error {1}", nodeName, num3);
				throw ExceptionHelp.Build<ApplicationException>((int)num3, new string[1] { Resources.Dns_ErrorValidatingAdminstratorsGroup_Text });
			}
			SafeNetApiBuffer safeNetApiBuffer = new SafeNetApiBuffer((_LOCALGROUP_MEMBERS_INFO_1*)handle);
			for (uint num4 = 0u; num4 < num; num4++)
			{
				long num5 = (long)num4 * 24L;
				if (global::_003CModule_003E._wcsicmp((ushort*)(*(ulong*)(num5 + (nint)safeNetApiBuffer.DangerousLocalGroupMembersInfo1Pointer() + 16)), ptr2) == 0 && *(int*)(num5 + (nint)safeNetApiBuffer.DangerousLocalGroupMembersInfo1Pointer() + 8) == 1)
				{
					foundMember = true;
					break;
				}
			}
			SafeNetApiBuffer safeNetApiBuffer2 = safeNetApiBuffer;
			IDisposable disposable = safeNetApiBuffer;
			((IDisposable)safeNetApiBuffer)?.Dispose();
			return builtinAccountName;
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
			if (ptr2 != null)
			{
				InteropHelp.FreeWstr(ptr2);
			}
		}
	}
}
