using System.Diagnostics;
using System.Windows.Forms;

namespace MS.Internal.ServerClusters.Management;

internal class CursorManager
{
	private Cursor oldCursor;

	private volatile int numOutstanding;

	private Control parentControl;

	private CursorType cursorType;

	private object lockObject;

	internal CursorManager(Control parentControl)
	{
		this.parentControl = parentControl;
		lockObject = new object();
		Reset();
	}

	internal void BeginCursor(CursorType type)
	{
		lock (lockObject)
		{
			cursorType = type;
			numOutstanding++;
			if (numOutstanding == 1)
			{
				oldCursor = parentControl.Cursor;
				parentControl.Cursor = GetCursor();
			}
		}
	}

	[Conditional("CURSOR_MANAGER_TRACING")]
	private void LogCursorData()
	{
		StackTrace stackTrace = new StackTrace(fNeedFileInfo: false);
		DebugLog.LogVerbose("Cursor Count = {0}\n\tStack:{1}", numOutstanding, stackTrace);
	}

	internal void EndCursor()
	{
		lock (lockObject)
		{
			if (--numOutstanding < 0)
			{
				numOutstanding = 0;
			}
			if (numOutstanding == 0)
			{
				parentControl.Cursor = oldCursor;
				Reset();
			}
		}
	}

	private void Reset()
	{
		numOutstanding = 0;
		oldCursor = null;
		cursorType = CursorType.None;
	}

	private Cursor GetCursor()
	{
		Cursor result = null;
		switch (cursorType)
		{
		case CursorType.None:
			DebugLog.LogWarning("CursorType cannot be None");
			break;
		case CursorType.DataLoad:
			result = Cursors.AppStarting;
			break;
		default:
			DebugLog.LogWarning("Unknown CursorType");
			break;
		}
		return result;
	}
}
