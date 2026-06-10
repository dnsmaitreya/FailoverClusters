using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_DiskMSFT_iSCSISessionToDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_iSCSISession> iSCSISession
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_iSCSISessionToDisk", "MSFT_iSCSISession", "Disk", "iSCSISession") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_iSCSISessionToDisk", "MSFT_iSCSISession", "Disk", "iSCSISession")
					select new MSFT_iSCSISession(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_DiskMSFT_iSCSISessionToDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

