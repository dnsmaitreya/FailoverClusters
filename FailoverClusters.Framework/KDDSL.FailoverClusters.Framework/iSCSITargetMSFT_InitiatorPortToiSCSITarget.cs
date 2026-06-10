using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSITargetMSFT_InitiatorPortToiSCSITarget
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_InitiatorPort> InitiatorPort
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSITarget", "MSFT_InitiatorPort", "iSCSITarget", "InitiatorPort") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSITarget", "MSFT_InitiatorPort", "iSCSITarget", "InitiatorPort")
					select new MSFT_InitiatorPort(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSITargetMSFT_InitiatorPortToiSCSITarget(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

