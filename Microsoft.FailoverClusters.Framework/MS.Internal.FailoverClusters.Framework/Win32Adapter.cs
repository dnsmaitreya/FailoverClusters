using System;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.FailoverClusters.Framework;
using Microsoft.FailoverClusters.UI.Common;

namespace MS.Internal.FailoverClusters.Framework;

public class Win32Adapter : IWin32Adapter
{
	private const string IsVirtualMachineQuery = "SELECT Manufacturer FROM Win32_ComputerSystem";

	public bool IsVirtualMachine(string hostName)
	{
		return ExecuteAndCatchWmiExceptions(delegate
		{
			ManagementScope win32WmiConnection = WmiHelper.GetWin32WmiConnection(hostName);
			bool isVirtualMachine = false;
			win32WmiConnection.GetFirstElement("SELECT Manufacturer FROM Win32_ComputerSystem", delegate(ManagementObject managementObject)
			{
				isVirtualMachine = managementObject.GetString("Manufacturer").Equals("Microsoft Corporation", StringComparison.CurrentCultureIgnoreCase);
			});
			return isVirtualMachine;
		});
	}

	private T ExecuteAndCatchWmiExceptions<T>(Func<T> action)
	{
		try
		{
			return action.SafeCall();
		}
		catch (ManagementException exception)
		{
			ClusterDialogException ex = ConvertException(exception);
			if (ex != null)
			{
				throw ex;
			}
			throw;
		}
		catch (COMException exception2)
		{
			ClusterDialogException ex2 = ConvertException(exception2);
			if (ex2 != null)
			{
				throw ex2;
			}
			throw;
		}
		catch (UnauthorizedAccessException exception3)
		{
			ClusterDialogException ex3 = ConvertException(exception3);
			if (ex3 != null)
			{
				throw ex3;
			}
			throw;
		}
	}

	private void ExecuteAndCatchWmiExceptions(Action action)
	{
		ExecuteAndCatchWmiExceptions((Func<object>)delegate
		{
			action.SafeCall();
			return null;
		});
	}

	private ClusterDialogException ConvertException(Exception exception)
	{
		if (exception is ManagementException ex)
		{
			return new ClusterDefaultException(new ClusterWmiWin32Exception((int)ex.ErrorCode, ex.Message, ex.StackTrace));
		}
		if (exception is COMException ex2)
		{
			return new ClusterDefaultException(new ClusterWmiWin32Exception(ex2.ErrorCode, exception.StackTrace));
		}
		if (exception is UnauthorizedAccessException)
		{
			return new ClusterDefaultException(exception);
		}
		return null;
	}
}
