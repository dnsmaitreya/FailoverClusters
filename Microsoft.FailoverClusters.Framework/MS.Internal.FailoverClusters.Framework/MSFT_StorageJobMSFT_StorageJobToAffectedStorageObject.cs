using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageJobMSFT_StorageJobToAffectedStorageObject
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StorageObject> AffectedStorageObject
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageJobToAffectedStorageObject", "MSFT_StorageObject", "StorageJob", "AffectedStorageObject") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageJobToAffectedStorageObject", "MSFT_StorageObject", "StorageJob", "AffectedStorageObject")
					select new MSFT_StorageObject(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageJobMSFT_StorageJobToAffectedStorageObject(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
