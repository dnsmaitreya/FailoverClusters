#define DEBUG
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KDDSL.ServerClusters;

public sealed class ThreadWatchdog
{
	private static Dictionary<int, string> m_uiThreadsStacks;

	private static Dictionary<int, int> m_uiThreads;

	private static ReaderWriterLockSlim m_uiThreadLock;

	private static bool extraExceptionData;

	private static int initialUIThreadId;

	private static volatile bool messageOnScreen;

	private static IWin32Window initialUIControl;

	private static string msgText;

	private static string msgCaption;

	static ThreadWatchdog()
	{
		m_uiThreadsStacks = new Dictionary<int, string>();
		m_uiThreads = new Dictionary<int, int>();
		extraExceptionData = false;
		initialUIThreadId = 0;
		messageOnScreen = false;
		initialUIControl = null;
		if (DebugLog.ExtraExceptionData)
		{
			extraExceptionData = true;
		}
		m_uiThreadLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
	}

	private static void ShowMessageOnScreen(object state)
	{
		if (!messageOnScreen)
		{
			messageOnScreen = true;
			MessageBox.Show(msgText, msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			messageOnScreen = false;
		}
	}

	public static void RegisterUIThread(IWin32Window ownerControl)
	{
		string text = null;
		m_uiThreadLock.EnterWriteLock();
		try
		{
			Thread currentThread = Thread.CurrentThread;
			int value = (m_uiThreads.TryGetValue(currentThread.ManagedThreadId, out value) ? value : 0);
			value++;
			m_uiThreads[currentThread.ManagedThreadId] = value;
			if (!extraExceptionData)
			{
				return;
			}
			if (initialUIControl == null)
			{
				initialUIControl = ownerControl;
			}
			string stackTrace = DebugLog.GetStackTrace();
			text = null;
			if (!m_uiThreadsStacks.TryGetValue(currentThread.ManagedThreadId, out text))
			{
				m_uiThreadsStacks[currentThread.ManagedThreadId] = stackTrace;
			}
			if (m_uiThreadsStacks.Keys.Count == 1)
			{
				initialUIThreadId = currentThread.ManagedThreadId;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Registering a second UI thread.  This should never happen!!\n\n");
			stringBuilder.AppendLine(stackTrace);
			stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, "UI Thread '{0}', new UI Thread '{1}'", initialUIThreadId, currentThread.ManagedThreadId));
			if (Debugger.IsAttached)
			{
				Debugger.Log(4, "Critical", stringBuilder.ToString());
				return;
			}
			DebugLog.LogCritical(stringBuilder.ToString());
			msgText = stringBuilder.ToString();
			msgCaption = "WARNING: Unstable Condition...";
			ThreadPool.QueueUserWorkItem(ShowMessageOnScreen);
			Debug.Assert(condition: false, stringBuilder.ToString());
		}
		finally
		{
			m_uiThreadLock.ExitWriteLock();
		}
	}

	public static void UnregisterUIThread()
	{
		m_uiThreadLock.EnterWriteLock();
		try
		{
			Thread currentThread = Thread.CurrentThread;
			if (m_uiThreads.TryGetValue(currentThread.ManagedThreadId, out var value))
			{
				value += -1;
				if (value == 0)
				{
					m_uiThreads.Remove(currentThread.ManagedThreadId);
				}
				else
				{
					m_uiThreads[currentThread.ManagedThreadId] = value;
				}
			}
		}
		finally
		{
			m_uiThreadLock.ExitWriteLock();
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public static bool IsUIThread()
	{
		//Discarded unreachable code: IL_0038
		m_uiThreadLock.EnterReadLock();
		try
		{
			Thread currentThread = Thread.CurrentThread;
			if (!m_uiThreads.TryGetValue(currentThread.ManagedThreadId, out var _))
			{
				return false;
			}
			return true;
		}
		finally
		{
			m_uiThreadLock.ExitReadLock();
		}
	}

	public static void PerformUIThreadCheck()
	{
	}
}
