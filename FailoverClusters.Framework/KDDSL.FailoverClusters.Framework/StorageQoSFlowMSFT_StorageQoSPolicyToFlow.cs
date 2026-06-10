using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageQoSFlowMSFT_StorageQoSPolicyToFlow
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageQoSPolicy> Policy
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageQoSPolicyToFlow", "MSFT_StorageQoSPolicy", "Flow", "Policy") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageQoSPolicyToFlow", "MSFT_StorageQoSPolicy", "Flow", "Policy")
					select new MSFT_StorageQoSPolicy(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageQoSFlowMSFT_StorageQoSPolicyToFlow(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

