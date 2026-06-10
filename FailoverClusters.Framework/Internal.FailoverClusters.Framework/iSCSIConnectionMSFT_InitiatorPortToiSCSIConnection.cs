using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSIConnectionMSFT_InitiatorPortToiSCSIConnection
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_InitiatorPort> InitiatorPort
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_InitiatorPortToiSCSIConnection", "MSFT_InitiatorPort", "iSCSIConnection", "InitiatorPort") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_InitiatorPortToiSCSIConnection", "MSFT_InitiatorPort", "iSCSIConnection", "InitiatorPort")
					select new MSFT_InitiatorPort(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSIConnectionMSFT_InitiatorPortToiSCSIConnection(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

