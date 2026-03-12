using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_PhysicalDiskMSFT_StorageNodeToPhysicalDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageNode> StorageNode
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToPhysicalDisk", "MSFT_StorageNode", "PhysicalDisk", "StorageNode") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToPhysicalDisk", "MSFT_StorageNode", "PhysicalDisk", "StorageNode")
					select new MSFT_StorageNode(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_PhysicalDiskMSFT_StorageNodeToPhysicalDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
