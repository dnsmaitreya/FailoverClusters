using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageNodeMSFT_StorageSubSystemToStorageNode
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageSubSystem> StorageSubSystem
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStorageNode", "MSFT_StorageSubSystem", "StorageNode", "StorageSubSystem") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStorageNode", "MSFT_StorageSubSystem", "StorageNode", "StorageSubSystem")
					select new MSFT_StorageSubSystem(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageNodeMSFT_StorageSubSystemToStorageNode(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

