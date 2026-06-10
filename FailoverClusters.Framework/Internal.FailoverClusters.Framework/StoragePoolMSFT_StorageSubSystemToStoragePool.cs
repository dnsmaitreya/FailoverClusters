using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StoragePoolMSFT_StorageSubSystemToStoragePool
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageSubSystem> StorageSubSystem
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStoragePool", "MSFT_StorageSubSystem", "StoragePool", "StorageSubSystem") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStoragePool", "MSFT_StorageSubSystem", "StoragePool", "StorageSubSystem")
					select new MSFT_StorageSubSystem(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StoragePoolMSFT_StorageSubSystemToStoragePool(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

