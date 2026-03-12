using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.ServerClusters;

public class WindowsService : IDisposable
{
	private class ServiceControlManagerState
	{
		public string machineName;

		public int desiredAccess;

		public SafeServiceHandle managerHandle;

		public uint lastError;

		public ServiceControlManagerState(string machineName, int desiredAccess)
		{
			this.machineName = machineName;
			this.desiredAccess = desiredAccess;
		}
	}

	private static string ExeSuffix = ".exe";

	private SafeServiceHandle m_service;

	private UnmanagedBuffer m_serviceConfig;

	private string m_serviceName;

	public string Name => m_serviceName;

	public WindowsService(string serviceName, string machineName, WindowsServicePermissions permissions, NodeState nodeState)
	{
		Init(serviceName, machineName, permissions, nodeState);
	}

	public WindowsService(string serviceName, string machineName, WindowsServicePermissions permissions)
	{
		Init(serviceName, machineName, permissions, NodeState.Unknown);
	}

	private WindowsService(SafeServiceHandle serviceManager, string serviceName, WindowsServicePermissions permissions)
	{
		Construct(serviceManager, serviceName, permissions);
	}

	private void Construct(SafeServiceHandle serviceManager, string serviceName, WindowsServicePermissions permissions)
	{
		m_serviceName = serviceName;
		m_service = OpenServiceW(serviceManager, serviceName, permissions);
		m_serviceConfig = null;
	}

	private unsafe static WindowsService GetService(SafeServiceHandle serviceManager, _ENUM_SERVICE_STATUS_PROCESSW* pEnumService, WindowsServicePermissions permissions)
	{
		//IL_0007: Expected I, but got I8
		string serviceName = InteropHelp.WstrToString((ushort*)(*(ulong*)pEnumService));
		return new WindowsService(serviceManager, serviceName, permissions);
	}

	private unsafe static SafeServiceHandle OpenServiceManager(string machineName, WindowsServicePermissions permissions)
	{
		//Discarded unreachable code: IL_00ae
		ThreadWatchdog.PerformUIThreadCheck();
		Thread thread = new Thread(OpenServiceControlManager);
		uint num = 1u;
		num = (((permissions & WindowsServicePermissions.Query) != 0) ? 131077u : num);
		if ((permissions & WindowsServicePermissions.ChangeState) != 0)
		{
			num |= 0x20000u;
		}
		ServiceControlManagerState serviceControlManagerState = new ServiceControlManagerState(machineName, (int)num);
		thread.Start(serviceControlManagerState);
		if (thread.Join(15000))
		{
			if (serviceControlManagerState.managerHandle.GetHandle() == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(args: new string[2]
				{
					Resources.OpenServiceManager_Failed_Time_Out_Text,
					machineName
				}, resultCode: (int)serviceControlManagerState.lastError);
			}
			return serviceControlManagerState.managerHandle;
		}
		thread.Abort();
		throw ExceptionHelp.Build<ApplicationException>(-2147023174, new string[2]
		{
			Resources.OpenServiceManager_Failed_Text,
			machineName
		});
	}

