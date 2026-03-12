using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageEnclosureMSFT_StorageNodeToStorageEnclosure
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageNode> StorageNode
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToStorageEnclosure", "MSFT_StorageNode", "StorageEnclosure", "StorageNode") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToStorageEnclosure", "MSFT_StorageNode", "StorageEnclosure", "StorageNode")
					select new MSFT_StorageNode(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageEnclosureMSFT_StorageNodeToStorageEnclosure(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
