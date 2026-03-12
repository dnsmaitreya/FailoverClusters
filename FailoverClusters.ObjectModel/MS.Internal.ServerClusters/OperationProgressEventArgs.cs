using System;

namespace MS.Internal.ServerClusters;

public class OperationProgressEventArgs : EventArgs
{
	private string m_message;

	private OperationProgressWarningLevel m_warningLevel;

	private int m_percentDone;

	public int PercentDone => m_percentDone;

	public OperationProgressWarningLevel WarningLevel => m_warningLevel;

	public string Message => m_message;

	private void Construct(OperationProgressWarningLevel warningLevel, string message, int percentDone)
	{
		m_message = message;
		m_warningLevel = warningLevel;
		m_percentDone = percentDone;
	}

	internal OperationProgressEventArgs(OperationProgressWarningLevel warningLevel, string message, int percentDone)
	{
		Construct(warningLevel, message, percentDone);
	}

	internal OperationProgressEventArgs(OperationProgressWarningLevel warningLevel, string message, int workComplete, int totalWork)
	{
		Construct(warningLevel, message, (int)((double)workComplete / (double)totalWork * 100.0));
	}
}
