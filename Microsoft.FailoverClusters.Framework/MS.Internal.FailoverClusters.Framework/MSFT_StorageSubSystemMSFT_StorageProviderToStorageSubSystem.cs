using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageProviderToStorageSubSystem
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageProvider> StorageProvider
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageProviderToStorageSubSystem", "MSFT_StorageProvider", "StorageSubSystem", "StorageProvider") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageProviderToStorageSubSystem", "MSFT_StorageProvider", "StorageSubSystem", "StorageProvider")
					select new MSFT_StorageProvider(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageProviderToStorageSubSystem(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
