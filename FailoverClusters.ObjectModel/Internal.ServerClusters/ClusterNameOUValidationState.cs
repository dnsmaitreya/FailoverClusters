using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class ClusterNameOUValidationState
{
	private bool m_createComputerAccess;

	private bool m_readAllPropsAccess;

	private bool m_defaultOU;

	private string m_ouName;

	public string OUName => m_ouName;

	public bool HasReadAllPropsAccess
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_readAllPropsAccess;
		}
	}

	public bool HasCreateComputerPermissions
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_createComputerAccess;
		}
	}

	public bool IsDefaultOU
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_defaultOU;
		}
	}

	internal ClusterNameOUValidationState(int createComputerAccess, int readAllPropsAccess, int defaultOU, string ouName)
	{
		m_createComputerAccess = ((createComputerAccess != 0) ? true : false);
		m_readAllPropsAccess = ((readAllPropsAccess != 0) ? true : false);
		m_defaultOU = ((defaultOU != 0) ? true : false);
		m_ouName = ouName;
	}
}
