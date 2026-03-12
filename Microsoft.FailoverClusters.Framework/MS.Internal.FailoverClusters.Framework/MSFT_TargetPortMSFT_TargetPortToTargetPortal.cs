using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_TargetPortMSFT_TargetPortToTargetPortal
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_TargetPortal> TargetPortal
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_TargetPortToTargetPortal", "MSFT_TargetPortal", "TargetPort", "TargetPortal") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_TargetPortToTargetPortal", "MSFT_TargetPortal", "TargetPort", "TargetPortal")
					select new MSFT_TargetPortal(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_TargetPortMSFT_TargetPortToTargetPortal(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
