using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageFaultDomainMSFT_StorageFaultDomainToStorageFaultDomain
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageFaultDomain> TargetStorageFaultDomain
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageFaultDomainToStorageFaultDomain", "MSFT_StorageFaultDomain", "SourceStorageFaultDomain", "TargetStorageFaultDomain") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageFaultDomainToStorageFaultDomain", "MSFT_StorageFaultDomain", "SourceStorageFaultDomain", "TargetStorageFaultDomain")
					select new MSFT_StorageFaultDomain(Session, i)).ToArray();
			}
			return null;
		}
	}

	public IEnumerable<MSFT_StorageFaultDomain> SourceStorageFaultDomain
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageFaultDomainToStorageFaultDomain", "MSFT_StorageFaultDomain", "TargetStorageFaultDomain", "SourceStorageFaultDomain") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageFaultDomainToStorageFaultDomain", "MSFT_StorageFaultDomain", "TargetStorageFaultDomain", "SourceStorageFaultDomain")
					select new MSFT_StorageFaultDomain(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageFaultDomainMSFT_StorageFaultDomainToStorageFaultDomain(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