	private unsafe SafeServiceHandle OpenServiceW(SafeServiceHandle serviceManager, string serviceName, WindowsServicePermissions permissions)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		SafeServiceHandle safeServiceHandle = null;
		ushort* ptr = InteropHelp.StringToWstr(serviceName);
		try
		{
			uint num = 0u;
			num = (((permissions & WindowsServicePermissions.Query) != 0) ? 5u : num);
			if ((permissions & WindowsServicePermissions.ChangeState) != 0)
			{
				num |= 0x70u;
			}
			SC_HANDLE__* ptr2 = global::_003CModule_003E.OpenServiceW(serviceManager.GetHandle(), ptr, num);
			if (ptr2 == null)
			{
				throw ExceptionHelp.Build<ApplicationException>(Marshal.GetLastWin32Error(), new string[2]
				{
					Resources.OpenService_Failed_Text,
					serviceName
				});
			}
			return new SafeServiceHandle(ptr2);
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	private unsafe static void OpenServiceControlManager(object parameter)
	{
		//IL_0021: Expected I, but got I8
		ServiceControlManagerState serviceControlManagerState = (ServiceControlManagerState)parameter;
		ushort* ptr = InteropHelp.StringToWstr(serviceControlManagerState.machineName);
		try
		{
			SC_HANDLE__* handle = global::_003CModule_003E.OpenSCManagerW(ptr, null, (uint)serviceControlManagerState.desiredAccess);
			serviceControlManagerState.managerHandle = new SafeServiceHandle(handle);
			serviceControlManagerState.lastError = global::_003CModule_003E.GetLastError();
		}
		finally
		{
			InteropHelp.FreeWstr(ptr);
		}
	}

	private static uint GetServiceManagerPermissions(WindowsServicePermissions permissions)
	{
		uint num = 1u;
		num = (((permissions & WindowsServicePermissions.Query) != 0) ? 131077u : num);
		if ((permissions & WindowsServicePermissions.ChangeState) != 0)
		{
			num |= 0x20000u;
		}
		return num;
	}

	private uint GetServicePermissions(WindowsServicePermissions permissions)
	{
		uint num = 0u;
		num = (((permissions & WindowsServicePermissions.Query) != 0) ? 5u : num);
		if ((permissions & WindowsServicePermissions.ChangeState) != 0)
		{
			num |= 0x70u;
		}
		return num;
	}

	public unsafe string GetDescription()
	{
		return GetDescription(m_service.GetHandle());
	}

	private unsafe static string GetDescription(SC_HANDLE__* service)
	{
		//IL_0016: Expected I, but got I8
		//IL_0094: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		uint num = 0u;
		string result = null;
		if (global::_003CModule_003E.QueryServiceConfig2W(service, 1u, null, 0u, &num) == 0)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			switch (lastWin32Error)
			{
			case 1168:
				return string.Empty;
			default:
				throw ExceptionHelp.Build<ApplicationException>(lastWin32Error, new string[1] { Resources.QueryServiceConfig2_Failed_Text });
			case 122:
				break;
			}
		}
		_SERVICE_DESCRIPTIONW* ptr = (_SERVICE_DESCRIPTIONW*)InteropHelp.AllocateArray(num);
		try
		{
			int num2 = global::_003CModule_003E.QueryServiceConfig2W(service, 1u, (byte*)ptr, num, &num);
			if (0 == num2)
			{
				throw ExceptionHelp.Build<ApplicationException>(Marshal.GetLastWin32Error(), new string[1] { Resources.QueryServiceConfig2_Failed_Text });
			}
			ulong num3 = *(ulong*)ptr;
			result = ((num3 == 0L) ? string.Empty : InteropHelp.WstrToString((ushort*)num3));
		}
		finally
		{
			InteropHelp.FreeArray(ptr);
		}
		return result;
	}

	public unsafe WindowsServiceStart GetStartMode()
	{
		return GetStartMode(m_service.GetHandle());
	}

