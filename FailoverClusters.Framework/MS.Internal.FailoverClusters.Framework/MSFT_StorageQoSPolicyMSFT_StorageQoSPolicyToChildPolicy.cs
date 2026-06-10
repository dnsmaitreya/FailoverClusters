using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToChildPolicy
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageQoSPolicy> ParentPolicy
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageQoSPolicyToChildPolicy", "MSFT_StorageQoSPolicy", "ChildPolicy", "ParentPolicy") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageQoSPolicyToChildPolicy", "MSFT_StorageQoSPolicy", "ChildPolicy", "ParentPolicy")
					select new MSFT_StorageQoSPolicy(Session, i)).ToArray();
			}
			return null;
		}
	}

	public IEnumerable<MSFT_StorageQoSPolicy> ChildPolicy
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageQoSPolicyToChildPolicy", "MSFT_StorageQoSPolicy", "ParentPolicy", "ChildPolicy") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageQoSPolicyToChildPolicy", "MSFT_StorageQoSPolicy", "ParentPolicy", "ChildPolicy")
					select new MSFT_StorageQoSPolicy(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageQoSPolicyMSFT_StorageQoSPolicyToChildPolicy(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

