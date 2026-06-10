using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSISessionMSFT_InitiatorPortToiSCSISession
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_InitiatorPort> InitiatorPort
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSISession", "MSFT_InitiatorPort", "iSCSISession", "InitiatorPort") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSISession", "MSFT_InitiatorPort", "iSCSISession", "InitiatorPort")
					select new MSFT_InitiatorPort(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSISessionMSFT_InitiatorPortToiSCSISession(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

