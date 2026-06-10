using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSIConnectionMSFT_iSCSIConnectionToiSCSITargetPortal
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSITargetPortal> iSCSITargetPortal
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSIConnectionToiSCSITargetPortal", "MSFT_iSCSITargetPortal", "iSCSIConnection", "iSCSITargetPortal") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSIConnectionToiSCSITargetPortal", "MSFT_iSCSITargetPortal", "iSCSIConnection", "iSCSITargetPortal")
					select new MSFT_iSCSITargetPortal(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSIConnectionMSFT_iSCSIConnectionToiSCSITargetPortal(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

