using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSIConnectionMSFT_iSCSISessionToiSCSIConnection
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSISession> iSCSISession
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSISessionToiSCSIConnection", "MSFT_iSCSISession", "iSCSIConnection", "iSCSISession") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSISessionToiSCSIConnection", "MSFT_iSCSISession", "iSCSIConnection", "iSCSISession")
					select new MSFT_iSCSISession(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSIConnectionMSFT_iSCSISessionToiSCSIConnection(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
