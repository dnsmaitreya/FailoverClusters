using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_iSCSIConnectionMSFT_iSCSIConnectionToDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_Disk> Disk
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSIConnectionToDisk", "MSFT_Disk", "iSCSIConnection", "Disk") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_iSCSIConnectionToDisk", "MSFT_Disk", "iSCSIConnection", "Disk")
					select new MSFT_Disk(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_iSCSIConnectionMSFT_iSCSIConnectionToDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
