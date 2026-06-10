using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageSubSystemToInitiatorId
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_InitiatorId> InitiatorId
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToInitiatorId", "MSFT_InitiatorId", "StorageSubSystem", "InitiatorId") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageSubSystemToInitiatorId", "MSFT_InitiatorId", "StorageSubSystem", "InitiatorId")
					select new MSFT_InitiatorId(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToInitiatorId(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

