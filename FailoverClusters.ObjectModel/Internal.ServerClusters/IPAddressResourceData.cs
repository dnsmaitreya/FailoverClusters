using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

internal class IPAddressResourceData
{
	private ClusterResource m_resource;

	private IPAddressInfo m_ipAddressInfo;

	private bool m_used;

	public bool IsUsed
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_used;
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			m_used = value;
		}
	}

	public ClusterResource Resource => m_resource;

	public IPAddressInfo IPAddressInfo => m_ipAddressInfo;

	public IPAddressResourceData(ClusterResource resource, IPAddressInfo ipAddressInfo)
	{
		m_ipAddressInfo = ipAddressInfo;
		m_resource = resource;
		m_used = false;
	}
}
