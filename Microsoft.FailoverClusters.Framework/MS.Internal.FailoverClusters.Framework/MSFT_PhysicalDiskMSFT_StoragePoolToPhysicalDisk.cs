using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_PhysicalDiskMSFT_StoragePoolToPhysicalDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StoragePool> StoragePool
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToPhysicalDisk", "MSFT_StoragePool", "PhysicalDisk", "StoragePool") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToPhysicalDisk", "MSFT_StoragePool", "PhysicalDisk", "StoragePool")
					select new MSFT_StoragePool(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_PhysicalDiskMSFT_StoragePoolToPhysicalDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
