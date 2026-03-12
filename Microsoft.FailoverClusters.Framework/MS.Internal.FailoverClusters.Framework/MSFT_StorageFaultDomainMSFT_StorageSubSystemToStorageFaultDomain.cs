using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageFaultDomainMSFT_StorageSubSystemToStorageFaultDomain
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageSubSystem> StorageSubSystem
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStorageFaultDomain", "MSFT_StorageSubSystem", "StorageFaultDomain", "StorageSubSystem") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToStorageFaultDomain", "MSFT_StorageSubSystem", "StorageFaultDomain", "StorageSubSystem")
					select new MSFT_StorageSubSystem(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageFaultDomainMSFT_StorageSubSystemToStorageFaultDomain(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
