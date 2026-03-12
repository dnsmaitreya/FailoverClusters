using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageFaultDomain
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageFaultDomain> StorageFaultDomain
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStorageFaultDomain", "MSFT_StorageFaultDomain", "StorageSubSystem", "StorageFaultDomain") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStorageFaultDomain", "MSFT_StorageFaultDomain", "StorageSubSystem", "StorageFaultDomain")
					select new MSFT_StorageFaultDomain(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToStorageFaultDomain(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
