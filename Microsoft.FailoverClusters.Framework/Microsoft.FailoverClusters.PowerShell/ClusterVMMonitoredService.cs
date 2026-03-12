using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;
using MS.Internal.FailoverClusters.Framework;

namespace Microsoft.FailoverClusters.PowerShell;

[CLSCompliant(false)]
public class ClusterVMMonitoredService : ClusterVMMonitoredItem
{
	public ClusterVMMonitoredService(string name, Guid id, string vmName)
		: base("Service", name, id, vmName)
	{
	}

	internal override void RegisterItem(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string computerName, bool overrideSettings = false)
	{
		string text = ValidateServiceAndDetermineDisplayName(computerName, overrideSettings);
		CreateItemKey(computerName);
		base.ItemKey.SetValue("DisplayName", text);
		string eventQuery = "<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Service Control Manager'] and (EventID=7034)] and EventData[Data[@Name='param1'] = '" + text + "' ]]</Select></Query></QueryList>";
		try
		{
			CreateTask(taskService, taskFolder, eventQuery, CommonResources.ClusterVMMonitoredService_FailureTaskDescription.FormatCurrentCulture(base.Name), base.Name + CommonResources.NameSuffixServiceFailure, base.Name, base.Id.ToString(), "1");
			eventQuery = "<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Service Control Manager'] and (EventID=7036)] and EventData[Data[@Name='param1'] = '" + text + "' ]and EventData[Data[@Name='param2'] = 'running' ]]</Select></Query></QueryList>";
			CreateTask(taskService, taskFolder, eventQuery, CommonResources.ClusterVMMonitoredService_SuccessTaskDescription.FormatCurrentCulture(base.Name), base.Name + CommonResources.NameSuffixServiceSuccess, base.Name, base.Id.ToString(), "0");
		}
		catch (Exception)
		{
			DeleteItemKey(computerName, taskService);
			throw;
		}
	}

	internal override void UnregisterItem(NativeMethods.ITaskService taskService, NativeMethods.ITaskFolder taskFolder, string computerName)
	{
		Exception ex = null;
		try
		{
			taskFolder.DeleteTask(base.Name + CommonResources.NameSuffixServiceSuccess, 0);
		}
		catch (Exception ex2)
		{
			ex = ex2;
		}
		try
		{
			taskFolder.DeleteTask(base.Name + CommonResources.NameSuffixServiceFailure, 0);
		}
		catch (Exception ex3)
		{
			ex = ex3;
		}
		DeleteItemKey(computerName, taskService);
		if (ex != null)
		{
			throw ex;
		}
	}

	private static void CopyMemory(IntPtr source, IntPtr destination, int byteCount)
	{
		byte[] array = new byte[byteCount];
		Marshal.Copy(source, array, 0, byteCount);
		Marshal.Copy(array, 0, destination, byteCount);
	}

