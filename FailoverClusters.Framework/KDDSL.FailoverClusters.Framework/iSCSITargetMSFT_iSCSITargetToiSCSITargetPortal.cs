using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSITargetMSFT_iSCSITargetToiSCSITargetPortal
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSITargetPortal> iSCSITargetPortal
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_iSCSITargetToiSCSITargetPortal", "MSFT_iSCSITargetPortal", "iSCSITarget", "iSCSITargetPortal") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_iSCSITargetToiSCSITargetPortal", "MSFT_iSCSITargetPortal", "iSCSITarget", "iSCSITargetPortal")
					select new MSFT_iSCSITargetPortal(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSITargetMSFT_iSCSITargetToiSCSITargetPortal(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

