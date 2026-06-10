using System;

namespace KDDSL.ServerClusters;

public class ConfirmationMessage
{
	private string m_message;

	private string m_subMessage;

	public string MessageSubtext => m_subMessage;

	public string MessageText => m_message;

	public ConfirmationMessage(string message)
	{
		Initialize(message, null);
	}

	public ConfirmationMessage(string message, string subMessage)
	{
		Initialize(message, subMessage);
	}

	private void Initialize(string message, string subMessage)
	{
		if (string.IsNullOrEmpty(message))
		{
			throw new ArgumentNullException("message");
		}
		m_message = message;
		m_subMessage = subMessage;
	}
}