	private string ValidateServiceAndDetermineDisplayName(string computerName, bool overrideSettings)
	{
		string text = new ServiceController(base.Name, computerName).DisplayName;
		if (string.IsNullOrEmpty(text))
		{
			text = base.Name;
		}
		using (SafeServiceHandle hSCManager = GetServiceHandle(computerName, overrideSettings))
		{
			int pcbBytesNeeded = 0;
			bool flag = false;
			if (!NativeMethods.QueryServiceConfig2(hSCManager, 2, IntPtr.Zero, 0, ref pcbBytesNeeded))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 122)
				{
					throw new Win32Exception(lastWin32Error);
				}
				using NativeMethods.UnmanagedBuffer unmanagedBuffer = new NativeMethods.UnmanagedBuffer(pcbBytesNeeded);
				NativeMethods.SC_ACTION structure = default(NativeMethods.SC_ACTION);
				using NativeMethods.UnmanagedBuffer unmanagedBuffer2 = new NativeMethods.UnmanagedBuffer(Marshal.SizeOf(structure) * 3);
				if (unmanagedBuffer.IsMemoryValid)
				{
					if (!NativeMethods.QueryServiceConfig2(hSCManager, 2, unmanagedBuffer.IntPtr, pcbBytesNeeded, ref pcbBytesNeeded))
					{
						lastWin32Error = Marshal.GetLastWin32Error();
						throw new Win32Exception(lastWin32Error);
					}
					NativeMethods.SERVICE_FAILURE_ACTIONS sERVICE_FAILURE_ACTIONS = (NativeMethods.SERVICE_FAILURE_ACTIONS)Marshal.PtrToStructure(unmanagedBuffer.IntPtr, typeof(NativeMethods.SERVICE_FAILURE_ACTIONS));
					if (sERVICE_FAILURE_ACTIONS.numActions == 0)
					{
						return text;
					}
					bool flag2 = false;
					for (int i = 0; i < 3; i++)
					{
						if (i < sERVICE_FAILURE_ACTIONS.numActions)
						{
							CopyMemory(sERVICE_FAILURE_ACTIONS.actions + i * Marshal.SizeOf(structure), unmanagedBuffer2.IntPtr + i * Marshal.SizeOf(structure), Marshal.SizeOf(structure));
						}
						else
						{
							CopyMemory(sERVICE_FAILURE_ACTIONS.actions + (sERVICE_FAILURE_ACTIONS.numActions - 1) * Marshal.SizeOf(structure), unmanagedBuffer2.IntPtr + i * Marshal.SizeOf(structure), Marshal.SizeOf(structure));
						}
					}
					sERVICE_FAILURE_ACTIONS.numActions = 3;
					sERVICE_FAILURE_ACTIONS.actions = unmanagedBuffer2.IntPtr;
					Marshal.WriteInt32(unmanagedBuffer.IntPtr, Marshal.OffsetOf(typeof(NativeMethods.SERVICE_FAILURE_ACTIONS), "numActions").ToInt32(), sERVICE_FAILURE_ACTIONS.numActions);
					Marshal.WriteIntPtr(unmanagedBuffer.IntPtr, Marshal.OffsetOf(typeof(NativeMethods.SERVICE_FAILURE_ACTIONS), "actions").ToInt32(), unmanagedBuffer2.IntPtr);
					for (int j = 0; j < sERVICE_FAILURE_ACTIONS.numActions; j++)
					{
						NativeMethods.SC_ACTION sC_ACTION = (NativeMethods.SC_ACTION)Marshal.PtrToStructure(sERVICE_FAILURE_ACTIONS.actions + j * Marshal.SizeOf(structure), typeof(NativeMethods.SC_ACTION));
						if (sC_ACTION.type == NativeMethods.SC_ACTION_TYPE.SC_ACTION_NONE)
						{
							flag2 = true;
							if (!flag)
							{
								break;
							}
							continue;
						}
						if (sC_ACTION.type == NativeMethods.SC_ACTION_TYPE.SC_ACTION_REBOOT)
						{
							if (!overrideSettings)
							{
								throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, ExceptionResources.ServiceInvalidRecoveryAction, text));
							}
							flag2 = true;
							flag = true;
							Marshal.WriteInt32(sERVICE_FAILURE_ACTIONS.actions + j * Marshal.SizeOf(structure), 0);
						}
						if (j == sERVICE_FAILURE_ACTIONS.numActions - 1 && !flag2 && overrideSettings)
						{
							flag = true;
							flag2 = true;
							Marshal.WriteInt32(sERVICE_FAILURE_ACTIONS.actions + j * Marshal.SizeOf(structure), 0);
						}
					}
					if (flag && !NativeMethods.ChangeServiceConfig2(hSCManager, 2, unmanagedBuffer.IntPtr))
					{
						lastWin32Error = Marshal.GetLastWin32Error();
						throw new Win32Exception(lastWin32Error);
					}
					if (!flag2)
					{
						throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, ExceptionResources.ServiceTooManyRecoveryActions, text));
					}
				}
			}
		}
		return text;
	}

	private SafeServiceHandle GetServiceHandle(string computerName, bool writeAccess = false)
	{
		return GetServiceHandle(computerName, base.Name, writeAccess);
	}

	internal static SafeServiceHandle GetServiceHandle(string computerName, string serviceName, bool writeAccess)
	{
		using SafeServiceHandle safeServiceHandle = NativeMethods.OpenSCManager(computerName, null, writeAccess ? 983103 : 4);
		if (safeServiceHandle.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 0)
			{
				throw new Win32Exception(lastWin32Error);
			}
		}
		SafeServiceHandle safeServiceHandle2 = NativeMethods.OpenService(safeServiceHandle, serviceName, (!writeAccess) ? 1 : 983551);
		if (safeServiceHandle2.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			throw new Win32Exception(lastWin32Error);
		}
		return safeServiceHandle2;
	}
}
