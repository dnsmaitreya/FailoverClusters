using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSISessionMSFT_iSCSISessionToiSCSIConnection
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSIConnection> iSCSIConnection
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSISessionToiSCSIConnection", "MSFT_iSCSIConnection", "iSCSISession", "iSCSIConnection") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSISessionToiSCSIConnection", "MSFT_iSCSIConnection", "iSCSISession", "iSCSIConnection")
					select new MSFT_iSCSIConnection(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSISessionMSFT_iSCSISessionToiSCSIConnection(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

