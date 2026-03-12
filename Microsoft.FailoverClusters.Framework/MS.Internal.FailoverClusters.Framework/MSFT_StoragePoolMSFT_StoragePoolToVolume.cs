using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;

namespace MS.Internal.FailoverClusters.Framework;

[GeneratedCode("migen.exe", "1.0.0")]
public class MSFT_StoragePoolMSFT_StoragePoolToVolume
{
	public CimSession Session { get; set; }

	public CimInstance Instance { get; set; }

	public IEnumerable<MSFT_Volume> Volume
	{
		get
		{
			if (Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToVolume", "MSFT_Volume", "StoragePool", "Volume") != null)
			{
				return (from i in Session.EnumerateAssociatedInstances("root/microsoft/windows/storage", Instance, "MSFT_StoragePoolToVolume", "MSFT_Volume", "StoragePool", "Volume")
					select new MSFT_Volume(Session, i)).ToArray();
			}
			return null;
		}
	}

	public MSFT_StoragePoolMSFT_StoragePoolToVolume(CimSession session, CimInstance instance)
	{
		Session = session;
		Instance = instance;
	}
}
