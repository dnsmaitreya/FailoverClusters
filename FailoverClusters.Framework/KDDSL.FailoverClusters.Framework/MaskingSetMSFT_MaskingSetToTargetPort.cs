using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_MaskingSetMSFT_MaskingSetToTargetPort
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_TargetPort> TargetPort
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_MaskingSetToTargetPort", "MSFT_TargetPort", "MaskingSet", "TargetPort") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_MaskingSetToTargetPort", "MSFT_TargetPort", "MaskingSet", "TargetPort")
					select new MSFT_TargetPort(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_MaskingSetMSFT_MaskingSetToTargetPort(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

