namespace MS.Internal.ServerClusters;

internal class IPAddressConfigureData
{
	private ClusterResource m_resource;

	private IPAddressInfo m_ipAddressInfo;

	public ClusterResource Resource
	{
		get
		{
			return m_resource;
		}
		set
		{
			m_resource = value;
		}
	}

	public IPAddressInfo IPAddressInfo => m_ipAddressInfo;

	public IPAddressConfigureData(IPAddressInfo ipAddressInfo)
	{
		m_ipAddressInfo = ipAddressInfo;
	}
}