	private unsafe WindowsServiceStart GetStartMode(SC_HANDLE__* service)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (m_serviceConfig == null)
		{
			GetServiceConfig(service);
		}
		return *(WindowsServiceStart*)((ulong)(nint)m_serviceConfig.Pointer + 4uL);
	}

	public unsafe WindowsServiceState GetState()
	{
		return GetState(m_service.GetHandle());
	}

	private unsafe WindowsServiceState GetState(SC_HANDLE__* service)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _SERVICE_STATUS sERVICE_STATUS);
		if (global::_003CModule_003E.QueryServiceStatus(service, &sERVICE_STATUS) == 0)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 122)
			{
				throw ExceptionHelp.Build<ApplicationException>(lastWin32Error, new string[1] { Resources.QueryServiceStatus_Failed_Text });
			}
		}
		return System.Runtime.CompilerServices.Unsafe.As<_SERVICE_STATUS, WindowsServiceState>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sERVICE_STATUS, 4));
	}

	private unsafe void GetServiceConfig(SC_HANDLE__* service)
	{
		//Discarded unreachable code: IL_00a6
		//IL_0013: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		uint num = 0u;
		if (global::_003CModule_003E.QueryServiceConfigW(service, null, 0u, &num) == 0)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 122)
			{
				throw ExceptionHelp.Build<ApplicationException>(lastWin32Error, new string[1] { Resources.QueryServiceConfig_Failed_Text });
			}
		}
		(m_serviceConfig = new UnmanagedBuffer()).Allocate(num);
		try
		{
			int num2 = global::_003CModule_003E.QueryServiceConfigW(service, (_QUERY_SERVICE_CONFIGW*)m_serviceConfig.Pointer, num, &num);
			if (0 == num2)
			{
				throw ExceptionHelp.Build<ApplicationException>(Marshal.GetLastWin32Error(), new string[1] { Resources.QueryServiceConfig_Failed_Text });
			}
		}
		catch (Exception)
		{
			((IDisposable)m_serviceConfig)?.Dispose();
			m_serviceConfig = null;
			throw;
		}
	}

	public unsafe string GetDisplayName()
	{
		return GetDisplayName(m_service.GetHandle());
	}

	private unsafe string GetDisplayName(SC_HANDLE__* service)
	{
		//IL_002f: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		_ = string.Empty;
		if (m_serviceConfig == null)
		{
			GetServiceConfig(service);
		}
		return new string((char*)(*(ulong*)((ulong)(nint)m_serviceConfig.Pointer + 56uL)));
	}

	public unsafe string GetStartupParameters()
	{
		return GetStartupParameters(m_service.GetHandle());
	}

	private unsafe string GetStartupParameters(SC_HANDLE__* service)
	{
		//IL_002f: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		string result = string.Empty;
		if (m_serviceConfig == null)
		{
			GetServiceConfig(service);
		}
		string text = new string((char*)(*(ulong*)((ulong)(nint)m_serviceConfig.Pointer + 16uL)));
		if (!string.IsNullOrEmpty(text))
		{
			int num = text.IndexOf(ExeSuffix, StringComparison.OrdinalIgnoreCase);
			if (num > 0)
			{
				num = Math.Min(ExeSuffix.Length + num + 1, text.Length);
				result = text.Substring(num);
			}
		}
		return result;
	}

	private void Init(string serviceName, string machineName, WindowsServicePermissions permissions, NodeState nodeState)
	{
		ThreadWatchdog.PerformUIThreadCheck();
		if (nodeState != 0)
		{
			DebugLog.LogVerbose(string.Format(CultureInfo.InvariantCulture, "Server {0} service status is '{1}' pinging server before connect to Service Control Manager (SCM)", machineName, NodeState.Unknown));
			if (!NetworkHelper.CanPing(machineName))
			{
				DebugLog.LogVerbose(string.Format(CultureInfo.InvariantCulture, "Server {0} service status is '{1}' ping failed... aborting Service Control Manager (SCM) connection", machineName, NodeState.Unknown));
				global::_003CModule_003E.SetLastError(1722u);
				throw ExceptionHelp.Build<ApplicationException>(-2147023174, new string[2]
				{
					Resources.OpenServiceManager_Failed_Ping_Text,
					machineName
				});
			}
			DebugLog.LogVerbose(string.Format(CultureInfo.InvariantCulture, "Server {0} ping succeeded... trying Service Control Manager (SCM) connection", machineName));
		}
		using SafeServiceHandle serviceManager = OpenServiceManager(machineName, permissions);
		Construct(serviceManager, serviceName, permissions);
	}

	private void _007EWindowsService()
	{
		SafeServiceHandle service = m_service;
		if (service != null)
		{
			service.Close();
			((IDisposable)m_service)?.Dispose();
			m_service = null;
		}
		UnmanagedBuffer serviceConfig = m_serviceConfig;
		if (serviceConfig != null)
		{
			((IDisposable)serviceConfig).Dispose();
			m_serviceConfig = null;
		}
	}

	public unsafe void Start(params string[] serviceArgs)
	{
		//IL_0046: Expected I8, but got I
		//IL_00ac: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		int num = serviceArgs.Length;
		long num2 = num;
		ushort** ptr;
		if (num == 0)
		{
			ptr = (ushort**)InteropHelp.AllocateArray(8uL);
			*(long*)ptr = 0L;
		}
		else
		{
			ptr = (ushort**)InteropHelp.AllocateArray((ulong)num * 8uL);
			int num3 = 0;
			long num4 = 0L;
			if (0 < num2)
			{
				do
				{
					*(long*)(num4 * 8 + (nint)ptr) = (nint)InteropHelp.StringToWstr(serviceArgs[num3]);
					num3++;
					num4++;
				}
				while (num4 < num2);
			}
		}
		try
		{
			if (global::_003CModule_003E.StartServiceW(m_service.GetHandle(), (uint)num, ptr) == 0)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (1056 != lastWin32Error)
				{
					throw ExceptionHelp.Build<ApplicationException>(lastWin32Error, new string[1] { Resources.StartService_Failed_Text });
				}
			}
		}
		finally
		{
			long num5 = 0L;
			if (0 < num2)
			{
				do
				{
					InteropHelp.FreeWstr((ushort*)(*(ulong*)(num5 * 8 + (nint)ptr)));
					num5++;
				}
				while (num5 < num2);
			}
			InteropHelp.FreeArray(ptr);
		}
	}

	public void Start()
	{
		Start(new string[0]);
	}

	public unsafe void Stop()
	{
		//IL_0010: Expected I4, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _SERVICE_STATUS sERVICE_STATUS);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref sERVICE_STATUS, 0, 28);
		if (global::_003CModule_003E.ControlService(m_service.GetHandle(), 1u, &sERVICE_STATUS) == 0)
		{
			throw ExceptionHelp.Build<ApplicationException>(Marshal.GetLastWin32Error(), new string[1] { Resources.StopService_Failed_Text });
		}
	}

	public unsafe uint GetServiceSpecificStatus()
	{
		ThreadWatchdog.PerformUIThreadCheck();
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _SERVICE_STATUS_PROCESS sERVICE_STATUS_PROCESS);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (global::_003CModule_003E.QueryServiceStatusEx(m_service.GetHandle(), (_SC_STATUS_TYPE)0, (byte*)(&sERVICE_STATUS_PROCESS), 36u, &num) == 0)
		{
			throw new Win32Exception((int)global::_003CModule_003E.GetLastError());
		}
		return (System.Runtime.CompilerServices.Unsafe.As<_SERVICE_STATUS_PROCESS, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sERVICE_STATUS_PROCESS, 12)) == 1066) ? System.Runtime.CompilerServices.Unsafe.As<_SERVICE_STATUS_PROCESS, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sERVICE_STATUS_PROCESS, 16)) : System.Runtime.CompilerServices.Unsafe.As<_SERVICE_STATUS_PROCESS, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sERVICE_STATUS_PROCESS, 12));
	}

	public unsafe static ICollection<WindowsService> GetServices(string machineName, WindowsServicePermissions permissions)
	{
		//IL_0008: Expected I, but got I8
		//IL_0050: Expected I, but got I8
		//IL_0073: Expected I, but got I8
		ThreadWatchdog.PerformUIThreadCheck();
		void* ptr = null;
		List<WindowsService> list = new List<WindowsService>();
		SafeServiceHandle safeServiceHandle = OpenServiceManager(machineName, permissions);
		try
		{
			uint num = 4096u;
			uint num2 = 0u;
			ptr = InteropHelp.AllocateArray(4096uL);
			while (true)
			{
				int num3 = 0;
				uint num4 = 0u;
				uint num5 = 0u;
				int num6 = global::_003CModule_003E.EnumServicesStatusExW(safeServiceHandle.GetHandle(), (_SC_ENUM_TYPE)0, 48u, 3u, (byte*)ptr, num, &num4, &num5, &num2, null);
				uint lastError = global::_003CModule_003E.GetLastError();
				_ENUM_SERVICE_STATUS_PROCESSW* ptr2 = (_ENUM_SERVICE_STATUS_PROCESSW*)ptr;
				for (uint num7 = 0u; num7 < num5; num7++)
				{
					WindowsService service = GetService(safeServiceHandle, (_ENUM_SERVICE_STATUS_PROCESSW*)((long)num7 * 56L + (nint)ptr2), permissions);
					list.Add(service);
				}
				if (0 == num6)
				{
					if (lastError != 234)
					{
						throw ExceptionHelp.Build<ApplicationException>((int)lastError, new string[2]
						{
							Resources.EnumServices_Fail_Text,
							machineName
						});
					}
					int num8 = 1;
					num = num4;
					InteropHelp.ReallocateArray(&ptr, num4);
				}
				else if (num3 == 0)
				{
					break;
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)safeServiceHandle)?.Dispose();
			if (ptr != null)
			{
				InteropHelp.FreeArray(ptr);
			}
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			_007EWindowsService();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
