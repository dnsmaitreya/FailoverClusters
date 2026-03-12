using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSITargetPortalMSFT_iSCSITargetToiSCSITargetPortal
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSITarget> iSCSITarget
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSITargetToiSCSITargetPortal", "MSFT_iSCSITarget", "iSCSITargetPortal", "iSCSITarget") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSITargetToiSCSITargetPortal", "MSFT_iSCSITarget", "iSCSITargetPortal", "iSCSITarget")
					select new MSFT_iSCSITarget(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSITargetPortalMSFT_iSCSITargetToiSCSITargetPortal(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
