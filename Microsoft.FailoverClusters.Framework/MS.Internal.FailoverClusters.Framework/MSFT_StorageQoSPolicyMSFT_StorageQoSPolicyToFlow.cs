using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToFlow
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageQoSFlow> Flow
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageQoSPolicyToFlow", "MSFT_StorageQoSFlow", "Policy", "Flow") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageQoSPolicyToFlow", "MSFT_StorageQoSFlow", "Policy", "Flow")
					select new MSFT_StorageQoSFlow(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToFlow(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
