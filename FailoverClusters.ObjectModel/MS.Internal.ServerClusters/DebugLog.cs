using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.FailoverClusters.UI.Common;
using Microsoft.Win32;

namespace MS.Internal.ServerClusters;

public sealed class DebugLog
{
	private static bool? m_extraExceptionData = null;

	private static bool? m_enablePrivateComponents = null;

	private static bool? m_enableLegacyComponents = null;

	public static string PerfCounterCategoryName = "Failover Cluster Manager";

	public static bool LegacyComponentsEnabled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			string value = "EnableLegacyComponents";
			return GetUserRegistryValue(m_enableLegacyComponents, value);
		}
	}

	public static bool PrivateComponentsEnabled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			string value = "EnablePrivateComponents";
			return GetUserRegistryValue(m_enablePrivateComponents, value);
		}
	}

	public static bool ExtraExceptionData
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			string value = "ExceptionData";
			return GetUserRegistryValue(m_extraExceptionData, value);
		}
	}

	public static string EnableLegacyComponents => "EnableLegacyComponents";

	public static string EnablePrivateComponents => "EnablePrivateComponents";

	public static string ExceptionDataName => "ExceptionData";

	public static string ManagementSettingsKeyName => "Software\\Microsoft\\Windows\\FailoverClusters";

	public static RegistryKey ManagementSettingsKey
	{
		get
		{
			string name = "Software\\Microsoft\\Windows\\FailoverClusters";
			return Registry.CurrentUser.OpenSubKey(name, writable: false);
		}
	}

	private DebugLog()
	{
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private static bool GetUserRegistryValue(bool? setting, string value)
	{
		if (!setting.HasValue)
		{
			setting = false;
			RegistryKey managementSettingsKey = ManagementSettingsKey;
			if (managementSettingsKey != null)
			{
				try
				{
					if (managementSettingsKey.GetValue(value) != null)
					{
						bool? flag = true;
						bool? flag2 = flag;
						setting = flag;
					}
				}
				catch (Exception caughtException)
				{
					ExceptionHelp.LogException(caughtException, "Failed to get the {0} value from the registry.", value);
				}
				finally
				{
					((IDisposable)managementSettingsKey)?.Dispose();
				}
			}
		}
		return (bool)setting;
	}

	internal static void LogException(LogLevel level, Exception exception, string debugMessage)
	{
		string message = string.Format(CultureInfo.CurrentCulture, "{0} - {1}", debugMessage, exception);
		switch (level)
		{
		case LogLevel.Critical:
			ClusterLog.LogCritical(message);
			break;
		case LogLevel.Error:
			ClusterLog.LogError(message);
			break;
		case LogLevel.Warning:
			ClusterLog.LogWarning(message);
			break;
		case LogLevel.Info:
			ClusterLog.LogInfo(message);
			break;
		case LogLevel.Verbose:
			ClusterLog.LogVerbose(LogSubcategory.ClusWinFx, message);
			break;
		}
	}

	internal static void LogException(Exception exception, string debugMessage)
	{
		LogException(LogLevel.Error, exception, debugMessage);
	}

	public static string GetStackTrace(StackTrace stackTrace)
	{
		try
		{
			StackFrame[] frames = stackTrace.GetFrames();
			StringBuilder stringBuilder = new StringBuilder();
			StackFrame[] array = frames;
			for (int i = 0; i < (nint)array.LongLength; i++)
			{
				StackFrame stackFrame = array[i];
				if (stackFrame.GetMethod().DeclaringType != null)
				{
					stringBuilder.Append(stackFrame.GetMethod().DeclaringType.ToString());
				}
				else
				{
					stringBuilder.Append("Global instance type");
				}
				stringBuilder.Append(".");
				stringBuilder.Append(stackFrame.GetMethod().Name);
				stringBuilder.Append("(");
				bool flag = true;
				ParameterInfo[] parameters = stackFrame.GetMethod().GetParameters();
				for (int j = 0; j < (nint)parameters.LongLength; j++)
				{
					ParameterInfo parameterInfo = parameters[j];
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(parameterInfo.ParameterType.Name);
					stringBuilder.Append(" ");
					stringBuilder.Append(parameterInfo.Name);
					flag = false;
				}
				stringBuilder.Append(") ");
				if (stackFrame.GetFileLineNumber() != 0)
				{
					stringBuilder.Append(stackFrame.GetFileName());
					stringBuilder.Append("(");
					stringBuilder.Append(stackFrame.GetFileLineNumber());
					stringBuilder.AppendLine(")");
				}
				else
				{
					stringBuilder.AppendLine(stackFrame.GetFileName());
				}
			}
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}
		catch (Exception exception)
		{
			return string.Format(CultureInfo.CurrentCulture, "Error getting the call stack:\n{0}", ExceptionHelp.GetExceptionMessage(exception));
		}
	}

	public static string GetStackTrace()
	{
		return GetStackTrace(new StackTrace(1, fNeedFileInfo: true));
	}

	public static void LogInfo(string format, params object[] args)
	{
		ClusterLog.LogInfo(format, args);
	}

	public static void LogInfo(string debugMessage)
	{
		ClusterLog.LogInfo(debugMessage);
	}

	public static void LogWarning(string format, params object[] args)
	{
		ClusterLog.LogWarning(format, args);
	}

	public static void LogWarning(string debugMessage)
	{
		ClusterLog.LogWarning(debugMessage);
	}

	public static void LogVerbose(string format, params object[] args)
	{
		ClusterLog.LogVerbose(LogSubcategory.ClusWinFx, format, args);
	}

	public static void LogVerbose(string debugMessage)
	{
		ClusterLog.LogVerbose(LogSubcategory.ClusWinFx, debugMessage);
	}

	public static void LogError(string format, params object[] args)
	{
		ClusterLog.LogError(format, args);
	}

	public static void LogError(string debugMessage)
	{
		ClusterLog.LogError(debugMessage);
	}

	public static void LogCritical(string format, params object[] args)
	{
		ClusterLog.LogCritical(format, args);
	}

	public static void LogCritical(string debugMessage)
	{
		ClusterLog.LogCritical(debugMessage);
	}
}
