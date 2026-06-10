using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_InitiatorPortMSFT_InitiatorPortToiSCSITarget
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSITarget> iSCSITarget
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSITarget", "MSFT_iSCSITarget", "InitiatorPort", "iSCSITarget") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_InitiatorPortToiSCSITarget", "MSFT_iSCSITarget", "InitiatorPort", "iSCSITarget")
					select new MSFT_iSCSITarget(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_InitiatorPortMSFT_InitiatorPortToiSCSITarget(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

