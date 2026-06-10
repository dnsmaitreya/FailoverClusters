using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StoragePoolMSFT_StoragePoolToPhysicalDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_PhysicalDisk> PhysicalDisk
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StoragePoolToPhysicalDisk", "MSFT_PhysicalDisk", "StoragePool", "PhysicalDisk") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StoragePoolToPhysicalDisk", "MSFT_PhysicalDisk", "StoragePool", "PhysicalDisk")
					select new MSFT_PhysicalDisk(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StoragePoolMSFT_StoragePoolToPhysicalDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

