using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageObjectMSFT_StorageJobToAffectedStorageObject
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageJob> StorageJob
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageJobToAffectedStorageObject", "MSFT_StorageJob", "AffectedStorageObject", "StorageJob") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StorageJobToAffectedStorageObject", "MSFT_StorageJob", "AffectedStorageObject", "StorageJob")
					select new MSFT_StorageJob(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageObjectMSFT_StorageJobToAffectedStorageObject(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

