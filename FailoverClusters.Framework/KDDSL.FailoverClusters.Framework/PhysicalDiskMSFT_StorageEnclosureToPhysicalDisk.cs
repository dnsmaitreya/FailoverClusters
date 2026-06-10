using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_PhysicalDiskMSFT_StorageEnclosureToPhysicalDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageEnclosure> StorageEnclosure
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageEnclosureToPhysicalDisk", "MSFT_StorageEnclosure", "PhysicalDisk", "StorageEnclosure") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageEnclosureToPhysicalDisk", "MSFT_StorageEnclosure", "PhysicalDisk", "StorageEnclosure")
					select new MSFT_StorageEnclosure(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_PhysicalDiskMSFT_StorageEnclosureToPhysicalDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

