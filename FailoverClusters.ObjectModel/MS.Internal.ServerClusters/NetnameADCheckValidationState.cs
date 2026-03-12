using System.Runtime.InteropServices;

namespace MS.Internal.ServerClusters;

public class NetnameADCheckValidationState
{
	private bool m_dcUnreachable;

	private bool m_badPwd;

	private bool m_noObject;

	private bool m_disabledAccount;

	public bool IsAccountDisabled
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_disabledAccount;
		}
	}

	public bool IsObjectMissing
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_noObject;
		}
	}

	public bool HasBadPwd
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_badPwd;
		}
	}

	public bool IsDCUnreachable
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_dcUnreachable;
		}
	}

	internal unsafe NetnameADCheckValidationState(_CLUSCTL_RESOURCE_NETNAME_CHECK_AD_STATE_OUTPUT* pCheckADStateOutput)
	{
		int num = *(int*)pCheckADStateOutput;
		int dcUnreachable = ((num == 1) ? 1 : 0);
		m_dcUnreachable = (byte)dcUnreachable != 0;
		int badPwd = ((num == 4) ? 1 : 0);
		m_badPwd = (byte)badPwd != 0;
		int noObject = ((num == 2) ? 1 : 0);
		m_noObject = (byte)noObject != 0;
		int disabledAccount = ((num == 8) ? 1 : 0);
		m_disabledAccount = (byte)disabledAccount != 0;
	}
}
