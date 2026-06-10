using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using FailoverClusters.UI.Common;
using Virtualization.Client.Common;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public static class UIHelper
{
	private class OldWindow : System.Windows.Forms.IWin32Window
	{
		private readonly IntPtr handle;

		public IntPtr Handle => handle;

		public OldWindow(IntPtr handle)
		{
			this.handle = handle;
		}
	}

	public static bool AssertHyperVToolsSupport(IServer server, HyperVComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return VerifyHyperVToolsSupport(server, component, errorAsDialog: true);
	}

	public static bool VerifyHyperVToolsSupport(IServer server, HyperVComponent component, bool errorAsDialog)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string message = default(string);
		if (!server.IsHyperVComponentSupported(component, ref message))
		{
			Exception ex = new ClusterHyperVNotSupportedException(message);
			ClusterLog.LogException(ex, "An error occurred running the VM Settings dialog");
			if (errorAsDialog)
			{
				ClusterDialogException.ShowTaskDialog(ex);
				return false;
			}
			throw ex;
		}
		return true;
	}

	internal static void ExecuteOnDispatcher(Queue<Action> functions)
	{
		Action<Queue<Action>> uiActions = delegate
		{
			while (functions.Count > 0)
			{
				functions.Dequeue()();
			}
		};
		if (Global.DefaultDispatcher != null)
		{
			Global.DefaultDispatcher.BeginInvoke(uiActions, functions);
		}
		else if (System.Windows.Application.Current != null && System.Windows.Application.Current.Dispatcher != null)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(uiActions, functions);
		}
		else
		{
			Worker.Start(delegate
			{
				uiActions(functions);
			});
		}
	}

	public static void ExecuteOnDispatcher(Action function, OperationType operationType)
	{
		ExecuteOnDispatcher(function, operationType, null);
	}

	internal static void ExecuteOnDispatcher(Action function, OperationType operationType, Queue<Action> actionQueue)
	{
		if (function == null)
		{
			return;
		}
		if (operationType == OperationType.Sync)
		{
			if (Global.DefaultDispatcher != null)
			{
				Global.DefaultDispatcher.Invoke(function);
			}
			else
			{
				function();
			}
		}
		else if (actionQueue != null)
		{
			actionQueue.Enqueue(function);
		}
		else if (Global.DefaultDispatcher != null)
		{
			if (Global.DefaultDispatcher.Thread == Thread.CurrentThread)
			{
				function();
			}
			else
			{
				Global.DefaultDispatcher.BeginInvoke(function);
			}
		}
		else if (System.Windows.Application.Current != null && System.Windows.Application.Current.Dispatcher != null)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(function);
		}
		else
		{
			Worker.Start(delegate
			{
				function();
			});
		}
	}

	internal static void ExecuteOnDispatcher<T>(Action<T> function, OperationType operationType, T parameter)
	{
		ExecuteOnDispatcher(function, operationType, null, parameter);
	}

	private static void ExecuteOnDispatcher<T>(Action<T> function, OperationType operationType, Queue<Action<T>> actionQueue, T parameter)
	{
		if (operationType == OperationType.Sync)
		{
			if (Global.DefaultDispatcher != null)
			{
				Global.DefaultDispatcher.Invoke(function, parameter);
			}
			else
			{
				function(parameter);
			}
		}
		else if (actionQueue != null)
		{
			actionQueue.Enqueue(function);
		}
		else if (Global.DefaultDispatcher != null)
		{
			if (Global.DefaultDispatcher.Thread == Thread.CurrentThread)
			{
				function(parameter);
				return;
			}
			Global.DefaultDispatcher.BeginInvoke(function, parameter);
		}
		else if (System.Windows.Application.Current != null && System.Windows.Application.Current.Dispatcher != null)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(function, parameter);
		}
		else
		{
			Worker.Start(delegate
			{
				function(parameter);
			});
		}
	}

	public static bool ApplicationActivate(Process process)
	{
		if (process == null)
		{
			return false;
		}
		try
		{
			if (process.HasExited)
			{
				return false;
			}
			int num = 100;
			while (--num > 0 && process.MainWindowHandle == IntPtr.Zero)
			{
				Thread.Sleep(50);
				process.Refresh();
				if (process.HasExited)
				{
					return false;
				}
			}
			return ApplicationActivate(process.MainWindowHandle);
		}
		catch (Exception exception)
		{
			ClusterLog.LogException(exception, "Could not activate application for the current process.");
			return false;
		}
	}

	public static bool ApplicationActivate(IntPtr windowsHandle)
	{
		try
		{
			if (windowsHandle == IntPtr.Zero)
			{
				return false;
			}
			int lpdwProcessId = -1;
			new UIPermission(UIPermissionWindow.AllWindows).Demand();
			if (!NativeMethods.IsWindowEnabled(windowsHandle) || !NativeMethods.IsWindowVisible(windowsHandle))
			{
				IntPtr window = NativeMethods.GetWindow(windowsHandle, NativeMethods.GetWindow_Cmd.GW_HWNDFIRST);
				while (window != IntPtr.Zero)
				{
					if (NativeMethods.GetWindow(window, NativeMethods.GetWindow_Cmd.GW_OWNER) == windowsHandle)
					{
						if (NativeMethods.IsWindowEnabled(window) && NativeMethods.IsWindowVisible(window))
						{
							break;
						}
						windowsHandle = window;
						window = NativeMethods.GetWindow(windowsHandle, NativeMethods.GetWindow_Cmd.GW_HWNDFIRST);
					}
					window = NativeMethods.GetWindow(window, NativeMethods.GetWindow_Cmd.GW_HWNDNEXT);
				}
				if (window == IntPtr.Zero)
				{
					return false;
				}
				windowsHandle = window;
			}
			int windowThreadProcessId = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), ref lpdwProcessId);
			int windowThreadProcessId2 = NativeMethods.GetWindowThreadProcessId(windowsHandle, ref lpdwProcessId);
			if (windowThreadProcessId != windowThreadProcessId2)
			{
				bool num = NativeMethods.AttachThreadInput(windowThreadProcessId, windowThreadProcessId2, 1);
				NativeMethods.SetForegroundWindow(windowsHandle);
				NativeMethods.SetActiveWindow(windowsHandle);
				if (num)
				{
					NativeMethods.AttachThreadInput(windowThreadProcessId, windowThreadProcessId2, 0);
				}
			}
			else
			{
				NativeMethods.SetForegroundWindow(windowsHandle);
				NativeMethods.SetActiveWindow(windowsHandle);
			}
		}
		catch (Exception exception)
		{
			ClusterLog.LogException(exception, "Could not activate application for handle '{0}.".FormatCurrentCulture());
			return false;
		}
		return true;
	}

	public static System.Windows.Forms.IWin32Window GetIWin32Window(Visual visual)
	{
		IntPtr handle = IntPtr.Zero;
		if (visual is Window window)
		{
			handle = new WindowInteropHelper(window).Handle;
		}
		else if (PresentationSource.FromVisual(visual) is HwndSource hwndSource)
		{
			handle = hwndSource.Handle;
		}
		return new OldWindow(handle);
	}

	internal static string FindSystem32Application(string application)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("windir");
		if (environmentVariable == null)
		{
			return null;
		}
		string text = Path.Combine(environmentVariable, Path.Combine("system32", application));
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}
}

