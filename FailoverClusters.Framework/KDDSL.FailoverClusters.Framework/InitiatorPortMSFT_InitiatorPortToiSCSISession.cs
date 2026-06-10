using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_InitiatorPortMSFT_InitiatorPortToiSCSISession
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSISession> iSCSISession
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSISession", "MSFT_iSCSISession", "InitiatorPort", "iSCSISession") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSISession", "MSFT_iSCSISession", "InitiatorPort", "iSCSISession")
					select new MSFT_iSCSISession(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_InitiatorPortMSFT_InitiatorPortToiSCSISession(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

