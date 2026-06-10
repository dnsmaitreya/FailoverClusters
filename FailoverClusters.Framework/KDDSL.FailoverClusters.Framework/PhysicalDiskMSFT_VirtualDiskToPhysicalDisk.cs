using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_PhysicalDiskMSFT_VirtualDiskToPhysicalDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_VirtualDisk> VirtualDisk
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_VirtualDiskToPhysicalDisk", "MSFT_VirtualDisk", "PhysicalDisk", "VirtualDisk") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_VirtualDiskToPhysicalDisk", "MSFT_VirtualDisk", "PhysicalDisk", "VirtualDisk")
					select new MSFT_VirtualDisk(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_PhysicalDiskMSFT_VirtualDiskToPhysicalDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

