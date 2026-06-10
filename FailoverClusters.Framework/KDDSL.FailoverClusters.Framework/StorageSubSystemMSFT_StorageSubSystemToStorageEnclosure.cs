using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageEnclosure
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageEnclosure> StorageEnclosure
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToStorageEnclosure", "MSFT_StorageEnclosure", "StorageSubSystem", "StorageEnclosure") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToStorageEnclosure", "MSFT_StorageEnclosure", "StorageSubSystem", "StorageEnclosure")
					select new MSFT_StorageEnclosure(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageEnclosure(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

