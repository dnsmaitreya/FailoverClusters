using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSITargetPortalMSFT_iSCSIConnectionToiSCSITargetPortal
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSIConnection> iSCSIConnection
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_iSCSIConnectionToiSCSITargetPortal", "MSFT_iSCSIConnection", "iSCSITargetPortal", "iSCSIConnection") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_iSCSIConnectionToiSCSITargetPortal", "MSFT_iSCSIConnection", "iSCSITargetPortal", "iSCSIConnection")
					select new MSFT_iSCSIConnection(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSITargetPortalMSFT_iSCSIConnectionToiSCSITargetPortal(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

