using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_MaskingSetMSFT_MaskingSetToInitiatorId
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_InitiatorId> InitiatorId
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_MaskingSetToInitiatorId", "MSFT_InitiatorId", "MaskingSet", "InitiatorId") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_MaskingSetToInitiatorId", "MSFT_InitiatorId", "MaskingSet", "InitiatorId")
					select new MSFT_InitiatorId(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_MaskingSetMSFT_MaskingSetToInitiatorId(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

