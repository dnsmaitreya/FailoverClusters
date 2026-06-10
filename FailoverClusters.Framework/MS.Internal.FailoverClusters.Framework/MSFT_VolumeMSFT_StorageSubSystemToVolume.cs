using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_VolumeMSFT_StorageSubSystemToVolume
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageSubSystem> StorageSubSystem
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToVolume", "MSFT_StorageSubSystem", "Volume", "StorageSubSystem") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToVolume", "MSFT_StorageSubSystem", "Volume", "StorageSubSystem")
					select new MSFT_StorageSubSystem(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_VolumeMSFT_StorageSubSystemToVolume(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

