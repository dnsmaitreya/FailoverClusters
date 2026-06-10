using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_DiskMSFT_StorageNodeToDisk
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageNode> StorageNode
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageNodeToDisk", "MSFT_StorageNode", "Disk", "StorageNode") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageNodeToDisk", "MSFT_StorageNode", "Disk", "StorageNode")
					select new MSFT_StorageNode(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_DiskMSFT_StorageNodeToDisk(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

