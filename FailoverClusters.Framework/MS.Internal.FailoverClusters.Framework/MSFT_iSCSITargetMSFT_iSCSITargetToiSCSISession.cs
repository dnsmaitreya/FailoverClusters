using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSITargetMSFT_iSCSITargetToiSCSISession
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSISession> iSCSISession
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSITargetToiSCSISession", "MSFT_iSCSISession", "iSCSITarget", "iSCSISession") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSITargetToiSCSISession", "MSFT_iSCSISession", "iSCSITarget", "iSCSISession")
					select new MSFT_iSCSISession(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSITargetMSFT_iSCSITargetToiSCSISession(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

