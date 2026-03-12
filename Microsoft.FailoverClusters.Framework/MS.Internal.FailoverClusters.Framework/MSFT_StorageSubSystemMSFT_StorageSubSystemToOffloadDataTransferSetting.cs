using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StorageSubSystemMSFT_StorageSubSystemToOffloadDataTransferSetting
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_OffloadDataTransferSetting> OffloadDataTransferSetting
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToOffloadDataTransferSetting", "MSFT_OffloadDataTransferSetting", "StorageSubSystem", "OffloadDataTransferSetting") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StorageSubSystemToOffloadDataTransferSetting", "MSFT_OffloadDataTransferSetting", "StorageSubSystem", "OffloadDataTransferSetting")
					select new MSFT_OffloadDataTransferSetting(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StorageSubSystemMSFT_StorageSubSystemToOffloadDataTransferSetting(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
