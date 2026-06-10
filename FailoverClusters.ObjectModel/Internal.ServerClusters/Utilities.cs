using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using _003CCppImplementationDetails_003E;
using FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public sealed class Utilities
{
	private static Regex m_expression;

	static Utilities()
	{
		m_expression = new Regex("^\\{?[0-9a-f]{8}(-?[0-9a-f]{4}){3}-?[0-9a-f]{12}\\}?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
	}

	private Utilities()
	{
	}

	private static string SingletonWMIQuery(string node, string queryObject, string queryClass)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (!NetworkHelper.CanPing(node))
		{
			throw ExceptionHelp.Build<ClusterRpcConnectionException>(-2147023174, new string[1] { node });
		}
		ManagementScope managementScope = new ManagementScope(string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\root\\cimv2", node));
		managementScope.Connect();
		ObjectQuery query = new ObjectQuery(string.Format(CultureInfo.CurrentCulture, "SELECT {0} FROM {1}", queryObject, queryClass));
		using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, query))
		{
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			if (managementObjectCollection != null)
			{
				ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						try
						{
							object obj = managementObject[queryObject];
							if (obj != null)
							{
								return obj.ToString();
							}
						}
						finally
						{
							if (managementObject != null)
							{
								ManagementObject managementObject2 = managementObject;
								IDisposable disposable = managementObject;
								((IDisposable)managementObject).Dispose();
							}
						}
					}
				}
				finally
				{
					ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = enumerator;
					IDisposable disposable2 = enumerator;
					((IDisposable)enumerator)?.Dispose();
				}
			}
		}
		return null;
	}

	private static void DisposeObjectWorkItem(object state)
	{
		try
		{
			DisposeObject(state);
		}
		catch (ThreadAbortException)
		{
		}
		catch (Exception caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Error while disposing object");
		}
	}

	private unsafe static uint GetObjectFlags(string domainName, string machineName)
	{
		//Discarded unreachable code: IL_005c
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		ushort* ptr = null;
		ushort* ptr2 = null;
		uint result = 0u;
		try
		{
			ptr = InteropHelp.StringToWstr(domainName);
			ptr2 = InteropHelp.StringToWstr(machineName);
			uint num = global::_003CModule_003E.ClRtlGetObjectFlags(ptr2, ptr, &result);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[3]
				{
					Resources.GetMachineAccountInformation_Failed_Text,
					domainName,
					machineName
				});
			}
			return result;
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

	public unsafe static string FormatError(int sc)
	{
		//IL_000e: Expected I4, but got I8
		//IL_0025: Expected I, but got I8
		//IL_0025: Expected I, but got I8
		//IL_004c: Expected I, but got I8
		//IL_0062: Expected I4, but got I8
		//IL_00a7: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0EAA_0040G _0024ArrayType_0024_0024_0024BY0EAA_0040G);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref _0024ArrayType_0024_0024_0024BY0EAA_0040G, 0, 2048);
		ulong num = global::_003CModule_003E.FormatMessageW(4096u, null, (uint)sc, 0u, (ushort*)(&_0024ArrayType_0024_0024_0024BY0EAA_0040G), 1024u, null);
		if (num == 0L)
		{
			num = global::_003CModule_003E.FormatMessageW(2560u, global::_003CModule_003E.GetModuleHandleW((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BE_0040LCNNIAEL_0040_003F_0024AAN_003F_0024AAT_003F_0024AAD_003F_0024AAL_003F_0024AAL_003F_0024AA_003F4_003F_0024AAD_003F_0024AAL_003F_0024AAL_0040)), (uint)sc, 0u, (ushort*)(&_0024ArrayType_0024_0024_0024BY0EAA_0040G), 1024u, null);
			if (num == 0L)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BAF_0040G _0024ArrayType_0024_0024_0024BY0BAF_0040G);
				// IL initblk instruction
				System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref _0024ArrayType_0024_0024_0024BY0BAF_0040G, 0, 522);
				if (global::_003CModule_003E.GetSystemDirectoryW((ushort*)(&_0024ArrayType_0024_0024_0024BY0BAF_0040G), 261u) != 0 && global::_003CModule_003E.StringCchCatW((ushort*)(&_0024ArrayType_0024_0024_0024BY0BAF_0040G), 261uL, (ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1BM_0040CFBALOLG_0040_003F_0024AA_003F2_003F_0024AAA_003F_0024AAC_003F_0024AAT_003F_0024AAI_003F_0024AAV_003F_0024AAE_003F_0024AAD_003F_0024AAS_003F_0024AA_003F4_003F_0024AAD_003F_0024AAL_003F_0024AAL_0040)) >= 0)
				{
					HINSTANCE__* ptr = global::_003CModule_003E.LoadLibraryW((ushort*)(&_0024ArrayType_0024_0024_0024BY0BAF_0040G));
					if (ptr != null)
					{
						num = global::_003CModule_003E.FormatMessageW(2560u, ptr, (uint)sc, 0u, (ushort*)(&_0024ArrayType_0024_0024_0024BY0EAA_0040G), 1024u, null);
						if (global::_003CModule_003E.FreeLibrary(ptr) == 0)
						{
							DebugLog.LogWarning("FreeLibrary failed with error {0}", global::_003CModule_003E.GetLastError());
						}
						if (num != 0L)
						{
							goto IL_00e9;
						}
					}
				}
				return string.Format(CultureInfo.CurrentCulture, Resources.GenericErrorFormatString_Text, sc);
			}
		}
		goto IL_00e9;
		IL_00e9:
		string text = InteropHelp.WstrToString((ushort*)(&_0024ArrayType_0024_0024_0024BY0EAA_0040G));
		text = text.TrimEnd(Environment.NewLine.ToCharArray());
		return text.Replace("\r\n", "");
	}

	public static int Win32ToHResult(int sc)
	{
		return (sc > 0) ? ((sc & 0xFFFF) | -2147024896) : sc;
	}

	public unsafe static string GetFileSystem(string path)
	{
		//Discarded unreachable code: IL_0055
		//IL_0003: Expected I, but got I8
		//IL_0020: Expected I, but got I8
		//IL_0020: Expected I, but got I8
		//IL_0020: Expected I, but got I8
		//IL_0020: Expected I, but got I8
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(path);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BAF_0040G _0024ArrayType_0024_0024_0024BY0BAF_0040G);
			if (global::_003CModule_003E.GetVolumeInformationW(ptr, null, 0u, null, null, null, (ushort*)(&_0024ArrayType_0024_0024_0024BY0BAF_0040G), 261u) == 0)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[2]
				{
					Resources.GetVolumeInformationFailed_Text,
					path
				}, resultCode: (int)global::_003CModule_003E.GetLastError());
			}
			return InteropHelp.WstrToString((ushort*)(&_0024ArrayType_0024_0024_0024BY0BAF_0040G));
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsComputerAccountEnabled(string domainName, string machineName)
	{
		return (byte)(~(GetObjectFlags(domainName, machineName) >> 1) & (true ? 1u : 0u)) != 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool CanCreateComputerObject(string domainName, string ouName)
	{
		//Discarded unreachable code: IL_003d
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(domainName);
			ptr2 = InteropHelp.StringToWstr(ouName);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out int num);
			if (global::_003CModule_003E.ClRtlCheckCreateComputerObjectAccess(ptr2, &num) >= 0 && num != 0)
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
			if (ptr2 != null)
			{
				InteropHelp.FreeWstr(ptr2);
			}
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool HasReadAllPropertiesPermissions(string ouName)
	{
		//Discarded unreachable code: IL_002b
		//IL_0003: Expected I, but got I8
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(ouName);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out int num);
			if (global::_003CModule_003E.ClRtlCheckPermissionsOnOU(ptr, 16u, &num) == 0 && num != 0)
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

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool IsOUProtectedFromChildDeletion(string ouName)
	{
		//Discarded unreachable code: IL_002b
		//IL_0003: Expected I, but got I8
		//IL_0014: Expected I, but got I8
		ushort* ptr = null;
		try
		{
			ptr = InteropHelp.StringToWstr(ouName);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out int num);
			if (global::_003CModule_003E.ClRtlIsEveryoneDeniedAccess(ptr, null, &num) == 0 && num != 0)
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

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsDomainJoined(string nodeName)
	{
		if (nodeName == null)
		{
			throw new ArgumentNullException("nodeName");
		}
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			string text = SingletonWMIQuery(nodeName, "PartOfDomain", "Win32_ComputerSystem");
			int num = ((text != null && text.ToLower() == "true") ? 1 : 0);
			return (byte)num != 0;
		}
		catch (ManagementException caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception querying domain for node '{0}'", nodeName);
		}
		catch (COMException caughtException2)
		{
			ExceptionHelp.LogException(caughtException2, "Exception querying domain for node '{0}'", nodeName);
		}
		return false;
	}

	public static void BackgroundDisposeObject(object value)
	{
		ThreadPool.QueueUserWorkItem(DisposeObjectWorkItem, value);
	}

	public static void DisposeObject(object value)
	{
		if (value is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsGuid(string value)
	{
		return m_expression.IsMatch(value);
	}

	public static string GetClusterActiveDirectoryDomain(ICollection<string> nodeNames)
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
				return GetNodeActiveDirectoryDomain(nodeName);
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

	public static string GetClusterActiveDirectoryDomain(Cluster cluster)
	{
		if (cluster == null)
		{
			throw new ArgumentNullException("cluster");
		}
		List<string> list = new List<string>();
		foreach (ClusterNode node in cluster.GetNodes())
		{
			list.Add(node.FqdnName);
		}
		return GetClusterActiveDirectoryDomain(list);
	}

	public static string GetNodeActiveDirectoryDomain(string node)
	{
		ManagementException ex = null;
		COMException ex2 = null;
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			return SingletonWMIQuery(node, "Domain", "Win32_ComputerSystem");
		}
		catch (ManagementException caughtException)
		{
			ExceptionHelp.LogException(caughtException, "Exception querying domain for node '{0}'", node);
		}
		catch (COMException caughtException2)
		{
			ExceptionHelp.LogException(caughtException2, "Exception querying domain for node '{0}'", node);
		}
		return string.Empty;
	}

	public static string GetSystemDrive(string node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return SingletonWMIQuery(node, "SystemDrive", "Win32_OperatingSystem");
	}

	public unsafe static string GetSiteNameForNode(string nodeName)
	{
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		//IL_0026: Expected I, but got I8
		//IL_0026: Expected I, but got I8
		//IL_0026: Expected I, but got I8
		//IL_0068: Expected I, but got I8
		ushort* ptr = null;
		_DOMAIN_CONTROLLER_INFOW* ptr2 = null;
		string result = "";
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			uint num = global::_003CModule_003E.DsGetDcNameW(ptr, null, null, null, 1073741840u, &ptr2);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.GetDcName_Failed_Text,
					nodeName
				});
			}
			ulong num2 = *(ulong*)((ulong)(nint)ptr2 + 72uL);
			result = ((num2 != 0L) ? InteropHelp.WstrToString((ushort*)num2) : InteropHelp.WstrToString((ushort*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_11LOCGONAA_0040_0040)));
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
			if (ptr2 != null)
			{
				global::_003CModule_003E.NetApiBufferFree(ptr2);
			}
		}
		return result;
	}

	public unsafe static NodeInRodcSiteState IsNodeInRodcSite(string nodeName)
	{
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		//IL_000f: Expected I, but got I8
		//IL_0013: Expected I, but got I8
		//IL_0030: Expected I, but got I8
		//IL_0030: Expected I, but got I8
		//IL_0030: Expected I, but got I8
		//IL_006f: Expected I, but got I8
		//IL_0082: Expected I, but got I8
		//IL_0082: Expected I, but got I8
		//IL_0082: Expected I, but got I8
		//IL_00ce: Expected I, but got I8
		//IL_00ce: Expected I, but got I8
		//IL_00ef: Expected I, but got I8
		//IL_0114: Expected I, but got I8
		//IL_012b: Expected I, but got I8
		//IL_0147: Expected I, but got I8
		//IL_0147: Expected I, but got I8
		NodeInRodcSiteState nodeInRodcSiteState = NodeInRodcSiteState.Unknown;
		bool flag = true;
		ushort* ptr = null;
		_DOMAIN_CONTROLLER_INFOW* ptr2 = null;
		void* ptr3 = null;
		byte* ptr4 = null;
		uint num = 0u;
		try
		{
			ptr = InteropHelp.StringToWstr(nodeName);
			uint num2 = global::_003CModule_003E.DsGetDcNameW(ptr, null, null, null, 1074266112u, &ptr2);
			switch (num2)
			{
			default:
				throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
				{
					Resources.GetDcName_Failed_Text,
					nodeName
				});
			case 1355u:
			{
				if (ptr2 != null)
				{
					global::_003CModule_003E.NetApiBufferFree(ptr2);
					ptr2 = null;
				}
				uint num7 = global::_003CModule_003E.DsGetDcNameW(ptr, null, null, null, 1073741840u, &ptr2);
				if (num7 != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>((int)num7, new string[2]
					{
						Resources.GetDcName_Failed_Text,
						nodeName
					});
				}
				flag = false;
				return NodeInRodcSiteState.Unknown;
			}
			case 0u:
			{
				if (*(long*)((ulong)(nint)ptr2 + 72uL) == 0L)
				{
					return NodeInRodcSiteState.Unknown;
				}
				uint num3 = global::_003CModule_003E.DsBindW((ushort*)(*(ulong*)ptr2), null, &ptr3);
				if (num3 != 0)
				{
					throw ExceptionHelp.Build<ApplicationException>((int)num3, new string[3]
					{
						Resources.BindToDc_Failed_Text,
						InteropHelp.WstrToString((ushort*)(*(ulong*)ptr2)),
						nodeName
					});
				}
				int num4 = 3;
				if (global::_003CModule_003E.DsGetDomainControllerInfoW(ptr3, (ushort*)(*(ulong*)((ulong)(nint)ptr2 + 40uL)), 3u, &num, (void**)(&ptr4)) != 0)
				{
					break;
				}
				for (uint num5 = 0u; num5 < num; num5++)
				{
					DS_DOMAIN_CONTROLLER_INFO_3W* ptr5 = (DS_DOMAIN_CONTROLLER_INFO_3W*)((long)num5 * 136L + (nint)ptr4);
					ulong num6 = *(ulong*)((ulong)(nint)ptr5 + 16uL);
					if (num6 != 0L && global::_003CModule_003E._wcsicmp((ushort*)(*(ulong*)((ulong)(nint)ptr2 + 72uL)), (ushort*)num6) == 0)
					{
						nodeInRodcSiteState = ((*(int*)((ulong)(nint)ptr5 + 68uL) == 0) ? ((nodeInRodcSiteState == NodeInRodcSiteState.Present) ? NodeInRodcSiteState.Mixed : NodeInRodcSiteState.Absent) : ((nodeInRodcSiteState != 0) ? NodeInRodcSiteState.Present : NodeInRodcSiteState.Mixed));
					}
				}
				break;
			}
			}
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
			if (ptr4 != null)
			{
				global::_003CModule_003E.DsFreeDomainControllerInfoW((!flag) ? 1u : 3u, num, ptr4);
			}
			if (ptr3 != null)
			{
				global::_003CModule_003E.DsUnBindW(&ptr3);
			}
			if (ptr2 != null)
			{
				global::_003CModule_003E.NetApiBufferFree(ptr2);
			}
		}
		return nodeInRodcSiteState;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool IsWritableDcReachable(string nodeName)
	{
		//Discarded unreachable code: IL_00b9
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		//IL_000a: Expected I, but got I8
		ushort* ptr = null;
		ushort* ptr2 = null;
		ushort* ptr3 = null;
		try
		{
			int num = nodeName.IndexOf(".");
			if (num >= 0)
			{
				nodeName = nodeName.Substring(0, num);
			}
			ptr = InteropHelp.StringToWstr(nodeName);
			uint num2 = global::_003CModule_003E.ClRtlGetADDomain(ptr, &ptr2);
			if (num2 != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
				{
					Resources.GetDcName_Failed_Text,
					nodeName
				});
			}
			ushort* intPtr = ptr;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out int num3);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out int num4);
			uint num5 = global::_003CModule_003E.ClRtlFindSuitableDC(intPtr, intPtr, ptr2, &ptr3, 4096u, &num3, &num4);
			if (num5 != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num5, new string[2]
				{
					Resources.GetDcName_Failed_Text,
					nodeName
				});
			}
			if (num4 != 0)
			{
				return false;
			}
			return true;
		}
		finally
		{
			if (ptr != null)
			{
				InteropHelp.FreeWstr(ptr);
			}
			if (ptr3 != null)
			{
				InteropHelp.FreeWstr(ptr3);
			}
			if (ptr2 != null)
			{
				InteropHelp.FreeWstr(ptr2);
			}
		}
	}
}

