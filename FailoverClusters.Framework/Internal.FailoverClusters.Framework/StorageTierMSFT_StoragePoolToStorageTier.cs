using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageTierMSFT_StoragePoolToStorageTier
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StoragePool> StoragePool
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToStorageTier", "MSFT_StoragePool", "StorageTier", "StoragePool") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToStorageTier", "MSFT_StoragePool", "StorageTier", "StoragePool")
					select new MSFT_StoragePool(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageTierMSFT_StoragePoolToStorageTier(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

