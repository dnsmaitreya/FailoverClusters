using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StoragePoolMSFT_StoragePoolToResiliencySetting
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_ResiliencySetting> ResiliencySetting
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToResiliencySetting", "MSFT_ResiliencySetting", "StoragePool", "ResiliencySetting") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToResiliencySetting", "MSFT_ResiliencySetting", "StoragePool", "ResiliencySetting")
					select new MSFT_ResiliencySetting(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StoragePoolMSFT_StoragePoolToResiliencySetting(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
