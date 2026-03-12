using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageReliabilityCounterMSFT_PhysicalDiskToStorageReliabilityCounter
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_PhysicalDisk> PhysicalDisk
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_PhysicalDiskToStorageReliabilityCounter", "MSFT_PhysicalDisk", "StorageReliabilityCounter", "PhysicalDisk") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_PhysicalDiskToStorageReliabilityCounter", "MSFT_PhysicalDisk", "StorageReliabilityCounter", "PhysicalDisk")
					select new MSFT_PhysicalDisk(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageReliabilityCounterMSFT_PhysicalDiskToStorageReliabilityCounter(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
