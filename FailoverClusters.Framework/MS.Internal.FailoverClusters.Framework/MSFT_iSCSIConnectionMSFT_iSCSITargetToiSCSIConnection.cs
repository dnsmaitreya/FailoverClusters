using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSIConnectionMSFT_iSCSITargetToiSCSIConnection
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSITarget> iSCSITarget
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSITargetToiSCSIConnection", "MSFT_iSCSITarget", "iSCSIConnection", "iSCSITarget") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSITargetToiSCSIConnection", "MSFT_iSCSITarget", "iSCSIConnection", "iSCSITarget")
					select new MSFT_iSCSITarget(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSIConnectionMSFT_iSCSITargetToiSCSIConnection(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

