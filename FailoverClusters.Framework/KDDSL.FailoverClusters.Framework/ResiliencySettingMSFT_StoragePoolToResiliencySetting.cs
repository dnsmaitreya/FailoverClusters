using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Management.Infrastructure;

namespace KDDSL.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_ResiliencySettingMSFT_StoragePoolToResiliencySetting
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_StoragePool> StoragePool
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StoragePoolToResiliencySetting", "MSFT_StoragePool", "ResiliencySetting", "StoragePool") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/windows/storage", Instance, "MSFT_StoragePoolToResiliencySetting", "MSFT_StoragePool", "ResiliencySetting", "StoragePool")
					select new MSFT_StoragePool(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_ResiliencySettingMSFT_StoragePoolToResiliencySetting(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}

