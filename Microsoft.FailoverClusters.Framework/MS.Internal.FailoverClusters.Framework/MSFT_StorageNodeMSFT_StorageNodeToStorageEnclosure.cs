using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageNodeMSFT_StorageNodeToStorageEnclosure
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageEnclosure> StorageEnclosure
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToStorageEnclosure", "MSFT_StorageEnclosure", "StorageNode", "StorageEnclosure") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageNodeToStorageEnclosure", "MSFT_StorageEnclosure", "StorageNode", "StorageEnclosure")
					select new MSFT_StorageEnclosure(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageNodeMSFT_StorageNodeToStorageEnclosure(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
