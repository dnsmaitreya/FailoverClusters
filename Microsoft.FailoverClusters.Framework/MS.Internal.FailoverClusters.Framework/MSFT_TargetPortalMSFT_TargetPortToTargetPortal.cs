using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_TargetPortalMSFT_TargetPortToTargetPortal
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_TargetPort> TargetPort
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_TargetPortToTargetPortal", "MSFT_TargetPort", "TargetPortal", "TargetPort") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_TargetPortToTargetPortal", "MSFT_TargetPort", "TargetPortal", "TargetPort")
					select new MSFT_TargetPort(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_TargetPortalMSFT_TargetPortToTargetPortal(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
