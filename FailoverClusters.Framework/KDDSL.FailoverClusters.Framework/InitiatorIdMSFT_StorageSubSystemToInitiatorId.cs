using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_InitiatorIdMSFT_StorageSubSystemToInitiatorId
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageSubSystem> StorageSubSystem
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToInitiatorId", "MSFT_StorageSubSystem", "InitiatorId", "StorageSubSystem") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToInitiatorId", "MSFT_StorageSubSystem", "InitiatorId", "StorageSubSystem")
					select new MSFT_StorageSubSystem(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_InitiatorIdMSFT_StorageSubSystemToInitiatorId(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

