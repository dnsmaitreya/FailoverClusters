namespace KDDSL.ServerClusters;

internal class NotificationPayload
{
	private ulong m_dwNotifyKey;

	private string m_name;

	private uint m_dwFilterType;

	internal ulong NotifyKey => m_dwNotifyKey;

	public uint FilterType => m_dwFilterType;

	public string Name => m_name;

	internal NotificationPayload(ulong dwNotifyKey, string name, uint dwFilterType)
	{
		m_dwNotifyKey = dwNotifyKey;
		m_name = name;
		m_dwFilterType = dwFilterType;
	}
}
