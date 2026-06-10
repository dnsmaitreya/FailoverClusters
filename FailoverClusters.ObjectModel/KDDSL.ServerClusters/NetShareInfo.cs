using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace KDDSL.ServerClusters;

public class NetShareInfo
{
	private string m_server;

	private string m_name;

	private string m_path;

	private string m_fullName;

	private string m_remark;

	private string m_adminSharePath;

	private NetShareType m_type;

	private uint m_maxUsers;

	private uint m_currentUsers;

	private SafeSecurityDescriptor m_securityDescriptor;

	public uint MaxUsers
	{
		get
		{
			return m_maxUsers;
		}
		set
		{
			m_maxUsers = value;
		}
	}

	public string Remark
	{
		get
		{
			return m_remark;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			m_remark = value;
		}
	}

	public uint CurrentUsers => m_currentUsers;

	public NetShareType ShareType => m_type;

	public string AdminSharePath => m_adminSharePath;

	public string FullName => m_fullName;

	public string Path => m_path;

	public string Server => m_server;

	public virtual string Name => m_name;

	public unsafe void SaveProperties()
	{
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		//IL_002e: Expected I4, but got I8
		//IL_0035: Expected I8, but got I
		//IL_0048: Expected I8, but got I
		//IL_005e: Expected I, but got I8
		//IL_0065: Expected I8, but got I
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(m_remark);
			ptr2 = InteropHelp.StringToWstr(m_path);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _SHARE_INFO_502 sHARE_INFO_);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref sHARE_INFO_, 0, 72);
			System.Runtime.CompilerServices.Unsafe.As<_SHARE_INFO_502, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sHARE_INFO_, 16)) = (nint)ptr;
			System.Runtime.CompilerServices.Unsafe.As<_SHARE_INFO_502, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sHARE_INFO_, 28)) = m_maxUsers;
			System.Runtime.CompilerServices.Unsafe.As<_SHARE_INFO_502, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sHARE_INFO_, 40)) = (nint)ptr2;
			SafeSecurityDescriptor securityDescriptor = m_securityDescriptor;
			void* ptr3 = ((securityDescriptor == null) ? null : securityDescriptor.DangerousPointer());
			System.Runtime.CompilerServices.Unsafe.As<_SHARE_INFO_502, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sHARE_INFO_, 64)) = (nint)ptr3;
			SetShareInfo((byte*)(&sHARE_INFO_), 502u);
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

	public unsafe uint GetShi1005Flags()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		string name = m_name;
		string server = m_server;
		return *(uint*)new SafeNetApiBuffer((_SHARE_INFO_1005*)NetShareGetInfo(1005u, server, name)).DangerousShareInfo1005Pointer();
	}

	public unsafe void SetShi1005Flags(uint flags)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _SHARE_INFO_1005 sHARE_INFO_);
		*(uint*)(&sHARE_INFO_) = flags;
		SetShareInfo((byte*)(&sHARE_INFO_), 1005u);
	}

	public unsafe void Delete()
	{
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(string.Format(CultureInfo.InvariantCulture, "\\\\{0}", m_server));
			ptr2 = InteropHelp.StringToWstr(m_name);
			uint num = global::_003CModule_003E.NetShareDel(ptr, ptr2, 0u);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.NetShareDel_Failed_Text,
					m_fullName
				});
			}
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

	public void Refresh()
	{
		//Discarded unreachable code: IL_0098
		ThreadWatchdog.PerformUIThreadCheck();
		try
		{
			NetShareInfo netShareInfo = Get(m_server, m_name);
			m_server = netShareInfo.m_server;
			m_name = netShareInfo.m_name;
			m_path = netShareInfo.m_path;
			m_remark = netShareInfo.m_remark;
			m_type = netShareInfo.m_type;
			m_maxUsers = netShareInfo.m_maxUsers;
			m_currentUsers = netShareInfo.m_currentUsers;
			m_securityDescriptor = netShareInfo.m_securityDescriptor;
		}
		catch (Exception caughtException)
		{
			Win32Exception firstException = ExceptionHelp.GetFirstException<Win32Exception>(caughtException);
			if (firstException != null && firstException.NativeErrorCode == -2147022586)
			{
				ClusApiExceptionFactory.ThrowObjectDeletedException();
				throw;
			}
			throw;
		}
	}

	public unsafe void GrantClusterPermissionToShare(Cluster cluster)
	{
		//Discarded unreachable code: IL_00a5, IL_00a7
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		void* ptr = null;
		void* ptr2 = null;
		try
		{
			if (m_securityDescriptor == null)
			{
				throw new InvalidOperationException(Resources.NoSecurityDescriptor_Text);
			}
			ptr2 = GetCNOSid(cluster);
			uint num = global::_003CModule_003E.ClRtlAddAceToSd(m_securityDescriptor.DangerousPointer(), ptr2, 1245631u, &ptr);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[3]
				{
					Resources.NetShareAddCnoSidFailed_Text,
					cluster.Name,
					m_name
				});
			}
			CopySD(ptr);
			SaveProperties();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.NetShareInfo_GrantClusterPermission_Fail_Text,
				m_name,
				cluster.Name
			});
		}
		finally
		{
			if (ptr2 != null)
			{
				global::_003CModule_003E.LocalFree(ptr2);
			}
			if (ptr != null)
			{
				global::_003CModule_003E.LocalFree(ptr);
			}
		}
	}

	public unsafe void RemoveClusterPermissionFromShare(Cluster cluster)
	{
		//Discarded unreachable code: IL_00a0, IL_00a2
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		void* ptr = null;
		void* ptr2 = null;
		try
		{
			if (m_securityDescriptor == null)
			{
				throw new InvalidOperationException(Resources.NoSecurityDescriptor_Text);
			}
			ptr2 = GetCNOSid(cluster);
			uint num = global::_003CModule_003E.ClRtlRemoveAceFromSd(m_securityDescriptor.DangerousPointer(), ptr2, &ptr);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[3]
				{
					Resources.NetShareRemoveCnoSidFailed_Text,
					cluster.Name,
					m_name
				});
			}
			CopySD(ptr);
			SaveProperties();
		}
		catch (Exception innerException)
		{
			throw ExceptionHelp.Build<ApplicationException>(innerException, new string[3]
			{
				Resources.NetShareInfo_RemoveClusterPermission_Fail_Text,
				m_name,
				cluster.Name
			});
		}
		finally
		{
			if (ptr2 != null)
			{
				global::_003CModule_003E.LocalFree(ptr2);
			}
			if (ptr != null)
			{
				global::_003CModule_003E.LocalFree(ptr);
			}
		}
	}

	public unsafe static NetShareInfo Get(string serverName, string shareName)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		SafeNetApiBuffer safeNetApiBuffer = new SafeNetApiBuffer((_SHARE_INFO_502*)NetShareGetInfo(502u, serverName, shareName));
		NetShareInfo result = new NetShareInfo(serverName, safeNetApiBuffer.DangerousShareInfo502Pointer());
		((IDisposable)safeNetApiBuffer)?.Dispose();
		return result;
	}

	public unsafe static ICollection<NetShareInfo> Enum(string serverName)
	{
		//IL_0039: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		uint num = 0u;
		SafeNetApiBuffer safeNetApiBuffer = new SafeNetApiBuffer(NetShareEnum(serverName, &num));
		List<NetShareInfo> list = new List<NetShareInfo>();
		_SHARE_INFO_502* ptr = safeNetApiBuffer.DangerousShareInfo502Pointer();
		uint num2 = 0u;
		if (0 < num)
		{
			do
			{
				list.Add(new NetShareInfo(serverName, (_SHARE_INFO_502*)((long)num2 * 72L + (nint)ptr)));
				num2++;
			}
			while (num2 < num);
		}
		((IDisposable)safeNetApiBuffer)?.Dispose();
		return new ReadOnlyCollection<NetShareInfo>(list);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static bool TryParseFileShare(string fileSharePath, out string serverName, out string shareName, out string subDir)
	{
		if (fileSharePath == null)
		{
			return false;
		}
		Match match = new Regex(new string((char*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_1II_0040HPMJHFKM_0040_003F_0024AA_003F_0024FO_003F_0024AA_003F2_003F_0024AA_003F2_003F_0024AA_003F2_003F_0024AA_003F2_003F_0024AA_003F_0024CI_003F_0024AA_003F_0024DP_003F_0024AA_003F_0024DM_003F_0024AAs_003F_0024AAe_003F_0024AAr_003F_0024AAv_003F_0024AAe_003F_0024AAr_003F_0024AA_003F_0024DO_0040))).Match(fileSharePath);
		if (match == Match.Empty)
		{
			return false;
		}
		serverName = match.Groups["server"].Value;
		shareName = match.Groups["share"].Value;
		subDir = match.Groups["dir"].Value;
		return true;
	}

	public static void ParseFileShare(string fileSharePath, out string serverName, out string shareName, out string subDir)
	{
		if (!TryParseFileShare(fileSharePath, out serverName, out shareName, out subDir))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidFileSharePathArg_Text, fileSharePath), "fileSharePath");
		}
	}

	private unsafe NetShareInfo(string serverName, _SHARE_INFO_502* shareInfo)
	{
		//IL_0015: Expected I, but got I8
		//IL_0026: Expected I, but got I8
		//IL_0058: Expected I, but got I8
		//IL_00a4: Expected I, but got I8
		m_server = serverName;
		m_name = InteropHelp.WstrToString((ushort*)(*(ulong*)shareInfo));
		m_path = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)shareInfo + 40uL)));
		m_fullName = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\{1}", m_server, m_name);
		m_remark = InteropHelp.WstrToString((ushort*)(*(ulong*)((ulong)(nint)shareInfo + 16uL)));
		m_type = *(NetShareType*)((ulong)(nint)shareInfo + 8uL);
		m_maxUsers = *(uint*)((ulong)(nint)shareInfo + 28uL);
		m_currentUsers = *(uint*)((ulong)(nint)shareInfo + 32uL);
		m_adminSharePath = BuildAdminSharePath(serverName, m_path);
		ulong num = *(ulong*)((ulong)(nint)shareInfo + 64uL);
		if (num != 0L)
		{
			CopySD((void*)num);
		}
		else
		{
			m_securityDescriptor = null;
		}
	}

	private unsafe void CopySD(void* securityDescriptor)
	{
		if ((m_securityDescriptor = new SafeSecurityDescriptor(global::_003CModule_003E.ClRtlCopySecurityDescriptor(securityDescriptor))).IsInvalid)
		{
			throw ExceptionHelp.Build<ApplicationException>(args: new string[2]
			{
				Resources.NetShareCopySDFailed_Text,
				m_name
			}, resultCode: (int)global::_003CModule_003E.GetLastError());
		}
	}

	private unsafe void SetShareInfo(byte* buffer, uint level)
	{
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		//IL_002e: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(m_server);
			ptr2 = InteropHelp.StringToWstr(m_name);
			uint num = global::_003CModule_003E.NetShareSetInfo(ptr, ptr2, level, buffer, null);
			if (num != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.NetShareSetInfoFailed_Text,
					m_fullName
				});
			}
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

	private unsafe void* GetCNOSid(Cluster cluster)
	{
		//Discarded unreachable code: IL_006b
		//IL_0008: Expected I, but got I8
		//IL_000b: Expected I, but got I8
		//IL_0015: Expected I, but got I8
		//IL_0053: Expected I, but got I8
		//IL_0075: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		ushort* ptr = null;
		void* ptr2 = null;
		try
		{
			string text = null;
			uint num = 0u;
			void* ptr3 = null;
			ptr = InteropHelp.StringToWstr(FileShareQuorumSettings.GetClusterCnoName(cluster));
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _SID_NAME_USE sID_NAME_USE);
			uint num2 = global::_003CModule_003E.ClRtlConvertDomainAccountToSid(ptr, &ptr2, &sID_NAME_USE);
			if (num2 != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
				{
					Resources.NetShareLookupCNOFailed_Text,
					cluster.Name
				});
			}
			void* result = ptr2;
			ptr2 = null;
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
				global::_003CModule_003E.LocalFree(ptr2);
			}
		}
	}

	private string BuildAdminSharePath(string serverName, string path)
	{
		return string.Concat(serverName + "\\", path.Replace(':', '$'));
	}

	private unsafe static _SHARE_INFO_1005* NetShareGetInfo1005(string serverName, string shareName)
	{
		return (_SHARE_INFO_1005*)NetShareGetInfo(1005u, serverName, shareName);
	}

	private unsafe static _SHARE_INFO_502* NetShareGetInfo502(string serverName, string shareName)
	{
		return (_SHARE_INFO_502*)NetShareGetInfo(502u, serverName, shareName);
	}

	private unsafe static byte* NetShareGetInfo(uint dwLevel, string serverName, string shareName)
	{
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		ushort* ptr = null;
		ushort* ptr2 = null;
		try
		{
			ptr = InteropHelp.StringToWstr(serverName);
			ptr2 = InteropHelp.StringToWstr(shareName);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out byte* ptr3);
			uint num = global::_003CModule_003E.NetShareGetInfo(ptr, ptr2, dwLevel, &ptr3);
			return num switch
			{
				5u => throw ExceptionHelp.Build<ApplicationException>(5, new string[3]
				{
					Resources.NetShareGetInfoFailedAccessDenied_Text,
					serverName,
					shareName
				}), 
				0u => ptr3, 
				_ => throw ExceptionHelp.Build<ApplicationException>((int)num, new string[2]
				{
					Resources.NetShareGetInfoFailed_Text,
					shareName
				}), 
			};
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

	private unsafe static _SHARE_INFO_502* NetShareEnum(string serverName, uint* pdwEntries)
	{
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		//IL_0023: Expected I, but got I8
		_SHARE_INFO_502* result = null;
		ushort* ptr = null;
		uint num = 0u;
		try
		{
			ptr = InteropHelp.StringToWstr(serverName);
			uint num2 = global::_003CModule_003E.NetShareEnum(ptr, 502u, (byte**)(&result), uint.MaxValue, pdwEntries, &num, null);
			if (num2 != 0)
			{
				throw ExceptionHelp.Build<ApplicationException>((int)num2, new string[2]
				{
					Resources.NetShareEnumFail_Text,
					serverName
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
		}
	}
}
